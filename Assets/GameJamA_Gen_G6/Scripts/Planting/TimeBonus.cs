using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Reloj de arena: al tocarlo el jugador suma segundos al LevelTimer y desaparece.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TimeBonus : MonoBehaviour
    {
        [SerializeField] private float seconds = 10f;
        [SerializeField] private float spinSpeed = 90f;
        [SerializeField] private float bobHeight = 0.12f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private AudioCue collectCue;

        private LevelTimer timer;
        private Vector3 origin;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            origin = transform.position;
        }

        private void Start()
        {
            timer = FindFirstObjectByType<LevelTimer>();
        }

        private void Update()
        {
            transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
            transform.position = origin + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerMovement>() == null)
            {
                return;
            }

            if (timer != null)
            {
                timer.AddTime(seconds);
            }

            collectCue.Play(transform.position);
            Destroy(gameObject);
        }
    }
}
