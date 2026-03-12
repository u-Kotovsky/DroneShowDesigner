using System;
using System.Collections.Generic;
using Runtime.Dmx.Fixtures;
using Runtime.Dmx.Fixtures.Drones;
using UnityEngine;

namespace Runtime.Core.Formations
{
    public class FormationManager : MonoBehaviour
    {
        public bool enableOverrideColors = false;
        public Color overrideColor = Color.white;
        public bool localPosition = false;
    
        public List<FormationInstance> formationInstances;
    
        public List<Transform> drones;
        public List<LightingDrone> lightingDrones;

        public bool placeDrones = false;

        private void Start()
        {
            FixtureSpawnManager.Instance.OnLightingDronesEnabled += OnLightingDronesEnabled;
            FixtureSpawnManager.Instance.OnLightingDronesDisabled += OnLightingDronesDisabled;
        }

        private void OnLightingDronesDisabled(LightingDrone[] obj)
        {
            drones.Clear();
        }

        private void OnLightingDronesEnabled(LightingDrone[] obj)
        {
            drones.Clear();
            for (var i = 0; i < obj.Length; i++)
            {
                drones.Add(obj[i].transform);
                lightingDrones.Add(obj[i]);

                try
                {
                    obj[i].Color = Color.black;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void Update()
        {
            if (!placeDrones) return;
            int droneIndex = 0;
            
            for (var i = 0; i < formationInstances.Count; i++)
            {
                if (droneIndex >= drones.Count) break;
                
                var formation = formationInstances[i];
                if (formation.weight == 0) continue;
                
                for (var j = 0; j < formation.targetFormation.points.Length; j++)
                {
                    if (droneIndex >= drones.Count) break;
                    
                    var point = formation.targetFormation.points[j];
                    if (droneIndex >= drones.Count) break;
                    var drone = drones[droneIndex];
                    var ld = lightingDrones[droneIndex];

                    droneIndex++;
                    
                    if (point == null) continue;
                    
                    var position = formation.transform.TransformPoint(point.localPosition);
                    
                    if (localPosition)
                    {
                        drone.localPosition = position;
                    }
                    else
                    {
                        drone.position = position;
                    }

                    if (enableOverrideColors)
                    {
                        if (ld is not null)
                        {
                            ld.Color = overrideColor;
                        }
                    }
                }
            }
        }
    }
}
