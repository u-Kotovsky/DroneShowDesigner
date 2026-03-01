using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core.Selection
{
    public class Selectable : MonoBehaviour
    {
        // Cached property indexes
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int DecalBlendAlpha = Shader.PropertyToID("_DecalBlendAlpha"); // Selection outline
        
        // References
        public List<Material> originalMaterials = new();
        public List<Renderer> renderers = new();
        public Material selectedMaterial;
        
        public bool autoCollectRenderers = true;

        public bool useMaterialSwap = false;
        public bool useDecalBlendAlpha = true;

        private void Awake()
        {
            if (useDecalBlendAlpha)
            {
                SetupProps();
            }
            
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

        private MaterialPropertyBlock selectedProperty;
        private MaterialPropertyBlock deselectedProperty;

        private void SetupProps()
        {
            selectedProperty = new MaterialPropertyBlock();
            deselectedProperty = new MaterialPropertyBlock();

            selectedProperty.SetFloat(DecalBlendAlpha, 1);
            deselectedProperty.SetFloat(DecalBlendAlpha, 0);
        }

        public void OnObjectSelected()
        {
            foreach (var renderer1 in renderers)
            {
                var materials = renderer1.sharedMaterials;

                if (useMaterialSwap)
                {
                    for (var i = 0; i < materials.Length; i++)
                        materials[i] = selectedMaterial;
                    
                    renderer1.sharedMaterials = materials;
                }

                if (useDecalBlendAlpha)
                {
                    renderer1.SetPropertyBlock(selectedProperty);
                }
            }
        }

        public void OnObjectDeselected()
        {
            foreach (var renderer1 in renderers)
            {
                var materials = renderer1.sharedMaterials;

                if (useMaterialSwap)
                {
                    for (var i = 0; i < materials.Length; i++)
                        materials[i] = originalMaterials[i];
                    
                    renderer1.sharedMaterials = materials;
                }
                
                if (useDecalBlendAlpha)
                {
                    renderer1.SetPropertyBlock(deselectedProperty);
                }
            }
        }
    }
}
