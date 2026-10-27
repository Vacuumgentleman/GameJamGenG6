using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Contrato para criaturas que se asustan y huyen (cuervos). Quien las espanta indica de donde viene el susto.
    /// </summary>
    public interface IScareable
    {
        void Scare(Vector3 source);
    }
}
