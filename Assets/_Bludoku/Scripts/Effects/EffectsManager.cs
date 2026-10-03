using UnityEngine;
using _Bludoku.Scripts.Combo;
using _Bludoku.Scripts.Boards;

namespace _Bludoku.Scripts.Effects
{
    public class EffectsManager : MonoBehaviour
    {
        [Header("--- Components ---")]
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private ParticleSystem comboConfetti;
        [SerializeField] private ComboMediator comboMediator;
        [SerializeField] private ComboVisualSettings visualSettings;

        [Header("--- Clear Parameters ---")]
        [SerializeField, Min(1f)] private float comboSizeMultiplier = 1.15f;
        [SerializeField, Min(1f)] private float highComboSizeMultiplier = 1.3f;

        [Header("--- Confetti Parameters ---")]
        [SerializeField, Range(0.1f, 2f)] private float confettiSizeMultiplier = 1f;
        [SerializeField, Range(0.1f, 2f)] private float highComboConfettiSizeMultiplier = 1.15f;
        [SerializeField, Range(0f, 3f)] private float confettiAmountMultiplier = 1f;
        [SerializeField, Range(0f, 3f)] private float highComboConfettiAmountMultiplier = 1.5f;

        private ParticleEffect _particleEffect;
        private ParticleEffect _confettiEffect;
        private VibrationEffect _vibrationEffect;

        private int HighComboThreshold => ComboVisualSettings.ResolveHighComboThreshold(visualSettings);

        public void Initialize(ComboVisualSettings settings)
        {
            visualSettings = settings;
            CreateEffects();
            comboMediator.PlacementProcessed += OnFigurePlaced;
        }

        public void Dispose()
        {
            if (comboMediator != null)
            {
                comboMediator.PlacementProcessed -= OnFigurePlaced;
            }
        }

        private void CreateEffects()
        {
            _vibrationEffect = new VibrationEffect();
            _particleEffect = new ParticleEffect(particles);
            _confettiEffect = new ParticleEffect(comboConfetti);
        }

        private void OnFigurePlaced(ClearResult result, ComboState state)
        {
            _vibrationEffect.Play(result);

            PlayClearParticles(result, state);

            if (ShouldPlayComboConfetti(result, state))
            {
                PlayComboConfetti(result.EffectCenter, IsHighCombo(state));
            }
        }

        private void PlayClearParticles(ClearResult result, ComboState state)
        {
            float clearSize = GetClearSizeMultiplier(state);

            foreach (Vector3 position in result.ClearedPositions)
            {
                _particleEffect.Play(position, clearSize);
            }
        }

        private void PlayComboConfetti(Vector3 position, bool highCombo)
        {
            float sizeMultiplier = highCombo
                ? highComboConfettiSizeMultiplier
                : confettiSizeMultiplier;

            float amountMultiplier = highCombo
                ? highComboConfettiAmountMultiplier
                : confettiAmountMultiplier;

            _confettiEffect.Play(position, sizeMultiplier, amountMultiplier);
        }

        private float GetClearSizeMultiplier(ComboState state)
        {
            if (state.IsActive == false)
            {
                return 1f;
            }

            return IsHighCombo(state) 
                ? highComboSizeMultiplier 
                : comboSizeMultiplier;
        }

        private bool IsHighCombo(ComboState state) =>
            state.Count >= HighComboThreshold;

        private static bool ShouldPlayComboConfetti(ClearResult result, ComboState state) =>
            result.ClearedCount > 0 && state.IsActive;
    }
}
