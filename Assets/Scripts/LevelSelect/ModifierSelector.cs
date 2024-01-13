using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace NsfwMiniJam.LevelSelect
{
    public class ModifierSelector : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _txtSuddenDeath, _txtHidden, _txtReversed, _txtMines;

        public void UpdateSuddenDeath()
        {
            GlobalData.SuddenDeath++;
            if (GlobalData.SuddenDeath > Enum.GetValues(typeof(SuddenDeathType)).Cast<SuddenDeathType>().Max())
            {
                GlobalData.SuddenDeath = 0;
            }

            UpdateUI();
        }

        public void UpdateHidden()
        {
            GlobalData.Hidden++;
            if (GlobalData.Hidden > Enum.GetValues(typeof(HiddenType)).Cast<HiddenType>().Max())
            {
                GlobalData.Hidden = 0;
            }

            UpdateUI();
        }

        public void UpdateReversed()
        {
            GlobalData.Reversed = !GlobalData.Reversed;
            UpdateUI();
        }

        public void UpdateMines()
        {
            GlobalData.Mines = !GlobalData.Mines;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _txtSuddenDeath.text = GlobalData.SuddenDeath switch
            {
                SuddenDeathType.None => "Disabled",
                SuddenDeathType.Normal => "Enabled",
                SuddenDeathType.PerfectOnly => "Perfect Only",
                _ => throw new NotImplementedException()
            };
            _txtHidden.text = GlobalData.Hidden switch
            {
                HiddenType.None => "Disabled",
                HiddenType.Normal => "Enabled",
                HiddenType.Reversed => "Reversed",
                _ => throw new NotImplementedException()
            };
            _txtReversed.text = GlobalData.Reversed ? "Enabled" : "Disabled";
            _txtMines.text = GlobalData.Mines ? "Enabled" : "Disabled";
        }

        private void OnEnable()
        {
            UpdateUI();
        }
    }
}
