using UnityEngine;
using Cindy.Logic;

namespace CindyRun.Game
{
    public class ObjectCast : LogicTrigger
    {
        public int layer;

        bool flag = false;

        protected override bool Handle()
        {
            return flag;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (layer.Equals(hit.gameObject.layer))
            {
                flag = true;
                Execute();
                flag = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer.Equals(layer))
            {
                flag = true;
                Execute();
                flag = false;
            }
        }
    }
}