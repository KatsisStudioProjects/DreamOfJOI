using NsfwMiniJam.Menu;
using NsfwMiniJam.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

        [SerializeField]
        private GameOverPanel _victory;

        [SerializeField]
        private RectTransform _baseContainer;

        private int _combo;

        // Speed data
        private float _bpm;
        private float _speedMultiplier = 10f;

        private const float _height = 2000f; // TODO: ewh

        // List of all notes
        private readonly List<NoteInfo> _notes = new();

        // Position where we are hitting notes
        private float _hitYPos;

        // Time to display the "Great", "Good" etc text
        private float _timerDisplayText;

        private bool _isAlive = true;
        // When dead, slowly decrease the speed of all notes until we reach 0
        private float _deadSpeedTimer = 1f;

        private float _waitBeforeStart = 3f;

        private float _basePitch = 1f;

        // Manage the end of the game
        private int _leftToSpawn, _leftToTape;
        private float _volumeTimer;

        private int _maxPossibleScore;
        private int _score;

        private void Awake()
        {
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            var music = _info.Music[0];

            _leftToSpawn = music.NoteCount;
            _leftToTape = music.NoteCount;
            _bpm = music.Bpm;
            _bgm.clip = music.Music;

            _maxPossibleScore = music.NoteCount * _info.HitInfo.Last().Score;

            if (_info.Reversed)
            {
                _baseContainer.localScale = new(1f, -1f, 1f);
            }

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
            if (_leftToTape == 0)
            {
                _volumeTimer -= Time.deltaTime;
                if (_volumeTimer < 0f)
                {
                    _victory.gameObject.SetActive(true);
                    _victory.Init(_score, _maxPossibleScore, _info);
                    _isAlive = false;
                }
            }

            if (!_isAlive && _deadSpeedTimer > 0f)
            {
                _bgm.pitch = Mathf.Lerp(_deadSpeedTimer, _basePitch, 0f);
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

            if (_notes.Any())
            {
                foreach (var n in _notes)
                {
                    n.RT.Translate(Vector2.down * _bpm * Time.deltaTime * _deadSpeedTimer * _speedMultiplier);
                    if (_info.Hidden != HiddenType.None)
                    {
                        var c = n.Image.color;
                        var value = _info.Hidden == HiddenType.Normal
                            ? (1f - ((n.RT.anchoredPosition.y - _hitYPos) / _info.HiddenDistance))
                            : ((n.RT.anchoredPosition.y - _hitYPos) / _info.HiddenDistance);
                        n.Image.color = new(c.r, c.g, c.b, Mathf.Clamp01(value));
                    }
                }

                if (_notes[0].RT.anchoredPosition.y - _hitYPos < -_info.HitInfo[0].Distance)
                {
                    HitNote(_info.MissInfo);
                }
            }

            SpawnNotes();
        }

        private void SpawnNotes()
        {
            if (_leftToSpawn == 0) return;

            var lastYPos = _notes.Any() ? (_notes.Last().RT.anchoredPosition.y + (_bpm * _speedMultiplier)) : (_hitYPos + (_waitBeforeStart * _bpm * _speedMultiplier));

            for (float i = lastYPos; i < _height; i += _bpm * _speedMultiplier)
            {
                var n = Instantiate(_note, _noteContainer);

                var rTransform = (RectTransform)n.transform;
                rTransform.anchorMin = new(.5f, 0f);
                rTransform.anchorMax = new(.5f, 0f);
                rTransform.anchoredPosition = Vector2.up * i;

                var image = n.GetComponent<Image>();

                bool isTrap = _info.Mines ? Random.Range(0, 100f) < _info.MineChancePercent : false;
                if (isTrap)
                {
                    image.color = Color.red;
                }

                _notes.Add(new() { RT = rTransform, Image = image, IsTrap = isTrap });

                _leftToSpawn--;
            }
        }

        private void HitNote(HitInfo data)
        {
            if (!_isAlive) return;

            if (_notes[0].IsTrap)
            {
                if (data.Score > 0)
                {
                    data = _info.WrongInfo;
                }
                else
                {
                    data = _info.HitInfo.Last();
                }
            }

            _hitText.gameObject.SetActive(true);
            _hitText.text = data.DisplayText;
            _hitText.color = data.Color;
            _timerDisplayText = 1f;

            if (_info.SuddenDeath == SuddenDeathType.PerfectOnly && data.Score != _info.HitInfo.Last().Score)
            {
                data = _info.MissInfo;
                _isAlive = false;
            }
            else if (_info.SuddenDeath == SuddenDeathType.Normal && data.DoesBreakCombo)
            {
                data = _info.MissInfo;
                _isAlive = false;
            }

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

            _score += data.Score;

            Destroy(_notes[0].RT.gameObject);
            _notes.RemoveAt(0);

            _leftToTape--;
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
                    if (Mathf.Abs(_hitYPos - _notes[0].RT.anchoredPosition.y) < info.Distance)
                    {
                        HitNote(info);
                        break;
                    }
                }
                StartCoroutine(HitEffect());
            }
        }

        public class NoteInfo
        {
            public RectTransform RT;
            public Image Image;
            public bool IsTrap;
        }
    }
}
