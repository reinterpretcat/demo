// NOTE: used solution found in www
// Frankly speaking, I don't like code style below,
// but don't want to put extra-efforts to rewrite this so far
// 
// TODO: refactor this implementation in future

#define DEBUG_CONSOLE
#define DEBUG_LEVEL_LOG
#define DEBUG_LEVEL_WARN
#define DEBUG_LEVEL_ERROR

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define DEBUG
#endif

#if (UNITY_IOS || UNITY_ANDROID)
#define MOBILE
#endif

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.Scripts.Console.Commands;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Console.Watchers;
using UnityEngine;

namespace Assets.Scripts.Console
{
#if DEBUG_CONSOLE

    public class DebugConsole : MonoBehaviour
    {
        private const string Version = "0.1";
        private const string EntryField = "DebugConsoleEntryField";

        public VarWatcher Watcher { get; private set; }
        public CommandManager CommandManager { get; private set; }

        /// <summary>
        /// How many lines of text this console will display.
        /// </summary>
        public int maxLinesForDisplay = 500;

        /// <summary>
        /// Used to check (or toggle) the open state of the console.
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Key to press to toggle the visibility of the console.
        /// </summary>
        public static KeyCode toggleKey = KeyCode.BackQuote;

        private string _inputString = string.Empty;
        private Rect _windowRect;
#if MOBILE
  Rect _fakeWindowRect;
  Rect _fakeDragRect;
  bool dragging = false;
  GUIStyle windowOnStyle;
  GUIStyle windowStyle;
#if UNITY_EDITOR
  Vector2 prevMousePos;
#endif
#endif

        private Vector2 _logScrollPos = Vector2.zero;
        private Vector2 _rawLogScrollPos = Vector2.zero;
        private Vector2 _watchVarsScrollPos = Vector2.zero;
        private Vector3 _guiScale = Vector3.one;
        private Matrix4x4 restoreMatrix = Matrix4x4.identity;
        private bool _scaled = false;
        private StringBuilder _displayString = new StringBuilder();
        private FPSCounter fps;
        private bool dirty;

        #region GUI position values

        // Make these values public if you want to adjust layout of console window
        private readonly Rect scrollRect = new Rect(10, 20, 280, 362);
        private readonly Rect inputRect = new Rect(10, 388, 228, 24);
        private readonly Rect enterRect = new Rect(240, 388, 50, 24);
        private readonly Rect toolbarRect = new Rect(16, 416, 266, 25);
        private Rect messageLine = new Rect(4, 0, 264, 20);
        private int lineOffset = -4;
        private string[] tabs = new string[] {"Log", "Copy Log", "Watch Vars"};

        // Keep these private, their values are generated automatically
        private Rect nameRect;
        private Rect valueRect;
        private Rect innerRect = new Rect(0, 0, 0, 0);
        private int innerHeight = 0;
        private int toolbarIndex = 0;
        private GUIContent guiContent = new GUIContent();
        private GUI.WindowFunction[] windowMethods;
        private GUIStyle labelStyle;

        #endregion

        private List<ConsoleMessage> _messages = new List<ConsoleMessage>();
        private History _history = new History();
        private Regex _regex = null;

        public DebugConsole()
        {
            Watcher = new VarWatcher();
            CommandManager = new CommandManager();
        }

        private void OnEnable()
        {
            var scale = Screen.dpi/160.0f;

            if (scale != 0.0f && scale >= 1.1f)
            {
                _scaled = true;
                _guiScale.Set(scale, scale, scale);
            }

            windowMethods = new GUI.WindowFunction[] {LogWindow, CopyLogWindow, WatchVarWindow};

            fps = new FPSCounter();
            StartCoroutine(fps.Update());

            nameRect = messageLine;
            valueRect = messageLine;

#if MOBILE
    this.useGUILayout = false;
    _windowRect = new Rect(5.0f, 5.0f, 300.0f, 450.0f);
    _fakeWindowRect = new Rect(0.0f, 0.0f, _windowRect.width, _windowRect.height);
    _fakeDragRect = new Rect(0.0f, 0.0f, _windowRect.width - 32, 24);
#else
            _windowRect = new Rect(30.0f, 30.0f, 300.0f, 450.0f);
#endif

            LogMessage(ConsoleMessage.System(string.Format(" Mercraft Engine, version {0}", Version)));
            LogMessage(ConsoleMessage.System(" type '/?' for available commands."));
            LogMessage(ConsoleMessage.System(""));

            CommandManager.RegisterDefaults();
            RegisterTerminalCommands();
        }

