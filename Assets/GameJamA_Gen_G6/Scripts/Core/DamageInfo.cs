using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Informacion de un golpe, enviada a cualquier cosa que implemente IDamageable.
    /// </summary>
    public readonly struct DamageInfo
    {
        public float Amount { get; }
        public Vector3 Point { get; }
        public Vector3 Direction { get; }
        public GameObject Source { get; }

        public DamageInfo(float amount, Vector3 point, Vector3 direction, GameObject source)
        {
            Amount = amount;
            Point = point;
            Direction = direction;
            Source = source;
        }
    }
}
