using System.Collections.Generic;
using UnityEngine;

public static class TrianglePointGenerator
{
    /// <summary>
    /// Returns points inside a triangle placed on a regular square grid with given world‑space spacing.
    /// </summary>
    public static List<Vector3> GeneratePointsPlanar(Vector3 v0, Vector3 v1, Vector3 v2, float spacing)
    {
        var points = new List<Vector3>();

        // Edge vectors
        Vector3 e0 = v1 - v0;
        Vector3 e1 = v2 - v0;

        // Build orthonormal basis
        Vector3 axisX = e0.normalized;
        Vector3 normal = Vector3.Cross(e0, e1).normalized;
        if (normal.sqrMagnitude < 0.5f) return points; // degenerate
        Vector3 axisY = Vector3.Cross(normal, axisX).normalized;

        // Convert vertices to 2D
        Vector2 v0_2D = Vector2.zero;
        Vector2 v1_2D = new Vector2(Vector3.Dot(e0, axisX), Vector3.Dot(e0, axisY)); // = (len0, 0)
        Vector2 v2_2D = new Vector2(Vector3.Dot(e1, axisX), Vector3.Dot(e1, axisY));

        // 2D bounding box with a small safety margin
        float minX = Mathf.Min(v0_2D.x, v1_2D.x, v2_2D.x) - spacing * 0.1f;
        float maxX = Mathf.Max(v0_2D.x, v1_2D.x, v2_2D.x) + spacing * 0.1f;
        float minY = Mathf.Min(v0_2D.y, v1_2D.y, v2_2D.y) - spacing * 0.1f;
        float maxY = Mathf.Max(v0_2D.y, v1_2D.y, v2_2D.y) + spacing * 0.1f;

        // Iterate grid
        for (float x = minX; x <= maxX; x += spacing)
        {
            for (float y = minY; y <= maxY; y += spacing)
            {
                Vector2 p = new Vector2(x, y);
                if (IsPointInTriangleBarycentric(p, v0_2D, v1_2D, v2_2D, tolerance: 1e-6f))
                {
                    Vector3 worldPoint = v0 + x * axisX + y * axisY;
                    points.Add(worldPoint);
                }
            }
        }

        return points;
    }

    // Barycentric test with a small absolute tolerance (safer than area comparison)
    private static bool IsPointInTriangleBarycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, float tolerance = 1e-6f)
    {
        // Compute barycentric coordinates u, v, w such that p = u*a + v*b + w*c, u+v+w=1
        Vector2 v0 = b - a;
        Vector2 v1 = c - a;
        Vector2 v2 = p - a;

        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);

        float denom = d00 * d11 - d01 * d01;
        if (Mathf.Abs(denom) < 1e-12f) return false; // degenerate

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        // Use a small tolerance to include/exclude edges
        return u >= -tolerance && v >= -tolerance && w >= -tolerance;
    }
}