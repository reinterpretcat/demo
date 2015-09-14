using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts;
using Assets.Scripts.MapEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scenes.GuiEditor
{
    /// <summary> Controller for editor main menu. </summary>
    public class EditorMenuController : MonoBehaviour
    {
        public GameObject MainMenuContainer;
        public GameObject AddMenuContainer;
        public GameObject EditMenuContainer;

        #region Main container

        public Button AddButton;
        public Button DeleteButton;
        public Button EditButton;

        #endregion

        #region Add container

        public Button BackFromAddButton;
        public Button BuildingButton;
        public Button RoadButton;
        public Button TreeButton;
        public Button BarrierButton;

        #endregion

        #region Edit container

        public Button BackFromEditButton;
        public Button TerrainUp;
        public Button TerrainDown;

        #endregion

        private IMessageBus _messageBus;

        void Start ()
        {
            _messageBus = ApplicationManager.Instance.GetService<IMessageBus>();
            ListenMainMenu();
            ListenAddMenu();
            ListenEditMenu();
        }

        private void ListenMainMenu()
        {
            AddButton.onClick.AsObservable().Subscribe(_ =>
            {
                MainMenuContainer.SetActive(false);
                AddMenuContainer.SetActive(true);
            });

            EditButton.onClick.AsObservable().Subscribe(_ =>
            {
                MainMenuContainer.SetActive(false);
                EditMenuContainer.SetActive(true);
            });
        }

        private void ListenAddMenu()
        {
            BackFromAddButton.onClick.AsObservable().Subscribe(_ => GoBackToMainMenu());

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

            TreeButton.onClick.AsObservable().Subscribe(_ =>
            {
                _messageBus.Send(EditorActionMode.AddTree);
                _messageBus.Send(TerrainInputMode.SetPoint);
            });
        }

        private void ListenEditMenu()
        {
            BackFromEditButton.onClick.AsObservable().Subscribe(_ => GoBackToMainMenu());

            TerrainUp.onClick.AsObservable().Subscribe(_ =>
            {
                _messageBus.Send(EditorActionMode.TerrainUp);
                _messageBus.Send(TerrainInputMode.SetPoint);
            });

            TerrainDown.onClick.AsObservable().Subscribe(_ =>
            {
                _messageBus.Send(EditorActionMode.TerrainDown);
                _messageBus.Send(TerrainInputMode.SetPoint);
            });
        }

        private void GoBackToMainMenu()
        {
            MainMenuContainer.SetActive(true);
            AddMenuContainer.SetActive(false);
            EditMenuContainer.SetActive(false);
            _messageBus.Send(EditorActionMode.None);
            _messageBus.Send(TerrainInputMode.None);
        }
    }
}
