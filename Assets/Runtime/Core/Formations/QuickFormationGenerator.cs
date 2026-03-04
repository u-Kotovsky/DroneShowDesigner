using System;
using System.Collections.Generic;
using Runtime.Core.Formations.Shapes;
using UnityEngine;

namespace Runtime.Core.Formations
{
    public class QuickFormationGenerator : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeReference]
        public IShapeGenerator shapeGenerator;
        public int shapeGeneratorType = -1;
        public GameObject visual;
        public Transform root;
        public List<Transform> points;
        public bool visualize = true;
        public bool affectTransform = true;

        [Range(0, 1)]
        public float weight = 1;
        
        [Header("Gizmo options")]
        public bool drawDirectionGizmos = true;
        public bool drawPointGizmos = true;

        public event Action OnDrawGizmosSelectedEvent = delegate { };
        private void OnDrawGizmosSelected()
        {
            OnDrawGizmosSelectedEvent.Invoke();
        }

        public void ApplyTransform(Vector3[] positions, Transform pivot)
        {
            for (var i = 0; i < positions.Length; i++)
            {
                positions[i] = pivot.TransformPoint(positions[i]);
            }
        }
    }
}