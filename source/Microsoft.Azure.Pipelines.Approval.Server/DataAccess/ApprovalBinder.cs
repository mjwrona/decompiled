// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalBinder
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalBinder : ObjectBinder<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>
  {
    private SqlColumnBinder m_approvalId = new SqlColumnBinder("ApprovalId");
    private SqlColumnBinder m_instructions = new SqlColumnBinder("Instructions");
    private SqlColumnBinder m_approvalCreatedOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_timeout = new SqlColumnBinder("Timeout");
    private SqlColumnBinder m_minRequiredApprovers = new SqlColumnBinder("MinRequiredApprovers");

    protected override Microsoft.Azure.Pipelines.Approval.WebApi.Approval Bind() => new Microsoft.Azure.Pipelines.Approval.WebApi.Approval()
    {
      Id = this.m_approvalId.GetGuid((IDataReader) this.Reader),
      CreatedOn = this.m_approvalCreatedOn.GetDateTime((IDataReader) this.Reader),
      Instructions = this.m_instructions.GetString((IDataReader) this.Reader, true),
      MinRequiredApprovers = new int?(this.m_minRequiredApprovers.GetInt32((IDataReader) this.Reader))
    };
  }
}