        private void RegisterTerminalCommands()
        {
            CommandManager.Register("close", new Command("closes console", _ =>
            {
                IsOpen = false;
                return "opened";
            }));
            CommandManager.Register("clear", new Command("clears console", _ =>
            {
                ClearLog();
                return "clear";
            }));
            CommandManager.Register("filter", new Command("filter console items", args =>
            {
                const string enabledStr = "-e:";
                const string disabledStr = "-d";

                if (args.Length == 2)
                {
                    var value = args[1].Trim();
                    if (value.StartsWith(enabledStr))
                    {
                        var regexStr = value.Substring(enabledStr.Length);
                        _regex = new Regex(regexStr, RegexOptions.IgnoreCase);
                        return "filter enabled";
                    }
                    if (value.StartsWith(disabledStr))
                    {
                        _regex = null;
                        return "filter disabled";
                    }
                }
                LogMessage(ConsoleMessage.Output("Wrong syntax: \n\tfilter -e:<regex> \n\tfilter -d"));
                return "";
            }));

            CommandManager.Register("grep", new GrepCommand(_messages));
        }

        [Conditional("DEBUG_CONSOLE"),
         Conditional("UNITY_EDITOR"),
         Conditional("DEVELOPMENT_BUILD")]
        private void OnGUI()
        {
            var evt = Event.current;

            if (_scaled)
            {
                restoreMatrix = GUI.matrix;

                GUI.matrix = GUI.matrix*Matrix4x4.Scale(_guiScale);
            }

            while (_messages.Count > maxLinesForDisplay)
            {
                _messages.RemoveAt(0);
            }
#if (!MOBILE && DEBUG) || UNITY_EDITOR
            // Toggle key shows the console in non-iOS dev builds
            if (evt.keyCode == toggleKey && evt.type == EventType.KeyUp)
                IsOpen = !IsOpen;
#endif
#if MOBILE
    if (Input.touchCount == 1) {
      var touch = Input.GetTouch(0);
#if DEBUG
      // Triple Tap shows/hides the console in iOS/Android dev builds.
      if (evt.type == EventType.Repaint && (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) && touch.tapCount == 3) {
        IsOpen = !IsOpen;
      }
#endif
      if (IsOpen) {
        var pos = touch.position;
        pos.y = Screen.height - pos.y;

        if (dragging && (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)) {
          dragging = false;
        }
        else if (!dragging && (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)) {
          var dragRect = _fakeDragRect;

          dragRect.x = _windowRect.x * _guiScale.x;
          dragRect.y = _windowRect.y * _guiScale.y;
          dragRect.width *= _guiScale.x;
          dragRect.height *= _guiScale.y;

          // check to see if the touch is inside the dragRect.
          if (dragRect.Contains(pos)) {
            dragging = true;
          }
        }

        if (dragging && evt.type == EventType.Repaint) {
#if UNITY_ANDROID
          var delta = touch.deltaPosition * 2.0f;
#elif UNITY_IOS
          var delta = touch.deltaPosition;
          delta.x /= _guiScale.x;
          delta.y /= _guiScale.y;
#endif
          delta.y = -delta.y;

          _windowRect.center += delta;
        }
        else {
          var tapRect = scrollRect;
          tapRect.x += _windowRect.x * _guiScale.x;
          tapRect.y += _windowRect.y * _guiScale.y;
          tapRect.width -= 32;
          tapRect.width *= _guiScale.x;
          tapRect.height *= _guiScale.y;

          if (tapRect.Contains(pos)) {
            var scrollY = (tapRect.center.y - pos.y) / _guiScale.y;

            switch (toolbarIndex) {
            case 0:
              _logScrollPos.y -= scrollY;
              break;
            case 1:
              _rawLogScrollPos.y -= scrollY;
              break;
            case 2:
              _watchVarsScrollPos.y -= scrollY;
              break;
            }
          }
        }
      }
    }
    else if (dragging && Input.touchCount == 0) {
      dragging = false;
    }
#endif
            if (!IsOpen)
            {
                return;
            }

            labelStyle = GUI.skin.label;

            innerRect.width = messageLine.width;
#if !MOBILE
            _windowRect = GUI.Window(-1111, _windowRect, windowMethods[toolbarIndex],
                string.Format("Debug Console \tfps: {0:00.0}", fps.current));
            GUI.BringWindowToFront(-1111);
#else
    if (windowStyle == null) {
      windowStyle = new GUIStyle(GUI.skin.window);
      windowOnStyle = new GUIStyle(GUI.skin.window);
      windowOnStyle.normal.background = GUI.skin.window.onNormal.background;
    }

    GUI.BeginGroup(_windowRect);
#if UNITY_EDITOR
    if (GUI.RepeatButton(_fakeDragRect, string.Empty, GUIStyle.none)) {
      Vector2 delta = (Vector2) Input.mousePosition - prevMousePos;
      delta.y = -delta.y;

      _windowRect.center += delta;
      dragging = true;
    }

    if (evt.type == EventType.Repaint) {
      prevMousePos = Input.mousePosition;
    }
#endif
    GUI.Box(_fakeWindowRect, string.Format("Debug Console v{0}\tfps: {1:00.0}", Version, fps.current), dragging ? windowOnStyle : windowStyle);
    windowMethods[toolbarIndex](0);
    GUI.EndGroup();
#endif

            if (GUI.GetNameOfFocusedControl() == EntryField)
            {
                if (evt.isKey && evt.type == EventType.KeyUp)
                {
                    if (evt.keyCode == KeyCode.Return)
                    {
                        EvalInputString(_inputString);
                        _inputString = string.Empty;
                    }
                    else if (evt.keyCode == KeyCode.UpArrow)
                    {
                        _inputString = _history.Fetch(_inputString, true);
                    }
                    else if (evt.keyCode == KeyCode.DownArrow)
                    {
                        _inputString = _history.Fetch(_inputString, false);
                    }
                }
            }

            if (_scaled)
            {
                GUI.matrix = restoreMatrix;
            }

            if (dirty && evt.type == EventType.Repaint)
            {
                _logScrollPos.y = 50000.0f;
                _rawLogScrollPos.y = 50000.0f;

                BuildDisplayString();
                dirty = false;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #region GUI Window Methods

        private void DrawBottomControls()
        {
            GUI.SetNextControlName(EntryField);
            _inputString = GUI.TextField(inputRect, _inputString);

            if (GUI.Button(enterRect, "Enter"))
            {
                EvalInputString(_inputString);
                _inputString = string.Empty;
            }

            var index = GUI.Toolbar(toolbarRect, toolbarIndex, tabs);

            if (index != toolbarIndex)
            {
                toolbarIndex = index;
            }
#if !MOBILE
            GUI.DragWindow();
#endif
        }

        private void LogWindow(int windowID)
        {
            GUI.Box(scrollRect, string.Empty);

            innerRect.height = innerHeight < scrollRect.height ? scrollRect.height : innerHeight;

            _logScrollPos = GUI.BeginScrollView(scrollRect, _logScrollPos, innerRect, false, true);

            if (_messages != null || _messages.Count > 0)
            {
                Color oldColor = GUI.contentColor;

                messageLine.y = 0;

                foreach (ConsoleMessage m in _messages)
                {
                    foreach (var line in m.Text.Split('\n'))
                    {
                        GUI.contentColor = m.Color;
                        guiContent.text = line;
                        messageLine.height = labelStyle.CalcHeight(guiContent, messageLine.width);

                        GUI.Label(messageLine, guiContent);

                        messageLine.y += (messageLine.height + lineOffset);

                        innerHeight = messageLine.y > scrollRect.height ? (int)messageLine.y : (int)scrollRect.height;
                    }
                
                }
                GUI.contentColor = oldColor;
            }

            GUI.EndScrollView();

            DrawBottomControls();
        }

        private string GetDisplayString()
        {
            if (_messages == null)
            {
                return string.Empty;
            }

            return _displayString.ToString();
        }

        private void BuildDisplayString()
        {
            _displayString.Length = 0;

            foreach (ConsoleMessage m in _messages)
            {
                _displayString.AppendLine(m.Text);
            }
        }

        private void CopyLogWindow(int windowID)
        {

            guiContent.text = GetDisplayString();

            var calcHeight = GUI.skin.textArea.CalcHeight(guiContent, messageLine.width);

            innerRect.height = calcHeight < scrollRect.height ? scrollRect.height : calcHeight;

            _rawLogScrollPos = GUI.BeginScrollView(scrollRect, _rawLogScrollPos, innerRect, false, true);

            GUI.TextArea(innerRect, guiContent.text);

            GUI.EndScrollView();

            DrawBottomControls();
        }

        private void WatchVarWindow(int windowID)
        {
            GUI.Box(scrollRect, string.Empty);

            innerRect.height = innerHeight < scrollRect.height ? scrollRect.height : innerHeight;

            _watchVarsScrollPos = GUI.BeginScrollView(scrollRect, _watchVarsScrollPos, innerRect, false, true);

            int line = 0;

            nameRect.y = valueRect.y = 0;

            nameRect.x = messageLine.x;

            float totalWidth = messageLine.width - messageLine.x;
            float nameMin;
            float nameMax;
            float valMin;
            float valMax;
            float stepHeight;

            var textAreaStyle = GUI.skin.textArea;

            foreach (var kvp in Watcher.GetVariables)
            {
                var nameContent = new GUIContent(string.Format("{0}:", kvp.Name));
                var valContent = new GUIContent(kvp.ToString());

                labelStyle.CalcMinMaxWidth(nameContent, out nameMin, out nameMax);
                textAreaStyle.CalcMinMaxWidth(valContent, out valMin, out valMax);

                if (nameMax > totalWidth)
                {
                    nameRect.width = totalWidth - valMin;
                    valueRect.width = valMin;
                }
                else if (valMax + nameMax > totalWidth)
                {
                    valueRect.width = totalWidth - nameMin;
                    nameRect.width = nameMin;
                }
                else
                {
                    valueRect.width = valMax;
                    nameRect.width = nameMax;
                }

                nameRect.height = labelStyle.CalcHeight(nameContent, nameRect.width);
                valueRect.height = textAreaStyle.CalcHeight(valContent, valueRect.width);

                valueRect.x = totalWidth - valueRect.width + nameRect.x;

                GUI.Label(nameRect, nameContent);
                GUI.TextArea(valueRect, valContent.text);

                stepHeight = Mathf.Max(nameRect.height, valueRect.height) + 4;

                nameRect.y += stepHeight;
                valueRect.y += stepHeight;

                innerHeight = valueRect.y > scrollRect.height ? (int) valueRect.y : (int) scrollRect.height;

                line++;
            }

            GUI.EndScrollView();

            DrawBottomControls();
        }

        #endregion

        #region InternalFunctionality

        public void LogMessage(ConsoleMessage msg)
        {
            if (_regex != null && !_regex.IsMatch(msg.Text))
            {
                return;
            }

            _messages.Add(msg);
            dirty = true;
        }

        //--- Local version. Use the static version above instead.
        private void ClearLog()
        {
            _messages.Clear();
        }

        private void EvalInputString(string inputString)
        {
            inputString = inputString.Trim();

            if (string.IsNullOrEmpty(inputString))
            {
                LogMessage(ConsoleMessage.Input(string.Empty));
                return;
            }

            _history.Add(inputString);
            LogMessage(ConsoleMessage.Input(inputString));

            var input =
                new List<string>(inputString.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries));

            input = input.Select(low => low.ToLower()).ToList();
            var cmd = input[0];

            if (CommandManager.Contains(cmd))
            {
                LogMessage(ConsoleMessage.Output(CommandManager[cmd].Execute(input.ToArray())));
            }
            else
            {
                LogMessage(ConsoleMessage.Output(string.Format("*** Unknown Command: {0} ***", cmd)));
            }
        }

        #endregion
    }

#else
public static class DebugConsole {
  public static bool IsOpen;
  public static KeyCode toggleKey;
  public delegate object DebugCommand(params string[] args);

  public static object Log(object message) {
    return message;
  }

  public static object LogFormat(string format, params object[] args) {
    return string.Format(format, args);
  }

  public static object LogWarning(object message) {
    return message;
  }

  public static object LogError(object message) {
    return message;
  }

  public static object Log(object message, object messageType) {
    return message;
  }

  public static object Log(object message, Color displayColor) {
    return message;
  }

  public static object Log(object message, object messageType, Color displayColor) {
    return message;
  }

  public static void Clear() {
  }

  public static void RegisterCommand(string commandString, DebugCommand commandCallback) {
  }

  public static void UnRegisterCommand(string commandString) {
  }

  public static void RegisterWatchVar(object watchVar) {
  }

  public static void UnRegisterWatchVar(string name) {
  }
}
#endif
}