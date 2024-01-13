using NsfwMiniJam.LevelSelect;

namespace NsfwMiniJam
{
    public static class GlobalData
    {
        public static int LevelIndex;

        public static SuddenDeathType SuddenDeath;
        public static HiddenType Hidden;
        public static bool Reversed;
        public static bool Mines;

        // TODO: Unlock it only after level played
        public static bool NoFail;

        public static float CalculateMultiplier()
        {
            float mult = 1f;

            if (NoFail) mult = .1f;

            if (SuddenDeath == SuddenDeathType.Normal) mult += .05f;
            else if (SuddenDeath == SuddenDeathType.PerfectOnly) mult += .1f;

            if (Hidden != HiddenType.None) mult += .25f;

            if (Reversed) mult += .1f;

            if (Mines) mult += .15f;

            return mult;
        }
    }
    public enum HiddenType
    {
        None,
        Normal,
        Reversed
    }

    public enum SuddenDeathType
    {
        None,
        Normal,
        PerfectOnly
    }
}
