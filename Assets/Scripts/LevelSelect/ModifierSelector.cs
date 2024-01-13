using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace NsfwMiniJam.LevelSelect
{
    public class ModifierSelector : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _txtSuddenDeath;

        public void UpdateSuddenDeath()
        {
            GlobalData.SuddenDeath++;
            if (GlobalData.SuddenDeath > Enum.GetValues(typeof(SuddenDeathType)).Cast<SuddenDeathType>().Max())
            {
                GlobalData.SuddenDeath = 0;
            }

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
        }

        private void OnEnable()
        {
            UpdateUI();
        }
    }
}
