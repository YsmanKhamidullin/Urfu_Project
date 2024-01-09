using UnityEngine;

namespace Interfaces
{
    public interface IDragHandler
    {
        bool DragTryStart(Vector2 pos);
        void DragHandle(Vector2 from, Vector2 to);
        void DragFinish();
    }
}
