// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteComponent7
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteComponent7 : CheckSuiteComponent6
  {
    private static readonly SqlMetaData[] CheckSuiteCreateParametersTableType = new SqlMetaData[7]
    {
      new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.SmallInt),
      new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("TimeoutJobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("EvaluationOrder", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentVersion", SqlDbType.Int)
    };

    protected override IList<CheckRun> GetCheckRunsResponse()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CheckRun>((ObjectBinder<CheckRun>) new CheckSuiteBinder4());
        return (IList<CheckRun>) resultCollection.GetCurrent<CheckRun>().Items;
      }
    }

    protected override void BindPolicyEvaluationUpdateParameters(
      IReadOnlyDictionary<Guid, CheckRunResult> checkSuiteUpdateParameters)
    {
      this.BindTable("@updateParameters", "PipelinePolicy.typ_PolicyEvaluationUpdateParametersTable3", checkSuiteUpdateParameters.Select<KeyValuePair<Guid, CheckRunResult>, SqlDataRecord>((System.Func<KeyValuePair<Guid, CheckRunResult>, SqlDataRecord>) (parameter => CheckSuiteComponent7.ConvertToCheckSuiteUpdateTableSqlDataRecord(parameter.Key, parameter.Value))));
    }

    private static SqlDataRecord ConvertToCheckSuiteUpdateTableSqlDataRecord(
      Guid requestId,
      CheckRunResult row)
    {
      SqlDataRecord record = new SqlDataRecord(new SqlMetaData[4]
      {
        new SqlMetaData("RequestId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Status", SqlDbType.SmallInt),
        new SqlMetaData("ResultMessage", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier)
      });
      record.SetGuid(0, requestId);
      record.SetInt16(1, (short) row.Status);
      record.SetNullableString(2, row.ResultMessage);
      if (row.ModifiedBy?.Id != null)
        record.SetNullableGuid(3, new Guid(row.ModifiedBy.Id));
      return record;
    }

    protected override CheckRunUpdateBinder GetCheckRunUpdateBinder() => (CheckRunUpdateBinder) new CheckRunUpdateBinder2();

    protected override CheckSuiteBinder GetCheckSuiteBinder() => (CheckSuiteBinder) new CheckSuiteBinder4();

    protected override void BindPolicyEvaluationCreateParametersTable(IList<CheckRun> checkRuns) => this.BindTable("@createParameters", "PipelinePolicy.typ_PolicyEvaluationCreateParametersTable4", checkRuns.Select<CheckRun, SqlDataRecord>(new System.Func<CheckRun, SqlDataRecord>(((CheckSuiteComponent) this).ConvertToCheckSuiteCreateTableSqlDataRecord)));

    protected override SqlDataRecord ConvertToCheckSuiteCreateTableSqlDataRecord(CheckRun row)
    {
      SqlDataRecord record = new SqlDataRecord(CheckSuiteComponent7.CheckSuiteCreateParametersTableType);
      ArgumentUtility.CheckForNonPositiveInt(row.CheckConfigurationRef.Version, "Version");
      record.SetGuid(0, row.Id);
      record.SetInt32(1, row.CheckConfigurationRef.Id);
      record.SetInt16(2, (short) row.Status);
      record.SetNullableString(3, row.ResultMessage);
      record.SetNullableGuid(4, row.TimeoutJobId);
      record.SetByte(5, (byte) row.EvaluationOrder);
      record.SetInt32(6, row.CheckConfigurationRef.Version);
      return record;
    }
  }
}
