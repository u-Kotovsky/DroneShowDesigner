using UnityEngine;

namespace Runtime.Core.Formations.Shapes
{
    public interface IShapeGenerator
    {
        // TODO: rename it into Sampler, SamplePoint etc. as we are taking sample from the "formation" by index
        public bool GeneratePoint(int index, out Vector3 point);
        public bool Generate(out Vector3[] points);
    
#if UNITY_EDITOR
        public void DrawInspector();
#endif
    }
}