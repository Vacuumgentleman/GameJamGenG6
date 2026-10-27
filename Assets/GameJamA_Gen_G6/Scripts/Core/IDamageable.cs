namespace GameJam
{
    /// <summary>
    /// Contrato para todo lo que puede recibir dano (animales, enemigos, objetos destructibles).
    /// La lanza solo busca esta interfaz, asi no necesita conocer a cada animal.
    /// </summary>
    public interface IDamageable
    {
        bool IsDead { get; }
        void TakeDamage(in DamageInfo info);
    }
}
