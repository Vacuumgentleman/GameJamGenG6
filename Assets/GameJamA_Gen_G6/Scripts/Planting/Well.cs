using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Pozo: fuente de agua ilimitada para rellenar el anfora.
    /// </summary>
    public class Well : MonoBehaviour, IWaterSource, IHighlightable
    {
        public bool CanHighlight => true;
        public string HighlightText => "Pozo\nRellena el anfora con agua";

        public bool TryDraw()
        {
            return true;
        }
    }
}
