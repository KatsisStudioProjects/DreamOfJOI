using NsfwMiniJam.Persistency;
using NsfwMiniJam.SO;
using TMPro;
using UnityEngine;

namespace NsfwMiniJam.Menu
{
    public class MenuLevelInfo : MonoBehaviour
    {
        [SerializeField]
        private GameObject _played, _notPlayed;

        [SerializeField]
        private TMP_Text _levelName;

        [SerializeField]
        public TMP_Text _notation, _score, _finalScore;

        public void Init(RankInfo rank, MusicInfo music, ScoreData savedData)
        {
            if (savedData == null)
            {
                _levelName.text = "???";
                _notPlayed.SetActive(true);
                _played.SetActive(false);
            }
            else
            {
                _levelName.text = music.Name;
                _played.SetActive(true);
                _notPlayed.SetActive(false);

                _notation.text = rank.Notation;
                _notation.color = rank.Color;

                var score = savedData.Score * 1_000_000;
                _score.text = $"{score:0 000 000} x {savedData.Multiplier:0.00}";
                _finalScore.text = (score * savedData.Multiplier).ToString("0 000 000");
            }
        }
    }
}
