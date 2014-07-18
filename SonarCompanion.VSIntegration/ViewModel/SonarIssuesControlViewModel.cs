using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonarCompanion_VSIntegration.ViewModel
{
    public class SonarIssuesControlViewModel
    {
        private bool _isLoading;

        public SonarIssuesControlViewModel()
        {
            // do nothing.
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { _isLoading = value; }
        }
    }
}
