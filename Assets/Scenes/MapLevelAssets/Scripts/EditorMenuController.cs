using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts;
using Assets.Scripts.MapEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scenes.MapLevelAssets.Scripts
{
    /// <summary> Controller for editor main menu. </summary>
    public class EditorMenuController : MonoBehaviour
    {
        public GameObject MainMenuContainer;
        public GameObject SubMenuContainer;

        #region Main container

        public Button AddButton;
        public Button DeleteButton;
        public Button EditButton;

        #endregion

        #region Sub container

        public Button BackButton;
        public Button BuildingButton;
        public Button RoadButton;
        public Button TreeButton;
        public Button BarrierButton;

        #endregion

        private IMessageBus _messageBus;

        void Start ()
        {
            AddButton.onClick.AsObservable().Subscribe(_ =>
            {
                MainMenuContainer.SetActive(false);
                SubMenuContainer.SetActive(true);
            });

            BackButton.onClick.AsObservable().Subscribe(_ =>
            {
                MainMenuContainer.SetActive(true);
                SubMenuContainer.SetActive(false);
                _messageBus.Send(EditorActionMode.None);
                _messageBus.Send(TerrainInputMode.None);
            });

            BuildingButton.onClick.AsObservable().Subscribe(_ =>
            {
                _messageBus.Send(EditorActionMode.AddBuilding);
                _messageBus.Send(TerrainInputMode.DrawLine);
            });

            BarrierButton.onClick.AsObservable().Subscribe(_ =>
            {
                _messageBus.Send(EditorActionMode.AddBarrier);
                _messageBus.Send(TerrainInputMode.DrawLine);
            });

            _messageBus = ApplicationManager.Instance.MessageBus;
        }
    }
}
