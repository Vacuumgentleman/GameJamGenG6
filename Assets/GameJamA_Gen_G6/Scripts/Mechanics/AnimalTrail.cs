using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Animal))]
    public class AnimalTrail : MonoBehaviour
    {
        [SerializeField] private float lifetime = 12f;
        [SerializeField] private float startWidth = 0.3f;
        [SerializeField] private float endWidth = 0.05f;
        [SerializeField] private float groundOffset = 0.08f;
        [SerializeField] private float minSpeed = 0.15f;
        [SerializeField] private Color walkColor = new Color(1f, 0.7f, 0.2f, 0.9f);
        [SerializeField] private Color fleeColor = new Color(1f, 0.25f, 0.1f, 0.95f);

        private Animal animal;
        private TrailRenderer trail;
        private Material material;
        private Vector3 lastPosition;
        private bool fleeColorActive;

        private void Awake()
        {
            animal = GetComponent<Animal>();
            lastPosition = transform.position;
            CreateTrail();
        }

        private void OnDestroy()
        {
            if (material != null)
            {
                Destroy(material);
            }
        }

        private void Update()
        {
            Vector3 position = transform.position;
            float speed = (position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPosition = position;

            trail.emitting = !animal.IsDead && speed > minSpeed;

            bool fleeing = animal.IsFleeing;
            if (fleeing != fleeColorActive)
            {
                fleeColorActive = fleeing;
                trail.colorGradient = BuildGradient(fleeing ? fleeColor : walkColor);
            }
        }

        private void CreateTrail()
        {
            GameObject holder = new GameObject("Trail");
            holder.transform.SetParent(transform, false);
            holder.transform.localPosition = Vector3.up * groundOffset;

            material = new Material(Shader.Find("Sprites/Default"));

            trail = holder.AddComponent<TrailRenderer>();
            trail.sharedMaterial = material;
            trail.time = lifetime;
            trail.startWidth = startWidth;
            trail.endWidth = endWidth;
            trail.minVertexDistance = 0.2f;
            trail.alignment = LineAlignment.View;
            trail.numCapVertices = 3;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trail.receiveShadows = false;
            trail.colorGradient = BuildGradient(walkColor);
            trail.emitting = false;
        }

        private static Gradient BuildGradient(Color color)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                new[] { new GradientAlphaKey(color.a, 0f), new GradientAlphaKey(0f, 1f) });
            return gradient;
        }
    }
}
