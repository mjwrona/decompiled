// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestRunContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestRunContractConverter
  {
    internal static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun Convert(
      TestRun testRun)
    {
      if (testRun == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun legacyTestRun = new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun();
      legacyTestRun.TeamProjectUri = testRun.TeamProjectUri;
      legacyTestRun.TeamProject = testRun.TeamProject;
      legacyTestRun.Title = testRun.Title;
      legacyTestRun.Owner = testRun.Owner;
      legacyTestRun.OwnerName = testRun.OwnerName;
      legacyTestRun.BuildUri = testRun.BuildUri;
      legacyTestRun.BuildNumber = testRun.BuildNumber;
      legacyTestRun.BuildConfigurationId = testRun.BuildConfigurationId;
      legacyTestRun.BuildPlatform = testRun.BuildPlatform;
      legacyTestRun.BuildFlavor = testRun.BuildFlavor;
      legacyTestRun.CreationDate = testRun.CreationDate;
      legacyTestRun.LastUpdated = testRun.LastUpdated;
      legacyTestRun.StartDate = testRun.StartDate;
      legacyTestRun.CompleteDate = testRun.CompleteDate;
      legacyTestRun.Controller = testRun.Controller;
      legacyTestRun.TestPlanId = testRun.TestPlanId;
      legacyTestRun.TestSettingsId = testRun.TestSettingsId;
      legacyTestRun.PublicTestSettingsId = testRun.PublicTestSettingsId;
      legacyTestRun.TestEnvironmentId = testRun.TestEnvironmentId;
      legacyTestRun.Revision = testRun.Revision;
      legacyTestRun.LastUpdatedBy = testRun.LastUpdatedBy;
      legacyTestRun.LastUpdatedByName = testRun.LastUpdatedByName;
      legacyTestRun.Comment = testRun.Comment;
      legacyTestRun.BugsCount = testRun.BugsCount;
      legacyTestRun.RowVersion = testRun.RowVersion;
      legacyTestRun.ServiceVersion = testRun.ServiceVersion;
      legacyTestRun.TestRunId = testRun.TestRunId;
      IEnumerable<LegacyTestRunStatistic> source1 = TestRunStatisticConverter.Convert((IEnumerable<TestRunStatistic>) testRun.TestRunStatistics);
      legacyTestRun.TestRunStatistics = source1 != null ? source1.ToArray<LegacyTestRunStatistic>() : (LegacyTestRunStatistic[]) null;
      legacyTestRun.State = testRun.State;
      legacyTestRun.DropLocation = testRun.DropLocation;
      legacyTestRun.PostProcessState = testRun.PostProcessState;
      legacyTestRun.DueDate = testRun.DueDate;
      legacyTestRun.Iteration = testRun.Iteration;
      legacyTestRun.IterationId = testRun.IterationId;
      legacyTestRun.TestMessageLogId = testRun.TestMessageLogId;
      legacyTestRun.LegacySharePath = testRun.LegacySharePath;
      legacyTestRun.ErrorMessage = testRun.ErrorMessage;
      legacyTestRun.Type = testRun.Type;
      legacyTestRun.IsAutomated = testRun.IsAutomated;
      legacyTestRun.Version = testRun.Version;
      legacyTestRun.IsBvt = testRun.IsBvt;
      legacyTestRun.TotalTests = testRun.TotalTests;
      legacyTestRun.IncompleteTests = testRun.IncompleteTests;
      legacyTestRun.NotApplicableTests = testRun.NotApplicableTests;
      legacyTestRun.PassedTests = testRun.PassedTests;
      legacyTestRun.UnanalyzedTests = testRun.UnanalyzedTests;
      legacyTestRun.ReleaseUri = testRun.ReleaseUri;
      legacyTestRun.ReleaseEnvironmentUri = testRun.ReleaseEnvironmentUri;
      legacyTestRun.BuildReference = TestRunContractConverter.Convert(testRun.BuildReference);
      legacyTestRun.ReleaseReference = TestRunContractConverter.Convert(testRun.ReleaseReference);
      legacyTestRun.Filter = testRun.Filter;
      legacyTestRun.ConfigurationIds = testRun.ConfigurationIds;
      legacyTestRun.DtlAutEnvironment = testRun.DtlAutEnvironment;
      legacyTestRun.DtlTestEnvironment = testRun.DtlTestEnvironment;
      legacyTestRun.TestMessageLogEntries = testRun.TestMessageLogEntries;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField> source2 = TestExtensionFieldConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>) testRun.CustomFields);
      legacyTestRun.CustomFields = source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>) null;
      legacyTestRun.SourceWorkflow = testRun.SourceWorkflow;
      legacyTestRun.Substate = testRun.Substate;
      legacyTestRun.RunTimeout = testRun.RunTimeout;
      legacyTestRun.CsmParameters = testRun.CsmParameters;
      legacyTestRun.CsmContent = testRun.CsmContent;
      legacyTestRun.SubscriptionName = testRun.SubscriptionName;
      legacyTestRun.TestConfigurationsMapping = testRun.TestConfigurationsMapping;
      return legacyTestRun;
    }

    internal static TestRun Convert(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun)
    {
      if (testRun == null)
        return (TestRun) null;
      TestRun testRun1 = new TestRun();
      testRun1.TeamProjectUri = testRun.TeamProjectUri;
      testRun1.TeamProject = testRun.TeamProject;
      testRun1.Title = testRun.Title;
      testRun1.Owner = testRun.Owner;
      testRun1.OwnerName = testRun.OwnerName;
      testRun1.BuildUri = testRun.BuildUri;
      testRun1.BuildNumber = testRun.BuildNumber;
      testRun1.BuildConfigurationId = testRun.BuildConfigurationId;
      testRun1.BuildPlatform = testRun.BuildPlatform;
      testRun1.BuildFlavor = testRun.BuildFlavor;
      testRun1.CreationDate = testRun.CreationDate;
      testRun1.LastUpdated = testRun.LastUpdated;
      testRun1.StartDate = testRun.StartDate;
      testRun1.CompleteDate = testRun.CompleteDate;
      testRun1.Controller = testRun.Controller;
      testRun1.TestPlanId = testRun.TestPlanId;
      testRun1.TestSettingsId = testRun.TestSettingsId;
      testRun1.PublicTestSettingsId = testRun.PublicTestSettingsId;
      testRun1.TestEnvironmentId = testRun.TestEnvironmentId;
      testRun1.Revision = testRun.Revision;
      testRun1.LastUpdatedBy = testRun.LastUpdatedBy;
      testRun1.LastUpdatedByName = testRun.LastUpdatedByName;
      testRun1.Comment = testRun.Comment;
      testRun1.BugsCount = testRun.BugsCount;
      testRun1.RowVersion = testRun.RowVersion;
      testRun1.ServiceVersion = testRun.ServiceVersion;
      testRun1.TestRunId = testRun.TestRunId;
      IEnumerable<TestRunStatistic> source1 = TestRunStatisticConverter.Convert((IEnumerable<LegacyTestRunStatistic>) testRun.TestRunStatistics);
      testRun1.TestRunStatistics = source1 != null ? source1.ToArray<TestRunStatistic>() : (TestRunStatistic[]) null;
      testRun1.State = testRun.State;
      testRun1.DropLocation = testRun.DropLocation;
      testRun1.PostProcessState = testRun.PostProcessState;
      testRun1.DueDate = testRun.DueDate;
      testRun1.Iteration = testRun.Iteration;
      testRun1.IterationId = testRun.IterationId;
      testRun1.TestMessageLogId = testRun.TestMessageLogId;
      testRun1.LegacySharePath = testRun.LegacySharePath;
      testRun1.ErrorMessage = testRun.ErrorMessage;
      testRun1.Type = testRun.Type;
      testRun1.IsAutomated = testRun.IsAutomated;
      testRun1.Version = testRun.Version;
      testRun1.IsBvt = testRun.IsBvt;
      testRun1.TotalTests = testRun.TotalTests;
      testRun1.IncompleteTests = testRun.IncompleteTests;
      testRun1.NotApplicableTests = testRun.NotApplicableTests;
      testRun1.PassedTests = testRun.PassedTests;
      testRun1.UnanalyzedTests = testRun.UnanalyzedTests;
      testRun1.ReleaseUri = testRun.ReleaseUri;
      testRun1.ReleaseEnvironmentUri = testRun.ReleaseEnvironmentUri;
      testRun1.BuildReference = TestRunContractConverter.Convert(testRun.BuildReference);
      testRun1.ReleaseReference = TestRunContractConverter.Convert(testRun.ReleaseReference);
      testRun1.Filter = testRun.Filter;
      testRun1.ConfigurationIds = testRun.ConfigurationIds;
      testRun1.DtlAutEnvironment = testRun.DtlAutEnvironment;
      testRun1.DtlTestEnvironment = testRun.DtlTestEnvironment;
      testRun1.TestMessageLogEntries = testRun.TestMessageLogEntries;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField> source2 = TestExtensionFieldConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>) testRun.CustomFields);
      testRun1.CustomFields = source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>) null;
      testRun1.SourceWorkflow = testRun.SourceWorkflow;
      testRun1.Substate = testRun.Substate;
      testRun1.RunTimeout = testRun.RunTimeout;
      testRun1.CsmParameters = testRun.CsmParameters;
      testRun1.CsmContent = testRun.CsmContent;
      testRun1.SubscriptionName = testRun.SubscriptionName;
      testRun1.TestConfigurationsMapping = testRun.TestConfigurationsMapping;
      return testRun1;
    }

    internal static BuildConfiguration Convert(LegacyBuildConfiguration configuration)
    {
      if (configuration == null)
        return (BuildConfiguration) null;
      return new BuildConfiguration()
      {
        BuildConfigurationId = configuration.BuildConfigurationId,
        TeamProjectName = configuration.TeamProjectName,
        BuildUri = configuration.BuildUri,
        BuildFlavor = configuration.BuildFlavor,
        BuildPlatform = configuration.BuildPlatform,
        BuildId = configuration.BuildId,
        BuildNumber = configuration.BuildNumber,
        BuildDefinitionId = configuration.BuildDefinitionId,
        CreatedDate = configuration.CreatedDate,
        CompletedDate = configuration.CompletedDate,
        RepositoryId = configuration.RepositoryId,
        RepositoryType = configuration.RepositoryType,
        BranchName = configuration.BranchName,
        SourceVersion = configuration.SourceVersion,
        BuildSystem = configuration.BuildSystem,
        BuildQuality = configuration.BuildQuality,
        BuildDefinitionName = configuration.BuildDefinitionName,
        OldBuildConfigurationId = configuration.OldBuildConfigurationId
      };
    }

    internal static LegacyBuildConfiguration Convert(BuildConfiguration configuration)
    {
      if (configuration == null)
        return (LegacyBuildConfiguration) null;
      return new LegacyBuildConfiguration()
      {
        BuildConfigurationId = configuration.BuildConfigurationId,
        TeamProjectName = configuration.TeamProjectName,
        BuildUri = configuration.BuildUri,
        BuildFlavor = configuration.BuildFlavor,
        BuildPlatform = configuration.BuildPlatform,
        BuildId = configuration.BuildId,
        BuildNumber = configuration.BuildNumber,
        BuildDefinitionId = configuration.BuildDefinitionId,
        CreatedDate = configuration.CreatedDate,
        CompletedDate = configuration.CompletedDate,
        RepositoryId = configuration.RepositoryId,
        RepositoryType = configuration.RepositoryType,
        BranchName = configuration.BranchName,
        SourceVersion = configuration.SourceVersion,
        BuildSystem = configuration.BuildSystem,
        BuildQuality = configuration.BuildQuality,
        BuildDefinitionName = configuration.BuildDefinitionName,
        OldBuildConfigurationId = configuration.OldBuildConfigurationId
      };
    }

    internal static LegacyReleaseReference Convert(ReleaseReference releaseRef)
    {
      if (releaseRef == null)
        return (LegacyReleaseReference) null;
      return new LegacyReleaseReference()
      {
        ReleaseRefId = releaseRef.ReleaseRefId,
        ReleaseUri = releaseRef.ReleaseUri,
        ReleaseEnvUri = releaseRef.ReleaseEnvUri,
        ReleaseId = releaseRef.ReleaseId,
        ReleaseEnvId = releaseRef.ReleaseEnvId,
        ReleaseDefId = releaseRef.ReleaseDefId,
        ReleaseEnvDefId = releaseRef.ReleaseEnvDefId,
        Attempt = releaseRef.Attempt,
        ReleaseName = releaseRef.ReleaseName,
        ReleaseEnvName = releaseRef.ReleaseEnvName,
        ReleaseCreationDate = releaseRef.ReleaseCreationDate,
        EnvironmentCreationDate = releaseRef.EnvironmentCreationDate,
        PrimaryArtifactBuildId = releaseRef.PrimaryArtifactBuildId,
        PrimaryArtifactProjectId = releaseRef.PrimaryArtifactProjectId,
        PrimaryArtifactType = releaseRef.PrimaryArtifactType
      };
    }

    internal static ReleaseReference Convert(LegacyReleaseReference releaseRef)
    {
      if (releaseRef == null)
        return (ReleaseReference) null;
      return new ReleaseReference()
      {
        ReleaseRefId = releaseRef.ReleaseRefId,
        ReleaseUri = releaseRef.ReleaseUri,
        ReleaseEnvUri = releaseRef.ReleaseEnvUri,
        ReleaseId = releaseRef.ReleaseId,
        ReleaseEnvId = releaseRef.ReleaseEnvId,
        ReleaseDefId = releaseRef.ReleaseDefId,
        ReleaseEnvDefId = releaseRef.ReleaseEnvDefId,
        Attempt = releaseRef.Attempt,
        ReleaseName = releaseRef.ReleaseName,
        ReleaseEnvName = releaseRef.ReleaseEnvName,
        ReleaseCreationDate = releaseRef.ReleaseCreationDate,
        EnvironmentCreationDate = releaseRef.EnvironmentCreationDate,
        PrimaryArtifactBuildId = releaseRef.PrimaryArtifactBuildId,
        PrimaryArtifactProjectId = releaseRef.PrimaryArtifactProjectId,
        PrimaryArtifactType = releaseRef.PrimaryArtifactType
      };
    }

    internal static IEnumerable<TestRun> Convert(IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> testRuns) => testRuns == null ? (IEnumerable<TestRun>) null : testRuns.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun, TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun, TestRun>) (testRun => TestRunContractConverter.Convert(testRun)));

    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Convert(
      IEnumerable<TestRun> testRuns)
    {
      return testRuns == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null : testRuns.Select<TestRun, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>((Func<TestRun, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) (testRun => TestRunContractConverter.Convert(testRun)));
    }
  }
}
