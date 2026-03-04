using Runtime.Dmx.Fixtures.Drones;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Runtime.Core.Formations
{
    [CustomEditor(typeof(Formation))]
    public class FormationEditor : Editor
    {
        private Formation component;
        
        private Transform pointsRoot;
        private Transform dronesRoot;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            component ??= target as Formation;
            if (component is null)
            {
                EditorGUILayout.HelpBox("Target object is null.", MessageType.Error);
                return;
            }
            
            //pointsRoot = EditorGUILayout.ObjectField("Root of points", pointsRoot, typeof(Transform), true) as Transform;
            dronesRoot = EditorGUILayout.ObjectField("Root of drones", dronesRoot, typeof(Transform), true) as Transform;

            /*if (pointsRoot is not null && GUILayout.Button("Collect points"))
            {
                CollectPoints();
            }*/
            if (dronesRoot is not null && GUILayout.Button("Collect drones"))
            {
                CollectDrones();
            }
            
            if (component.rootsPoints is not null && component.rootsPoints.Count > 0 && GUILayout.Button("Collect points v2"))
            {
                CollectPoints2();
            }
        }
        private void CollectPoints2()
        {
            var list = new List<Transform>();
            
            for (var i = 0; i < component.rootsPoints.Count; i++)
            {
                var rootPoint = component.rootsPoints[i];
                
                for (int j = 0; j < rootPoint.transform.childCount; j++)
                {
                    list.Add(rootPoint.transform.GetChild(j));
                }
            }

            component.points = list.ToArray();
        }

        private void CollectPoints()
        {
            component.points = new Transform[pointsRoot.transform.childCount];

            for (int j = 0; j < component.points.Length; j++)
            {
                var point = pointsRoot.transform.GetChild(j);
                component.points[j] = point;
            }
        }

        private void CollectDrones()
        {
            component.drones.Clear();

            for (int j = 0; j < dronesRoot.transform.childCount; j++)
            {
                var drone = dronesRoot.transform.GetChild(j);
                component.drones.Add(drone);
                if (drone.TryGetComponent<LightingDrone>(out var ld))
                {
                    component.lightingDrones.Add(ld);
                }
            }
        }
    }
}
    
#endif