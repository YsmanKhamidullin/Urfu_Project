using UnityEngine.EventSystems;
using UnityEngine;

namespace Helpers.UI
{
    [RequireComponent(typeof(Graphics))]
    public class DragBlocker : UIBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}
