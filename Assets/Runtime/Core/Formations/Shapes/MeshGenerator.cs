using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Runtime.Core.Formations.Shapes
{
    [Serializable]
    public class MeshGenerator : IShapeGenerator
    {
        public Mesh mesh;
        public int count;
        public int offset;
        public bool fastMode = true;
        
        private Vector3[] vertices;
        private bool isBaked = false;

        private void TryBakeMesh(bool forceRebake = false, bool overrideCount = false) // kinda
        {
            if (!isBaked && !forceRebake)
            {
                return;
            }

            if (mesh == null)
            {
                vertices = new Vector3[0];
                isBaked = false;
                return;
            }
            
            
            vertices = new Vector3[mesh.vertices.Length];
            
            if (overrideCount) count = vertices.Length;
            
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = mesh.vertices[i]; // out of bounds
            }
        }
        
        public bool GeneratePoint(int index, out Vector3 point)
        {
            TryBakeMesh();
            if (mesh is null || index > count)
            {
                point = Vector3.zero;
                return false;
            }

            if (fastMode)
            {
                if (vertices == null)
                {
                    point = Vector3.zero;
                    return false;
                }
                if (index >= vertices.Length)
                {
                    point = Vector3.zero;
                    return false;
                }
                point = vertices[index];
            }

            float position = (float)index / count;
            float closestIndex = position * vertices.Length;
            point = vertices[(int)closestIndex];
            return true;
        }

        public bool Generate(out Vector3[] points)
        {
            if (mesh is null)
            {
                points = Array.Empty<Vector3>();
                return false;
            }
            
            var pointList = new List<Vector3>();
        
            for (int i = 0; i < count; i++)
            {
                if (GeneratePoint(i, out var position))
                {
                    pointList.Add(position);
                }
            }
        
            points = pointList.ToArray();
            return true;
        }

#if UNITY_EDITOR
        public void DrawInspector()
        {
            if (mesh != null && (vertices == null || vertices.Length == 0))
            {
                TryBakeMesh(true);
            }
            
            fastMode = EditorGUILayout.Toggle(nameof(fastMode), fastMode);
            
            EditorGUI.BeginChangeCheck();
            mesh = EditorGUILayout.ObjectField(mesh, typeof(Mesh), false) as Mesh;
            if (EditorGUI.EndChangeCheck())
            {
                isBaked = false;
                TryBakeMesh(true, true);
            }
            
            if (vertices != null) EditorGUILayout.LabelField($"Vertex count: {vertices.Length}"); // null
            
            count = EditorGUILayout.IntField(nameof(count), count);
            offset = EditorGUILayout.IntField(nameof(offset), offset);
        }
#endif
    }
}
