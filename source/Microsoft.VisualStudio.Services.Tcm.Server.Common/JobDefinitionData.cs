// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.JobDefinitionData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class JobDefinitionData
  {
    public const string MigrationJobExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.MigrateTcmArtifactsToWitJob";
    public const string MigrationJobName = "Migrate Tcm artifacts to Wit";
    public const int JobIntervalAfterMigrationCompletion = 0;
    public const int MigrationJobInterval = 86400;
    public const string PublishTestResultsJobName = "Publish test results.";
    public const string PublishTestResultsJobExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.PublishTestResultsJob";
    public const string TcmPublishTestResultsJobExtensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.TcmPublishTestResultsJob";
    public const string CleanupJob = "TestManagement.Jobs.CleanupJob";
    public const string SignalProcessorJob = "TestManagement.Jobs.SignalProcessorJob";
    public const string CoverageAnalyzerJob = "TestManagement.Jobs.CoverageAnalyzerJob";
    public const string CalculateTestInsightsJob = "TestManagement.Jobs.CalculateTestInsightsJob";
    public static readonly Dictionary<string, Guid> TcmJobDefinitions = new Dictionary<string, Guid>()
    {
      {
        "TestManagement.Jobs.CleanupJob",
        new Guid("893ADA09-0C06-46BF-88D1-11E8A2E0E70D")
      },
      {
        "TestManagement.Jobs.SignalProcessorJob",
        new Guid("D6082AEA-9394-4CD4-9C89-9149F702C888")
      },
      {
        "TestManagement.Jobs.CoverageAnalyzerJob",
        new Guid("0e0155b0-7265-4ebf-ba5a-f4cfe442e3d8")
      },
      {
        "TestManagement.Jobs.CalculateTestInsightsJob",
        new Guid("388C827D-DFEB-44B5-997C-7034DA2A0860")
      }
    };
    public static readonly Dictionary<string, Guid> TfsJobDefinitions = new Dictionary<string, Guid>()
    {
      {
        "TestManagement.Jobs.CleanupJob",
        new Guid("D4AD074E-592B-4B59-9EAE-2E4DBB388DC0")
      },
      {
        "TestManagement.Jobs.SignalProcessorJob",
        new Guid("062E8C92-2A6F-4D13-9AFE-7B10FE372B01")
      },
      {
        "TestManagement.Jobs.CoverageAnalyzerJob",
        new Guid("747665d7-cedd-4af8-85a7-693ef5760024")
      },
      {
        "TestManagement.Jobs.CalculateTestInsightsJob",
        new Guid("6CA347E9-26EE-4806-8950-36E9E430B426")
      }
    };
  }
}
