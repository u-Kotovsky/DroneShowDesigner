using System;
using System.Collections.Generic;
using Runtime.Dmx.Fixtures.Drones;
using Runtime.Dmx.Fixtures.Lights;
using Runtime.Dmx.Fixtures.Truss;
using Runtime.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.Core.Selection
{
    public class FixtureSelectionManager : MonoBehaviour
    {
        private Vector3 startWorldPosition;
        private Vector3 endWorldPosition;
        private Vector2 startScreenPosition;
        private Vector2 endScreenPosition;
        
        public GraphicRaycaster graphicRaycaster;
        public EventSystem eventSystem;

        public float timer;
        public int state;

        private List<SelectionEntry> selection = new();
        public List<SelectionEntry> Selection => selection;
        
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
        
        public static FixtureSelectionManager Instance { get; private set; }
        
        public event Action<List<SelectionEntry>> OnSelectionChanged = (selection) => { };
        public event Action<SelectionEntry> OnObjectSelected = (selection) => { };
        public event Action<SelectionEntry> OnObjectDeselected = (selection) => { };

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one instance of FixtureSelectionManager found.");
                return;
            }
            
            Instance = this;
        }

        public static void InsertInstanceIfNotExist()
        {
            if (Instance != null)
            {
                return;
            }
            
            var obj = new GameObject("FixtureSelectionManager (Instance)");
            var component = obj.AddComponent<FixtureSelectionManager>();
            var graphicRaycaster = FindFirstObjectByType<GraphicRaycaster>();
            var eventSystem = FindFirstObjectByType<EventSystem>();
            component.eventSystem = eventSystem;
            component.graphicRaycaster = graphicRaycaster;
            Instance = component;
        }
        
        private void Start()
        {
            if (Camera.main != null) MainCamera = Camera.main;
        }

        private bool _doLockSelection = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) _doLockSelection = !_doLockSelection;
            if (_doLockSelection) return;
            
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
                    startScreenPosition = Input.mousePosition;
                    startWorldPosition = MouseOnTransform;
                    state = 2;
                    EffectUI.SetBoxSelectionActive(true);
                }
                
                // Update end position
                if (state == 2) 
                {
                    endScreenPosition = Input.mousePosition;
                    endWorldPosition = MouseOnTransform;
                    EffectUI.SetBoxSelectionPosition(startScreenPosition, endScreenPosition);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (state != 0)
                {
                    endScreenPosition = Input.mousePosition;
                    endWorldPosition = MouseOnTransform;
                    // TODO: calculate cone-like rectangle to check what is selected and what is not
                    // or just a rectangle on screen and check what does hit this rectangle when raycasting(?)
                    EffectUI.SetBoxSelectionPosition(startScreenPosition, endScreenPosition);
                    EffectUI.SetBoxSelectionActive(false);
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

        private bool DidWeHitAnythingOnUI()
        {
            // Check if we hit anything on UI
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            // List to save found elements
            var raycastResultList = new List<RaycastResult>();
            
            // Find elements at pointerEventData position, then save to raycastList
            graphicRaycaster.Raycast(pointerEventData, raycastResultList);
            
            // Get rect transforms
            var raycastResult = raycastResultList.Find(element => element.gameObject.GetComponent<RectTransform>());
            if (raycastResult.gameObject != null) // Ignore action
            {
                return true;
            }

            return false;
        }
        
        private void SelectSingleObject()
        {
            if (!hasMainCamera)
            {
                return;
            }
            
            if (DidWeHitAnythingOnUI())
            {
                return;
            }
            
            screenRay = MainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(screenRay, out hitPoint, MainCamera.farClipPlane)) // No hit = ClearSelection
            {
                bool controlKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                bool shiftKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if (controlKey || shiftKey) return; // Do not reset selection when we holding shift or control keys
                ClearAllSelection();
                return;
            }
            
            var targetGameObject = hitPoint.transform?.gameObject;
            if (targetGameObject == null) // No Hit = ClearSelection
            {
                ClearAllSelection();
                return;
            }

            var parentOfTarget = targetGameObject.transform.parent;
            if (parentOfTarget == null) // No Parent = ClearSelection
            {
                ClearAllSelection();
                return;
            }

            if (!parentOfTarget.TryGetComponent<Selectable>(out var selectable)) // No Selectable = ClearSelection
            {
                ClearAllSelection();
                return;
            }

            if (parentOfTarget.TryGetComponent<MobileTruss>(out var mobileTruss))
            {
                OnObjectHit(ref selection, ref selectable, ref mobileTruss);
                return;
            }
            
            if (parentOfTarget.TryGetComponent<MobileLight>(out var mobileLight))
            {
                OnObjectHit(ref selection, ref selectable, ref mobileLight);
                return;
            }
            
            if (parentOfTarget.TryGetComponent<LightingDrone>(out var lightingDrone))
            {
                OnObjectHit(ref selection, ref selectable, ref lightingDrone);
                return;
            }
            
            if (parentOfTarget.TryGetComponent<PyroDrone>(out var pyroDrone))
            {
                OnObjectHit(ref selection, ref selectable, ref pyroDrone);
                return;
            }
            
            Debug.LogWarning("No supported component found.");
            ClearAllSelection();
        }

        private void OnObjectHit<T>(ref List<SelectionEntry> selection, ref Selectable selectable, ref T component) where T : Component
        {
            bool controlKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool aKey = Input.GetKey(KeyCode.A);
            
            // TODO: When selecting already selected object with control => deselect that object, or based on keybinds it's cases.
            
            // Clear all and select new
            if (!controlKey && !aKey)
            {
                if (selection.Count > 0) ClearSelection(ref selection, true);
                AddSelection(ref selection, new SelectionEntry(component.gameObject, component.GetType()), true);
                OnSelectionChanged?.Invoke(selection);
                return;
            }
            
            // Do not reset, add new
            if (controlKey && !aKey)
            {
                AddSelection(ref selection, new SelectionEntry(component.gameObject, component.GetType()), true);
                OnSelectionChanged?.Invoke(selection);
                return;
            }
            
            // Select all objects of current type (Additive)
            if (controlKey && aKey)
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
                        AddSelection(ref selection, new SelectionEntry(component1.gameObject, component.GetType()), true);
                    }
                }

                OnSelectionChanged?.Invoke(selection);
                return;
            }
            
            // Select all objects of current type (Replace)
            if (!controlKey && aKey)
            {
                if (selection.Count > 0) ClearSelection(ref selection);
                
                var components = FindObjectsByType<T>(FindObjectsSortMode.None);
                foreach (var fixture in components)
                {
                    AddSelection(ref selection, new SelectionEntry(fixture.gameObject, component.GetType()), true);
                }
                
                OnSelectionChanged?.Invoke(selection);

                return;
            }
        }

        private void ClearAllSelection(bool doNotCallSelectionChanged = false)
        {
            if (selection.Count > 0) ClearSelection(ref selection, doNotCallSelectionChanged);
        }

        private void AddSelection(ref List<SelectionEntry> selection, SelectionEntry newSelection, bool doNotCallSelectionChanged = false)
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
            
            OnObjectSelected?.Invoke(newSelection);
            if (!doNotCallSelectionChanged) OnSelectionChanged?.Invoke(selection);
        }

        private void ClearSelection(ref List<SelectionEntry> currentSelection, bool doNotCallSelectionChanged = false)
        {
            lock (currentSelection)
            {
                foreach (var selectionEntry in currentSelection)
                {
                    if (selectionEntry.GameObject.TryGetComponent<Selectable>(out var selectable))
                    {
                        selectable.OnObjectDeselected();
                        OnObjectDeselected?.Invoke(selectionEntry);
                    }
                }

                currentSelection.Clear();
            }
            
            if (!doNotCallSelectionChanged) OnSelectionChanged?.Invoke(currentSelection);
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