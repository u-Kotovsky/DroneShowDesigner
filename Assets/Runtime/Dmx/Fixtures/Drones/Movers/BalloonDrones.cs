using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Dmx.Fixtures.Drones
{
    public enum BalloonDroneState
    {
        None,
        Rising,
        Descending
    }

    public class BalloonDroneData
    {
        public BalloonDroneData(LightingDrone drone)
        {
            State = BalloonDroneState.None;
            
            LockOn(drone);
        }
        
        public BalloonDroneState State;

        public float Timer;
        
        public float RisingDuration = 5;
        public float DescendingDuration = 5;

        public Color RisingColor = Color.black;
        public Color DescendingColor = Color.black;
        
        public Vector3 StartPosition = new(0, 0, 0);
        public Vector3 EndPosition = new(0, 0, 0);
        
        public LightingDrone Drone;
        public Transform Transform;

        public void LockOn(LightingDrone drone)
        {
            Drone = drone;
            Transform = drone.transform;
        }
    }

    public class BalloonDrones : MonoBehaviour
    {
        public BalloonDroneData[] Data;
        
        public void Setup(LightingDrone[] drones)
        {
            Data = new BalloonDroneData[drones.Length];

            for (var i = 0; i < Data.Length; i++)
            {
                var data = new BalloonDroneData(drones[i]);
                
                var dist = Utility.GetRandomXZPosition(16, 50);
                data.StartPosition = dist;
                data.EndPosition = dist + Vector3.up * 50f;
                
                data.RisingDuration = 30;
                data.DescendingDuration = 8;
                data.RisingColor = Color.orangeRed;
                data.State = BalloonDroneState.Rising;
                data.Timer = Random.Range(0f, data.RisingDuration);
                data.Drone.ForceSetColor(data.RisingColor);

                Data[i] = data;
            }
        }

        private void UpdateRising(BalloonDroneData data)
        {
            float t = Utility.SmoothStep(data.Timer, data.RisingDuration);

            data.Transform.localPosition = Vector3.Lerp(data.StartPosition, data.EndPosition, t);
            data.Timer += Time.deltaTime;
                    
            // Lerp color - calculate separate timer for color transition
            if (data.Timer / data.RisingDuration > 0.7f) // Start color transition at 70%
            {
                data.Drone.ForceSetColor(Color.Lerp(data.RisingColor, data.DescendingColor, 
                    Utility.SmoothValue01(0.8f, data.Timer, data.RisingDuration)));
            }

            // Next state
            if (data.Timer >= data.RisingDuration) NextState(data);
        }

        private void UpdateDescending(BalloonDroneData data)
        {
            float t = Utility.SmoothStep(data.Timer, data.DescendingDuration);

            data.Transform.localPosition = Vector3.Lerp(data.EndPosition, data.StartPosition, t);
            data.Timer += Time.deltaTime;

            // Lerp color - calculate separate timer for color transition
            if (data.Timer / data.DescendingDuration > 0.7f) // Start color transition at %
            {
                data.Drone.ForceSetColor(Color.Lerp(data.DescendingColor, data.RisingColor, 
                    Utility.SmoothValue01(0.8f, data.Timer, data.DescendingDuration)));
            }

            // Next state
            if (data.Timer >= data.DescendingDuration) NextState(data);
        }

        private void NextState(BalloonDroneData data)
        {
            data.Timer = 0;
            
            switch (data.State)
            {
                case BalloonDroneState.Rising:
                    data.State = BalloonDroneState.Descending;
                    data.Drone.ForceSetColor(data.DescendingColor);
                    break;
                case BalloonDroneState.Descending:
                    data.State = BalloonDroneState.Rising;
                    data.Drone.ForceSetColor(data.RisingColor);
                    break;
            }
        }

        private void Update()
        {
            if (Data == null) return;

            foreach (var data in Data)
            {
                switch (data.State)
                {
                    case BalloonDroneState.Rising:
                        UpdateRising(data);
                        break;
                    case BalloonDroneState.Descending:
                        UpdateDescending(data);
                        break;
                    case BalloonDroneState.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}