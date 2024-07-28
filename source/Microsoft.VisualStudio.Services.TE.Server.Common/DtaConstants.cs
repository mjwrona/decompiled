// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaConstants
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public static class DtaConstants
  {
    public static readonly TimeSpan DefaultTestRunTimeout = TimeSpan.FromHours(24.0);
    private static readonly string TfsRegistryTestExecutionRunPrefix = "/Service/TestManagement/TestExecutionService/TestRun/";
    public static readonly int DefaultMaxDelaySeconds = -1;
    public static readonly int DefaultMaxDelaySecondsForNonExecutionPhase = 600;
    public static readonly int DefaultMaxDelaySecondsForExecutionPhase = 600;
    public static readonly int CleanupPhaseIntervalInSeconds = 10;
    public static readonly string TfsRegistryPathForRunTimeOut = DtaConstants.TfsRegistryTestExecutionRunPrefix + "TestRunTimeout";
    public static readonly string TfsRegistryPathForRunEnvLocation = DtaConstants.TfsRegistryTestExecutionRunPrefix + "TestRunEnvLocation";
    public static readonly string TfsRegistryPathForRunEnvName = DtaConstants.TfsRegistryTestExecutionRunPrefix + "TestRunEnvName";
    public static readonly string TfsRegistryPathForRunEnvTeamName = DtaConstants.TfsRegistryTestExecutionRunPrefix + "TestRunEnvTeamName";
    public static readonly string TfsRegistryPathForEnvironmentDeletionFlag = DtaConstants.TfsRegistryTestExecutionRunPrefix + "TestRunEnvironmentDeletionFlag";
    public static readonly string TfsRegistryPathForAgentConnectionTimeOut = DtaConstants.TfsRegistryTestExecutionRunPrefix + "AgentConnectionTimeOut";
    public static readonly string TfsRegistryPathForAllAgentsConnectionTimeOut = DtaConstants.TfsRegistryTestExecutionRunPrefix + "AllAgentsConnectionTimeOut";
    public static readonly string TfsRegistryPathForHealthJobDelay = DtaConstants.TfsRegistryTestExecutionRunPrefix + "HealthJobDelay";
    public static readonly string TfsRegistryPathForMaxSliceRetryCount = DtaConstants.TfsRegistryTestExecutionRunPrefix + "MaxSliceRetryCount";
    public static readonly string TfsRegistryPathForTestRunInformation = DtaConstants.TfsRegistryTestExecutionRunPrefix + "TestRunInformationSource";
    public static readonly string WorkFlowJobName = "TestWorkFlowJob";
    public static readonly string WorkFlowJobExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.TestWorkFlowJob";
    public static readonly string TcmWorkFlowJobName = "TestWorkFlowJob";
    public static readonly string TcmWorkFlowJobExtensionName = "Microsoft.Azure.Pipelines.TestExecution.Server.Jobs.TestWorkFlowJob";
    public static readonly string TestServiceUserNameParameter = "testServiceUserName";
    public static readonly string TestServiceUserPasswordParameter = "testServiceUserPassword";
    public static readonly string TestServiceMessageType = "TestExecutionServiceCommand";
    public static readonly string DataCollectionOnly = nameof (DataCollectionOnly);
    public static readonly string TagSeparator = ";";
    public static readonly string EqualsString = "=";
    public static readonly int MarkInUseForSeconds = 432000;
    public static readonly Guid MessageQueueId = new Guid("C8FF56B9-EA3D-4A05-81CE-AB6C42741D85");
    public static readonly string MessageQueuePrefix = "testrun-";
    public static readonly TimeSpan DefaultAgentConnectionTimeOut = TimeSpan.FromSeconds(300.0);
    public static readonly TimeSpan DefaultAllAgentsConnectionTimeOut = TimeSpan.FromSeconds(900.0);
    public static readonly int DefaultMaxSliceRetryCount = 3;
    public static readonly TimeSpan DefaultDiscoveryPhaseTimeOut = DtaConstants.DefaultAllAgentsConnectionTimeOut;
    public static readonly TimeSpan DefaultExecutionPhaseTimeOut = TimeSpan.FromSeconds(DtaConstants.DefaultDiscoveryPhaseTimeOut.TotalSeconds * 2.0);
    public static readonly string HealthMonitorJobId = "C5D62213-AD06-48DD-B5DE-A88A49310B3F";
    public static readonly Guid TestAgentArtifactKindId = new Guid("F3967935-CFC0-49EA-B9AF-123E4FF6D058");
    public static readonly Guid AutomationRunArtifactKindId = new Guid("315806B7-1F2B-4368-B94B-0E469F5E12FC");
    public static readonly Guid TcmTestAgentArtifactKindId = new Guid("0D79C70D-03E6-4380-95C5-F269AEC78F81");
    public static readonly Guid TcmAutomationRunArtifactKindId = new Guid("66736361-0D7C-45F1-AAEE-9E0D5CD3FB8E");
    public static readonly Guid TcmHealthMonitorJobId = new Guid("4644AB97-BD97-4F98-A233-5572E977D8B1");
    public static readonly TimeSpan DefaultHealthJobDelay = TimeSpan.FromSeconds(50.0);
    public static readonly int CleanupDaysForDistributedTestRuns = 7;
    public static readonly int FailedTestRun = -1;
    public static readonly string MinimumSliceTimeRegKey = "/Service/TestExecution/Settings/MinimumSliceTime/";
    public static readonly int MinimumSliceDefaultValue = 60000;
    public static readonly string PercentageSliceTimeRegKey = "/Service/TestExecution/Settings/PercentageSliceTime/";
    public static readonly int PercentageSliceTimeDefaultValue = 10;
    public static readonly string QueryResultTrendBatchCountRegKey = "/Service/TestExecution/Settings/PercentageSliceTime/";
    public static readonly int QueryResultTrendBatchCountDefaultValue = 10;
    public static readonly string QueryResultTrendRetryCountRegKey = "/Service/TestExecution/Settings/PercentageSliceTime/";
    public static readonly int QueryResultTrendRetryCountDefaultValue = 3;
    public static readonly string UseTcmServiceFeatureFlag = "TestExecution.UseTcmService";
  }
}
