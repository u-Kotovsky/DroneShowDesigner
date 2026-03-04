using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Runtime.Core.Formations.Shapes
{
    [Serializable]
    public class RectangleGenerator : IShapeGenerator
    {
        public float width = 1;
        public float height = 1;
        public bool stroke = false;
        public int count = 1;

        public bool GeneratePoint(int index, out Vector3 point)
        {
            if (stroke)
            {
                float perimeter = 2 * (width + height);
                float step = perimeter / count;
                float distance = (index * step) % perimeter;

                if (distance < width) // bottom edge: left to right
                {
                    point = new Vector3(-width / 2 + distance, 0, -height / 2);
                }
                else if (distance < width + height) // right edge: bottom to top
                {
                    point = new Vector3(width / 2, 0, -height / 2 + (distance - width));
                }
                else if (distance < width * 2 + height) // top edge: right to left
                {
                    point = new Vector3(width / 2 - (distance - (width + height)), 0, height / 2);
                }
                else // left edge: top to bottom
                {
                    point = new Vector3(-width / 2, 0, height / 2 - (distance - (width * 2 + height)));
                }
            }
            else
            {
                int columns = Mathf.CeilToInt(Mathf.Sqrt(count * (width / height)));
                int rows = Mathf.CeilToInt((float)count / columns);

                int col = index % columns;
                int row = index / columns;
                
                float stepX = width / columns;
                float stepY = height / rows;

                float x = -width / 2 + stepX / 2 + col * stepX;
                float z = -height / 2 + stepY / 2 + row * stepY;
                
                point = new Vector3(x, 0, z);
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
            stroke = EditorGUILayout.Toggle(nameof(stroke), stroke);
            count = EditorGUILayout.IntField(nameof(count), count);
        }
#endif
    }
}
