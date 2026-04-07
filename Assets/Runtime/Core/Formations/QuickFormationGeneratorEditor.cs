using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Core.Formations.Shapes;
using Runtime.Core.Resources;
using UnityEngine;
using Object = UnityEngine.Object;

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

            if (component.ShapeGenerator is null)
            {
                return;
            }
        
            component.ShapeGenerator.Generate(out var points);
            if (component.affectTransform) component.ApplyTransform(points, component.transform);

            FormationGizmo.DrawPoints(points, sizeOfDrone, Color.red, Color.white, component.drawDirectionGizmos, component.drawPointGizmos);
        }

        private void InitializeGenerator(bool forceReplace = false)
        {
            if (component.ShapeGenerator != null && !forceReplace)
            {
                return;
            }
        
            var type = shapeGenerators[component.ShapeGeneratorType];
            component.ShapeGenerator = Activator.CreateInstance(type) as IShapeGenerator;
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
            component.ShapeGeneratorType = EditorGUILayout.Popup("Shape Generator Type", component.ShapeGeneratorType, shapeGeneratorNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Debug.LogWarning("Types don't match. initialize generator.");
                InitializeGenerator(true);
            }

            if (component.ShapeGenerator == null)
            {
                Debug.LogError("Generator is null. Making new one.");
                EditorGUILayout.LabelField("failed to load generator. Generator is null.", StyleUtility.LabelError);
                InitializeGenerator();
                return;
            }
        
            EditorGUILayout.LabelField("Properties of generator");
            EditorGUI.indentLevel++;
            component.ShapeGenerator.DrawInspector();
            EditorGUI.indentLevel--;
        
            if (GUILayout.Button("Remove"))
            {
                component.ResetPointsRoot();
            }

            if (GUILayout.Button("Visualize"))
            {
                component.Visualize();
            }

            if (GUILayout.Button("Generate"))
            {
                component.Generate();
            }
            
            EditorGUILayout.Space();

            var col = GUI.color;
            GUI.color = Color.red;
            if (GUILayout.Button("Regenerate ALL"))
            {
                QuickFormationGenerator.RegenerateAll();
            }
            GUI.color = col;
        }
    }
}
#endif
