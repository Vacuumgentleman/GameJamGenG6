using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Sonido configurable desde el Inspector: una lista de clips (se elige uno al azar) y un volumen.
    /// Cualquier script puede tener campos AudioCue y llamar a Play(posicion).
    /// Si no hay clips asignados no hace nada, asi que se pueden dejar vacios hasta tener los sonidos.
    /// </summary>
    [System.Serializable]
    public class AudioCue
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        public bool HasClips => clips != null && clips.Length > 0;

        public void Play(Vector3 position)
        {
            if (!HasClips)
            {
                return;
            }

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
        }
    }
}
