using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace VG
{
    namespace Tweener
    {
        public class Shaker
        {
            public static IEnumerator Co_PerformShake(GameObject target, float duration, Vector3 positionInfluence, Vector3 rotationInfluence, bool useUnscaledTime)
            {
                Vector3 startPosition = Vector3.zero;
                Vector3 startRotation = Vector3.zero;
                float magnitude = 1f;
                float t = 0f;
                while (t < duration)
                {
                    Vector3 shakeVec = new Vector3((Random.value - 0.5f) * positionInfluence.x * magnitude,
                                                   (Random.value - 0.5f) * positionInfluence.y * magnitude,
                                                   (Random.value - 0.5f) * positionInfluence.z * magnitude);

                    Vector3 shakeRotVec = new Vector3((Random.value - 0.5f) * rotationInfluence.x * magnitude,
                                                      (Random.value - 0.5f) * rotationInfluence.y * magnitude,
                                                      (Random.value - 0.5f) * rotationInfluence.z * magnitude);

                    if (target == null) yield break;

                    target.transform.localPosition = startPosition + shakeVec;
                    target.transform.localRotation = Quaternion.Euler(startRotation + shakeRotVec);

                    magnitude = Tweener.GetValue(1f - (t / duration), EasingStyle.Quintic, EasingDirection.In);
                    t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }
                if (target == null) yield break;
                target.transform.localPosition = startPosition;
                target.transform.localRotation = Quaternion.Euler(startRotation);
            }

            public static IEnumerator Co_PerformRotShakeCinemachine(CinemachineRecomposer target, float duration, Vector3 rotationInfluence, bool useUnscaledTime)
            {
                Vector3 startRotation = Vector3.zero;
                float magnitude = 1f;
                float t = 0f;
                while (t < duration)
                {
                    Vector3 shakeRotVec = new Vector3((Random.value - 0.5f) * rotationInfluence.x * magnitude,
                                                      (Random.value - 0.5f) * rotationInfluence.y * magnitude,
                                                      (Random.value - 0.5f) * rotationInfluence.z * magnitude);

                    if (target == null) yield break;

                    target.Tilt = startRotation.x + shakeRotVec.x;
                    target.Pan = startRotation.y + shakeRotVec.y;
                    target.Dutch = startRotation.z + shakeRotVec.z;


                    magnitude = Tweener.GetValue(1f - (t / duration), EasingStyle.Quintic, EasingDirection.In);
                    t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    yield return null;
                }
                if (target == null) yield break;
                target.Tilt = startRotation.x;
                target.Pan = startRotation.y;
                target.Dutch = startRotation.z;
            }
        }
    }
}