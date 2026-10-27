using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Punado de semillas. Se lanza con la misma carga y trayectoria que la lanza.
    /// Si cae sobre un terreno cavado (ISeedBed) se planta y el punado se gasta;
    /// si no, queda en el suelo y se puede recoger de nuevo. Si golpea a algo que se asusta (cuervo), lo espanta.
    /// </summary>
    public class SeedHandful : ChargedThrowable
    {
        [Header("Semilla")]
        [SerializeField] private CropType crop = CropType.Olive;

        // Si es verdadera, la planta crece al instante sin necesidad de riego.
        [SerializeField] private bool golden;

        [Header("Sonidos de impacto")]
        [SerializeField] private AudioCue landCue;

        [Header("Efectos")]
        [SerializeField] private Color dustColor = new Color(0.7f, 0.6f, 0.4f);

        public CropType Crop => crop;
        public bool IsGolden => golden;

        protected override void OnImpact(Collision collision, Vector3 point)
        {
            ISeedBed bed = collision.collider.GetComponentInParent<ISeedBed>();
            if (bed != null && bed.TryPlant(crop, golden))
            {
                Destroy(gameObject);
                return;
            }

            collision.collider.GetComponentInParent<IScareable>()?.Scare(point);

            landCue.Play(point);
            ParticleBurst.Play(point, dustColor, 8, 1.5f, 0.08f, 1.2f, 0.5f, 45f);
        }
    }
}
