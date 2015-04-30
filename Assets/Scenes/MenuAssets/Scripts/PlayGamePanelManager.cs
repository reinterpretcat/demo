using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayGamePanelManager: PanelManager
    {
        public InputField NameInputField;
        public InputField CoordinateInputField;
        public Button SearchButton;

        private string[] _results = new string[2]{"Berlin Hauptbahnhof", "Berlin Naturkundemuseum"};
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

        public void OnSearch()
        {
            if (_isSearchClick)
            {
                _currentIndex = 0;
                _isSearchClick = false;
                Debug.Log("Launch search for:" + NameInputField.text);
                // NOTE should be done from different thread
                ShowResult();
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

            NameInputField.text = _results[_currentIndex];
            CoordinateInputField.text = _results[_currentIndex];
        }
    }
}
