using UnityEngine;

namespace NsfwMiniJam.LevelSelect.Npc
{
    public class ShowUI : AInteractible
    {
        [SerializeField]
        private GameObject _go;

        public override void Interact()
        {
            _go.SetActive(true);
        }

        public override void InteractionCancel()
        {
            _go.SetActive(false);
        }
    }
}
