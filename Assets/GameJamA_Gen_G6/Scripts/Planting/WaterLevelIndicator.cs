using UnityEngine;

namespace GameJam
{
    public class WaterLevelIndicator : MonoBehaviour
    {
        [SerializeField] private WaterJar jar;
        [SerializeField] private Renderer target;

        private Vector3 fullScale;

        private void Awake()
        {
            if (jar == null)
            {
                jar = GetComponentInParent<WaterJar>();
            }

            if (target == null)
            {
                target = GetComponent<Renderer>();
            }

            fullScale = transform.localScale;
        }

        private void OnEnable()
        {
            jar.Changed += Refresh;
        }

        private void Start()
        {
            Refresh(jar);
        }

        private void OnDisable()
        {
            jar.Changed -= Refresh;
        }

        private void Refresh(WaterJar source)
        {
            float level = source.Capacity > 0 ? Mathf.Clamp01((float)source.Charges / source.Capacity) : 0f;
            target.enabled = level > 0f;
            transform.localScale = fullScale * level;
        }
    }
}
