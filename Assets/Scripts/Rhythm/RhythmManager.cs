using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwMiniJam.Rhythm
{
    public class RhythmManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _noteContainer;

        [SerializeField]
        private GameObject _note;

        private const float _offsetBeforeStart = 3f;

        private int _spawnIndex;
        private float _startTime;

        private float _bpm = 60f;

        private const float _height = 2000f; // TODO: ewh

        private readonly List<GameObject> _notes = new();

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
        }

        private void SpawnNotes()
        {
            var elapsedTime = Time.unscaledTime - _startTime;

            for (float i = 0f; i < _height; i += _bpm)
            {
                var n = Instantiate(_note, _noteContainer);

                ((RectTransform)n.transform).anchoredPosition = Vector2.up * i;

                _notes.Add(n);
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
