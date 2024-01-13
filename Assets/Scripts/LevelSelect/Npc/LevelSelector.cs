using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwMiniJam.LevelSelect.Npc
{
    public class LevelSelector : AInteractible
    {
        [SerializeField]
        private int _index;

        public override void Interact()
        {
            GlobalData.LevelIndex = _index;
            SceneManager.LoadScene("Main");
        }
    }
}
