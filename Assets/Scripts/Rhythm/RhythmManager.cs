using NsfwMiniJam.Menu;
using NsfwMiniJam.Persistency;
using NsfwMiniJam.SO;
using NsfwMiniJam.VN;
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

        [SerializeField]
        private TMP_Text _startCountdown;

        [SerializeField]
        private RectTransform _cumFill;

        [SerializeField]
        private Animator _anim;

        [SerializeField]
        private TMP_Text _cumText;

        [SerializeField]
        private TMP_Text _hypnotismCounter;

        private MusicInfo _music;

        private float _cumLevel;

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

        private int _hypnotismHits;

        private int _cumRequirementStoke;

        private int _noteSpawnIndex;
        private float _refTime;

        private void Awake()
        {
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            _music = _info.Music[GlobalData.LevelIndex];

            _anim.runtimeAnimatorController = _music.Controller;

            _leftToSpawn = _music.NoteCount;
            _leftToTape = _music.NoteCount;
            _bpm = _music.Bpm;
            _bgm.clip = _music.Music;

            _maxPossibleScore = _music.NoteCount * _info.HitInfo.Last().Score;

            if (GlobalData.Reversed)
            {
                _baseContainer.localScale = new(1f, -1f, 1f);
            }

            _hitAreaImage = _hitArea.GetComponent<Image>();
            _hitYPos = _hitArea.anchoredPosition.y;

            SpawnNotes();
        }

        private void Start()
        {
            VNManager.Instance.ShowStory(_music.Intro, () =>
            {
                _anim.SetTrigger("Start");
                _startCountdown.gameObject.SetActive(true);
                _refTime = Time.unscaledTime;
                StartCoroutine(WaitAndStartBpm());
            });
        }

        private IEnumerator WaitAndStartBpm()
        {
            yield return new WaitForSeconds(_waitBeforeStart);
            _bgm.Play();
        }

        private void Update()
        {
            if (VNManager.Instance.IsPlayingStory) return;

            if (_waitBeforeStart > 0f)
            {
                _waitBeforeStart -= Time.deltaTime;
                _startCountdown.text = Mathf.CeilToInt(_waitBeforeStart).ToString();
                if (_waitBeforeStart <= 0f)
                {
                    _startCountdown.gameObject.SetActive(false);
                }
            }

            if (_volumeTimer > 0f && _leftToTape == 0)
            {
                _volumeTimer -= Time.deltaTime;
                _bgm.volume = _volumeTimer;
                if (_volumeTimer <= 0f)
                {
                    _cumRequirementStoke = _info.CumStrokeCountRequirement;
                    _cumText.gameObject.SetActive(true);
                    _isAlive = false;
                }
            }

            if (!_isAlive && _deadSpeedTimer > 0f && _cumRequirementStoke == 0)
            {
                _bgm.pitch = Mathf.Lerp(_deadSpeedTimer, _basePitch, 0f);
                _deadSpeedTimer -= Time.deltaTime;
                if (_deadSpeedTimer < 0f)
                {
                    _deadSpeedTimer = 0f;
                    ShowGameOver();
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
                    if (GlobalData.Hidden != HiddenType.None)
                    {
                        var c = n.Image.color;
                        var value = GlobalData.Hidden == HiddenType.Normal
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

        private void ShowGameOver()
        {
            _victory.gameObject.SetActive(true);
            _victory.Init(_score, _maxPossibleScore, _info);

            PersistencyManager.Instance.SaveData.AddScore(GlobalData.LevelIndex, new() { Score = _score / (float)_maxPossibleScore, Multiplier = GlobalData.CalculateMultiplier() });
            PersistencyManager.Instance.Save();
        }

        private bool SpawnSingleNote(int stepCost)
        {
            var step = _speedMultiplier * _bpm * (60f / _bpm);
            var spentTime = Time.unscaledTime - _refTime;
            var y = (_waitBeforeStart * step * stepCost) + (_noteSpawnIndex * step * stepCost) - spentTime;


            if (y > 2000f && _leftToSpawn == 0) return false;

            var n = Instantiate(_note, _noteContainer);

            var rTransform = (RectTransform)n.transform;
            rTransform.anchorMin = new(.5f, 0f);
            rTransform.anchorMax = new(.5f, 0f);
            rTransform.anchoredPosition = Vector2.up * y;

            var image = n.GetComponent<Image>();

            // If we are not the last note, if hypnotism is enabled, we are not already hypnotised and chance check pass
            bool isHypnotic = _leftToSpawn > 1 && (GlobalData.Hypnotised || _music.HypnotisedOverrides) && _hypnotismHits == 0 && Random.Range(0f, 100f) < _info.HypnotismChance;

            // If not hypnotised (effects don't stack), mines are enabled and chance check pass
            bool isTrap = !isHypnotic && GlobalData.Mines && Random.Range(0, 100f) < _info.MineChancePercent;
            if (isHypnotic)
            {
                image.color = new(.5f, 0f, .5f);
            }
            else if (isTrap)
            {
                image.color = Color.red;
            }

            _notes.Add(new() { RT = rTransform, Image = image, IsTrap = isTrap, IsHypnotic = isHypnotic });

            _leftToSpawn--;
            _noteSpawnIndex += stepCost;

            if (isHypnotic)
            {
                return SpawnSingleNote(_info.HypnotismNextNoteDelay);
            }

            return true;
        }

        private void SpawnNotes()
        {
            while (SpawnSingleNote(1))
            { }
        }

        private void HitNote(HitInfo data)
        {
            if (!_isAlive) return;

            var note = _notes[0];

            // Update hit if note is a trap
            if (note.IsTrap)
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

            // Update combo
            if (GlobalData.SuddenDeath == SuddenDeathType.PerfectOnly && data.Score != _info.HitInfo.Last().Score)
            {
                data = _info.MissInfo;
                _isAlive = false;
                _combo = 0;
                _cumLevel = 1f;
            }
            else if (GlobalData.SuddenDeath == SuddenDeathType.Normal && data.DoesBreakCombo)
            {
                data = _info.MissInfo;
                _isAlive = false;
                _combo = 0;
                _cumLevel = 1f;
            }
            else
            {
                if (data.DoesBreakCombo)
                {
                    _anim.SetTrigger("FailCombo");

                    _combo = 0;
                    _cumLevel += _info.IncreaseOnMiss;
                }
                else
                {
                    _combo++;

                    if (_combo == 5)
                    {
                        _anim.SetTrigger("SuccessCombo");
                    }

                    _cumLevel -= _info.DecreaseOnHit;
                }
            }

            // Update hit display
            _hitText.gameObject.SetActive(true);
            _hitText.text = data.DisplayText;
            _hitText.color = data.Color;
            _timerDisplayText = 1f;

            // Update cum level
            _cumLevel = Mathf.Clamp01(_cumLevel);
            UpdateCumBar();

            // We die if cum reach max level
            if (_cumLevel == 1f)
            {
                _isAlive = false;
            }

            // Prevent failure
            if (GlobalData.NoFail || _music.NoFailOverrides)
            {
                _isAlive = true;
            }

            // Check victory condition
            if (!_isAlive)
            {
                _anim.SetTrigger("Defeat");
            }
            else
            {
                _leftToTape--;
                if (_leftToTape == 0)
                {
                    _volumeTimer = 1f;
                    _anim.SetTrigger("Victory");
                }
            }

            // Update combo text
            _comboText.gameObject.SetActive(_combo >= 5);
            _comboText.text = $"Combo x{_combo}";

            // Update score
            _score += data.Score;

            // Update bar color for hypnotism mode
            if (note.IsHypnotic)
            {
                _hitAreaImage.color = new(.5f, 0f, .5f);
                _hypnotismHits = _info.HypnotismHitCount;
                _hypnotismCounter.gameObject.SetActive(true);
                _hypnotismCounter.text = _hypnotismHits.ToString();
            }

            // Destroy note
            Destroy(note.RT.gameObject);
            _notes.RemoveAt(0);

        }

        private void UpdateCumBar()
        {
            _cumFill.localScale = new(1f, _cumLevel, 1f);
        }

        private IEnumerator HitEffect()
        {
            var baseColor = _hitAreaImage.color;
            _hitAreaImage.color = Color.black;
            yield return new WaitForSeconds(.1f);
            _hitAreaImage.color = baseColor;
        }

        private IEnumerator WaitAndShowGameOver()
        {
            yield return new WaitForSeconds(2f);
            ShowGameOver();
        }

        private IEnumerator CumHit()
        {
            _cumText.color = new(0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(.1f);
            _cumText.color = Color.black;
        }

        public void OnHit(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (VNManager.Instance.IsPlayingStory)
                {
                    VNManager.Instance.DisplayNextDialogue();
                }
                else if (_cumRequirementStoke > 0)
                {
                    StartCoroutine(CumHit());
                    _cumRequirementStoke--;
                    if (_cumRequirementStoke == 0)
                    {
                        _cumText.gameObject.SetActive(false);
                        _anim.SetTrigger("Cum");
                        StartCoroutine(WaitAndShowGameOver());
                    }
                }
                else if (_isAlive)
                {
                    if (_hypnotismHits > 0)
                    {
                        _hypnotismHits--;
                        _hypnotismCounter.text = _hypnotismHits.ToString();
                        if (_hypnotismHits == 0)
                        {
                            _hypnotismCounter.gameObject.SetActive(false);
                            _hitAreaImage.color = Color.white;
                        }
                        return;
                    }

                    if (_notes.Any())
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
                    }
                    StartCoroutine(HitEffect());
                }
            }
        }

        public class NoteInfo
        {
            public RectTransform RT;
            public Image Image;
            public bool IsTrap;
            public bool IsHypnotic;
        }
    }
}
