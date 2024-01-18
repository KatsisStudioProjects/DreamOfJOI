using NsfwMiniJam.LevelSelect.Npc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwMiniJam.LevelSelect.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _interactionText;
        private AInteractible _interactible;

        private Vector2 _mov;

        private Rigidbody2D _rb;
        private Animator _anim;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = _mov * 5f;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Npc"))
            {
                _interactible = collision.GetComponent<AInteractible>();
                _interactionText.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_interactible != null && _interactible.gameObject.GetInstanceID() == collision.gameObject.GetInstanceID())
            {
                _interactible.InteractionCancel();
                _interactible = null;
                _interactionText.SetActive(false);
            }
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>();

            if (_mov.magnitude != 0f)
            {
                _anim.SetFloat("X", _mov.x);
                _anim.SetFloat("Y", _mov.y);
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed && _interactible != null)
            {
                _interactible.Interact();
            }
        }
    }
}
