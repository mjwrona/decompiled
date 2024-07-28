// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseEnvironmentStepStatusConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseEnvironmentStepStatusConverter
  {
    private static readonly IDictionary<ReleaseEnvironmentStepStatus, ApprovalStatus> StepStatusToApprovalStatusMap = (IDictionary<ReleaseEnvironmentStepStatus, ApprovalStatus>) new Dictionary<ReleaseEnvironmentStepStatus, ApprovalStatus>()
    {
      {
        ReleaseEnvironmentStepStatus.Undefined,
        ApprovalStatus.Undefined
      },
      {
        ReleaseEnvironmentStepStatus.Pending,
        ApprovalStatus.Pending
      },
      {
        ReleaseEnvironmentStepStatus.Reassigned,
        ApprovalStatus.Reassigned
      },
      {
        ReleaseEnvironmentStepStatus.Rejected,
        ApprovalStatus.Rejected
      },
      {
        ReleaseEnvironmentStepStatus.Done,
        ApprovalStatus.Approved
      },
      {
        ReleaseEnvironmentStepStatus.Stopped,
        ApprovalStatus.Undefined
      },
      {
        ReleaseEnvironmentStepStatus.Canceled,
        ApprovalStatus.Canceled
      },
      {
        ReleaseEnvironmentStepStatus.Skipped,
        ApprovalStatus.Skipped
      },
      {
        ReleaseEnvironmentStepStatus.PartiallySucceeded,
        ApprovalStatus.Approved
      },
      {
        ReleaseEnvironmentStepStatus.Abandoned,
        ApprovalStatus.Canceled
      }
    };
    private static readonly IDictionary<ApprovalStatus, ReleaseEnvironmentStepStatus> ApprovalToStepStatusMap = (IDictionary<ApprovalStatus, ReleaseEnvironmentStepStatus>) new Dictionary<ApprovalStatus, ReleaseEnvironmentStepStatus>()
    {
      {
        ApprovalStatus.Undefined,
        ReleaseEnvironmentStepStatus.Undefined
      },
      {
        ApprovalStatus.Pending,
        ReleaseEnvironmentStepStatus.Pending
      },
      {
        ApprovalStatus.Reassigned,
        ReleaseEnvironmentStepStatus.Reassigned
      },
      {
        ApprovalStatus.Rejected,
        ReleaseEnvironmentStepStatus.Rejected
      },
      {
        ApprovalStatus.Approved,
        ReleaseEnvironmentStepStatus.Done
      },
      {
        ApprovalStatus.Canceled,
        ReleaseEnvironmentStepStatus.Canceled
      },
      {
        ApprovalStatus.Skipped,
        ReleaseEnvironmentStepStatus.Skipped
      }
    };

    public static ApprovalStatus ToWebApi(this ReleaseEnvironmentStepStatus status) => ReleaseEnvironmentStepStatusConverter.StepStatusToApprovalStatusMap[status];

    public static ReleaseEnvironmentStepStatus FromWebApi(this ApprovalStatus status) => ReleaseEnvironmentStepStatusConverter.ApprovalToStepStatusMap[status];
  }
}
