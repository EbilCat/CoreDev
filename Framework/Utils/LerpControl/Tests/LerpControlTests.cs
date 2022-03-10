using CoreDev.Utils;
using UnityEngine;

public class LerpControlTests : MonoBehaviour
{
    [ContextMenu("RunTests")]
    protected void RunTests()
    {
        Test_ExcessProgress_DirectionForward_LoopRestart_ProgressIsOne();
        Test_ExcessProgress_DirectionForward_LoopYoyo_ProgressIsOne();

        Test_ExcessProgress_DirectionForward_LoopRestart_ExcessCarriedOver();
        Test_ExcessProgress_DirectionBackwards_LoopRestart_ExcessCarriedOver();
        Test_ExcessProgress_DirectionForward_LoopYoyo_ExcessCarriedOver();
        Test_ExcessProgress_DirectionBackwards_LoopYoyo_ExcessCarriedOver();

        Test_LerpCompletion_DirectionForward_LoopRestart_CallbackFired();
        Test_LerpCompletion_DirectionBackwards_LoopRestart_CallbackFired();
        Test_LerpCompletion_DirectionForward_LoopYoyo_CallbackFired();
        Test_LerpCompletion_DirectionBackwards_LoopYoyo_CallbackFired();
        
        Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_ProgressIsZero();
        Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_CallbackFired();
        Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_ProgressIsZero();
        Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_CallbackFired();
        Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_ProgressIsZero();
        Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_CallbackFired();
        Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_ProgressIsZero();
        Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_CallbackFired();
        
        Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_ProgressIsOne();
        Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_CallbackFired();
        Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_ProgressIsOne();
        Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_CallbackFired();
        Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_ProgressIsOne();
        Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_CallbackFired();
        Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_ProgressIsOne();
        Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_CallbackFired();
    }

