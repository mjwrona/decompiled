// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.RunDeploymentPlanInputConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class RunDeploymentPlanInputConstants
  {
    public const string HealthPercent = "HealthPercent";
    public const string DeploymentHealthOption = "DeploymentHealthOption";
    public const string MachineGroupId = "MachineGroupId";
    public const string ProjectId = "ProjectId";
    public const string Tags = "Tags";
    public const string DeploymentTargetIds = "DeploymentTargetIds";
    public const string MaxAttemptCount = "MaxAttemptCount";
    public const int DefaultMaxAttemptCount = 5;
  }
}
