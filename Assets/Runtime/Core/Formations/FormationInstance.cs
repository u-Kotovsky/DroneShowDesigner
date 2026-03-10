using UnityEngine;

namespace Runtime.Core.Formations
{
    public class FormationInstance : MonoBehaviour
    {
        public Formation targetFormation;

        [Range(0, 1)]
        public float weight;

        public bool preview = true;
        public float previewSize = .2f;
        
        // TODO: Object exclusion by colliders or something.

        private void OnDrawGizmosSelected()
        {
            if (!preview) return;

            for (var i = 0; i < targetFormation.points.Length; i++)
            {
                var point = targetFormation.points[i];
                var position = transform.TransformPoint(point.localPosition);
                Gizmos.DrawWireSphere(position, previewSize);
            }
        }
    }
}
