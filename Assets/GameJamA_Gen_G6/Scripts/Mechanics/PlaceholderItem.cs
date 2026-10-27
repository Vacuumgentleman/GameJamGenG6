using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Objeto de prueba para comprobar recoger, cambiar, usar y soltar.
    /// Al usarlo solo escribe un mensaje en la Consola. Se puede borrar cuando haya objetos reales.
    /// </summary>
    public class PlaceholderItem : HeldItem
    {
        public override UseResult Use(in UseContext context)
        {
            Debug.Log($"{DisplayName} usado hacia {context.Aim.direction}", this);
            return UseResult.Kept;
        }
    }
}
