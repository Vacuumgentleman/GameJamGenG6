using UnityEngine;

namespace GameJam
{
    public class NumeroSimple : MonoBehaviour, IPickable
    {
        [SerializeField] int value = 5;
        public string DisplayName => $"Numero {value}";
        public GameObject GameObject => gameObject;

        Rigidbody rb;
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
        }

        public void OnPickedUp()
        {
            Debug.Log($"Recogí {value}");
            rb.isKinematic = true;
            rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
        }

        public void OnDropped(Vector3 pos, Quaternion rot, Vector3 vel)
        {
            transform.SetParent(null);
            gameObject.SetActive(true);

            if (pos == Vector3.zero)
                pos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

            transform.position = pos + Vector3.up * 0.3f;
            transform.rotation = rot;

            GetComponent<Collider>().enabled = true;
            rb.isKinematic = true; 
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Debug.Log($"Soltado en {transform.position}");
        }
    }
}