// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteComponent3
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
  internal class CheckSuiteComponent3 : CheckSuiteComponent2
  {
    private static readonly SqlMetaData[] CheckSuiteCreateParametersTableType = new SqlMetaData[5]
    {
      new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("TimeoutJobId", SqlDbType.UniqueIdentifier)
    };

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
        this.BindTable("@createParameters", "PipelinePolicy.typ_PolicyEvaluationCreateParametersTable2", checkRuns.Select<CheckRun, SqlDataRecord>(new System.Func<CheckRun, SqlDataRecord>(((CheckSuiteComponent) this).ConvertToCheckSuiteCreateTableSqlDataRecord)));
        this.BindString("@resources", parameterValue2, 4000, false, SqlDbType.NVarChar);
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    internal override List<CheckSuite> GetCheckSuiteResponses()
    {
      List<CheckSuite> checkSuiteResponses = new List<CheckSuite>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CheckSuiteContext>((ObjectBinder<CheckSuiteContext>) new CheckSuiteContextBinder2());
        resultCollection.AddBinder<CheckRun>((ObjectBinder<CheckRun>) this.GetCheckSuiteBinder());
        List<CheckSuiteContext> items = resultCollection.GetCurrent<CheckSuiteContext>().Items;
        resultCollection.NextResult();
        IEnumerable<IGrouping<Guid, CheckRun>> source = resultCollection.GetCurrent<CheckRun>().Items.GroupBy<CheckRun, Guid>((System.Func<CheckRun, Guid>) (record => record.CheckSuiteRef.Id));
        if (source.Any<IGrouping<Guid, CheckRun>>())
        {
          foreach (IGrouping<Guid, CheckRun> grouping in source)
          {
            IGrouping<Guid, CheckRun> group = grouping;
            CheckSuiteContext suiteContext = items.FirstOrDefault<CheckSuiteContext>((System.Func<CheckSuiteContext, bool>) (x => x.SuiteId == group.Key));
            CheckSuite checkSuiteResponse = CheckSuiteResult.GetCheckSuiteResponse(group.Key, group.ToList<CheckRun>(), suiteContext);
            checkSuiteResponses.Add(checkSuiteResponse);
          }
        }
        else
        {
          foreach (CheckSuiteContext suiteContext in items)
          {
            CheckSuite checkSuiteResponse = CheckSuiteResult.GetCheckSuiteResponse(suiteContext.SuiteId, new List<CheckRun>(), suiteContext);
            checkSuiteResponses.Add(checkSuiteResponse);
          }
        }
      }
      return checkSuiteResponses;
    }

    protected override SqlDataRecord ConvertToCheckSuiteCreateTableSqlDataRecord(CheckRun row)
    {
      SqlDataRecord record = new SqlDataRecord(CheckSuiteComponent3.CheckSuiteCreateParametersTableType);
      record.SetGuid(0, row.Id);
      record.SetInt32(1, row.CheckConfigurationRef.Id);
      record.SetByte(2, (byte) row.Status);
      record.SetNullableString(3, row.ResultMessage);
      record.SetNullableGuid(4, row.TimeoutJobId);
      return record;
    }

    protected override CheckSuiteBinder GetCheckSuiteBinder() => (CheckSuiteBinder) new CheckSuiteBinder2();
  }
}
