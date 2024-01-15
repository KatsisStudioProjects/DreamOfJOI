using NsfwMiniJam.SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.Menu
{
    public class PlayLevelPopup : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _nameText, _modifierName, _modifierDescription;

        public void Init(MusicInfo music)
        {
            _nameText.text = music.Name;

            _modifierName.text = music.ModifierName;
            _modifierDescription.text = music.ModifierDescription;
        }

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
