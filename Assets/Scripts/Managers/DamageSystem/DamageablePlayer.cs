using UnityEngine;

namespace Assets.Scripts.Managers.DamageSystem
{
    public class DamageablePlayer : MonoBehaviour , IDamageable
    {

        public int MaxHealth;
        public int InvulnerabilityTime;
        public bool IsDead { get; private set; }
        public bool IsInvulnerable { get; private set; }
        public bool CanHeal { get; set; } = true;
        public bool CanDie { get; set; } = true;

        private int _currentHealth;
        private float _invulnerabilityTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            _currentHealth = MaxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (_currentHealth <= 0)
            {
                if (CanDie)
                {
                    ApplyDeath();
                }
            }
            if (IsInvulnerable)
            {
                _invulnerabilityTimer += Time.deltaTime;
                if (_invulnerabilityTimer >= InvulnerabilityTime)
                {
                    IsInvulnerable = false;
                    _invulnerabilityTimer = 0.0f;
                }
            }
        }

        public void ApplyHeal(int amount)
        {
            if (!CanHeal)
                return;

            _currentHealth += amount;
            if (_currentHealth > MaxHealth)
                _currentHealth = MaxHealth;
        }

        public void ApplyDamage(DamageMessage damageMessage)
        {
            if (IsDead || IsInvulnerable)
                return;

            _currentHealth -= damageMessage.Amount;
            if (_currentHealth <= 0)
            {
                if (CanDie)
                {
                    IsDead = true;
                    _currentHealth = 0;
                }
            }
            else
            {
                IsInvulnerable = true;
                _invulnerabilityTimer = 0.0f;
            }
            Vector3 pushDirection = damageMessage.Direction;
            pushDirection.y = 0;
            pushDirection.Normalize();
            pushDirection *= 5;
            pushDirection.y = 5;
            GetComponent<Rigidbody>().AddForce(pushDirection, ForceMode.Impulse);
            Debug.Log("Damage applied to " + gameObject.name + " with amount " + damageMessage.Amount);
        }

        public void ApplyDeath()
        {
            IsDead = true;
            _currentHealth = 0;
            Destroy(gameObject);
        }


    }
}
