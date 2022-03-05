using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ScriptExecutionOrder : Attribute
{
    public readonly int executionIndex;
    public ScriptExecutionOrder(int executionIndex)
    {
        this.executionIndex = executionIndex;
    }
}
