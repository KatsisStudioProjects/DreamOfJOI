using Buttplug.Client;
using System;
using System.Collections.Generic;

namespace NsfwMiniJam
{
    public static class GlobalData
    {
        public static int LevelIndex;

        public static SuddenDeathType SuddenDeath;
        public static HiddenType Hidden;
        public static bool Reversed;
        public static PitchType PitchValue;

        public static float[] BpmValues = new[] { 400f, 600f, 800f, 1000f };
        public static int TargetBpm = 1;

        public static ButtplugClient ButtplugClient;
        public static List<ButtplugClientDevice> Devices { get; } = new();

        public static void AddDevice(object sender, DeviceAddedEventArgs e)
        {
            Devices.Add(e.Device);
        }

        public static void RemoveDevice(object sender, DeviceRemovedEventArgs e)
        {
            Devices.Remove(e.Device);
        }

        public static float CalculateMultiplier()
        {
            float mult = 1f;

            if (SuddenDeath == SuddenDeathType.Normal) mult += .05f;
            else if (SuddenDeath == SuddenDeathType.PerfectOnly) mult += .1f;

            if (Hidden == HiddenType.Normal) mult += .25f;
            if (Hidden == HiddenType.Reversed) mult += .5f;

            if (Reversed) mult += .1f;

            if (PitchValue == PitchType.IncTwo) mult += .2f;
            else if (PitchValue == PitchType.IncThree) mult += .3f;

            return mult;
        }
    }

    public enum PitchType
    {
        Normal,
        IncTwo,
        IncThree
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
