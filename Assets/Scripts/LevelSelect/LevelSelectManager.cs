using NsfwMiniJam.SO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.LevelSelect
{
    public class LevelSelectManager : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        private void Awake()
        {
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);
        }
    }
}
