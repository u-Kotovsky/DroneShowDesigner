using System;
using UnityEngine;

namespace Runtime.Core.Selection
{
    public enum ObjectManipulateType
    {
        None,
        Move,
        Rotate
    }

    public enum ObjectAxis
    {
        X,
        Y,
        Z,
        Screen
    }
    
    public class ObjectManipulateManager : MonoBehaviour
    {
        private ObjectManipulateType manipulateType = ObjectManipulateType.None;
        private ObjectAxis moveAxis = ObjectAxis.Screen;
        private ObjectAxis rotateAxis = ObjectAxis.Screen;

        private void Start()
        {
            FixtureSelectionManager.Instance.OnObjectDeselected += OnDeselected;
        }

        private void OnDeselected(SelectionEntry selection)
        {
            if (FixtureSelectionManager.Instance.Selection.Count <= 0) ResetManipulation();
        }

        private void ResetManipulation()
        {
            manipulateType = ObjectManipulateType.None;
            moveAxis = ObjectAxis.Screen;
            rotateAxis = ObjectAxis.Screen;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                manipulateType = manipulateType == ObjectManipulateType.Move ? ObjectManipulateType.None : ObjectManipulateType.Move;
                
                if (FixtureSelectionManager.Instance.Selection.Count <= 0) ResetManipulation();
                
                Debug.Log("Manipulation type: " + manipulateType);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                manipulateType = manipulateType == ObjectManipulateType.Rotate ? ObjectManipulateType.None : ObjectManipulateType.Rotate;
                
                if (FixtureSelectionManager.Instance.Selection.Count <= 0) ResetManipulation();
                
                Debug.Log("Manipulation type: " + manipulateType);
            }

            switch (manipulateType)
            {
                case ObjectManipulateType.Move:
                    Move();
                    break;
                case ObjectManipulateType.Rotate:
                    Rotate();
                    break;
            }
        }

        private const float speed = 0.5f;

        private void Move()
        {
            SetAxis(ref moveAxis);
            
            // TODO: once we enable manipulation, save current position into cache,
            // just in case when user wants to cancel operation, we just revert it to those positions.
            // TODO: on ESC stop manipulation and revert changes
            // TODO: enable/disable fixed step
            // TODO: let user change speed/power of movement
            // TODO: visual indication that position/rotation manipulation is on or off.

            switch (moveAxis)
            {
                case ObjectAxis.Screen:

                    break;
                case ObjectAxis.X:
                    foreach (var selection in FixtureSelectionManager.Instance.Selection)
                    {
                        selection.GameObject.transform.localPosition += Vector3.right * (Input.mousePositionDelta.x * speed);
                    }
                    break;
                case ObjectAxis.Y:
                    foreach (var selection in FixtureSelectionManager.Instance.Selection)
                    {
                        selection.GameObject.transform.localPosition += Vector3.up * (Input.mousePositionDelta.y * speed);
                    }
                    break;
                case ObjectAxis.Z:
                    foreach (var selection in FixtureSelectionManager.Instance.Selection)
                    {
                        selection.GameObject.transform.localPosition += Vector3.forward * (Input.mousePositionDelta.x * speed);
                    }
                    break;
            }
        }

        private void Rotate()
        {
            SetAxis(ref rotateAxis);
            
            switch (moveAxis)
            {
                case ObjectAxis.Screen:

                    break;
                case ObjectAxis.X:
                    
                    break;
                case ObjectAxis.Y:

                    break;
                case ObjectAxis.Z:

                    break;
            }
        }

        private void SetAxis(ref ObjectAxis axis)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                axis = axis == ObjectAxis.X ? ObjectAxis.Screen : ObjectAxis.X;
                Debug.Log(axis);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                axis = axis == ObjectAxis.Y ? ObjectAxis.Screen : ObjectAxis.Y;
                Debug.Log(axis);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                axis = axis == ObjectAxis.Z ? ObjectAxis.Screen : ObjectAxis.Z;
                Debug.Log(axis);
            }
        }
    }
}
