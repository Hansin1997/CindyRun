using Cindy.Logic;
using UnityEngine.Advertisements;
using Cindy.Logic.ReferenceValues;

namespace CindyRun.Util
{
    public class AdsReady : Condition
    {
        public ReferenceString placementId;

        public override bool Check()
        {
            return Advertisement.IsReady(placementId.Value);
        }
    }
}
