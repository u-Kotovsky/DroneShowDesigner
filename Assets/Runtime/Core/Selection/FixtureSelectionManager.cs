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

        private List<SelectionEntry> selection = new();
        
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
                    startWorldPosition = MouseOnTransform;
                    state = 2;
                }
                
                // Update end position
                if (state == 2) 
                {
                    endWorldPosition = MouseOnTransform;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (state != 0)
                {
                    endWorldPosition = MouseOnTransform;
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
                OnObjectHit(ref selection, ref selectable, ref mobileTruss);
            }
            else if (parentOfTarget.TryGetComponent<MobileLight>(out var mobileLight))
            {
                OnObjectHit(ref selection, ref selectable, ref mobileLight);
            }
            else if (parentOfTarget.TryGetComponent<LightingDrone>(out var lightingDrone))
            {
                OnObjectHit(ref selection, ref selectable, ref lightingDrone);
            }
            else if (parentOfTarget.TryGetComponent<PyroDrone>(out var pyroDrone))
            {
                OnObjectHit(ref selection, ref selectable, ref pyroDrone);
            }
            else
            {
                // No supported component found.

                ClearAllSelection();
            }
        }

        private static void OnObjectHit<T>(ref List<SelectionEntry> selection, ref Selectable selectable, ref T component) where T : Component
        {
            bool controlKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool aKey = Input.GetKey(KeyCode.A);
            
            // Clear all and select new
            if (!controlKey && !aKey)
            {
                ClearSelection(ref selection);
                AddSelection(ref selection, new SelectionEntry(component.gameObject));
            }
            // Do not reset, add new
            else if (controlKey && !aKey)
            {
                AddSelection(ref selection, new SelectionEntry(component.gameObject));
            }
            // Select all objects of current type (Additive)
            else if (controlKey && aKey)
            {
                var components = FindObjectsByType<T>(FindObjectsSortMode.None);
                
                foreach (var component1 in components)
                {
                    var obj = component1.gameObject;
                    var flag1 = false;

                    foreach (var selection2 in selection)
                    {
                        if (obj.gameObject.GetInstanceID() == selection2.GameObject.GetInstanceID())
                        {
                            flag1 = true;
                        }
                    }

                    if (!flag1)
                    {
                        AddSelection(ref selection, new SelectionEntry(component1.gameObject));
                    }
                }
            }
            // Select all objects of current type (Replace)
            else if (!controlKey && aKey)
            {
                ClearSelection(ref selection);
                
                var components = FindObjectsByType<T>(FindObjectsSortMode.None);
                foreach (var fixture in components)
                {
                    AddSelection(ref selection, new SelectionEntry(fixture.gameObject));
                }
            }
        }

        private void ClearAllSelection()
        {
            ClearSelection(ref selection);
        }

        private static void AddSelection(ref List<SelectionEntry> selection, SelectionEntry newSelection)
        {
            lock (selection)
            {
                var flag1 = false;
                
                foreach (var selection2 in selection)
                {
                    if (selection2.GameObject.GetInstanceID() == newSelection.GameObject.GetInstanceID())
                    {
                        flag1 = true;
                    }
                }
                
                if (!flag1)
                {
                    selection.Add(newSelection);
                    if (newSelection.GameObject.TryGetComponent<Selectable>(out var selectable2))
                    {
                        selectable2.OnObjectSelected();
                    }
                }
            }
        }

        private static void ClearSelection(ref List<SelectionEntry> selection)
        {
            lock (selection)
            {
                foreach (var obj in selection)
                {
                    if (obj.GameObject.TryGetComponent<Selectable>(out var selectable))
                    {
                        selectable.OnObjectDeselected();
                    }
                }

                selection.Clear();
            }
        }

        #region Cursor in world space
        private static Vector3 MouseWorldPosOnNearClipPlane =>
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        private static Vector3 CameraToMouse => MouseWorldPosOnNearClipPlane - Camera.main.transform.position;

        private Vector3 MouseOnTransform {
            get {
                Vector3 camToTransform = transform.position - Camera.main.transform.position;
                return Camera.main.transform.position + CameraToMouse * 
                       (Vector3.Dot(Camera.main.transform.forward, camToTransform) / Vector3.Dot(Camera.main.transform.forward, CameraToMouse));
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