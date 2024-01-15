using UnityEngine;

namespace NsfwMiniJam.LevelSelect.Npc
{
    public class ShowStats : AInteractible
    {
        public override void Interact()
        {
            LevelSelectManager.Instance.ToggleMenu(true);
        }

        public override void InteractionCancel()
        {
            LevelSelectManager.Instance.ToggleMenu(false);
        }
    }
}
