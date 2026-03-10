using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshPointVisualizer : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float spacing = 0.1f;          // world units between points
    public float debugRadiusMultiplier = 0.1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private List<Vector3> allPoints;

    private void OnValidate()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        mesh = meshFilter.sharedMesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        
        allPoints = new List<Vector3>();
        
        CollectPoints();
    }

    private void OnTransformChildrenChanged()
    {
        CollectPoints();
    }

    private void OnTransformParentChanged()
    {
        CollectPoints();
    }

    private void OnEnable()
    {
        CollectPoints();
    }

    private void CollectPoints()
    {
        allPoints.Clear();

        if (spacing < 0.05) return;

        // Loop over all triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // Convert to world space if the object is moved/scaled
            v0 = transform.TransformPoint(v0);
            v1 = transform.TransformPoint(v1);
            v2 = transform.TransformPoint(v2);

            List<Vector3> triPoints = TrianglePointGenerator.GeneratePointsPlanar(v0, v1, v2, spacing);
            allPoints.AddRange(triPoints);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (allPoints == null) return;
        float r = spacing * debugRadiusMultiplier;
        foreach (var allPoint in allPoints)
        {
            Gizmos.color = Color.azure;
            Gizmos.DrawWireSphere(allPoint, r);
            Gizmos.color = Color.white;
        }
    }

    void Start()
    {
        if (meshFilter == null) return;
        
        CollectPoints();
    }
}