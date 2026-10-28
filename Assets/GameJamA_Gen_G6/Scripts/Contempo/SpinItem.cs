using UnityEngine;

namespace GameJam
{
    public class SpinItem : MonoBehaviour
    {
        [Header("Giro")]
        public float velocidad = 90f; // grados por segundo
        public Vector3 eje = Vector3.up;
        public bool girarEnElSuelo = true;
        public bool girarEnInventario = false;

        void Update()
        {
            // Si está agarrado (collider apagado) y no quieres que gire en inventario
            var col = GetComponent<Collider>();
            bool estaAgarrado = col != null && !col.enabled;

            if (estaAgarrado && !girarEnInventario) return;
            if (!estaAgarrado && !girarEnElSuelo) return;

            // Gira aunque sea kinematic
            transform.Rotate(eje, velocidad * Time.deltaTime, Space.World);
        }
    }
}