using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class Vector3IntUtil
    {
        public static Vector3Int ClampToAbs1(this Vector3Int v)
        {
            return new Vector3Int
            {
                x = Mathf.Clamp(v.x, -1, 1),
                y = Mathf.Clamp(v.y, -1, 1),
                z = Mathf.Clamp(v.z, -1, 1),
            };
        }

        public static void BuildSolidLine(Vector3Int v1, Vector3Int v2, bool includeStartAndFinish, ref List<Vector3Int> toFill)
        {
            Vector3Int v1tov2 = v2 - v1;

            int dx = Mathf.Abs(v1tov2.x);
            int dy = Mathf.Abs(v1tov2.y);
            int dz = Mathf.Abs(v1tov2.z);
            int dMax = dx > dy && dx > dz ? dx : dy > dz ? dy : dz;

            for (int i = 0, maxi = dMax; i <= maxi; i++)
            {
                if (i == 0)
                {
                    if (includeStartAndFinish == true)
                    {
                        toFill.Add(v1);
                    }
                }
                else if (i == maxi)
                {
                    if (includeStartAndFinish == true)
                    {
                        toFill.Add(v2);
                    }
                }
                else
                {
                    float normalizedPosition = (float)i / (float)maxi;
                    Vector3Int offset = Vector3Int.RoundToInt((Vector3)v1tov2 * normalizedPosition);
                    toFill.Add(v1 + offset);
                }
            }
        }

        public static void BuildRay(Vector3Int v1, Vector3Int v2, int maxLength, ref List<Vector3Int> toFill)
        {
            Vector3Int v1tov2 = v2 - v1;

            int dx = Mathf.Abs(v1tov2.x);
            int dy = Mathf.Abs(v1tov2.y);
            int dz = Mathf.Abs(v1tov2.z);

            int dMax =
                dx > dy && dx > dz ? dx :
                dy > dz ? dy :
                dz;

            for (int i = 0; i <= maxLength; i++)
            {
                if (i == 0)
                {
                    toFill.Add(v1);
                }
                else if (i == dMax)
                {
                    toFill.Add(v2);
                }
                else
                {
                    float normalizedPosition = (float)i / (float)dMax;

                    Vector3Int offset = Vector3Int.RoundToInt((Vector3)v1tov2 * normalizedPosition);

                    if ((offset - v1).sqrMagnitude > maxLength * maxLength)
                    {
                        break;
                    }

                    toFill.Add(v1 + offset);
                }
            }
        }

        public static Vector3Int Lerp(Vector3Int from, Vector3Int to, int percentage)
        {
            return new Vector3Int()
            {
                x = from.x + Multiply(to.x - from.x, 100, percentage),
                y = from.y + Multiply(to.y - from.y, 100, percentage),
                z = from.z + Multiply(to.z - from.z, 100, percentage),
            };
        }

        public static Vector3Int ChangeLengthTo(this Vector3Int v, int length)
        {
            v = v * 100;

            int nativeLength = (int)v.magnitude;

            if (nativeLength == 0) { return Vector3Int.zero; }

            Vector3Int result = new Vector3Int()
            {
                x = Multiply(v.x, nativeLength, length),
                y = Multiply(v.y, nativeLength, length),
                z = Multiply(v.z, nativeLength, length),
            };

            return result;
        }

        private static int Multiply(int origin, int oldLength, int newLength)
        {
            int sign = origin == 0 ? 0 : origin > 0 ? 1 : -1;
            return (origin * 2 * newLength / oldLength + sign) / 2;
        }
    }
}
