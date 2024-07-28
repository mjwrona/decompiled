// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent8
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent8 : ProjectComponent7
  {
    private static SqlMetaData[] typ_ProjectRevisionTable = new SqlMetaData[2]
    {
      new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Revision", SqlDbType.BigInt)
    };

    internal override Dictionary<Guid, IEnumerable<ProjectOperation>> GetUnpublishedProjectChanges()
    {
      this.PrepareStoredProcedure("prc_ProjectQueryUnpublishedChanges");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        return resultCollection.GetCurrent<ProjectOperation>().Items.GroupBy<ProjectOperation, Guid, ProjectOperation>((System.Func<ProjectOperation, Guid>) (projectOperation => projectOperation.ProjectId), (System.Func<ProjectOperation, ProjectOperation>) (projectOperation => projectOperation)).ToDictionary<IGrouping<Guid, ProjectOperation>, Guid, IEnumerable<ProjectOperation>>((System.Func<IGrouping<Guid, ProjectOperation>, Guid>) (group => group.Key), (System.Func<IGrouping<Guid, ProjectOperation>, IEnumerable<ProjectOperation>>) (group => (IEnumerable<ProjectOperation>) group));
      }
    }

    internal override void UpdateProjectWatermarks(IDictionary<Guid, long> revisions)
    {
      this.PrepareStoredProcedure("prc_ProjectUpdateWatermarks");
      ProjectComponent8.BindRevisionTable((TeamFoundationSqlResourceComponent) this, "@projectRevisions", revisions);
      this.ExecuteNonQuery();
    }

    internal override bool HasUnacknowledgedProjectChanges(string projectName)
    {
      this.PrepareStoredProcedure("prc_ProjectQueryPendingByName");
      this.BindProjectName("@projectName", projectName);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (dataReader => 1)));
        return resultCollection.GetCurrent<int>().Items.Any<int>();
      }
    }

    internal override ProjectOperation ReserveProject(
      Guid projectId,
      string projectName,
      Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectCreate");
      this.BindGuid("@projectId", projectId);
      this.BindProjectName("@projectName", projectName);
      this.BindProjectAbbreviation("@projectAbbreviation", (string) null);
      this.BindString("@projectState", "CreatePending", 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullValue("@pendingProjectId", SqlDbType.UniqueIdentifier);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      this.BindDescription("@projectDescription", (string) null);
      this.BindProjectVisibility("@projectVisibility", new byte?());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        return resultCollection.GetCurrent<ProjectOperation>().Items.First<ProjectOperation>();
      }
    }

    internal override ProjectOperation CreateProject(
      Guid projectId,
      string projectName,
      string projectAbbreviation,
      string projectDescription,
      Guid pendingProjectGuid,
      Guid userIdentity,
      ProjectVisibility projectVisibility)
    {
      this.PrepareStoredProcedure("prc_ProjectCreate");
      this.BindGuid("@projectId", projectId);
      this.BindProjectName("@projectName", projectName);
      this.BindProjectAbbreviation("@projectAbbreviation", projectAbbreviation);
      this.BindString("@projectState", (string) null, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@pendingProjectId", pendingProjectGuid);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      this.BindDescription("@projectDescription", projectDescription);
      this.BindProjectVisibility("@projectVisibility", new byte?((byte) projectVisibility));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        return resultCollection.GetCurrent<ProjectOperation>().Items.First<ProjectOperation>();
      }
    }

    internal override ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject,
      bool mergeProperties = false)
    {
      this.PrepareUpdate(project, userIdentity, mergeProperties);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        updatedProject = this.GetProject(rc);
        rc.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        rc.NextResult();
        return rc.GetCurrent<ProjectOperation>().Items.First<ProjectOperation>();
      }
    }

    internal override ProjectOperation DeleteProject(Guid projectId, Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectDelete");
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        return resultCollection.GetCurrent<ProjectOperation>().Items.First<ProjectOperation>();
      }
    }

    protected override void BindMinRevision(string paramName, long revision) => this.BindLong(paramName, revision);

    protected override void BindModifiedRevision(string paramName, long revision) => this.BindLong(paramName, revision);

    protected override void BindDeletedRevision(string paramName, long revision) => this.BindLong(paramName, revision);

    protected override ObjectBinder<ProjectInfo> CreateProjectInfoColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectInfoColumns2();

    protected override ObjectBinder<ProjectInfo> CreateProjectHistoryColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectHistoryColumns2();

    private static SqlParameter BindRevisionTable(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IDictionary<Guid, long> revisions)
    {
      IDictionary<Guid, long> source = revisions ?? (IDictionary<Guid, long>) new Dictionary<Guid, long>();
      System.Func<KeyValuePair<Guid, long>, SqlDataRecord> selector = (System.Func<KeyValuePair<Guid, long>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectComponent8.typ_ProjectRevisionTable);
        sqlDataRecord.SetGuid(0, row.Key);
        sqlDataRecord.SetInt64(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_ProjectRevisionTable", source.Select<KeyValuePair<Guid, long>, SqlDataRecord>(selector));
    }
  }
}
