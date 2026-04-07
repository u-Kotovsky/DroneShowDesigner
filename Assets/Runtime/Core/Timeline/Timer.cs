using System;

namespace Runtime.Core.Timeline
{
    [Serializable]
    public class Timer
    {
        public bool IsRunning;
        public bool IsLoop;
        public float CurrentTime;
        public float MaxTime;
        public float TimeScale = 1;

        public bool IsOvertime => CurrentTime >= MaxTime;

        public void Tick(float timeStep)
        {
            
            CurrentTime += timeStep * TimeScale;
        }
    }
}
