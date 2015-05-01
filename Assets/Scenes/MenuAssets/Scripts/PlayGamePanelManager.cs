using System;
using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Maps.GeoCoding;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayGamePanelManager: PanelManager
    {
        private const string LogCategory = "PlayGame";

        public InputField NameInputField;
        public InputField CoordinateInputField;
        public Button SearchButton;

        private ITrace _trace;
        private IGeocoder _geoCoder;

        private GeocoderResult[] _results;
        private int _currentIndex = 0;
        private bool _isSearchClick = true;

        private void Start()
        {
            _trace = ApplicationManager.Instance.GetService<ITrace>();
            _geoCoder = ApplicationManager.Instance.GetService<IGeocoder>();

            NameInputField.text = "Moscow, Red Square";
            CoordinateInputField.text = ApplicationManager.Instance.Coordinate.ToString();

            NameInputField.onEndEdit.AddListener((_) =>
            {
                SearchButton.GetComponentInChildren<Text>().text = "Search";
                _isSearchClick = true;
            });
        }

        public void OnSearch()
        {
            if (_isSearchClick)
            {
                _currentIndex = 0;
                _isSearchClick = false;
                _geoCoder.Search(NameInputField.text)
                    .ToArray()
                    .SubscribeOnMainThread()  // have to run on UI threads for web builds
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

        public void OnPlayClick()
        {
            var coordText = CoordinateInputField.text;
            _trace.Info(LogCategory, "Parsing geocoordinate: {0}", coordText);
            var coordParts = coordText.Split(',');
            ApplicationManager.Instance.Coordinate = 
                new GeoCoordinate(double.Parse(coordParts[0]), double.Parse(coordParts[1]));

            Application.LoadLevel("MapLevel");
        }
    }
}
