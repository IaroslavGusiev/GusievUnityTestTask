using UnityEngine;

namespace _Bludoku.Scripts.Effects
{
    public class ParticleEffect
    {
        private const float MinimumSizeMultiplier = 0.1f;
        private const float MaximumSizeMultiplier = 2f;
        private const float MaximumAmountMultiplier = 3f;

        private readonly ParticleSystem _template;

        public ParticleEffect(ParticleSystem particleSystem) =>
            _template = particleSystem;

        public void Play(Vector3 position, float sizeMultiplier = 1f, float amountMultiplier = 1f)
        {
            ParticleSystem instance = CreateInstance(position);
            ApplyMultipliers(instance, sizeMultiplier, amountMultiplier);
            instance.Play(true);
        }

        private ParticleSystem CreateInstance(Vector3 position)
        {
            ParticleSystem instance = Object.Instantiate(_template, position, _template.transform.rotation);
            instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return instance;
        }

        private static void ApplyMultipliers(ParticleSystem instance, float sizeMultiplier, float amountMultiplier)
        {
            sizeMultiplier = Mathf.Clamp(sizeMultiplier, MinimumSizeMultiplier, MaximumSizeMultiplier);
            amountMultiplier = Mathf.Clamp(amountMultiplier, 0f, MaximumAmountMultiplier);

            foreach (ParticleSystem system in instance.GetComponentsInChildren<ParticleSystem>(true))
            {
                ScaleSize(system, sizeMultiplier);
                ScaleEmission(system, amountMultiplier);
            }
        }

        private static void ScaleSize(ParticleSystem system, float multiplier)
        {
            ParticleSystem.MainModule main = system.main;
            main.startSizeMultiplier *= multiplier;
        }

        private static void ScaleEmission(ParticleSystem system, float multiplier)
        {
            ParticleSystem.EmissionModule emission = system.emission;
            ScaleEmissionRates(emission, multiplier);
            ScaleBursts(emission, multiplier);
        }

        private static void ScaleEmissionRates(ParticleSystem.EmissionModule emission, float multiplier)
        {
            emission.rateOverTimeMultiplier *= multiplier;
            emission.rateOverDistanceMultiplier *= multiplier;
        }

        private static void ScaleBursts(ParticleSystem.EmissionModule emission, float multiplier)
        {
            for (var i = 0; i < emission.burstCount; i++)
            {
                ParticleSystem.Burst burst = emission.GetBurst(i);
                burst.count = ScaleBurstCount(burst.count, multiplier);
                emission.SetBurst(i, burst);
            }
        }

        private static ParticleSystem.MinMaxCurve ScaleBurstCount(ParticleSystem.MinMaxCurve count, float multiplier)
        {
            switch (count.mode)
            {
                case ParticleSystemCurveMode.Constant:
                {
                    count.constant *= multiplier;
                    break;
                }
                case ParticleSystemCurveMode.TwoConstants:
                {
                    count.constantMin *= multiplier;
                    count.constantMax *= multiplier;
                    break;
                }
                case ParticleSystemCurveMode.Curve:
                case ParticleSystemCurveMode.TwoCurves:
                {
                    count.curveMultiplier *= multiplier;
                    break;
                }
            }

            return count;
        }
    }
}
