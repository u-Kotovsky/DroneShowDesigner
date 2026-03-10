using UnityEngine;

namespace Runtime.Core.Formations
{
    public abstract class FormationGizmo
    {
        public static void DrawPoints(Vector3[] points, Vector3 sizeOfDrone, Color directionColor, Color pointColor, bool directionGizmo, bool pointGizmo)
        {
            for (var i = 0; i < points.Length; i++)
            {
                var position = points[i];
                
                if (directionGizmo)
                {
                    if (points.Length == 1)
                    {
                        Debug.DrawLine(position, position + Vector3.up, directionColor);
                        continue;
                    }
                    
                    Vector3 nextPosition;

                    if (i < points.Length - 1)
                    {
                        nextPosition = points[i + 1];
                    }
                    else
                    {
                        nextPosition = points[0];
                    }
                
                    Debug.DrawLine(position, nextPosition, directionColor);
                }

                if (pointGizmo)
                {
                    Gizmos.color = pointColor;
                    Gizmos.DrawWireCube(position, sizeOfDrone);
                    Gizmos.color = Color.white;
                }
            }
        }
    }
}
