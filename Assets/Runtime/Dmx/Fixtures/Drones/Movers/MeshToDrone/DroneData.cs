using System;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Movers.MeshToDrone
{
    [Serializable]
    public class DroneData
    {
        [SerializeField]
        private BaseDrone drone;

        [SerializeField]
        private int meshIndex;

        [SerializeField]
        private int vertexIndex;
        
        [SerializeField]
        private int globalVertexIndex;

        public BaseDrone Drone
        {
            get => drone;
            set => drone = value;
        }

        public int MeshIndex
        {
            get => meshIndex;
            private set => meshIndex = value;
        }

        public int VertexIndex
        {
            get => vertexIndex;
            private set => vertexIndex = value;
        }
        
        // Global, based on Target Index AND Vertex Indexes
        public int GlobalVertexIndex
        {
            get => globalVertexIndex;
            private set => globalVertexIndex = value;
        } 

        public void SetIndex(int mIndex, int vIndex, int globalIndex)
        {
            MeshIndex = mIndex;
            VertexIndex = vIndex;
            GlobalVertexIndex = globalIndex;
        }

        public DroneData(BaseDrone drone)
        {
            Drone = drone;
        }
    }
}
