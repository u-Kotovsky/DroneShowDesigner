using System;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public class RendererToDrones : MonoBehaviour
    {
        public MeshRenderer target;
        public Transform viewPoint;
        
        private MeshFilter targetMeshFilter;
        private Vector3[] vertices;
        private const int HardLimit = 1000;
        public float maxRaycastDistance = 0.1f;

        private void AssignMeshFilter()
        {
            if (target == null) return;
            if (targetMeshFilter != null) return;
            
            targetMeshFilter = target.GetComponent<MeshFilter>();
        }

        private void AssignVertices()
        {
            if (targetMeshFilter == null) return;
            if (targetMeshFilter.sharedMesh == null) return;
            
            vertices = targetMeshFilter.sharedMesh.vertices;
        }
        
        
        private void OnDrawGizmos()
        {
            if (target == null) return;
            
            AssignMeshFilter();
            AssignVertices();

            if (vertices == null) return;

            for (var i = 0; i < vertices.Length && i < HardLimit; i++)
            {
                CheckForSpace(i, (worldVertex, hitPoint, isHitDrone) =>
                {
                    //if (isHitDrone) return;
                    //Debug.DrawLine(worldVertex, viewPoint.position, isHitDrone ? Color.green : Color.red);
                    //Debug.DrawLine(hitPoint, worldVertex, isHitDrone ? Color.green : Color.red);
                });
            }
        }

        public void SetDronesPosition(LightingDrone[] drones) // go through all 1k drones
        {
            if (vertices == null)
            {
                AssignVertices();
                AssignMeshFilter();

                return;
            }
            
            for (var i = 0; i < drones.Length && i < vertices.Length; i++)
            {
                drones[i].transform.position = vertices[i];
            }
        }
        
        
        public void CheckForSpace(int vertexIndex, Action<Vector3, Vector3, bool> callback) // go through all 1k drones
        {
            if (vertices == null)
            {
                AssignVertices();
                AssignMeshFilter();

                return;
            }
            
            if (vertexIndex >= vertices.Length) return;
            
            var vertex = vertices[vertexIndex];
            Vector3 worldVertex = target.transform.TransformPoint(vertex);
            
            // Distance from view point to vertex in world
            var distance = Vector3.Distance(worldVertex, viewPoint.position);

            if (viewPoint != null)
            {
                Vector3 direction = worldVertex - viewPoint.position;
                var hitWasMade = Physics.Raycast(viewPoint.position, direction, out var hit, 50);
                
                // check distance between world vertex and hit point, if distance is too high, skip raycast
                // todo: subdivide vertex count to get less gaps between vertex points

                if (hitWasMade)
                {
                    var distanceVertexToHit = Vector3.Distance(worldVertex, hit.point);
                    if (distanceVertexToHit > maxRaycastDistance)
                    {
                        //Debug.DrawLine(worldVertex, hit.point, Color.pink);
                        return;
                    }

                    //Debug.DrawLine(viewPoint.transform.position, hit.point,
                    //    distanceVertexToHit > distance ? Color.red : Color.green);

                    //Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.08f,
                    //    distanceVertexToHit > distance ? Color.yellow : Color.blue);

                    callback?.Invoke(worldVertex, hit.point, hit.distance <= distance);
                }
            }
            else
            {
                callback?.Invoke(worldVertex, worldVertex, true);
            }
        }

        public void SetDronePosition(LightingDrone drone) // go through all 1k drones
        {
            if (vertices == null)
            {
                AssignVertices();
                AssignMeshFilter();

                return;
            }
            
            if (drone.fixtureIndex >= vertices.Length) return;
            
            // TODO: check if that vertex is not obscured by other drones
            // We have viewPoint that is a sphere, so we can check by raycasting from viewpoint into drone direction,
            // if it hits something that's not drone -> skip that drone
            
            CheckForSpace(drone.fixtureIndex, (worldVertex, hitPoint, isHitDrone) =>
            {
                if (isHitDrone)
                {
                    drone.transform.position = hitPoint;
                }
                drone.color = isHitDrone ? Vector3.Distance(worldVertex, hitPoint) < maxRaycastDistance ? Color.green : Color.red
                        : Color.red;
            });
        }
    }
}
