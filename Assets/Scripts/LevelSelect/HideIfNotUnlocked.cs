using NsfwMiniJam.Persistency;
using UnityEngine;

namespace NsfwMiniJam.LevelSelect
{
    public class HideIfNotUnlocked : MonoBehaviour
    {
        [SerializeField]
        private int _index;

        [SerializeField]
        private bool _reverse;

        private void Awake()
        {
            var savedData = PersistencyManager.Instance.SaveData.GetScore(_index);
            if (savedData == null && !_reverse)
            {
                gameObject.SetActive(false);
            }
            else if (savedData != null && _reverse)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
