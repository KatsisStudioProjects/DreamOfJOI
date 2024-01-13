using NsfwMiniJam.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

        [SerializeField]
        private RectTransform _hitArea;
        private Image _hitAreaImage;

        [SerializeField]
        private TMP_Text _hitText;

        [SerializeField]
        private TMP_Text _comboText;

        [SerializeField]
        private AudioSource _bgm;

        private int _combo;

        // Speed data
        private float _bpm = 60f;
        private float _speedMultiplier = 10f;

        private const float _height = 2000f; // TODO: ewh

        // List of all notes
        private readonly List<RectTransform> _notes = new();

        // Position where we are hitting notes
        private float _hitYPos;

        // Time to display the "Great", "Good" etc text
        private float _timerDisplayText;

        private bool _isAlive = true;
        // When dead, slowly decrease the speed of all notes until we reach 0
        private float _deadSpeedTimer = 1f;

        private float _waitBeforeStart = 3f;

        private void Awake()
        {
            _hitAreaImage = _hitArea.GetComponent<Image>();
            _hitYPos = _hitArea.anchoredPosition.y;

            SpawnNotes();
            StartCoroutine(WaitAndStartBpm());
        }

        private IEnumerator WaitAndStartBpm()
        {
            yield return new WaitForSeconds(_waitBeforeStart);
            _bgm.Play();
        }

        private void Update()
        {
            if (!_isAlive && _deadSpeedTimer > 0f)
            {
                _deadSpeedTimer -= Time.deltaTime;
                if (_deadSpeedTimer < 0f)
                {
                    _deadSpeedTimer = 0f;
                }
            }

            if (_timerDisplayText > 0f)
            {
                _timerDisplayText -= Time.deltaTime;
                if (_timerDisplayText <= 0f)
                {
                    _hitText.gameObject.SetActive(false);
                }
            }

            foreach (var n in _notes)
            {
                n.transform.Translate(Vector2.down * _bpm * Time.deltaTime * _deadSpeedTimer * _speedMultiplier);
            }

            if (_notes[0].anchoredPosition.y - _hitYPos < -_info.HitInfo[0].Distance)
            {
                HitNote(_info.MissInfo);
            }

            SpawnNotes();
        }

        private void SpawnNotes()
        {
            var lastYPos = _notes.Any() ? (_notes.Last().anchoredPosition.y + (_bpm * _speedMultiplier)) : (_hitYPos + (_waitBeforeStart * _bpm * _speedMultiplier));

            for (float i = lastYPos; i < _height; i += _bpm * _speedMultiplier)
            {
                var n = Instantiate(_note, _noteContainer);

                var rTransform = (RectTransform)n.transform;
                rTransform.anchorMin = new(.5f, 0f);
                rTransform.anchorMax = new(.5f, 0f);
                rTransform.anchoredPosition = Vector2.up * i;

                _notes.Add(rTransform);
            }
        }

        private void HitNote(HitInfo data)
        {
            _hitText.gameObject.SetActive(true);
            _hitText.text = data.DisplayText;
            _hitText.color = data.Color;
            _timerDisplayText = 1f;

            if (data.DoesBreakCombo)
            {
                _combo = 0;
            }
            else
            {
                _combo++;
            }
            _comboText.gameObject.SetActive(_combo >= 5);
            _comboText.text = $"Combo x{_combo}";

            Destroy(_notes[0].gameObject);
            _notes.RemoveAt(0);
        }

        private IEnumerator HitEffect()
        {
            _hitAreaImage.color = Color.black;
            yield return new WaitForSeconds(.1f);
            _hitAreaImage.color = Color.white;
        }

        public void OnHit(InputAction.CallbackContext value)
        {
            if (value.performed && _isAlive)
            {
                for (int i = _info.HitInfo.Length - 1; i >= 0; i--)
                {
                    var info = _info.HitInfo[i];
                    if (Mathf.Abs(_hitYPos - _notes[0].anchoredPosition.y) < info.Distance)
                    {
                        HitNote(info);
                        break;
                    }
                }
                StartCoroutine(HitEffect());
            }
        }
    }
}
