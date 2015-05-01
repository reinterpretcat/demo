using System;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Maps.Geocoding;
using ActionStreetMap.Maps.GeoCoding;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayGamePanelManager: PanelManager
    {
        public InputField NameInputField;
        public InputField CoordinateInputField;
        public Button SearchButton;

        private IGeocoder _geoCoder;

        private GeocoderResult[] _results;
        private int _currentIndex = 0;
        private bool _isSearchClick = true;

        private void Start()
        {
            NameInputField.onEndEdit.AddListener((_) =>
            {
                SearchButton.GetComponentInChildren<Text>().text = "Search";
                _isSearchClick = true;
            });
        }

        private IGeocoder GetGeocoder()
        {
            if (_geoCoder == null)
                _geoCoder = ApplicationManager.Instance.GetService<IGeocoder>();
            return _geoCoder;
        }

        public void OnSearch()
        {
            if (_isSearchClick)
            {
                _currentIndex = 0;
                _isSearchClick = false;
                GetGeocoder().Search(NameInputField.text)
                    .ToArray()
                    .SubscribeOnMainThread()  // have to run on UI threads on web builds
                    .ObserveOnMainThread()
                    .Subscribe(results =>
                    {
                        _results = results;
                        ShowResult();
                    });                
            }
            else
            {
                _currentIndex++;
                ShowResult();
            }
        }

        private void ShowResult()
        {
            if (_results == null || _results.Length == 0)
                return;

            if (_currentIndex == _results.Length)
                _currentIndex = 0;

            SearchButton.GetComponentInChildren<Text>().text =
                String.Format("Result {0} of {1}", _currentIndex + 1, _results.Length);

            NameInputField.text = _results[_currentIndex].DisplayName;
            CoordinateInputField.text = _results[_currentIndex].Coordinate.ToString();
        }
    }
}
