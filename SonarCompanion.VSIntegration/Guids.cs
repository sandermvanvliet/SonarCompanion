// Guids.cs
// MUST match guids.h
using System;

namespace Rabobank.SonarCompanion_VSIntegration
{
    static class GuidList
    {
        public const string guidSonarCompanion_VSIntegrationPkgString = "cec42c42-3d1a-4507-b5de-39c56bb8248c";
        public const string guidSonarCompanion_VSIntegrationCmdSetString = "f5795b9c-6b5c-403e-844f-ba388af82e99";
        public const string guidToolWindowPersistanceString = "70bd26a2-c867-4f03-b1a3-b60814283b2b";

        public static readonly Guid guidSonarCompanion_VSIntegrationCmdSet = new Guid(guidSonarCompanion_VSIntegrationCmdSetString);
    };
}