using System;

namespace Helpers.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class VisibilityThroughWindowAttribute : Attribute
    {
        public readonly VisibilityThroughWindowMode visibilityThroughWindowMode;

        public VisibilityThroughWindowAttribute(VisibilityThroughWindowMode visibilityThroughWindowMode)
        {
            this.visibilityThroughWindowMode = visibilityThroughWindowMode;
        }
    }

    public enum VisibilityThroughWindowMode : byte
    {
        NothingIsVisible = 10,
        TintedRaycastBlocker = 20,
        TransparentRaycastBlocker = 30,
        Hud = 50,
    }
}
