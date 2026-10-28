using UnityEngine;

namespace GameJam
{
    public class NumPickableX : MonoBehaviour, IPickable
    {
        public enum Tipo { phones, cellphone, bitcoin, pinkbox, fork }

        [Header("Configura cada objeto")]
        public Tipo tipo = Tipo.phones;

        public string DisplayName => tipo.ToString();
        public GameObject GameObject => gameObject;

        Rigidbody rb;
        Collider col;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rb.useGravity = false;
            rb.isKinematic = false;
            gameObject.tag = "CollectableNum";
        }

        public void OnPickedUp()
        {
            transform.SetParent(null);
            col.enabled = false;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void OnDropped(Vector3 pos, Quaternion rot, Vector3 vel)
        {
            transform.SetParent(null);
            gameObject.SetActive(true);

            if (pos == Vector3.zero)
                pos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

            transform.position = pos + Vector3.up * 0.3f;
            transform.rotation = rot;

            col.enabled = true;

            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}