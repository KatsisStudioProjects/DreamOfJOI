using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NsfwMiniJam.Achievement
{
    public class AchievementPrefab : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TMP_Text _text;

        public void Init(Sprite sprite, string txt)
        {
            _image.sprite = sprite;
            _text.text = txt;
        }
    }
}
