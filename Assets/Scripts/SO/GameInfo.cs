using UnityEngine;

namespace NsfwMiniJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Song info")]
        public MusicInfo[] Music;

        [Header("Note info")]
        public HitInfo[] HitInfo;
        public HitInfo MissInfo;
        public HitInfo WrongInfo;

        [Header("Rank info")]
        public RankInfo[] RankInfo;

        [Header("Modifiers")]

        public float HiddenDistance;
        public float MineChancePercent;
    }

    [System.Serializable]
    public class MusicInfo
    {
        public string Name;
        public AudioClip Music;
        public float Bpm;
        public int NoteCount;
    }

    [System.Serializable]
    public class HitInfo
    {
        public string DisplayText;
        public float Distance;
        public Color Color;
        public bool DoesBreakCombo;

        public int Score;
    }

    [System.Serializable]
    public class RankInfo
    {
        public string Notation;
        public Color Color;
        [Range(0f, 1f)]
        public float ScoreRequirement;
    }
}