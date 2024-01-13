using UnityEngine;

namespace NsfwMiniJam.LevelSelect.Npc
{
    public abstract class AInteractible : MonoBehaviour
    {
        public abstract void Interact();
        public abstract void InteractionCancel();
    }
}
