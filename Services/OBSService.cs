using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.Services
{

    public enum OBS_STATE
    {
        DISCONNECTED,
        CONNECTING,
        CONNECTED,
        STREAMING,
        STOPPED,
    }

    public class OBSService
    {

        #region SINGLETON

        private static OBSService? instance = null;

        private OBSService()
        {


        }

        public static OBSService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OBSService();
                }
                return instance;
            }
        }

        #endregion


        private readonly OBSWebsocket _obs = new();

        public string Ip { get; set; } = "ws://127.0.0.1:4455";
        public string Password { get; set; } = "123456";
        public OutputState OutputState { get; set; }
        public int lastPosition = 1;

        OBS_STATE State { get; set; }

        public List<SceneBasicInfo> Scenes
        {
            get
            {
                if (_obs.IsConnected)
                    return _obs.ListScenes();

                else
                    return new List<SceneBasicInfo>();
            }
        }

        public List<SceneItemDetails> Sources
        {
            get
            {
                if (_obs.IsConnected)
                    return _obs.GetSceneItemList(CurrentScene);
                else
                    return new List<SceneItemDetails>();
            }
        }

        public string CurrentScene
        {
            get
            {
                if (_obs.IsConnected)
                    return _obs.GetCurrentProgramScene();

                return "";
            }
            set
            {
                if (_obs.IsConnected)
                {
                    _obs.SetCurrentProgramScene(value);
                }
            }
        }

        public bool IsConnected
        {
            get { return _obs.IsConnected; }
        }


        public void Start()
        {
            State = OBS_STATE.DISCONNECTED;
            _obs.StreamStateChanged += OnStreamStateChanged;
            _obs.Connected += OnConnected;
            _obs.Disconnected += OnDisconnected;
            ConnectObs();
        }

        public void Stop()
        {
            StopStreaming();
            State = OBS_STATE.STOPPED;
            _obs.Disconnect();
        }



        public void StartStreaming()
        {
            try
            {
                if (State == OBS_STATE.CONNECTED || State == OBS_STATE.STREAMING)
                {
                    if (OutputState == OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED || OutputState == OutputState.OBS_WEBSOCKET_OUTPUT_PAUSED || OutputState == OutputState.OBS_WEBSOCKET_OUTPUT_STARTING)
                    {
                        _obs.StartStream();
                        State = OBS_STATE.STREAMING;
                    }
                }
            }
            catch
            {

            }

        }

        public void StopStreaming()
        {
            if (State == OBS_STATE.STREAMING)
            {
                if (OutputState == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED)
                {
                    _obs.StopStream();
                    State = OBS_STATE.CONNECTED;
                }
            }
        }

        public void PauseStreaming()
        {
            if (State == OBS_STATE.STREAMING)
            {
                if (OutputState == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED)
                {
                    _obs.StopStream();
                    State = OBS_STATE.CONNECTED;
                }
            }
        }

        public Image GetSceneImage(string source)
        {
            string screenshot = _obs.GetSourceScreenshot(source, "jpg");
            byte[] bytes = Convert.FromBase64String(screenshot.Substring(screenshot.LastIndexOf(',') + 1));

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }

        public MemoryStream GetSceneStream(string source)
        {
            string screenshot = _obs.GetSourceScreenshot(source, "jpg");
            byte[] bytes = Convert.FromBase64String(screenshot.Substring(screenshot.LastIndexOf(',') + 1));

            return new MemoryStream(bytes);
        }

        public int GetSceneItemId(string item)
        {
            if (CurrentScene != string.Empty)
            {
                var items = _obs.GetSceneItemList(CurrentScene);
                if (items != null)
                {
                    SceneItemDetails? sceneItemDetails = items.Where(x => x.SourceName == item).FirstOrDefault();
                    if (sceneItemDetails != null)
                        return sceneItemDetails.ItemId;
                }

            }

            return -1;
        }

        public bool GetSceneItemStatus(string item)
        {
            try
            {
                int itemId = GetSceneItemId(item);
                if (itemId > 0)
                {
                    var items = _obs.GetSceneItemList(CurrentScene);
                    if (item.Contains("Camera"))
                        return _obs.GetSceneItemEnabled(CurrentScene, itemId);
                    else
                        return _obs.GetSceneItemIndex(CurrentScene, itemId) == (items.Count - lastPosition);
                }
            }
            catch
            {

            }

            return false;

        }



        public void DisableSource(string sourceName)
        {
            int itemId = GetSceneItemId(sourceName);
            if (itemId > 0)
                _obs.SetSceneItemEnabled(CurrentScene, itemId, false);
        }

        public void EnableSource(string sourceName)
        {
            int itemId = GetSceneItemId(sourceName);
            if (itemId > 0)
                _obs.SetSceneItemEnabled(CurrentScene, itemId, true);
        }

        public void StreamSource(string sourceName)
        {
            int itemId = GetSceneItemId(sourceName);
            if (itemId > 0)
            {
                var items = _obs.GetSceneItemList(CurrentScene);
                _obs.SetSceneItemIndex(CurrentScene, itemId, items.Count - 1);
            }

        }


        internal void GetInputSettings()
        {
            List<InputBasicInfo> list = new List<InputBasicInfo>();
            list = _obs.GetInputList();
            foreach (InputBasicInfo info in list)
            {
                InputSettings inputSettings = _obs.GetInputSettings(info.InputName);
                MessageBox.Show(inputSettings.Settings.Type.ToString());
                string json = @"{'input': 'http://10.1.1.31:82/9_20221121100516GHA2A78/9_20221121100516GHA2A78.mkv','input_format': 'mkv','is_local_file': false}";
                JObject newSettings = JObject.Parse(json);
                inputSettings.Settings = newSettings;
                _obs.SetInputSettings(inputSettings);
            }
        }



        public void ActivateSceneItem(string item)
        {
            int itemId = GetSceneItemId(item);
            int camera1ItemId = GetSceneItemId("Camera1");
            int camera2ItemId = GetSceneItemId("Camera2");
            if (itemId > 0 && camera1ItemId > 0 && camera2ItemId > 0)
            {
                var items = _obs.GetSceneItemList(CurrentScene);
                if (item == "Camera2")
                {
                    _obs.SetSceneItemEnabled(CurrentScene, camera1ItemId, false);
                    _obs.SetSceneItemIndex(CurrentScene, camera2ItemId, items.Count - lastPosition);
                    _obs.SetSceneItemEnabled(CurrentScene, itemId, true);
                    lastPosition = 2;
                }
                else if (item == "Camera1")
                {
                    _obs.SetSceneItemEnabled(CurrentScene, camera2ItemId, false);
                    _obs.SetSceneItemEnabled(CurrentScene, camera1ItemId, true);
                    lastPosition = 1;
                    _obs.SetSceneItemIndex(CurrentScene, itemId, items.Count - lastPosition);
                }
                else
                {
                    _obs.SetSceneItemIndex(CurrentScene, itemId, items.Count - lastPosition);
                }

            }
        }

        public void DesactivateSceneItem(string item)
        {
            int itemId = GetSceneItemId(item);
            int camera1ItemId = GetSceneItemId("Camera1");
            int camera2ItemId = GetSceneItemId("Camera2");
            if (itemId > 0 && camera1ItemId > 0 && camera2ItemId > 0)
            {
                var items = _obs.GetSceneItemList(CurrentScene);
                if (item == "Camera2")
                {
                    _obs.SetSceneItemEnabled(CurrentScene, camera1ItemId, true);
                    _obs.SetSceneItemEnabled(CurrentScene, itemId, false);
                    lastPosition = 1;
                }
                else if (item == "Camera1")
                {
                    _obs.SetSceneItemEnabled(CurrentScene, camera2ItemId, true);
                    _obs.SetSceneItemIndex(CurrentScene, camera1ItemId, items.Count - lastPosition);
                    _obs.SetSceneItemEnabled(CurrentScene, itemId, false);
                    lastPosition = 2;
                }
                else
                {
                    _obs.SetSceneItemIndex(CurrentScene, itemId, items.Count - lastPosition);
                }

            }
        }


        private void ConnectObs()
        {
            try
            {
                _obs.ConnectAsync(Ip, Password);
                if (State == OBS_STATE.DISCONNECTED)
                    State = OBS_STATE.CONNECTING;
            }
            catch
            {
                if (State == OBS_STATE.CONNECTING)
                    State = OBS_STATE.DISCONNECTED;
                Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    ConnectObs();
                });
            }
        }


        private void OnConnected(object? sender, EventArgs e)
        {
            if (State == OBS_STATE.CONNECTING)
                State = OBS_STATE.CONNECTED;

            StreamingService settings = _obs.GetStreamServiceSettings();
            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            settings.Settings.Server = $"rtmp://{serverConfig.Ip}:1935";
            settings.Settings.Key = $"stream_{AppSessionService.Instance.User.Id}";
            _obs.SetStreamServiceSettings(settings);
        }

        private void OnDisconnected(object? sender, ObsDisconnectionInfo e)
        {
            if (State != OBS_STATE.STOPPED)
            {
                if (State == OBS_STATE.CONNECTED)
                    State = OBS_STATE.DISCONNECTED;

                Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    ConnectObs();
                });
            }
        }


        private void OnStreamStateChanged(object? sender, StreamStateChangedEventArgs args)
        {
            if (State == OBS_STATE.STREAMING)
            {
                if (args.OutputState.State != OutputState.OBS_WEBSOCKET_OUTPUT_STARTING ||
                   args.OutputState.State != OutputState.OBS_WEBSOCKET_OUTPUT_STARTED ||
                    args.OutputState.State != OutputState.OBS_WEBSOCKET_OUTPUT_RESUMED)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(3000);
                        StartStreaming();
                    });
                }
            }

            OutputState = args.OutputState.State;
        }

    }
}
