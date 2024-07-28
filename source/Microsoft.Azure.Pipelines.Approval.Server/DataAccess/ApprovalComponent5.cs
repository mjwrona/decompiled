// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent5
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model;
using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalComponent5 : ApprovalComponent4
  {
    private static readonly SqlMetaData[] AddApprovalConfigTableType3 = new SqlMetaData[7]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Instructions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("MinRequiredApprovers", SqlDbType.Int),
      new SqlMetaData("ApprovalOrder", SqlDbType.TinyInt),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Owner", SqlDbType.TinyInt),
      new SqlMetaData("Context", SqlDbType.NVarChar, 4000L)
    };

    public override IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> CreateApprovals(
      Guid projectId,
      IList<CreateApprovalParameter> createApprovalParameters)
    {
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (CreateApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindTable("@approvalConfigs", "PipelinePolicy.typ_ApprovalCreateParametersTable3", createApprovalParameters.Select<CreateApprovalParameter, SqlDataRecord>(new System.Func<CreateApprovalParameter, SqlDataRecord>(this.ConvertToAddApprovalConfigTableSqlDataRecord3)));
        this.BindTable("@assignedApprovers", "PipelinePolicy.typ_AssignedApproverTable", this.GetBindableAssignedApproverTable(createApprovalParameters));
        this.BindTable("@blockedApprovers", "PipelinePolicy.typ_BlockedApproverTable", this.GetBindableBlockedApproverTable(createApprovalParameters));
        return this.FetchApprovals();
      }
    }

    protected SqlDataRecord ConvertToAddApprovalConfigTableSqlDataRecord3(
      CreateApprovalParameter createApprovalParameter)
    {
      SqlDataRecord record = new SqlDataRecord(ApprovalComponent5.AddApprovalConfigTableType3);
      ApprovalConfig config = createApprovalParameter.Config;
      JObject context = createApprovalParameter.Context;
      string str1 = context == null ? (string) null : JsonConvert.SerializeObject((object) context);
      int ordinal1 = 0;
      record.SetGuid(ordinal1, createApprovalParameter.ApprovalId);
      int ordinal2;
      int num1 = ordinal2 = ordinal1 + 1;
      string instructions = config.Instructions;
      record.SetNullableString(ordinal2, instructions);
      int ordinal3;
      int num2 = ordinal3 = num1 + 1;
      int num3 = config.MinRequiredApprovers.Value;
      record.SetInt32(ordinal3, num3);
      int ordinal4;
      int num4 = ordinal4 = num2 + 1;
      int executionOrder = (int) (byte) config.ExecutionOrder;
      record.SetByte(ordinal4, (byte) executionOrder);
      int ordinal5;
      int num5 = ordinal5 = num4 + 1;
      Guid createdBy = createApprovalParameter.CreatedBy;
      record.SetGuid(ordinal5, createdBy);
      int ordinal6;
      int num6 = ordinal6 = num5 + 1;
      int owner = (int) (byte) createApprovalParameter.Owner;
      record.SetByte(ordinal6, (byte) owner);
      int ordinal7;
      int num7 = ordinal7 = num6 + 1;
      string str2 = str1;
      record.SetNullableString(ordinal7, str2);
      return record;
    }

    protected override ApprovalBinder GetApprovalBinder() => (ApprovalBinder) new ApprovalBinder3();
  }
}
