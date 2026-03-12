using System.Collections.Generic;
using UnityEngine;
using Runtime.Core.Formations;

namespace Runtime.Core.Timeline
{
    /**
     * A component that calculates drone positions from keyframes.
     */
    public class Timeline : MonoBehaviour
    {
        public FormationManager formationManager;
        
        public List<TimelineKeyFrame> keyframes = new List<TimelineKeyFrame>();
        
        public bool isRunning = false;
        public float time = 0;
        public float timeScale = 1f;
        
        public void OnEnable()
        {
            RefreshKeyframes();
        }
        
        public void OnValidate()
        {
            RefreshKeyframes();
        }

        private void RefreshKeyframes()
        {
            keyframes.Clear();
            foreach (Transform o in transform)
            {
                if (o.TryGetComponent<TimelineKeyFrame>(out var keyframe))
                {
                    keyframes.Add(keyframe);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!isRunning) return;

            CalculateTimeline(Time.fixedDeltaTime);
        }
        
        private Vector3[] dronePositions;
        public void CalculateTimeline(float timeStep)
        {
            time += timeStep * timeScale;

            if (!GetTargetKeyframeIndex(out int leftIndex, out int rightIndex))
                return;
            
            var leftKeyframe = keyframes[leftIndex];
            var rightKeyframe = keyframes[rightIndex];
            
            float leftEffective = leftKeyframe.time + leftKeyframe.delay;
            float rightEffective = rightKeyframe.time;
            float t = 0;
            if (leftIndex != rightIndex && rightEffective > leftEffective)
            {
                t = Mathf.Clamp01((time - leftEffective) / (rightEffective - leftEffective));
            }

            var leftPoints = new Dictionary<int, (Transform formationTransform, Vector3 localPos)>();
            var rightPoints = new Dictionary<int, (Transform formationTransform, Vector3 localPos)>();
            
            foreach (var data in leftKeyframe.indexData)
            {
                var formation = leftKeyframe.formationInstances[data.formationInstanceIndex];
                var point = formation.targetFormation.points[data.formationPointIndex].localPosition;
                leftPoints[data.globalPointIndex] = (formation.transform, point);
            }
            
            foreach (var data in rightKeyframe.indexData)
            {
                var formation = rightKeyframe.formationInstances[data.formationInstanceIndex];
                var point = formation.targetFormation.points[data.formationPointIndex].localPosition;
                rightPoints[data.globalPointIndex] = (formation.transform, point);
            }
            
            for (var i = 0; i < formationManager.drones.Count; i++)
            {
                var drone = formationManager.drones[i];
                if (drone == null) 
                    continue;
                
                var ld = formationManager.lightingDrones[i];

                var hasLeft = leftPoints.TryGetValue(i, out var leftData);
                var hasRight = rightPoints.TryGetValue(i, out var rightData);
                
                // TODO: separate color state
                ld.Color = t == 0 ? Color.white * 0.5f : Color.black;
                
                Vector3 targetPosition;
                if (hasLeft && hasRight)
                {
                    Vector3 leftWorld = leftData.formationTransform.TransformPoint(leftData.localPos);
                    Vector3 rightWorld = rightData.formationTransform.TransformPoint(rightData.localPos);
                    targetPosition = Vector3.Lerp(leftWorld, rightWorld, t);
                }
                else if (hasLeft)
                {
                    targetPosition = leftData.formationTransform.TransformPoint(leftData.localPos);
                }
                else if (hasRight)
                {
                    targetPosition = rightData.formationTransform.TransformPoint(rightData.localPos);
                }
                else
                {
                    continue;
                }
                
                drone.localPosition = targetPosition;
            }
            
            // Clear things
            leftPoints.Clear();
            rightPoints.Clear();
        }

        private bool GetTargetKeyframeIndex(out int left, out int right)
        {
            left = -1;
            right = -1;
            
            if (keyframes.Count == 0)
                return false;
            
            // Find the last keyframe
            int currentIdx = -1;
            for (int i = 0; i < keyframes.Count; i++)
            {
                if (keyframes[i].time <= time)
                    currentIdx = i;
            }

            if (currentIdx == -1)
            {
                left = 0;
                right = 0;
                return true;
            }

            float holdEnd = keyframes[currentIdx].time + keyframes[currentIdx].delay;
            if (time <= holdEnd) // Hold
            {
                left = currentIdx;
                right = currentIdx;
            }
            else if (currentIdx < keyframes.Count - 1) // Moving to next keyframe
            {
                left = currentIdx;
                right = currentIdx + 1;
            }
            else // Hold last keyframe
            {
                left = keyframes.Count - 1;
                right = keyframes.Count - 1;
            }
            
            return true;
        }
    }
}
