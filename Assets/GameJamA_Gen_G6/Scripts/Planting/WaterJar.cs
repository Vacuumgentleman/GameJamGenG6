using System;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Anfora de agua: riega lo que se mira (IWaterable) y se rellena en una fuente (IWaterSource).
    /// Cada uso gasta agua aunque no haya nada que regar: el agua se derrama en el suelo.
    /// </summary>
    public class WaterJar : HandTool
    {
        [SerializeField] private int capacity = 4;

        [Header("Sonidos")]
        [SerializeField] private AudioCue refillCue;
        [SerializeField] private AudioCue spillCue;

        private int charges;

        // Anfora que el jugador tiene en la mano. Lo usa la interfaz.
        public static WaterJar Equipped { get; private set; }

        public int Capacity => capacity;
        public int Charges => charges;

        public event Action<WaterJar> Changed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Equipped = null;
        }

        protected override void Awake()
        {
            base.Awake();
            charges = capacity;
        }

        public override void OnEquipped(Transform holdPoint)
        {
            base.OnEquipped(holdPoint);
            Equipped = this;
        }

        public override void OnUnequipped()
        {
            base.OnUnequipped();
            if (Equipped == this)
            {
                Equipped = null;
            }
        }

        private void OnDisable()
        {
            if (Equipped == this)
            {
                Equipped = null;
            }
        }

        protected override bool Apply(bool hasHit, in RaycastHit hit)
        {
            IWaterSource source = hasHit ? hit.collider.GetComponentInParent<IWaterSource>() : null;
            if (source != null)
            {
                return Refill(source);
            }

            if (charges <= 0)
            {
                return false;
            }

            IWaterable target = hasHit ? hit.collider.GetComponentInParent<IWaterable>() : null;
            if (target == null || !target.TryWater())
            {
                Vector3 point = hasHit ? hit.point : transform.position + transform.forward * 1.2f;
                spillCue.Play(point);
                ParticleBurst.Play(point, new Color(0.3f, 0.6f, 1f), 12, 2f, 0.08f, 1.4f, 0.6f, 55f);
            }

            charges--;
            Changed?.Invoke(this);
            return true;
        }

        private bool Refill(IWaterSource source)
        {
            if (charges >= capacity || !source.TryDraw())
            {
                return false;
            }

            charges = capacity;
            refillCue.Play(transform.position);
            Changed?.Invoke(this);
            return true;
        }
    }
}
