using CoreDev.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ExampleLerp : MonoBehaviour
{
    public Image icon;
    public Color start = Color.red;
    public Color end = Color.green;
    public float durationSecs = 1.0f;
    public LerpDirection direction = LerpDirection.FORWARD;
    public bool isPlaying = false;
    public AnimationCurve animationCurve;
    public LoopTypes loopType = LoopTypes.RESTART;
    public int numTimeToPlaythrough = 1;

    private LerpColor lerpControl = new LerpColor(Color.white);


//*====================
//* UNITY
//*====================
    private void Start()
    {
        this.ApplySettings();
        lerpControl.lerpCompleted += OnLerpCompleted;
    }

    private void OnDestroy()
    {
        lerpControl.lerpCompleted -= OnLerpCompleted;
    }

    private void Update()
    {
        this.icon.color = this.lerpControl.Pulse(Time.deltaTime / this.durationSecs);
    }


//*====================
//* CALLBACKS
//*====================
    private void OnLerpCompleted()
    {
        Debug.Log("Lerp Completed");
    }


//*====================
//* PUBLIC
//*====================
    [ContextMenu("ApplySettings")]
    public void ApplySettings()
    {
        lerpControl
            .StartValue(this.start)
            .EndValue(this.end)
            .IsPlaying(isPlaying)
            .Direction(direction)
            .LerpProgress01((direction == LerpDirection.FORWARD) ? 0.0f : 1.0f)
            .TimesToLoop(numTimeToPlaythrough)
            .AnimCurve(animationCurve)
            .LoopType(loopType);

    }

    [ContextMenu("Play")]
    public void Play()
    {
        this.lerpControl.IsPlaying(true);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        this.lerpControl.IsPlaying(false);
    }

    [ContextMenu("ReversePlayDirection")]
    public void ReversePlayDirection()
    {
        LerpDirection direction = (this.lerpControl.Direction() == LerpDirection.FORWARD) ? LerpDirection.REVERSE : LerpDirection.FORWARD;
        this.lerpControl.Direction(direction);
    }

    [ContextMenu("CompleteOnNextStart")]
    public void CompleteOnNextStart()
    {
        this.lerpControl.ConcludeWhenProgressIsZero(true);
    }

    [ContextMenu("CompleteOnNextEnd")]
    public void CompleteOnNextEnd()
    {
        this.lerpControl.ConcludeWhenProgressIsOne(true);
    }
}
