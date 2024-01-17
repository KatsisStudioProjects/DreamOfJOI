using Ink.Runtime;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwMiniJam.VN
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { private set; get; }

        [Header("Components")]
        [SerializeField]
        private TextDisplay _display;

        private Story _story;

        [SerializeField]
        private GameObject _container;

        private bool _isSkipEnabled;
        private float _skipTimer;
        private float _skipTimerRef = .1f;

        private Action _onDone;

        private void Awake()
        {
            Instance = this;
        }

        public bool IsPlayingStory => _container.activeInHierarchy;

        private void Update()
        {
            if (_isSkipEnabled)
            {
                _skipTimer -= Time.deltaTime;
                if (_skipTimer < 0)
                {
                    _skipTimer = _skipTimerRef;
                    DisplayNextDialogue();
                }
            }
        }

        public void ShowStory(TextAsset asset, Action onDone)
        {
            Debug.Log($"[NOV] Playing {asset.name}");
            _onDone = onDone;
            _story = new(asset.text);
            _isSkipEnabled = false;
            DisplayStory(_story.Continue());
        }

        private void DisplayStory(string text)
        {
            _container.SetActive(true);

            _display.ToDisplay = text;
        }

        public void DisplayNextDialogue()
        {
            if (!_container.activeInHierarchy || _story == null)
            {
                return;
            }
            if (!_display.IsDisplayDone)
            {
                // We are slowly displaying a text, force the whole display
                _display.ForceDisplay();
            }
            else if (_story.canContinue && // There is text left to write
                !_story.currentChoices.Any()) // We are not currently in a choice
            {
                DisplayStory(_story.Continue());
            }
            else if (!_story.canContinue && !_story.currentChoices.Any())
            {
                _container.SetActive(false);
                _onDone?.Invoke();
            }
        }

        public void ForceStop()
        {
            _container.SetActive(false);
        }

        public void ToggleSkip(bool value)
            => _isSkipEnabled = value;

        public void OnToggleSkip(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started) ToggleSkip(true);
            else if (value.phase == InputActionPhase.Canceled) ToggleSkip(false);
        }
    }
}