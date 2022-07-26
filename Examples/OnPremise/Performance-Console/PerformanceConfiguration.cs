using FiftyOne.Pipeline.Engines;

namespace FiftyOne.DeviceDetection.Examples.OnPremise.Performance
{
    public class PerformanceConfiguration
    {
        public PerformanceProfiles Profile { get; set; }
        public bool AllProperties { get; set; }
        public bool PerformanceGraph { get; set; }
        public bool PredictiveGraph { get; set; }

        public PerformanceConfiguration(PerformanceProfiles profile,
            bool allProperties, bool performanceGraph, bool predictiveGraph)
        {
            Profile = profile;
            AllProperties = allProperties;
            PerformanceGraph = performanceGraph;
            PredictiveGraph = predictiveGraph;
        }
    }
}
