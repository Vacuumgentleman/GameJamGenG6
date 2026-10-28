using UnityEngine;
using UnityEngine.Events;

namespace GameJam
{
    public class BoxSlot : MonoBehaviour
    {
        public NumPickableX.Tipo tipoRequerido;
        public Transform puntoEncaje; // donde se va a pegar el objeto

        [Header("Colores ITEM")]
        public Color colorCorrecto = Color.green;
        Color colorOriginal;

        [Header("Estado")]
        public bool estaOcupadoCorrecto = false;
        public NumPickableX objetoActual;

        public UnityEvent alColocarCorrecto;
        public UnityEvent alQuitar;

        void Awake()
        {
            if (puntoEncaje == null) puntoEncaje = transform;
        }

        void OnTriggerEnter(Collider other)
        {
            if (estaOcupadoCorrecto) return;

            if (other.TryGetComponent<NumPickableX>(out var item))
            {
                // Solo acepta si es el tipo correcto
                if (item.tipo == tipoRequerido)
                {
                    Colocar(item);
                }
                else
                {
                    //Debug.Log($"Este slot es para {tipoRequerido}, trajiste {item.tipo}");
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<NumPickableX>(out var item) && item == objetoActual)
            {
                if (item == objetoActual)
                {
                    estaOcupadoCorrecto = false;
                    objetoActual = null;
                    alQuitar?.Invoke();
                }
            }
        }

        void Colocar(NumPickableX item)
        {
            objetoActual = item;
            estaOcupadoCorrecto = true;
            //Guarda color original y pinta

            var rend = item.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                colorOriginal = rend.material.color;
                rend.material.color = colorCorrecto;
            }
            // Lo encaja y lo bloquea
            var rb = item.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            item.transform.position = puntoEncaje.position;
            item.transform.rotation = puntoEncaje.rotation;
            item.transform.SetParent(puntoEncaje);

            // desactivar que se pueda volver a agarrar
            item.GetComponent<Collider>().enabled = false;

            // Detener giro
            if (item.TryGetComponent<SpinItem>(out var spin))
                spin.enabled = false;

            //Debug.Log($"¡{item.tipo} colocado correctamente!");
            alColocarCorrecto?.Invoke();
        }

        // Para poder quitarlo de nuevo si quieres
        public void Liberar()
        {
            if (objetoActual != null)
            {
                objetoActual.transform.SetParent(null);
                objetoActual.GetComponent<Collider>().enabled = true;
                objetoActual = null;
                estaOcupadoCorrecto = false;
            }
        }
    }
}