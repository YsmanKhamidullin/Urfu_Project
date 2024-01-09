using System;
using System.Collections.Generic;
using System.Reflection;

namespace Helpers
{
    public static class AttributeUtil
    {
        public static void GetAttributes<T>(Type baseType, ref List<T> result) where T : Attribute
        {
            foreach (Attribute attr in baseType.GetCustomAttributes(false))
            {
                if (attr is T)
                {
                    result.Add(attr as T);
                }
            }
        }

        public static T GetAttribute<T>(Type baseType) where T : Attribute
        {
            foreach (Attribute attr in baseType.GetCustomAttributes(false))
            {
                if (attr is T)
                {
                    return attr as T;
                }
            }
            return null;
        }

        public static T GetAttribute<T>(MethodInfo baseMethod) where T : Attribute
        {
            foreach (Attribute attr in baseMethod.GetCustomAttributes(false))
            {
                if (attr is T)
                {
                    return attr as T;
                }
            }
            return null;
        }
    }
}
