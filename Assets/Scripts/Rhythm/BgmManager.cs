using UnityEngine;

namespace NsfwMiniJam.Rhythm
{
    public class BgmManager : MonoBehaviour
    {
        public static BgmManager Instance { get; private set; }

        [SerializeField]
        private AudioSource _bgm;

        private float _bpm;
        private float _yHitArea;

        private float _speedMultiplier;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _bpm = RhythmManager.Instance.Music.Bpm;
            _bgm.clip = RhythmManager.Instance.Music.Music;
            _yHitArea = RhythmManager.Instance.YHitArea;

            _speedMultiplier = 600f / _bpm;
        }

        public void MoveNote(NoteInfo n)
        {
            n.RT.Translate(Vector3.down *  _speedMultiplier * _bpm * Time.deltaTime * RhythmManager.Instance.BasePitch);
        }

        public float GetNoteNextPos(int index)
        {
            var elapsed = RhythmManager.Instance.WaitBeforeStart > 0f ? RhythmManager.Instance.WaitBeforeStart * _bpm * RhythmManager.Instance.BasePitch : _bgm.time * (120f - _bpm);

            return elapsed * _speedMultiplier + _speedMultiplier * 60f * index + _yHitArea;
        }

        public void StartBgm()
        {
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
