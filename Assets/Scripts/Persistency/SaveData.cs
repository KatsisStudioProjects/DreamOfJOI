using NsfwMiniJam.Achievement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NsfwMiniJam.Persistency
{
    public class SaveData
    {
        public List<AchievementID> UnlockedAchievements { set; get; } = new();

        public Dictionary<int, ScoreData> Scores { set; get; } = new();

        public void AddScore(int level, ScoreData score)
        {
            if (!Scores.ContainsKey(level))
            {
                Scores.Add(level, score);
            }
            else
            {
                var stored = Scores[level];
                if (stored.Score * stored.Multiplier < score.Score * score.Multiplier) // Beat best score
                {
                    Scores[level] = score;
                }
            }

            if (IsAllFC()) AchievementManager.Instance.Unlock(AchievementID.FullFC);
            if (IsAllPerfect()) AchievementManager.Instance.Unlock(AchievementID.FullPerfect);

            if (UnlockedAchievements.Count == Enum.GetValues(typeof(AchievementID)).Length - 1)
            {
                AchievementManager.Instance.Unlock(AchievementID.AllAchievements);
            }
        }

        public ScoreData GetScore(int level)
            => Scores.ContainsKey(level) ? Scores[level] : null;

        public bool IsUnlocked(AchievementID id)
            => UnlockedAchievements.Contains(id);

        public void Unlock(AchievementID id)
        {
            UnlockedAchievements.Add(id);
            PersistencyManager.Instance.Save();
        }

        private bool IsAllFC() => Scores.Count == 5 && Scores.All(x => x.Value.IsFullCombo);
        private bool IsAllPerfect() => Scores.Count == 5 && Scores.All(x => x.Value.Score == 1f);
    }

    public class ScoreData
    {
        public float Score;
        public float Multiplier;
        public bool IsFullCombo;
    }
}