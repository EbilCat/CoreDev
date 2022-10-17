using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Sequencing
{
    public class TimeElapsedExecutor
    {
        private List<TimeElapsedHandler> pendingAddition = new List<TimeElapsedHandler>();
        private List<TimeElapsedHandler> pendingRemoval = new List<TimeElapsedHandler>();
        private List<TimeElapsedHandler> timeElapsedHandlers = new List<TimeElapsedHandler>();


        //*====================
        //* PUBLIC
        //*====================
        public void TimeElapsed(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            timeElapsedHandlers.AddRange(pendingAddition);
            pendingAddition.Clear();

            for (int i = timeElapsedHandlers.Count - 1; i >= 0; i--)
            {
                TimeElapsedHandler timeElapsedHandler = timeElapsedHandlers[i];
                if (this.pendingRemoval.Contains(timeElapsedHandler) == false)
                {
                    try
                    {
                        timeElapsedHandler(deltaTime, unscaledDeltaTime, executionOrder);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            this.timeElapsedHandlers.RemoveAll(pendingRemoval.Contains);
            this.pendingRemoval.Clear();
        }

        public void RegisterForTimeElapsed(TimeElapsedHandler timeElapsedHandler)
        {
            this.pendingRemoval.Remove(timeElapsedHandler);

            if (timeElapsedHandlers.Contains(timeElapsedHandler) == false && pendingAddition.Contains(timeElapsedHandler) == false)
            {
                if (this.pendingAddition.Contains(timeElapsedHandler) == false)
                {
                    this.pendingAddition.Add(timeElapsedHandler);
                }
            }
        }

        public void UnregisterFromTimeElapsed(TimeElapsedHandler timeElapsedHandler)
        {
            this.pendingAddition.Remove(timeElapsedHandler);

            if (this.pendingRemoval.Contains(timeElapsedHandler) == false)
            {
                this.pendingRemoval.Add(timeElapsedHandler);
            }
        }

        public void PrintTimeElapsedHandlers()
        {
            foreach (TimeElapsedHandler timeElapsedHandler in timeElapsedHandlers)
            {
                Debug.Log($"  {timeElapsedHandler.GetType()}");
            }
        }
    }
}