// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent
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
  internal class ApprovalComponent : ApprovalSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<ApprovalComponent>(1),
      (IComponentCreator) new ComponentCreator<ApprovalComponent2>(2),
      (IComponentCreator) new ComponentCreator<ApprovalComponent3>(3),
      (IComponentCreator) new ComponentCreator<ApprovalComponent4>(4),
      (IComponentCreator) new ComponentCreator<ApprovalComponent5>(5),
      (IComponentCreator) new ComponentCreator<ApprovalComponent6>(6),
      (IComponentCreator) new ComponentCreator<ApprovalComponent7>(7)
    }, "Approval", "PipelinePolicy");
    private static readonly SqlMetaData[] AddApprovalConfigTableType = new SqlMetaData[7]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Instructions", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("Timeout", SqlDbType.Int),
      new SqlMetaData("TimeoutJobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("MinRequiredApprovers", SqlDbType.Int),
      new SqlMetaData("ApprovalOrder", SqlDbType.TinyInt),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] AssignedApproverConfigTableType = new SqlMetaData[3]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignedApprover", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ApproverOrderId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] BlockedApproverConfigTableType = new SqlMetaData[2]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BlockedApprover", SqlDbType.UniqueIdentifier)
    };
    protected static readonly SqlMetaData[] ApprovalConfigTableType = new SqlMetaData[4]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StepId", SqlDbType.Int),
      new SqlMetaData("Comment", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("Status", SqlDbType.TinyInt)
    };
    public const int c_DefaultApprovalTimeout = 43200;

    public ApprovalComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> CreateApprovals(
      Guid projectId,
      IList<CreateApprovalParameter> createApprovalParameters)
    {
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (CreateApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindTable("@approvalConfigs", "PipelinePolicy.typ_ApprovalCreateParametersTable", createApprovalParameters.Select<CreateApprovalParameter, SqlDataRecord>(new System.Func<CreateApprovalParameter, SqlDataRecord>(this.ConvertToAddApprovalConfigTableSqlDataRecord)));
        this.BindTable("@assignedApprovers", "PipelinePolicy.typ_AssignedApproverTable", this.GetBindableAssignedApproverTable(createApprovalParameters));
        this.BindTable("@blockedApprovers", "PipelinePolicy.typ_BlockedApproverTable", this.GetBindableBlockedApproverTable(createApprovalParameters));
        return this.FetchApprovals();
      }
    }

    public virtual Microsoft.Azure.Pipelines.Approval.WebApi.Approval GetApproval(
      Guid projectId,
      Guid approvalId)
    {
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (GetApproval)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetApproval");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@approvalId", approvalId);
        return this.FetchApprovals().FirstOrDefault<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>();
      }
    }

    public virtual IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovals(
      Guid projectId,
      IList<Guid> approvalIds)
    {
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (QueryApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_QueryApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuidTable("@approvalIds", (IEnumerable<Guid>) approvalIds);
        return this.FetchApprovals();
      }
    }

    public virtual IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovals(
      Guid projectId,
      IList<Guid> approvalIds,
      IList<Guid> approverIds,
      ApprovalStatus approvalStatus,
      int rowCount)
    {
      return (IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) Enumerable.Empty<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>().ToList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>();
    }

    public virtual IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> UpdateApprovals(
      Guid projectId,
      Guid approverId,
      IEnumerable<ApprovalUpdateParameters> approvalUpdateParameters)
    {
      IEnumerable<ApprovalUpdateParameters> source = approvalUpdateParameters.Where<ApprovalUpdateParameters>((System.Func<ApprovalUpdateParameters, bool>) (x => x.ReassignTo == null || x.ReassignTo.Id == null));
      if (source.Count<ApprovalUpdateParameters>() <= 0)
        return (IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) new List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>();
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (UpdateApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_UpdateApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@approverId", approverId);
        this.BindTable("@updateParameters", "PipelinePolicy.typ_ApprovalUpdateParametersTable", source.Select<ApprovalUpdateParameters, SqlDataRecord>(new System.Func<ApprovalUpdateParameters, SqlDataRecord>(this.ConvertToApprovalConfigTableSqlDataRecord)));
        return this.FetchApprovals();
      }
    }

    public virtual void DeleteApprovals(Guid projectId, IList<Guid> approvalIds)
    {
    }

    protected SqlDataRecord ConvertToAddApprovalConfigTableSqlDataRecord(
      CreateApprovalParameter createApprovalParameter)
    {
      SqlDataRecord record = new SqlDataRecord(ApprovalComponent.AddApprovalConfigTableType);
      ApprovalConfig config = createApprovalParameter.Config;
      record.SetGuid(0, createApprovalParameter.ApprovalId);
      record.SetNullableString(1, config.Instructions);
      record.SetInt32(2, 43200);
      record.SetGuid(3, createApprovalParameter.TimeoutJobId);
      record.SetInt32(4, config.MinRequiredApprovers.Value);
      record.SetByte(5, (byte) config.ExecutionOrder);
      record.SetGuid(6, createApprovalParameter.CreatedBy);
      return record;
    }

    protected SqlDataRecord ConvertToAssignedApproverConfigTableSqlDataRecord(
      Guid approvalId,
      Guid assignedApprover,
      int approverOrderId)
    {
      SqlDataRecord tableSqlDataRecord = new SqlDataRecord(ApprovalComponent.AssignedApproverConfigTableType);
      tableSqlDataRecord.SetGuid(0, approvalId);
      tableSqlDataRecord.SetGuid(1, assignedApprover);
      tableSqlDataRecord.SetInt32(2, approverOrderId);
      return tableSqlDataRecord;
    }

    protected SqlDataRecord ConvertToBlockedApproverConfigTableSqlDataRecord(
      Guid approvalId,
      Guid blockedApprover)
    {
      SqlDataRecord tableSqlDataRecord = new SqlDataRecord(ApprovalComponent.BlockedApproverConfigTableType);
      tableSqlDataRecord.SetGuid(0, approvalId);
      tableSqlDataRecord.SetGuid(1, blockedApprover);
      return tableSqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToApprovalConfigTableSqlDataRecord(
      ApprovalUpdateParameters row)
    {
      SqlDataRecord record = new SqlDataRecord(ApprovalComponent.ApprovalConfigTableType);
      record.SetGuid(0, row.ApprovalId);
      record.SetNullableInt32(1, row.StepId);
      record.SetNullableString(2, row.Comment);
      record.SetByte(3, (byte) row.Status.Value);
      return record;
    }

    protected virtual IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> FetchApprovals()
    {
      IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> items1;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>((ObjectBinder<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) this.GetApprovalBinder());
        resultCollection.AddBinder<ApprovalStep>((ObjectBinder<ApprovalStep>) this.GetApprovalStepBinder());
        resultCollection.AddBinder<BlockedApproverType>((ObjectBinder<BlockedApproverType>) new ApprovalBlockedApproverBinder());
        items1 = (IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>) resultCollection.GetCurrent<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>().Items;
        resultCollection.NextResult();
        List<ApprovalStep> items2 = resultCollection.GetCurrent<ApprovalStep>().Items;
        resultCollection.NextResult();
        List<BlockedApproverType> items3 = resultCollection.GetCurrent<BlockedApproverType>().Items;
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

    protected virtual ApprovalBinder GetApprovalBinder() => new ApprovalBinder();

    protected virtual ApprovalStepBinder GetApprovalStepBinder() => new ApprovalStepBinder();

    protected virtual IEnumerable<SqlDataRecord> GetBindableAssignedApproverTable(
      IList<CreateApprovalParameter> createApprovalParameters)
    {
      IList<SqlDataRecord> assignedApproverTable = (IList<SqlDataRecord>) new List<SqlDataRecord>();
      foreach (CreateApprovalParameter approvalParameter in (IEnumerable<CreateApprovalParameter>) createApprovalParameters)
      {
        for (int index = 0; index < approvalParameter.Config.Approvers.Count; ++index)
        {
          IdentityRef identityRef = approvalParameter.Config.Approvers.ElementAt<IdentityRef>(index);
          assignedApproverTable.Add(this.ConvertToAssignedApproverConfigTableSqlDataRecord(approvalParameter.ApprovalId, new Guid(identityRef.Id), index + 1));
        }
      }
      return (IEnumerable<SqlDataRecord>) assignedApproverTable;
    }

    protected virtual IEnumerable<SqlDataRecord> GetBindableBlockedApproverTable(
      IList<CreateApprovalParameter> createApprovalParameters)
    {
      IList<SqlDataRecord> blockedApproverTable = (IList<SqlDataRecord>) new List<SqlDataRecord>();
      foreach (CreateApprovalParameter approvalParameter in (IEnumerable<CreateApprovalParameter>) createApprovalParameters)
      {
        foreach (IdentityRef blockedApprover in approvalParameter.Config.BlockedApprovers)
          blockedApproverTable.Add(this.ConvertToBlockedApproverConfigTableSqlDataRecord(approvalParameter.ApprovalId, new Guid(blockedApprover.Id)));
      }
      return (IEnumerable<SqlDataRecord>) blockedApproverTable;
    }
  }
}
