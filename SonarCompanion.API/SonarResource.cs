using System;

namespace SonarCompanion.API
{
    public class SonarResource
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Scope { get; set; }
        public string Qualifier { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public string lname { get; set; }
        public string Lang { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}, {2}, {3}", Id, Name, Lang, Version);
        }
    }
}