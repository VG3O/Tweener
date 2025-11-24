using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace VG
{
    namespace Tweener
    {
        public sealed partial class Tweener
        {
            //public static Tween Custom(float start, float end, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, int repeatCount = 0)
            //{
            //    TweeningManager manager = TweeningManager.instance;
            //    TweenInfo newInfo = new TweenInfo()
            //    {
            //        duration = duration,
            //        startValue = new ValueContainer(start),
            //        endValue = new ValueContainer(end),
            //        direction = direction,
            //        easeStyle = style,
            //        repeatCount = repeatCount,
            //    };

            //    return manager.startTweenInternal(newInfo);
            //}

            /// <summary>
            /// Interpolates from the current passed shader value to the target value.
            /// </summary>
            /// <param name="mat">The material to tween.</param>
            /// <param name="shaderValue">The shader float parameter.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetValue">The value to tween to.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ShaderFloat(
                Material mat,
                string shaderValue,
                float targetValue,
                float duration,
                EasingStyle style = EasingStyle.Linear,
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f,
                float? startValue = null,
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    matShaderValue = shaderValue,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : mat.GetFloat(shaderValue)),
                    endValue = new ValueContainer(targetValue),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.ShaderFloat
                };

                return manager.startTweenInternal(mat, newInfo);
            }

            /// <summary>
            /// Interpolates from the current passed shader value to the target value.
            /// </summary>
            /// <param name="mat">The material to tween.</param>
            /// <param name="shaderValue">The shader float parameter.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetColor">The color to tween to.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ShaderColor(
                Material mat, 
                string shaderValue, 
                Color targetColor, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                Color? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    matShaderValue = shaderValue,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : mat.GetColor(shaderValue)),
                    endValue = new ValueContainer(targetColor),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.ShaderColor
                };

                return manager.startTweenInternal(mat, newInfo);
            }

            /// <summary>
            /// Tweens a material's color to the target value.
            /// </summary>
            /// <param name="mat">The material to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetColor">The color to tween to.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween MaterialColor(
                Material mat, 
                Color targetColor, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f, 
                Color? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : mat.color),
                    endValue = new ValueContainer(targetColor),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.MaterialColor
                };

                return manager.startTweenInternal(mat, newInfo);
            }

            /// <summary>
            /// Tweens a shader float global.
            /// </summary>
            /// <param name="varName">The global variable to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target value.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ShaderGlobalFloat(
                string varName, 
                float target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                float? startValue = null,
                bool reverses = false,
                float reverseDuration = -1f, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : Shader.GetGlobalFloat(varName)),
                    endValue = new ValueContainer(target),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    matShaderValue = varName,
                    updateType = updateType,
                    updater = manager.ShaderGlobalFloat
                };

                return manager.startTweenInternal(null, newInfo);
            }

            /// <summary>
            /// Tweens a shader vector global.
            /// </summary>
            /// <param name="varName">The global variable to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target value.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ShaderGlobalVector3(
                string varName,
                Vector3 target,
                float duration,
                EasingStyle style = EasingStyle.Linear,
                EasingDirection direction = EasingDirection.Out,
                Vector3? startValue = null,
                bool reverses = false,
                float reverseDuration = -1f,
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : (Vector3)Shader.GetGlobalVector(varName)),
                    endValue = new ValueContainer(target),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    matShaderValue = varName,
                    updateType = updateType,
                    updater = manager.ShaderGlobalVector3
                };

                return manager.startTweenInternal(null, newInfo);
            }

            /// <summary>
            /// Tweens an image's color to a target value.
            /// </summary>
            /// <param name="image">The image to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetColor">The target color that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ImageColor(
                Image image, 
                Color targetColor, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f,
                Color? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : image.color),
                    endValue = new ValueContainer(targetColor),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.ImageColor
                };

                return manager.startTweenInternal(image, newInfo);
            }

            /// <summary>
            /// Tweens a text's color to a target value.
            /// </summary>
            /// <param name="text">The text to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetColor">The target color that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween TextColor(
                TMP_Text text, 
                Color targetColor, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f, 
                Color? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : text.color),
                    endValue = new ValueContainer(targetColor),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.TextColor
                };

                return manager.startTweenInternal(text, newInfo);
            }

            /// <summary>
            /// Tweens a slider's value to the target specified.
            /// </summary>
            /// <param name="slider">The slider to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetValue">The target value that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween SliderValue(
                Slider slider, 
                float targetValue, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                float? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : slider.value),
                    endValue = new ValueContainer(targetValue),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.SliderValue
                };

                return manager.startTweenInternal(slider, newInfo);
            }
            /// <summary>
            /// Interpolates between the current position and target position.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween Position(
                GameObject targetObject, 
                Vector3 target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                Vector3? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.position),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.Position
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current rotation and target rotation.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween Rotation(
                GameObject targetObject, 
                Quaternion target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                Quaternion? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.rotation),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.Rotation
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current rotation and target rotation.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween LocalRotation(
                GameObject targetObject, 
                Quaternion target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f, 
                Quaternion? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.localRotation),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.LocalRotation
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current scale and target scale.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target scale that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween LocalScale(
                GameObject targetObject, 
                Vector3 target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f, 
                Vector3? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.localScale),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.LocalScale
                };

                return manager.startTweenInternal(targetObject, newInfo);

            }

            /// <summary>
            /// Interpolates between the current position and target position.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween LocalPosition(
                GameObject targetObject, 
                Vector3 target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                Vector3? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.localPosition),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.LocalPosition
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Tweens a canvas group's alpha from one value to the other.
            /// </summary>
            /// <param name="targetObject">The canvas group to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target alpha that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween CanvasGroupAlpha(
                CanvasGroup targetObject, 
                float target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                float? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.alpha),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.CanvasGroupAlpha
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current direction and target direction.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target direction that should be hit. This should be a normalized vector.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween TransformUp(
                GameObject targetObject, 
                Vector3 target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f, 
                Vector3? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.up),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.TransformUp
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current FOV and target FOV.
            /// </summary>
            /// <param name="cam">The object to tween.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="target">The target FOV that should be hit.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="startValue">(OPTIONAL) Overrides the starting value of the tween.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween CameraFOV(
                CinemachineCamera cam, 
                float target, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out,
                bool reverses = false,
                float reverseDuration = -1f, 
                float? startValue = null, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : cam.Lens.FieldOfView),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    updateType = updateType,
                    updater = manager.CameraFOV
                };

                return manager.startTweenInternal(cam, newInfo);
            }

            /// <summary>
            /// Tweens between two bezier curve knots.
            /// </summary>
            /// <param name="container">The container of splines.</param>
            /// <param name="start">The starting knot.</param>
            /// <param name="end">The ending knot.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="splineIndex">The spline's index within the container.</param>
            /// <param name="knotIndex">The knot's index within the spline.</param>
            /// <param name="style">(Default: Linear) The easing style to use.</param>
            /// <param name="direction">(Default: Out) The direction to ease with.</param>
            /// <param name="reverseDuration">(OPTIONAL) How long a reversed tween should last, leave at -1 for the same duration as the tween</param>
            /// <param name="reverses">(OPTIONAL) Reverses the tween after it first completes.</param>
            /// <param name="repeatCount">(OPTIONAL) How many times to repeat the tween after completion.</param>
            /// <param name="updateType">(OPTIONAL) Which loop will handle this tween.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween BezierKnotPosition(
                SplineContainer container, 
                int splineIndex, 
                int knotIndex, 
                BezierKnot start, 
                BezierKnot end, 
                float duration, 
                EasingStyle style = EasingStyle.Linear, 
                EasingDirection direction = EasingDirection.Out, 
                bool reverses = false,
                float reverseDuration = -1f, 
                int repeatCount = 0,
                UpdateType updateType = UpdateType.Update
            )
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(start),
                    endValue = new ValueContainer(end),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverses,
                    reverseDuration = (reverses && reverseDuration == -1f) ? duration : reverseDuration,
                    repeatCount = repeatCount,
                    splineIndex = splineIndex,
                    knotIndex = knotIndex,
                    updateType = updateType,
                    updater = manager.BezierKnotPosition
                };

                return manager.startTweenInternal(container, newInfo);
            }

            /// <summary>
            /// Shakes the passed game object based on the settings provided.
            /// </summary>
            /// <param name="targetObject">The object to shake.</param>
            /// <param name="duration">How long the shake will last.</param>
            /// <param name="positionInfluence">The maximum value of the positional shake @ magnitude = 1.</param>
            /// <param name="rotationInfluence">The maximum value of the rotational shake @ magnitude = 1, uses euler angles</param>
            /// <param name="useUnscaledTime">Whether or not you want the shake to run without time scale.</param>
            public static void Shake(GameObject targetObject, float duration, Vector3 positionInfluence, Vector3 rotationInfluence = default(Vector3), bool useUnscaledTime = false)
            {
                TweeningManager manager = TweeningManager.instance;

                manager.Shake(targetObject, duration, positionInfluence, rotationInfluence, useUnscaledTime);
            }

            /// <summary>
            /// Shakes the rotation of a cinemachine recomposer attached onto a cinemachine camera.
            /// </summary>
            /// <param name="target">The recomposer to shake.</param>
            /// <param name="duration">How long the shake will last.</param>
            /// <param name="rotationInfluence">The maximum value of the rotational shake @ magnitude = 1, uses euler angles.</param>
            /// <param name="useUnscaledTime">Whether or not you want the shake to run without time scale.</param>
            public static void CinemachineRecomposerShake(CinemachineRecomposer target, float duration, Vector3 rotationInfluence = default(Vector3), bool useUnscaledTime = false)
            {
                TweeningManager manager = TweeningManager.instance;

                manager.CinemachineRecomposerShake(target, duration, rotationInfluence, useUnscaledTime);
            }
        }
    }
}