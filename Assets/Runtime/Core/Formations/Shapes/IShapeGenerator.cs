using UnityEngine;

namespace Runtime.Core.Formations.Shapes
{
    public interface IShapeGenerator
    {
        public bool GeneratePoint(int index, out Vector3 point);
        public bool Generate(out Vector3[] points);
    
#if UNITY_EDITOR
        public void DrawInspector();
#endif
    }
}