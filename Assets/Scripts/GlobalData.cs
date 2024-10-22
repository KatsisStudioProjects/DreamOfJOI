﻿#if !UNITY_WEBGL
using Buttplug.Client;
#endif
using NsfwMiniJam.SO;
using System;
using UnityEngine;

namespace NsfwMiniJam
{
    public static class GlobalData
    {
        public static int LevelIndex;

        public static string CustomSongPath;
        public static MusicInfo CustomMusic;
        public static AudioType CustomMusicAudioType;

        public static SuddenDeathType SuddenDeath;
        public static HiddenType Hidden;
        public static bool Reversed;
        public static PitchType PitchValue;

        public static float[] BpmValues = new[] { 400f, 600f, 800f, 1000f, 1200f, 1400f };
        public static int TargetBpm = 1;

        public static bool SkipDialogues;

#if !UNITY_WEBGL
        public static ButtplugClient ButtplugClient;
#endif

        static GlobalData()
        {
            AppDomain.CurrentDomain.DomainUnload += (sender, e) =>
            {
#if !UNITY_WEBGL
                if (ButtplugClient != null && ButtplugClient.Connected)
                {
                    ButtplugClient.DisconnectAsync().GetAwaiter().GetResult();
                }
#endif
            };
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
