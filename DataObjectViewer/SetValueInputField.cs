using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CoreDev.DataObjectInspector
{
    public class SetValueInputField : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private InspectedObservableVarSetValue inspectedObservableVarSetValue;
        [SerializeField] private InputAction submit;


//*====================
//* INTERFACES
//*====================
        public void OnSelect(BaseEventData eventData)
        {
            this.submit.Enable();
            this.submit.performed += OnSubmitPerformed;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.submit.Disable();
            this.submit.performed -= OnSubmitPerformed;
        }


//*====================
//* CALLBACKS
//*====================
        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            this.inspectedObservableVarSetValue.Submit();
        }
    }
}