using Helpers.UI.Attributes;

namespace Windows
{
    [VisibilityThroughWindow(VisibilityThroughWindowMode.Hud)]
    public class GameplayWindow : BaseWindow<object>
    {
        public override void  OnLoad()
        {
            base.OnLoad();
        }

    }
}
