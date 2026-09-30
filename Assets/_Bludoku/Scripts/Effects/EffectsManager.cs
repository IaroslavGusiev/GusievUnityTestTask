using _Bludoku.Scripts.Boards;
using UnityEngine;

namespace _Bludoku.Scripts.Effects
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] private Board board;
        [SerializeField] private ParticleSystem particles;

        private ParticleEffect _particleEffect;
        private VibrationEffect _vibrationEffect;
        
        private void Awake()
        {
            _particleEffect = new ParticleEffect(particles);
            _vibrationEffect = new VibrationEffect();
            
            board.OnFigurePlaced += OnFigurePlaced;
        }

        private void OnFigurePlaced(ClearResult result)
        {
            _vibrationEffect.Play(result);
            _particleEffect.Play(result);
        }
    }
}
