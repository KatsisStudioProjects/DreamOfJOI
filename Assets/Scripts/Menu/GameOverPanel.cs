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

        public void Init(int score, int scoreMax, GameInfo info)
        {
            _scoreText.text = score.ToString("000000");

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
