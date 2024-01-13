using NsfwMiniJam.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwMiniJam.Rhythm
{
    public class RhythmManager : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private RectTransform _noteContainer;

        [SerializeField]
        private GameObject _note;

        private const float _offsetBeforeStart = 3f;

        private int _spawnIndex;
        private float _startTime;

        private float _bpm = 300f;

        private const float _height = 2000f; // TODO: ewh

        private readonly List<RectTransform> _notes = new();

        private void Awake()
        {
            _startTime = Time.unscaledTime;
            SpawnNotes();
        }

        private void Update()
        {
            foreach (var n in _notes)
            {
                n.transform.Translate(Vector2.down * _bpm * Time.deltaTime);
            }

            if (_notes[0].anchoredPosition.y < -_info.BadDistance)
            {
                Destroy(_notes[0].gameObject);
                _notes.RemoveAt(0);
                // TODO: Display "miss"
            }

            SpawnNotes();
        }

        private void SpawnNotes()
        {
            var lastYPos = _notes.Any() ? _notes.Last().anchoredPosition.y : 0f;

            for (float i = lastYPos + _bpm; i < _height; i += _bpm)
            {
                var n = Instantiate(_note, _noteContainer);

                var rTransform = (RectTransform)n.transform;
                rTransform.anchorMin = new(.5f, 0f);
                rTransform.anchorMax = new(.5f, 0f);
                rTransform.anchoredPosition = Vector2.up * i;

                _notes.Add(rTransform);
            }
        }

        public void OnHit(InputAction.CallbackContext value)
        {
            if (value.performed)
            {

            }
        }
    }
}
