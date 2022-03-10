using System;
using UnityEngine;

namespace CoreDev.Utils
{
    public enum LoopTypes { RESTART, YOYO }
    public enum LerpDirection { FORWARD, REVERSE }

    public abstract class LerpControl<T>
    {
        public const int PlayInfinitely = -1;

        private T startValue = default(T);
        private T endValue = default(T);
        private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1.0f, 1.0f);
        private LerpDirection lerpDirection = LerpDirection.FORWARD;
        private bool isPlaying = false;
        private LoopTypes loopType = LoopTypes.RESTART;
        private int timesToLoop = 1;
        private float lerpProgress01 = 1.0f;


        private bool concludeOnProgressZero = false;
        private bool concludeOnProgressOne = false;
        private bool isForceConclude = false;

        public event Action lerpCompleted = delegate { };

//*====================
//* SETTERS
//*====================
        public LerpControl<T> StartValue(T start) { this.startValue = start; return this; }
        public LerpControl<T> EndValue(T end) { this.endValue = end; return this; }
        public LerpControl<T> AnimCurve(AnimationCurve animationCurve) { this.animationCurve = animationCurve; return this; }
        public LerpControl<T> IsPlaying(bool isPlaying) { this.isPlaying = isPlaying; return this; }
        public LerpControl<T> LoopType(LoopTypes loopType) { this.loopType = loopType; return this; }
        public LerpControl<T> TimesToLoop(int timesToLoop) { this.timesToLoop = timesToLoop; return this; }
        public LerpControl<T> LerpProgress01(float lerpProgress01) { this.lerpProgress01 = lerpProgress01; return this; }
        public LerpControl<T> Direction(LerpDirection playForward, bool resetProgress = false)
        {
            this.lerpDirection = playForward;
            if (resetProgress) { lerpProgress01 = (playForward == LerpDirection.FORWARD) ? 0.0f : 1.0f; }
            return this;
        }


//*====================
//* GETTERS
//*====================
        public T StartValue() { return startValue; }
        public T EndValue() { return endValue; }
        public AnimationCurve AnimCurve() { return animationCurve; }
        public bool IsPlaying() { return isPlaying; }
        public LoopTypes LoopType() { return loopType; }
        public int TimesToLoop() { return timesToLoop; }
        public float LerpProgress01() { return lerpProgress01; }
        public LerpDirection Direction() { return lerpDirection; }

        public LerpControl(T defaultValue)
        {
            this.StartValue(defaultValue);
            this.EndValue(defaultValue);
        }

        public T Pulse(float deltaProgress01)
        {
            if (ShouldKeepPlaying())
            {
                //Add deltaProgress to current progress
                this.lerpProgress01 += ((IsPlayingForward()) ? deltaProgress01 : -deltaProgress01); //Progress with excess
                ProcessProgress();
            }

            float curveValue = animationCurve.Evaluate(lerpProgress01);
            T result = PerformLerp(this.startValue, this.endValue, curveValue);

            if (isPlaying && ShouldKeepPlaying() == false)
            {
                ConcludeLerp();
            }
            return result;
        }

