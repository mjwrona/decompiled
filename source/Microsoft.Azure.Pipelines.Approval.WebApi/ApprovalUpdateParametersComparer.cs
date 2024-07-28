// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalUpdateParametersComparer
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  public class ApprovalUpdateParametersComparer : EqualityComparer<ApprovalUpdateParameters>
  {
    public override bool Equals(ApprovalUpdateParameters x, ApprovalUpdateParameters y)
    {
      if (x.ApprovalId == y.ApprovalId)
      {
        int? stepId1 = x.StepId;
        int? stepId2 = y.StepId;
        if (stepId1.GetValueOrDefault() == stepId2.GetValueOrDefault() & stepId1.HasValue == stepId2.HasValue && x.Comment == y.Comment)
        {
          ApprovalStatus? status1 = x.Status;
          ApprovalStatus? status2 = y.Status;
          if (status1.GetValueOrDefault() == status2.GetValueOrDefault() & status1.HasValue == status2.HasValue && this.CompareIdentityRef(x.AssignedApprover, y.AssignedApprover))
            return this.CompareIdentityRef(x.ReassignTo, y.ReassignTo);
        }
      }
      return false;
    }

    public override int GetHashCode(ApprovalUpdateParameters obj)
    {
      if (obj == null)
        return 0;
      Guid approvalId = obj.ApprovalId;
      int hashCode = obj.ApprovalId.GetHashCode();
      int? stepId = obj.StepId;
      int num1;
      if (stepId.HasValue)
      {
        stepId = obj.StepId;
        num1 = stepId.GetHashCode();
      }
      else
        num1 = 0;
      int num2 = num1;
      return hashCode ^ num2;
    }

    private bool CompareIdentityRef(IdentityRef x, IdentityRef y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase);
    }
  }
}
