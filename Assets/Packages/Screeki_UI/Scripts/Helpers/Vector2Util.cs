using UnityEngine;

namespace Helpers
{
    public static class Vector2Util
    {
        public static bool ApproximatelyEquals(Vector2 v1, Vector2 v2)
        {
            return
                Mathf.Approximately(v1.x, v2.x)
                &&
                Mathf.Approximately(v1.y, v2.y);
        }
    }
}
