using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class OfflineMapsPanelManager: PanelManager
    {
        void Start()
        {
            InitializeInstalledMapData();
        }

        #region Map import logic

        public InputField PathField;
        public Button ImportButton;

        public void OnImportClick()
        {
            var path = PathField.text;
            Debug.Log("Import from:" + path);
        }

        #endregion

        #region Map list logic

        public Text InstalledMapText;

        private void InitializeInstalledMapData()
        {
            
        }

        #endregion
    }
}
