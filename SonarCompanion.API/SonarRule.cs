namespace SonarCompanion.API
{
    public enum SonarRuleStatus
    {
        Ready
    }

    public class SonarRule
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public SonarRuleStatus Status { get; set; }
    }
}