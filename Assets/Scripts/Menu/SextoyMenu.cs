using Buttplug.Client.Connectors.WebsocketConnector;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NsfwMiniJam.Menu
{
    public class SextoyMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _webGLContainer, _desktopContainer;

        [SerializeField]
        private TMP_Text _infoText;

        [SerializeField]
        private Button _enableBtn;

        private ButtplugWebsocketConnector _connector;

        private bool _canConnect = true;

        private void Awake()
        {
            if (false)//Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _webGLContainer.SetActive(true);
                _desktopContainer.SetActive(false);
            }
            else
            {
                _webGLContainer.SetActive(false);
                _desktopContainer.SetActive(true);
            }

            _infoText.text = "Not Connected";

            StartCoroutine(ConnectingCheck());
        }

        public void Enable()
        {
            if (!_canConnect) return;

            _canConnect = false;
            GlobalData.ButtplugClient = new("Nsfw Mini Jam Client");

            GlobalData.ButtplugClient.DeviceAdded += GlobalData.AddDevice;
            GlobalData.ButtplugClient.DeviceRemoved += GlobalData.RemoveDevice;

            _connector = new ButtplugWebsocketConnector(
                new Uri("ws://localhost:12345/buttplug"));

            Task.Run(LoadAsync);
        }

        public void Test()
        {
            if (GlobalData.ButtplugClient.Connected && GlobalData.ButtplugClient.Devices.Any())
            {

            }
        }

        private IEnumerator ConnectingCheck()
        {
            while (true)
            {
                if (_connector == null || !GlobalData.ButtplugClient.Connected)
                {
                    _infoText.text = "Not Connected";
                }
                else
                {
                    _infoText.text = GlobalData.Devices.Any() ? ("Devices found:\n\n" + string.Join("\n", GlobalData.Devices.Select(x => x.Name))) : "No device found";
                }

                yield return new WaitForSeconds(.1f);
            }
        }

        private async Task LoadAsync()
        {
            Debug.Log("[BUT] Connecting...");
            try
            {
                if (_connector.Connected) await _connector.DisconnectAsync();
                await GlobalData.ButtplugClient.ConnectAsync(_connector);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            Debug.Log($"[BUT] Connected state: {GlobalData.ButtplugClient.Connected}");

            _canConnect = true;
        }
    }
}
