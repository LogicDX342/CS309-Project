using UnityEngine;

namespace Assets.Scripts.Managers.DamageSystem
{
    public struct DamageMessage
    {
        public int Amount;
        public Vector3 Direction;
        public MonoBehaviour Damager;
    }
}