using System;
using UnityEngine;

namespace Runtime.Core.Timeline
{
    /**
     * A container for indexes.
     */
    [Serializable]
    public class FormationIndexData
    {
        [SerializeField]
        public int droneIndex;
        [SerializeField]
        public int formationInstanceIndex;
        [SerializeField]
        public int formationPointIndex;
        [SerializeField]
        public int globalPointIndex;

        public FormationIndexData(int droneIndex, int formationInstanceIndex, int formationPointIndex, int globalPointIndex)
        {
            this.droneIndex = droneIndex;
            this.formationInstanceIndex = formationInstanceIndex;
            this.formationPointIndex = formationPointIndex;
            this.globalPointIndex = globalPointIndex;
        }
    }
}
