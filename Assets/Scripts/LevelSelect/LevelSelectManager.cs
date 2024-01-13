using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.LevelSelect
{
    public class LevelSelectManager : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);
        }
    }
}
