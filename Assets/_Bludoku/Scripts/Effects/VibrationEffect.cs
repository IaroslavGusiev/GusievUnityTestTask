using _Bludoku.Scripts.Boards;
//using Lofelt.NiceVibrations;
using UnityEngine;

namespace _Bludoku.Scripts.Effects
{
    public class VibrationEffect
    {
        public async void Play(ClearResult result)
        {
            for (int i = 0; i < result.ClearedGroupsCount; i++)
            {
                //HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                await System.Threading.Tasks.Task.Delay(100);
            }
        }
    }
}
