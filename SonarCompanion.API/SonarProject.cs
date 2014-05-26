namespace SonarCompanion.API
{
    public class SonarProject
    {
        public string nm { get; set; }
        public string id { get; set; }
        public string k { get; set; }
        public string sc { get; set; }
        public string qu { get; set; }

        public string Name { get { return nm; } }
        public string Id { get { return id; } }
        public string Key { get { return k; } }
    }
}