using System.Collections.Generic;
using Runtime.Core.Resources;
using Runtime.Dmx.Fixtures.Drones;
using Runtime.Dmx.Fixtures.Lights;
using Runtime.Dmx.Fixtures.Truss;
using UnityEngine;

namespace Runtime.Core.Selection
{
    public class FixtureSelectionManager : MonoBehaviour
    {
        public Vector3 startWorldPosition;
        public Vector3 endWorldPosition;

        public float timer;
        public int state;
        
        public List<GameObject> selectedGameObjects = new();

        public List<MobileTruss> selectedMobileTrusses = new();
        public List<MobileLight> selectedMobileLights = new();
        public List<LightingDrone> selectedLightingDrones = new();
        public List<PyroDrone> selectedPyroDrones = new();
        
        public float timeToMultiSelect = 0.15f;

        private Camera mainCamera;
        private bool hasMainCamera;
        private Camera MainCamera
        {
            get => mainCamera;
            set
            {
                mainCamera = value;
                hasMainCamera = value != null;
            }
        }
        
        private Ray screenRay;
        private RaycastHit hitPoint;
        
        private Vector3 cameraPositionAtMultiSelectStart;
        
        private void Start()
        {
            if (Camera.main != null) MainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (state == 0)
                {
                    timer += Time.deltaTime;
                    // Wait before action to not mistake for single click
                    if (timer >= timeToMultiSelect) 
                    {
                        state = 1;
                        timer = 0;
                        cameraPositionAtMultiSelectStart = MainCamera.transform.position;
                    }
                }
                
                // Update start position once
                if (state == 1) 
                {
                    startWorldPosition = mouseOnTransform;
                    state = 2;
                }
                
                // Update end position
                if (state == 2) 
                {
                    endWorldPosition = mouseOnTransform;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (state != 0)
                {
                    endWorldPosition = mouseOnTransform;
                    // TODO: calculate cone-like rectangle to check what is selected and what is not
                    // or just a rectangle on screen and check what does hit this rectangle when raycasting(?)
                }
                else
                {
                    SelectSingleObject();
                }
                
                // TODO: calculate rect
                
                // Reset timer
                timer = 0;
                state = 0;
            }
        }

        private void SelectSingleObject()
        {
            if (!hasMainCamera)
            {
                ClearAllSelection();
                return;
            }
            
            screenRay = MainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(screenRay, out hitPoint, MainCamera.farClipPlane))
            {
                ClearAllSelection();
                return;
            }
            
            var targetGameObject = hitPoint.transform?.gameObject;
            if (targetGameObject == null)
            {
                ClearAllSelection();
                return;
            }

            var parentOfTarget = targetGameObject.transform.parent;
            if (parentOfTarget == null)
            {
                ClearAllSelection();
                return;
            }

            if (!parentOfTarget.TryGetComponent<Selectable>(out var selectable))
            {
                ClearAllSelection();
                return;
            }

            if (parentOfTarget.TryGetComponent<MobileTruss>(out var mobileTruss))
            {
                OnObjectHit(ref selectedGameObjects, ref selectedMobileTrusses, ref selectable, ref mobileTruss);
            }
            else if (parentOfTarget.TryGetComponent<MobileLight>(out var mobileLight))
            {
                OnObjectHit(ref selectedGameObjects, ref selectedMobileLights, ref selectable, ref mobileLight);
            }
            else if (parentOfTarget.TryGetComponent<LightingDrone>(out var lightingDrone))
            {
                OnObjectHit(ref selectedGameObjects, ref selectedLightingDrones, ref selectable, ref lightingDrone);
            }
            else if (parentOfTarget.TryGetComponent<PyroDrone>(out var pyroDrone))
            {
                OnObjectHit(ref selectedGameObjects, ref selectedPyroDrones, ref selectable, ref pyroDrone);
            }
            else
            {
                // No supported component found.

                ClearAllSelection();
            }
        }

