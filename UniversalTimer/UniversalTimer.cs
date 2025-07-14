using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Sequencing
{
    public delegate void TimeElapsedHandler(float deltaTime, float unscaledDeltaTime, int executionOrder);

    public class UniversalTimer : MonoBehaviour
    {
        private enum CountDownType { DELTA_TIME, UNSCALED_DELTA_TIME, FRAMES }
        private static UniversalTimer universalTimerInstance;
        private static void InitDriverGO()
        {
            if (universalTimerInstance == null)
            {
                Debug.Log("UniversalTimer Created");
                GameObject universalTimerGO = new GameObject("[UniversalTimer]");
                universalTimerInstance = universalTimerGO.AddComponent<UniversalTimer>();
            }
        }

        private static SortedList<int, InitExecutor> initExecutors = new SortedList<int, InitExecutor>();
        private static SortedList<int, TimeElapsedExecutor> timeElapsedExecutors = new SortedList<int, TimeElapsedExecutor>();

        private static List<Action<object[]>> timedCallbacks = new List<Action<object[]>>();
        private static List<float> countDowns = new List<float>();
        private static List<CountDownType> countdownType = new List<CountDownType>();
        private static List<object[]> payloads = new List<object[]>();
        private static List<int> frameAdded = new List<int>();
        private static List<Action<object[]>> unschedulingCallbacks = new List<Action<object[]>>();

        private static bool initPhaseCompleted = false;


        //*====================
        //* UNITY
        //*====================
        protected void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            foreach (KeyValuePair<int, InitExecutor> kvp in initExecutors)
            {
                InitExecutor initExecutor = kvp.Value;
                initExecutor.Init();
            }
            initExecutors.Clear();
            initPhaseCompleted = true;
        }

        protected void Update()
        {
            ProcessTimeElapsedExecutors();
            ProcessScheduledCallbacks();
        }

        protected void OnDestroy()
        {
            initExecutors.Clear();
            timeElapsedExecutors.Clear();
            timedCallbacks.Clear();
            countDowns.Clear();
            countdownType.Clear();
            payloads.Clear();
            frameAdded.Clear();
            unschedulingCallbacks.Clear();
            initPhaseCompleted = false;

            universalTimerInstance = null;

            Debug.Log("UniversalTimer Destroyed");
        }


        //*====================
        //* PUBLIC
        //*====================
        /// <summary>
        /// Registers a callback which is fired during Universal Timer's Start() function
        /// </summary>
        /// <param name="initHandler">Implementing class of IHasInitHandler to be registered</param>
        /// <param name="executionOrder">Dictates the execution order. Callbacks with lower numbers are executed first</param>
        public static void RegisterForInit(IHasInitHandler initHandler, int executionOrder = 0)
        {
            InitDriverGO();

            InitExecutor initExecutor = null;
            initExecutors.TryGetValue(executionOrder, out initExecutor);
            if (initExecutor == null)
            {
                initExecutor = new InitExecutor();
                initExecutors.Add(executionOrder, initExecutor);
            }

            if (initPhaseCompleted)
            {
                initHandler.Init();
            }
            else
            {
                initExecutor.RegisterForInit(initHandler);
            }
        }

        /// <summary>
        /// Registers a callback which is fired everytime there progression of time
        /// </summary>
        /// <param name="timeElapsedHandler">Implementing class of TimeElapsedHandler to be registered</param>
        /// <param name="executionOrder">Dictates the execution order. Callbacks with lower numbers are executed first</param>
        public static void RegisterForTimeElapsed(TimeElapsedHandler timeElapsedHandler, int executionOrder = 0)
        {
            InitDriverGO();

            TimeElapsedExecutor timeExecutor = null;
            timeElapsedExecutors.TryGetValue(executionOrder, out timeExecutor);
            if (timeExecutor == null)
            {
                timeExecutor = new TimeElapsedExecutor();
                timeElapsedExecutors.Add(executionOrder, timeExecutor);
            }

            timeExecutor.RegisterForTimeElapsed(timeElapsedHandler);
        }

        /// <summary>
        /// Unregisters callback from any future time progression updates
        /// </summary>
        /// <param name="timeElapsedHandler">Implementing class of TimeElapsedHandler to be unregistered</param>
        public static void UnregisterFromTimeElapsed(TimeElapsedHandler timeElapsedHandler)
        {
            if (universalTimerInstance == null) { return; }

            foreach (KeyValuePair<int, TimeElapsedExecutor> kvp in timeElapsedExecutors)
            {
                TimeElapsedExecutor timeExecutor = kvp.Value;
                timeExecutor.UnregisterFromTimeElapsed(timeElapsedHandler);
            }
        }

        /// <summary>
        /// Schedules a callback to be fired after a specified amount of time has passed. Registering a callback more than once
        /// replaces the previous callback.
        /// </summary>
        /// <param name="callback">Callback to be fired</param>
        /// <param name="countDownSecs">Time after which callback will be fired (Default value fires callback in the next frame)</param>
        /// <param name="payload">Any params to be passed back through the callback</param>
        public static void ScheduleCallback(Action<object[]> callback, float countDownSecs = float.Epsilon, params object[] payload)
        {
            InitDriverGO();

            int index = timedCallbacks.IndexOf(callback);
            UpdateCallback(callback, countDownSecs, CountDownType.DELTA_TIME, payload, index);
        }


        /// <summary>
        /// Schedules a callback to be fired after a specified amount of time has passed. Registering a callback more than once
        /// replaces the previous callback.
        /// </summary>
        /// <param name="callback">Callback to be fired</param>
        /// <param name="countDownSecs">Time after which callback will be fired (Default value fires callback in the next frame)</param>
        /// <param name="payload">Any params to be passed back through the callback</param>
        public static void ScheduleCallbackUnscaled(Action<object[]> callback, float countDownSecs = float.Epsilon, params object[] payload)
        {
            InitDriverGO();

            int index = timedCallbacks.IndexOf(callback);
            UpdateCallback(callback, countDownSecs, CountDownType.UNSCALED_DELTA_TIME, payload, index);
        }


        /// <summary>
        /// Schedules a callback to be fired after a specified amount of frames have passed. Registering a callback more than once
        /// replaces the previous callback.
        /// </summary>
        /// <param name="callback">Callback to be fired</param>
        /// <param name="frames">Frames after which callback will be fired (Default value fires callback in the next frame)</param>
        /// <param name="payload">Any params to be passed back through the callback</param>
        public static void ScheduleCallbackFrames(Action<object[]> callback, int frames = 1, params object[] payload)
        {
            InitDriverGO();

            int index = timedCallbacks.IndexOf(callback);
            UpdateCallback(callback, frames, CountDownType.FRAMES, payload, index);
        }

        /// <summary>
        /// Unschedules a scheduled callback
        /// </summary>
        /// <param name="callback">Callback to unschedule</param>
        public static void UnscheduleCallback(Action<object[]> callback)
        {
            if (universalTimerInstance == null) { return; }
            unschedulingCallbacks.Add(callback);
        }


        //*====================
        //* PRIVATE
        //*====================
        private static void ProcessTimeElapsedExecutors()
        {
            foreach (KeyValuePair<int, TimeElapsedExecutor> kvp in timeElapsedExecutors)
            {
                TimeElapsedExecutor timeExecutor = kvp.Value;
                int executionOrder = kvp.Key;
                timeExecutor.TimeElapsed(Time.deltaTime, Time.unscaledDeltaTime, executionOrder);
            }
        }

        private static void ProcessScheduledCallbacks()
        {
            //Process scheduled callbacks
            int timedCallbackCount = timedCallbacks.Count;
            for (int i = timedCallbackCount - 1; i >= 0; i--)
            {
                if (frameAdded[i] != Time.frameCount)
                {
                    FireCallbackIfCountdownReached(i);
                    RemoveCallbackIfCountdownReached(i);
                }
            }

            //Clear any callbacks slated for unscheduling
            for (int i = 0; i < unschedulingCallbacks.Count; i++)
            {
                int index = timedCallbacks.IndexOf(unschedulingCallbacks[i]);
                if (index != -1)
                {
                    RemoveCallback(index);
                }
            }
            unschedulingCallbacks.Clear();
        }

        private static void FireCallbackIfCountdownReached(int callbackIndex)
        {
            Action<object[]> timedCallback = timedCallbacks[callbackIndex];

            if (unschedulingCallbacks.Contains(timedCallback) == false)
            {
                float countDownSecs = countDowns[callbackIndex];
                float timeElapsed = 0.0f;

                switch (countdownType[callbackIndex])
                {
                    case CountDownType.DELTA_TIME:
                        timeElapsed = Time.deltaTime;
                        break;
                    case CountDownType.UNSCALED_DELTA_TIME:
                        timeElapsed = Time.unscaledDeltaTime;
                        break;
                    case CountDownType.FRAMES:
                        timeElapsed = 1.0f;
                        break;
                }

                countDownSecs -= timeElapsed;

                countDowns[callbackIndex] = countDownSecs;
                if (countDownSecs <= 0.0f)
                {
                    try
                    {
                        timedCallback(payloads[callbackIndex]);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Object obj = timedCallback.Target as UnityEngine.Object;
                        Debug.LogException(e, obj);
                    }
                }
            }
        }

        private static void RemoveCallbackIfCountdownReached(int callbackIndex)
        {
            float countDownSecs = countDowns[callbackIndex];
            if (countDownSecs <= 0.0f)
            {
                RemoveCallback(callbackIndex);
            }
        }


        //*====================
        //* PRIVATE
        //*====================
        private static void UpdateCallback(Action<object[]> callback, float countDownSecs, CountDownType countDownType, object[] payload, int index = -1)
        {
            if (index != -1)
            {
                timedCallbacks[index] = callback;
                countDowns[index] = countDownSecs;
                countdownType[index] = countDownType;
                payloads[index] = payload;
            }
            else
            {
                timedCallbacks.Add(callback);
                countDowns.Add(countDownSecs);
                countdownType.Add(countDownType);
                payloads.Add(payload);
                frameAdded.Add(Time.frameCount);
            }
        }

        private static void RemoveCallback(int callbackIndex)
        {
            timedCallbacks.RemoveAt(callbackIndex);
            countDowns.RemoveAt(callbackIndex);
            countdownType.RemoveAt(callbackIndex);
            payloads.RemoveAt(callbackIndex);
            frameAdded.RemoveAt(callbackIndex);
        }

        [ContextMenu("PrintAllRegistered")]
        protected void PrintAllRegisteredCallbacks()
        {
            foreach (KeyValuePair<int, TimeElapsedExecutor> kvp in timeElapsedExecutors)
            {
                TimeElapsedExecutor timeExecutor = kvp.Value;
                Debug.Log($"Excution Order: {kvp.Key}");
                timeExecutor.PrintTimeElapsedHandlers();
            }
        }
    }
}