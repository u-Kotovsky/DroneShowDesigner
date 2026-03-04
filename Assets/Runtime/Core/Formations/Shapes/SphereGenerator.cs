using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime.Core.Formations.Shapes
{
    [Serializable]
    public class SphereGenerator : IShapeGenerator
    {
        public float radius = 1;
        public int count = 1;
        
        public void GenerateCirclePoint(int index, out Vector3 point)
        {
            float p = ((float)index / count) * Mathf.PI * 2;
            point = new Vector3(Mathf.Sin(p), 0, Mathf.Cos(p)) * radius;
        }
        
    public bool GeneratePoint(int index, out Vector3 point)
    {
        // Using golden spiral algorithm for even distribution on sphere
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        
        float i = index;
        float n = count;
        
        // Map index to spherical coordinates with even distribution
        float theta = 2 * Mathf.PI * i / goldenRatio;
        float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / n);
        
        // Convert to Cartesian coordinates
        float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = radius * Mathf.Cos(phi);
        
        point = new Vector3(x, y, z);

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

    // Alternative method using spherical coordinates with fixed increments (poles will have clustering)
    public void GenerateSimple(out Vector3[] points)
    {
        var pointList = new List<Vector3>();
        
        // Number of latitude rings (adjust for better distribution)
        int rings = Mathf.RoundToInt(Mathf.Sqrt(count));
        int pointsPerRing = Mathf.RoundToInt((float)count / rings);
        
        for (int ring = 0; ring < rings; ring++)
        {
            // Latitude angle from -90 to 90 degrees
            float phi = Mathf.PI * ((float)ring / (rings - 1) - 0.5f);
            float y = radius * Mathf.Sin(phi);
            float ringRadius = radius * Mathf.Cos(phi);
            
            int pointsInThisRing = (ring == 0 || ring == rings - 1) ? 1 : pointsPerRing;
            
            for (int j = 0; j < pointsInThisRing; j++)
            {
                // Longitude angle
                float theta = 2 * Mathf.PI * ((float)j / pointsInThisRing);
                
                float x = ringRadius * Mathf.Cos(theta);
                float z = ringRadius * Mathf.Sin(theta);
                
                pointList.Add(new Vector3(x, y, z));
            }
        }
        
        points = pointList.ToArray();
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
