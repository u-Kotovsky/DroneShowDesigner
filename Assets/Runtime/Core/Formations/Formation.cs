using System;
using UnityEngine;

namespace Runtime.Core.Formations
{
    public class Formation : MonoBehaviour
    {
        public Transform[] points;
        public Color[] colors;

        public void DeletePoints()
        {
            foreach (var t in points)
            {
                try // if we can't destroy it, skip it.
                {
                    if (t == null) continue;
                    if (Application.isPlaying)
                    {
                        Destroy(t.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(t.gameObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}