using UnityEngine;

namespace NsfwMiniJam.LevelSelect
{
    public class SetYLayer : MonoBehaviour
    {
        private SpriteRenderer _sr;

        [SerializeField]
        private float _offset;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _sr.sortingOrder = (int)(-(transform.position.y + _offset) * 100f + 10_000_000f);
        }
    }
}
