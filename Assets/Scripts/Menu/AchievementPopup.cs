using NsfwMiniJam.Achievement;
using NsfwMiniJam.Persistency;
using TMPro;
using UnityEngine;

namespace NsfwMiniJam.Menu
{
    public class AchievementPopup : MonoBehaviour
    {
        [SerializeField]
        private Transform _achievementContainer;

        [SerializeField]
        private GameObject _achievementPrefab;

        [SerializeField]
        private GameObject _sidePanel;

        [SerializeField]
        private TMP_Text _sideTitle, _sideDesc;

        private void Awake()
        {
            foreach (var c in AchievementManager.Instance.Achievements)
            {
                var key = c.Key;
                var ta = c.Value;

                var ach = Instantiate(_achievementPrefab, _achievementContainer).GetComponent<AchievementIcon>();
                ach.OnPointerEnterEvt.AddListener(new(() =>
                {
                    _sidePanel.gameObject.SetActive(true);


                    var unlocked = PersistencyManager.Instance.SaveData.UnlockedAchievements.Contains(key);
                    _sideTitle.text = unlocked ? ta.Name : "???";
                    _sideDesc.text = unlocked ? ta.Description : "???";
                }));
                ach.OnPointerExitEvt.AddListener(new(() =>
                {
                    _sidePanel.gameObject.SetActive(false);
                }));
            }
        }
    }
}
