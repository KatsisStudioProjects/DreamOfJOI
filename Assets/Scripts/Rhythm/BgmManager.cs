using System.Collections;
using UnityEngine;

namespace NsfwMiniJam.Rhythm
{
    public class BgmManager : MonoBehaviour
    {
        public static BgmManager Instance { get; private set; }

        [SerializeField]
        private AudioSource _bgm;

        private float _waitBeforeStart = 3f;

        private float _bpm;
        private float _yHitArea;

        private float _speedMultiplier = 10f;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _bpm = RhythmManager.Instance.Music.Bpm;
            _bgm.clip = RhythmManager.Instance.Music.Music;
            _yHitArea = RhythmManager.Instance.YHitArea;
        }

        public void MoveNote()
        {

        }

        public float GetNoteNextPos(int index)
        {
            return index * 100f;
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
