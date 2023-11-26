using Assets.Scripts.Managers.DamageSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class MeleeWeapon : MonoBehaviour
    {
        public int DamageAmount = 1;
        public float AttackDuration = 0.5f;
        public float AttackRange = 1.5f;
        public float AttackAngle = 45.0f;
        public LayerMask AttackLayerMask;

        private Transform _attackOrigin;
        private GameObject _owner;
        private bool _isAttacking;
        private float _attackTimer;
        private HashSet<GameObject> _attackedObjects;

        // Start is called before the first frame update
        void Start()
        {


        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialize(GameObject owner, Transform attackOrigin)
        {
            _owner = owner;
            _attackOrigin = attackOrigin;
        }

        public void Attack()
        {
            if (_isAttacking)
                return;

            _isAttacking = true;
            _attackTimer = AttackDuration;
            _attackedObjects = new  HashSet<GameObject>();
        }


        private void FixedUpdate()
        {
            if (!_isAttacking)
                return;

            _attackTimer -= Time.fixedDeltaTime;
            if (_attackTimer <= 0.0f)
            {
                _isAttacking = false;
                return;
            }

            var colliders = Physics.OverlapSphere(_attackOrigin.position, AttackRange);
            foreach (var collider in colliders)
            {
                var direction = collider.transform.position - _attackOrigin.position;
                var angle = Vector3.Angle(_attackOrigin.forward, direction);
                if (angle > AttackAngle)
                    continue;

                var damageable = collider.GetComponent<DamageablePlayer>();
                if (damageable == null)
                    continue;
                if (damageable.gameObject == _owner)
                    continue;
                if (_attackedObjects.Contains(damageable.gameObject))
                    continue;

                _attackedObjects.Add(damageable.gameObject);
                var msg = new DamageMessage()
                {
                    Amount = DamageAmount,
                    Direction = direction,
                    Damager = this
                };
                damageable.ApplyDamage(msg);
            }
        }
    }
}