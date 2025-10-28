using UnityEngine;

namespace Fixtures.Truss
{
    public class MobileTrussNavigation : MonoBehaviour
    {
        public enum TrussState
        {
            None,
            WaitingForTimer,
            Moving
        }
        
        private MobileTruss mobileTruss;

        public bool cyclePresets = true;

        private void Awake()
        {
            mobileTruss = GetComponent<MobileTruss>();
        }

        public bool playTrussPresetSwap;

        public TrussState trussState = TrussState.WaitingForTimer;
        private float trussSwapTimer;
        private int currentTrussPreset;
        public int nextTrussPreset = 1;
        [Tooltip("How much time pass before it'll start move trusses")]
        public float timeToWait = 2;
        [Tooltip("How much time it needs to move trusses")]
        public float timeToMove = 2;
        
        private void Update()
        {
            if (!playTrussPresetSwap) return;
            
            switch (trussState)
            {
                case TrussState.WaitingForTimer:
                    trussSwapTimer += Time.deltaTime;
                    if (trussSwapTimer >= timeToWait)
                    {
                        trussSwapTimer = 0;
                        trussState = TrussState.Moving;
                    }
                    break;
                case TrussState.Moving:
                    trussSwapTimer += Time.deltaTime;
                    
                    transform.localPosition = Vector3.Lerp(
                        MobileTrussPresetManager.trussPresets[currentTrussPreset][mobileTruss.fixtureIndex].GetPosition(), 
                        MobileTrussPresetManager.trussPresets[nextTrussPreset][mobileTruss.fixtureIndex].GetPosition(), 
                        Utility.MapRange(trussSwapTimer, 0, timeToMove + 0.001f, 0, 1));
                
                    transform.localRotation = Quaternion.Slerp(
                        MobileTrussPresetManager.trussPresets[currentTrussPreset][mobileTruss.fixtureIndex].GetRotation(),
                        MobileTrussPresetManager.trussPresets[nextTrussPreset][mobileTruss.fixtureIndex].GetRotation(), 
                        Utility.MapRange(trussSwapTimer, 0, timeToMove + 0.001f, 0, 1));
                
                    if (trussSwapTimer >= timeToMove)
                    {
                        trussSwapTimer = 0;
                        currentTrussPreset = nextTrussPreset;
                        nextTrussPreset++;
                        trussState = cyclePresets ? TrussState.WaitingForTimer : TrussState.None;
                        if (nextTrussPreset >= MobileTrussPresetManager.trussPresets.Length) nextTrussPreset = 0;
                    }
                    break;
            }
        }
    }
}
