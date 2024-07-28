// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteComponent
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteComponent : ChecksSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent>(1),
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent2>(2),
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent3>(3),
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent4>(4),
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent5>(5),
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent6>(6),
      (IComponentCreator) new ComponentCreator<CheckSuiteComponent7>(7)
    }, "PolicyEvaluation", "PipelinePolicy");
    private static readonly SqlMetaData[] CheckSuiteUpdateParametersTableType = new SqlMetaData[3]
    {
      new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L)
    };
    private static readonly SqlMetaData[] CheckSuiteCreateParametersTableType = new SqlMetaData[4]
    {
      new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L)
    };

    public CheckSuiteComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual CheckSuite AddCheckSuite(
      Guid projectId,
      Guid checkSuiteId,
      List<CheckRun> checkRuns,
      List<Resource> resources,
      JObject evaluationContext)
    {
      if (checkRuns == null || !checkRuns.Any<CheckRun>())
        return CheckSuiteResult.GetCheckSuiteResponse(checkSuiteId, (List<CheckRun>) null, (CheckSuiteContext) null);
      string parameterValue = evaluationContext != null ? evaluationContext.ToString() : string.Empty;
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckSuite)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddPolicyBatchEvaluation");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@batchRequestId", checkSuiteId);
        this.BindString("@context", parameterValue, 4000, false, SqlDbType.NVarChar);
        this.BindTable("@createParameters", "PipelinePolicy.typ_PolicyEvaluationCreateParametersTable", checkRuns.Select<CheckRun, SqlDataRecord>(new System.Func<CheckRun, SqlDataRecord>(this.ConvertToCheckSuiteCreateTableSqlDataRecord)));
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    public virtual CheckSuite GetCheckSuite(Guid projectId, Guid checkSuiteId)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckSuite)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyBatchEvaluation");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@batchRequestId", checkSuiteId);
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    public virtual List<CheckSuite> GetCheckSuitesByIds(Guid projectId, List<Guid> checkSuiteIds) => new List<CheckSuite>();

    public virtual IList<CheckRun> GetCheckRuns(Guid projectId, CheckRunFilter checkRunFilter) => (IList<CheckRun>) new List<CheckRun>();

    public virtual CheckSuite AddCheckRuns(
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns)
    {
      return new CheckSuite();
    }

    public virtual IList<CheckSuite> DeleteCheckSuites(Guid projectId, IList<Guid> checkSuiteIds) => (IList<CheckSuite>) new List<CheckSuite>();

    public virtual List<CheckSuite> GetCheckSuitesByRequestIds(
      Guid projectId,
      List<Guid> requestIds)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckSuitesByRequestIds)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyBatchEvaluationByRequestIds");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuidTable("@requestIds", (IEnumerable<Guid>) requestIds);
        return this.GetCheckSuiteResponses();
      }
    }

    public virtual List<CheckSuite> UpdateCheckSuite(
      Guid projectId,
      IReadOnlyDictionary<Guid, CheckRunResult> checkSuiteUpdateParameters,
      Dictionary<string, object> auditData)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (UpdateCheckSuite)))
      {
        if (auditData != null && auditData.Count != 0)
          this.PrepareForAuditingAction("CheckSuite.Completed", auditData, projectId, excludeSqlParameters: true);
        this.PrepareStoredProcedure("PipelinePolicy.prc_UpdatePolicyBatchEvaluation");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindTable("@updateParameters", "PipelinePolicy.typ_PolicyEvaluationUpdateParametersTable", checkSuiteUpdateParameters.Select<KeyValuePair<Guid, CheckRunResult>, SqlDataRecord>((System.Func<KeyValuePair<Guid, CheckRunResult>, SqlDataRecord>) (parameter => this.ConvertToCheckSuiteUpdateTableSqlDataRecord(parameter.Key, parameter.Value))));
        return this.GetCheckSuiteResponses();
      }
    }

    public virtual CheckSuite CancelCheckSuit(
      Guid projectId,
      Guid batchRequestId,
      string cancelMessage)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (CancelCheckSuit)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_CancelPolicyEvaluationBatch");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@batchRequestId", batchRequestId);
        this.BindString("@cancelMessage", cancelMessage, 4000, true, SqlDbType.NVarChar);
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    internal virtual List<CheckSuite> GetCheckSuiteResponses()
    {
      List<CheckSuite> checkSuiteResponses = new List<CheckSuite>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CheckSuiteContext>((ObjectBinder<CheckSuiteContext>) new CheckSuiteContextBinder());
        resultCollection.AddBinder<CheckRun>((ObjectBinder<CheckRun>) this.GetCheckSuiteBinder());
        List<CheckSuiteContext> items = resultCollection.GetCurrent<CheckSuiteContext>().Items;
        resultCollection.NextResult();
        foreach (IGrouping<Guid, CheckRun> grouping in resultCollection.GetCurrent<CheckRun>().Items.GroupBy<CheckRun, Guid>((System.Func<CheckRun, Guid>) (record => record.CheckSuiteRef.Id)))
        {
          IGrouping<Guid, CheckRun> group = grouping;
          CheckSuiteContext suiteContext = items.FirstOrDefault<CheckSuiteContext>((System.Func<CheckSuiteContext, bool>) (x => x.SuiteId == group.Key));
          CheckSuite checkSuiteResponse = CheckSuiteResult.GetCheckSuiteResponse(group.Key, group.ToList<CheckRun>(), suiteContext);
          checkSuiteResponses.Add(checkSuiteResponse);
        }
      }
      return checkSuiteResponses;
    }

    private SqlDataRecord ConvertToCheckSuiteUpdateTableSqlDataRecord(
      Guid requestId,
      CheckRunResult row)
    {
      SqlDataRecord record = new SqlDataRecord(CheckSuiteComponent.CheckSuiteUpdateParametersTableType);
      record.SetGuid(0, requestId);
      record.SetByte(1, (byte) row.Status);
      record.SetNullableString(2, row.ResultMessage);
      return record;
    }

    protected virtual SqlDataRecord ConvertToCheckSuiteCreateTableSqlDataRecord(CheckRun row)
    {
      SqlDataRecord record = new SqlDataRecord(CheckSuiteComponent.CheckSuiteCreateParametersTableType);
      record.SetGuid(0, row.Id);
      record.SetInt32(1, row.CheckConfigurationRef.Id);
      record.SetByte(2, (byte) row.Status);
      record.SetNullableString(3, row.ResultMessage);
      return record;
    }

    protected virtual CheckSuiteBinder GetCheckSuiteBinder() => new CheckSuiteBinder();
  }
}
