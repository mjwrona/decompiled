// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalStepBinder
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalStepBinder : ObjectBinder<ApprovalStep>
  {
    protected SqlColumnBinder m_stepId = new SqlColumnBinder("StepId");
    protected SqlColumnBinder m_initiatedOn = new SqlColumnBinder("InitiatedOn");
    protected SqlColumnBinder m_lastModifiedBy = new SqlColumnBinder("LastModifiedBy");
    protected SqlColumnBinder m_lastModifiedOn = new SqlColumnBinder("LastModifiedOn");
    protected SqlColumnBinder m_assignedApproverId = new SqlColumnBinder("AssignedApproverId");
    protected SqlColumnBinder m_actualApproverId = new SqlColumnBinder("ActualApproverId");
    protected SqlColumnBinder m_status = new SqlColumnBinder("Status");
    protected SqlColumnBinder m_comment = new SqlColumnBinder("Comment");
    protected SqlColumnBinder m_order = new SqlColumnBinder("Rank");
    protected SqlColumnBinder m_approvalId = new SqlColumnBinder("ApprovalId");

    protected override ApprovalStep Bind()
    {
      ApprovalStep approvalStep = new ApprovalStep()
      {
        StepId = this.m_stepId.GetInt32((IDataReader) this.Reader),
        AssignedApprover = new IdentityRef()
        {
          Id = this.m_assignedApproverId.GetGuid((IDataReader) this.Reader, false).ToString("D")
        },
        Comment = this.m_comment.GetString((IDataReader) this.Reader, true),
        InitiatedOn = this.m_initiatedOn.GetNullableDateTime((IDataReader) this.Reader),
        LastModifiedBy = new IdentityRef()
        {
          Id = this.m_lastModifiedBy.GetGuid((IDataReader) this.Reader, true).ToString("D")
        },
        LastModifiedOn = this.m_lastModifiedOn.GetDateTime((IDataReader) this.Reader),
        Status = (ApprovalStatus) this.m_status.GetByte((IDataReader) this.Reader),
        Order = this.m_order.GetInt32((IDataReader) this.Reader, 1, 1),
        ApprovalId = this.m_approvalId.GetGuid((IDataReader) this.Reader, true)
      };
      this.AdditionalActions(approvalStep);
      Guid? nullableGuid = this.m_actualApproverId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid.HasValue)
        approvalStep.ActualApprover = new IdentityRef()
        {
          Id = nullableGuid.Value.ToString("D")
        };
      return approvalStep;
    }

    protected virtual void AdditionalActions(ApprovalStep approvalStep)
    {
    }
  }
}
