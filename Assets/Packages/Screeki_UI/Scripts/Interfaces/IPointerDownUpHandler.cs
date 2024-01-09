using UnityEngine;

namespace Interfaces
{
    public interface IPointerDownUpHandler
    {
        void PointerDown(Vector2 pos);
        void PointerUp();
    }
}
