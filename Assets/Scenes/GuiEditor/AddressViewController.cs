using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Explorer.Commands;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scenes.GuiEditor
{
    public class AddressViewController: MonoBehaviour
    {
        public Text AddressText;

        void Start()
        {
            ApplicationManager.Instance.GetService<IMessageBus>()
                .AsObservable<TileLoadFinishMessage>()
                .Take(1)
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var commandController = ApplicationManager.Instance
                        .GetService<CommandController>();
                    gameObject.AddComponent<AddressLocatorBehaviour>()
                        .SetCommandController(commandController)
                        .GetObservable()
                        .ObserveOnMainThread()
                        .Subscribe(OnAddressUpdate);
                });
        }

        private void OnAddressUpdate(Address address)
        {
            string addressString = address.Street;

            if (!String.IsNullOrEmpty(address.Name))
                addressString += String.Format(", {0}", address.Name);

            if (!String.IsNullOrEmpty(address.Code))
                addressString += String.Format(", {0}", address.Code);

            AddressText.text = addressString;
        }
    }
}