        private void OnObjectHit<T>(ref List<GameObject> gameObjects, ref List<T> fixturesOfType, ref Selectable selectable, ref T component) where T : Component
        {
            bool controlKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool aKey = Input.GetKey(KeyCode.A);
            
            // Clear all and select new
            if (!controlKey && !aKey)
            {
                ClearSelection(ref gameObjects, ref fixturesOfType);
                
                fixturesOfType.Add(component);
                gameObjects.Add(component.gameObject);
                selectable.OnObjectSelected();
            }
            // Do not reset, add new
            else if (controlKey && !aKey)
            {
                fixturesOfType.Add(component);
                gameObjects.Add(component.gameObject);
                selectable.OnObjectSelected();
            }
            // Select all objects of current type (Additive)
            else if (controlKey && aKey)
            {
                var components = FindObjectsByType<T>(FindObjectsSortMode.None);
                for (var i = 0; i < gameObjects.Count; i++)
                {
                    var fixture = fixturesOfType[i];

                    foreach (var fixture2 in components)
                    {
                        if (fixture == fixture2) continue;
                        
                        gameObjects.Add(fixture2.gameObject);
                        fixturesOfType.Add(fixture2);
                        if (fixture2.TryGetComponent<Selectable>(out var selectable2))
                        {
                            selectable2.OnObjectSelected();
                        }
                    }
                }
            }
            // Select all objects of current type (Replace)
            else if (!controlKey && aKey)
            {
                ClearSelection(ref gameObjects, ref fixturesOfType);
                
                var components = FindObjectsByType<T>(FindObjectsSortMode.None);
                foreach (var fixture in components)
                {
                    fixturesOfType.Add(fixture);
                    gameObjects.Add(fixture.gameObject);
                    if (fixture.TryGetComponent<Selectable>(out var selectable2))
                    {
                        selectable2.OnObjectSelected();
                    }
                }
            }
        }

        private void ClearAllSelection()
        {
            ClearSelection(ref selectedGameObjects, ref selectedMobileTrusses);
            ClearSelection(ref selectedGameObjects, ref selectedMobileLights);
            ClearSelection(ref selectedGameObjects, ref selectedLightingDrones);
            ClearSelection(ref selectedGameObjects, ref selectedPyroDrones);
        }

        private void ClearSelection<T>(ref List<GameObject> gameObjects, ref List<T> selectedObjects) where T : Component
        {
            // TODO: redo indexing part as it is not right way of doing it.
            for (var i = 0; i < gameObjects.Count; i++)
            {
                var obj = gameObjects[i];
                if (obj.TryGetComponent<T>(out var fixture1))
                {
                    if (obj.TryGetComponent<Selectable>(out var selectable))
                    {
                        gameObjects.RemoveAt(i);
                        selectedObjects.Remove(fixture1);
                        selectable.OnObjectDeselected();
                    }
                }

            }
        }

        #region Cursor in world space
        private static Vector3 mouseWorldPosOnNearClipPlane =>
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        private static Vector3 cameraToMouse => mouseWorldPosOnNearClipPlane - Camera.main.transform.position;

        private Vector3 mouseOnTransform {
            get {
                Vector3 camToTransform = transform.position - Camera.main.transform.position;
                return Camera.main.transform.position + cameraToMouse * 
                       (Vector3.Dot(Camera.main.transform.forward, camToTransform) / Vector3.Dot(Camera.main.transform.forward, cameraToMouse));
            }
        }
        #endregion

        private void OnDrawGizmos()
        {
            Debug.DrawLine(startWorldPosition, endWorldPosition, Color.aquamarine);
            
            Debug.DrawLine(cameraPositionAtMultiSelectStart, startWorldPosition, Color.yellow);
            Debug.DrawLine(cameraPositionAtMultiSelectStart, endWorldPosition, Color.yellow);

            Debug.DrawLine(screenRay.origin, screenRay.direction, Color.blue);
        }
    }
}