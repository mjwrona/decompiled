// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent3
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.Server.DataAccess.Model;
using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalComponent3 : ApprovalComponent2
  {
    private static readonly SqlMetaData[] ApprovalReassignTableType = new SqlMetaData[5]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StepId", SqlDbType.Int),
      new SqlMetaData("Comment", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("AssignedApproverId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ReassignedApproverId", SqlDbType.UniqueIdentifier)
    };

    public override IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> UpdateApprovals(
      Guid projectId,
      Guid currentUserId,
      IEnumerable<ApprovalUpdateParameters> approvalUpdateParameters)
    {
      IEnumerable<ApprovalUpdateParameters> updateParameters = approvalUpdateParameters.Where<ApprovalUpdateParameters>((System.Func<ApprovalUpdateParameters, bool>) (x => x.ReassignTo == null || x.ReassignTo.Id == null));
      IEnumerable<ApprovalUpdateParameters> source = approvalUpdateParameters.Where<ApprovalUpdateParameters>((System.Func<ApprovalUpdateParameters, bool>) (x => x.ReassignTo != null && x.ReassignTo.Id != null));
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (UpdateApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_UpdateApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@currentUserId", currentUserId);
        this.BindApprovalUpdateParametersTable(updateParameters);
        this.BindTable("@reassignParameters", "PipelinePolicy.typ_ApprovalReassignParametersTable", source.Select<ApprovalUpdateParameters, SqlDataRecord>(new System.Func<ApprovalUpdateParameters, SqlDataRecord>(this.ConvertToApprovalReassignTableSqlDataRecord)));
        return this.FetchApprovals();
      }
    }

    protected virtual void BindApprovalUpdateParametersTable(
      IEnumerable<ApprovalUpdateParameters> updateParameters)
    {
      this.BindTable("@updateParameters", "PipelinePolicy.typ_ApprovalUpdateParametersTable", updateParameters.Select<ApprovalUpdateParameters, SqlDataRecord>(new System.Func<ApprovalUpdateParameters, SqlDataRecord>(((ApprovalComponent) this).ConvertToApprovalConfigTableSqlDataRecord)));
    }

    protected SqlDataRecord ConvertToApprovalReassignTableSqlDataRecord(ApprovalUpdateParameters row)
    {
      SqlDataRecord record = new SqlDataRecord(ApprovalComponent3.ApprovalReassignTableType);
      int ordinal1 = 0;
      int num1 = ordinal1 + 1;
      record.SetGuid(ordinal1, row.ApprovalId);
      int ordinal2 = num1;
      int num2 = ordinal2 + 1;
      record.SetInt32(ordinal2, row.StepId.Value);
      int ordinal3 = num2;
      int num3 = ordinal3 + 1;
      record.SetNullableString(ordinal3, row.Comment);
      int ordinal4 = num3;
      int num4 = ordinal4 + 1;
      record.SetGuid(ordinal4, Guid.Parse(row.AssignedApprover.Id));
      int ordinal5 = num4;
      int num5 = ordinal5 + 1;
      record.SetGuid(ordinal5, Guid.Parse(row.ReassignTo.Id));
      return record;
    }

    protected override IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> FetchApprovals()
    {
      IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> items1;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((ObjectBinder<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) this.GetApprovalBinder());
        resultCollection.AddBinder<ApprovalStep>((ObjectBinder<ApprovalStep>) this.GetApprovalStepBinder());
        resultCollection.AddBinder<BlockedApproverType>((ObjectBinder<BlockedApproverType>) new ApprovalBlockedApproverBinder());
        resultCollection.AddBinder<ApprovalStepHistory>((ObjectBinder<ApprovalStepHistory>) new ApprovalStepHistoryBinder());
        items1 = (IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) resultCollection.GetCurrent<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>().Items;
        resultCollection.NextResult();
        List<ApprovalStep> items2 = resultCollection.GetCurrent<ApprovalStep>().Items;
        resultCollection.NextResult();
        List<BlockedApproverType> items3 = resultCollection.GetCurrent<BlockedApproverType>().Items;
        resultCollection.NextResult();
        List<ApprovalStepHistory> items4 = resultCollection.GetCurrent<ApprovalStepHistory>().Items;
        foreach (ApprovalStep approvalStep in items2)
        {
          ApprovalStep step = approvalStep;
          step.History = (IList<ApprovalStepHistory>) items4.Where<ApprovalStepHistory>((System.Func<ApprovalStepHistory, bool>) (stepHistory => stepHistory.ApprovalId == step.ApprovalId && stepHistory.StepId == step.StepId)).OrderBy<ApprovalStepHistory, DateTime>((System.Func<ApprovalStepHistory, DateTime>) (stepHistory => stepHistory.CreatedOn)).ToList<ApprovalStepHistory>();
        }
        foreach (Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval1 in (IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) items1)
        {
          Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval = approval1;
          approval.Steps = items2.Where<ApprovalStep>((System.Func<ApprovalStep, bool>) (step => step.ApprovalId == approval.Id)).ToList<ApprovalStep>();
          approval.BlockedApprovers = items3.Where<BlockedApproverType>((System.Func<BlockedApproverType, bool>) (blockedApprover => blockedApprover.ApprovalId == approval.Id)).Select<BlockedApproverType, IdentityRef>((System.Func<BlockedApproverType, IdentityRef>) (blockedApprover => new IdentityRef()
          {
            Id = blockedApprover.BlockedApproverId.ToString("D")
          })).ToList<IdentityRef>();
        }
      }
      return items1;
    }
  }
}
