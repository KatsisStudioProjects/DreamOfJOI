using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwMiniJam.Rhythm
{
    public class RhythmManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _noteContainer;

        [SerializeField]
        private GameObject _note;

        public void OnHit(InputAction.CallbackContext value)
        {
            if (value.performed)
            {

            }
        }
    }
}
