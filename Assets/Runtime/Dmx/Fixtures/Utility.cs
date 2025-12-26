using System;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Dmx.Fixtures
{
    /**
     * Thanks a lot to Micca, happyrobot33, Talos for explaining coarse & fine workflow! <3
     */

    public static class Utility
    {
        /// <summary>
        /// Map a value with min/max ranges
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromMin"></param>
        /// <param name="fromMax"></param>
        /// <param name="toMin"></param>
        /// <param name="toMax"></param>
        /// <returns></returns>
        public static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax) {
            // Clamp input to source range
            value = Mathf.Clamp(value, fromMin, fromMax);
    
            // Map from source range to 0-1
            float normalized = (value - fromMin) / (fromMax - fromMin);
    
            // Map from 0-1 to target range
            return toMin + (normalized * (toMax - toMin));
        }
    
        /// <summary>
        /// Calculates coarse from value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static byte GetCoarse(float input, float minValue, float maxValue)
        {
            // 1) normalize
            double normalized = Mathf.InverseLerp(minValue, maxValue, input);
            // 2) scale
            uint value = (uint)(normalized * ushort.MaxValue);
            // 3) get upper byte
            double coarse = value >> 8;
            // 4) return byte value
            return (byte)coarse;
        }

        /// <summary>
        /// Calculates fine from value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static byte GetFine(float input, float minValue, float maxValue)
        {
            // 1) normalize
            double normalized = Mathf.InverseLerp(minValue, maxValue, input);
            // 2) scale
            uint value = (uint)(normalized * ushort.MaxValue);
            // 3) get upper byte
            double fine = value & 0xFF;
            // 4) return byte value
            return (byte)fine;
        }
    
        /// <summary>
        /// Calculates coarse from value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte GetCoarse(float input) // input = 0 .. 1
        {
            // 1) scale
            uint value = (uint)(input * ushort.MaxValue);
            // 2) get upper byte
            double coarse = value >> 8;
            // 3) return byte value
            return (byte)coarse;
        }

        /// <summary>
        /// Calculates fine from value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte GetFine(float input) // input = 0 .. 1
        {
            // 1) scale
            uint value = (uint)(input * ushort.MaxValue);
            // 2) get upper byte
            double fine = value & 0xFF;
            // 3) return byte value
            return (byte)fine;
        }
    
        /// <summary>
        /// Calculates value by coarse, fine
        /// </summary>
        /// <param name="coarse"></param>
        /// <param name="fine"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        [Obsolete("Use GetValueFromCoarseFine", true)]
        public static float ReverseCoarseFine(byte coarse, byte fine, float minValue = -800, float maxValue = 800)
        {
            ushort fullValue = (ushort)((fine << 8) | coarse);
            float value = fullValue / (float)ushort.MaxValue;
            return Mathf.Lerp(minValue, maxValue, value);
        }
    
        /// <summary>
        /// Calculates value from coarse, fine with value remap
        /// </summary>
        /// <param name="coarse"></param>
        /// <param name="fine"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float GetValueFromCoarseFine(byte coarse, byte fine, float minValue, float maxValue)
        {
            float normalized = GetNormalizedValueFromCoarseFine(coarse, fine);
            return Mathf.Lerp(minValue, maxValue, normalized);
        }
    
        /// <summary>
        /// Calculates normalized value from coarse, fine
        /// </summary>
        /// <param name="coarse"></param>
        /// <param name="fine"></param>
        /// <returns></returns>
        public static float GetNormalizedValueFromCoarseFine(byte coarse, byte fine)
        {
            uint combinedValue = ((uint)coarse << 8) | fine;
            return combinedValue / (float)ushort.MaxValue;
        }
        
        /// <summary>
        /// Copy all DMX512 values as native array
        /// </summary>
        /// <param name="buffer"></param>
        public static void CopyDmxValuesAsArray(byte[] buffer)
        {
            GUIUtility.systemCopyBuffer = "[" + string.Join(", ", buffer) + "]";
        }

        public static byte[] GetRange(this byte[] buffer, int start, int count)
        {
            byte[] bytes = new byte[count];
                    
            Buffer.BlockCopy(buffer, start, bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// Copy DMX512 values as native array
        /// </summary>
        /// <param name="dmxData"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public static void CopyDmxValuesAsArray(byte[] dmxData, int offset, int size)
        {
            byte[] bytes = dmxData.GetRange(offset, size);

            CopyDmxValuesAsArray(bytes);
        }

        /// <summary>
        /// Copy string buffer with separator
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="separator"></param>
        public static void CopyValuesAsArray(string[] buffer, string separator = "\n")
        {
            string data = string.Join(separator, buffer); 
            GUIUtility.systemCopyBuffer = data;
        }

        /// <summary>
        /// Copy string
        /// </summary>
        /// <param name="value"></param>
        public static void CopyValue(string value)
        {
            string data = value;
            GUIUtility.systemCopyBuffer = data; // Like why the hell it copying only 3 lines and then fucks it up?
        }

        /// <summary>
        /// Copy all DMX512 data with offset as "UNVIERSE.CHANNEL VALUE"
        /// </summary>
        /// <param name="dmxData"></param>
        /// <param name="globalChannelStart"></param>
        public static void CopyAllDmxValuesAsMa3Representation(byte[] dmxData, int globalChannelStart)
        {
            int universe = (int)Mathf.Floor(globalChannelStart / 512) + 1;
            int addressStart = (globalChannelStart % 512) + 1;
            string[] values = new string[dmxData.Length];

            for (var i = 0; i < dmxData.Length; i++)
            {
                values[i] = $"{universe}.{addressStart + i} {dmxData[i]}";
            }

            CopyValuesAsArray(values);
        }

        /// <summary>
        /// Copy selected DMX512 data with offset as "UNVIERSE.CHANNEL VALUE"
        /// </summary>
        /// <param name="dmxData"></param>
        /// <param name="globalChannelStart"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public static void CopyDmxValuesWithOffsetAsMa3Representation(byte[] dmxData, int globalChannelStart, int offset, int size)
        {
            byte[] bytes = dmxData.GetRange(offset, size);
            
            int universe = (int)Mathf.Floor(globalChannelStart / 512) + 1;
            int addressStart = (globalChannelStart % 512) + 1;
            string[] values = new string[bytes.Length];
            
            for (var i = 0; i < values.Length; i++)
                values[i] = $"{universe}.{addressStart + i} {bytes[i]}";

            CopyValuesAsArray(values);
        }
        
        /// <summary>
        /// Get random position on a flat plane inside circle
        /// </summary>
        /// <param name="minDist"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public static Vector3 GetRandomXZPosition(float minDist, float maxDist)
        {
            // Generate random angle
            float angle = Random.Range(0f, 2f * Mathf.PI);
        
            // Generate random distance (excluding center radius)
            float distance = Random.Range(minDist, maxDist);
        
            float x = distance * Mathf.Cos(angle);
            float z = distance * Mathf.Sin(angle);
        
            return new Vector3(x, 0, z);
        }
        
        /// <summary>
        /// Get random position inside a sphere
        /// </summary>
        /// <param name="minDist"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public static Vector3 GetRandomSphericalPosition(float minDist, float maxDist)
        {
            Vector3 direction = Random.insideUnitSphere.normalized;
            float distance = Random.Range(minDist, maxDist);
    
            return direction * distance;
        }
        
        /// <summary>
        /// Smooth value over time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static float SmoothStep(float time, float duration)
        {
            var t = time / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="time"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static float SmoothValue01(float edge, float time, float duration)
        {
            edge = Mathf.Clamp(edge, 0, 1);
            
            // Calculate color transition progress (0 to 1) over the remaining % of duration
            float colorTransitionStart = edge * duration; // % of total duration
            float elapsedInPhase = time - colorTransitionStart; // Time spent in phase
            float phaseDuration = (1 - edge) * duration; // % of total duration

            // Clamp to prevent going over 1
            float colorT = Mathf.Clamp01(elapsedInPhase / phaseDuration);
            float smoothT = SmoothStep(colorT, 1f); // Smooth transition from 0 to 1

            return smoothT;
        }
        
        /// <summary>
        /// Find dimensions closest to square
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static (int width, int height) GetClosestToSquare(int n)
        {
            int sqrt = (int)Math.Sqrt(n);
        
            // Find the largest factor ≤ sqrt(n)
            for (int i = sqrt; i >= 1; i--)
                if (n % i == 0) return (i, n / i);
            
            return (1, n); // fallback
        }

        /// <summary>
        /// Get most rectangular (greatest aspect ratio)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static (int width, int height) GetMostRectangular(int n)
        {
            // Return the pair with smallest factor as width, largest as height
            for (int i = 1; i <= (int)Math.Sqrt(n); i++)
                if (n % i == 0) return (i, n / i); // This gives smallest × largest
            
            return (1, n);
        }
    }
}