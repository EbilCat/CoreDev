using CoreDev.Sequencing;
using UnityEngine;

namespace TimeElapsedTester
{
    public class SchedulingTest : MonoBehaviour
    {
        private int lastPayloadValue;
        private bool callback1Fired = false;
        private bool callback2Fired = false;

        public void Awake()
        {
            Invoke("ScheduleCallback_CheckThatCallbackisFired", 5.0f);
            Invoke("ScheduleCallbackUnscaled_CheckThatCallbackisFired", 10.0f);
            Invoke("ReplaceCallback_CheckThatCallbackisFired", 15.0f);
            Invoke("ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired", 20.0f);
            Invoke("UnscheduleCallback_CheckThatCallbackisNotFired", 25.0f);
            Invoke("UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired", 30.0f);
            Invoke("FireTwoCallbacks_CheckThatBothCallbacksAreFired", 35.0f);
            Invoke("FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired", 40.0f);
            Invoke("FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast", 45.0f);
            Invoke("FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast", 50.0f);
            Invoke("UnregisterCallbackWhenFired_NoExceptionsThrown", 55.0f);
        }


    //*====================
    //* ScheduleCallback_CheckThatCallbackisFired
    //*====================
        [ContextMenu("ScheduleCallback_CheckThatCallbackisFired")]
        private void ScheduleCallback_CheckThatCallbackisFired()
        {
            Debug.Log("Running Test: ScheduleCallback_CheckThatCallbackisFired");
            this.Reset();
            UniversalTimer.ScheduleCallback(CallbackFunction, 2.0f, 1);
            Invoke("CheckResultFor_ScheduleCallback_CheckThatCallbackisFired", 2.1f);
        }

        private void CheckResultFor_ScheduleCallback_CheckThatCallbackisFired()
        {
            if (callback1Fired == true && callback2Fired == false)
            {
                Debug.Log("ScheduleCallback_CheckThatCallbackisFired: Passed");
            }
            else
            {
                Debug.Log("ScheduleCallback_CheckThatCallbackisFired: Failed");
            }
        }


    //*====================
    //* ScheduleCallbackUnscaled_CheckThatCallbackisFired
    //*====================
        [ContextMenu("ScheduleCallbackUnscaled_CheckThatCallbackisFired")]
        private void ScheduleCallbackUnscaled_CheckThatCallbackisFired()
        {
            Debug.Log("Running Test: ScheduleCallbackUnscaled_CheckThatCallbackisFired");
            this.Reset();
            UniversalTimer.ScheduleCallbackUnscaled(CallbackFunction, 2.0f, 1);
            Invoke("CheckResultFor_ScheduleCallbackUnscaled_CheckThatCallbackisFired", 2.1f);
        }

        private void CheckResultFor_ScheduleCallbackUnscaled_CheckThatCallbackisFired()
        {
            if (callback1Fired == true && callback2Fired == false)
            {
                Debug.Log("ScheduleCallbackUnscaled_CheckThatCallbackisFired: Passed");
            }
            else
            {
                Debug.Log("ScheduleCallbackUnscaled_CheckThatCallbackisFired: Failed");
            }
        }


    //*====================
    //* ReplaceCallback_CheckThatCallbackisFired
    //*====================
        [ContextMenu("ReplaceCallback_CheckThatCallbackisFired")]
        private void ReplaceCallback_CheckThatCallbackisFired()
        {
            Debug.Log("Running Test: ReplaceCallback_CheckThatCallbackisFired");
            this.Reset();
            UniversalTimer.ScheduleCallback(CallbackFunction, 2.0f, 1);
            Invoke("ReplaceCallback", 1.5f);
            Invoke("CheckResultFor_ReplaceCallback_CheckThatCallbackisFired", 2.6f);
        }

        private void ReplaceCallback()
        {
            Debug.Log("Replacing callback");
            UniversalTimer.ScheduleCallback(CallbackFunction, 1.0f, 2);
        }

        private void CheckResultFor_ReplaceCallback_CheckThatCallbackisFired()
        {
            if (callback1Fired == false && callback2Fired == true)
            {
                Debug.Log("ReplaceCallback_CheckThatCallbackisFired: Passed");
            }
            else
            {
                Debug.Log("ReplaceCallback_CheckThatCallbackisFired: Failed");
            }
        }


