// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.GreenlightingConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class GreenlightingConstants
  {
    public const int DefaultSamplingIntervalInMinutes = 15;
    public const int MinimumSamplingIntervalInMinutes = 5;
    public const int DefaultStabilizationTimeInMinutes = 0;
    public const int DefaultMinimumSuccessWindow = 0;
    public const int DefaultJobTimeoutInMinutes = 2880;
    public const int DefaultTopRecordCountForGate = 5;
    public const int MaxTopRecordCountForGate = 10;
    public const string InitialAttemptNumberKey = "InitialAttemptNumber";
    public const string InitialConsecutiveSuccessSamplesKey = "InitialConsecutiveSuccessSamples";
    public const int MaxSamplesPerOrchestration = 200;
    public const string OrchestrationStartTimeKey = "OrchestrationStartTime";
    public const string IgnoredGates = "IgnoredGates";
  }
}
