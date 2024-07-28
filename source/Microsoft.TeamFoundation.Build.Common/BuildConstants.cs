// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildConstants
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [GenerateSpecificConstants(null)]
  public static class BuildConstants
  {
    public static readonly int MaxPathLength = 259;
    public static readonly int MaxPathNameLength = 248;
    public static readonly int MaxUriLength = 2048;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly int WebRequestRetryCount = 5;
    [GenerateConstant(null)]
    public static readonly string BuildOptionAdditionalFieldsName = "additionalFields";
    private const string serviceApplicationName = "TFSBuildServiceHost";
    private const string windowsServiceName = "TFSBuildServiceHost.2013";
    private const string processName = "TFSBuildServiceHost.exe";
    private const string eventSourceName = "TFSBuildServiceHost.2013";
    private const string buildService = "BuildService";
    private const string buildService3 = "BuildService3";
    private const string buildTypeFolderName = "TeamBuildTypes";
    private const string projectFileName = "TFSBuild.proj";
    private const string logFileName = "BuildLog.txt";
    private const string customEventsFileName = "CustomEvents.xml";
    private static string star = "*";
    private static string[] allInformationTypes = new string[1]
    {
      BuildConstants.star
    };
    private static string[] allPropertyNames = new string[1]
    {
      BuildConstants.star
    };
    private static string[] noPropertyNames = new string[0];
    private static string serviceHostUrlPath = "Build/v5.0/Services";
    private const int currentVersion = 3;
    private static string doNotAutoDeleteProperty = "DoNotAutoDelete";
    private const string firewallExceptionFormat = "Microsoft Visual Studio Team Foundation Build Services 2015 ({0})";
    private const string argBuildUri = "BuildUri";
    private const string argTFSUrl = "TFSUrl";
    private const string argTFSProjectFile = "TFSProjectFile";
    private const string argLogFilePerProject = "LogFilePerProject";
    private const string argDropLocation = "DropLocation";
    private const string argBuildNumber = "BuildNumber";
    private const string argInformationNodeId = "InformationNodeId";
    [Obsolete("moved to Microsoft.TeamFoundation.Build2.Server.OrchestrationConstants")]
    private const string orchestrationHubName = "Build";
    private const int defaultDaysToKeepDefinitionMetrics = 8;
    private const int defaultDaysToKeepDailyProjectMetrics = 30;
    private const int defaultDaysToKeepHourlyProjectMetrics = 3;
    private const int defaultDaysToKeepFiredEvents = 8;
    private const int defaultDaysToKeepFailedEvents = 16;
    private const int defaultBuildEventsJobBatchSize = 100;
    private const int defaultBuildCompletionEventListenerJobTimeoutMinutes = 20;
    private const int defaultServiceConnectionHistoryRegistrationJobTimeoutMinutes = 20;
    private const int defaultDaysToKeepFiredCheckEvents = 2;
    private const int defaultDaysToKeepFailedCheckEvents = 14;
    private const int defaultCheckEventsMaxAttempts = 5;
    private const int defaultCheckEventsBatchSizeExecute = 100;
    private const int defaultCheckEventsMaxDegreeOfParallelism = 5;
    public static readonly string WorkflowIntegrationServiceConfigurationKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.Configuration";
    public static readonly string TeamFoundationServerUrlKey = "TFSServerURL";
    public static readonly string BuildServiceHostUriKey = "BuildServiceHostUri";
    public static readonly string NetworkIsolated = nameof (NetworkIsolated);
    public static readonly string SequenceIdKey = "SequenceId";
    public static readonly string RetryIdKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.RetryId";
    public static readonly char WorkflowIntegrationConfigurationValueSeparator = '|';
    public static readonly string WorkflowIntegrationConfigurationValueFormat = "{0}={1}{2}";
    public static readonly string CredentialsKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.Credentials";
    public static readonly string DomainNameToken = "DomainName";
    public static readonly string UserNameToken = "UserName";
    public static readonly string PasswordToken = "Password";
    public static readonly string DisclaimerToken = "Disclaimer";
    public static readonly string ServiceHostUriKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.BuildServiceHostUri";
    public static readonly string AgentUriKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.BuildAgentUri";
    public static readonly string BuildServiceHostDiagnosticInfoKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.BuildServiceHostError";
    public static readonly string BuildServiceHostStatusKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.BuildServiceHostStatus";
    public static readonly string BuildAgentDiagnosticInfoKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.BuildAgentError";
    public static readonly string ServiceContractVersionKey = "ServiceContractVersion";
    public static readonly string ServiceVersionKey = "ServiceVersion";
    public static readonly string AgentContractVersionKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.AgentContractVersion";
    public static readonly string AgentVersionKey = "Microsoft.TeamFoundation.Lab.WorkflowIntegration.AgentVersion";
    public const string FenceAgentSubkey = "Software\\Microsoft\\LabManagement\\12.0\\NetworkAgent";
    public const string FenceAgentConfigurationStatusValue = "Finish";
    public const string FenceAgentInstructionValue = "FenceAgent";
    public const int MaxAgentKeyCount = 20;

    public static string ProjectFileName => "TFSBuild.proj";

    public static string BuildTypeFolderName => "TeamBuildTypes";

    [Obsolete("This property has been deprecated. Please remove all references and use WindowsServiceName, ProcessName or EventSourceName instead.")]
    public static string ServiceApplicationName => "TFSBuildServiceHost";

    public static string WindowsServiceName => "TFSBuildServiceHost.2013";

    public static string ProcessName => "TFSBuildServiceHost.exe";

    public static string EventSourceName => "TFSBuildServiceHost.2013";

    public static string BuildService => nameof (BuildService);

    public static string BuildService3 => nameof (BuildService3);

    public static string BuildLogFileName => "BuildLog.txt";

    public static string CustomEventsFileName => "CustomEvents.xml";

    public static string DefaultServiceHostUrlPath => BuildConstants.serviceHostUrlPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string Star => BuildConstants.star;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string FirewallExceptionFormat => "Microsoft Visual Studio Team Foundation Build Services 2015 ({0})";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string[] AllInformationTypes => BuildConstants.allInformationTypes;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string[] AllPropertyNames => BuildConstants.allPropertyNames;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string[] NoPropertyNames => BuildConstants.noPropertyNames;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int CurrentVersion => 3;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string DoNotAutoDeleteProperty => BuildConstants.doNotAutoDeleteProperty;

    public static string BuildUriArgument => "BuildUri";

    public static string TfsUrlArgument => "TFSUrl";

    public static string TfsProjectFileArgument => "TFSProjectFile";

    public static string LogFilePerProjectArgument => "LogFilePerProject";

    public static string DropLocationArgument => "DropLocation";

    public static string BuildNumberArgument => "BuildNumber";

    public static string InformationNodeIdArgument => "InformationNodeId";

    [Obsolete("moved to Microsoft.TeamFoundation.Build2.Server.OrchestrationConstants")]
    public static string OrchestrationHubName => "Build";

    public static string Disabled => nameof (Disabled);

    public static int DefaultDaysToKeepDefinitionMetrics => 8;

    public static int DefaultDaysToKeepDailyProjectMetrics => 30;

    public static int DefaultDaysToKeepHourlyProjectMetrics => 3;

    public static int DefaultDaysToKeepFiredEvents => 8;

    public static int DefaultDaysToKeepFailedEvents => 16;

    public static int DefaultBuildEventsJobBatchSize => 100;

    public static int DefaultBuildCompletionEventListenerJobTimeoutMinutes => 20;

    public static int DefaultServiceConnectionHistoryRegistrationJobTimeoutMinutes => 20;

    public static int DefaultDaysToKeepFiredCheckEvents => 2;

    public static int DefaultDaysToKeepFailedCheckEvents => 14;

    public static int DefaultCheckEventsMaxAttempts => 5;

    public static int DefaultCheckEventsBatchSizeExecute => 100;

    public static int DefaultCheckEventsMaxDegreeOfParallelism => 5;

    [Obsolete("This enum has been deprecated. Please remove all references.")]
    public enum BuildStatusIconID
    {
      Default = 0,
      BuildSucceeded = 100, // 0x00000064
      BuildFailed = 200, // 0x000000C8
      BuildStopped = 300, // 0x0000012C
    }

    [Obsolete("This class has been deprecated. Please remove all references.")]
    public static class BuildStatus
    {
      private static string buildInitializing = BuildTypeResource.BuildInitializingState();
      private static string sync = BuildTypeResource.SyncState();
      private static string syncCompleted = BuildTypeResource.SyncCompletedState();
      private static string startedCompilation = BuildTypeResource.StartedCompilationState();
      private static string finishedCompilation = BuildTypeResource.FinishedCompilationState();
      private static string startedTesting = BuildTypeResource.StartedTestingState();
      private static string testCompleted = BuildTypeResource.TestCompletedState();
      private static string buildSucceeded = BuildTypeResource.Status_V1_Succeeded();
      private static string buildFailed = BuildTypeResource.Status_Failed();
      private static string buildAborted = BuildTypeResource.Status_Stopped();

      public static string BuildInitializing => BuildConstants.BuildStatus.buildInitializing;

      public static string Sync => BuildConstants.BuildStatus.sync;

      public static string SyncCompleted => BuildConstants.BuildStatus.syncCompleted;

      public static string StartedCompilation => BuildConstants.BuildStatus.startedCompilation;

      public static string FinishedCompilation => BuildConstants.BuildStatus.finishedCompilation;

      public static string StartedTesting => BuildConstants.BuildStatus.startedTesting;

      public static string TestCompleted => BuildConstants.BuildStatus.testCompleted;

      public static string BuildSucceeded => BuildConstants.BuildStatus.buildSucceeded;

      public static string BuildFailed => BuildConstants.BuildStatus.buildFailed;

      public static string BuildStopped => BuildConstants.BuildStatus.buildAborted;
    }
  }
}
