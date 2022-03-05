using System.Collections.Generic;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;

public class DataObjectInfoDO : IDataObject
{
    public OList<ObservableVarInfoDO> observableVarInfos;

    public DataObjectInfoDO(List<FieldInfo> fieldInfos)
    {
        this.observableVarInfos = new OList<ObservableVarInfoDO>(this);

        int orderIndex = 0;
        for (int i = 0; i < fieldInfos.Count; i++)
        {
            FieldInfo fieldInfo = fieldInfos[i];
            if (typeof(IObservableVar).IsAssignableFrom(fieldInfo.FieldType))
            {
                observableVarInfos.Add(new ObservableVarInfoDO(fieldInfo, orderIndex));
                orderIndex++;
            }
        }
    }

        
//*====================
//* IDataObject
//*====================
        public void Dispose() { }
}