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
        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private Transform _levelsContainer;

        [SerializeField]
        private GameObject _levelPrefab;

        private void Awake()
        {
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            for (int i = 0; i < _info.Music.Length; i++)
            {
                var music = _info.Music[i];

                var savedData = PersistencyManager.Instance.SaveData.GetScore(i);
                RankInfo rank = null;

                if (savedData != null)
                {
                    rank = _info.RankInfo.First(x => savedData.Score >= x.ScoreRequirement);
                }
                Instantiate(_levelPrefab, _levelsContainer).GetComponent<MenuLevelInfo>().Init(rank, music, savedData);
            }
        }

        public void OnDisplayMenu(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _menu.SetActive(!_menu.activeInHierarchy);
            }
        }
    }
}
