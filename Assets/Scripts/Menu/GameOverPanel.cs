using NsfwMiniJam.Achievement;
using NsfwMiniJam.SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.Menu
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _notation;

        [SerializeField]
        private TMP_Text _scoreText;

        [SerializeField]
        private GameObject _fullCombo;

        public void Init(int score, int scoreMax, GameInfo info, bool isFullCombo)
        {
            if (score == 1f)
            {
                AchievementManager.Instance.Unlock(AchievementID.Perfect);
            }

            _fullCombo.SetActive(isFullCombo);

            var finalScore = (score * 1_000_000f / scoreMax) * GlobalData.CalculateMultiplier();
            _scoreText.text = finalScore.ToString("0 000 000");
            if (finalScore > 1_000_000f)
            {
                AchievementManager.Instance.Unlock(AchievementID.MoreThanPerfectScore);
            }

            var s = score / (float)scoreMax;
            foreach (var r in info.RankInfo)
            {
                if (s >= r.ScoreRequirement)
                {
                    _notation.text = r.Notation;
                    _notation.color = r.Color;
                    break;
                }
            }
        }

        public void BackToLevelSelect()
        {
            SceneManager.LoadScene("LevelSelect");
        }

        public void Retry()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
