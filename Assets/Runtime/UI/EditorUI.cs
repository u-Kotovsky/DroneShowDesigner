using System;
using System.Collections;
using System.Collections.Generic;
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
                    throw new Exception("FixtureSelectionManager.Instance is null.");
                
                if (Instance == null)
                    throw new Exception("EditorUI.Instance is null.");
                
                FixtureSelectionManager.Instance.OnSelectionChanged += Instance.OnSelectionChanged;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void ClearInspector()
        {
            foreach (Transform child in _inspector.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void DrawInspector()
        {
            var selection = FixtureSelectionManager.Instance.Selection;

            Type type = null;
            bool hasMultipleTypes = false;
            bool hasMultipleComponentsOfOneType = false;

            for (var i = 0; i < selection.Count; i++)
            {
                var selection1 = selection[i];
                if (type == null)
                {
                    type = selection1.GetType();
                    continue;
                }

                if (selection1.GetType() != type)
                {
                    hasMultipleTypes = true;
                }

                if (!hasMultipleComponentsOfOneType)
                {
                    for (var j = 0; j < selection.Count; j++)
                    {
                        var selection2 = selection[j];
                        if (selection1.GetType() == selection2.GetType() && i != j)
                        {
                            hasMultipleComponentsOfOneType = true;
                        }
                    }
                }
            }

            var errors = new StringBuilder();

            if (hasMultipleTypes)
            {
                // We dont handle multiple types yet.
                //UIUtility.AddText(_inspector, "Multiple components of different types found. We don't handle this situation yet.", Color.red);
                errors.AppendLine("Multiple components of different types found. We don't handle this situation yet.");
                //return;
            }

            if (hasMultipleComponentsOfOneType)
            {
                // We dont handle multiple components of one type yet.
                //UIUtility.AddText(_inspector, "Multiple components of one type found. We don't handle this situation yet.", Color.red);
                errors.AppendLine("Multiple components of one type found. We don't handle this situation yet.");
                //return;
            }

            if (errors.Length > 0)
            {
                UIUtility.AddText(_inspector, "Failed to load inspector:\n" + errors.ToString(), Color.red);
                errors.Clear();
                return;
            }
            
            // TODO: use a separate "combined" version of handling multiple components of one type. (Basic Copy, Save, Paste, Edit?)

            // Very very WIP, unoptimized, buggy and so on.
            StartCoroutine(LoadVisualComponents(selection));
        }

        private static IEnumerator LoadVisualComponents(List<SelectionEntry> selection)
        {
            if (_inspector == null) yield break;
            
            // TODO: Ensure that user clicked on UI, and not on any other collider if UI is in front to not accidentally deselect all items.
            
            foreach (var selectionEntry in selection)
            {
                // TODO: ONLY LOAD THOSE THAT ARE IN VIEW, OTHERS SHOULD BE IGNORED OR WE'LL HAVE PERFORMANCE ISSUES
                // TODO: Put load of these components into Coroutine and make sure to turn it off when page changed so we don't do weird stuff
                var componentRoot = UIUtility.AddRect(_inspector, $"Component from '{selectionEntry.GameObject.name}'")
                    .WithVerticalLayout()
                    //.SetAllStretch(Vector4.zero)
                    .ControlChildSize(true, false)
                    .ForceExpand(true, false)
                    .GetRect()
                    .WithImage(Color.white * 0.2f);
                
                if (selectionEntry.GameObject.TryGetComponent<MobileTruss>(out var mobileTruss))
                {
                    MobileTrussInspector.OnInspector(componentRoot, mobileTruss);
                }
                else if (selectionEntry.GameObject.TryGetComponent<MobileLight>(out var mobileLight))
                {
                    MobileLightInspector.OnInspector(componentRoot, mobileLight);
                }
                else if (selectionEntry.GameObject.TryGetComponent<LightingDrone>(out var lightingDrone))
                {
                    LightingDroneInspector.OnInspector(componentRoot, lightingDrone);
                }
                else if (selectionEntry.GameObject.TryGetComponent<PyroDrone>(out var pyroDrone))
                {
                    PyroDroneInspector.OnInspector(componentRoot, pyroDrone);
                }
                else
                {
                    UIUtility.AddText(componentRoot, "No supported components found in selection", Color.red)
                        .GetRect()
                        .WithSizeDelta(new Vector2(0, 20));
                }

                TransformInspector.OnInspector(componentRoot, selectionEntry.GameObject.transform);
            }
            
            yield return null;
        }

        private static void AddVector3(RectTransform parent, string title, Vector3 vector3)
        {
            Color elementColor = Color.white * 0.2f;
            Color textColor = Color.white;
            
            var rotationInfo = UIUtility.AddItemToList(parent, 0, 15, title);
            var rotationRoot = UIUtility.AddItemToList(parent, 0, 20);
            
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(vector3.x.ToString())
                .WithPlaceholder("X")
                .OnValueChanged(value => { vector3.Set(float.Parse(value), vector3.y, vector3.z); });
            UIUtility.AddInputField(rotationRoot, elementColor,textColor)
                .WithText(vector3.y.ToString())
                .WithPlaceholder("Y")
                .OnValueChanged(value => { vector3.Set(vector3.x, float.Parse(value), vector3.z); });
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(vector3.z.ToString())
                .WithPlaceholder("Z")
                .OnValueChanged(value => { vector3.Set(vector3.x, vector3.y, float.Parse(value)); });
        }

        private void OnSelectionChanged(List<SelectionEntry> obj)
        {
            // Redraw inspector
            ClearInspector();
            DrawInspector();
        }

        public static void DeconstructUI()
        {
            // TODO: unhook from OnobjectSelected
            
        }
    }
}
