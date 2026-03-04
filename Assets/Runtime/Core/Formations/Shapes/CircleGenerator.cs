using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Runtime.Core.Formations.Shapes
{
    [Serializable]
    public class CircleGenerator : IShapeGenerator
    {
        public float radius = 1;
        public int count = 1;

        public bool GeneratePoint(int index, out Vector3 point)
        {
            float p = ((float)index / count) * Mathf.PI * 2;
            point = new Vector3(Mathf.Sin(p), 0, Mathf.Cos(p)) * radius;
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
            radius = EditorGUILayout.FloatField(nameof(radius), radius);
            count = EditorGUILayout.IntField(nameof(count), count);
        }
#endif
    }
}
