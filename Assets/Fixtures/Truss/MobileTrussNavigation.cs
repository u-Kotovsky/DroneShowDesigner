using UnityEngine;

namespace Fixtures.Truss
{
    public class MobileTrussNavigation : MonoBehaviour
    {
        private enum TrussState
        {
            None,
            WaitingForTimer,
            Moving
        }
        
        private MobileTruss mobileTruss;

        private void Awake()
        {
            mobileTruss = GetComponent<MobileTruss>();
        }

        public bool playTrussPresetSwap;

        private TrussState trussState = TrussState.WaitingForTimer;
        private float trussSwapTimer;
        private int currentTrussPreset;
        private int nextTrussPreset = 1;
        
        private void Update()
        {
            if (!playTrussPresetSwap) return;
            
            switch (trussState)
            {
                case TrussState.WaitingForTimer:
                    trussSwapTimer += Time.deltaTime;
                    if (trussSwapTimer >= 2)
                    {
                        trussSwapTimer = 0;
                        trussState = TrussState.Moving;
                    }
                    break;
                case TrussState.Moving:
                    trussSwapTimer += Time.deltaTime;
                    
                    transform.localPosition = Vector3.Lerp(
                        MobileTrussPresetManager.trussPresets[currentTrussPreset][mobileTruss.index].GetPosition(), 
                        MobileTrussPresetManager.trussPresets[nextTrussPreset][mobileTruss.index].GetPosition(), 
                        Utility.MapRange(trussSwapTimer, 0, 2, 0, 1));
                
                    transform.localRotation = Quaternion.Slerp(
                        MobileTrussPresetManager.trussPresets[currentTrussPreset][mobileTruss.index].GetRotation(),
                        MobileTrussPresetManager.trussPresets[nextTrussPreset][mobileTruss.index].GetRotation(), 
                        Utility.MapRange(trussSwapTimer, 0, 2, 0, 1));
                
                    if (trussSwapTimer >= 2f)
                    {
                        trussSwapTimer = 0;
                        currentTrussPreset = nextTrussPreset;
                        nextTrussPreset++;
                        trussState = TrussState.WaitingForTimer;
                        if (nextTrussPreset >= MobileTrussPresetManager.trussPresets.Length) nextTrussPreset = 0;
                    }
                    break;
            }
        }
    }
}
