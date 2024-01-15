using NsfwMiniJam.Achievement;
using NsfwMiniJam.SO;
using System.Linq;
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

        public void Init(MusicInfo m, int score, int scoreMax, GameInfo info, bool isFullCombo)
        {
            if (score == 1f)
            {
                AchievementManager.Instance.Unlock(AchievementID.Perfect);
            }

            _fullCombo.SetActive(isFullCombo);
            if (isFullCombo)
            {
                if (GlobalData.ButtplugClient != null && GlobalData.ButtplugClient.Devices.Any())
                {
                    AchievementManager.Instance.Unlock(AchievementID.FCToy);
                }

                switch (GlobalData.LevelIndex)
                {
                    case 0: AchievementManager.Instance.Unlock(AchievementID.FCTutorial); break;
                    case 1: AchievementManager.Instance.Unlock(AchievementID.FCHypnotism); break;
                    case 2: AchievementManager.Instance.Unlock(AchievementID.FCChurch); break;
                    case 3: AchievementManager.Instance.Unlock(AchievementID.FCSecret); break;
                    case 4: AchievementManager.Instance.Unlock(AchievementID.FCSecret); break;
                }

                if (GlobalData.Hidden == HiddenType.Reversed)
                {
                    AchievementManager.Instance.Unlock(AchievementID.ReverseHiddenFC);
                }
                else if (GlobalData.Hidden == HiddenType.Normal && m.MinesOverrides)
                {
                    AchievementManager.Instance.Unlock(AchievementID.MineHiddenFC);
                }

                if (GlobalData.Hidden != HiddenType.None && GlobalData.SuddenDeath != SuddenDeathType.None && GlobalData.Reversed && GlobalData.PitchValue != PitchType.Normal)
                {
                    AchievementManager.Instance.Unlock(AchievementID.AllModifiersFC);
                }

                if (GlobalData.PitchValue == PitchType.IncThree && m.HypnotisedOverrides)
                {
                    AchievementManager.Instance.Unlock(AchievementID.SpeedHypnotismFC);
                }
            }

            var finalScore = (score * 1_000_000f / scoreMax) * GlobalData.CalculateMultiplier();
            _scoreText.text = finalScore.ToString("0 000 000");
            if (finalScore > 1_000_000f)
            {
                AchievementManager.Instance.Unlock(AchievementID.MoreThanPerfectScore);
            }

            var last = info.RankInfo.Last();
            _notation.text = last.Notation;
            _notation.color = last.Color;

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
