// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteComponent6
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteComponent6 : CheckSuiteComponent5
  {
    private static readonly SqlMetaData[] CheckSuiteCreateParametersTableType = new SqlMetaData[6]
    {
      new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("TimeoutJobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("EvaluationOrder", SqlDbType.TinyInt)
    };

    public override CheckSuite AddCheckRuns(
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckRuns)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddCheckRuns");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@batchRequestId", checkSuiteId);
        this.BindPolicyEvaluationCreateParametersTable(checkRuns);
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    public override CheckSuite AddCheckSuite(
      Guid projectId,
      Guid checkSuiteId,
      List<CheckRun> checkRuns,
      List<Resource> resources,
      JObject evaluationContext)
    {
      string parameterValue1 = evaluationContext != null ? evaluationContext.ToString() : string.Empty;
      string parameterValue2 = resources != null ? JsonConvert.SerializeObject((object) resources) : string.Empty;
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckSuite)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddPolicyBatchEvaluation");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@batchRequestId", checkSuiteId);
        this.BindString("@context", parameterValue1, 4000, false, SqlDbType.NVarChar);
        this.BindPolicyEvaluationCreateParametersTable((IList<CheckRun>) checkRuns);
        this.BindString("@resources", parameterValue2, 4000, false, SqlDbType.NVarChar);
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    protected virtual void BindPolicyEvaluationCreateParametersTable(IList<CheckRun> checkRuns) => this.BindTable("@createParameters", "PipelinePolicy.typ_PolicyEvaluationCreateParametersTable3", checkRuns.Select<CheckRun, SqlDataRecord>(new System.Func<CheckRun, SqlDataRecord>(((CheckSuiteComponent) this).ConvertToCheckSuiteCreateTableSqlDataRecord)));

    protected override SqlDataRecord ConvertToCheckSuiteCreateTableSqlDataRecord(CheckRun row)
    {
      SqlDataRecord record = new SqlDataRecord(CheckSuiteComponent6.CheckSuiteCreateParametersTableType);
      record.SetGuid(0, row.Id);
      record.SetInt32(1, row.CheckConfigurationRef.Id);
      record.SetByte(2, (byte) row.Status);
      record.SetNullableString(3, row.ResultMessage);
      record.SetNullableGuid(4, row.TimeoutJobId);
      record.SetByte(5, (byte) row.EvaluationOrder);
      return record;
    }
  }
}
