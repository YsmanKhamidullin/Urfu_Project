using UnityEngine;

namespace Helpers
{
    public static class Vector3Util
    {
        public static bool ApproximatelyEquals(Vector3 v1, Vector3 v2)
        {
            return
                Mathf.Approximately(v1.x, v2.x)
                &&
                Mathf.Approximately(v1.y, v2.y)
                &&
                Mathf.Approximately(v1.z, v2.z);
        }
    }
}
