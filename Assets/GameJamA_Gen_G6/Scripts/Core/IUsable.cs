using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Contrato para objetos que se pueden usar con el clic izquierdo mientras se sostienen.
    /// Cada objeto (lanza, antorcha, espada) implementa su propio comportamiento.
    /// </summary>
    public interface IUsable
    {
        // El objeto pasa a estar en la mano del jugador.
        void OnEquipped(Transform holdPoint);

        // El objeto deja de estar en la mano (se cambio de slot o se solto).
        void OnUnequipped();

        // Clic izquierdo. Devuelve Consumed si el objeto deja de ser del jugador (por ejemplo, una lanza lanzada).
        UseResult Use(in UseContext context);
    }
}
