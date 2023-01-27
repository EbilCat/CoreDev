using CoreDev.Framework;
using CoreDev.Observable;


namespace CoreDev.Examples
{
    public class OAngryCube : ObservableVar<AngryCubeDO>
    {
        public OAngryCube() : base(default(AngryCubeDO)) { }
        public OAngryCube(AngryCubeDO startValue) : base(startValue, default(IDataObject)) { }
        public OAngryCube(IDataObject dataObject) : base(default(AngryCubeDO), dataObject) { }
        public OAngryCube(AngryCubeDO startValue, IDataObject dataObject) : base(startValue, dataObject) { }

        protected override bool AreEqual(AngryCubeDO var, AngryCubeDO value)
        {
            bool areEqual = (var == value);
            return areEqual;
        }

        public override string ToString()
        {
            AngryCubeDO angryCubeDO = this.Value;
            return angryCubeDO == null ? "<NULL>" : angryCubeDO.id.ToString();
        }

        public override void SetValueFromString(string strVal)
        {
            if (int.TryParse(strVal, out int id))
            {
                AngryCubeDO angryCubeDO = DataObjectMasterRepository.GetDataObject<AngryCubeDO>(x => x.id.Value == id);
                this.Value = angryCubeDO;
            }
            else
            {
                this.Value = null;
            }
        }
    }
}