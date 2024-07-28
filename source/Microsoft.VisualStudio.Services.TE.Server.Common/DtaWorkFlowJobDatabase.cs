// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DtaWorkFlowJobDatabase
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.Server;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DtaWorkFlowJobDatabase : TestExecutionServiceDatabaseBase, IDtaWorkFlowJobDatabase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<DtaWorkFlowJobDatabase>(6),
      (IComponentCreator) new ComponentCreator<DtaWorkFlowJobDatabase>(5),
      (IComponentCreator) new ComponentCreator<DtaWorkFlowJobDatabase>(4)
    }, "TestExecutionService");

    public DtaWorkFlowJobDatabase()
    {
    }

    internal DtaWorkFlowJobDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public virtual void AddWorkFlowJob(WorkFlowJobDetails workFlowJobDetail)
    {
      this.DtaLogger.Verbose(6200701, "Queueing workflow job .TestRunId : {0}, Phase step :{1}, Phase state :{2}, Phase data : {3}", (object) workFlowJobDetail.TestRunId, (object) workFlowJobDetail.PhaseState, (object) workFlowJobDetail.PhaseStep, (object) workFlowJobDetail.PhaseData);
      this.PrepareStoredProcedure("prc_AddDtaWorkFlowJob");
      this.BindInt("@testRunId", workFlowJobDetail.TestRunId);
      this.BindInt("@phaseStep", (int) workFlowJobDetail.PhaseStep);
      this.BindInt("@phaseState", (int) workFlowJobDetail.PhaseState);
      this.BindString("@phaseData", workFlowJobDetail.PhaseData, 512, true, SqlDbType.NVarChar);
      this.BindGuid("@jobId", workFlowJobDetail.JobId);
      this.BindDateTime("@timeout", workFlowJobDetail.Timeout.UtcDateTime);
      this.BindString("@workflowDetails", JsonConvert.SerializeObject((object) workFlowJobDetail.Context), -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200702, "Done queuing workflow job. TestRunId :{0}.", (object) workFlowJobDetail.TestRunId);
    }

    public virtual void UpdateWorkFlowJob(WorkFlowJobDetails workFlowJobDetails)
    {
      this.DtaLogger.Verbose(6200703, "Updating workflow job .TestRunId : {0}, Phase step :{1}, Phase state :{2}. Phase data :{3}", (object) workFlowJobDetails.TestRunId, (object) workFlowJobDetails.PhaseState, (object) workFlowJobDetails.PhaseStep, (object) workFlowJobDetails.PhaseData);
      this.PrepareStoredProcedure("prc_UpdateDtaWorkFlowJob");
      this.BindInt("@testRunId", workFlowJobDetails.TestRunId);
      this.BindInt("@phaseStep", (int) workFlowJobDetails.PhaseStep);
      this.BindInt("@phaseState", (int) workFlowJobDetails.PhaseState);
      this.BindString("@phaseData", workFlowJobDetails.PhaseData, 512, true, SqlDbType.NVarChar);
      this.BindString("@workflowDetails", JsonConvert.SerializeObject((object) workFlowJobDetails.Context), -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200704, "Done updating workflow job. TestRunId :{0}.", (object) workFlowJobDetails.TestRunId);
    }

    public virtual void AbortWorkFlowJob(int testRunId, Guid abortedBy)
    {
      this.DtaLogger.Warning(6200705, "Aborting test workflow job in database. TestRunId : {0}", (object) testRunId);
      this.PrepareStoredProcedure("prc_AbortDtaWorkFlowJob");
      this.BindInt("@testRunId", testRunId);
      this.BindGuid("@abortedBy", abortedBy);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200706, "Work flow job aborted in database. TestRunId : {0}", (object) testRunId);
    }

    public virtual WorkFlowJobDetails QueryWorkflowJob(int testRunId)
    {
      this.DtaLogger.Verbose(6200707, "Querying test workflow job details from database. TestRunId : {0}", (object) testRunId);
      this.PrepareStoredProcedure("prc_QueryDtaWorkFlowJob");
      this.BindInt("@testRunId", testRunId);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (!reader.Read())
        {
          this.DtaLogger.Warning(6200708, "{0} has been executed and no workflow job found with TestRunId {1}.", (object) "prc_QueryDtaWorkFlowJob", (object) testRunId);
          return (WorkFlowJobDetails) null;
        }
        WorkFlowJobDetails workFlowJobDetails = new DtaWorkFlowJobDatabase.DbWorkFlowJobColumns().Bind(reader);
        this.DtaLogger.Verbose(6200709, "{0} has been executed and a work flow job is found for TestRunId {1}.", (object) "prc_QueryDtaWorkFlowJob", (object) testRunId);
        return workFlowJobDetails;
      }
    }

    public virtual void CleanupTestRunArtifacts(int testRunId)
    {
      this.DtaLogger.Verbose(6200710, "Cleaning up test artifacts from database. TestRunId : {0}", (object) testRunId);
      this.PrepareStoredProcedure("prc_CleanupDtaRunArtifacts");
      this.BindInt(nameof (testRunId), testRunId);
      this.ExecuteNonQuery();
      this.DtaLogger.Verbose(6200711, "Done cleaning up test artifacts from database. TestRunId : {0}", (object) testRunId);
    }

    private class DbWorkFlowJobColumns
    {
      private SqlColumnBinder _phaseStep = new SqlColumnBinder("PhaseStep");
      private SqlColumnBinder _phaseState = new SqlColumnBinder("PhaseState");
      private SqlColumnBinder _testRunId = new SqlColumnBinder("TestRunId");
      private SqlColumnBinder _jobId = new SqlColumnBinder("JobId");
      private SqlColumnBinder _timeout = new SqlColumnBinder("TestRunTimeout");
      private SqlColumnBinder _phaseData = new SqlColumnBinder("PhaseData");
      private SqlColumnBinder _abortRequested = new SqlColumnBinder("AbortRequested");
      private SqlColumnBinder _abortedBy = new SqlColumnBinder("AbortedBy");
      private SqlColumnBinder _workflowDetails = new SqlColumnBinder("WorkflowDetails");

      internal WorkFlowJobDetails Bind(SqlDataReader reader)
      {
        WorkFlowJobDetails workFlowJobDetails = new WorkFlowJobDetails()
        {
          PhaseStep = (Phase) this._phaseStep.GetByte((IDataReader) reader),
          PhaseState = (AutomationPhaseState) this._phaseState.GetByte((IDataReader) reader),
          TestRunId = this._testRunId.GetInt32((IDataReader) reader),
          PhaseData = this._phaseData.GetString((IDataReader) reader, true),
          JobId = this._jobId.GetGuid((IDataReader) reader),
          Timeout = this._timeout.GetDateTimeOffset(reader),
          AbortRequested = this._abortRequested.GetBoolean((IDataReader) reader),
          AbortedBy = this._abortedBy.GetGuid((IDataReader) reader, true)
        };
        string str = this._workflowDetails.GetString((IDataReader) reader, true);
        if (!string.IsNullOrWhiteSpace(str))
        {
          try
          {
            workFlowJobDetails.Context = JsonConvert.DeserializeObject<WorkFlowContext>(str);
          }
          catch (JsonSerializationException ex)
          {
            throw new TestExecutionObjectNotFoundException("Failed to deserialize work flow details fetched from the db.", (Exception) ex);
          }
        }
        else
          workFlowJobDetails.Context = new WorkFlowContext()
          {
            TestRunId = workFlowJobDetails.TestRunId
          };
        return workFlowJobDetails;
      }
    }
  }
}
