// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceResources
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class TestExecutionServiceResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal TestExecutionServiceResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (TestExecutionServiceResources.resourceMan == null)
          TestExecutionServiceResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceResources", typeof (TestExecutionServiceResources).Assembly);
        return TestExecutionServiceResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => TestExecutionServiceResources.resourceCulture;
      set => TestExecutionServiceResources.resourceCulture = value;
    }

    public static string AgentNotFound => TestExecutionServiceResources.ResourceManager.GetString(nameof (AgentNotFound), TestExecutionServiceResources.resourceCulture);

    public static string ArtifactRegistrationFailed => TestExecutionServiceResources.ResourceManager.GetString(nameof (ArtifactRegistrationFailed), TestExecutionServiceResources.resourceCulture);

    public static string BuildCancelled => TestExecutionServiceResources.ResourceManager.GetString(nameof (BuildCancelled), TestExecutionServiceResources.resourceCulture);

    public static string BuildDropLocationCannotBeEmpty => TestExecutionServiceResources.ResourceManager.GetString(nameof (BuildDropLocationCannotBeEmpty), TestExecutionServiceResources.resourceCulture);

    public static string CannotManageTestConfigurations => TestExecutionServiceResources.ResourceManager.GetString(nameof (CannotManageTestConfigurations), TestExecutionServiceResources.resourceCulture);

    public static string CannotPublishTestResults => TestExecutionServiceResources.ResourceManager.GetString(nameof (CannotPublishTestResults), TestExecutionServiceResources.resourceCulture);

    public static string CannotReadProject => TestExecutionServiceResources.ResourceManager.GetString(nameof (CannotReadProject), TestExecutionServiceResources.resourceCulture);

    public static string CannotStartTheRun => TestExecutionServiceResources.ResourceManager.GetString(nameof (CannotStartTheRun), TestExecutionServiceResources.resourceCulture);

    public static string CheckPermissionFailed => TestExecutionServiceResources.ResourceManager.GetString(nameof (CheckPermissionFailed), TestExecutionServiceResources.resourceCulture);

    public static string ErrorOccured => TestExecutionServiceResources.ResourceManager.GetString(nameof (ErrorOccured), TestExecutionServiceResources.resourceCulture);

    public static string ExecutionError => TestExecutionServiceResources.ResourceManager.GetString(nameof (ExecutionError), TestExecutionServiceResources.resourceCulture);

    public static string FilterValuesNotPresent => TestExecutionServiceResources.ResourceManager.GetString(nameof (FilterValuesNotPresent), TestExecutionServiceResources.resourceCulture);

    public static string InvalidAgentId => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidAgentId), TestExecutionServiceResources.resourceCulture);

    public static string InvalidDtlEnvironmentUrl => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidDtlEnvironmentUrl), TestExecutionServiceResources.resourceCulture);

    public static string InvalidIteration => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidIteration), TestExecutionServiceResources.resourceCulture);

    public static string InvalidJobDefinition => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidJobDefinition), TestExecutionServiceResources.resourceCulture);

    public static string InvalidJobType => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidJobType), TestExecutionServiceResources.resourceCulture);

    public static string InvalidOperationRequested => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidOperationRequested), TestExecutionServiceResources.resourceCulture);

    public static string InvalidProjectInfo => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidProjectInfo), TestExecutionServiceResources.resourceCulture);

    public static string InvalidRerunProperties => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidRerunProperties), TestExecutionServiceResources.resourceCulture);

    public static string InvalidRunId => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidRunId), TestExecutionServiceResources.resourceCulture);

    public static string InvalidSliceId => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidSliceId), TestExecutionServiceResources.resourceCulture);

    public static string InvalidTestManagementRunId => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidTestManagementRunId), TestExecutionServiceResources.resourceCulture);

    public static string InvalidTestPointId => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidTestPointId), TestExecutionServiceResources.resourceCulture);

    public static string InvalidTestRunState => TestExecutionServiceResources.ResourceManager.GetString(nameof (InvalidTestRunState), TestExecutionServiceResources.resourceCulture);

    public static string MalFormedXml => TestExecutionServiceResources.ResourceManager.GetString(nameof (MalFormedXml), TestExecutionServiceResources.resourceCulture);

    public static string MessageQueueDetailsAlreadyExists => TestExecutionServiceResources.ResourceManager.GetString(nameof (MessageQueueDetailsAlreadyExists), TestExecutionServiceResources.resourceCulture);

    public static string NewTestRunSubmitted => TestExecutionServiceResources.ResourceManager.GetString(nameof (NewTestRunSubmitted), TestExecutionServiceResources.resourceCulture);

    public static string NoConfigurationsFound => TestExecutionServiceResources.ResourceManager.GetString(nameof (NoConfigurationsFound), TestExecutionServiceResources.resourceCulture);

    public static string NullArgument => TestExecutionServiceResources.ResourceManager.GetString(nameof (NullArgument), TestExecutionServiceResources.resourceCulture);

    public static string NumberOfTestCasesDiscovered => TestExecutionServiceResources.ResourceManager.GetString(nameof (NumberOfTestCasesDiscovered), TestExecutionServiceResources.resourceCulture);

    public static string ProjectContributorsGroupFullNameFormat => TestExecutionServiceResources.ResourceManager.GetString(nameof (ProjectContributorsGroupFullNameFormat), TestExecutionServiceResources.resourceCulture);

    public static string ProjectContributorsGroupName => TestExecutionServiceResources.ResourceManager.GetString(nameof (ProjectContributorsGroupName), TestExecutionServiceResources.resourceCulture);

    public static string QueryRunDetailsFailed => TestExecutionServiceResources.ResourceManager.GetString(nameof (QueryRunDetailsFailed), TestExecutionServiceResources.resourceCulture);

    public static string QueueNotFound => TestExecutionServiceResources.ResourceManager.GetString(nameof (QueueNotFound), TestExecutionServiceResources.resourceCulture);

    public static string ReservedForTestExecutionService => TestExecutionServiceResources.ResourceManager.GetString(nameof (ReservedForTestExecutionService), TestExecutionServiceResources.resourceCulture);

    public static string RunNotInProgress => TestExecutionServiceResources.ResourceManager.GetString(nameof (RunNotInProgress), TestExecutionServiceResources.resourceCulture);

    public static string RunTimeoutError => TestExecutionServiceResources.ResourceManager.GetString(nameof (RunTimeoutError), TestExecutionServiceResources.resourceCulture);

    public static string SliceAborted => TestExecutionServiceResources.ResourceManager.GetString(nameof (SliceAborted), TestExecutionServiceResources.resourceCulture);

    public static string SliceRetryLimitExhausted => TestExecutionServiceResources.ResourceManager.GetString(nameof (SliceRetryLimitExhausted), TestExecutionServiceResources.resourceCulture);

    public static string TagRegistrationHasFailed => TestExecutionServiceResources.ResourceManager.GetString(nameof (TagRegistrationHasFailed), TestExecutionServiceResources.resourceCulture);

    public static string TestEnvironmentAlreadyExistsException => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestEnvironmentAlreadyExistsException), TestExecutionServiceResources.resourceCulture);

    public static string TestExecutionServiceIdentityName => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestExecutionServiceIdentityName), TestExecutionServiceResources.resourceCulture);

    public static string TestPlanNotSpecified => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestPlanNotSpecified), TestExecutionServiceResources.resourceCulture);

    public static string TestRunFinishedNoSliceAvailable => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestRunFinishedNoSliceAvailable), TestExecutionServiceResources.resourceCulture);

    public static string TestRunQueued => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestRunQueued), TestExecutionServiceResources.resourceCulture);

    public static string TestRunTitleNotSpecified => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestRunTitleNotSpecified), TestExecutionServiceResources.resourceCulture);

    public static string TestRunUpdate => TestExecutionServiceResources.ResourceManager.GetString(nameof (TestRunUpdate), TestExecutionServiceResources.resourceCulture);

    public static string UpdateFailed => TestExecutionServiceResources.ResourceManager.GetString(nameof (UpdateFailed), TestExecutionServiceResources.resourceCulture);

    public static string WorkFlowNotFound => TestExecutionServiceResources.ResourceManager.GetString(nameof (WorkFlowNotFound), TestExecutionServiceResources.resourceCulture);
  }
}
