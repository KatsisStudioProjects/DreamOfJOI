using NsfwMiniJam.VN;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NsfwMiniJam.Rhythm
{
    public class BgmManager : MonoBehaviour
    {
        public static BgmManager Instance { get; private set; }

        [SerializeField]
        private AudioSource _bgm;

        private float _waitBeforeStart = 3f;

        private void Awake()
        {
            Instance = this;

            _bgm.clip = RhythmManager.Instance.Music.Music;
        }

        private void Update()
        {
            if (VNManager.Instance.IsPlayingStory) return;

            if (_notes.Any())
            {
                foreach (var n in _notes)
                {
                    // TODO: Move note

                    if (GlobalData.Hidden != HiddenType.None)
                    {
                        var c = n.Image.color;
                        var value = GlobalData.Hidden == HiddenType.Normal
                            ? (1f - ((n.RT.anchoredPosition.y - _hitYPos) / _info.HiddenDistance))
                            : (1f - (Screen.height - n.RT.anchoredPosition.y - _hitYPos) / _info.HiddenDistance);
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


        private void SpawnSingleNote(int stepCount)
        {
            _noteSpawnIndex += stepCount;

            var step = _speedMultiplier * 60f;
            var y = (_waitBeforeStartRef * _bpm * _speedMultiplier) + (_noteSpawnIndex * step) + _hitArea.anchoredPosition.y;


            if (y > 2000f && _leftToSpawn == 0) return;

            var n = Instantiate(_note, _noteContainer);

            n.name = $"Note {_noteSpawnIndex}";

            // DEBUG
            // n.GetComponentInChildren<TMP_Text>().text = _noteSpawnIndex.ToString();

            var rTransform = (RectTransform)n.transform;
            rTransform.anchorMin = new(.5f, 0f);
            rTransform.anchorMax = new(.5f, 0f);
            rTransform.anchoredPosition = Vector2.up * y;

            var image = n.GetComponent<Image>();

            // If we are not the last note, if hypnotism is enabled, we are not already hypnotised and chance check pass
            bool isHypnotic = _leftToSpawn > 1 && (GlobalData.Hypnotised || Music.HypnotisedOverrides) && _hypnotismHits == 0 && Random.Range(0f, 100f) < _info.HypnotismChance;

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

            _notes.Add(new() { RT = rTransform, Image = image, IsTrap = isTrap, IsHypnotic = isHypnotic, Id = _noteSpawnIndex });

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

        public IEnumerator WaitAndStartBpm()
        {
            yield return new WaitForSeconds(_waitBeforeStart);
            _bgm.Play();
        }

        public void SetVolume(float value)
        {
            _bgm.volume = value;
        }

        public void SetPitch(float value)
        {
            _bgm.pitch = value;
        }
    }
}
