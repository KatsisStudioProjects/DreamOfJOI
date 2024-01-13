using UnityEngine;

namespace NsfwMiniJam.LevelSelect.Npc
{
    public class LevelSelector : AInteractible
    {
        [SerializeField]
        private int _index;

        public override void Interact()
        {
            GlobalData.LevelIndex = _index;
            LevelSelectManager.Instance.ShowPlayLevelPopUp(_index);
        }

        public override void InteractionCancel()
        {
            LevelSelectManager.Instance.HidePlayerLevelPopUp();
        }
    }
}
