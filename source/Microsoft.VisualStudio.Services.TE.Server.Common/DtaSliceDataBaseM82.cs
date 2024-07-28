// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaSliceDataBaseM82
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
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
  public class DtaSliceDataBaseM82 : DtaSliceDatabase
  {
    private static readonly SqlMetaData[] typ_TestAutomationRunSliceTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("tcmRunId", SqlDbType.Int),
      new SqlMetaData("SliceData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("sliceType", SqlDbType.TinyInt),
      new SqlMetaData("requirements", SqlDbType.NVarChar, 1024L)
    };

    protected override SqlParameter BindTestAutomationRunSliceTypeTable(
      string parameterName,
      IEnumerable<TestAutomationRunSlice> sliceDetails)
    {
      sliceDetails = sliceDetails ?? Enumerable.Empty<TestAutomationRunSlice>();
      return this.BindTable(parameterName, "typ_DtaSliceDetailsParameter2", this.BindTestAutomationRunSliceTypeTableRows(sliceDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestAutomationRunSliceTypeTableRows(
      IEnumerable<TestAutomationRunSlice> sliceDetails)
    {
      foreach (TestAutomationRunSlice sliceDetail in sliceDetails)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DtaSliceDataBaseM82.typ_TestAutomationRunSliceTypeTable);
        sqlDataRecord.SetInt32(0, sliceDetail.TestRunInformation.TcmRun.Id);
        sqlDataRecord.SetString(1, JsonConvert.SerializeObject((object) sliceDetail.LastPhaseResults));
        sqlDataRecord.SetByte(2, (byte) sliceDetail.Type);
        sqlDataRecord.SetSqlString(3, string.IsNullOrEmpty(sliceDetail.Requirements) ? SqlString.Null : (SqlString) sliceDetail.Requirements);
        yield return sqlDataRecord;
      }
    }

    public override void UpdateSlice(TestAutomationRunSlice slice)
    {
      this.DtaLogger.Verbose(6200848, "Updating slice details. slice id {0} . slice status {1}. TestRunId : {2}", (object) slice.Id, (object) slice.Status, (object) slice.TestRunInformation.TcmRun.Id);
      string storedProcedure = "prc_UpdateDtaSlice";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindByte("@sliceStatus", (byte) slice.Status);
      this.BindInt("@sliceId", slice.Id);
      this.BindString("@SliceResults", slice.Results, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@Messages", slice.Messages == null ? (string) null : JsonConvert.SerializeObject((object) slice.Messages, Formatting.None), int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (sqlDataReader.Read())
        {
          int int32 = sqlDataReader.GetInt32(0);
          if (int32 > 0)
            CILogger.Instance.PublishCI(this.TestExecutionRequestContext, "SliceCompleted", new Dictionary<string, object>()
            {
              {
                "SliceType",
                (object) slice.Type.ToString()
              },
              {
                "TcmRunId",
                (object) slice.TestRunInformation.TcmRun.Id
              },
              {
                "SliceDuration",
                (object) int32
              },
              {
                "SliceStatus",
                (object) slice.Status.ToString()
              },
              {
                "SliceId",
                (object) slice.Id
              }
            });
        }
      }
      this.DtaLogger.Verbose(6200849, "{0} is called and slice got updated. SliceId : {1}\nSliceResults : {2}. TestRunId : {3}", (object) storedProcedure, (object) slice.Id, (object) slice.Results, (object) slice.TestRunInformation.TcmRun.Id);
    }

    public override void QueueSlices(List<TestAutomationRunSlice> sliceDetailsList)
    {
      this.DtaLogger.Verbose(6200850, "Queueing slices to database. Total number of slices : {0} ", (object) sliceDetailsList.Count);
      this.LogSliceDetails(sliceDetailsList);
      string storedProcedure = "prc_QueueDtaSlice";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindTestAutomationRunSliceTypeTable("@sliceDetails", (IEnumerable<TestAutomationRunSlice>) sliceDetailsList);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200851, "{0} has been executed and new slices are queued. Total Number of slices queued are {1} ", (object) storedProcedure, (object) sliceDetailsList.Count);
    }

    public override void QueueSliceForAgents(TestAutomationRunSlice slice, List<int> autAgents)
    {
      List<TestAutomationRunSlice> automationRunSliceList = new List<TestAutomationRunSlice>()
      {
        slice
      };
      this.DtaLogger.Verbose(6200852, "Queueing slices to database. Total number of slices : {0} ", (object) automationRunSliceList.Count);
      this.LogSliceDetails(automationRunSliceList);
      string storedProcedure = "prc_QueueDtaSlice";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindTestAutomationRunSliceTypeTable("@sliceDetails", (IEnumerable<TestAutomationRunSlice>) automationRunSliceList);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200853, "{0} has been executed and new slices are queued. Total Number of slices queued are {1} ", (object) storedProcedure, (object) automationRunSliceList.Count);
    }

    private void LogSliceDetails(List<TestAutomationRunSlice> sliceDetailsList)
    {
      for (int index = 0; index < sliceDetailsList.Count; ++index)
        this.DtaLogger.Verbose(6200854, "Slice details: sliceType : {0}, TestRunId : {1} , sliceId : {2}", (object) sliceDetailsList[index].Type, (object) sliceDetailsList[index].TestRunInformation.TcmRun.Id, (object) sliceDetailsList[index].Id);
    }
  }
}
