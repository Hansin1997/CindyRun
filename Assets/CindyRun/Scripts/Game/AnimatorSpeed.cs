using UnityEngine;
using Cindy.Logic.VariableObjects;

namespace CindyRun
{
    public class AnimatorSpeed : FloatObject
    {
        public Animator animator;

        protected override void Start()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
            base.Start();
        }

        public override float GetValue()
        {
            value = animator.speed;
            return base.GetValue();
        }

        public override void SetValue(float value)
        {
            animator.speed = value;
            base.SetValue(value);
        }

        protected override void OnValueChanged(bool save = true, bool notify = true)
        {
            base.OnValueChanged(save, notify);
            animator.speed = value;
        }
    }
}
