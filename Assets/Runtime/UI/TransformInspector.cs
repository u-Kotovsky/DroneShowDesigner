using System;
using System.Globalization;
using UnityEngine;

namespace Runtime.UI
{
    public static class TransformInspector
    {
        public static void OnInspector(RectTransform parent, Transform transform)
        {
            var positionInfo = UIUtility.AddItemToList(parent, 0, 15, "Position");
            var positionRoot = UIUtility.AddItemToList(parent, 0, 20);

            Color elementColor = Color.white * 0.2f;
            Color textColor = Color.white;
            
            // May not properly update origin, need to look into other solution for that.
            // TODO: fix alignment
            
            // TODO: Show if input number are not correct
            
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.x.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("X")
                .OnValueChanged(value =>
                {
                    try
                    {
                        //transform.localPosition.Set(float.Parse(value), transform.localPosition.y, transform.localPosition.z);
                        var num = float.Parse(value);
                        ApplyLocalPositionChanges(transform, num, 0);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            UIUtility.AddInputField(positionRoot, elementColor,textColor)
                .WithText(transform.localPosition.y.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("Y")
                .OnValueChanged(value =>
                {
                    try
                    {
                        //transform.localPosition.Set(transform.localPosition.x, float.Parse(value), transform.localPosition.z);
                        var num = float.Parse(value);
                        ApplyLocalPositionChanges(transform, num, 1);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.z.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("Z")
                .OnValueChanged(value =>
                {
                    try
                    {
                        //transform.localPosition.Set(transform.localPosition.x, transform.localPosition.y, float.Parse(value));
                        var num = float.Parse(value);
                        ApplyLocalPositionChanges(transform, num, 2);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            
            var rotationInfo = UIUtility.AddItemToList(parent, 0, 15, "Rotation");
            var rotationRoot = UIUtility.AddItemToList(parent, 0, 20);
            
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(transform.localRotation.eulerAngles.x.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("X")
                .OnValueChanged(value =>
                {
                    var num = float.Parse(value);
                    ApplyLocalRotationChanges(transform, num, 0);
                });
            UIUtility.AddInputField(rotationRoot, elementColor,textColor)
                .WithText(transform.localRotation.eulerAngles.y.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("Y")
                .OnValueChanged(value =>
                {
                    var num = float.Parse(value);
                    ApplyLocalRotationChanges(transform, num, 1);
                });
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(transform.localRotation.eulerAngles.z.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("Z")
                .OnValueChanged(value =>
                {
                    var num = float.Parse(value);
                    ApplyLocalRotationChanges(transform, num, 2);
                });
            
            /*Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            
            transform.GetPositionAndRotation(out position, out rotation);
            
            Vector3 eulerAngles = transform.localRotation.eulerAngles;
            // Questionable, not sure if it'll update original part of transform
            AddVector3(rect, "Position", position);
            AddVector3(rect, "Rotation", eulerAngles);*/
        }
        
        public static void OnInspector(RectTransform parent, Transform[] transforms)
        {
            var positionInfo = UIUtility.AddItemToList(parent, 0, 15, "Position");
            var positionRoot = UIUtility.AddItemToList(parent, 0, 20);

            Color elementColor = Color.white * 0.2f;
            Color textColor = Color.white;
            
            // May not properly update origin, need to look into other solution for that.
            // TODO: fix alignment
            
            // TODO: Show if input number are not correct
            
            // TODO: Look through all values of each type in each transform and if all values of one type match = put it in inputfield
            // if not, put an empty and when calcualte, empty = ignore and actual value = move obj
            
            /*UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.x.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("X")
                .OnValueChanged(value =>
                {
                    try
                    {
                        //transform.localPosition.Set(float.Parse(value), transform.localPosition.y, transform.localPosition.z);
                        var num = float.Parse(value);
                        ApplyChanges(transform, num, 0);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            UIUtility.AddInputField(positionRoot, elementColor,textColor)
                .WithText(transform.localPosition.y.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("Y")
                .OnValueChanged(value =>
                {
                    try
                    {
                        //transform.localPosition.Set(transform.localPosition.x, float.Parse(value), transform.localPosition.z);
                        var num = float.Parse(value);
                        ApplyChanges(transform, num, 1);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            UIUtility.AddInputField(positionRoot, elementColor, textColor)
                .WithText(transform.localPosition.z.ToString(CultureInfo.CurrentCulture))
                .WithPlaceholder("Z")
                .OnValueChanged(value =>
                {
                    try
                    {
                        //transform.localPosition.Set(transform.localPosition.x, transform.localPosition.y, float.Parse(value));
                        var num = float.Parse(value);
                        ApplyChanges(transform, num, 2);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });*/
            
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

        private static void ApplyLocalPositionChanges(Transform transform, float value, byte axis = 0)
        {
            switch (axis)
            {
                case 0:
                    transform.SetLocalPositionAndRotation(
                        new Vector3(value, transform.localPosition.y, transform.localPosition.z), 
                        transform.localRotation);
                    break;
                case 1:
                    transform.SetLocalPositionAndRotation(
                        new Vector3(transform.localPosition.x, value, transform.localPosition.z), 
                        transform.localRotation);
                    break;
                case 2:
                    transform.SetLocalPositionAndRotation(
                        new Vector3(transform.localPosition.x, transform.localPosition.y, value), 
                        transform.localRotation);
                    break;
            }
        }
        
        private static void ApplyLocalRotationChanges(Transform transform, float value, byte axis = 0)
        {
            switch (axis)
            {
                case 0:
                    transform.SetLocalPositionAndRotation(
                        transform.localPosition,
                        Quaternion.Euler(value, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z));
                    break;
                case 1:
                    transform.SetLocalPositionAndRotation(
                        transform.localPosition,
                        Quaternion.Euler(transform.localRotation.eulerAngles.x, value, transform.localRotation.eulerAngles.z));
                    break;
                case 2:
                    transform.SetLocalPositionAndRotation(
                        transform.localPosition,
                        Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, value));
                    break;
            }
        }
        
        private static void AddVector3(RectTransform parent, string title, Vector3 vector3)
        {
            Color elementColor = Color.white * 0.2f;
            Color textColor = Color.white;
            
            var rotationInfo = UIUtility.AddItemToList(parent, 0, 15, title);
            var rotationRoot = UIUtility.AddItemToList(parent, 0, 20);
            
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(vector3.x.ToString())
                .WithPlaceholder("X")
                .OnValueChanged(value => { vector3.Set(float.Parse(value), vector3.y, vector3.z); });
            UIUtility.AddInputField(rotationRoot, elementColor,textColor)
                .WithText(vector3.y.ToString())
                .WithPlaceholder("Y")
                .OnValueChanged(value => { vector3.Set(vector3.x, float.Parse(value), vector3.z); });
            UIUtility.AddInputField(rotationRoot, elementColor, textColor)
                .WithText(vector3.z.ToString())
                .WithPlaceholder("Z")
                .OnValueChanged(value => { vector3.Set(vector3.x, vector3.y, float.Parse(value)); });
        }
    }
}
