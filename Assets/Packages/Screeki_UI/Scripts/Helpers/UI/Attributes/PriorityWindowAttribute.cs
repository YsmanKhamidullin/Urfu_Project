using System;

namespace Helpers.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PriorityWindowAttribute : Attribute
    {
        public readonly bool isPriorityWindow;

        public PriorityWindowAttribute(bool isPriorityWindow)
        {
            this.isPriorityWindow = isPriorityWindow;
        }
    }
}
