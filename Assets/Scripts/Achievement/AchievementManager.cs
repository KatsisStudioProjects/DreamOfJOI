using NsfwMiniJam.Persistency;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NsfwMiniJam.Achievement
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Icon[] _icons;

        [SerializeField]
        private Sprite _unlocked;

        public Sprite GetIcon(AchievementID id)
            => _icons.FirstOrDefault(x => x.Key == id)?.Sprite ?? _unlocked;

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

            instance.GetComponent<AchievementPrefab>().Init(GetIcon(achievement), data.Name);

            PersistencyManager.Instance.SaveData.Unlock(achievement);

            Destroy(instance, 5f);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.Perfect, new() { Name = "Sperm Bender", Description = "Get a SSS rank (official songs only)", Hint = "Once getting that you know there is no point click 'Replay' anymore at the end of the level" } },
            { AchievementID.MoreThanPerfectScore, new() { Name = "Modded Ejaculation (official songs only)", Description = "Get a score superior to 1 000 000", Hint = "Get a score even better than you possibly could" } },

            { AchievementID.ReverseHiddenFC, new() { Name = "Blindfolded", Description = "Get a full combo with reverse hidden modifier on (official songs only)", Hint = null } },
            { AchievementID.MineHiddenFC, new() { Name = "Surprise Delivery", Description = "Get a full combo with normal hidden modifier on the church level", Hint = null } },
            { AchievementID.SpeedHypnotismFC, new() { Name = "Brainwash, Heavy Duty Cycle", Description = "Get a full combo with x3 speed modifier on the hypnotism stage", Hint = null } },
            { AchievementID.AllModifiersFC, new() { Name = "Don't Fight The Music", Description = "Get a full combo with all the optional modifiers on (official songs only)", Hint = null } },

            { AchievementID.TutorialSD, new() { Name = "Bad Student", Description = "Attempt to play the tutorial with sudden death or perfect only modifier on", Hint = "The tutorial wasn't made for this modifier, it will be ignored!" } },

            { AchievementID.WaitCum, new() { Name = "Staying on the Edge", Description = "Wait 20s without cumming at the end of a game", Hint = "Resist the urge to cum at the end of a level" } },

            { AchievementID.FCTutorial, new() { Name = "Textbook Gameplay", Description = "Get a full combo on the tutorial", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FCHypnotism, new() { Name = "Mind of Steel", Description = "Get a full combo on the hypnotism level", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FCChurch, new() { Name = "Purified", Description = "Get a full combo on the church level", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FCLamia, new() { Name = "Snake's Snack", Description = "Get a full combo on the lamia level", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FCSecret, new() { Name = "Bad End Night", Description = "Get a full combo on the secret level", Hint = "Get a full combo on a specific level" } },

            { AchievementID.CustomFC, new() { Name = "Bring Your Own JOI", Description = "Full combo a custom song", Hint = "Not enough song? There is still a way..." } },
            { AchievementID.CustomPerfect, new() { Name = "Bringing Perfection", Description = "Perfect a custom song", Hint = "It's yours after all, no reason to not perfect it" } },

            { AchievementID.FCToy, new() { Name = "Welcome to the Rodeo", Description = "Get a full combo buttplug.io API and a device plugged-in", Hint = "You can use a controller if you don't have a sextoy" } },

            { AchievementID.FullFC, new() { Name = "Bringing Joy in JOI", Description = "Get a full combo on all official levels", Hint = "Get a full combo on a specific level" } },
            { AchievementID.FullPerfect, new() { Name = "Ecstacy", Description = "Get a SSS rank on all official levels", Hint = "Get a full combo on a specific level" } },

            { AchievementID.AllAchievements, new() { Name = "Midas's Touch", Description = "Get all the achievements", Hint = "Get a full combo on a specific level" } }
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
        FCChurch,
        FCLamia,
        FCSecret,

        FCToy,

        FullFC,
        FullPerfect,

        AllAchievements,

        CustomFC,
        CustomPerfect
    }

    [System.Serializable]
    public class Icon
    {
        public AchievementID Key;
        public Sprite Sprite;
    }

    public record Achievement
    {
        public string Name;
        public string Description;
        public string Hint;
    }
}