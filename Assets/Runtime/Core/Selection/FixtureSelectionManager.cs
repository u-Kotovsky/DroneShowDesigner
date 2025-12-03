using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core.Selection
{
    public class FixtureSelectionManager : MonoBehaviour
    {
        public Vector3 startWorldPosition;
        public Vector3 endWorldPosition;

        public float timer;
        public int state;
        
        public List<GameObject> gameObjects;
        
        private GameObject startObject;
        private GameObject endObject;

        public float timeToMultiSelect = 0.15f;

        private Camera _mainCamera;
        private bool _hasMainCamera;
        private Camera MainCamera
        {
            get
            {
                return _mainCamera;
            }
            set
            {
                _mainCamera = value;
                _hasMainCamera = value != null;
            }
        }
        
        private void Start()
        {
            startObject = new GameObject("Start");
            endObject = new GameObject("End");

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
                        cameraPositionAtMultiSelectStart = Camera.main.transform.position;
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
                    SetWorldPositions();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (state != 0)
                {
                    endWorldPosition = mouseOnTransform;
                    SetWorldPositions();
                }
                else
                {
                    // Single object select
                    directionPoint = mouseOnTransform;
                    startPoint = Camera.main.transform.position;

                    var fakeDirection = Vector3.Lerp(startPoint, directionPoint, 0.025f);
                    var ray = new Ray(startPoint, fakeDirection);

                    if (Physics.Raycast(ray, out hitPoint, 1000, Physics.AllLayers))
                    {
                        Debug.Log(hitPoint.transform.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Hit void!");
                    }
                }
                
                // TODO: calculate rect
                
                // Reset timer
                timer = 0;
                state = 0;
            }
        }

        private Vector3 directionPoint;
        private Vector3 startPoint;
        private RaycastHit hitPoint;
        
        private Vector3 cameraPositionAtMultiSelectStart;
        
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

        private void SetWorldPositions()
        {
            startObject.transform.position = startWorldPosition;
            endObject.transform.position = endWorldPosition;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(startWorldPosition, endWorldPosition, Color.orange);
            
            Debug.DrawLine(cameraPositionAtMultiSelectStart, startWorldPosition, Color.yellow);
            Debug.DrawLine(cameraPositionAtMultiSelectStart, endWorldPosition, Color.yellow);

            if (directionPoint != null && startPoint != null)
            {
                Debug.DrawLine(startPoint, directionPoint, Color.blue);
                Debug.DrawLine(directionPoint, directionPoint + Vector3.up, Color.blue);
                //Debug.DrawLine(startPoint, hitPoint.point, Color.orange);
                //Debug.DrawLine(startPoint, Vector3.LerpUnclamped(startPoint, directionPoint, Camera.main.farClipPlane), Color.magenta);
            }
        }
    }
}
