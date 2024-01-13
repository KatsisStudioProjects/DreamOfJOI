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

        private float _bpm = 750f;
        private float _baseDist = .25f;

        private const float _height = 2000f; // TODO: ewh

        private readonly List<RectTransform> _notes = new();

        private float _hitYPos;

        private float _timerDisplayText;

        private void Awake()
        {
            _hitAreaImage = _hitArea.GetComponent<Image>();
            _hitYPos = _hitArea.anchoredPosition.y;

            SpawnNotes();
        }

        private void Update()
        {
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
                n.transform.Translate(Vector2.down * _bpm * Time.deltaTime);
            }

            if (_notes[0].anchoredPosition.y - _hitYPos < -_info.HitInfo[0].Distance)
            {
                HitNote(_info.MissInfo);
            }

            SpawnNotes();
        }

        private void SpawnNotes()
        {
            var lastYPos = _notes.Any() ? _notes.Last().anchoredPosition.y : _hitYPos;

            for (float i = lastYPos + (_bpm * _baseDist); i < _height; i += _bpm * _baseDist)
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
            if (value.performed)
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
