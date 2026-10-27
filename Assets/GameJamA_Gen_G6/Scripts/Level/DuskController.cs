using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Atardecer progresivo: en los ultimos segundos del LevelTimer la luz solar baja y se vuelve rojiza.
    /// Si se suma tiempo, el dia vuelve poco a poco.
    /// </summary>
    public class DuskController : MonoBehaviour
    {
        [SerializeField] private Light sun;
        [SerializeField] private LevelTimer timer;

        // Segundos finales del nivel en los que ocurre el atardecer.
        [SerializeField] private float duskDuration = 30f;

        [SerializeField] private Color duskColor = new Color(1f, 0.45f, 0.2f);
        [SerializeField] private float duskIntensity = 0.8f;
        [SerializeField] private float duskPitch = 8f;

        // Rapidez con la que la luz se acerca a su valor objetivo (por segundo, de 0 a 1).
        [SerializeField] private float blendSpeed = 0.2f;

        private Color dayColor;
        private float dayIntensity;
        private Vector3 dayEuler;
        private float blend;
        private float lastEnvironmentUpdate;
        private bool initialized;

        private void Start()
        {
            if (timer == null)
            {
                timer = FindFirstObjectByType<LevelTimer>();
            }

            if (sun == null)
            {
                sun = RenderSettings.sun != null ? RenderSettings.sun : FindDirectionalLight();
            }

            if (sun == null)
            {
                enabled = false;
                return;
            }

            dayColor = sun.color;
            dayIntensity = sun.intensity;
            dayEuler = sun.transform.eulerAngles;
            initialized = true;
        }

        private void OnDisable()
        {
            if (initialized && sun != null)
            {
                Apply(0f);
            }
        }

        private void Update()
        {
            if (!initialized || timer == null)
            {
                return;
            }

            float target = Mathf.Clamp01(1f - timer.Remaining / Mathf.Max(0.01f, duskDuration));
            blend = Mathf.MoveTowards(blend, target, blendSpeed * Time.deltaTime);
            Apply(blend);

            if (blend > 0f && Time.time - lastEnvironmentUpdate > 0.5f)
            {
                lastEnvironmentUpdate = Time.time;
                DynamicGI.UpdateEnvironment();
            }
        }

        private void Apply(float amount)
        {
            sun.color = Color.Lerp(dayColor, duskColor, amount);
            sun.intensity = Mathf.Lerp(dayIntensity, duskIntensity, amount);
            sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(dayEuler.x, duskPitch, amount), dayEuler.y, dayEuler.z);
        }

        private static Light FindDirectionalLight()
        {
            foreach (Light candidate in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (candidate.type == LightType.Directional)
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