        private void ProcessProgress()
        {
            if (IsEndOfLoop())
            {
                if (timesToLoop != PlayInfinitely) { this.timesToLoop -= 1; } //Reduce timesToLoop if this is not infinite lerp

                if (isForceConclude || this.timesToLoop == 0)
                {
                    if (this.loopType == LoopTypes.RESTART)
                    {
                        if (concludeOnProgressOne)
                        {
                            this.lerpProgress01 = 1.0f;
                            this.timesToLoop = 0;
                            return;
                        }
                        if (concludeOnProgressZero)
                        {
                            this.lerpProgress01 = 0.0f;
                            this.timesToLoop = 0;
                            return;
                        }
                        this.lerpProgress01 = Mathf.Clamp01(this.lerpProgress01);
                        return;
                    }

                    if (this.loopType == LoopTypes.YOYO)
                    {
                        if (concludeOnProgressOne && this.lerpProgress01 >= 1.0f && IsPlayingForward())
                        {
                            this.lerpProgress01 = 1.0f;
                            this.timesToLoop = 0;
                            return;
                        }

                        if (concludeOnProgressZero && this.lerpProgress01 <= 0.0f && IsPlayingReverse())
                        {
                            this.lerpProgress01 = 0.0f;
                            this.timesToLoop = 0;
                            return;
                        }

                        if (concludeOnProgressZero == true || concludeOnProgressOne == true)
                        {
                            this.timesToLoop = 1; //Loop one more time to conclude on desired conclusion value
                        }
                        else
                        {
                            this.lerpProgress01 = Mathf.Clamp01(this.lerpProgress01);
                            return;
                        }
                    }
                }

                { //Calculate Excess
                    if (this.lerpProgress01 >= 1.0f)
                    {
                        this.lerpProgress01 -= 1.0f;
                        if (this.loopType == LoopTypes.YOYO) { this.lerpDirection = LerpDirection.REVERSE; }
                    }
                    if (this.lerpProgress01 <= 0.0f)
                    {
                        this.lerpProgress01 = Mathf.Abs(this.lerpProgress01);
                        if (this.loopType == LoopTypes.YOYO) { this.lerpDirection = LerpDirection.FORWARD; }
                    }
                    if (IsPlayingReverse()) { this.lerpProgress01 = 1 - this.lerpProgress01; }
                }

                if (this.lerpProgress01 > 1.0f || this.lerpProgress01 < 0.0f)
                {
                    ProcessProgress();
                }
            }
        }

        public void ConcludeWhenProgressIsZero(bool forceConclude)
        {
            this.isForceConclude = forceConclude;
            this.concludeOnProgressZero = true;
        }

        public void ConcludeWhenProgressIsOne(bool forceConclude)
        {
            this.isForceConclude = true;
            this.concludeOnProgressOne = true;
        }

        protected abstract T PerformLerp(T start, T end, float progress01);


        //*====================
        //* PRIVATE
        //*====================
        private bool ShouldKeepPlaying()
        {
            // Debug.LogFormat("timesToLoop:{0} isPlaying:{1}", timesToLoop, isPlaying);
            bool shouldKeepPlaying = (timesToLoop > 0 || timesToLoop == PlayInfinitely);
            shouldKeepPlaying &= isPlaying;
            return shouldKeepPlaying;
        }

        private void ConcludeLerp()
        {
            this.isPlaying = false;
            this.concludeOnProgressZero = false;
            this.concludeOnProgressOne = false;
            this.isForceConclude = false;
            this.lerpCompleted();
        }

        private bool IsPlayingForward()
        {
            bool isPlayingForward = lerpDirection == LerpDirection.FORWARD;
            return isPlayingForward;
        }

        private bool IsPlayingReverse()
        {
            bool isPlayingReverse = lerpDirection == LerpDirection.REVERSE;
            return isPlayingReverse;
        }

        private bool IsEndOfLoop()
        {
            bool isEndOfPlaythrough = (this.lerpProgress01 >= 1.0f || this.lerpProgress01 <= 0.0f);
            return isEndOfPlaythrough;
        }
    }


    public class LerpVector3 : LerpControl<Vector3>
    {
        public LerpVector3(Vector3 defaultValue) : base(defaultValue) { }

        protected override Vector3 PerformLerp(Vector3 start, Vector3 end, float progress01)
        {
            Vector3 result = Vector3.Lerp(start, end, progress01);
            return result;
        }
    }

    public class LerpQuaternion : LerpControl<Quaternion>
    {
        public LerpQuaternion(Quaternion defaultValue) : base(defaultValue) { }

        protected override Quaternion PerformLerp(Quaternion start, Quaternion end, float progress01)
        {
            Quaternion result = Quaternion.Lerp(start, end, progress01);
            return result;
        }
    }

    public class LerpColor : LerpControl<Color>
    {
        public LerpColor(Color defaultValue) : base(defaultValue) { }

        protected override Color PerformLerp(Color start, Color end, float progress01)
        {
            Color result = Color.Lerp(start, end, progress01);
            return result;
        }
    }

    public class LerpFloat : LerpControl<float>
    {
        public LerpFloat(float defaultValue) : base(defaultValue) { }

        protected override float PerformLerp(float start, float end, float progress01)
        {
            float result = Mathf.Lerp(start, end, progress01);
            return result;
        }
    }
}