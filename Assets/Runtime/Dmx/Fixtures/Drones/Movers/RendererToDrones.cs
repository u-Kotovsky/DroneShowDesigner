using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public class RendererToDrones : MonoBehaviour
    {
        public MeshRenderer target;
        private MeshFilter targetMeshFilter;
        private Vector3[] vertices;
        
        public Transform viewPoint;
        
        private const int HardLimit = 1000;
        public float maxRaycastDistance = 0.1f;
        public float minVertDistance = 0.1f;
        
        private Dictionary<int, int> droneVertex = new Dictionary<int, int>();

        public bool drawDebugLines;
        public bool drawDebugEdgeLines;
        
        // TODO: SO, I create a linear path for each triangle's edge so the we can find out what is connected and
        // then look if there's anything can be subdivided or decimated to optimize drone positions.

        private void AssignMeshData()
        {
            if (target == null) return;
            if (targetMeshFilter != null || (targetMeshFilter != null /*&& 
                                             targetMeshFilter.Length != target.Length*/)) return;
            
            //targetMeshFilter = new MeshFilter[target.Length];
            targetMeshFilter = target.GetComponent<MeshFilter>();
            vertices = targetMeshFilter.sharedMesh.vertices;
            /*for (var i = 0; i < targetMeshFilter.Length; i++)
            {
                targetMeshFilter[i] = target[i].GetComponent<MeshFilter>();
                vertices[i] = targetMeshFilter[i].sharedMesh.vertices;
            }*/
        }

        public void SetupDronePoolToVertices(LightingDrone[] pool)
        {
            droneVertex = new Dictionary<int, int>();
            
            AssignMeshData();

            if (vertices.Length < pool.Length) // something is null
            {
                Debug.LogWarning($"Too low vertices. ({vertices.Length}/{pool.Length})");
                // Good to go, or maybe subdivide result.
                for (var i = 0; i < vertices.Length; i++)
                {
                    bool isNearby = false;
                    
                    // Check if there's one nearby already, if so -> skip
                    for (var i1 = 0; i1 < vertices.Length; i1++)
                    {
                        if (Vector3.Distance(vertices[i], vertices[i1]) < minVertDistance && droneVertex.ContainsValue(i1))
                        {
                            isNearby = true;
                        }
                    }
                    
                    if (!isNearby) droneVertex.Add(i, i);
                }

                return;
            }

            if (vertices.Length > pool.Length)
            {
                Debug.LogWarning($"Too many vertices. ({vertices.Length}/{pool.Length})");
                // Decimate vertices
                // Or also calculate only visible
                for (var i = 0; i < vertices.Length; i++)
                {
                    bool isNearby = false;
                    
                    // Check if there's one nearby already, if so -> skip
                    for (var i1 = 0; i1 < vertices.Length; i1++)
                    {
                        if (Vector3.Distance(vertices[i], vertices[i1]) < minVertDistance)
                            isNearby = true;
                    }
                    
                    if (!isNearby) droneVertex.Add(i, i);
                }
                return;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (target == null) return;
            
            AssignMeshData();

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
        
        public void CheckForSpace(int vertexIndex, Action<Vector3, Vector3, bool> callback) // go through all 1k drones
        {
            if (vertices == null)
            {
                AssignMeshData();
                return;
            }
            
            if (vertexIndex >= vertices.Length) return;
            
            var vertex = vertices[vertexIndex];
            Vector3 worldVertex = target.transform.TransformPoint(vertex);

            if (viewPoint != null)
            {
                // Distance from view point to vertex in world
                var vertexToView = Vector3.Distance(worldVertex, viewPoint.position);
                
                Vector3 direction = worldVertex - viewPoint.position;
                var hitWasMade = Physics.Raycast(viewPoint.position, direction, out var hit, 50);
                
                // check distance between world vertex and hit point, if distance is too high, skip raycast
                // todo: subdivide vertex count to get less gaps between vertex points

                if (hitWasMade)
                {
                    var vertexToHit = Vector3.Distance(worldVertex, hit.point);
                    if (vertexToHit > maxRaycastDistance) return;

                    if (drawDebugLines)
                    {
                        Debug.DrawLine(viewPoint.transform.position, hit.point, vertexToHit > vertexToView ? Color.red : Color.green);
                        Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.08f, vertexToHit > vertexToView ? Color.yellow : Color.blue);
                    }
                }
                
                callback?.Invoke(worldVertex, hit.point, hitWasMade);
            }
            else
            {
                callback?.Invoke(worldVertex, worldVertex, true); // atm using this
            }
            
            if (drawDebugEdgeLines)
            {
                if (vertices == null || vertices.Length < 2) return;
                
                var next = vertices.Length + 1 >= vertexIndex 
                           && vertices.Length - 1 >= 0 ? vertexIndex - 1 : vertexIndex + 1;

                if (vertexIndex == 0) next = 1;
                
                try
                { // todo: subdivision
                    var worldNext = vertices[next];
                    Vector3 worldVertex2 = target.transform.TransformPoint(worldNext);
                    Vector3 worldVertex3 = Vector3.Lerp(worldVertex, worldVertex2, 0.5f);
                    
                    Debug.DrawLine(worldVertex, worldVertex2, Color.grey); // Mesh Edge
                    Debug.DrawLine(worldVertex, worldVertex + Vector3.up * 0.1f, Color.green); // Actual Vertex
                    Debug.DrawLine(worldVertex3, worldVertex3 + Vector3.up * 0.1f, Color.blue); // Additive Vertex
                    //Debug.DrawLine(worldVertex2, worldVertex2 + Vector3.up * 0.1f, Color.green);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.Log($"{vertexIndex} {next}/{vertices.Length}");
                }
            }
        }

        public void SetDronePosition(LightingDrone drone) // go through all 1k drones
        {
            if (vertices == null)
            {
                AssignMeshData();
                return;
            }
            
            if (drone.fixtureIndex >= vertices.Length) return;
            
            
            CheckForSpace(drone.fixtureIndex, (worldVertex, hitPoint, isHitDrone) =>
            {
                if (!droneVertex.TryGetValue(drone.fixtureIndex, out var vertexIndex)) return;

                // TODO: check if that vertex is not obscured by other drones
                // We have viewPoint that is a sphere, so we can check by raycasting from viewpoint into drone direction,
                // if it hits something that's not drone -> skip that drone
                if (viewPoint != null)
                {
                    //var vertexToView = Vector3.Distance(worldVertex, viewPoint.position);
                    var vertexToHit = Vector3.Distance(worldVertex, hitPoint);
                    if (isHitDrone)
                    {
                        drone.transform.position = hitPoint;
                    }
                    if (isHitDrone && vertexToHit < maxRaycastDistance)
                    {
                        drone.color = GetColorByVertex(target, targetMeshFilter, vertexIndex);
                    }
                    else
                    {
                        drone.color =  Color.black;
                    }
                    drone.WriteDmxColor(drone.color);
                }
                else
                {
                    WriteVertexToDrone(drone, worldVertex, GetColorByVertex(target, targetMeshFilter, vertexIndex));
                }
            });
        }

        public static void WriteVertexToDrone(LightingDrone drone, Vector3 position, Color color)
        {
            drone.transform.position = position;
            drone.color = color;
            drone.WriteDmxColor(drone.color);
        }

        public static Color GetColorByVertex(MeshRenderer renderer, MeshFilter filter, int vertexIndex)
        {
            // Get UV coordinates
            Vector2[] uvs = filter.sharedMesh.uv;

            // Get UV for this vertex
            Vector2 uv = uvs[vertexIndex];

            // Sample texture at UV (if material has a main texture)
            Texture2D mainTexture = renderer.material.mainTexture as Texture2D;

            if (mainTexture != null)
            {
                // Convert UV to pixel coordinates
                int x = Mathf.RoundToInt(uv.x * mainTexture.width);
                int y = Mathf.RoundToInt(uv.y * mainTexture.height);
    
                Color textureColor = mainTexture.GetPixel(x, y);
                return textureColor * renderer.material.color;
            }
            else
            {
                // No texture, just use material color
                return renderer.material.color;
            }
        }
    }
}
