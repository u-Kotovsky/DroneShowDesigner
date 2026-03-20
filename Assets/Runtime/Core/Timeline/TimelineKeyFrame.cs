using System.Collections.Generic;
using UnityEngine;
using Runtime.Core.Formations;

namespace Runtime.Core.Timeline
{
    /**
     * Keyframe that contains selected formations and points drone indexes to their point indexes.
     */
    public class TimelineKeyFrame : MonoBehaviour
    {
        public float time;
        public float delay;
        public bool overrideIndexes;
        public int offset;
        public int length;
        
        public List<FormationInstance> formationInstances = new List<FormationInstance>();

        public List<FormationIndexData> indexData = new List<FormationIndexData>();

        private void OnEnable()
        {
            RefreshFormationInstances();
        }

        private void OnValidate()
        {
            RefreshFormationInstances();
        }

        public void RefreshFormationInstances()
        {
            formationInstances.Clear();
            foreach (Transform o in transform)
            {
                if (o.TryGetComponent<FormationInstance>(out var formationInstance))
                {
                    formationInstances.Add(formationInstance);
                }
            }

            if (indexData == null || indexData.Count == 0) RefreshIndexes();
        }

        private void RefreshIndexes()
        {
            if (indexData == null)
            {
                indexData = new List<FormationIndexData>();
            }
            else
            {
                indexData.Clear();
            }
            
            int droneIndex = 0;
            
            for (var i = 0; i < formationInstances.Count; i++)
            {
                var formation = formationInstances[i];

                for (var j = 0; j < formation.targetFormation.points.Length; j++)
                {
                    //var point = formation.targetFormation.points[j];
                    
                    indexData.Add(new FormationIndexData(droneIndex, i, j, droneIndex)); // for now we assume point index = drone index.
                    
                    droneIndex++;
                }
            }
        }
    }
}
