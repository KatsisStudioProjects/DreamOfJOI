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
