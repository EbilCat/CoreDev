using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.Examples
{
    public class GameSettingsDO : MonoBehaviour, IDataObject
    {
        [SerializeField] private Color[] _teamColors;
        public OList<Color> teamColors;

        private void Init()
        {
            this.teamColors = new OList<Color>(this);
            this.teamColors.AddRange(_teamColors);

            DataObjectMasterRepository.RegisterDataObject(this);
        }


        //*====================
        //* SINGLETON
        //*====================
        private static GameSettingsDO singleton;
        public static GameSettingsDO Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = GameObject.FindObjectOfType<GameSettingsDO>();
                    singleton.Init();
                }
                return singleton;
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