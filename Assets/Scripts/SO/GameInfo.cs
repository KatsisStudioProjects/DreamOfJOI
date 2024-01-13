using UnityEngine;

namespace NsfwMiniJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Note info")]
        public HitInfo[] HitInfo;
        public HitInfo MissInfo;

        [Header("Active modifiers (debug)")]
        public bool SuddenDeath;
        public bool Ghost;
        public bool Reversed;

        public float GhostDistance;
    }

    [System.Serializable]
    public class HitInfo
    {
        public string DisplayText;
        public float Distance;
        public Color Color;
        public bool DoesBreakCombo;
    }
}