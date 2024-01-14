using NsfwMiniJam.Persistency;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NsfwMiniJam.Achievement
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _prefab;

        public static AchievementManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Unlock(AchievementID achievement)
        {
            if (PersistencyManager.Instance.SaveData.IsUnlocked(achievement))
            {
                return;
            }
            var instance = Instantiate(_prefab, _container);

            var data = Achievements[achievement];
            instance.GetComponentInChildren<TMP_Text>().text = data.Name;

            PersistencyManager.Instance.SaveData.Unlock(achievement);
            PersistencyManager.Instance.Save();

            Destroy(instance, 5f);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.Perfect, new() { Name = "Perfect control", Description = "Get a SSS rank" } },
            { AchievementID.MoreThanPerfectScore, new() { Name = "Exceeded Expectations", Description = "Get a score superior to 1 000 000" } }
        };
    }

    public enum AchievementID
    {
        Perfect,
        MoreThanPerfectScore
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}