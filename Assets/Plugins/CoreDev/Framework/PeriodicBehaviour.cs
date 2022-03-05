using UnityEngine;

public abstract class PeriodicBehaviour : MonoBehaviour
{
    public bool isFixedFrequency = true;
    public bool isTimeScaleDependent = false;

    [SerializeField]
    protected float frequencyHz = 1.0f;
    
    protected float timeOffset;
    protected float timeSinceLastAction = 0.0f;
    protected float TimeTillNextActionSecs { get { return ((1f / frequencyHz) - timeOffset); } }

    /// <summary>
    /// If isFixedFrequency isfalse, DoPeriodicAction on every Update frame
    /// </summary>
    protected virtual void Update()
    {
        if (!isFixedFrequency)
        {
            DoPeriodicAction();
        }
        else
        {
            timeSinceLastAction += isTimeScaleDependent ? Time.deltaTime : Time.unscaledDeltaTime;
            if (IsTimeToAct())
            {
                DoPeriodicAction();
                timeSinceLastAction = 0.0f;
            }
        }
    }

    protected abstract void DoPeriodicAction();

    protected virtual bool IsTimeToAct()
    {
        bool timeToAct = (timeSinceLastAction >= TimeTillNextActionSecs);
        return timeToAct;
    }
}
