using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Clase base para objetos que el jugador puede recoger, sostener y usar.
    /// Se encarga de lo comun: apagar la fisica al recogerlo y encenderla al soltarlo.
    /// Para crear un objeto nuevo (lanza, antorcha...) solo hay que heredar de esta clase
    /// e implementar el metodo Use. No hace falta tocar nada del jugador.
    ///
    /// El GameObject del objeto debe tener un Collider y, para que caiga al suelo, un Rigidbody.
    /// </summary>
    public abstract class HeldItem : MonoBehaviour, IPickable, IUsable
    {
        [SerializeField] private string displayName = "Item";

        private Rigidbody body;
        private Collider[] colliders;

        public string DisplayName => displayName;
        public GameObject GameObject => gameObject;

        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>(true);
        }

        // Al recogerlo: sin colliders ni fisica, para que no estorbe en la mano.
        public virtual void OnPickedUp()
        {
            SetPhysics(false);
        }

        // Al soltarlo: vuelve a ser un objeto fisico que cae al suelo.
        public virtual void OnDropped(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            transform.SetPositionAndRotation(position, rotation);
            gameObject.SetActive(true);
            SetPhysics(true);
            if (body != null)
            {
                body.linearVelocity = velocity;
            }
        }

        // Ganchos opcionales: las clases hijas los sobrescriben si necesitan reaccionar.
        public virtual void OnEquipped(Transform holdPoint)
        {
        }

        public virtual void OnUnequipped()
        {
        }

        // Comportamiento del clic izquierdo. Cada objeto lo define a su manera.
        public abstract UseResult Use(in UseContext context);

        protected void SetPhysics(bool enabled)
        {
            foreach (Collider item in colliders)
            {
                item.enabled = enabled;
            }

            if (body == null)
            {
                return;
            }

            // Se detiene el movimiento antes de volverlo kinematico para evitar avisos de Unity.
            if (!enabled && !body.isKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            body.isKinematic = !enabled;
        }
    }
}
