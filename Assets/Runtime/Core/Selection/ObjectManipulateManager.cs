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
        
        private void Awake()
        {
            
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                manipulateType = manipulateType == ObjectManipulateType.Move ? ObjectManipulateType.None : ObjectManipulateType.Move;
                
                if (FixtureSelectionManager.Instance.Selection.Count <= 0) manipulateType = ObjectManipulateType.None;
                
                Debug.Log(manipulateType);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                manipulateType = manipulateType == ObjectManipulateType.Rotate ? ObjectManipulateType.None : ObjectManipulateType.Rotate;
                
                if (FixtureSelectionManager.Instance.Selection.Count <= 0) manipulateType = ObjectManipulateType.None;
                
                Debug.Log(manipulateType);
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

        private void Move()
        {
            SetAxis(ref moveAxis);

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
