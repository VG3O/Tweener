using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace VG
{
    namespace Tweener
    {
        internal delegate void TweenUpdater(TweenObject obj, float interpolationFactor);

        // Singleton that creates a pool of tweening objects
        // All tweens in the pool are assigned an id at creation, which tells them where to go in the pool after they are freed.
        // Useful for returning internal information about tween objects through a struct.
        internal sealed partial class TweeningManager : MonoBehaviour
        {
            private static TweeningManager _instance = null;

            internal static TweeningManager instance // only needs to be accessed by tween class
            {
                get
                {
                    return _instance;
                }
            }

            // data
            private int _poolSize = 256;
            private long _internalIdCounter = 1;  
            internal Stack<TweenObject> _tweenPool = new Stack<TweenObject>();
            [SerializeField] internal List<TweenObject> _activeTweens = new List<TweenObject>();

            internal bool _initialized = false;

            #region Tween Pool Management
            internal static void init()
            {
                if (_instance == null)
                {
                    _instance = new GameObject("VG Tweening Manager").AddComponent<TweeningManager>();
                    DontDestroyOnLoad(_instance);
                }
                else
                {
                    return;
                }

                if (_instance._initialized) return;

                _instance.fillPool();
                _instance._initialized = true;
            }

            private void fillPool()
            {
                for (int i = 0; i < _poolSize >> 1; i++)
                {
                    _tweenPool.Push(new TweenObject());
                }
            }

            // this should never need to be used if enough tweens are present in the pool
            private void allocPool()
            {
                //Assert.IsNotNull(_tweenPool, "Reallocation attempted before pool was created.");
                Debug.LogWarning("Pool size was exceeded, please increase pool size to prevent runtime re-allocation!");
                _poolSize <<= 1;
                fillPool();
            }
            #endregion

            #region Internal Methods

            /// <summary>
            /// Starts a new tween with the settings provided.
            /// </summary>
            /// <param name="targetObject">The gameobject to target</param>
            /// <param name="settings">The settings provided from the creation function.</param>
            /// <returns>A tween struct for the user to track.</returns>
            internal Tween startTweenInternal(UnityEngine.Object targetObject, TweenInfo settings)
            {
                if (_tweenPool.Count == 0) allocPool();

                // pull from pool
                TweenObject obj = _tweenPool.Pop();

                obj.inUse = true;
                obj.id = _internalIdCounter++;
                obj._object = targetObject;
                obj.duration = settings.duration;
                obj.initialDuration = settings.duration;
                obj.useUnscaledTime = settings.useUnscaledTime;
                obj.reverses = settings.reverses;
                obj.reverseDuration = settings.reverseDuration;
                obj.repeatCount = settings.repeatCount;
                obj.start = settings.startValue;
                obj.end = settings.endValue;
                obj.updateFnc = settings.updater;
                obj.easeDirection = settings.direction;
                obj.easeStyle = settings.easeStyle;
                obj.matShaderValue = settings.matShaderValue;
                obj.splineIndex = settings.splineIndex;
                obj.knotIndex = settings.knotIndex;

                if (_activeTweens.Count > 0) tweenOverrideCheck(obj);

                // add to update tweens
                _activeTweens.Add(obj);

                Tween newTween = new Tween(obj);

                return newTween;
            }

            /// <summary>
            /// Frees a tween back into the tween pool.
            /// </summary>
            /// <param name="toFree">The object to free.</param>
            internal void freeTweenToPoolInternal(TweenObject toFree)
            {
                //Assert.IsTrue(toFree.inUse, "TweenObject already in pool.");

                // reset values back to default
                _activeTweens.Remove(toFree);

                toFree.inUse = false;
                toFree._object = null;
                toFree.id = 0;
                toFree.easeStyle = EasingStyle.Linear;
                toFree.easeDirection = EasingDirection.Out;
                toFree.updateFnc = null;
                toFree.delay = 0;
                toFree.duration = 0;
                toFree.initialDuration = 0;
                toFree.repeatCount = 0;
                toFree.pasued = false;
                toFree.looping = false;
                toFree.reverses = false;
                toFree.reverseDuration = 0;
                toFree.currentTime = 0;

                // clear callbacks
                toFree.onUpdateActions = null;
                toFree.onCompleteActions = null;

                _tweenPool.Push(toFree);
            }

            /// <summary>
            /// Checks to see if the tween is not tracking anything, or if it has already been freed.
            /// </summary>
            /// <param name="toCheck">The tween object to check.</param>
            /// <returns>True if the object is good, false if the object was not tracking anything.</returns>
            internal bool assessTracking(TweenObject toCheck)
            {
                if (!toCheck.inUse) return false; // object has already been cleaned up so halt
                if (toCheck._object == null) { freeTweenToPoolInternal(toCheck); return false; } // we are not tracking an object

                return true;
            }

            internal void tweenOverrideCheck(TweenObject toCheck)
            {
                for (int i = 0; i < _activeTweens.Count; i++)
                {
                    TweenObject other = _activeTweens[i];
                    if (toCheck.id != other.id && toCheck == other)
                    {
                        if (other._object is SplineContainer && toCheck._object is SplineContainer)
                        {
                            if (other.knotIndex != toCheck.knotIndex || other.splineIndex != toCheck.splineIndex)
                            {
                                continue;
                            }
                        }

                        freeTweenToPoolInternal(other);
                        --i;
                    }
                }
            }
            #endregion

            #region Unity Methods

            private void Awake()
            {
                Debug.Log("Awaken the beast inside......");
            }

            private void Update()
            {
                if (!_initialized) return;

                for (int i = 0; i < _activeTweens.Count; i++)
                {
                    TweenObject obj = _activeTweens[i];
                    if (!assessTracking(obj)) continue;

                    // calculate interpolation factor
                    obj.currentTime += obj.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                    if (obj.currentTime > obj.duration) // has tween completed
                    {
                        obj.updateFnc.Invoke(obj, 1f);

                        if (obj.reverses)
                        {
                            ValueContainer start = obj.end, end = obj.start;
                            obj.start = start;
                            obj.end = end;
                            obj.currentTime = 0;
                            obj.duration = obj.reverseDuration;
                            obj.reverses = false;
                            continue;
                        }
                        else if (!obj.reverses && obj.reverseDuration > 0f)
                        {
                            ValueContainer start = obj.end, end = obj.start;
                            obj.start = start;
                            obj.end = end;
                            obj.duration = obj.initialDuration;
                            obj.reverses = true;
                        }

                        if (obj.repeatCount > 0)
                        {
                            --obj.repeatCount;
                            obj.currentTime = 0;
                            continue;
                        }

                        freeTweenToPoolInternal(obj);
                        continue;
                    }

                    bool easeIn = obj.easeDirection == EasingDirection.In;

                    float interpolationFactor = Easing.EasingFunctions[(int)obj.easeStyle].Invoke(obj.currentTime / obj.duration, easeIn);

                    if (!assessTracking(obj)) continue;
                    obj.updateFnc.Invoke(obj, interpolationFactor);
                    obj.onUpdateActions?.Invoke(new Tween(obj));
                }
            }
            #endregion
        }

        public sealed class Tweener
        {
            public delegate void TweenCallback(Tween tween);

            public enum UpdateType
            {
                Update = 0,
                LateUpdate = 1,
                FixedUpdate = 2,
                UnscaledUpdate = 3,
                UnscaledFixedUpdate = 4,
                UnscaledLateUpdate = 5
            };

            public static bool initialized { get { return TweeningManager.instance != null && TweeningManager.instance._initialized; } }

            public static void Init()
            {
                if (initialized) { Debug.LogWarning("VG_Tween is already initialized."); return; }

                TweeningManager.init();
            }

            /// <summary>
            /// Gets a new interpolation value from the values passed. 
            /// </summary>
            /// <param name="t">The initial alpha value.</param>
            /// <param name="easingStyle">The easing style to ease t.</param>
            /// <param name="easingDirection">The direction to ease t in.</param>
            /// <returns>A new 't' value that will be eased.</returns>
            public static float GetValue(float t, EasingStyle easingStyle, EasingDirection easingDirection = EasingDirection.Out)
            {
                bool easeIn = easingDirection == EasingDirection.In;
                return Easing.EasingFunctions[(int)easingStyle](t, easeIn);
            }

            /// <summary>
            /// Cancels all tweens attached to an object.
            /// </summary>
            /// <param name="obj">The object to cancel tweens for.</param>
            public static void CancelTweens(UnityEngine.Object obj)
            {
                TweeningManager manager = TweeningManager.instance;
                for (int i = manager._activeTweens.Count-1; i >= 0; i--)
                {
                    TweenObject currentObj = manager._activeTweens[i];
                    if (obj != currentObj._object) continue;
                    
                    manager.freeTweenToPoolInternal(currentObj);
                }
            }

            #region Interpolation Methods

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
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ShaderFloat(Material mat, string shaderValue, float targetValue, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, float startValue = default(float), int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    matShaderValue = shaderValue,
                    startValue = new ValueContainer(mat.GetFloat(shaderValue)),
                    endValue = new ValueContainer(targetValue),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.ShaderFloat
                };

                return manager.startTweenInternal(mat, newInfo);
            }

            /// <summary>
            /// Interpolates from the current passed shader value to the target value.
            /// </summary>
            /// <param name="mat">The material to tween.</param>
            /// <param name="shaderValue">The shader color parameter.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="targetColor">The color to tween to.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween ShaderColor(Material mat, string shaderValue, Color targetColor, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Color? startValue = null, int repeatCount = 0)
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
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.ShaderColor
                };

                return manager.startTweenInternal(mat, newInfo);
            }

            public static Tween MaterialColor(Material mat, Color targetColor, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Color? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : mat.color),
                    endValue = new ValueContainer(targetColor),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.MaterialColor
                };

                return manager.startTweenInternal(mat, newInfo);
            }

            public static Tween GlobalMatVar(GameObject refGameOBJ, string varName, float start, float target, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(start),
                    endValue = new ValueContainer(target),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    matShaderValue = varName,
                    updater = manager.GlobalMatVar
                };

                return manager.startTweenInternal(refGameOBJ, newInfo);
            }

            public static Tween ImageColor(Image image, Color targetColor, float duration , EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Color? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : image.color),
                    endValue = new ValueContainer(targetColor),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    useUnscaledTime = true,
                    updater = manager.ImageColor
                };

                return manager.startTweenInternal(image, newInfo);
            }

            public static Tween TextColor(TMP_Text text, Color targetColor, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Color? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : text.color),
                    endValue = new ValueContainer(targetColor),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.TextColor
                };

                return manager.startTweenInternal(text, newInfo);
            }

            public static Tween SliderValue(Slider slider, float targetValue, float duration, EasingStyle style, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, float? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : slider.value),
                    endValue = new ValueContainer(targetValue),
                    easeStyle = style,
                    direction = direction,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.SliderValue
                };

                return manager.startTweenInternal(slider, newInfo);
            }

            /// <summary>
            /// Interpolates between the current position and target position.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween Position(GameObject targetObject, Vector3 target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Vector3? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo() 
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.position),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.Position
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current rotation and the target rotation.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target rotation that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween Rotation(GameObject targetObject, Quaternion target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Quaternion? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.rotation),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.Rotation
                };
                
                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current rotation and the target rotation.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target rotation that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween LocalRotation(GameObject targetObject, Quaternion target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Quaternion? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.rotation),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.LocalRotation
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current scale and target scale.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween LocalScale(GameObject targetObject, Vector3 target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Vector3? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.localScale),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.LocalScale
                };

                return manager.startTweenInternal(targetObject, newInfo);

            }

            /// <summary>
            /// Interpolates between the current position and target position. Make sure the game object passed is parented to something to get use out of this function.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target position that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween LocalPosition(GameObject targetObject, Vector3 target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Vector3? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.localPosition),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.LocalPosition
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates a canvas group's alpha value.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target alpha that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween CanvasGroupAlpha(CanvasGroup targetObject, float target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, float? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.alpha),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    useUnscaledTime = true,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.CanvasGroupAlpha
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }

            /// <summary>
            /// Interpolates between the current direction and target direction.
            /// </summary>
            /// <param name="targetObject">The object to tween.</param>
            /// <param name="target">The target direction that should be hit.</param>
            /// <param name="duration">How long the tween should last.</param>
            /// <param name="style">The easing style to use.</param>
            /// <param name="direction">The direction to ease with.</param>
            /// <returns>A tween struct to help in controlling the tween after it is created.</returns>
            public static Tween TransformUp(GameObject targetObject, Vector3 target, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, Vector3? startValue = null, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(startValue.HasValue ? startValue.Value : targetObject.transform.up),
                    endValue = new ValueContainer(target),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.TransformUp
                };

                return manager.startTweenInternal(targetObject, newInfo);
            }
            public static Tween CameraFOV(CinemachineCamera cam, float start, float end, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, float startValue = 0, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(start),
                    endValue = new ValueContainer(end),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    updater = manager.CameraFOV
                };

                return manager.startTweenInternal(cam, newInfo);
            }

            public static Tween BezierKnotPosition(SplineContainer container, int splineIndex, int knotIndex, BezierKnot start, BezierKnot end, float duration, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, float reverseDuration = -1, float startValue = 0, int repeatCount = 0)
            {
                TweeningManager manager = TweeningManager.instance;
                TweenInfo newInfo = new TweenInfo()
                {
                    duration = duration,
                    startValue = new ValueContainer(start),
                    endValue = new ValueContainer(end),
                    direction = direction,
                    easeStyle = style,
                    reverses = reverseDuration >= 0,
                    reverseDuration = reverseDuration,
                    repeatCount = repeatCount,
                    splineIndex = splineIndex,
                    knotIndex = knotIndex,
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
            #endregion
        }
        internal struct TweenInfo
        {
            public float duration;
            public float reverseDuration;
            public bool reverses;
            public bool useUnscaledTime;
            public int repeatCount;
            public EasingStyle easeStyle;
            public EasingDirection direction;

            public ValueContainer startValue;
            public ValueContainer endValue;

            // other values
            public string matShaderValue;
            public int knotIndex;
            public int splineIndex;

            public TweenUpdater updater;
        }

        public struct Tween
        {
            internal long id;
            internal TweenObject _target;

            // State variables

            /// <summary>
            /// The Tween's status. If false, the tween will no longer be interactable.
            /// </summary>
            public bool active { get { return id != 0 && id == _target.id && _target.inUse; } }

            public float duration { get { return active ? _target.duration : 0f; } }

            public float currentTime { get { return active ? _target.currentTime : 0f; } }

            public float progress { get { return active ? _target.currentTime / _target.duration : 0f; } }


            // Tween Control Methods //

            /// <summary>
            /// Starts/resumes the tween.
            /// </summary>
            public void Play()
            {
            }

            /// <summary>
            /// Pauses the tween.
            /// </summary>
            public void Pause()
            {

            }

            /// <summary>
            /// Instantly pauses tween and cleans it up.
            /// </summary>
            public void Stop()
            {
                if (!active) return;
                if (_target == null) return;

                TweeningManager.instance.freeTweenToPoolInternal(_target);
                _target = null;
            }

            public void Complete()
            {
                if (!active) return;
                if (_target == null) return;

                _target.updateFnc.Invoke(_target, 1f);
                TweeningManager.instance.freeTweenToPoolInternal(_target);
            }

            // Callback Methods //
            public void OnUpdate(Tweener.TweenCallback callback)
            {
                _target.onUpdateActions += callback;
            }


            // constructor
            internal Tween(TweenObject target)
            {
                _target = target;
                id = target.id;
            }
        }

        [Serializable]
        internal class TweenObject
        {
            internal long id = 0;
            public bool inUse = false;

            // tween props
            public float duration = 0f; // length of tween
            public float reverseDuration = 0f; // length of reverse tween
            public float initialDuration = 0f; // if repeats as well as reverses, needs to be here
            public float delay = 0f;
            public bool reverses = false;
            public float currentTime = 0f;
            public int repeatCount = 0;
            public bool looping = true;
            public bool pasued = false;
            public bool useUnscaledTime = false;

            // easing stuff
            public EasingStyle easeStyle = EasingStyle.Linear;
            public EasingDirection easeDirection = EasingDirection.Out;
            public TweenUpdater updateFnc = null;

            // callbacks
            public Tweener.TweenCallback onCompleteActions = null;
            public Tweener.TweenCallback onUpdateActions = null;

            // members
            public ValueContainer start;
            public ValueContainer end;

            public UnityEngine.Object _object = null;

            public string matShaderValue = null;
            public int knotIndex;
            public int splineIndex;

            public static bool operator ==(TweenObject a, TweenObject b)
            {
                if (a is null || b is null) return false;
                if (a._object != b._object) return false;          
                if (!a.updateFnc.Equals(b.updateFnc)) return false;
                if (a.id != b.id) return false;

                return true;
            }
            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                return this == obj as TweenObject;
            }

            public override int GetHashCode() // silencing compiler warnings
            {
                return 0;
            }

            public static bool operator !=(TweenObject a, TweenObject b)
            {
                return !(a == b);
            }
        }
    }
}