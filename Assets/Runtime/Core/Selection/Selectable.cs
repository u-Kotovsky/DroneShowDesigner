using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core.Selection
{
    public class Selectable : MonoBehaviour
    {
        // Cached property indexes
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        
        // References
        public List<Material> originalMaterials = new();
        public List<Renderer> renderers = new();
        public Material selectedMaterial;
        
        public bool autoCollectRenderers = true;

        private void Awake()
        {
            if (selectedMaterial == null)
            {
                selectedMaterial = new Material(Shader.Find("Unlit/Color"));
                selectedMaterial.SetColor(Color1, Color.orange);
            }

            if (autoCollectRenderers)
            {
                var renderersInChilds = gameObject.GetComponentsInChildren<Renderer>();
                renderers.AddRange(renderersInChilds);
            }

            foreach (var renderer1 in renderers)
            {
                foreach (var renderer1Material in renderer1.materials)
                    originalMaterials.Add(renderer1Material);
            }
        }

        public void OnObjectSelected()
        {
            foreach (var renderer1 in renderers)
            {
                var materials = renderer1.sharedMaterials;

                for (var i = 0; i < materials.Length; i++)
                    materials[i] = selectedMaterial;
                
                renderer1.sharedMaterials = materials;
            }
        }

        public void OnObjectDeselected()
        {
            foreach (var t in renderers)
            {
                var materials = t.sharedMaterials;
                
                for (var i1 = 0; i1 < materials.Length; i1++)
                    materials[i1] = originalMaterials[i1];
                
                t.sharedMaterials = materials;
            }
        }
    }
}
