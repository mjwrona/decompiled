// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaAgentDatabase
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaAgentDatabase : 
    TestExecutionServiceDatabaseBase,
    IDtaAgentDatabaseComponent,
    IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<DtaAgentDatabase>(4),
      (IComponentCreator) new ComponentCreator<DtaAgentDatabase>(5),
      (IComponentCreator) new ComponentCreator<DtaAgentDatabase>(6)
    }, "TestExecutionService");

    public DtaAgentDatabase()
    {
    }

    public DtaAgentDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public virtual TestAgent RegisterAgent(TestAgent testAgent)
    {
      this.TfsRequestContext.Trace(6200662, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Registering test agent: Agent name:{0} , dtlMachineName:{1} , dtlenvironmenturl:{2}", (object) testAgent.Name, (object) testAgent.DtlMachine.Name, (object) testAgent.DtlEnvironment.Url);
      this.PrepareStoredProcedure("prc_AddDtaAgent");
      this.BindString("@name", testAgent.Name, testAgent.Name.Length, false, SqlDbType.NVarChar);
      this.BindString("@dtlMachineName", testAgent.DtlMachine.Name, testAgent.DtlMachine.Name.Length, false, SqlDbType.NVarChar);
      this.BindString("@dtlEnvUrl", testAgent.DtlEnvironment.Url, testAgent.DtlEnvironment.Url.Length, false, SqlDbType.NVarChar);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        reader.Read();
        DtaAgentDatabase.AddTestAgentColumns testAgentColumns = new DtaAgentDatabase.AddTestAgentColumns();
        testAgent.Id = testAgentColumns.Id.GetInt32((IDataReader) reader);
      }
      this.TfsRequestContext.Trace(6200664, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Agent registered successfully. Agent id : {0}", (object) testAgent.Id);
      return testAgent;
    }

    public virtual void UnRegisterAgent(int testAgentId)
    {
      this.TfsRequestContext.Trace(6200682, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Unregistering test agent. Test agent id:{0}", (object) testAgentId);
      this.PrepareStoredProcedure("prc_DeleteDtaAgent");
      this.BindInt("@id", testAgentId);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200683, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Successfully unregistered test agent. Test agent id :{0}", (object) testAgentId);
    }

    public virtual TestAgent QueryTestAgent(int testAgentId)
    {
      this.TfsRequestContext.Trace(6200672, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Querying test agent details. Test agent id:{0}", (object) testAgentId);
      TestAgent testAgent = (TestAgent) null;
      this.PrepareStoredProcedure("prc_QueryDtaAgentById");
      this.BindInt("@id", testAgentId);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.Read())
        {
          testAgent = new DtaAgentDatabase.DbTestAgentColoumns().Bind(reader);
          this.TfsRequestContext.Trace(6200674, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "{0} executed successfully. Found a registered test agent with the given AgentId : {1}", (object) "prc_QueryDtaAgentById", (object) testAgentId);
        }
        else
        {
          this.TfsRequestContext.Trace(6200673, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "{0} executed successfully. Could not find any registered test agent with the given AgentId : {1}", (object) "prc_QueryDtaAgentById", (object) testAgentId);
          throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.AgentNotFound, (object) testAgentId));
        }
      }
      return testAgent;
    }

    public virtual IEnumerable<TestAgent> QueryTestAgents(TestAgentsQuery query)
    {
      this.TfsRequestContext.Trace(6200675, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Querying test agents. Query : {0}", (object) query));
      this.PrepareStoredProcedure("prc_QueryDtaAgents");
      if (!string.IsNullOrEmpty(query.Name))
        this.BindString("@name", query.Name, 256, false, SqlDbType.NVarChar);
      this.BindInt("@testRunId", query.TestRunId > 0 ? query.TestRunId : 0);
      List<TestAgent> testAgentList = new List<TestAgent>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        DtaAgentDatabase.DbTestAgentColoumns testAgentColoumns = new DtaAgentDatabase.DbTestAgentColoumns();
        while (reader.Read())
          testAgentList.Add(testAgentColoumns.Bind(reader));
      }
      this.TfsRequestContext.Trace(6200676, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "{0} executed successfully", (object) "prc_QueryDtaAgents");
      return (IEnumerable<TestAgent>) testAgentList;
    }

    public virtual void AssociateAgentsWithRun(
      int testRunId,
      string testEnvironmentUrl,
      string autEnvironmentUrl)
    {
      this.PrepareStoredProcedure("prc_UpdateDtaAgentsWithRunId");
      this.BindString("@testEnvironmentUrl", testEnvironmentUrl, 2048, true, SqlDbType.NVarChar);
      this.BindString("@autEnvironmentUrl", autEnvironmentUrl, 2048, true, SqlDbType.NVarChar);
      this.BindInt("@testRunId", testRunId);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateAgentHeartBeat(int testAgentId)
    {
      this.TfsRequestContext.Trace(6200832, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Updating agent heartbeat. Test agent id:{0}", (object) testAgentId);
      this.PrepareStoredProcedure("prc_UpdateDtaAgentHeartBeat");
      this.BindInt("@agentId", testAgentId);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200833, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "{0} executed successfully for agent id : {1}", (object) "prc_UpdateDtaAgentHeartBeat", (object) testAgentId);
    }

    private class AddTestAgentColumns
    {
      internal SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
    }

    private class DbTestAgentColoumns
    {
      private SqlColumnBinder _agentId = new SqlColumnBinder("Id");
      private SqlColumnBinder _agentName = new SqlColumnBinder("Name");
      private SqlColumnBinder _agentDtlMachine = new SqlColumnBinder("DtlMachineName");
      private SqlColumnBinder _agentDtlEnvironment = new SqlColumnBinder("DtlEnvUrl");
      private SqlColumnBinder _testRunId = new SqlColumnBinder("TestRunId");
      private SqlColumnBinder _lastHeartBeatTime = new SqlColumnBinder("LastHeartBeatTime");

      public TestAgent Bind(SqlDataReader reader) => new TestAgent()
      {
        Id = this._agentId.GetInt32((IDataReader) reader),
        Name = this._agentName.GetString((IDataReader) reader, true),
        DtlEnvironment = new ShallowReference()
        {
          Url = this._agentDtlEnvironment.GetString((IDataReader) reader, false)
        },
        DtlMachine = new ShallowReference()
        {
          Name = this._agentDtlMachine.GetString((IDataReader) reader, false)
        },
        TestRunId = this._testRunId.GetInt32((IDataReader) reader, -1, -1),
        LastHeartBeat = this._lastHeartBeatTime.GetDateTime((IDataReader) reader, DateTime.MinValue)
      };
    }
  }
}
