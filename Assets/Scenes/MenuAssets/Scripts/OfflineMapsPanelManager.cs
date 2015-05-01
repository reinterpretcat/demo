using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class OfflineMapsPanelManager: PanelManager
    {
        private MapIndexUtility _mapIndexUtility;
        void Start()
        {
            _mapIndexUtility = ApplicationManager.Instance.GetService<MapIndexUtility>();
            InitializeImportPaths();
        }

        #region Map import logic

        public InputField SourcePathField;
        public InputField DestinationPathField;
        public Button ImportButton;

        private void InitializeImportPaths()
        {
            SourcePathField.text = @"Maps/import/test.osm.pbf";
            DestinationPathField.text = @"Maps/osm/your_city";
        }

        public void OnImportClick()
        {
            var sourcePath = SourcePathField.text;
            var destinationPath = DestinationPathField.text;

            Scheduler.ThreadPool.Schedule(() =>
            {
                _mapIndexUtility.BuildIndex(sourcePath, destinationPath);
            });
        }

        #endregion

        #region Map list logic

        public Text InstalledMapText;

        public void OnUpdateList()
        {
            var entries = new List<MapIndexUtility.IndexEntry>();
            _mapIndexUtility
                .GetIndexEntries()
                .Subscribe(entries.Add, () =>
                {
                    Debug.Log("entries:" + entries.Count);
                    var sb = new StringBuilder();
                    foreach (var indexEntry in entries)
                        sb.AppendFormat("{0}:{1}\n", indexEntry.Path, indexEntry.DisplayName);
                    InstalledMapText.text = sb.ToString();
                });
        }

        #endregion
    }
}
