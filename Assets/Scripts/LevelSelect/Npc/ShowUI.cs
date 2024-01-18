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
            if (_go != null)
            {
                _go.SetActive(false);
            }
        }
    }
}