    private void Test_ExcessProgress_DirectionForward_LoopRestart_ProgressIsOne()
    {
        //Setup
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(1);
        lerpFloat.IsPlaying(true);
        lerpFloat.Pulse(1.25f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 1.0f))
        {
            Debug.Log("Test_ExcessProgress_DirectionForward_LoopRestart_ProgressIsOne passed");
        }
        else
        {
            Debug.LogError("Test_ExcessProgress_DirectionForward_LoopRestart_ProgressIsOne failed");
        }
    }

    private void Test_ExcessProgress_DirectionForward_LoopYoyo_ProgressIsOne()
    {
        //Setup
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(1);
        lerpFloat.IsPlaying(true);
        lerpFloat.Pulse(1.25f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 1.0f))
        {
            Debug.Log("Test_ExcessProgress_DirectionForward_LoopYoyo_ProgressIsOne passed");
        }
        else
        {
            Debug.LogErrorFormat("Test_ExcessProgress_DirectionForward_LoopYoyo_ProgressIsOne failed. \r\nExpected:{0} Result:{1}", 1.0f, lerpFloat.LerpProgress01());
        }
    }

    private void Test_ExcessProgress_DirectionForward_LoopRestart_ExcessCarriedOver()
    {
        //Setup
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(2);
        lerpFloat.IsPlaying(true);
        lerpFloat.Pulse(1.25f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.25f))
        {
            Debug.Log("Test_ExcessProgress_DirectionForward_LoopRestart_ExcessCarriedOver passed");
        }
        else
        {
            Debug.LogError("Test_ExcessProgress_DirectionForward_LoopRestart_ExcessCarriedOver failed");
        }
    }
    
    private void Test_ExcessProgress_DirectionBackwards_LoopRestart_ExcessCarriedOver()
    {
        //Setup
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(2);
        lerpFloat.IsPlaying(true);
        lerpFloat.Pulse(1.25f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.75f))
        {
            Debug.Log("Test_ExcessProgress_DirectionBackwards_LoopRestart_ExcessCarriedOver passed");
        }
        else
        {
            Debug.LogErrorFormat("Test_ExcessProgress_DirectionBackwards_LoopRestart_ExcessCarriedOver failed\r\n Expected:{0} Result:{1}", 0.75f, lerpFloat.LerpProgress01());
        }
    }

    private void Test_ExcessProgress_DirectionForward_LoopYoyo_ExcessCarriedOver()
    {
        //Setup
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(2);
        lerpFloat.IsPlaying(true);
        lerpFloat.Pulse(1.25f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.75f))
        {
            Debug.Log("Test_ExcessProgress_DirectionForward_LoopYoyo_ExcessCarriedOver passed");
        }
        else
        {
            Debug.LogErrorFormat("Test_ExcessProgress_DirectionForward_LoopYoyo_ExcessCarriedOver failed. \r\nExpected:{0} Result:{1}", 0.75f, lerpFloat.LerpProgress01());
        }
    }
    
    private void Test_ExcessProgress_DirectionBackwards_LoopYoyo_ExcessCarriedOver()
    {
        //Setup
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(2);
        lerpFloat.IsPlaying(true);
        lerpFloat.Pulse(1.25f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.25f))
        {
            Debug.Log("Test_ExcessProgress_DirectionBackwards_LoopYoyo_ExcessCarriedOver passed");
        }
        else
        {
            Debug.LogError("Test_ExcessProgress_DirectionBackwards_LoopYoyo_ExcessCarriedOver failed");
        }
    }

    private void Test_LerpCompletion_DirectionForward_LoopRestart_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(1);
        lerpFloat.IsPlaying(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        float lerpResult = lerpFloat.Pulse(1.0f);
        if (callbackFired)
        {
            Debug.Log("Test_LerpCompletion_DirectionForward_LoopRestart_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_LerpCompletion_DirectionForward_LoopRestart_CallbackFired failed");
        }
    }

    private void Test_LerpCompletion_DirectionBackwards_LoopRestart_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(1);
        lerpFloat.IsPlaying(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        float lerpResult = lerpFloat.Pulse(1.0f);
        if (callbackFired)
        {
            Debug.Log("Test_LerpCompletion_DirectionBackwards_LoopRestart_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_LerpCompletion_DirectionBackwards_LoopRestart_CallbackFired failed");
        }
    }

    private void Test_LerpCompletion_DirectionForward_LoopYoyo_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(1);
        lerpFloat.IsPlaying(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        float lerpResult = lerpFloat.Pulse(1.0f);
        if (callbackFired)
        {
            Debug.Log("Test_LerpCompletion_DirectionForward_LoopYoyo_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_LerpCompletion_DirectionForward_LoopYoyo_CallbackFired failed");
        }
    }
    
    private void Test_LerpCompletion_DirectionBackwards_LoopYoyo_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(1);
        lerpFloat.IsPlaying(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        float lerpResult = lerpFloat.Pulse(1.0f);
        if (callbackFired)
        {
            Debug.Log("Test_LerpCompletion_DirectionBackwards_LoopYoyo_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_LerpCompletion_DirectionBackwards_LoopYoyo_CallbackFired failed");
        }
    }

    private void Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_ProgressIsZero()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.0f))
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_ProgressIsZero passed");
        }
        else
        {
            Debug.LogErrorFormat("Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_ProgressIsZero failed.\r\nExpected:{0} Result:{1}", 0.0f, lerpFloat.LerpProgress01());
        }
    }

    private void Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressZero_DirectionForward_LoopRestart_CallbackFired failed");
        }
    }

    private void Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_ProgressIsZero()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.0f))
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_ProgressIsZero passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_ProgressIsZero failed");
        }
    }

    private void Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressZero_DirectionBackwards_LoopRestart_CallbackFired failed");
        }
    }

    private void Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_ProgressIsZero()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.0f))
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_ProgressIsZero passed");
        }
        else
        {
            Debug.LogErrorFormat("Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_ProgressIsZero failed.\r\nExpected:{0} Result:{1}", 0.0f, lerpFloat.LerpProgress01());
        }
    }

    private void Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressZero_DirectionForward_LoopYoyo_CallbackFired failed");
        }
    }
    
    private void Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_ProgressIsZero()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 0.0f))
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_ProgressIsZero passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_ProgressIsZero failed");
        }
    }
    
    
    private void Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsZero(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressZero_DirectionBackwards_LoopYoyo_CallbackFired failed");
        }
    }

    private void Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_ProgressIsOne()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 1.0f))
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_ProgressIsOne passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_ProgressIsOne failed");
        }
    }

    private void Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionForward_LoopRestart_CallbackFired failed");
        }
    }

    private void Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_ProgressIsOne()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 1.0f))
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_ProgressIsOne passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_ProgressIsOne failed");
        }
    }
    
    private void Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.RESTART);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionBackwards_LoopRestart_CallbackFired failed");
        }
    }

    private void Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_ProgressIsOne()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 1.0f))
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_ProgressIsOne passed");
        }
        else
        {
            Debug.LogErrorFormat("Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_ProgressIsOne failed\r\nExpected:{0} Result{1}", 1.0f, lerpFloat.LerpProgress01());
        }
    }

    private void Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(0.0f);
        lerpFloat.Direction(LerpDirection.FORWARD);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionForward_LoopYoyo_CallbackFired failed");
        }
    }
    
    private void Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_ProgressIsOne()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);

        lerpFloat.Pulse(5.5f);

        if (Mathf.Approximately(lerpFloat.LerpProgress01(), 1.0f))
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_ProgressIsOne passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_ProgressIsOne failed");
        }
    }
    
    private void Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_CallbackFired()
    {
        LerpFloat lerpFloat = new LerpFloat(0.0f);
        lerpFloat.StartValue(0.0f);
        lerpFloat.EndValue(10.0f);
        lerpFloat.LerpProgress01(1.0f);
        lerpFloat.Direction(LerpDirection.REVERSE);
        lerpFloat.LoopType(LoopTypes.YOYO);
        lerpFloat.TimesToLoop(20);
        lerpFloat.IsPlaying(true);
        lerpFloat.ConcludeWhenProgressIsOne(true);
        bool callbackFired = false;
        lerpFloat.lerpCompleted += () => { callbackFired = true; };

        lerpFloat.Pulse(5.5f);

        if (callbackFired)
        {
            Debug.Log("Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_CallbackFired passed");
        }
        else
        {
            Debug.LogError("Test_ConcludeOnProgressOne_DirectionBackwards_LoopYoyo_CallbackFired failed");
        }
    }
}
