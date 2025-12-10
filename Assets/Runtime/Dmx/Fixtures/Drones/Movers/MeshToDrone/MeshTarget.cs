using System;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Movers.MeshToDrone
{
    [Serializable]
    public class MeshTarget
    {
        [SerializeField]
        private Renderer renderer;

        [SerializeField]
        private MeshFilter meshFilter;
        
        [SerializeField]
        private Vector3[] vertices;

        public Renderer Renderer
        {
            get => renderer;
            private set => renderer = value;
        }

        public MeshFilter MeshFilter
        {
            get => meshFilter;
            private set => meshFilter = value;
        }

        public Vector3[] Vertices
        {
            get => vertices;
            private set => vertices = value;
        }

        public MeshTarget(Renderer renderer)
        {
            Renderer = renderer;
        }

        private bool isReady = false;

        public void Setup()
        {
            if (isReady) return;
            if (Renderer == null) throw new Exception("Renderer is null");
            //if (MeshFilter != null || (MeshFilter != null //&& targetMeshFilter.Length != target.Length
            //    )) return false;
            
            //MeshFilter = new MeshFilter[target.Length];
            MeshFilter = Renderer.GetComponent<MeshFilter>();
            Vertices = MeshFilter.sharedMesh.vertices;
            //for (var i = 0; i < targetMeshFilter.Length; i++)
            //{
            //    targetMeshFilter[i] = target[i].GetComponent<MeshFilter>();
            //    vertices[i] = targetMeshFilter[i].sharedMesh.vertices;
            //}
            isReady = true;
        }
        
        public Color GetColorByVertex(int vertexIndex)
        {
            // Get UV coordinates
            Vector2[] uvs = meshFilter.sharedMesh.uv;

            // Get UV for this vertex
            Vector2 uv = uvs[vertexIndex];

            // Sample texture at UV (if material has a main texture)
            Texture2D mainTexture = renderer.material.mainTexture as Texture2D;

            if (mainTexture == null)
            {
                // No texture, just use material color
                return renderer.material.color;
            }
            
            // Convert UV to pixel coordinates
            int x = Mathf.RoundToInt(uv.x * mainTexture.width);
            int y = Mathf.RoundToInt(uv.y * mainTexture.height);

            Color textureColor = mainTexture.GetPixel(x, y);
            return textureColor * renderer.material.color;
        }
    }
}
