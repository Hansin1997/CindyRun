using UnityEngine;
using Cindy.Logic.ReferenceValues;

namespace CindyRun.Game
{
    public class Curver : MonoBehaviour
    {
        public ReferenceFloat input;
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));
        public ReferenceFloat output;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            output.Value = curve.Evaluate(input.Value);
        }
    }
}
