// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaSliceDatabase
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
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaSliceDatabase : TestExecutionServiceDatabaseBase, IDtaSliceDatabase
  {
    private static readonly SqlMetaData[] typ_TestAutomationRunSliceTypeTable = new SqlMetaData[6]
    {
      new SqlMetaData("tcmRunId", SqlDbType.Int),
      new SqlMetaData("SliceData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("sliceType", SqlDbType.TinyInt),
      new SqlMetaData("requirements", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("assignmentOrder", SqlDbType.Int),
      new SqlMetaData("agentId", SqlDbType.Int)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<DtaSliceDataBaseM154>(4),
      (IComponentCreator) new ComponentCreator<DtaSliceDataBaseM154>(5),
      (IComponentCreator) new ComponentCreator<DtaSliceDatabase>(6)
    }, "TestExecutionService");
    private ITestManagementRunHelper _tcmRunHelper;
    protected const int m_defaultIdValue = -1;

    public DtaSliceDatabase()
    {
    }

    internal DtaSliceDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    protected virtual SqlParameter BindTestAutomationRunSliceTypeTable(
      string parameterName,
      IEnumerable<TestAutomationRunSlice> sliceDetails)
    {
      sliceDetails = sliceDetails ?? Enumerable.Empty<TestAutomationRunSlice>();
      return this.BindTable(parameterName, "typ_DtaSliceDetailsParameter4", this.BindTestAutomationRunSliceTypeTableRows(sliceDetails));
    }

    private IEnumerable<SqlDataRecord> BindTestAutomationRunSliceTypeTableRows(
      IEnumerable<TestAutomationRunSlice> sliceDetails)
    {
      foreach (TestAutomationRunSlice sliceDetail in sliceDetails)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DtaSliceDatabase.typ_TestAutomationRunSliceTypeTable);
        sqlDataRecord.SetInt32(0, sliceDetail.TestRunInformation.TcmRun.Id);
        sqlDataRecord.SetString(1, JsonConvert.SerializeObject((object) sliceDetail.LastPhaseResults));
        sqlDataRecord.SetByte(2, (byte) sliceDetail.Type);
        sqlDataRecord.SetSqlString(3, string.IsNullOrEmpty(sliceDetail.Requirements) ? SqlString.Null : (SqlString) sliceDetail.Requirements);
        sqlDataRecord.SetInt32(5, sliceDetail.AssignmentOrder);
        yield return sqlDataRecord;
      }
    }

    protected virtual SqlParameter BindTestAutomationRunSliceTypeTable(
      string parameterName,
      TestAutomationRunSlice sliceDetail,
      List<int> Autagents)
    {
      return this.BindTable(parameterName, "typ_DtaSliceDetailsParameter4", this.BindTestAutomationRunSliceTypeTableRows(sliceDetail, Autagents));
    }

    private IEnumerable<SqlDataRecord> BindTestAutomationRunSliceTypeTableRows(
      TestAutomationRunSlice sliceDetail,
      List<int> AutAgents)
    {
      foreach (int autAgent in AutAgents)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DtaSliceDatabase.typ_TestAutomationRunSliceTypeTable);
        sqlDataRecord.SetInt32(0, sliceDetail.TestRunInformation.TcmRun.Id);
        sqlDataRecord.SetString(1, JsonConvert.SerializeObject((object) sliceDetail.LastPhaseResults));
        sqlDataRecord.SetByte(2, (byte) sliceDetail.Type);
        sqlDataRecord.SetSqlString(3, string.IsNullOrEmpty(sliceDetail.Requirements) ? SqlString.Null : (SqlString) sliceDetail.Requirements);
        sqlDataRecord.SetInt32(4, autAgent);
        sqlDataRecord.SetInt32(5, sliceDetail.AssignmentOrder);
        yield return sqlDataRecord;
      }
    }

    public virtual void QueueSlices(List<TestAutomationRunSlice> sliceDetailsList)
    {
      this.DtaLogger.Verbose(6200801, "Queueing slices to database. Total number of slices : {0} ", (object) sliceDetailsList.Count);
      this.LogSliceDetails(sliceDetailsList);
      string storedProcedure = "prc_QueueDtaSlice";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindTestAutomationRunSliceTypeTable("@sliceDetails", (IEnumerable<TestAutomationRunSlice>) sliceDetailsList);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200802, "{0} has been executed and new slices are queued. Total Number of slices queued are {1} ", (object) storedProcedure, (object) sliceDetailsList.Count);
    }

    public virtual void QueueSliceForAgents(TestAutomationRunSlice slice, List<int> autAgents)
    {
      this.DtaLogger.Verbose(6200803, "Queueing session slices to database. Total number of slices : {0} ", (object) autAgents.Count);
      this.LogSliceDetails(new List<TestAutomationRunSlice>()
      {
        slice
      });
      string storedProcedure = "prc_QueueDtaSlice";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindTestAutomationRunSliceTypeTable("@sliceDetails", slice, autAgents);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200804, "{0} has been executed and new slices are queued. Total Number of slices queued are {1} ", (object) storedProcedure, (object) autAgents.Count);
    }

    public virtual TestAutomationRunSlice GetSlice(
      int testAgentId,
      string testAgentCapabilities,
      int testRunId)
    {
      this.DtaLogger.Verbose(6200805, "Executing {0}. Test agent id : {1}. TestRunId : {2}", (object) "prc_AllocateDtaSlice", (object) testAgentId, (object) testRunId);
      this.PrepareStoredProcedure("prc_AllocateDtaSlice");
      this.BindInt("@testAgentId", testAgentId);
      if (!string.IsNullOrEmpty(testAgentCapabilities))
        this.BindString("@testAgentCapabilities", testAgentCapabilities, 1024, false, SqlDbType.NVarChar);
      if (testRunId > 0)
        this.BindInt("@testRunId", testRunId);
      this.BindByte("@maxRetryCount", (byte) this.GetMaxSliceRetryCount());
      this.BindString("@sliceMessageOnError", JsonConvert.SerializeObject((object) new List<Message>()
      {
        new Message()
        {
          Type = MessageType.Error,
          Data = TestExecutionServiceResources.SliceAborted
        }
      }), 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
        {
          int num = new DtaSliceDatabase.AutomatedTestRunSliceColumns().BindSliceId(reader);
          if (num != -1)
          {
            this.DtaLogger.Error(6200806, "Reallocating Slice id {0} as its agent didnt publish any results. TestRunId : {1}", (object) num, (object) testRunId);
            CILogger.Instance.PublishCI(this.TestExecutionRequestContext, "SliceRetried", new Dictionary<string, object>()
            {
              {
                "SliceRetryReason",
                (object) "AgentMissedPreviousSliceUpdate"
              },
              {
                "TcmRunId",
                (object) testRunId
              }
            });
          }
          else
            break;
        }
        reader.NextResult();
        if (reader.Read())
        {
          DtaSliceDatabase.AutomatedTestRunSliceColumns testRunSliceColumns = new DtaSliceDatabase.AutomatedTestRunSliceColumns();
          TestAutomationRunSlice slice = testRunSliceColumns.Bind(reader);
          int num = testRunSliceColumns.BindRetriesAttempted(reader);
          this.DtaLogger.Verbose(6200807, "{0} has been executed and a slice with slice id {1} is found. Slice has been executed {2} times earlier. TestRunId : {3}", (object) "prc_AllocateDtaSlice", (object) slice.Id, (object) num, (object) testRunId);
          return slice;
        }
        this.DtaLogger.Verbose(6200808, "{0} has been executed and no available slices found. TestRunId : {1}", (object) "prc_AllocateDtaSlice", (object) testRunId);
        return (TestAutomationRunSlice) null;
      }
    }

    public virtual void CancelSlices(int testRunId)
    {
      this.DtaLogger.Info(6200809, "Cancelling all slices with testRunId: {0}", (object) testRunId);
      this.PrepareStoredProcedure("prc_CancelDtaSlices");
      this.BindInt("@testRunId", testRunId);
      this.ExecuteNonQuery();
      this.DtaLogger.Info(6200810, "Slices of test run with id : {0} are cancelled.", (object) testRunId);
    }

    public virtual void UpdateSlice(TestAutomationRunSlice slice)
    {
      this.DtaLogger.Verbose(6200811, "Updating slice details. slice id {0} . slice status {1}. TestRunId : {2}", (object) slice.Id, (object) slice.Status, (object) slice.TestRunInformation.TcmRun.Id);
      string storedProcedure = "prc_UpdateDtaSlice";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindByte("@sliceStatus", (byte) slice.Status);
      this.BindInt("@sliceId", slice.Id);
      this.BindString("@SliceResults", slice.Results, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@Messages", slice.Messages == null ? (string) null : JsonConvert.SerializeObject((object) slice.Messages, Formatting.None), int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.Read())
          this.PublishSliceUpdate(slice, reader);
      }
      this.DtaLogger.Verbose(6200812, "{0} is called and slice got updated. SliceId : {1}\nSliceResults : {2}. TestRunId : {3}", (object) storedProcedure, (object) slice.Id, (object) slice.Results, (object) slice.TestRunInformation.TcmRun.Id);
    }

    public virtual TestAutomationRunSlice QuerySliceById(int sliceId)
    {
      this.DtaLogger.Verbose(6200813, "Querying slice details from database with slice id: {0}.", (object) sliceId);
      this.PrepareStoredProcedure("prc_QueryDtaSliceDetailsById");
      this.BindInt("@sliceId", sliceId);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (!reader.Read())
        {
          this.DtaLogger.Verbose(6200814, "{0} has been executed, and no slice found for sliceId {1}", (object) "prc_QueryDtaSliceDetailsById", (object) sliceId);
          throw new TestExecutionObjectNotFoundException(string.Format(TestExecutionServiceResources.InvalidSliceId, (object) sliceId));
        }
        TestAutomationRunSlice automationRunSlice = new DtaSliceDatabase.AutomatedTestRunSliceColumns().BindPartial(reader);
        this.DtaLogger.Verbose(6200815, "{0} has been executed successfully. SliceId : {1} testRunId : {2}", (object) "prc_QueryDtaSliceDetailsById", (object) sliceId, (object) automationRunSlice.TestRunInformation.TcmRun.Id);
        return automationRunSlice;
      }
    }

    public virtual IEnumerable<TestAutomationRunSlice> QuerySlicesByTestRunId(int testRunId)
    {
      this.DtaLogger.Verbose(6200816, "Querying slice details from database with testRunId: {0}.", (object) testRunId);
      this.PrepareStoredProcedure("prc_QueryDtaSlicesByTestRunId");
      this.BindInt("@testRunId", testRunId);
      List<TestAutomationRunSlice> automationRunSliceList = new List<TestAutomationRunSlice>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        DtaSliceDatabase.AutomatedTestRunSliceColumns testRunSliceColumns = new DtaSliceDatabase.AutomatedTestRunSliceColumns();
        while (reader.Read())
          automationRunSliceList.Add(testRunSliceColumns.BindIdStatusTypeMessages(reader));
      }
      this.DtaLogger.Verbose(6200817, "Querying slice details from database with testRunId: {0}.", (object) testRunId);
      return (IEnumerable<TestAutomationRunSlice>) automationRunSliceList;
    }

    public virtual void AbortSlicesByRunId(int testRunId)
    {
      this.PrepareStoredProcedure("prc_AbortDtaSlicesByRunId");
      this.BindInt("@testRunId", testRunId);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<int> RetrySlicesOfUnReachableAgentsAndGetTestRunIds(
      int allowedDownTimeInSecs,
      int maxRetryCount,
      int testRunId = 0)
    {
      this.DtaLogger.Verbose(6200841, "Retrying slices of unreachable agents");
      this.PrepareStoredProcedure("prc_RetrySlicesOfUnreachableDtaAgents");
      this.BindInt("@agentDownTimeAllowedInSecs", allowedDownTimeInSecs);
      this.BindByte("@maxRetryCount", (byte) maxRetryCount);
      this.BindNullableInt("@testRunId", testRunId, 0);
      this.BindString("@sliceMessageOnError", JsonConvert.SerializeObject((object) new List<Message>()
      {
        new Message()
        {
          Type = MessageType.Error,
          Data = TestExecutionServiceResources.SliceRetryLimitExhausted
        }
      }), 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      HashSet<int> testRunIds = new HashSet<int>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          this.PublishSliceRetry(reader);
        reader.NextResult();
        while (reader.Read())
        {
          int num = new DtaSliceDatabase.AutomatedTestRunSliceColumns().BindTestRunId(reader);
          testRunIds.Add(num);
          CILogger.Instance.PublishCI(this.TestExecutionRequestContext, "SliceAborted", new Dictionary<string, object>()
          {
            {
              "SliceAbortReason",
              (object) "RetryCountExhausted"
            },
            {
              "TcmRunId",
              (object) num
            }
          });
        }
      }
      return (IEnumerable<int>) testRunIds;
    }

    public virtual IEnumerable<int> AbortSlicesIfAllAgentsAreDownAndGetTestRunIds(
      int allowedDownTimeInSecs,
      int testRunId = 0)
    {
      this.DtaLogger.Verbose(6200842, "Aborting slices of any test run in which all agents are down");
      this.PrepareStoredProcedure("prc_AbortDtaSlicesWhenAllDtaAgentsAreDown");
      this.BindInt("@agentDownTimeAllowedInSecs", allowedDownTimeInSecs);
      this.BindNullableInt("@testRunId", testRunId, 0);
      this.BindString("@sliceMessageOnError", JsonConvert.SerializeObject((object) new List<Message>()
      {
        new Message()
        {
          Type = MessageType.Error,
          Data = TestExecutionServiceResources.SliceAborted
        }
      }), 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      HashSet<int> tcmRunIds = new HashSet<int>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          this.PublishSliceAbort(reader, tcmRunIds);
      }
      return (IEnumerable<int>) tcmRunIds;
    }

    private void PublishSliceUpdate(TestAutomationRunSlice slice, SqlDataReader reader)
    {
      int durationInSecs;
      int agentId;
      TestAutomationRunSlice automationRunSlice = new DtaSliceDatabase.AutomatedTestRunSliceColumns().BindCiData(reader, out durationInSecs, out agentId);
      string[] array = automationRunSlice.LastPhaseResults.Select<SlicedTestData, string>((System.Func<SlicedTestData, string>) (o => o.ExecutorUri)).Distinct<string>().ToArray<string>();
      if (durationInSecs <= 0)
        return;
      CILogger.Instance.PublishCI(this.TestExecutionRequestContext, "SliceCompleted", new Dictionary<string, object>()
      {
        {
          "SliceType",
          (object) automationRunSlice.Type.ToString()
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
          (object) agentId
        },
        {
          "SliceDuration",
          (object) durationInSecs
        },
        {
          "TestCasesCount",
          (object) automationRunSlice.LastPhaseResults.Count
        },
        {
          "SliceStatus",
          (object) slice.Status.ToString()
        },
        {
          "ExecutorUri",
          (object) string.Join(",", array)
        }
      });
    }

    private void PublishSliceAbort(SqlDataReader reader, HashSet<int> tcmRunIds)
    {
      TestAutomationRunSlice automationRunSlice = new DtaSliceDatabase.AutomatedTestRunSliceColumns().BindIds(reader);
      tcmRunIds.Add(automationRunSlice.TestRunInformation.TcmRun.Id);
      this.DtaLogger.Verbose(6200843, "Aborted slice id : {0} as all agents in the test run seem to be down. TestRunId : {1}", (object) automationRunSlice.Id, (object) automationRunSlice.TestRunInformation.TcmRun.Id);
      CILogger.Instance.PublishCI(this.TestExecutionRequestContext, "SliceAborted", new Dictionary<string, object>()
      {
        {
          "SliceAbortReason",
          (object) "AllAgentsDown"
        },
        {
          "TcmRunId",
          (object) automationRunSlice.TestRunInformation.TcmRun.Id
        }
      });
    }

    private void PublishSliceRetry(SqlDataReader reader)
    {
      int agentId;
      TestAutomationRunSlice automationRunSlice = new DtaSliceDatabase.AutomatedTestRunSliceColumns().BindAgentAndIds(reader, out agentId);
      this.DtaLogger.Error(6200844, "Retrying slice id : {0} as its agent id : {1} seems to have gone down", (object) automationRunSlice.Id, (object) agentId);
      CILogger.Instance.PublishCI(this.TestExecutionRequestContext, "SliceRetried", new Dictionary<string, object>()
      {
        {
          "SliceRetryReason",
          (object) "AgentUnreachable"
        },
        {
          "AgentId",
          (object) agentId
        },
        {
          "TcmRunId",
          (object) automationRunSlice.TestRunInformation.TcmRun.Id
        }
      });
    }

    private int GetMaxSliceRetryCount() => Utilities.GetValueFromTfsRegistry<int>(this.TestExecutionRequestContext, DtaConstants.TfsRegistryPathForMaxSliceRetryCount, DtaConstants.DefaultMaxSliceRetryCount, DtaSliceDatabase.\u003C\u003EO.\u003C0\u003E__TryParse ?? (DtaSliceDatabase.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));

    public ITestManagementRunHelper TestManagementRunHelper
    {
      get => this._tcmRunHelper ?? (this._tcmRunHelper = (ITestManagementRunHelper) new Microsoft.TeamFoundation.TestExecution.Server.TestManagementRunHelper());
      set => this._tcmRunHelper = value;
    }

    private void LogSliceDetails(List<TestAutomationRunSlice> sliceDetailsList)
    {
      for (int index = 0; index < sliceDetailsList.Count; ++index)
        this.TfsRequestContext.Trace(6200603, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Slice details: sliceType : {0}, TestRunId : {1} , sliceId : {2}, assignmentOrder : {0}", (object) sliceDetailsList[index].Type, (object) sliceDetailsList[index].TestRunInformation.TcmRun.Id, (object) sliceDetailsList[index].Id, (object) sliceDetailsList[index].AssignmentOrder);
    }

    protected class AutomatedTestRunSliceColumns
    {
      internal SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      internal SqlColumnBinder SliceId = new SqlColumnBinder("Id");
      internal SqlColumnBinder SliceType = new SqlColumnBinder(nameof (SliceType));
      internal SqlColumnBinder SliceData = new SqlColumnBinder(nameof (SliceData));
      internal SqlColumnBinder SliceStatus = new SqlColumnBinder(nameof (SliceStatus));
      internal SqlColumnBinder Requirements = new SqlColumnBinder(nameof (Requirements));
      internal SqlColumnBinder AssignmentOrder = new SqlColumnBinder(nameof (AssignmentOrder));
      internal SqlColumnBinder Results = new SqlColumnBinder(nameof (Results));
      internal SqlColumnBinder Messages = new SqlColumnBinder(nameof (Messages));
      internal SqlColumnBinder RetriesAttempted = new SqlColumnBinder(nameof (RetriesAttempted));
      internal SqlColumnBinder AgentId = new SqlColumnBinder(nameof (AgentId));
      internal SqlColumnBinder DurationInSecs = new SqlColumnBinder(nameof (DurationInSecs));

      public TestAutomationRunSlice Bind(SqlDataReader reader)
      {
        TestAutomationRunSlice automationRunSlice = new TestAutomationRunSlice();
        automationRunSlice.TestRunInformation.TcmRun.Id = this.TestRunId.GetInt32((IDataReader) reader);
        automationRunSlice.Id = this.SliceId.GetInt32((IDataReader) reader);
        automationRunSlice.Status = (AutomatedTestRunSliceStatus) this.SliceStatus.GetByte((IDataReader) reader);
        automationRunSlice.Type = (AutomatedTestRunSliceType) this.SliceType.GetByte((IDataReader) reader);
        if (automationRunSlice.Type == AutomatedTestRunSliceType.Execution)
          automationRunSlice.LastPhaseResults = JsonConvert.DeserializeObject<List<SlicedTestData>>(this.SliceData.GetString((IDataReader) reader, false));
        automationRunSlice.Requirements = this.Requirements.GetString((IDataReader) reader, true);
        automationRunSlice.AssignmentOrder = this.AssignmentOrder.ColumnExists((IDataReader) reader) ? this.AssignmentOrder.GetInt32((IDataReader) reader) : 0;
        return automationRunSlice;
      }

      public TestAutomationRunSlice BindIdStatusTypeMessages(SqlDataReader reader)
      {
        TestAutomationRunSlice automationRunSlice = new TestAutomationRunSlice();
        automationRunSlice.Id = this.SliceId.GetInt32((IDataReader) reader);
        automationRunSlice.Status = (AutomatedTestRunSliceStatus) this.SliceStatus.GetByte((IDataReader) reader);
        automationRunSlice.Type = (AutomatedTestRunSliceType) this.SliceType.GetByte((IDataReader) reader);
        if (!this.Messages.IsNull((IDataReader) reader))
          automationRunSlice.Messages = JsonConvert.DeserializeObject<List<Message>>(this.Messages.GetString((IDataReader) reader, false));
        return automationRunSlice;
      }

      public TestAutomationRunSlice BindPartial(SqlDataReader reader) => new TestAutomationRunSlice()
      {
        TestRunInformation = {
          TcmRun = {
            Id = this.TestRunId.GetInt32((IDataReader) reader)
          }
        },
        Type = (AutomatedTestRunSliceType) this.SliceType.GetByte((IDataReader) reader),
        Status = (AutomatedTestRunSliceStatus) this.SliceStatus.GetByte((IDataReader) reader),
        Requirements = this.Requirements.GetString((IDataReader) reader, true),
        Results = this.Results.GetString((IDataReader) reader, true)
      };

      public TestAutomationRunSlice BindAgentAndIds(SqlDataReader reader, out int agentId)
      {
        agentId = this.AgentId.GetInt32((IDataReader) reader, -1);
        return this.BindIds(reader);
      }

      public TestAutomationRunSlice BindIds(SqlDataReader reader) => new TestAutomationRunSlice()
      {
        TestRunInformation = {
          TcmRun = {
            Id = this.BindTestRunId(reader)
          }
        },
        Id = this.BindSliceId(reader)
      };

      public int BindSliceId(SqlDataReader reader) => this.SliceId.GetInt32((IDataReader) reader, -1);

      public int BindTestRunId(SqlDataReader reader) => this.TestRunId.GetInt32((IDataReader) reader, -1);

      public int BindRetriesAttempted(SqlDataReader reader) => (int) this.RetriesAttempted.GetByte((IDataReader) reader);

      public TestAutomationRunSlice BindCiData(
        SqlDataReader reader,
        out int durationInSecs,
        out int agentId)
      {
        TestAutomationRunSlice automationRunSlice = new TestAutomationRunSlice();
        durationInSecs = this.DurationInSecs.GetInt32((IDataReader) reader, 0);
        agentId = this.AgentId.GetInt32((IDataReader) reader, -1);
        automationRunSlice.Type = (AutomatedTestRunSliceType) this.SliceType.GetByte((IDataReader) reader);
        automationRunSlice.LastPhaseResults = new List<SlicedTestData>();
        if (automationRunSlice.Type == AutomatedTestRunSliceType.Execution)
          automationRunSlice.LastPhaseResults = JsonConvert.DeserializeObject<List<SlicedTestData>>(this.SliceData.GetString((IDataReader) reader, false));
        return automationRunSlice;
      }
    }
  }
}
