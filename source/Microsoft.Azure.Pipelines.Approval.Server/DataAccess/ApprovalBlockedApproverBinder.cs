// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalBlockedApproverBinder
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal sealed class ApprovalBlockedApproverBinder : ObjectBinder<BlockedApproverType>
  {
    private SqlColumnBinder m_blockedApproverId = new SqlColumnBinder("BlockedApproverId");
    private SqlColumnBinder m_approvalId = new SqlColumnBinder("ApprovalId");

    protected override BlockedApproverType Bind() => new BlockedApproverType()
    {
      BlockedApproverId = this.m_blockedApproverId.GetGuid((IDataReader) this.Reader),
      ApprovalId = this.m_approvalId.GetGuid((IDataReader) this.Reader)
    };
  }
}
