// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteComponent5
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteComponent5 : CheckSuiteComponent4
  {
    public override List<CheckSuite> UpdateCheckSuite(
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
        this.BindPolicyEvaluationUpdateParameters(checkSuiteUpdateParameters);
        return this.GetCheckSuiteResponses();
      }
    }

    protected virtual void BindPolicyEvaluationUpdateParameters(
      IReadOnlyDictionary<Guid, CheckRunResult> checkSuiteUpdateParameters)
    {
      this.BindTable("@updateParameters", "PipelinePolicy.typ_PolicyEvaluationUpdateParametersTable2", checkSuiteUpdateParameters.Select<KeyValuePair<Guid, CheckRunResult>, SqlDataRecord>((System.Func<KeyValuePair<Guid, CheckRunResult>, SqlDataRecord>) (parameter => CheckSuiteComponent5.ConvertToCheckSuiteUpdateTableSqlDataRecord(parameter.Key, parameter.Value))));
    }

    private static SqlDataRecord ConvertToCheckSuiteUpdateTableSqlDataRecord(
      Guid requestId,
      CheckRunResult row)
    {
      SqlDataRecord record = new SqlDataRecord(new SqlMetaData[4]
      {
        new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier)
      });
      record.SetGuid(0, requestId);
      record.SetByte(1, (byte) row.Status);
      record.SetNullableString(2, row.ResultMessage);
      if (row.ModifiedBy?.Id != null)
        record.SetNullableGuid(3, new Guid(row.ModifiedBy.Id));
      return record;
    }

    protected virtual CheckRunUpdateBinder GetCheckRunUpdateBinder() => new CheckRunUpdateBinder();

    internal override List<CheckSuite> GetCheckSuiteResponses()
    {
      List<CheckSuite> checkSuiteResponses = new List<CheckSuite>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CheckSuiteContext>((ObjectBinder<CheckSuiteContext>) new CheckSuiteContextBinder2());
        resultCollection.AddBinder<CheckRun>((ObjectBinder<CheckRun>) this.GetCheckSuiteBinder());
        resultCollection.AddBinder<CheckRunUpdate>((ObjectBinder<CheckRunUpdate>) this.GetCheckRunUpdateBinder());
        List<CheckSuiteContext> items1 = resultCollection.GetCurrent<CheckSuiteContext>().Items;
        resultCollection.NextResult();
        List<CheckRun> items2 = resultCollection.GetCurrent<CheckRun>().Items;
        IList<CheckRunUpdate> source1 = (IList<CheckRunUpdate>) null;
        if (resultCollection.TryNextResult())
          source1 = (IList<CheckRunUpdate>) resultCollection.GetCurrent<CheckRunUpdate>().Items.OrderByDescending<CheckRunUpdate, DateTime>((System.Func<CheckRunUpdate, DateTime>) (resultUpdate => resultUpdate.ModifiedOn)).ToList<CheckRunUpdate>();
        IEnumerable<IGrouping<Guid, CheckRun>> source2 = items2.GroupBy<CheckRun, Guid>((System.Func<CheckRun, Guid>) (record => record.CheckSuiteRef.Id));
        IEnumerable<IGrouping<\u003C\u003Ef__AnonymousType0<Guid, Guid>, CheckRunUpdate>> checkRunUpdateHistoryGroups = source1 != null ? source1.GroupBy(record => new
        {
          CheckSuiteId = record.CheckSuiteId,
          CheckRunId = record.CheckRunId
        }) : null;
        if (source2.Any<IGrouping<Guid, CheckRun>>())
        {
          foreach (IGrouping<Guid, CheckRun> grouping1 in source2)
          {
            IGrouping<Guid, CheckRun> runsGroup = grouping1;
            if (!checkRunUpdateHistoryGroups.IsNullOrEmpty<IGrouping<\u003C\u003Ef__AnonymousType0<Guid, Guid>, CheckRunUpdate>>())
              runsGroup.ForEach<CheckRun>((Action<CheckRun>) (run =>
              {
                IGrouping<\u003C\u003Ef__AnonymousType0<Guid, Guid>, CheckRunUpdate> grouping2 = checkRunUpdateHistoryGroups.FirstOrDefault<IGrouping<\u003C\u003Ef__AnonymousType0<Guid, Guid>, CheckRunUpdate>>(rug => rug.Key.CheckSuiteId.Equals(runsGroup.Key) && rug.Key.CheckRunId.Equals(run.Id));
                if (grouping2.IsNullOrEmpty<CheckRunUpdate>())
                  return;
                run.ResultUpdates.AddRange((IEnumerable<CheckRunUpdate>) grouping2.ToList<CheckRunUpdate>());
              }));
            CheckSuiteContext suiteContext = items1.FirstOrDefault<CheckSuiteContext>((System.Func<CheckSuiteContext, bool>) (x => x.SuiteId == runsGroup.Key));
            CheckSuite checkSuiteResponse = CheckSuiteResult.GetCheckSuiteResponse(runsGroup.Key, runsGroup.ToList<CheckRun>(), suiteContext);
            checkSuiteResponses.Add(checkSuiteResponse);
          }
        }
        else
        {
          foreach (CheckSuiteContext suiteContext in items1)
          {
            CheckSuite checkSuiteResponse = CheckSuiteResult.GetCheckSuiteResponse(suiteContext.SuiteId, new List<CheckRun>(), suiteContext);
            checkSuiteResponses.Add(checkSuiteResponse);
          }
        }
      }
      return checkSuiteResponses;
    }
  }
}
