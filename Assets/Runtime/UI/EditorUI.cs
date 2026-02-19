using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Runtime.Core.Selection;
using Runtime.Dmx.Fixtures.Drones;
using Runtime.Dmx.Fixtures.Lights;
using Runtime.Dmx.Fixtures.Truss;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.UI
{
    public class EditorUI : MonoBehaviour
    {
        private static RectTransform _root;
        private static RectTransform _hierarchy;
        private static RectTransform _inspector;
        
        public static EditorUI Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Only one instance of EditorUI is allowed.");
                enabled = false;
                return;
            }
            
            Instance = this;
        }

        private static void InsertInstanceIfNotExist()
        {
            if (Instance != null)
            {
                return;
            }
            
            var obj = new GameObject("EditorUI (Instance)");
            var component = obj.AddComponent<EditorUI>();
            Instance = component;
        }
        
        public static void BuildUI(RectTransform parent)
        {
            InsertInstanceIfNotExist();
            FixtureSelectionManager.InsertInstanceIfNotExist();
            
            _root = UIUtility.AddRect(parent, "EditorUI")
                //.WithHorizontalLayout()
                //.ControlChildSize(true, false)
                //.ForceExpand(true, false)
                .SetAllStretch(Vector4.zero);
                //.GetRect();
            
            
            // Todo: scrollview
            _hierarchy = UIUtility.AddRect(_root, "Hierarchy")
                .WithVerticalLayout()
                .GetRect()
                .WithImage(Color.white * 0.2f);
            
            _hierarchy.pivot = new Vector2(0, 1);
            _hierarchy.anchorMin = new Vector2(0, 0);
            _hierarchy.anchorMax = new Vector2(0, 1);
            _hierarchy.sizeDelta = new Vector2(220, 0);
            
            _inspector = UIUtility.AddRect(_root, "Inspector")
                .WithVerticalLayout()
                .ControlChildSize(true, false)
                .ForceExpand(true, false)
                .GetRect()
                .WithImage(Color.white * 0.2f);

            _inspector.pivot = Vector2.one;
            _inspector.anchorMin = new Vector2(1, 0);
            _inspector.anchorMax = new Vector2(1, 1);
            _inspector.sizeDelta = new Vector2(220, 0);

            // TODO: hook to OnobjectSelected
            
            // Null
            try
            {
                if (FixtureSelectionManager.Instance == null)
                {
                    throw new Exception("FixtureSelectionManager.Instance is null.");
                }

                if (Instance == null)
                {
                    throw new Exception("EditorUI.Instance is null.");
                }
                
                FixtureSelectionManager.Instance.OnSelectionChanged += Instance.OnSelectionChanged;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void ClearInspector()
        {
            if (_inspector.transform.childCount < 1) return;
            if (_inspector == null) return;
            foreach (Transform child in _inspector.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void DrawInspector()
        {
            var selection = FixtureSelectionManager.Instance.Selection;

            bool hasMultipleComponentsOfOneType = false;
            bool hasMultipleComponentsOfMultipleTypes = false;

            for (var i = 0; i < selection.Count; i++)
            {
                var selection1 = selection[i];

                for (var j = 0; j < selection.Count; j++)
                {
                    var selection2 = selection[j];
                    if (selection1.Type == selection2.Type && i != j) // If types are the same when index doesnt match (different objects, same types)
                    {
                        hasMultipleComponentsOfOneType = true;
                    }
                    
                    if (selection1.Type != selection2.Type)
                    {
                        hasMultipleComponentsOfMultipleTypes = true;
                    }
                }
            }

            var errors = new StringBuilder();

            if (hasMultipleComponentsOfMultipleTypes)
            {
                // We dont handle multiple different types yet.
                errors.AppendLine("Multiple components of different types found. We don't handle this situation yet.");
            }

            if (errors.Length > 0)
            {
                UIUtility.AddText(_inspector, "Failed to load inspector:\n" + errors, Color.red);
                errors.Clear();
                return;
            }
            
            // TODO: use a separate "combined" version of handling multiple components of one type. (Basic Copy, Save, Paste, Edit?)

            // Very very WIP, unoptimized, buggy and so on.
            LoadVisualComponents(selection, hasMultipleComponentsOfOneType);
        }

        private static void LoadVisualComponents(List<SelectionEntry> selection, bool hasMultipleComponentsOfOneType)
        {
            if (_inspector == null) return;
            
            // TODO: Ensure that user clicked on UI, and not on any other collider if UI is in front to not accidentally deselect all items.

            if (hasMultipleComponentsOfOneType)
            {
                CreateMultiComponentInspectorOfOneType(selection);
            }
            else
            {
                foreach (var selectionEntry in selection)
                {
                    CreateSingleComponentInspector(selectionEntry);
                }
            }
        }

        private static void CreateMultiComponentInspectorOfOneType(List<SelectionEntry> selection)
        {
            var componentRoot = UIUtility.AddRect(_inspector, "Multi-Component")
                .WithVerticalLayout()
                //.SetAllStretch(Vector4.zero)
                .ControlChildSize(true, false)
                .ForceExpand(true, false)
                .GetRect()
                .WithImage(Color.white * 0.2f);
            
            var obj = selection[0].GameObject;

            if (obj.GetComponent<MobileTruss>())
            {
                var pool = selection.Select(x => x.GameObject.GetComponent<MobileTruss>()).ToArray();
                var poolT = selection.Select(x => x.GameObject.transform).ToArray();
                TransformInspector.OnInspector(componentRoot, poolT);
                MobileTrussInspector.OnInspector(componentRoot, pool);
                return;
            }

            if (obj.GetComponent<MobileLight>())
            {
                var pool = selection.Select(x => x.GameObject.GetComponent<MobileLight>()).ToArray();
                var poolT = selection.Select(x => x.GameObject.transform).ToArray();
                TransformInspector.OnInspector(componentRoot, poolT);
                MobileLightInspector.OnInspector(componentRoot, pool);
                return;
            }

            if (obj.GetComponent<LightingDrone>())
            {
                var pool = selection.Select(x => x.GameObject.GetComponent<LightingDrone>()).ToArray();
                var poolT = selection.Select(x => x.GameObject.transform).ToArray();
                TransformInspector.OnInspector(componentRoot, poolT);
                LightingDroneInspector.OnInspector(componentRoot, pool);
                return;
            }

            if (obj.GetComponent<PyroDrone>())
            {
                var pool = selection.Select(x => x.GameObject.GetComponent<PyroDrone>()).ToArray();
                var poolT = selection.Select(x => x.GameObject.transform).ToArray();
                TransformInspector.OnInspector(componentRoot, poolT);
                PyroDroneInspector.OnInspector(componentRoot, pool);
                return;
            }
        }

        private static void CreateSingleComponentInspector(SelectionEntry selection)
        {
            // TODO: ONLY LOAD THOSE THAT ARE IN VIEW, OTHERS SHOULD BE IGNORED OR WE'LL HAVE PERFORMANCE ISSUES
            // TODO: Put load of these components into Coroutine and make sure to turn it off when page changed so we don't do weird stuff
            var componentRoot = UIUtility.AddRect(_inspector, $"Component from '{selection.GameObject.name}'")
                .WithVerticalLayout()
                //.SetAllStretch(Vector4.zero)
                .ControlChildSize(true, false)
                .ForceExpand(true, false)
                .GetRect()
                .WithImage(Color.white * 0.2f);
                
            if (selection.GameObject.TryGetComponent<MobileTruss>(out var mobileTruss))
            {
                MobileTrussInspector.OnInspector(componentRoot, mobileTruss);
            }
            else if (selection.GameObject.TryGetComponent<MobileLight>(out var mobileLight))
            {
                MobileLightInspector.OnInspector(componentRoot, mobileLight);
            }
            else if (selection.GameObject.TryGetComponent<LightingDrone>(out var lightingDrone))
            {
                LightingDroneInspector.OnInspector(componentRoot, lightingDrone);
            }
            else if (selection.GameObject.TryGetComponent<PyroDrone>(out var pyroDrone))
            {
                PyroDroneInspector.OnInspector(componentRoot, pyroDrone);
            }
            else
            {
                UIUtility.AddText(componentRoot, "No supported components found in selection", Color.red)
                    .GetRect()
                    .WithSizeDelta(new Vector2(0, 20));
            }

            TransformInspector.OnInspector(componentRoot, selection.GameObject.transform);
        }

        private void OnSelectionChanged(List<SelectionEntry> obj)
        {
            // Redraw inspector
            ClearInspector();
            DrawInspector();
        }

        public static void DeconstructUI()
        {
            MainUIController.Instance.OnDeconstructUI -= DeconstructUI;
            FixtureSelectionManager.Instance.OnSelectionChanged -= Instance.OnSelectionChanged;
        }
    }
}
