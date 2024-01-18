using NsfwMiniJam.Achievement;
using NsfwMiniJam.Menu;
using NsfwMiniJam.Persistency;
using NsfwMiniJam.SO;
using NsfwMiniJam.VN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace NsfwMiniJam.Rhythm
{
    public class RhythmManager : MonoBehaviour
    {
        public static RhythmManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private RectTransform _noteContainer;

        [SerializeField]
        private GameObject _note;

        [SerializeField]
        private RectTransform[] _hitArea;
        private Image[] _hitAreaImage;

        [SerializeField]
        private TMP_Text _hitText;

        [SerializeField]
        private TMP_Text _comboText;

        [SerializeField]
        private GameOverPanel _victory;

        [SerializeField]
        private RectTransform _baseContainer;

        [SerializeField]
        private TMP_Text _startCountdown;

        [SerializeField]
        private RectTransform _errorFill, _cumFill;

        [SerializeField]
        private Animator _anim, _penisAnim;

        [SerializeField]
        private TMP_Text _cumText;

        [SerializeField]
        private TMP_Text _hypnotismCounter;

        [SerializeField]
        private GameObject _containerSingleHit, _containerMultHit;

        [SerializeField]
        private Image _blindScreen;

        [SerializeField]
        private TMP_Text _midGameDialogues;

        [SerializeField]
        private Image[] _6kImgs;

        [SerializeField]
        private GameObject[] _6kObjs;

        public MusicInfo Music { private set; get; }

        private float _errorLevel;

        private int _combo;

        // Position where we are hitting notes
        private float _hitYPos;

        // Time to display the "Great", "Good" etc text
        private float _timerDisplayText;

        private bool _isAlive = true;
        // When dead, slowly decrease the speed of all notes until we reach 0
        private float _deadSpeedTimer = 1f;

        public float WaitBeforeStart { private set; get; } = 3f;

        public float BasePitch { private set; get; }

        // Manage the end of the game
        private int _leftToSpawn, _leftToTape;
        private float _volumeTimer;

        private int _maxPossibleScore;
        private int _score;

        private int _hypnotismHits;

        private int _cumRequirementStoke;

        private int _noteSpawnIndex = -1;

        private float _cumAchievementTimer = 20f;

        // List of all notes
        private List<NoteInfo> Notes { get; } = new();

        private Color _targetColor;

        private float _vibrationTimer = -1f;
        private const float _vibrationTimerRef = .2f;

        private float _blindTimer;
        private bool _blindDir;
        private float _blindDuration;

        private int _speNoteInterval;

        private float _midDialogueTimer;

        private bool _is6K;

        private bool _canStart;

        private void Awake()
        {
            Instance = this;

            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            Music = GlobalData.LevelIndex == -1 ? GlobalData.CustomMusic : _info.Music[GlobalData.LevelIndex];

            if (Music.Controller == null)
            {
                _anim.gameObject.SetActive(false);
            }
            else
            {
                _anim.runtimeAnimatorController = Music.Controller;
            }

            if (Music.KeyOverrides)
            {
                _containerMultHit.SetActive(true);
                _containerSingleHit.SetActive(false);
            }
            else
            {
                _containerMultHit.SetActive(false);
                _containerSingleHit.SetActive(true);
            }

            _leftToSpawn = Music.NoteCount;
            _leftToTape = Music.NoteCount;

            _maxPossibleScore = Music.NoteCount * _info.HitInfo.Last().Score;

            BasePitch = GlobalData.PitchValue switch
            {
                PitchType.Normal => 1f,
                PitchType.IncTwo => 2f,
                PitchType.IncThree => 3f,
                _ => throw new System.NotImplementedException()
            };

            if (GlobalData.Reversed)
            {
                _baseContainer.localScale = new(1f, -1f, 1f);
            }

            _hitAreaImage = _hitArea.Select(x => x.GetComponent<Image>()).ToArray();
            _targetColor = _hitAreaImage[0].color;
            _hitYPos = _hitArea[0].anchoredPosition.y;
        }

        private void Start()
        {
            if (GlobalData.LevelIndex == 0 && GlobalData.SuddenDeath != SuddenDeathType.None)
            {
                AchievementManager.Instance.Unlock(AchievementID.TutorialSD);
                GlobalData.SuddenDeath = SuddenDeathType.None;
            }

            void onEnd()
            {
                _canStart = true;
                _penisAnim.SetTrigger("Grow");
                if (Music.HaveReadyUpAnim)
                {
                    _anim.SetTrigger("ReadyUp");
                }
                else
                {
                    _anim.SetTrigger("Start");
                }
                _startCountdown.gameObject.SetActive(true);
            }

            BgmManager.Instance.SetPitch(BasePitch);
            if (GlobalData.LevelIndex == -1)
            {
                VNManager.Instance.ForceStop();
                _midGameDialogues.gameObject.SetActive(true);
                _midGameDialogues.text = "Please wait while the song is loading";
                StartCoroutine(LoadCustomSong(onEnd));
            }
            else
            {
                VNManager.Instance.ShowStory(Music.Intro, onEnd);
            }
        }

        private IEnumerator LoadCustomSong(Action onEnd)
        {
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{GlobalData.CustomSongPath}", GlobalData.CustomMusicAudioType);
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
                _midGameDialogues.gameObject.SetActive(false);
                BgmManager.Instance.SetClip(DownloadHandlerAudioClip.GetContent(req));
                onEnd();
            }
            else
            {
                Debug.LogError($"Failed to fetch file: {req.responseCode}");
                SceneManager.LoadScene("LevelSelect");
            }
        }

        private void Update()
        {
            if (VNManager.Instance.IsPlayingStory || !_canStart) return;

            if (_blindDir && _blindTimer < .1f) _blindTimer += Time.deltaTime;
            else if (!_blindDir && _blindTimer > 0f) _blindTimer -= Time.deltaTime;
            _blindTimer = Mathf.Clamp01(_blindTimer);

            _blindScreen.color = new(0f, 0f, 0f, _blindTimer * 10f);
            if (_blindDuration > 0f)
            {
                _blindDuration -= Time.deltaTime;
                if (_blindDuration <= 0f)
                {
                    _blindDuration = 0f;
                    _blindDir = false;
                }
            }

            if (_midDialogueTimer > 0f)
            {
                _midDialogueTimer -= Time.deltaTime;

                if (_midDialogueTimer <= 0f)
                {
                    _midGameDialogues.gameObject.SetActive(false);
                }
            }

            if (_vibrationTimer > 0f)
            {
                _vibrationTimer -= Time.deltaTime;
                if (_vibrationTimer <= 0f)
                {
                    UpdateVibrations(0f);
                }
            }

            if (WaitBeforeStart > 0f)
            {
                WaitBeforeStart -= Time.deltaTime;
                _startCountdown.text = Mathf.CeilToInt(WaitBeforeStart).ToString();
                if (WaitBeforeStart <= 0f)
                {
                    _startCountdown.gameObject.SetActive(false);
                    BgmManager.Instance.StartBgm();
                    _anim.SetTrigger("Start");
                }
            }

            _cumFill.transform.localScale = new(1f, 1f - (_leftToTape / (float)Music.NoteCount));
            if (_volumeTimer > 0f && _leftToTape == 0)
            {
                _volumeTimer -= Time.deltaTime;
                BgmManager.Instance.SetVolume(_volumeTimer * .4f);
                if (_volumeTimer <= 0f)
                {
                    _cumRequirementStoke = _info.CumStrokeCountRequirement;
                    _cumText.gameObject.SetActive(true);
                    _isAlive = false;
                }
            }

            if (_cumText.gameObject.activeInHierarchy && _cumAchievementTimer > 0f)
            {
                _cumAchievementTimer -= Time.deltaTime;
                if (_cumAchievementTimer <= 0f)
                {
                    AchievementManager.Instance.Unlock(AchievementID.WaitCum);
                }
            }

            if (!_isAlive && _deadSpeedTimer > 0f && _cumRequirementStoke == 0)
            {
                BgmManager.Instance.SetPitch(Mathf.Lerp(_deadSpeedTimer, BasePitch, 0f));
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


            if (Notes.Any())
            {
                foreach (var n in Notes)
                {
                    BgmManager.Instance.MoveNote(n);

                    if (GlobalData.Hidden != HiddenType.None)
                    {
                        var c = n.Image.color;
                        var value = GlobalData.Hidden == HiddenType.Normal
                            ? (1f - ((n.RT.anchoredPosition.y - _hitYPos) / _info.HiddenDistance))
                            : (1f - (Screen.height - n.RT.anchoredPosition.y - _hitYPos) / _info.HiddenDistance);
                        n.Image.color = new(c.r, c.g, c.b, Mathf.Clamp01(value));
                    }
                }

                if (Notes[0].RT.anchoredPosition.y - _hitYPos < -_info.HitInfo[0].Distance)
                {
                    HitNote(_info.MissInfo, Notes[0].LineRequirement);
                }
            }

            SpawnNotes();
        }

        private void DisplayMidDialogue(string[] lines)
        {
            if (lines == null || !lines.Any()) return;

            _midDialogueTimer = 3f;
            _midGameDialogues.gameObject.SetActive(true);
            _midGameDialogues.text = lines[Random.Range(0, lines.Length)];
        }

        public void UpdateVibrations(float value)
        {
            if (GlobalData.ButtplugClient != null && GlobalData.ButtplugClient.Devices.Any())
            {
                foreach (var device in  GlobalData.ButtplugClient.Devices)
                {
                    device.VibrateAsync(value);
                }
            }
        }

        public float YHitArea => _hitYPos;

        private void SpawnSingleNote(int stepCount)
        {
            var y = BgmManager.Instance.GetNoteNextPos(_noteSpawnIndex + stepCount);

            if (_leftToSpawn == 0) return;

            _noteSpawnIndex += stepCount;

            var n = Instantiate(_note, _noteContainer);

            n.name = $"Note {_noteSpawnIndex}";

            int req;
            if (Music.KeyOverrides)
            {
                if (_is6K)
                {
                    req = Random.Range(1, 7);
                }
                else
                {
                    req = Random.Range(1, 5);
                }
            }
            else
            {
                req = 0;
            }
            var rPos = (RectTransform)_hitArea[req].transform;

            if (req != 0)
            {
                //n.GetComponentInChildren<TMP_Text>().text = _reqKeys[req - 1].ToString();
            }

            var rTransform = (RectTransform)n.transform;
            rTransform.anchorMin = new(.5f, 0f);
            rTransform.anchorMax = new(.5f, 0f);

            // Too lazy to do proper maths
            rTransform.anchoredPosition = Vector2.up * y;
            rTransform.position = new(rPos.position.x, rTransform.position.y);
            rTransform.sizeDelta = new(rPos.sizeDelta.x, rTransform.sizeDelta.y);

            var image = n.GetComponent<Image>();

            bool isHypnotic = false, isTrap = false, isBlind = false, is6K = false;

            /*if (Music.KeyOverrides && _leftToSpawn == Music.NoteCount / 2)
            {
                is6K = true;
                _is6K = true;
                _noteSpawnIndex += 10;
            }
            else */if (_speNoteInterval < _info.SpeNoteMinInterval || _leftToSpawn == 1)
            { }
            else if (_speNoteInterval > _info.SpeNoteMaxInterval)
            {
                if (Music.HypnotisedOverrides) isHypnotic = true;
                else if (Music.MinesOverrides) isTrap = true;
                else if (Music.BlindOverrides) isBlind = true;
            }
            else
            {
                // If we are not the last note, if hypnotism is enabled, we are not already hypnotised and chance check pass
                isHypnotic = _leftToSpawn > 1 && Music.HypnotisedOverrides && _hypnotismHits == 0 && Random.Range(0, 100) < _info.HypnotismChance;

                // If not hypnotised (effects don't stack), mines are enabled and chance check pass
                isTrap = !isHypnotic && Music.MinesOverrides && Random.Range(0, 100) < _info.MineChancePercent;

                isBlind = Music.BlindOverrides && Random.Range(0, 100) < _info.BlindChance;
            }

            if (isHypnotic)
            {
                image.color = new(.5f, 0f, .5f);
                _speNoteInterval = 0;
            }
            else if (isTrap)
            {
                image.color = Color.red;
                _speNoteInterval = 0;
            }
            else if (isBlind)
            {
                image.color = Color.gray;
                _speNoteInterval = 0;
            }
            else if (is6K)
            {
                image.color = Color.blue;
            }
            else
            {
                _speNoteInterval++;
            }

            Notes.Add(new() { RT = rTransform, Image = image, IsTrap = isTrap, IsHypnotic = isHypnotic, IsBlind = isBlind, Is6K = is6K, Id = _noteSpawnIndex, LineRequirement = req });

            _leftToSpawn--;

            if (isHypnotic)
            {
                SpawnSingleNote(_info.HypnotismNextNoteDelay);
            }
        }

        private void SpawnNotes()
        {
            while (_leftToSpawn > 0)
            {
                SpawnSingleNote(1);
            }
        }

        private void ShowGameOver()
        {
            _victory.gameObject.SetActive(true);
            _victory.Init(Music, _score, _maxPossibleScore, _info, _combo == Music.NoteCount);

            PersistencyManager.Instance.SaveData.AddScore(GlobalData.LevelIndex, new() { Score = _score / (float)_maxPossibleScore, Multiplier = GlobalData.CalculateMultiplier(), IsFullCombo = _combo == Music.NoteCount });
            PersistencyManager.Instance.Save();

            if (_score == _maxPossibleScore)
            {
                DisplayMidDialogue(Music.DialogueInfo.Perfect);
            }
            else if (_combo == Music.NoteCount)
            {
                DisplayMidDialogue(Music.DialogueInfo.FullCombo);
            }
            else
            {
                DisplayMidDialogue(Music.DialogueInfo.Victory);
            }
        }

        private void HitNote(HitInfo data, int line)
        {
            if (!_isAlive) return;

            var note = Notes[0];

            if (note.Is6K)
            {
                foreach (var o in _6kImgs)
                {
                    o.enabled = true;
                }
                foreach (var o in _6kObjs)
                {
                    o.SetActive(true);
                }
            }

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
            else if (note.LineRequirement != 0 && note.LineRequirement != line)
            {
                data = _info.WrongInfo;
            }

            // Update combo
            if (GlobalData.SuddenDeath == SuddenDeathType.PerfectOnly && data.Score != _info.HitInfo.Last().Score)
            {
                data = _info.MissInfo;
                _isAlive = false;
                _combo = 0;
                _errorLevel = 1f;
            }
            else if (GlobalData.SuddenDeath == SuddenDeathType.Normal && data.DoesBreakCombo)
            {
                data = _info.MissInfo;
                _isAlive = false;
                _combo = 0;
                _errorLevel = 1f;
            }
            else
            {
                var isSpeNote = note.IsHypnotic || note.IsTrap || note.IsBlind || note.Is6K;
                string targetAnim = null;
                if (isSpeNote && Music.HaveSpeNoteAnim)
                {
                    targetAnim = "SpeNote";
                }
                if (data.DoesBreakCombo)
                {
                    targetAnim ??= "FailCombo";

                    if (_combo == Music.NoteCount / 3)
                    {
                        DisplayMidDialogue(Music.DialogueInfo.FailAfterComboSmall);
                    }
                    else if (_combo >= Music.NoteCount / 3)
                    {
                        DisplayMidDialogue(Music.DialogueInfo.ComboFail);
                    }

                    _combo = 0;
                }
                else
                {
                    _combo++;
                    _penisAnim.SetTrigger("Pulse");

                    if (_combo == Music.NoteCount / 3)
                    {
                        DisplayMidDialogue(Music.DialogueInfo.ComboSmall);
                    }
                    else if (_combo == 2 * Music.NoteCount / 3)
                    {
                        DisplayMidDialogue(Music.DialogueInfo.ComboBig);
                    }
                }

                if (isSpeNote)
                {
                    DisplayMidDialogue(Music.DialogueInfo.SpeNotes);
                }

                if (targetAnim != null)
                {
                    _anim.SetTrigger(targetAnim);
                }
                _anim.SetInteger("Combo", _combo);

                _errorLevel -= data.IncreaseOnHit;
            }

            // buttplug.io
            if (data.VibrationForce > 0f)
            {
                UpdateVibrations(data.VibrationForce);
                _vibrationTimer = _vibrationTimerRef;
            }

            // Update hit display
            _hitText.gameObject.SetActive(true);
            _hitText.text = data.DisplayText;
            _hitText.color = data.Color;
            _timerDisplayText = 1f;

            // Update cum level
            _errorLevel = Mathf.Clamp01(_errorLevel);
            UpdateCumBar();

            // We die if cum reach max level
            if (_errorLevel == 1f)
            {
                _isAlive = false;
            }

            // Prevent failure
            if (Music.NoFailOverrides)
            {
                _isAlive = true;
            }

            // Check victory condition
            if (!_isAlive)
            {
                _anim.SetTrigger("Defeat");
                DisplayMidDialogue(Music.DialogueInfo.Defeat);
            }
            else
            {
                _leftToTape--;
            }

            // Update combo text
            _comboText.gameObject.SetActive(_combo >= 5);
            _comboText.text = $"Combo x{_combo}";

            // Update score
            _score += data.Score;

            // Update bar color for hypnotism mode
            if (note.IsHypnotic)
            {
                _targetColor = new(.5f, 0f, .5f);
                foreach (var a in _hitAreaImage) a.color = new(.5f, 0f, .5f);
                _hypnotismHits = Mathf.CeilToInt(_info.HypnotismHitCount / BasePitch);
                _hypnotismCounter.gameObject.SetActive(true);
                _hypnotismCounter.text = _hypnotismHits.ToString();
            }

            if (note.IsBlind)
            {
                _blindDuration = _info.BlindDurationSec;
                _blindDir = true;
            }

            if (_leftToTape == 0)
            {
                _volumeTimer = 1f;
                _anim.SetTrigger("Victory");
            }

            // Destroy note
            Destroy(note.RT.gameObject);
            Notes.RemoveAt(0);

        }

        private void UpdateCumBar()
        {
            _errorFill.localScale = new(1f, _errorLevel, 1f);
        }

        private IEnumerator HitEffect(int index)
        {
            index = Music.KeyOverrides ? index : 0;

            _hitAreaImage[index].color = Color.black;
            yield return new WaitForSeconds(.1f);
            _hitAreaImage[index].color = _targetColor;
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

        public void HitNoteCallback(InputAction.CallbackContext value, int line)
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
                        if (_combo == Music.NoteCount && Music.HaveCumFC)
                        {
                            _anim.SetTrigger("CumFC");
                        }
                        else
                        {
                            _anim.SetTrigger("Cum");
                        }
                        StartCoroutine(WaitAndShowGameOver());

                        UpdateVibrations(1f);
                        _vibrationTimer = 1f;
                    }
                    else
                    {
                        UpdateVibrations(.5f);
                        _vibrationTimer = _vibrationTimerRef;
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
                            _targetColor = Color.white;
                            foreach (var a in _hitAreaImage) a.color = Color.white;
                        }
                        return;
                    }

                    if (Notes.Any())
                    {
                        for (int i = _info.HitInfo.Length - 1; i >= 0; i--)
                        {
                            var info = _info.HitInfo[i];
                            if (Mathf.Abs(_hitYPos - Notes[0].RT.anchoredPosition.y) < info.Distance)
                            {
                                HitNote(info, line);
                                break;
                            }
                        }
                    }
                    StartCoroutine(HitEffect(line));
                }
            }
        }

        public void OnHit(InputAction.CallbackContext value) => HitNoteCallback(value, 0);
        public void OnHitS(InputAction.CallbackContext value) => HitNoteCallback(value, 1);
        public void OnHitD(InputAction.CallbackContext value) => HitNoteCallback(value, 2);
        public void OnHitK(InputAction.CallbackContext value) => HitNoteCallback(value, 3);
        public void OnHitL(InputAction.CallbackContext value) => HitNoteCallback(value, 4);
        public void OnHitF(InputAction.CallbackContext value) => HitNoteCallback(value, 5);
        public void OnHitJ(InputAction.CallbackContext value) => HitNoteCallback(value, 6);

        public void OnRestart(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                SceneManager.LoadScene("Main");
            }
        }

        public void OnQuit(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                SceneManager.LoadScene("LevelSelect");
            }
        }
    }

    [System.Serializable]
    public class NoteInfo
    {
        public RectTransform RT;
        public Image Image;
        public bool IsTrap;
        public bool IsHypnotic;
        public bool IsBlind;
        public bool Is6K;
        public int Id;

        public int LineRequirement;
    }
}