    //*====================
    //* ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired
    //*====================
        [ContextMenu("ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired")]
        private void ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired()
        {
            Debug.Log("Running Test: ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired");
            this.Reset();
            UniversalTimer.ScheduleCallbackUnscaled(CallbackFunction, 2.0f, 1);
            Invoke("ReplaceCallbackUnscaled", 1.5f);
            Invoke("CheckResultFor_ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired", 2.6f);
        }

        private void ReplaceCallbackUnscaled()
        {
            Debug.Log("Replacing callback");
            UniversalTimer.ScheduleCallbackUnscaled(CallbackFunction, 1.0f, 2);
        }

        private void CheckResultFor_ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired()
        {
            if (callback1Fired == false && callback2Fired == true)
            {
                Debug.Log("ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired: Passed");
            }
            else
            {
                Debug.Log("ReplaceCallbackUnscaled_CheckThatCallbackUnscaledisFired: Failed");
            }
        }


    //*====================
    //* UnscheduleCallback_CheckThatCallbackisNotFired
    //*====================
        [ContextMenu("UnscheduleCallback_CheckThatCallbackisNotFired")]
        private void UnscheduleCallback_CheckThatCallbackisNotFired()
        {
            Debug.Log("Running Test: UnscheduleCallback_CheckThatCallbackisNotFired");
            this.Reset();
            UniversalTimer.ScheduleCallback(CallbackFunction, 2.0f, 1);
            Invoke("UnscheduleCallback", 1.5f);
            Invoke("CheckResultFor_UnscheduleCallback_CheckThatCallbackisNotFired", 3.0f);
        }

        private void UnscheduleCallback()
        {
            Debug.Log("Unscheduling callback");
            UniversalTimer.UnscheduleCallback(CallbackFunction);
        }

        private void CheckResultFor_UnscheduleCallback_CheckThatCallbackisNotFired()
        {
            if (callback1Fired == false && callback2Fired == false)
            {
                Debug.Log("UnscheduleCallback_CheckThatCallbackisNotFired: Passed");
            }
            else
            {
                Debug.Log("UnscheduleCallback_CheckThatCallbackisNotFired: Failed");
            }
        }


    //*====================
    //* UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired
    //*====================
        [ContextMenu("UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired")]
        private void UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired()
        {
            Debug.Log("Running Test: UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired");
            this.Reset();
            UniversalTimer.ScheduleCallbackUnscaled(CallbackFunction, 2.0f, 1);
            Invoke("UnscheduleCallbackUnscaled", 1.5f);
            Invoke("CheckResultFor_UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired", 3.0f);
        }

        private void UnscheduleCallbackUnscaled()
        {
            Debug.Log("Unscheduling callback");
            UniversalTimer.UnscheduleCallback(CallbackFunction);
        }

        private void CheckResultFor_UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired()
        {
            if (callback1Fired == false && callback2Fired == false)
            {
                Debug.Log("UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired: Passed");
            }
            else
            {
                Debug.Log("UnscheduleCallbackUnscaled_CheckThatCallbackUnscaledisNotFired: Failed");
            }
        }


    //*====================
    //* FireTwoCallbacks_CheckThatBothCallbacksAreFired
    //*====================
        [ContextMenu("FireTwoCallbacks_CheckThatBothCallbacksAreFired")]
        private void FireTwoCallbacks_CheckThatBothCallbacksAreFired()
        {
            Debug.Log("Running Test: FireTwoCallbacks_CheckThatBothCallbacksAreFired");
            this.Reset();
            UniversalTimer.ScheduleCallback((x) => CallbackFunction(x), 2.0f, 1);
            UniversalTimer.ScheduleCallback((x) => CallbackFunction(x), 3.0f, 2);
            Invoke("CheckResultFor_FireTwoCallbacks_CheckThatBothCallbacksAreFired", 3.5f);
        }

        private void CheckResultFor_FireTwoCallbacks_CheckThatBothCallbacksAreFired()
        {
            if (callback1Fired == true && callback2Fired == true)
            {
                Debug.Log("FireTwoCallbacks_CheckThatBothCallbacksAreFired: Passed");
            }
            else
            {
                Debug.Log("FireTwoCallbacks_CheckThatBothCallbacksAreFired: Failed");
            }
        }


