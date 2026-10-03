using UnityEngine;

namespace _Bludoku.Scripts.Combo
{
    [CreateAssetMenu(menuName = "Bludoku/Combo Visual Settings")]
    public sealed class ComboVisualSettings : ScriptableObject
    {
        private const int DefaultHighComboThreshold = 5;
        
        private static readonly Color DefaultComboColor = new(0.15f, 0.8f, 1f, 1f);
        private static readonly Color DefaultHighComboColor = new(1f, 0.76f, 0.2f, 1f);

        [SerializeField, Min(ComboState.ActivationThreshold)] private int highComboThreshold = DefaultHighComboThreshold;
        [SerializeField] private Color comboColor = DefaultComboColor;
        [SerializeField] private Color highComboColor = DefaultHighComboColor;

        public static int ResolveHighComboThreshold(ComboVisualSettings settings)
        {
            return settings != null
                ? settings.HighComboThreshold
                : DefaultHighComboThreshold;
        }

        public static Color ResolveColor(ComboVisualSettings settings, bool highCombo)
        {
            if (settings != null)
            {
                return highCombo 
                    ? settings.highComboColor 
                    : settings.comboColor;
            }
            
            return highCombo 
                ? DefaultHighComboColor 
                : DefaultComboColor;
        }

        private int HighComboThreshold => 
            Mathf.Max(ComboState.ActivationThreshold, highComboThreshold);
    }
}
