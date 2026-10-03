using System;
using System.Text;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

namespace _Bludoku.Scripts.Analytics
{
    public sealed class ConsoleAnalyticsProvider : IAnalyticsProvider
    {
        public void Track(AnalyticsEvent analyticsEvent)
        {
            var message = new StringBuilder("[Analytics] ");
            
            message.Append(analyticsEvent.Name);

            foreach (KeyValuePair<string, object> parameter in analyticsEvent.Parameters)
            {
                message.Append(" | ")
                    .Append(parameter.Key)
                    .Append('=')
                    .Append(Convert.ToString(parameter.Value, CultureInfo.InvariantCulture));
            }

            Debug.Log($"<color=yellow>{message}</color>");
        }
    }
}
