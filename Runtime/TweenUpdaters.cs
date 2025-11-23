using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace VG
{
    namespace Tweener
    {
        internal sealed partial class TweeningManager : MonoBehaviour
        {
            // Shake methods
            internal void Shake(GameObject targetObject, float duration, Vector3 positionInfluence, Vector3 rotationInfluence, bool useUnscaledTime)
            {
                StartCoroutine(Shaker.Co_PerformShake(targetObject, duration, positionInfluence, rotationInfluence, useUnscaledTime));
            }

            internal void CinemachineRecomposerShake(CinemachineRecomposer target, float duration, Vector3 rotationInfluence, bool useUnscaledTime)
            {
                StartCoroutine(Shaker.Co_PerformRotShakeCinemachine(target, duration, rotationInfluence, useUnscaledTime));
            }

            // Internal Updater methods (delegates)
            internal void Custom(TweenObject obj, float interpolationFactor) { }
            internal void Position(TweenObject obj, float interpolationFactor)
            {
                GameObject tweenTarget = obj._object as GameObject;
                tweenTarget.transform.position = Vector3.LerpUnclamped(obj.start._vec3, obj.end._vec3, interpolationFactor);
            }

            internal void Rotation(TweenObject obj, float interpolationFactor)
            {
                GameObject tweenTarget = obj._object as GameObject;
                tweenTarget.transform.rotation = Quaternion.LerpUnclamped(obj.start._quat, obj.end._quat, interpolationFactor);
            }

            internal void LocalPosition(TweenObject obj, float interpolationFactor)
            {
                GameObject tweenTarget = obj._object as GameObject;
                tweenTarget.transform.localPosition = Vector3.LerpUnclamped(obj.start._vec3, obj.end._vec3, interpolationFactor);
            }

            internal void LocalRotation(TweenObject obj, float interpolationFactor)
            {
                GameObject tweenTarget = obj._object as GameObject;
                tweenTarget.transform.localRotation = Quaternion.LerpUnclamped(obj.start._quat, obj.end._quat, interpolationFactor);
            }

            internal void TransformUp(TweenObject obj, float interpolationFactor)
            {
                GameObject tweenTarget = obj._object as GameObject;
                tweenTarget.transform.up = Vector3.LerpUnclamped(obj.start._vec3, obj.end._vec3, interpolationFactor);
            }

            internal void CameraFOV(TweenObject obj, float interpolationFactor)
            {
                CinemachineCamera cam = obj._object as CinemachineCamera;
                cam.Lens.FieldOfView = Mathf.LerpUnclamped(obj.start._number, obj.end._number, interpolationFactor);
            }

            internal void BezierKnotPosition(TweenObject obj, float interpolationFactor)
            {
                SplineContainer splineContainer = obj._object as SplineContainer;
                BezierKnot knot = splineContainer.Splines[obj.splineIndex][obj.knotIndex];
                knot.Position = Vector3.LerpUnclamped(obj.start._knot.Position, obj.end._knot.Position, interpolationFactor);
                knot.Rotation = Quaternion.LerpUnclamped(obj.start._knot.Rotation, obj.end._knot.Rotation, interpolationFactor);
                knot.TangentIn = Vector3.LerpUnclamped(obj.start._knot.TangentIn, obj.end._knot.TangentIn, interpolationFactor);
                knot.TangentOut = Vector3.LerpUnclamped(obj.start._knot.TangentOut, obj.end._knot.TangentOut, interpolationFactor);
                splineContainer.Splines[obj.splineIndex].SetKnot(obj.knotIndex, knot);
            }

            internal void LocalScale(TweenObject obj, float interpolationFactor)
            {
                GameObject tweenTarget = obj._object as GameObject;
                tweenTarget.transform.localScale = Vector3.LerpUnclamped(obj.start._vec3, obj.end._vec3, interpolationFactor);
            }

            internal void ShaderFloat(TweenObject obj, float interpolationFactor)
            {
                Material mat = obj._object as Material;
                mat.SetFloat(obj.matShaderValue, Mathf.LerpUnclamped(obj.start._number, obj.end._number, interpolationFactor));
            }

            internal void ShaderColor(TweenObject obj, float interpolationFactor)
            {
                Material mat = obj._object as Material;
                mat.SetColor(obj.matShaderValue, Color.LerpUnclamped(obj.start._color, obj.end._color, interpolationFactor));
            }

            internal void ShaderGlobalFloat(TweenObject obj, float interpolationFactor)
            {
                Shader.SetGlobalFloat(obj.matShaderValue, Mathf.LerpUnclamped(obj.start._number, obj.end._number, interpolationFactor));
            }

            internal void ShaderGlobalVector(TweenObject obj, float interpolationFactor)
            {
                Shader.SetGlobalVector(obj.matShaderValue, Vector3.LerpUnclamped(obj.start._vec3, obj.end._vec3, interpolationFactor));
            }

            internal void MaterialColor(TweenObject obj, float interpolationFactor)
            {
                Material mat = obj._object as Material;
                mat.color = Color.LerpUnclamped(obj.start._color, obj.end._color, interpolationFactor);
            }

            internal void GlobalMatVar(TweenObject obj, float interpolationFactor)
            {
                float value = Mathf.LerpUnclamped(obj.start._number, obj.end._number, interpolationFactor);
                Shader.SetGlobalFloat(obj.matShaderValue, value);
            }

            internal void ImageColor(TweenObject obj, float interpolationFactor)
            {
                Image image = obj._object as Image;
                image.color = Color.LerpUnclamped(obj.start._color, obj.end._color, interpolationFactor);
            }

            internal void TextColor(TweenObject obj, float interpolationFactor)
            {
                TMP_Text text = obj._object as TMP_Text;
                text.color = Color.LerpUnclamped(obj.start._color, obj.end._color, interpolationFactor);
            }

            internal void SliderValue(TweenObject obj, float interpolationFactor)
            {
                Slider slider = obj._object as Slider;
                slider.value = Mathf.LerpUnclamped(obj.start._number, obj.end._number, interpolationFactor);
            }
            
            internal void CanvasGroupAlpha(TweenObject obj, float interpolationFactor)
            {
                CanvasGroup group = obj._object as CanvasGroup;
                group.alpha = Mathf.LerpUnclamped(obj.start._number, obj.end._number, interpolationFactor);
            }
        }
    }
}
