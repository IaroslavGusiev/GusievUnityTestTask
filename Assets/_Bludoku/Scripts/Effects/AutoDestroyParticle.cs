using UnityEngine;

namespace _Bludoku.Scripts.Effects
{
    public class AutoDestroyParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake() => 
            _particleSystem = GetComponent<ParticleSystem>();

        private void Update()
        {
            if (_particleSystem.IsAlive(true) == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
