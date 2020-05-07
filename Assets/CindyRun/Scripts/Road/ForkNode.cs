using UnityEngine;

namespace CindyRun.Road
{
    public class ForkNode : AbstractPathNode
    {
        public Transform forward, left, right;
        public float crossOffset;

    }
}
