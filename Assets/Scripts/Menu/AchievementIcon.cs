using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NsfwMiniJam.Menu
{
    public class AchievementIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnPointerEnterEvt { get; } = new();
        public UnityEvent OnPointerExitEvt { get; } = new();

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvt.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvt.Invoke();
        }
    }
}
