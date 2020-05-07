using Cindy.Logic.VariableObjects;

namespace CindyRun.Game
{
    public class MaxFloat : FloatObject
    {
        public FloatObject target;
        protected override void Start()
        {
            target.valueChangedEvent.AddListener(() =>
            {
                if (target.Value > Value)
                    Value = target.Value;
            });
            base.Start();
        }

        public override void Save()
        {
            //base.Save();
        }

        private void OnApplicationQuit()
        {
            base.Save();
        }
    }

}