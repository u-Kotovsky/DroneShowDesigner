using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.Core.Formations.Shapes
{
    [Serializable]
    public class SplineGenerator : IShapeGenerator
    {
        public SplineContainer splineContainer;
        public int count = 1;

        public bool GeneratePoint(int index, out Vector3 point)
        {
            float t = (float)index / count;
            if (splineContainer == null) // heavy check uh...
            {
                point = Vector3.zero;
                return false;
            }
            point = splineContainer.EvaluatePosition(t);
            return true;
        }

        public bool Generate(out Vector3[] points)
        {
            var pointList = new List<Vector3>();
        
            for (int i = 0; i < count; i++)
            {
                GeneratePoint(i, out var position);
                pointList.Add(position);
            }
        
            points = pointList.ToArray();
            return true;
        }

#if UNITY_EDITOR
        public void DrawInspector()
        {
            splineContainer = EditorGUILayout.ObjectField(nameof(splineContainer), splineContainer, typeof(SplineContainer), true) as SplineContainer;
            count = EditorGUILayout.IntField(nameof(count), count);
        }
#endif
    }
}
