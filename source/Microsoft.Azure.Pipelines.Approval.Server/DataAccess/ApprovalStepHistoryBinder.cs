// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalStepHistoryBinder
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal sealed class ApprovalStepHistoryBinder : ObjectBinder<ApprovalStepHistory>
  {
    private SqlColumnBinder m_approvalId = new SqlColumnBinder("ApprovalId");
    private SqlColumnBinder m_stepId = new SqlColumnBinder("ApprovalStepId");
    private SqlColumnBinder m_CreatedBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_CreatedOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_assignedApproverId = new SqlColumnBinder("AssignedApproverId");
    private SqlColumnBinder m_comment = new SqlColumnBinder("Comment");

    protected override ApprovalStepHistory Bind() => new ApprovalStepHistory()
    {
      ApprovalId = this.m_approvalId.GetGuid((IDataReader) this.Reader, true),
      StepId = this.m_stepId.GetInt32((IDataReader) this.Reader),
      CreatedBy = new IdentityRef()
      {
        Id = this.m_CreatedBy.GetGuid((IDataReader) this.Reader, true).ToString("D")
      },
      CreatedOn = this.m_CreatedOn.GetDateTime((IDataReader) this.Reader),
      AssignedTo = new IdentityRef()
      {
        Id = this.m_assignedApproverId.GetGuid((IDataReader) this.Reader, false).ToString("D")
      },
      Comment = this.m_comment.GetString((IDataReader) this.Reader, true)
    };
  }
}
