// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaSliceDataBaseM84
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaSliceDataBaseM84 : DtaSliceDatabase
  {
    public override void UpdateSlice(TestAutomationRunSlice slice)
    {
      this.DtaLogger.Verbose(6200846, "Updating slice details. slice id {0} . slice status {1}. TestRunId : {2}", (object) slice.Id, (object) slice.Status, (object) slice.TestRunInformation.TcmRun.Id);
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
          int int32_1 = sqlDataReader.GetInt32(0);
          int int32_2 = sqlDataReader.GetInt32(1);
          if (int32_1 > 0)
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
                "SliceId",
                (object) slice.Id
              },
              {
                "AgentId",
                (object) int32_2
              },
              {
                "SliceDuration",
                (object) int32_1
              },
              {
                "SliceStatus",
                (object) slice.Status.ToString()
              }
            });
        }
      }
      this.DtaLogger.Verbose(6200847, "{0} is called and slice got updated. SliceId : {1}\nSliceResults : {2}. TestRunId : {3}", (object) storedProcedure, (object) slice.Id, (object) slice.Results, (object) slice.TestRunInformation.TcmRun.Id);
    }
  }
}
