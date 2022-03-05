using System.Collections.Generic;
using UnityEngine;

namespace CoreDev.Sequencing
{
    public class InitExecutor
    {
        private List<IHasInitHandler> initHandlers = new List<IHasInitHandler>();


        //*====================
        //* PUBLIC
        //*====================
        public void Init()
        {
            for (int i = initHandlers.Count - 1; i >= 0; i--)
            {
                IHasInitHandler initHandler = initHandlers[i];
                initHandler.Init();
            }
            initHandlers.Clear();
        }

        public void RegisterForInit(IHasInitHandler initHandler)
        {
            if (initHandlers.Contains(initHandler) == false)
            {
                this.initHandlers.Add(initHandler);
            }
        }

        public void PrintInitHandlers()
        {
            foreach (IHasInitHandler initHandler in initHandlers)
            {
                Debug.Log($"  {initHandler.GetType()}");
            }
        }
    }
}