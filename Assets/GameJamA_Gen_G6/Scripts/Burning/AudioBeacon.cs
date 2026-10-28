using UnityEngine;

namespace GameJam
{
    public class AudioBeacon : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0f, 1f)] private float volume = 0.8f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 22f;
        [SerializeField] private float minPitch = 0.85f;
        [SerializeField] private float maxPitch = 1.2f;

        private AudioSource source;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.volume = volume;
            source.pitch = Random.Range(minPitch, maxPitch);
        }

        private void OnEnable()
        {
            if (clip == null)
            {
                return;
            }

            source.time = Random.value * clip.length;
            source.Play();
        }

        private void OnDisable()
        {
            if (source != null)
            {
                source.Stop();
            }
        }
    }
}
