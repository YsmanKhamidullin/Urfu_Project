using System;

namespace Helpers.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RequiresBackgroundAttribute : Attribute
    {
        public readonly BackgroundType backgroundType;

        public RequiresBackgroundAttribute(BackgroundType backgroundType)
        {
            this.backgroundType = backgroundType;
        }
    }
}
