using System;
using System.Collections;
using System.Collections.Generic;
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
                try
                {
                    Object.Destroy(child.gameObject);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void DrawInspector()
        {
            var selection = FixtureSelectionManager.Instance.Selection;

            Type type = null;
            bool hasMultipleTypes = false;

            foreach (var selectionEntry in selection)
            {
                if (type == null)
                {
                    type = selectionEntry.GetType();
                    continue;
                }

                if (selectionEntry.GetType() != type)
                {
                    hasMultipleTypes = true;
                }
            }

            if (hasMultipleTypes)
            {
                // We dont handle multiple types yet.
                UIUtility.AddText(_inspector, "Multiple components of different types found. We don't handle this situation yet.", Color.red);
                return;
            }

            // Very very WIP, unoptimized, buggy and so on.
            StartCoroutine(LoadVisualComponents(selection));
        }

        private static IEnumerator LoadVisualComponents(List<SelectionEntry> selection)
        {
            foreach (var selectionEntry in selection)
            {
                if (_inspector == null) yield break;
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
                    UIUtility.AddText(componentRoot, "MobileTruss", Color.white)
                        .GetRect()
                        .WithSizeDelta(new Vector2(0, 20));
                }
                else if (selectionEntry.GameObject.TryGetComponent<MobileLight>(out var mobileLight))
                {
                    UIUtility.AddText(componentRoot, "MobileLight", Color.white)
                        .GetRect()
                        .WithSizeDelta(new Vector2(0, 20));
                }
                else if (selectionEntry.GameObject.TryGetComponent<LightingDrone>(out var lightingDrone))
                {
                    UIUtility.AddText(componentRoot, "LightingDrone", Color.white)
                        .GetRect()
                        .WithSizeDelta(new Vector2(0, 20));
                }
                else if (selectionEntry.GameObject.TryGetComponent<PyroDrone>(out var pyroDrone))
                {
                    UIUtility.AddText(componentRoot, "PyroDrone", Color.white)
                        .GetRect()
                        .WithSizeDelta(new Vector2(0, 20));
                }
                else
                {
                    UIUtility.AddText(componentRoot, "No supported components found in selection", Color.red)
                        .GetRect()
                        .WithSizeDelta(new Vector2(0, 20));
                }

                AddTransformControls(componentRoot, selectionEntry.GameObject.transform);
            }
            
            
            yield return null;
        }

        private static void AddTransformControls(RectTransform parent, Transform transform)
        {
            var positionInfo = UIUtility.AddItemToList(parent, 0, 15, "Position");
            var positionRoot = UIUtility.AddItemToList(parent, 0, 20);

            Color elementColor = Color.white * 0.2f;
            Color textColor = Color.white;
            
            // May not properly update origin, need to look into other solution for that.
            // TODO: fix alignment
            
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.x.ToString())
                .WithPlaceholder("X")
                .OnValueChanged(value => { transform.localPosition.Set(float.Parse(value), transform.localPosition.y, transform.localPosition.z); });
            UIUtility.AddInputField(positionRoot, elementColor,textColor)
                .WithText(transform.localPosition.y.ToString())
                .WithPlaceholder("Y")
                .OnValueChanged(value => { transform.localPosition.Set(transform.localPosition.x, float.Parse(value), transform.localPosition.z); });
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.z.ToString())
                .WithPlaceholder("Z")
                .OnValueChanged(value => { transform.localPosition.Set(transform.localPosition.x, transform.localPosition.y, float.Parse(value)); });
            
            /*var rotationInfo = UIUtility.AddItemToList(parent, 0, 15, "Rotation");
            var rotationRoot = UIUtility.AddItemToList(parent, 0, 20);
            
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(transform.localRotation.eulerAngles.x.ToString())
                .WithPlaceholder("X")
                .OnValueChanged(value => { transform.localRotation.eulerAngles.Set(float.Parse(value), transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z); });
            UIUtility.AddInputField(rotationRoot, elementColor,textColor)
                .WithText(transform.localRotation.eulerAngles.y.ToString())
                .WithPlaceholder("Y")
                .OnValueChanged(value => { transform.localRotation.eulerAngles.Set(transform.localRotation.eulerAngles.x, float.Parse(value), transform.localRotation.eulerAngles.z); });
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(transform.localRotation.eulerAngles.z.ToString())
                .WithPlaceholder("Z")
                .OnValueChanged(value => { transform.localRotation.eulerAngles.Set(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, float.Parse(value)); });*/
            
            
            /*Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            
            transform.GetPositionAndRotation(out position, out rotation);
            
            Vector3 eulerAngles = transform.localRotation.eulerAngles;
            // Questionable, not sure if it'll update original part of transform
            AddVector3(rect, "Position", position);
            AddVector3(rect, "Rotation", eulerAngles);*/
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
