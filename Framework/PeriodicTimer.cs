using System;

public class PeriodicTimer : PeriodicBehaviour
{
    public event Action TimeToAct = delegate { };

    protected override void DoPeriodicAction() { TimeToAct(); }
}
