// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaMessageQueueDatabaseM81_82
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaMessageQueueDatabaseM81_82 : DtaMessageQueueDatabase
  {
    public override void AddMessageQueueDetails(MessageQueueDetails queueDetails)
    {
      this.TfsRequestContext.Trace(6200782, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Adding message queue details to database. TestRunId: {0} ,message queue name : {1} ,Dtl Environment url: {2} ,Session id :{3}", (object) queueDetails.TestRunId, (object) queueDetails.QueueName, (object) queueDetails.TestEnvironmentUrl, (object) queueDetails.SessionId);
      this.PrepareStoredProcedure("prc_AddMessageQueueDetails");
      this.BindInt("@testRunId", queueDetails.TestRunId);
      this.BindGuid("@sessionId", queueDetails.SessionId);
      this.BindString("@dtlEnvironmentUrl", queueDetails.TestEnvironmentUrl, 2048, true, SqlDbType.NVarChar);
      this.BindString("@queueName", queueDetails.QueueName, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200783, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Successfully done adding message queue details to database. TestRunId :{0}", (object) queueDetails.TestRunId);
    }

    public override MessageQueueDetails QueryMessageQueueDetailsByTestRunId(int testRunId)
    {
      this.TfsRequestContext.Trace(6200802, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Querying message queue details from database by testrun id. TestRunId :{0}", (object) testRunId);
      this.PrepareStoredProcedure("prc_QueryMessageQueueDetailsbyTestRunId");
      this.BindInt("@testRunId", testRunId);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (!sqlDataReader.Read())
        {
          this.TfsRequestContext.Trace(6200803, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "No message queue details found. Test run id :{0}", (object) testRunId);
          return (MessageQueueDetails) null;
        }
        DtaMessageQueueDatabaseM81_82.MessageQueueColumns_82 messageQueueColumns82 = new DtaMessageQueueDatabaseM81_82.MessageQueueColumns_82();
        MessageQueueDetails messageQueueDetails1 = new MessageQueueDetails();
        SqlDataReader reader = sqlDataReader;
        MessageQueueDetails messageQueueDetails2 = messageQueueDetails1;
        messageQueueColumns82.Bind(reader, messageQueueDetails2);
        this.TfsRequestContext.Trace(6200804, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "{0} returned message queue details. Test run id :{1} ,MessageQueueName :{2} ", (object) "prc_QueryMessageQueueDetailsbyTestRunId", (object) testRunId, (object) messageQueueDetails1.QueueName);
        return messageQueueDetails1;
      }
    }

    public override MessageQueueDetails QueryMessageQueueDetailsByDtlEnvUrl(string dtlEnvUrl)
    {
      this.TfsRequestContext.Trace(6200812, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Querying message queue details from database by dtl environment url. Dtl environment url : {0}", (object) dtlEnvUrl);
      this.PrepareStoredProcedure("prc_QueryMessageQueueDetailsByEnvUrl");
      this.BindString("@dtlEnvUrl", dtlEnvUrl, 2048, true, SqlDbType.NVarChar);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (!sqlDataReader.Read())
        {
          this.TfsRequestContext.Trace(6200813, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "No message queue details found. Dtl Environment url : {0}", (object) dtlEnvUrl);
          return (MessageQueueDetails) null;
        }
        DtaMessageQueueDatabaseM81_82.MessageQueueColumns_82 messageQueueColumns82 = new DtaMessageQueueDatabaseM81_82.MessageQueueColumns_82();
        MessageQueueDetails messageQueueDetails1 = new MessageQueueDetails();
        SqlDataReader reader = sqlDataReader;
        MessageQueueDetails messageQueueDetails2 = messageQueueDetails1;
        messageQueueColumns82.Bind(reader, messageQueueDetails2);
        this.TfsRequestContext.Trace(6200814, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "{0} returned message queue details. TestRunId :{1} ,MessageQueueName :{2} ,Dtl environment url : {3}", (object) "prc_QueryMessageQueueDetailsByEnvUrl", (object) messageQueueDetails1.TestRunId, (object) messageQueueDetails1.QueueName, (object) dtlEnvUrl);
        return messageQueueDetails1;
      }
    }

    private class MessageQueueColumns_82
    {
      internal SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      internal SqlColumnBinder DtlEnvironmentUrl = new SqlColumnBinder("DtlEnvUrl");
      internal SqlColumnBinder SessionId = new SqlColumnBinder(nameof (SessionId));
      internal SqlColumnBinder QueueName = new SqlColumnBinder(nameof (QueueName));

      internal void Bind(SqlDataReader reader, MessageQueueDetails messageQueueDetails)
      {
        messageQueueDetails.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        messageQueueDetails.TestEnvironmentUrl = this.DtlEnvironmentUrl.GetString((IDataReader) reader, false);
        messageQueueDetails.SessionId = this.SessionId.GetGuid((IDataReader) reader);
        messageQueueDetails.QueueName = this.QueueName.GetString((IDataReader) reader, false);
      }
    }
  }
}
