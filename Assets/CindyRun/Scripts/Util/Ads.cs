using Cindy.Logic;
using Cindy.Logic.ReferenceValues;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace CindyRun.Util
{
    public class Ads : LogicNode
    {

        public ReferenceString appStoreGameId;
        public ReferenceString googlePlayGameId;

        public ReferenceString placementId;

        public ReferenceBool test;

        public LogicNode success, fail,skip;
        
        protected void Start()
        {
            string gid = Application.platform == RuntimePlatform.Android ? googlePlayGameId.Value : appStoreGameId.Value;
            Advertisement.Initialize(gid, test.Value);
        }

        protected override void Run()
        {
            ShowOptions o = new ShowOptions();
            o.resultCallback = (s) =>
            {
                if (s == ShowResult.Finished)
                    success.Execute();
                if (s == ShowResult.Failed)
                    fail.Execute();
                else if(s == ShowResult.Skipped)
                    skip.Execute();

            };
            Advertisement.Show(placementId.Value, o);
        }
    }
}
