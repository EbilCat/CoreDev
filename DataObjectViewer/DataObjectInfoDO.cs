using System;
using System.Collections.Generic;
using System.Reflection;
using CoreDev.Framework;
using CoreDev.Observable;

namespace CoreDev.DataObjectInspector
{
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
                    ObservableVarInfoDO observableVarInfoDO = new ObservableVarInfoDO(fieldInfo, orderIndex);
                    if (fieldInfo.GetCustomAttribute<Bookmark>(true) != null) { observableVarInfoDO.isBookedMarked.Value = true; }
                    observableVarInfos.Add(observableVarInfoDO);
                    orderIndex++;
                }
            }
        }


//*====================
//* IDataObject
//*====================
        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }
    }
}