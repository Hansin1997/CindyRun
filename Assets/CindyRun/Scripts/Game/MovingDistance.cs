using UnityEngine;
using Cindy.Logic.VariableObjects;
using Cindy.Logic.ReferenceValues;

namespace CindyRun.Game
{
    public class MovingDistance : FloatObject
    {
        public Transform target;
        public ReferenceInt format;

        Vector3 lastPosition;

        private void Awake()
        {
            lastPosition = target.position;
        }

        private void LateUpdate()
        {
            value += (target.position - lastPosition).magnitude;
            format.Value = (int)value;
            lastPosition = target.position;
        }

        public override void SetValue(float value)
        {
            lastPosition = target.position;
            base.SetValue(value);
        }
    }
}