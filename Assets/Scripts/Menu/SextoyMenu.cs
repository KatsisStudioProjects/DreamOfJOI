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

        private ButtplugWebsocketConnector _connector;

        private bool _canConnect = true;
        private bool _canTest = true;

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

            _connector = new ButtplugWebsocketConnector(
                new Uri("ws://localhost:12345/buttplug"));

            Task.Run(LoadAsync);
        }

        public void Test()
        {
            if (!_canTest) return;

            _canTest = false;

            try
            {
                if (GlobalData.ButtplugClient != null && GlobalData.ButtplugClient.Connected && GlobalData.ButtplugClient.Devices.Any())
                {
                    StartCoroutine(TestCoroutine());
                }
            }
            finally
            {
                _canTest = true;
            }
        }

        private IEnumerator TestCoroutine()
        {
            foreach (var device in GlobalData.ButtplugClient.Devices)
            {
                device.VibrateAsync(1f);
            }

            yield return new WaitForSeconds(1f);

            foreach (var device in GlobalData.ButtplugClient.Devices)
            {
                device.VibrateAsync(0f);
            }

            _canTest = true;    
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
                    _infoText.text = GlobalData.ButtplugClient.Devices.Any() ? ("Devices found:\n\n" + string.Join("\n", GlobalData.ButtplugClient.Devices.Select(x => x.Name))) : "No device found";
                }

                yield return new WaitForSeconds(.1f);
            }
        }

        private async Task LoadAsync()
        {
            Debug.Log("[BUT] Connecting...");
            try
            {
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
