using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CoreDev.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreDev.DataObjectInspector.AddOns
{
    public class PointerInputModuleInspector : MonoBehaviour
    {
        private BaseInputModule inputModule;
        private IEnumerable pointerDataEnumerable;
        private PointerEventData currentPointer;


//*====================
//* UNITY
//*====================
        private void Update()
        {
            if (EventSystem.current.currentInputModule != null)
            {
                this.inputModule = EventSystem.current.currentInputModule;
                Type inputModuleType = inputModule.GetType();
                FieldInfo oldPointerDataEnumerableFieldInfo = inputModuleType.GetField("m_PointerData", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo newPointerDataEnumerableFieldInfo = inputModuleType.GetField("m_PointerStates", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo pointerDataEnumerableFieldInfo = oldPointerDataEnumerableFieldInfo ?? newPointerDataEnumerableFieldInfo;
                this.pointerDataEnumerable = pointerDataEnumerableFieldInfo.GetValue(inputModule) as IEnumerable;

                if (oldPointerDataEnumerableFieldInfo != null)
                {
                    this.UpdateOldInputSystem();
                }
                else
                if (newPointerDataEnumerableFieldInfo != null)
                {
                    this.UpdateNewInputSystem();
                }
            }
        }


//*====================
//* CALLBACKS
//*====================
        private void UpdateNewInputSystem()
        {
            FieldInfo firstPointerModelFieldInfo = pointerDataEnumerable.GetType().GetField("firstValue", BindingFlags.Instance | BindingFlags.Public);
            object pointerModel = firstPointerModelFieldInfo.GetValue(pointerDataEnumerable);
            FieldInfo eventDataFieldInfo = pointerModel.GetType().GetField("eventData", BindingFlags.Instance | BindingFlags.Public);
            this.currentPointer = eventDataFieldInfo.GetValue(pointerModel) as PointerEventData;

            this.UpdatePointerDataObjects();
        }

        private void UpdateOldInputSystem()
        {
            foreach (object obj in pointerDataEnumerable)
            {
                if (obj is KeyValuePair<int, PointerEventData>)
                {
                    KeyValuePair<int, PointerEventData> kvp = (KeyValuePair<int, PointerEventData>)obj;
                    this.currentPointer = kvp.Value;
                }
                else
                {
                    object pointerModel = obj;
                }

                this.UpdatePointerDataObjects();
            }
        }


//*====================
//* PRIVATE
//*====================
        private void UpdatePointerDataObjects()
        {
            if (currentPointer != null)
            {
                PointerEventDataDO currentPointerDO = DataObjectMasterRepository.GetDataObject<PointerEventDataDO>(PointerPredicate);
                if (currentPointerDO == null)
                {
                    currentPointerDO = new PointerEventDataDO(this.currentPointer);
                }

                currentPointerDO.Update();

                this.currentPointer = null;
            }
        }


//*====================
//* PREDICATE
//*====================
        private bool PointerPredicate(PointerEventDataDO obj)
        {
            return obj.PointerId.Value == currentPointer.pointerId;
        }
    }
}