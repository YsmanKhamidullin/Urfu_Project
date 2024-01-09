using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Helpers.UI
{
    public class NoGraphicsImage : Image, IEventSystemHandler
    {
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}
