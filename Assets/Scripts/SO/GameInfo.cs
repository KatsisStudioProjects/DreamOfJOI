using UnityEngine;

namespace NsfwMiniJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Note info")]
        public HitInfo[] HitInfo;
        public HitInfo MissInfo;
        public HitInfo WrongInfo;

        [Header("Active modifiers (debug)")]
        public bool SuddenDeath;
        public HiddenType Hidden;
        public bool Reversed;
        public bool Mines;

        public float HiddenDistance;
        public float MineChancePercent;
    }

    public enum HiddenType
    {
        None,
        Normal,
        Reversed
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
}