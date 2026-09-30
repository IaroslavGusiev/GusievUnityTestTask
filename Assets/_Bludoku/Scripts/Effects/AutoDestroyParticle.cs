using UnityEngine;

namespace _Bludoku.Scripts.Effects
{
    public class AutoDestroyParticle : MonoBehaviour
    {
        private ParticleSystem _ps;

        private void Awake() => _ps = GetComponent<ParticleSystem>();

        private void Update()
        {
            if (!_ps.IsAlive())
                Destroy(gameObject);
        }
    }
}
