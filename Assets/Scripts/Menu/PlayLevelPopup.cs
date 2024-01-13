using NsfwMiniJam.SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.Menu
{
    public class PlayLevelPopup : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _nameText;

        public void Init(MusicInfo music)
        {
            _nameText.text = music.Name;
        }

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
