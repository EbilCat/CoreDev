using CoreDev.Framework;
using CoreDev.Observable;


namespace CoreDev.DataObjectInspector
{
    public class InspectedObservableVarDO : IDataObject
    {
        private IObservableVar observableVarInstance;
        public IObservableVar ObservableVarInstance { get { return observableVarInstance; } }

        private ObservableVarInfoDO observableVarInfoDO;
        public ObservableVarInfoDO ObservableVarInfoDO { get { return observableVarInfoDO; } }

        public OBool isInspected;

        public OString varName;
        public OBool printToConsole;

        public InspectedObservableVarDO(IObservableVar observableVarInstance, ObservableVarInfoDO fieldInfoDO, OBool isInspected)
        {
            this.observableVarInstance = observableVarInstance;
            this.observableVarInfoDO = fieldInfoDO;
            this.isInspected = isInspected;
            this.printToConsole = new OBool(false, this);

            this.varName = new OString(fieldInfoDO.Name, this);
        }


        //*====================
        //* IDataObject
        //*====================
        public void Dispose() { }
    }
}