    //*====================
    //* FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired
    //*====================
        [ContextMenu("FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired")]
        private void FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired()
        {
            Debug.Log("Running Test: FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired");
            this.Reset();
            UniversalTimer.ScheduleCallbackUnscaled((x) => CallbackFunction(x), 2.0f, 1);
            UniversalTimer.ScheduleCallbackUnscaled((x) => CallbackFunction(x), 3.0f, 2);
            Invoke("CheckResultFor_FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired", 3.5f);
        }

        private void CheckResultFor_FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired()
        {
            if (callback1Fired == true && callback2Fired == true)
            {
                Debug.Log("FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired: Passed");
            }
            else
            {
                Debug.Log("FireTwoCallbacksUnscaled_CheckThatBothCallbacksAreFired: Failed");
            }
        }


    //*====================
    //* FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast
    //*====================
        [ContextMenu("FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast")]
        private void FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast()
        {
            //This test needs to be started like this because using ContextMenu to run test affects UnscaledDeltaTime
            Debug.Log("Running Test: FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast");
            Invoke("Run_FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast", 1.0f);
        }

        private void Run_FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast()
        {
            this.Reset();
            Time.timeScale = 0.5f;
            UniversalTimer.ScheduleCallback((x) => CallbackFunction(x), 1.0f, 1);
            UniversalTimer.ScheduleCallbackUnscaled((x) => CallbackFunction(x), 1.0f, 2);
            Invoke("CheckResultFor_FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast", 3.5f);
        }

        private void CheckResultFor_FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast()
        {
            if (callback1Fired == true && callback2Fired == true && lastPayloadValue == 1)
            {
                Debug.Log("FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast: Passed");
            }
            else
            {
                Debug.Log("FireScaledAndUnscaledCallbackWithTimeScaleHalved_CheckThatScaledTimeCallbackFiredLast: Failed");
            }
            this.Reset();
        }


    //*====================
    //* FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast
    //*====================
        [ContextMenu("FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast")]
        private void FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast()
        {
            //This test needs to be started like this because using ContextMenu to run test affects UnscaledDeltaTime
            Debug.Log("Running Test: FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast");
            Invoke("Run_FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast", 1.0f);
        }

        private void Run_FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast()
        {
            this.Reset();
            Time.timeScale = 2.0f;
            UniversalTimer.ScheduleCallback((x) => CallbackFunction(x), 1.0f, 1);
            UniversalTimer.ScheduleCallbackUnscaled((x) => CallbackFunction(x), 1.0f, 2);
            Invoke("CheckResultFor_FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast", 3.5f);
        }

        private void CheckResultFor_FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast()
        {
            if (callback1Fired == true && callback2Fired == true && lastPayloadValue == 2)
            {
                Debug.Log("FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast: Passed");
            }
            else
            {
                Debug.Log("FireScaledAndUnscaledCallbackWithTimeScaleDoubled_CheckThatUnscaledTimeCallbackFiredLast: Failed");
            }
            this.Reset();
        }




    //*====================
    //* UnregisterCallbackWhenFired_NoExceptionsThrown
    //*====================
        [ContextMenu("UnregisterCallbackWhenFired_NoExceptionsThrown")]
        private void UnregisterCallbackWhenFired_NoExceptionsThrown()
        {
            Debug.Log("Running Test: UnregisterCallbackWhenFired_NoExceptionsThrown");
            this.Reset();
            UniversalTimer.ScheduleCallback(CustomCallback, 1.0f);
            Invoke("CheckResultFor_UnregisterCallbackWhenFired_NoExceptionsThrown", 2.0f);
        }

        private void CustomCallback(object[] obj)
        {
            UniversalTimer.UnscheduleCallback(CustomCallback);
        }

        private void CheckResultFor_UnregisterCallbackWhenFired_NoExceptionsThrown()
        {
            Debug.Log("If you see this and no exceptions are thrown, this test has passed");
        }


    //*====================
    //* PRIVATE
    //*====================
        private void CallbackFunction(object[] obj)
        {
            int payload = (int)obj[0];
            Debug.Log($"Callback fired with payload value: {payload}");

            lastPayloadValue = payload;

            if (payload == 1)
            {
                callback1Fired = true;
            }

            if (payload == 2)
            {
                callback2Fired = true;
            }
        }

        private void Reset()
        {
            Time.timeScale = 1.0f;
            this.callback1Fired = false;
            this.callback2Fired = false;
        }
    }
}