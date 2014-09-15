using SonarCompanion.API;

namespace SonarCompanion_VSIntegration.ViewModel
{
    public class SonarIssueViewModel
    {
        public SonarIssueViewModel(SonarIssue issue)
        {
            Issue = issue;

            var parts = issue.Component.Split(':');

            if (parts.Length == 2)
            {
                Project = parts[1].Trim();
            }
            else
            {
                Project = issue.Component;
            }

            FileName = issue.FileName;

            string folder = string.Empty;

            if (parts.Length >= 3)
            {
                folder = parts[2].Replace(FileName, string.Empty).TrimEnd(new[] {'/'});
            }

            Folder = folder;

            Line = issue.Line;
            Message = issue.Message;
            Severity = issue.Severity;
        }

        public Severity Severity { get; private set; }

        /// <summary>
        ///     Gets the project.
        /// </summary>
        public string Project { get; private set; }

        public string Folder { get; private set; }

        /// <summary>
        ///     Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        ///     Gets the line.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///     Gets the issue.
        /// </summary>
        public SonarIssue Issue { get; private set; }
    }
}