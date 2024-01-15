﻿using NsfwMiniJam.Persistency;
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

            if (GlobalData.LevelIndex == 0 && achievement != AchievementID.TutorialSD)
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
            { AchievementID.FullCombo, new() { Name = "Textbook gameplay", Description = "Get a full combo" } },
            { AchievementID.Perfect, new() { Name = "Sperm Bender", Description = "Get a SSS rank" } },
            { AchievementID.MoreThanPerfectScore, new() { Name = "Modded Ejaculation", Description = "Get a score superior to 1 000 000" } },

            { AchievementID.ReverseHiddenFC, new() { Name = "Blindfolded", Description = "Get a full combo with reverse hidden modifier on" } },
            { AchievementID.MineHiddenFC, new() { Name = "Surprise Delivery", Description = "Get a full combo with normal hidden and mines modifiers on" } },
            { AchievementID.AllModifiersFC, new() { Name = "Don't Fight The Music", Description = "Get a full combo with all the optional modifiers on" } },
            { AchievementID.SpeedHypnotismFC, new() { Name = "Brainwash, Heavy Duty Cycle", Description = "Get a full combo with x3 speed modifier on the hypnotism stage" } },

            { AchievementID.TutorialSD, new() { Name = "Bad Student", Description = "Attempt to play the tutorial with sudden death or perfect only modifier on" } },

            { AchievementID.WaitCum, new() { Name = "Staying on the Edge", Description = "Wait 20s without cumming at the end of a game" } }
        };
    }

    public enum AchievementID
    {
        FullCombo,
        Perfect,
        MoreThanPerfectScore,

        ReverseHiddenFC,
        MineHiddenFC,
        AllModifiersFC,

        TutorialSD,

        WaitCum,

        SpeedHypnotismFC
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}