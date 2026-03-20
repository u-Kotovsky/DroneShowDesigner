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
        [HideInInspector]
        private IShapeGenerator shapeGenerator;
        public IShapeGenerator ShapeGenerator 
        { 
            get => shapeGenerator;
            set => shapeGenerator = value;
        }

        [SerializeField] 
        private int shapeGeneratorType = -1;
        public int ShapeGeneratorType
        {
            get => shapeGeneratorType;
            set => shapeGeneratorType = value;
        }

        public GameObject visual;
        public Transform root;
        public List<Transform> points;
        public bool visualize = true;
        public bool affectTransform = true;
        
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
        
        public static void RegenerateAll()
        {
            var components = FindObjectsByType<QuickFormationGenerator>(FindObjectsSortMode.None);

            foreach (var component in components)
            {
                component.Generate();
            }
        }

        public bool CreateRootIfNotExists()
        {
            if (root != null) return false;
            
            root = new GameObject("rt-" + gameObject.name).transform;
            root.SetParent(transform);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
            root.localScale = Vector3.one;
            root.gameObject.AddComponent<Formation>();

            return true;
        }

        public bool ClearPointsIfRootExists()
        {
            if (root == null) return false;

            if (!root.gameObject.TryGetComponent<Formation>(out var component)) return false;
            
            component.DeletePoints();
            points.Clear();

            return true;
        }

        public bool ResetPointsRoot()
        {
            // If we have root, clear its contents to preserve references from other components
            if (!CreateRootIfNotExists())
            {
                // But then we lose references to the points....
                return ClearPointsIfRootExists();
            }

            return false;
        }
        
        public void Generate()
        {
            ResetPointsRoot();
            
            ShapeGenerator.Generate(out var positions);
        
            for (var i = 0; i < positions.Length; i++)
            {
                var position = positions[i];
                var point = new GameObject($"Point #{i}");
                point.transform.SetParent(root);
                point.transform.localPosition = position;
                
                points.Add(point.transform);
            }
            
            if (root.gameObject.TryGetComponent<Formation>(out var formation))
            {
                formation.points = points.ToArray();
            }
        }
        
        /*public void RemoveGenerated()
        {
            if (root == null) return;
        
            if (Application.isPlaying)
            {
                Destroy(root.gameObject);
            }
            else
            {
                DestroyImmediate(root.gameObject);
            }
            
            points.Clear();
        }*/
        
        public void Visualize()
        {
            visualize = !visualize;
            
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                if (visualize)
                {
                    AddVisual(visual, point.transform);
                }
                else
                {
                    RemoveVisual(point.transform);
                }
            }
        }
        
        private static void AddVisual(GameObject visual, Transform source)
        {
            var visualPoint = Instantiate(visual, source);
            visualPoint.name = "visual";
            visualPoint.transform.localPosition = Vector3.zero;
        }

        private static void RemoveVisual(Transform source)
        {
            if (source.childCount == 0) return;

            foreach (Transform o in source)
            {
                if (o.name.Contains("visual"))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(o.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(o.gameObject);
                    }
                }
            }
        }
    }
}