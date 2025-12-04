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
        
        public List<GameObject> selectedGameObjects = new(); // Selected objects

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
            if (Camera.main != null)
            {
                MainCamera = Camera.main;
            }
        }

        private void Update()
        {
            // OnMouseDown
            // - Save mouse position
            // - Set state
            // - Timer WAIT FOR FEW SECONDS TO ENSURE IT'S NOT JUST SINGLE SHORT CLICK
            //
            // OnMouseUp
            // - Save mouse position2
            // - Reset state
            // - Find rectangle/bound box
            // - Find all objects that are intersecting this bound box
            // - Put em all in selection pool
            // - On each fixture OnObjectSelected
            
            if (Input.GetMouseButton(0))
            {
                if (state == 0)
                {
                    timer += Time.deltaTime;
                    if (timer >= timeToMultiSelect) // Wait before action to not mistake for single click
                    {
                        state = 1;
                        timer = 0;
                        cameraPositionAtMultiSelectStart = MainCamera.transform.position;
                    }
                }

                if (state == 1) // Update start position once
                {
                    startWorldPosition = mouseOnTransform;
                    state = 2;
                }

                if (state == 2) // Update end position
                {
                    endWorldPosition = mouseOnTransform;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (state != 0)
                {
                    endWorldPosition = mouseOnTransform;
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
            screenRay = MainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(screenRay, out hitPoint, MainCamera.farClipPlane))
            {
                // Nothing was hit.
                return;
            }
            
            var targetGameObject = hitPoint.transform?.gameObject;
            if (targetGameObject == null) return;
            
            Debug.Log(hitPoint.transform.gameObject.name);

            var parentOfTarget = targetGameObject.transform.parent;
            if (parentOfTarget != null && parentOfTarget.TryGetComponent<Selectable>(out var selectable))
            {
                // TODO: User experience below
                // If user holds control, do not reset selection;
                // If user holds A, select all objects of that type.
                // If user holds control and object is already selected, deselect it.
                
                bool shiftKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                bool aKey = Input.GetKey(KeyCode.A);
                
                if (parentOfTarget.TryGetComponent<MobileTruss>(out var mobileTruss))
                {
                    if (!shiftKey && !aKey)
                    {
                        selectedGameObjects.Clear();
                        selectedMobileTrusses.Clear();
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedMobileTrusses");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedMobileTrusses.Add(mobileTruss);
                    }
                    else if (shiftKey && !aKey)
                    {
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedMobileTrusses");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedMobileTrusses.Add(mobileTruss);
                    }
                }
                else if (parentOfTarget.TryGetComponent<MobileLight>(out var mobileLight))
                {
                    if (!shiftKey && !aKey)
                    {
                        selectedGameObjects.Clear();
                        selectedMobileLights.Clear();
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedMobileLights");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedMobileLights.Add(mobileLight);
                    }
                    else if (shiftKey && !aKey)
                    {
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedMobileLights");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedMobileLights.Add(mobileLight);
                    }
                }
                else if (parentOfTarget.TryGetComponent<LightingDrone>(out var lightingDrone))
                {
                    if (!shiftKey && !aKey)
                    {
                        selectedGameObjects.Clear();
                        selectedLightingDrones.Clear();
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedLightingDrones");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedLightingDrones.Add(lightingDrone);
                    }
                    else if (shiftKey && !aKey)
                    {
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedLightingDrones");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedLightingDrones.Add(lightingDrone);
                    }
                }
                else if (parentOfTarget.TryGetComponent<PyroDrone>(out var pyroDrone))
                {
                    if (!shiftKey && !aKey)
                    {
                        selectedGameObjects.Clear();
                        selectedPyroDrones.Clear();
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedPyroDrones");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedPyroDrones.Add(pyroDrone);
                    }
                    else if (shiftKey && !aKey)
                    {
                        Debug.Log($"Added '{parentOfTarget.gameObject.name}' to selectedPyroDrones");
                        selectedGameObjects.Add(parentOfTarget.gameObject);
                        selectedPyroDrones.Add(pyroDrone);
                    }
                }
                else
                {
                    // No supported component found.
                }
            }
        }
        
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

        private void OnDrawGizmos()
        {
            Debug.DrawLine(startWorldPosition, endWorldPosition, Color.aquamarine);
            
            Debug.DrawLine(cameraPositionAtMultiSelectStart, startWorldPosition, Color.yellow);
            Debug.DrawLine(cameraPositionAtMultiSelectStart, endWorldPosition, Color.yellow);

            Debug.DrawLine(screenRay.origin, screenRay.direction, Color.blue);
        }
    }
}