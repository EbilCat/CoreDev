using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine.EventSystems;

namespace CoreDev.DataObjectInspector.AddOns
{
    public class PointerEventDataDO : IDataObject
    {
        private PointerEventData pointerEventData;
        [Bookmark]
        private OInt pointerId;
        public OInt PointerId => pointerId;

        private OBool useDragThreshold;
        public OBool UseDragThreshold => useDragThreshold;

        private OBool dragging;
        public OBool Dragging => dragging;

        private OInputButton button;
        public OInputButton Button => button;

        private OFloat pressure;
        public OFloat Pressure => pressure;

        private OFloat tangentialPressure;
        public OFloat TangentialPressure => tangentialPressure;

        private OFloat altitudeAngle;
        public OFloat AltitudeAngle => altitudeAngle;

        private OFloat azimuthAngle;
        public OFloat AzimuthAngle => azimuthAngle;

        private OFloat twist;
        public OFloat Twist => twist;

        private OVector2 radius;
        public OVector2 Radius => radius;

        private OVector2 radiusVariance;
        public OVector2 RadiusVariance => radiusVariance;

        private OBool fullyExited;
        public OBool FullyExited => fullyExited;

        private OBool reentered;
        public OBool Reentered => reentered;

        private OCamera enterEventCamera;
        public OCamera EnterEventCamera => enterEventCamera;

        private OCamera pressEventCamera;
        public OCamera PressEventCamera => pressEventCamera;

        private OGameObject pointerPress;
        public OGameObject PointerPress => pointerPress;

        private OVector2 scrollDelta;
        public OVector2 ScrollDelta => scrollDelta;

        private OInt clickCount;
        public OInt ClickCount => clickCount;

        private OFloat clickTime;
        public OFloat ClickTime => clickTime;

        private OGameObject pointerEnter;
        public OGameObject PointerEnter => pointerEnter;

        private OGameObject lastPress;
        public OGameObject LastPress => lastPress;

        private OGameObject rawPointerPress;
        public OGameObject RawPointerPress => rawPointerPress;

        private OGameObject pointerDrag;
        public OGameObject PointerDrag => pointerDrag;

        private OGameObject pointerClick;
        public OGameObject PointerClick => pointerClick;

        private OVector2 position;
        public OVector2 Position => position;

        private OVector2 delta;
        public OVector2 Delta => delta;

        private OVector2 pressPosition;
        public OVector2 PressPosition => pressPosition;

        private OBool eligibleForClick;
        public OBool EligibleForClick => eligibleForClick;

        private OBool isPointerMoving;
        public OBool IsPointerMoving => isPointerMoving;

        private OBool isScrolling;
        public OBool IsScrolling => isScrolling;

        private ORaycastResult pointerCurrentRaycast;
        public ORaycastResult PointerCurrentRaycast => pointerCurrentRaycast;

        private ORaycastResult pointerPressRaycast;


        public ORaycastResult PointerPressRaycast => pointerPressRaycast;


