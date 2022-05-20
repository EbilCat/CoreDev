using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCard : MonoBehaviour, ISpawnee, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private DataObjectInspectorDO dataObjectInspectorDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        private RearrangeableScrollViewItem rearrangeableScrollViewItem;
        private Text text;


//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            UnbindDO(this.inspectedObservableVarDO);
            UnbindDO(this.dataObjectInspectorDO);
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == null)
            {
                this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;
                this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
                this.observableVarInstance = inspectedObservableVarDO.ObservableVarInstance;

                this.name = this.observableVarInfoDO.Name;
                this.rearrangeableScrollViewItem = this.GetComponentInChildren<RearrangeableScrollViewItem>();
                rearrangeableScrollViewItem.RegisterForSiblingIndexChanged(OnSiblingIndexChanged);

                this.text = this.GetComponentInChildren<Text>();
                CompleteBinding();
            }

            if (dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == null)
            {
                this.dataObjectInspectorDO = dataObject as DataObjectInspectorDO;
                CompleteBinding();
            }
        }

        private void CompleteBinding()
        {
            if (this.inspectedObservableVarDO != null && this.dataObjectInspectorDO != null)
            {
                this.inspectedObservableVarDO.matchesFilter.RegisterForChanges(OnMatchesFilterChanged);
                this.inspectedObservableVarDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
                this.inspectedObservableVarDO.printToConsole.RegisterForChanges(OnPrintToConsoleChanged);

                this.observableVarInfoDO.isExpandedView.RegisterForChanges(OnIsExpandedViewChanged);
                this.observableVarInfoDO.orderIndex.RegisterForChanges(OnOrderIndexChanged);

                this.dataObjectInspectorDO.observableVarFilterString.RegisterForChanges(OnObservableVarFilterString);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject ||
                dataObject is DataObjectInspectorDO && this.dataObjectInspectorDO == dataObject as DataObjectInspectorDO)
            {
                this.rearrangeableScrollViewItem?.UnregisterFromSiblingIndexChanged(OnSiblingIndexChanged);

                this.inspectedObservableVarDO?.matchesFilter.UnregisterFromChanges(OnMatchesFilterChanged);
                this.inspectedObservableVarDO?.isInspected?.UnregisterFromChanges(OnIsInspectedChanged);
                this.inspectedObservableVarDO?.printToConsole.UnregisterFromChanges(OnPrintToConsoleChanged);

                this.observableVarInfoDO?.isExpandedView?.UnregisterFromChanges(OnIsExpandedViewChanged);
                this.observableVarInfoDO?.orderIndex?.UnregisterFromChanges(OnOrderIndexChanged);
                
                this.observableVarInfoDO?.UnregisterFromValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
                this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, RefreshDisplayedValues);
                this.observableVarInfoDO?.UnregisterFromModeratorsChanges(observableVarInstance, RefreshDisplayedValues);

                this.dataObjectInspectorDO.observableVarFilterString?.UnregisterFromChanges(OnObservableVarFilterString);

                this.rearrangeableScrollViewItem = null;
                this.observableVarInstance = null;

                this.inspectedObservableVarDO = null;
                this.observableVarInfoDO = null;
                this.dataObjectInspectorDO = null;
            }
        }


        //*====================
        //* CALLBACKS - RearrangeableScrollViewItem 
        //*====================
        private void OnSiblingIndexChanged(int obj)
        {
            this.observableVarInfoDO.orderIndex.Value = obj;
        }


//*====================
//* CALLBACKS - ObservableVarInfoDO 
//*====================
        private void OnOrderIndexChanged(ObservableVar<int> obj)
        {
            this.transform.SetSiblingIndex(obj.Value);
            this.name = this.observableVarInfoDO.Name + ":" + this.observableVarInfoDO.orderIndex.Value + ":" + this.transform.GetSiblingIndex();
        }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
        private void OnIsInspectedChanged(ObservableVar<bool> oIsInspected)
        {
            if (oIsInspected.Value)
            {
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, RefreshDisplayedValues);
                this.observableVarInfoDO.RegisterForModeratorsChanges(observableVarInstance, RefreshDisplayedValues);
                this.RefreshDisplayedValues();
            }
            else
            {
                this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, RefreshDisplayedValues);
                this.observableVarInfoDO?.UnregisterFromModeratorsChanges(observableVarInstance, RefreshDisplayedValues);
            }
        }

        private void OnPrintToConsoleChanged(ObservableVar<bool> obj)
        {
            if(obj.Value)
            {
                this.observableVarInfoDO.RegisterForValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
            }
            else
            {
                this.observableVarInfoDO?.UnregisterFromValueChangeBlocks(observableVarInstance, OnValueChangeBlocked);
            }
        }

        private void OnValueChangeBlocked(string moderatorName)
        {
            Debug.LogFormat("[Frame {0}] Change to {1} blocked by Moderator: {2}", Time.frameCount, this.observableVarInfoDO.Name, moderatorName);
        }

        private void OnIsExpandedViewChanged(ObservableVar<bool> obj)
        {
            this.RefreshDisplayedValues();
        }


