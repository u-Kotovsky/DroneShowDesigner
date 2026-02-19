using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Movers.MeshToDrone
{
    public class DroneFromMesh : MonoBehaviour
    {
        // I wrote this during 2-4 am on busy week, don't mind me for errors here xd
        [SerializeField] 
        private List<MeshTarget> targets = new();
        public List<MeshTarget> Targets
        {
            get => targets;
            set
            {
                targets = value;
                OnTargetsChanged();
            }
        }

        [SerializeField]
        public List<DroneData> drones = new();
        
        // TargetIndex, (VertexIndex, DroneIndex)
        private Dictionary<int, Dictionary<int, int>> dataPath = new();

        private bool isAllDronesReady = false;

        private void OnTargetsChanged()
        {
            var paths = new Dictionary<int, Dictionary<int, int>>();

            for (var i = 0; i < Targets.Count; i++)
            {
                var vertexToDrone = new Dictionary<int, int>();
                for (var j = 0; j < Targets[i].Vertices.Length; j++)
                {
                    vertexToDrone.Add(j, -1); // by default no drones assigned are -1
                }
                
                paths.Add(i, vertexToDrone);
            }
            
            dataPath = paths;
            
            if (isAllDronesReady) SetVertexForDrones();
        }

        public void OnAllDronesReady(LightingDrone[] pool)
        {
            foreach (var drone in pool)
            {
                var data = new DroneData(drone);
                drones.Add(data);
            }

            SetVertexForDrones();
            isAllDronesReady = true;
        }

        private void SetVertexForDrones()
        {
            int globalIndex = 0;
            
            for (var i = 0; i < dataPath.Count; i++)
            {
                var vertices = dataPath[i];

                for (var j = 0; j < vertices.Count; j++)
                {
                    //var droneId = vertices[j]; // Drone ID

                    //if (droneId != -1)
                    //{
                        // Vertex is taken by drone.
                    //}

                    drones[globalIndex].SetIndex(i, j, globalIndex);
                    dataPath[i][j] = globalIndex;

                    globalIndex++;
                }
            }
            
            //Debug.Log("SetVertexForDrones last: " + globalIndex);
        }

        private KeyValuePair<int, Dictionary<int, int>> GetTarget(int droneIndex)
        {
            foreach (var pair in dataPath.Where(pair => pair.Value.Any(pair2 => pair2.Value == droneIndex)))
            {
                return pair;
            }

            return new KeyValuePair<int, Dictionary<int, int>>(-1, null);
        }

        private KeyValuePair<int, int> GetVertex(int droneIndex)
        {
            foreach (var pair2 in dataPath.SelectMany(pair => pair.Value.Where(pair2 => pair2.Value == droneIndex)))
            {
                return pair2;
            }

            return new KeyValuePair<int, int>(-1, -1);
        }

        public void SetTransform(LightingDrone drone)
        {
            var targetPath = GetTarget(drone.fixtureIndex);
            var vertexPath = GetVertex(drone.fixtureIndex);

            if (targetPath.Key == -1 || vertexPath.Value == -1) return;

            var target = Targets[targetPath.Key]; // Null
            var vertex = target.Vertices[vertexPath.Key];
            var worldVertex = target.Renderer.transform.TransformPoint(vertex);
            drone.transform.position = worldVertex;
            
            var data = drones[drone.fixtureIndex];
            drone.Color = target.GetColorByVertex(data.VertexIndex);
        }

        #region Unity event methods
        private void Awake()
        {
            OnTargetsChanged();
            foreach (var target in Targets)
            {
                target.Setup();
            }
        }

        private void Start()
        {
            OnTargetsChanged();
        }
        #endregion
    }
}
