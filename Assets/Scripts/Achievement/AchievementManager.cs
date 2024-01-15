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

            if (GlobalData.LevelIndex == 0 && achievement != AchievementID.TutorialSD && achievement != AchievementID.FCTutorial)
            {
                return;
            }

            var instance = Instantiate(_prefab, _container);

            var data = Achievements[achievement];
            instance.GetComponentInChildren<TMP_Text>().text = data.Name;

            PersistencyManager.Instance.SaveData.Unlock(achievement);

            Destroy(instance, 5f);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.Perfect, new() { Name = "Sperm Bender", Description = "Get a SSS rank", Hint = "Once getting that you know there is no point click 'Replay' anymore at the end of the level" } },
            { AchievementID.MoreThanPerfectScore, new() { Name = "Modded Ejaculation", Description = "Get a score superior to 1 000 000", Hint = "Get a score even better than you possibly could" } },

            { AchievementID.ReverseHiddenFC, new() { Name = "Blindfolded", Description = "Get a full combo with reverse hidden modifier on", Hint = null } },
            { AchievementID.MineHiddenFC, new() { Name = "Surprise Delivery", Description = "Get a full combo with normal hidden modifier on the corruption level", Hint = null } },
            { AchievementID.AllModifiersFC, new() { Name = "Don't Fight The Music", Description = "Get a full combo with all the optional modifiers on", Hint = null } },
            { AchievementID.SpeedHypnotismFC, new() { Name = "Brainwash, Heavy Duty Cycle", Description = "Get a full combo with x3 speed modifier on the hypnotism stage", Hint = null } },

            { AchievementID.TutorialSD, new() { Name = "Bad Student", Description = "Attempt to play the tutorial with sudden death or perfect only modifier on", Hint = "The tutorial wasn't made for this modifier, it will be ignored!" } },

            { AchievementID.WaitCum, new() { Name = "Staying on the Edge", Description = "Wait 20s without cumming at the end of a game", Hint = "Resist the urge to cum" } },

            { AchievementID.FCTutorial, new() { Name = "Textbook Gameplay", Description = "Get a full combo on the tutorial", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FCHypnotism, new() { Name = "Mind of Steal", Description = "Get a full combo on the hypnotism level", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FCDemon, new() { Name = "Purified", Description = "Get a full combo on the corruption level", Hint = "Get a full combo on a specific level" } },
        };
    }

    public enum AchievementID
    {
        Perfect,
        MoreThanPerfectScore,

        ReverseHiddenFC,
        MineHiddenFC,
        AllModifiersFC,

        TutorialSD,

        WaitCum,

        SpeedHypnotismFC,

        FCTutorial,
        FCHypnotism,
        FCDemon
    }

    public record Achievement
    {
        public string Name;
        public string Description;
        public string Hint;
    }
}