using System;

namespace Helpers.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DontPauseGameAttribute : Attribute
    {
    }
}
