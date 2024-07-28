// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaSliceDataBaseM154
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaSliceDataBaseM154 : DtaSliceDatabase
  {
    private static readonly SqlMetaData[] typ_TestAutomationRunSliceTypeTable = new SqlMetaData[5]
    {
      new SqlMetaData("tcmRunId", SqlDbType.Int),
      new SqlMetaData("SliceData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("sliceType", SqlDbType.TinyInt),
      new SqlMetaData("requirements", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("agentId", SqlDbType.Int)
    };

    protected override SqlParameter BindTestAutomationRunSliceTypeTable(
      string parameterName,
      IEnumerable<TestAutomationRunSlice> sliceDetails)
    {
      sliceDetails = sliceDetails ?? Enumerable.Empty<TestAutomationRunSlice>();
      return this.BindTable(parameterName, "typ_DtaSliceDetailsParameter3", this.BindTestAutomationRunSliceTypeTableRows(sliceDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestAutomationRunSliceTypeTableRows(
      IEnumerable<TestAutomationRunSlice> sliceDetails)
    {
      foreach (TestAutomationRunSlice sliceDetail in sliceDetails)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DtaSliceDataBaseM154.typ_TestAutomationRunSliceTypeTable);
        sqlDataRecord.SetInt32(0, sliceDetail.TestRunInformation.TcmRun.Id);
        sqlDataRecord.SetString(1, JsonConvert.SerializeObject((object) sliceDetail.LastPhaseResults));
        sqlDataRecord.SetByte(2, (byte) sliceDetail.Type);
        sqlDataRecord.SetSqlString(3, string.IsNullOrEmpty(sliceDetail.Requirements) ? SqlString.Null : (SqlString) sliceDetail.Requirements);
        yield return sqlDataRecord;
      }
    }

    protected override SqlParameter BindTestAutomationRunSliceTypeTable(
      string parameterName,
      TestAutomationRunSlice sliceDetail,
      List<int> Autagents)
    {
      return this.BindTable(parameterName, "typ_DtaSliceDetailsParameter3", this.BindTestAutomationRunSliceTypeTableRows(sliceDetail, Autagents));
    }

    private IEnumerable<SqlDataRecord> BindTestAutomationRunSliceTypeTableRows(
      TestAutomationRunSlice sliceDetail,
      List<int> AutAgents)
    {
      foreach (int autAgent in AutAgents)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DtaSliceDataBaseM154.typ_TestAutomationRunSliceTypeTable);
        sqlDataRecord.SetInt32(0, sliceDetail.TestRunInformation.TcmRun.Id);
        sqlDataRecord.SetString(1, JsonConvert.SerializeObject((object) sliceDetail.LastPhaseResults));
        sqlDataRecord.SetByte(2, (byte) sliceDetail.Type);
        sqlDataRecord.SetSqlString(3, string.IsNullOrEmpty(sliceDetail.Requirements) ? SqlString.Null : (SqlString) sliceDetail.Requirements);
        sqlDataRecord.SetInt32(4, autAgent);
        yield return sqlDataRecord;
      }
    }
  }
}
