using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Azada: cava la parcela que se mira. Tambien se puede usar al aire.
    /// </summary>
    public class Hoe : HandTool
    {
        protected override bool Apply(bool hasHit, in RaycastHit hit)
        {
            if (hasHit)
            {
                hit.collider.GetComponentInParent<IDiggable>()?.TryDig();
            }

            return true;
        }
    }
}
