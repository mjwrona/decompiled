// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TaskCheck.WebApi.TaskCheckConstants
// Assembly: Microsoft.Azure.Pipelines.TaskCheck.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E88E420-FA63-4A56-A903-50B247686E79
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TaskCheck.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Pipelines.TaskCheck.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateSpecificConstants(null)]
  public class TaskCheckConstants
  {
    public const string AreaId = "86E984BB-2B89-439C-81B9-9151996765C0";
    public const string AreaName = "taskCheck";
    [GenerateConstant(null)]
    public const string TaskCheckTypeName = "Task Check";
    public const string TaskCheckTimelineRecordType = "Checkpoint.TaskCheck";
    public const string TaskCheckTypeIdString = "fe1de3ee-a436-41b4-bb20-f6eb4cb879a7";
    [GenerateConstant(null)]
    public static readonly Guid TaskCheckTypeId = new Guid("fe1de3ee-a436-41b4-bb20-f6eb4cb879a7");
    public static readonly Guid ArtifactPolicyCheckTaskDefinitionId = new Guid("1B4FDD02-B979-480D-9A7E-0B7AF7FE471F");
    public static readonly Guid BranchProtectionCheckTaskDefinitionId = new Guid("86b05a0c-73e6-4f7d-b3cf-e38f3b39a75b");
    public static readonly Guid BusinessHoursCheckTaskDefinitionId = new Guid("445FDE2F-6C39-441C-807F-8A59FF2E075F");
    [GenerateConstant(null)]
    public static readonly Guid InvokeRestApiTaskId = new Guid("9C3E8943-130D-4C78-AC63-8AF81DF62DFB");
    [GenerateConstant(null)]
    public static readonly Guid AzureFunctionTaskId = new Guid("537FDB7A-A601-4537-AA70-92645A2B5CE4");
    public static readonly List<Guid> ScalabilityComplienceCheckTasks = new List<Guid>()
    {
      TaskCheckConstants.InvokeRestApiTaskId,
      TaskCheckConstants.AzureFunctionTaskId
    };
    public static readonly List<Guid> UnsupportedBypassTaskChecks = new List<Guid>()
    {
      TaskCheckConstants.BranchProtectionCheckTaskDefinitionId,
      TaskCheckConstants.ArtifactPolicyCheckTaskDefinitionId
    };
    public const string ScalabilityComplianceDeadlineRegistrySetting = "/Service/Orchestration/Settings/Checks/ScalabilityComplianceDeadline";
    public const string DefaultRetryIntervalInMinutesRegistrySetting = "/Service/Orchestration/Settings/Checks/DefaultRetryIntervalInMinutes";
    public const string DefaultTimeoutInMinutesRegistrySetting = "/Service/Orchestration/Settings/Checks/DefaultTimeoutInMinutes";
    [GenerateConstant(null)]
    public const string TaskCheckMaxSyncRetries = "TaskCheck.MaxSyncRetries";
    [GenerateConstant(null)]
    public const string TaskCheckScalabilityComplianceDeadline = "TaskCheck.ScalabilityComplianceDeadline";
    [GenerateConstant(null)]
    public const string TaskCheckDefaultRetryIntervalInMinutes = "TaskCheck.DefaultRetryIntervalInMinutes";
    [GenerateConstant(null)]
    public const string TaskCheckDefaultTimeoutInMinutes = "TaskCheck.DefaultTimeoutInMinutes";
    public const string ScalabilityComplianceDeadlineDefault = "Autumn 2023";
  }
}