//*====================
//* CALLBACKS
//*====================
        private void OnObservableVarFilterString(ObservableVar<string> obj)
        {
            try
            {
                string observableVarFilterString = this.dataObjectInspectorDO.observableVarFilterString.Value;
                Match result = Regex.Match(this.text.text, observableVarFilterString, RegexOptions.Singleline);
                this.inspectedObservableVarDO.matchesFilter.Value = result.Success;
            }
            catch
            {
                // Debug.LogWarning("Illegal Regex used in filter");
            }
        }

        private void OnMatchesFilterChanged(ObservableVar<bool> obj)
        {
            this.gameObject.SetActive(obj.Value);
        }


//*====================
//* IMPLEMENTATION - IPointerDownHandler
//*====================
        private bool potentialClick = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                potentialClick = true;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            potentialClick = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && potentialClick)
            {
                bool isExpandedView = this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value;
                this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value = !isExpandedView;
                potentialClick = false;
            }
        }


//*====================
//* PRIVATE
//*====================
        private void RefreshDisplayedValues()
        {
            if (inspectedObservableVarDO == null) { return; }

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            PropertyInfo propertyInfo = observableVarInfoDO.FieldType.GetProperty("Value");
            object currentValue = propertyInfo.GetValue(this.observableVarInstance);

            string displayText = string.Empty;

            if (observableVarInfoDO.IsCollection)
            {
                ICollection collection = currentValue as ICollection;
                displayText = $"(COLLECTION) {this.inspectedObservableVarDO.varName.Value} Count: {collection.Count}";

                if (this.inspectedObservableVarDO.ObservableVarInfoDO.isExpandedView.Value)
                {
                    int index = 0;
                    foreach (var item in collection)
                    {
                        displayText += $"\r\n{index}: {item}";
                        index++;
                    }
                }
            }
            else
            {
                displayText = $"{inspectedObservableVarDO.varName.Value} : {inspectedObservableVarDO.ObservableVarInstance}";
            }

            FieldInfo moderatorListFieldInfo = inspectedObservableVarDO.ObservableVarInfoDO.FieldType.GetField("moderators", bindingFlags);
            if (moderatorListFieldInfo != null)
            {
                IEnumerable moderators = moderatorListFieldInfo.GetValue(this.inspectedObservableVarDO.ObservableVarInstance) as IEnumerable;

                foreach (object moderatorListObj in moderators)
                {
                    PropertyInfo keyProp = moderatorListObj.GetType().GetProperty("Key");
                    int priority = (int)keyProp.GetValue(moderatorListObj);

                    PropertyInfo valProp = moderatorListObj.GetType().GetProperty("Value");
                    IEnumerable moderatorList = valProp.GetValue(moderatorListObj) as IEnumerable;

                    foreach (object moderator in moderatorList)
                    {
                        PropertyInfo methodProperty = moderator.GetType().GetProperty("Method", bindingFlags);
                        object moderatorMethodObj = methodProperty.GetValue(moderator);
                        PropertyInfo namePropertyInfo = moderatorMethodObj.GetType().GetProperty("Name", bindingFlags);
                        displayText += $"\nModerator {priority}: {namePropertyInfo.GetValue(moderatorMethodObj)}";
                    }
                }
            }

            if (this.inspectedObservableVarDO.printToConsole.Value)
            {
                Debug.LogFormat("[Frame {0}] {1}", Time.frameCount, displayText);
            }

            this.text.text = displayText;
        }
    }
}