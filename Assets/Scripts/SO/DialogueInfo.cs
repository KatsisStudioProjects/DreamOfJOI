using UnityEngine;

namespace NsfwMiniJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/DialogueInfo", fileName = "DialogueInfo")]
    public class DialogueInfo : ScriptableObject
    {
        public string[] ComboSmall, ComboBig, ComboFail, SpeNotes, Defeat, Victory, FullCombo, Perfect;
    }
}