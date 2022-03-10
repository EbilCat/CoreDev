using System.Collections;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InspectedObservableVarCard : MonoBehaviour, ISpawnee, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
{
    private InspectedObservableVarDO inspectedObservableVarDO;
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
    }


//*====================
//* BINDING
//*====================
    public void BindDO(IDataObject dataObject)
    {
        if (dataObject is InspectedObservableVarDO)
        {
            UnbindDO(this.inspectedObservableVarDO);

            this.inspectedObservableVarDO = dataObject as InspectedObservableVarDO;
            this.observableVarInfoDO = inspectedObservableVarDO.ObservableVarInfoDO;
            this.observableVarInstance = inspectedObservableVarDO.ObservableVarInstance;

            this.name = this.observableVarInfoDO.Name;
            this.rearrangeableScrollViewItem = this.GetComponentInChildren<RearrangeableScrollViewItem>();
            rearrangeableScrollViewItem.RegisterForSiblingIndexChanged(OnSiblingIndexChanged);

            this.text = this.GetComponentInChildren<Text>();

            this.inspectedObservableVarDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            this.observableVarInfoDO.isExpandedView.RegisterForChanges(OnIsExpandedViewChanged);
            this.observableVarInfoDO.orderIndex.RegisterForChanges(OnOrderIndexChanged);

        }
    }

    private void OnSiblingIndexChanged(int obj)
    {
        this.observableVarInfoDO.orderIndex.Value = obj;
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
        {
            this.rearrangeableScrollViewItem.UnregisterFromSiblingIndexChanged(OnSiblingIndexChanged);

            this.inspectedObservableVarDO?.isInspected.UnregisterFromChanges(OnIsInspectedChanged);
            this.observableVarInfoDO?.isExpandedView.UnregisterFromChanges(OnIsExpandedViewChanged);
            this.observableVarInfoDO?.orderIndex.UnregisterFromChanges(OnOrderIndexChanged);

            this.observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshDisplayedValues);
            this.observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, RefreshDisplayedValues);

            this.inspectedObservableVarDO = null;
            this.observableVarInfoDO = null;
            this.observableVarInstance = null;
        }
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
            this.observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, RefreshDisplayedValues);
            this.observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, RefreshDisplayedValues);
        }
    }

    private void OnIsExpandedViewChanged(ObservableVar<bool> obj)
    {
        this.RefreshDisplayedValues();
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
                    displayText += string.Format("\r\n{0}: {1}", index, item.ToString());
                    index++;
                }
            }
        }
        else
        {
            displayText = $"{this.inspectedObservableVarDO.varName.Value} : {currentValue}";
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