        public PointerEventDataDO(PointerEventData pointerEventData)
        {
            this.pointerEventData = pointerEventData;

            this.pointerId = new OInt(pointerEventData.pointerId, this);
            this.useDragThreshold = new OBool(pointerEventData.useDragThreshold, this);
            this.dragging = new OBool(pointerEventData.dragging, this);
            this.button = new OInputButton(pointerEventData.button, this);
            this.pressure = new OFloat(pointerEventData.pressure, this);
            this.tangentialPressure = new OFloat(pointerEventData.tangentialPressure, this);
            this.altitudeAngle = new OFloat(pointerEventData.altitudeAngle, this);
            this.azimuthAngle = new OFloat(pointerEventData.azimuthAngle, this);
            this.twist = new OFloat(pointerEventData.twist, this);
            this.radius = new OVector2(pointerEventData.radius, this);
            this.radiusVariance = new OVector2(pointerEventData.radiusVariance, this);
            this.fullyExited = new OBool(pointerEventData.fullyExited, this);
            this.reentered = new OBool(pointerEventData.reentered, this);
            this.enterEventCamera = new OCamera(pointerEventData.enterEventCamera, this);
            this.pressEventCamera = new OCamera(pointerEventData.pressEventCamera, this);
            this.pointerPress = new OGameObject(pointerEventData.pointerPress, this);
            this.scrollDelta = new OVector2(pointerEventData.scrollDelta, this);
            this.clickCount = new OInt(pointerEventData.clickCount, this);
            this.clickTime = new OFloat(pointerEventData.clickTime, this);
            this.pointerEnter = new OGameObject(pointerEventData.pointerEnter, this);
            this.lastPress = new OGameObject(pointerEventData.lastPress, this);
            this.rawPointerPress = new OGameObject(pointerEventData.rawPointerPress, this);
            this.pointerDrag = new OGameObject(pointerEventData.pointerDrag, this);
            this.pointerCurrentRaycast = new ORaycastResult(pointerEventData.pointerCurrentRaycast, this);
            this.pointerPressRaycast = new ORaycastResult(pointerEventData.pointerPressRaycast, this);
            this.pointerClick = new OGameObject(pointerEventData.pointerClick, this);
            this.position = new OVector2(pointerEventData.position, this);
            this.delta = new OVector2(pointerEventData.delta, this);
            this.pressPosition = new OVector2(pointerEventData.pressPosition, this);
            this.eligibleForClick = new OBool(pointerEventData.eligibleForClick, this);
            this.isPointerMoving = new OBool(pointerEventData.IsPointerMoving(), this);
            this.isScrolling = new OBool(pointerEventData.IsScrolling(), this);

            DataObjectMasterRepository.RegisterDataObject(this);
        }

        public void Update()
        {
            this.pointerId.Value = pointerEventData.pointerId;
            this.useDragThreshold.Value = pointerEventData.useDragThreshold;
            this.dragging.Value = pointerEventData.dragging;
            this.button.Value = pointerEventData.button;
            this.pressure.Value = pointerEventData.pressure;
            this.tangentialPressure.Value = pointerEventData.tangentialPressure;
            this.altitudeAngle.Value = pointerEventData.altitudeAngle;
            this.azimuthAngle.Value = pointerEventData.azimuthAngle;
            this.twist.Value = pointerEventData.twist;
            this.radius.Value = pointerEventData.radius;
            this.radiusVariance.Value = pointerEventData.radiusVariance;
            this.fullyExited.Value = pointerEventData.fullyExited;
            this.reentered.Value = pointerEventData.reentered;
            this.enterEventCamera.Value = pointerEventData.enterEventCamera;
            this.pressEventCamera.Value = pointerEventData.pressEventCamera;
            this.pointerPress.Value = pointerEventData.pointerPress;
            this.scrollDelta.Value = pointerEventData.scrollDelta;
            this.clickCount.Value = pointerEventData.clickCount;
            this.clickTime.Value = pointerEventData.clickTime;
            this.pointerEnter.Value = pointerEventData.pointerEnter;
            this.lastPress.Value = pointerEventData.lastPress;
            this.rawPointerPress.Value = pointerEventData.rawPointerPress;
            this.pointerDrag.Value = pointerEventData.pointerDrag;
            this.pointerCurrentRaycast.Value = pointerEventData.pointerCurrentRaycast;
            this.pointerPressRaycast.Value = pointerEventData.pointerPressRaycast;
            this.pointerClick.Value = pointerEventData.pointerClick;
            this.position.Value = pointerEventData.position;
            this.delta.Value = pointerEventData.delta;
            this.pressPosition.Value = pointerEventData.pressPosition;
            this.eligibleForClick.Value = pointerEventData.eligibleForClick;
            this.isPointerMoving.Value = pointerEventData.IsPointerMoving();
            this.isScrolling.Value = pointerEventData.IsScrolling();
        }


        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }
    }
}