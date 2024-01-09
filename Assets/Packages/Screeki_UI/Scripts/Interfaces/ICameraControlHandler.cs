using UnityEngine;

namespace Interfaces
{
    public interface ICameraControlHandler
    {
        // void HandleZoom(Vector2 zoomCenter, float zoomDelta);
        // void HandlePan(Vector2 from, Vector2 to);
        // void HandleRotate(Vector2 rotateCenter, float rotateAngle);
        // bool TryGrabControl();
        // void ReleaseControl();
        Ray GetRay(Vector2 screenPosition);
    }
}
