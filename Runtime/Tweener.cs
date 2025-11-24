using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
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
            [SerializeField] internal List<TweenObject> _fixedUpdateTweens = new List<TweenObject>();
            [SerializeField] internal List<TweenObject> _lateUpdateTweens = new List<TweenObject>();

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
                obj.useUnscaledTime = settings.updateType == Tweener.UpdateType.UnscaledUpdate || settings.updateType == Tweener.UpdateType.UnscaledFixedUpdate || settings.updateType == Tweener.UpdateType.UnscaledLateUpdate;
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
                obj.updateType = settings.updateType;

                // add to update tweens
                if (settings.updateType == Tweener.UpdateType.Update || settings.updateType == Tweener.UpdateType.UnscaledUpdate)
                {
                    if (_activeTweens.Count > 0) tweenOverrideCheck(obj);
                    _activeTweens.Add(obj);
                }
                else if (settings.updateType == Tweener.UpdateType.FixedUpdate || settings.updateType == Tweener.UpdateType.UnscaledFixedUpdate)
                {
                    if (_fixedUpdateTweens.Count > 0) tweenOverrideCheck(obj);
                    _fixedUpdateTweens.Add(obj);
                }
                else if (settings.updateType == Tweener.UpdateType.LateUpdate || settings.updateType == Tweener.UpdateType.UnscaledLateUpdate)
                {
                    if (_lateUpdateTweens.Count > 0) tweenOverrideCheck(obj);
                    _lateUpdateTweens.Add(obj);
                }

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

                // before cleaning tween, fire off any completion callbacks
                toFree.onCompleteActions?.Invoke(new Tween(toFree));

                // remove tween from active list
                if (toFree.updateType == Tweener.UpdateType.Update || toFree.updateType == Tweener.UpdateType.UnscaledUpdate)
                {
                    if (_activeTweens.Count > 0) tweenOverrideCheck(toFree);
                    _activeTweens.Remove(toFree);
                }
                else if (toFree.updateType == Tweener.UpdateType.FixedUpdate || toFree.updateType == Tweener.UpdateType.UnscaledFixedUpdate)
                {
                    if (_fixedUpdateTweens.Count > 0) tweenOverrideCheck(toFree);
                    _fixedUpdateTweens.Remove(toFree);
                }
                else if (toFree.updateType == Tweener.UpdateType.LateUpdate || toFree.updateType == Tweener.UpdateType.UnscaledLateUpdate)
                {
                    if (_lateUpdateTweens.Count > 0) tweenOverrideCheck(toFree);
                    _lateUpdateTweens.Remove(toFree);
                }

                // reset values back to default
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
                toFree.paused = false;
                toFree.looping = false;
                toFree.reverses = false;
                toFree.useUnscaledTime = false;
                toFree.reverseDuration = -1f;
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

            internal bool overrideComparison(TweenObject toCheck, TweenObject other)
            {
                if (toCheck.id == other.id) return false;
                if (toCheck._object != other._object) return false;
                if (!toCheck.updateFnc.Equals(other.updateFnc)) return false;

                if (toCheck._object is SplineContainer)
                {
                    if (other.knotIndex != toCheck.knotIndex || other.splineIndex != toCheck.splineIndex)
                    {
                        return false;
                    }
                }

                return true;
            }

            internal void tweenOverrideCheck(TweenObject toCheck)
            {
                for (int i = 0; i < _activeTweens.Count; i++)
                {
                    TweenObject other = _activeTweens[i];

                    if (!overrideComparison(toCheck, other)) continue;

                    freeTweenToPoolInternal(other);
                    --i;
                }
                for (int i = 0; i < _fixedUpdateTweens.Count; i++)
                {
                    TweenObject other = _fixedUpdateTweens[i];

                    if (!overrideComparison(toCheck, other)) continue;

                    freeTweenToPoolInternal(other);
                    --i;
                }
                for (int i = 0; i < _lateUpdateTweens.Count; i++)
                {
                    TweenObject other = _lateUpdateTweens[i];

                    if (!overrideComparison(toCheck, other)) continue;

                    freeTweenToPoolInternal(other);
                    --i;
                }
            }

            internal void tweenUpdate(TweenObject obj, float dt, float unscaledDt)
            {
                if (!assessTracking(obj)) return;
                if (!obj.useUnscaledTime && dt == 0) return; // early exit for if game is paused when using scaled delta time
                if (obj.paused) return;

                // calculate interpolation factor
                obj.currentTime += obj.useUnscaledTime ? unscaledDt : dt;

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
                        return;
                    }
                    else if (!obj.reverses && obj.reverseDuration > 0f)
                    {
                        ValueContainer start = obj.end, end = obj.start;
                        obj.start = start;
                        obj.end = end;
                        obj.duration = obj.initialDuration;
                        obj.reverses = true;
                    }

                    if (obj.looping)
                    {
                        obj.currentTime = 0;
                        return;
                    }

                    if (obj.repeatCount > 0)
                    {
                        --obj.repeatCount;
                        obj.currentTime = 0;
                        return;
                    }
                    obj.currentTime = obj.duration;
                    freeTweenToPoolInternal(obj);
                    return;
                }

                bool easeIn = obj.easeDirection == EasingDirection.In;

                float interpolationFactor = Easing.EasingFunctions[(int)obj.easeStyle].Invoke(obj.currentTime / obj.duration, easeIn);

                if (!assessTracking(obj)) return;
                obj.updateFnc.Invoke(obj, interpolationFactor);
                obj.onUpdateActions?.Invoke(new Tween(obj));
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
                if (_activeTweens.Count == 0) return;

                float dt = Time.deltaTime, unscaledDt = Time.unscaledDeltaTime;

                for (int i = 0; i < _activeTweens.Count; i++)
                {
                    TweenObject obj = _activeTweens[i];
                    tweenUpdate(obj, dt, unscaledDt);
                }
            }

            private void FixedUpdate()
            {
                if (!_initialized) return;
                if (_fixedUpdateTweens.Count == 0) return;

                float dt = Time.fixedDeltaTime, unscaledDt = Time.fixedUnscaledDeltaTime;

                for (int i = 0; i < _fixedUpdateTweens.Count; i++)
                {
                    TweenObject obj = _fixedUpdateTweens[i];
                    tweenUpdate(obj, dt, unscaledDt);
                }
            }

            private void LateUpdate()
            {
                if (!_initialized) return;
                if (_lateUpdateTweens.Count == 0) return;

                float dt = Time.deltaTime, unscaledDt = Time.unscaledDeltaTime;

                for (int i = 0; i < _lateUpdateTweens.Count; i++)
                {
                    TweenObject obj = _lateUpdateTweens[i];
                    tweenUpdate(obj, dt, unscaledDt);
                }
            }

            #endregion
        }

        public sealed partial class Tweener
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
                for (int i = manager._activeTweens.Count - 1; i >= 0; i--)
                {
                    TweenObject currentObj = manager._activeTweens[i];
                    if (obj != currentObj._object) continue;

                    manager.freeTweenToPoolInternal(currentObj);
                }
                for (int i = manager._fixedUpdateTweens.Count - 1; i >= 0; i--)
                {
                    TweenObject currentObj = manager._fixedUpdateTweens[i];
                    if (obj != currentObj._object) continue;

                    manager.freeTweenToPoolInternal(currentObj);
                }
                for (int i = manager._lateUpdateTweens.Count - 1; i >= 0; i--)
                {
                    TweenObject currentObj = manager._lateUpdateTweens[i];
                    if (obj != currentObj._object) continue;

                    manager.freeTweenToPoolInternal(currentObj);
                }
            }
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

            public Tweener.UpdateType updateType;

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

            // State variables & getters

            /// <summary>
            /// The Tween's status. If false, the tween will no longer be interactable.
            /// </summary>
            public bool active { get { return id != 0 && id == _target.id && _target.inUse; } }

            public float duration { get { return active ? _target.duration : 0f; } }

            public float currentTime { get { return active ? _target.currentTime : 0f; } }

            public float progress { get { return active ? _target.currentTime / _target.duration : 0f; } }

            public string objectName { get { return active ? _target._object.name : null; } }

            
            // Tween Control Methods //

            /// <summary>
            /// Starts/resumes the tween.
            /// </summary>
            public void Play()
            {
                if (!active) return;

                _target.paused = false;
            }

            /// <summary>
            /// Pauses the tween.
            /// </summary>
            public void Pause()
            {
                if (!active) return;

                _target.paused = true;
            }

            /// <summary>
            /// Instantly pauses tween and cleans it up.
            /// </summary>
            public void Stop()
            {
                if (!active) return;

                TweeningManager.instance.freeTweenToPoolInternal(_target);
                _target = null;
            }

            public void Complete()
            {
                if (!active) return;

                _target.updateFnc.Invoke(_target, 1f);
                TweeningManager.instance.freeTweenToPoolInternal(_target);
            }
            
            public void SetLoopState(bool state)
            {
                if (!active) return;

                _target.looping = state;
            }

            public Tweener.UpdateType updateType { get { return _target.updateType; } }

            // Callback Methods //
            
            /// <summary>
            /// Calls the provided callback when a tween is updated within the update loop.
            /// </summary>
            /// <param name="callback"></param>
            public void OnUpdate(Tweener.TweenCallback callback)
            {
                _target.onUpdateActions += callback;
            }

            /// <summary>
            /// Calls the provided callback when a tween completes. A tween can be completed in a few different ways:
            ///<br/> - Getting overridden (another tween plays with the same object and update function)
            ///<br/> - Manually completing the tween using the Tween struct's "Complete" method
            ///<br/> - Letting the tween play out it's full duration
            /// </summary>
            /// <param name="callback">The callback function to use. Takes one parameter, which is a Tween struct.</param>
            public void OnComplete(Tweener.TweenCallback callback)
            {
                _target.onCompleteActions += callback;
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
            public float reverseDuration = -1f; // length of reverse tween
            public float initialDuration = 0f; // if repeats as well as reverses, needs to be here
            public float delay = 0f;
            public bool reverses = false;
            public float currentTime = 0f;
            public int repeatCount = 0;
            public bool looping = false;
            public bool paused = false;
            public bool useUnscaledTime = false;
            public Tweener.UpdateType updateType = Tweener.UpdateType.Update;

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