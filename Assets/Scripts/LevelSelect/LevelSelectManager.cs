using NsfwMiniJam.Menu;
using NsfwMiniJam.Persistency;
using NsfwMiniJam.SO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.LevelSelect
{
    public class LevelSelectManager : MonoBehaviour
    {
        public static LevelSelectManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private Transform _levelsContainer;

        [SerializeField]
        private GameObject _levelPrefab;

        [SerializeField]
        private PlayLevelPopup _playLevelPopup;

        private void Awake()
        {
            Instance = this;

            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            for (int i = 0; i < _info.Music.Length; i++)
            {
                var music = _info.Music[i];

                var savedData = PersistencyManager.Instance.SaveData.GetScore(i);
                RankInfo rank = null;

                if (savedData != null)
                {
                    rank = _info.RankInfo.FirstOrDefault(x => savedData.Score >= x.ScoreRequirement) ?? _info.RankInfo.Last();
                }
                Instantiate(_levelPrefab, _levelsContainer).GetComponent<MenuLevelInfo>().Init(rank, music, savedData);
            }
        }

        public void ToggleMenu(bool value)
        {
            _menu.SetActive(value);
        }

        public void OnDisplayMenu(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _menu.SetActive(!_menu.activeInHierarchy);
            }
        }

        public void ShowPlayLevelPopUp(int index)
        {
            _playLevelPopup.gameObject.SetActive(true);
            _playLevelPopup.Init(_info.Music[index]);
        }

        public void HidePlayerLevelPopUp()
        {
            _playLevelPopup.gameObject.SetActive(false);
        }
    }
}
