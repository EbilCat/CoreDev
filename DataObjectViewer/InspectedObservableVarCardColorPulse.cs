using System;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;
using UnityEngine.UI;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarCardColorPulse : MonoBehaviour, ISpawnee
    {
        private InspectedObservableVarDO inspectedObservableVarDO;
        private ObservableVarInfoDO observableVarInfoDO;
        private IObservableVar observableVarInstance;
        private Image image;
        [SerializeField] private Color valueChangeBlockedPulseColor = Color.red;
        [SerializeField] private Color moderatorsChangedPulseColor = new Color(1.0f, 0.75f, 0.0f); //Orange
        [SerializeField] private Color valueChangedPulseColor = Color.green;
        private Color activePulseColor;
        private Color pulseInactiveColor = Color.gray;
        [SerializeField] private float fadeDurationSecs = 0.5f;
        private float fadeProgress01 = 0.0f;


//*====================
//* IHasTimeElapsedHandler
//*====================
        public void TimeElapsed(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            fadeProgress01 += (unscaledDeltaTime / fadeDurationSecs);
            fadeProgress01 = Mathf.Clamp01(fadeProgress01);
            this.image.color = Color.Lerp(activePulseColor, pulseInactiveColor, fadeProgress01);

            if (Mathf.Approximately(fadeProgress01, 1.0f))
            {
                UniversalTimer.UnregisterFromTimeElapsed(TimeElapsed);
            }
        }


//*====================
//* UNITY
//*====================
        private void OnDestroy()
        {
            this.UnbindDO(inspectedObservableVarDO);
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

                this.image = this.GetComponent<Image>();
                this.pulseInactiveColor = this.image.color;

                this.inspectedObservableVarDO.isInspected.RegisterForChanges(OnIsInspectedChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is InspectedObservableVarDO && this.inspectedObservableVarDO == dataObject)
            {
                this.inspectedObservableVarDO.isInspected.UnregisterFromChanges(OnIsInspectedChanged);

                this.observableVarInfoDO.UnregisterFromValueChangeBlocks(observableVarInstance, StartValueChangeBlockedPulse);
                this.observableVarInfoDO.UnregisterFromValueChanges(observableVarInstance, StartValueChangedPulse);
                this.observableVarInfoDO.UnregisterFromModeratorsChanges(observableVarInstance, StartModeratorsChangedPulse);
                this.ResetFadeToInactive();

                this.inspectedObservableVarDO = null;
                this.observableVarInfoDO = null;
                this.observableVarInstance = null;

                UniversalTimer.UnregisterFromTimeElapsed(TimeElapsed);
            }
        }


//*====================
//* CALLBACKS - InspectedDataObjectDO
//*====================
        private void OnIsInspectedChanged(ObservableVar<bool> oIsInspected)
        {
            if (oIsInspected.Value)
            {
                this.observableVarInfoDO.RegisterForValueChangeBlocks(observableVarInstance, StartValueChangeBlockedPulse);
                this.observableVarInfoDO.RegisterForValueChanges(observableVarInstance, StartValueChangedPulse);
                this.observableVarInfoDO.RegisterForModeratorsChanges(observableVarInstance, StartModeratorsChangedPulse);
            }
            else
            {
                this.observableVarInfoDO?.UnregisterFromValueChangeBlocks(observableVarInstance, StartValueChangeBlockedPulse);
                this.observableVarInfoDO?.UnregisterFromValueChanges(observableVarInstance, StartValueChangedPulse);
                this.observableVarInfoDO?.UnregisterFromModeratorsChanges(observableVarInstance, StartModeratorsChangedPulse);
                this.ResetFadeToInactive();
            }
        }

        private void StartValueChangeBlockedPulse(string moderatorName)
        {
            this.activePulseColor = valueChangeBlockedPulseColor;
            this.StartFade();
        }

        private void StartValueChangedPulse()
        {
            this.activePulseColor = valueChangedPulseColor;
            this.StartFade();
        }

        private void StartModeratorsChangedPulse()
        {
            this.activePulseColor = moderatorsChangedPulseColor;
            this.StartFade();
        }


//*====================
//* PRIVATE
//*====================
        private void StartFade()
        {
            this.fadeProgress01 = 0.0f;
            this.image.color = activePulseColor;
            UniversalTimer.RegisterForTimeElapsed(TimeElapsed);
        }

        private void ResetFadeToInactive()
        {
            this.fadeProgress01 = 1.0f;
            this.image.color = pulseInactiveColor;
            UniversalTimer.UnregisterFromTimeElapsed(TimeElapsed);
        }
    }
}