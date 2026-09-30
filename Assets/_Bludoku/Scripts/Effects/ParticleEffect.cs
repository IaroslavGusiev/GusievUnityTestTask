using _Bludoku.Scripts.Boards;
using UnityEngine;

namespace _Bludoku.Scripts.Effects
{
    public class ParticleEffect
    {
        ParticleSystem _particle;
        
        public ParticleEffect(ParticleSystem particleSystem)
        {
            _particle = particleSystem;
        }
        
        public void Play(ClearResult result)
        {
            foreach (var pos in result.ClearedPositions)
            {
                var ps = Object.Instantiate(_particle, pos, Quaternion.identity);
                ps.Play();
                ps.gameObject.AddComponent<AutoDestroyParticle>();
            }
        }
    }
}
