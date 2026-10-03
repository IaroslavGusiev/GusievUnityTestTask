using UnityEngine;
using _Bludoku.Scripts.Combo;

namespace _Bludoku.Scripts.Score
{
    [DisallowMultipleComponent]
    public sealed class HeartSparklesView : MonoBehaviour
    {
        [Header("--- Components ---")]
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private ParticleSystem accentParticles;

        [Header("--- Combo Parameters ---")]
        [SerializeField] private ComboVisualSettings visualSettings;

        [Header("--- Heartbeat Parameters ---")]
        [SerializeField, Min(1)] private int heartbeatSparkleCount = 2;
        [SerializeField, Min(1)] private int highComboHeartbeatSparkleCount = 4;

        [Header("--- Arrival Parameters ---")]
        [SerializeField, Min(1)] private int arrivalSparkleCount = 4;
        [SerializeField, Min(1)] private int highComboArrivalSparkleCount = 6;

        private void OnDisable() =>
            Clear();

        public void SetVisible(bool visible)
        {
            if (visible == false)
            {
                Clear();
                return;
            }

            particles.gameObject.SetActive(true);
        }

        public void EmitHeartbeat(bool highCombo)
        {
            if (CanEmit() == false)
            {
                return;
            }

            int count = highCombo 
                ? highComboHeartbeatSparkleCount 
                : heartbeatSparkleCount;
            
            EmitSparkles(count, highCombo);
        }

        public void EmitArrival(bool highCombo)
        {
            if (CanEmit() == false)
            {
                return;
            }

            int count = highCombo 
                ? highComboArrivalSparkleCount 
                : arrivalSparkleCount;
            
            EmitSparkles(count, highCombo);
            EmitAccent();
        }

        public void Clear()
        {
            if (particles == null)
            {
                return;
            }

            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.gameObject.SetActive(false);
        }

        private void EmitSparkles(int count, bool highCombo)
        {
            ParticleSystem.MainModule main = particles.main;
            main.startColor = ComboVisualSettings.ResolveColor(visualSettings, highCombo);
            particles.Emit(count);
        }

        private bool CanEmit() =>
            isActiveAndEnabled && particles.gameObject.activeInHierarchy;

        private void EmitAccent() =>
            accentParticles.Emit(1);
    }
}
