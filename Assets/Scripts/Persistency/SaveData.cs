using NsfwMiniJam.Achievement;
using System.Collections.Generic;

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
        }

        public bool IsUnlocked(AchievementID id)
            => UnlockedAchievements.Contains(id);

        public void Unlock(AchievementID id)
        {
            UnlockedAchievements.Add(id);
            PersistencyManager.Instance.Save();
        }
    }

    public class ScoreData
    {
        public float Score;
        public float Multiplier;
    }
}