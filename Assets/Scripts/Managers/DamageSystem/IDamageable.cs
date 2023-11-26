namespace Assets.Scripts.Managers.DamageSystem
{
    interface IDamageable
    {
        public bool IsDead { get; }
        public bool IsInvulnerable { get; }
        public bool CanHeal { get; set; }
        public bool CanDie { get; set; }
        void ApplyHeal(int amount);
        void ApplyDamage(DamageMessage damageMessage);
        void ApplyDeath();
    }
}
