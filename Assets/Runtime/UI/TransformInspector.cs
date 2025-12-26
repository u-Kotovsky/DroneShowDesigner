using Runtime.Dmx.Fixtures.Truss;
using UnityEngine;

namespace Runtime.UI
{
    public static class TransformInspector
    {
        public static void OnInspector(RectTransform parent, Transform transform)
        {
            AddTransformControls(parent, transform);
        }
        
        private static void AddTransformControls(RectTransform parent, Transform transform)
        {
            var positionInfo = UIUtility.AddItemToList(parent, 0, 15, "Position");
            var positionRoot = UIUtility.AddItemToList(parent, 0, 20);

            Color elementColor = Color.white * 0.2f;
            Color textColor = Color.white;
            
            // May not properly update origin, need to look into other solution for that.
            // TODO: fix alignment
            
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.x.ToString())
                .WithPlaceholder("X")
                .OnValueChanged(value => { transform.localPosition.Set(float.Parse(value), transform.localPosition.y, transform.localPosition.z); });
            UIUtility.AddInputField(positionRoot, elementColor,textColor)
                .WithText(transform.localPosition.y.ToString())
                .WithPlaceholder("Y")
                .OnValueChanged(value => { transform.localPosition.Set(transform.localPosition.x, float.Parse(value), transform.localPosition.z); });
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.z.ToString())
                .WithPlaceholder("Z")
                .OnValueChanged(value => { transform.localPosition.Set(transform.localPosition.x, transform.localPosition.y, float.Parse(value)); });
            
            /*var rotationInfo = UIUtility.AddItemToList(parent, 0, 15, "Rotation");
            var rotationRoot = UIUtility.AddItemToList(parent, 0, 20);
            
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(transform.localRotation.eulerAngles.x.ToString())
                .WithPlaceholder("X")
                .OnValueChanged(value => { transform.localRotation.eulerAngles.Set(float.Parse(value), transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z); });
            UIUtility.AddInputField(rotationRoot, elementColor,textColor)
                .WithText(transform.localRotation.eulerAngles.y.ToString())
                .WithPlaceholder("Y")
                .OnValueChanged(value => { transform.localRotation.eulerAngles.Set(transform.localRotation.eulerAngles.x, float.Parse(value), transform.localRotation.eulerAngles.z); });
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(transform.localRotation.eulerAngles.z.ToString())
                .WithPlaceholder("Z")
                .OnValueChanged(value => { transform.localRotation.eulerAngles.Set(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, float.Parse(value)); });*/
            
            
            /*Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            
            transform.GetPositionAndRotation(out position, out rotation);
            
            Vector3 eulerAngles = transform.localRotation.eulerAngles;
            // Questionable, not sure if it'll update original part of transform
            AddVector3(rect, "Position", position);
            AddVector3(rect, "Rotation", eulerAngles);*/
        }
    }
}
