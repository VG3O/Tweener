//using Mono.Cecil;
//using UnityEngine;

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace VG
{
    namespace Tweener
    {
        public enum EasingStyle
        {
            Linear,
            Quadratic,
            Cubic,
            Quartic,
            Quintic,
            Sine,
            Exponential,
            Back,
            Circular,
            Elastic
        }

        public enum EasingDirection
        {
            In = 0,
            Out = 1
        }

        public class Easing
        {
            public delegate float EasingFunction(float t, bool easeIn);

            public static readonly List<EasingFunction> EasingFunctions = new List<EasingFunction>
            {
                linear,
                quad,
                cubic,
                quart,
                quint,
                sine,
                exponential,
                back,
                circular,
                elastic
            };

            private static float linear(float t, bool easeIn) { return easeIn ? t : 1 - (1 - t); }
            private static float quad(float t, bool easeIn) { return easeIn ? Mathf.Pow(t, 2) : 1 - Mathf.Pow(1 - t, 2); }
            private static float cubic(float t, bool easeIn) { return easeIn ? Mathf.Pow(t, 3) : 1 - Mathf.Pow(1 - t, 3); }
            private static float quart(float t, bool easeIn) { return easeIn ? Mathf.Pow(t, 4) : 1 - Mathf.Pow(1 - t, 4); }
            private static float quint(float t, bool easeIn) { return easeIn ? Mathf.Pow(t, 5) : 1 - Mathf.Pow(1 - t, 5); }
            private static float sine(float t, bool easeIn) { return easeIn ? (1 - Mathf.Cos((t * Mathf.PI) / 2)) : Mathf.Cos((t * Mathf.PI) / 2); }

            private static float exponential(float t, bool easeIn)
            {
                return easeIn ? (t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f))
                               : (t == 1f ? 1f : 1f - Mathf.Pow(2f, -10 * t));
            }
            private static float back(float t, bool easeIn)
            {
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                return easeIn ? (c3 * t * t * t - c1 * t * t) : 1 + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
            }
            private static float circular(float t, bool easeIn)
            {
                return easeIn ? (1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f))) : Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
            }

            private static float elastic(float t, bool easeIn)
            {
                float c4 = (2 * Mathf.PI) / 3;

                if (!easeIn)
                {
                    return t == 0
                        ? 0
                        : t == 1
                        ? 1
                        : Mathf.Pow(2f, -10 * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
                }
                else
                {
                    return t == 0
                        ? 0
                        : t == 1
                        ? 1
                        : -Mathf.Pow(2f, 10 * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
                }
            }

            public static float Interpolate(float start, float end, float time, EasingStyle ease, bool easeIn)
            {
                return Mathf.Lerp(start, end, EasingFunctions[(int)ease].Invoke(time, easeIn));
            }
        }
    }
}
