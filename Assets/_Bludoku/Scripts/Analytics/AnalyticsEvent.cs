using System.Collections.Generic;

namespace _Bludoku.Scripts.Analytics
{
    public sealed class AnalyticsEvent
    {
        private readonly Dictionary<string, object> _parameters = new();

        public string Name { get; }
        
        public IReadOnlyDictionary<string, object> Parameters => _parameters;

        public AnalyticsEvent(string name) =>
            Name = name;

        public AnalyticsEvent Add(string name, string value) =>
            AddParameter(name, value);

        public AnalyticsEvent Add(string name, long value) =>
            AddParameter(name, value);

        public AnalyticsEvent Add(string name, double value) =>
            AddParameter(name, value);

        private AnalyticsEvent AddParameter(string name, object value)
        {
            _parameters.Add(name, value);
            return this;
        }
    }
}
