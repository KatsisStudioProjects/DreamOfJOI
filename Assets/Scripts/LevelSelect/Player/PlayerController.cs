using UnityEngine;
using UnityEngine.InputSystem;

namespace NsfwMiniJam.LevelSelect.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2 _mov;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = _mov * 10f;
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>();
        }
    }
}
