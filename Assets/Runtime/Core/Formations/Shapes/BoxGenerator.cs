using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.Core.Formations.Shapes
{
    [Serializable]
    public class BoxGenerator : IShapeGenerator 
    {
        public float width = 1;
        public float height = 1;
        public float depth = 1;
        public bool stroke = false;
        public int count = 1;

        public bool GeneratePoint(int index, out Vector3 point)
        {
            if (stroke)
            {
                float perimeter = 4 * (width + height + depth);
                float step = perimeter / count;
                float distance = (index * step) % perimeter;

                point = new Vector3(
                    Mathf.Sin(distance * 0.1f) * width / 2,
                    Mathf.Cos(distance * 0.1f) * height / 2,
                    Mathf.Sin(distance * 0.05f) * depth / 2);
            }
            else
            {
                int perAxis = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
                int x = index % perAxis;
                int y = (index / perAxis) % perAxis;
                int z = index / (perAxis * perAxis);
                
                float stepX = width / perAxis;
                float stepY = height / perAxis;
                float stepZ = depth / perAxis;
        
                point = new Vector3(
                    -width / 2 + stepX / 2 + x * stepX,
                    -height / 2 + stepY / 2 + y * stepY,
                    -depth / 2 + stepZ / 2 + z * stepZ
                );
            }

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
            width = EditorGUILayout.FloatField(nameof(width), width);
            height = EditorGUILayout.FloatField(nameof(height), height);
            depth = EditorGUILayout.FloatField(nameof(depth), depth);
            stroke = EditorGUILayout.Toggle(nameof(stroke), stroke);
            count = EditorGUILayout.IntField(nameof(count), count);
        }
#endif
    }
}
