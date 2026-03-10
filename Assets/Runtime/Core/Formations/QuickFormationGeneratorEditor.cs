using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Core.Formations.Shapes;
using Runtime.Core.Resources;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Runtime.Core.Formations
{
    [CustomEditor(typeof(QuickFormationGenerator))]
    public class QuickFormationGeneratorEditor : Editor
    {
        private bool isListeningEvent = false;
        private QuickFormationGenerator component;
    
        public static List<Type> GetInterfaceImplementations<TInterface>()
        {
            var interfaceType = typeof(TInterface);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();
        }
    
        private List<Type> shapeGenerators;
        private List<string> shapeGeneratorNames = new List<string>();
        
        private readonly Vector3 sizeOfDrone = Vector3.one * .25f;
    
        private void OnDrawGizmosSelected()
        {
            if (component is null)
            {
                return;
            }

            if (component.shapeGenerator is null)
            {
                return;
            }
        
            component.shapeGenerator.Generate(out var points);
            if (component.affectTransform) component.ApplyTransform(points, component.transform);

            FormationGizmo.DrawPoints(points, sizeOfDrone, Color.red, Color.white, component.drawDirectionGizmos, component.drawPointGizmos);
        }

        private void RemoveGenerated()
        {
            if (component.root == null) return;
        
            if (Application.isPlaying)
            {
                Destroy(component.root.gameObject);
            }
            else
            {
                DestroyImmediate(component.root.gameObject);
            }
                
            component.points.Clear();
        }

        private void Visualize()
        {
            component.visualize = !component.visualize;
            
            for (var i = 0; i < component.points.Count; i++)
            {
                var point = component.points[i];

                if (component.visualize)
                {
                    AddVisual(component.visual, point.transform);
                }
                else
                {
                    RemoveVisual(point.transform);
                }
            }
        }

        private void Generate()
        {
            RemoveGenerated();
        
            component.root = new GameObject("rt-" + component.gameObject.name).transform;
            component.root.SetParent(component.transform);
            component.root.localPosition = Vector3.zero;
            component.root.localRotation = Quaternion.identity;
            component.root.localScale = Vector3.one;
            
        
            component.shapeGenerator.Generate(out var positions);
        
            for (var i = 0; i < positions.Length; i++)
            {
                var position = positions[i];
                var point = new GameObject($"Point #{i}");
                point.transform.SetParent(component.root);
                point.transform.localPosition = position;
                
                component.points.Add(point.transform);
            }
            
            var formation = component.root.gameObject.AddComponent<Formation>();
            formation.points = component.points.ToArray();
        }

        private void InitializeGenerator(bool forceReplace = false)
        {
            if (component.shapeGenerator != null && !forceReplace)
            {
                return;
            }
        
            var type = shapeGenerators[component.shapeGeneratorType];
            component.shapeGenerator = Activator.CreateInstance(type) as IShapeGenerator;
        }

        private void OnEnable()
        {
            component ??= target as QuickFormationGenerator;
            shapeGenerators = GetInterfaceImplementations<IShapeGenerator>();
            shapeGeneratorNames = shapeGenerators.Select(type => type.Name).ToList();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            component ??= target as QuickFormationGenerator;
            if (component is null)
            {
                EditorGUILayout.LabelField("failed to load component. Component is null.", StyleUtility.LabelError);
                return; 
            }

            if (!isListeningEvent)
            {
                isListeningEvent = true;
                component.OnDrawGizmosSelectedEvent += OnDrawGizmosSelected;
            }

            EditorGUI.BeginChangeCheck();
            component.shapeGeneratorType = EditorGUILayout.Popup("Shape Generator Type", component.shapeGeneratorType, shapeGeneratorNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Debug.LogWarning("Types don't match. initialize generator.");
                InitializeGenerator(true);
            }

            if (component.shapeGenerator == null)
            {
                Debug.LogError("Generator is null. Making new one.");
                EditorGUILayout.LabelField("failed to load generator. Generator is null.", StyleUtility.LabelError);
                InitializeGenerator();
                return;
            }
        
            EditorGUILayout.LabelField("Properties of generator");
            EditorGUI.indentLevel++;
            component.shapeGenerator.DrawInspector();
            EditorGUI.indentLevel--;
        
            if (GUILayout.Button("Remove"))
            {
                RemoveGenerated();
            }

            if (GUILayout.Button("Visualize"))
            {
                Visualize();
            }

            if (GUILayout.Button("Generate"))
            {
                Generate();
            }
        }

        private static void AddVisual(GameObject visual, Transform source)
        {
            var visualPoint = Instantiate(visual, source);
            visualPoint.name = "visual";
            visualPoint.transform.localPosition = Vector3.zero;
        }

        private static void RemoveVisual(Transform source)
        {
            if (source.childCount == 0) return;

            foreach (Transform o in source)
            {
                if (o.name.Contains("visual"))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(o.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(o.gameObject);
                    }
                }
            }
        }
    }
}
#endif
