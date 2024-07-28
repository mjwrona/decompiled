// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.TraceConstants
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal static class TraceConstants
  {
    public const string Area = "Pipeline.Checks";
    public const string ServiceLayer = "Service";
    public const string CheckConfigurationService = "CheckConfigurationService";
    public const string CheckSuiteService = "CheckSuiteService";
    public const int Start = 34001900;
    public const int DefaultTracePoint = 34001900;
    public const int BatchEvaluationUpdatedEventPublishInfo = 34001901;
    public const int BatchEvaluationUpdatedEventPublishError = 34001902;
    public const int InvalidCheckConfiguration = 34001903;
    public const int CheckConfigurationNotFound = 34001904;
    public const int ResourceUnauthorized = 34001905;
    public const int CheckConfigurationInfo = 34001906;
    public const int CheckSuiteIsAlreadyCompleted = 34001907;
    public const int CheckServiceTelemetryException = 34001908;
    public const int CheckSuiteUpdateException = 34001909;
    public const int CheckSuiteError = 34001910;
    public const int NoConfiguredCheck = 34001911;
    public const int ConfiguredChecks = 34001912;
    public const int CheckEvaluation = 34001913;
    public const int PluginCancellationError = 34001914;
    public const int CheckConfigurationUnauthorized = 34001915;
    public const int CheckConfigurationAdded = 34001916;
    public const int CheckConfigurationUpdated = 34001917;
    public const int CheckConfigurationDeleted = 34001918;
    public const int CheckConfiguration = 34001919;
    public const int CheckEvaluationCreated = 34001920;
    public const int CheckEvaluationUpdated = 34001921;
    public const int CheckEvaluationCanceled = 34001922;
    public const int CheckEvaluationFailed = 34001923;
    public const int CheckSuiteNotFound = 34001924;
    public const int CheckTimeout = 34001925;
    public const int CancelPendingJobs = 34001926;
    public const int CheckRunNotFound = 34001927;
    public const int PluginCheckRunDeletionError = 34001928;
    public const int DeleteCheckSuitesEventProcessingInfo = 34001929;
    public const int ErrorInAddingCheckSuite = 34001930;
    public const int CheckSuiteIsAlreadyCompletedInfo = 34001931;
    public const int UnexpectedCheckRunsCount = 34001932;
    public const int CheckAuditException = 34001933;
    public const int CheckRunsEvaluation = 34001934;
    public const int NonCompliantCheckConfigurations = 34001935;
    public const int ChecksCleanupJob = 34001937;
    public const int CheckSuiteRerunRequest = 34001938;
    public const int CheckSuiteBypassResultOverride = 34001939;
  }
}
