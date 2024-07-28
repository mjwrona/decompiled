// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent2
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model;
using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalComponent2 : ApprovalComponent
  {
    private static readonly SqlMetaData[] AddApprovalConfigTableType2 = new SqlMetaData[5]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Instructions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("MinRequiredApprovers", SqlDbType.Int),
      new SqlMetaData("ApprovalOrder", SqlDbType.TinyInt),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier)
    };

    public override IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> CreateApprovals(
      Guid projectId,
      IList<CreateApprovalParameter> createApprovalParameters)
    {
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (CreateApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindTable("@approvalConfigs", "PipelinePolicy.typ_ApprovalCreateParametersTable2", createApprovalParameters.Select<CreateApprovalParameter, SqlDataRecord>(new System.Func<CreateApprovalParameter, SqlDataRecord>(this.ConvertToAddApprovalConfigTableSqlDataRecord2)));
        this.BindTable("@assignedApprovers", "PipelinePolicy.typ_AssignedApproverTable", this.GetBindableAssignedApproverTable(createApprovalParameters));
        this.BindTable("@blockedApprovers", "PipelinePolicy.typ_BlockedApproverTable", this.GetBindableBlockedApproverTable(createApprovalParameters));
        return this.FetchApprovals();
      }
    }

    protected SqlDataRecord ConvertToAddApprovalConfigTableSqlDataRecord2(
      CreateApprovalParameter createApprovalParameter)
    {
      SqlDataRecord record = new SqlDataRecord(ApprovalComponent2.AddApprovalConfigTableType2);
      ApprovalConfig config = createApprovalParameter.Config;
      record.SetGuid(0, createApprovalParameter.ApprovalId);
      record.SetNullableString(1, config.Instructions);
      record.SetInt32(2, config.MinRequiredApprovers.Value);
      record.SetByte(3, (byte) config.ExecutionOrder);
      record.SetGuid(4, createApprovalParameter.CreatedBy);
      return record;
    }

    protected override ApprovalBinder GetApprovalBinder() => (ApprovalBinder) new ApprovalBinder2();
  }
}
