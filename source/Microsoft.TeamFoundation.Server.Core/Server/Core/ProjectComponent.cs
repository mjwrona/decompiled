// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

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
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  internal class ProjectComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[15]
    {
      (IComponentCreator) new ComponentCreator<ProjectComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ProjectComponent2>(2),
      (IComponentCreator) new ComponentCreator<ProjectComponent3>(3),
      (IComponentCreator) new ComponentCreator<ProjectComponent4>(4),
      (IComponentCreator) new ComponentCreator<ProjectComponent5>(5),
      (IComponentCreator) new ComponentCreator<ProjectComponent6>(6),
      (IComponentCreator) new ComponentCreator<ProjectComponent7>(7),
      (IComponentCreator) new ComponentCreator<ProjectComponent8>(8),
      (IComponentCreator) new ComponentCreator<ProjectComponent9>(9),
      (IComponentCreator) new ComponentCreator<ProjectComponent10>(10),
      (IComponentCreator) new ComponentCreator<ProjectComponent11>(11),
      (IComponentCreator) new ComponentCreator<ProjectComponent12>(12),
      (IComponentCreator) new ComponentCreator<ProjectComponent13>(13),
      (IComponentCreator) new ComponentCreator<ProjectComponent14>(14),
      (IComponentCreator) new ComponentCreator<ProjectComponent15>(15)
    }, "Project");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static ProjectComponent()
    {
      ProjectComponent.s_sqlExceptionFactories.Add(450001, new SqlExceptionFactory(typeof (ProjectDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectDoesNotExistException(sqEr.ExtractString("project_id")))));
      ProjectComponent.s_sqlExceptionFactories.Add(450004, new SqlExceptionFactory(typeof (ProjectAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectAlreadyExistsException(sqEr.ExtractString("project_name")))));
      ProjectComponent.s_sqlExceptionFactories.Add(450013, new SqlExceptionFactory(typeof (ProjectNameNotRecognizedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectNameNotRecognizedException(sqEr.ExtractString("project_name")))));
      ProjectComponent.s_sqlExceptionFactories.Add(440000, new SqlExceptionFactory(typeof (ProjectDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectDoesNotExistException(sqEr.ExtractString("project_id")))));
      ProjectComponent.s_sqlExceptionFactories.Add(440002, new SqlExceptionFactory(typeof (ProjectAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectAlreadyExistsException(sqEr.ExtractString("project_name")))));
      ProjectComponent.s_sqlExceptionFactories.Add(440001, new SqlExceptionFactory(typeof (ProjectNameNotRecognizedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectNameNotRecognizedException(sqEr.ExtractString("project_name")))));
      ProjectComponent.s_sqlExceptionFactories.Add(440003, new SqlExceptionFactory(typeof (ProjectWorkPendingException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectWorkPendingException(sqEr.ExtractString("project_name")))));
    }

    internal virtual ProjectInfo GetProject(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_css_get_project");
      this.BindGuid("@project_id", projectId);
      ProjectInfo project;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        project = resultCollection.GetCurrent<ProjectInfo>().Items.FirstOrDefault<ProjectInfo>();
      }
      if (project == null)
        throw new ProjectDoesNotExistException(projectId.ToString());
      project.Properties = this.GetProjectProperties(projectId);
      return project;
    }

    internal virtual IList<ProjectInfo> GetProjects()
    {
      this.PrepareStoredProcedure("prc_css_get_projects");
      IList<ProjectInfo> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        items = (IList<ProjectInfo>) resultCollection.GetCurrent<ProjectInfo>().Items;
      }
      foreach (ProjectInfo projectInfo in (IEnumerable<ProjectInfo>) items)
        projectInfo.Properties = this.GetProjectProperties(projectInfo.Id);
      return items;
    }

    internal virtual IList<ProjectInfo> GetProjects(IEnumerable<Guid> projectIds) => throw new NotImplementedException();

    internal virtual List<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision = 0)
    {
      throw new NotImplementedException();
    }

    internal virtual IList<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      long minRevision = 0)
    {
      return (IList<ProjectInfo>) new List<ProjectInfo>();
    }

    internal virtual Dictionary<Guid, IEnumerable<ProjectOperation>> GetUnpublishedProjectChanges() => throw new NotImplementedException();

    internal virtual IDictionary<Guid, long> GetProjectWatermarks(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      throw new NotImplementedException();
    }

    internal virtual void UpdateProjectWatermarks(IDictionary<Guid, long> revisions) => throw new NotImplementedException();

    internal virtual bool HasUnacknowledgedProjectChanges(string projectName) => false;

    internal virtual void GetChangedProjects(
      IVssRequestContext requestContext,
      long modifiedRevision,
      long deletedRevision,
      out IList<ProjectInfo> modifiedProjects,
      out IList<ProjectInfo> deletedProjects)
    {
      throw new NotImplementedException();
    }

    internal virtual Tuple<IEnumerable<ProjectInfo>, IEnumerable<ProjectInfo>> QueryForUnpublishedProjects(
      IVssRequestContext requestContext,
      byte[] modifiedWatermark,
      byte[] deletedWatermark)
    {
      throw new NotImplementedException();
    }

    internal virtual IList<ProjectProperty> GetProjectProperties(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_css_get_project_properties");
      this.BindGuid("@project_id", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        resultCollection.AddBinder<ProjectProperty>((ObjectBinder<ProjectProperty>) new ProjectPropertyColumns());
        resultCollection.NextResult();
        return (IList<ProjectProperty>) resultCollection.GetCurrent<ProjectProperty>().Items;
      }
    }

    internal virtual ProjectOperation ReserveProject(
      Guid pendingProjectGuid,
      string projectName,
      Guid userIdentity)
    {
      throw new NotImplementedException();
    }

    internal virtual ProjectOperation DeleteReservedProject(
      Guid pendingProjectGuid,
      Guid userIdentity)
    {
      throw new NotImplementedException();
    }

    internal virtual ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      Guid adminGroupId,
      string nodes,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_create_project");
      this.BindGuid("@project_id", projectId);
      this.BindProjectName("@project_name", projectName);
      this.BindGuid("@admin_id", adminGroupId);
      this.BindString("@nodes", nodes, 0, false, SqlDbType.NVarChar);
      this.BindGuid("@user_id", requestingIdentity.TeamFoundationId);
      this.BindDateTime("@time_stamp", timeStamp);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SequenceIdColumns());
        nodeSeqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
      return new ProjectInfo(projectId, projectName, ProjectState.New);
    }

    internal virtual ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      Guid adminGroupId,
      string nodes,
      DateTime timeStamp,
      Guid pendingProjectGuid,
      out int nodeSeqId)
    {
      throw new NotImplementedException();
    }

    internal virtual ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      string projectAbbreviation,
      string nodes,
      Guid pendingProjectGuid,
      out int nodeSeqId)
    {
      throw new NotImplementedException();
    }

    internal virtual ProjectOperation CreateProject(
      Guid projectId,
      string projectName,
      string projectAbbreviation,
      string projectDescription,
      Guid pendingProjectGuid,
      Guid userIdentity,
      ProjectVisibility projectVisibility)
    {
      throw new NotImplementedException();
    }

    internal virtual void DeleteProject(
      Guid projectId,
      string userName,
      DateTime timeStamp,
      out int seqId)
    {
      this.PrepareStoredProcedure("prc_css_delete_project");
      this.BindGuid("@project_id", projectId);
      this.BindString("@user_name", userName, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@time_stamp", timeStamp);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SequenceIdColumns());
        seqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
    }

    internal virtual void DeleteProject(Guid projectId, string userName, out int nodeSeqId) => throw new NotImplementedException();

    internal virtual ProjectOperation DeleteProject(Guid projectId, Guid userIdentity) => throw new NotImplementedException();

    internal virtual void SetProjectProperty(Guid projectId, string name, string value)
    {
      this.PrepareStoredProcedure("prc_css_set_project_property");
      this.BindGuid("@project_id", projectId);
      this.BindString("@name", name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@value", value, 0, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal virtual ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject,
      bool mergeProperties = false)
    {
      IList<KeyValuePair<string, string>> rows = (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
      if (project.Properties != null)
      {
        foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) project.Properties)
        {
          if (!TFStringComparer.CssProjectPropertyName.Equals("TemplateName", property.Name) && !string.IsNullOrEmpty((string) property.Value))
            rows.Add(new KeyValuePair<string, string>(property.Name, (string) property.Value));
        }
      }
      this.PrepareStoredProcedure("prc_css_set_project_properties");
      this.BindGuid("@project_id", project.Id);
      this.BindString("@state", project.State.ToString(), 64, false, SqlDbType.NVarChar);
      this.BindDateTime("@time_stamp", DateTime.UtcNow);
      this.BindKeyValuePairStringTable("@properties", (IEnumerable<KeyValuePair<string, string>>) rows);
      this.BindBoolean("@fDelete", project.Properties != null);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        updatedProject = resultCollection.GetCurrent<ProjectInfo>().Items.FirstOrDefault<ProjectInfo>();
      }
      return (ProjectOperation) null;
    }

    internal virtual ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject)
    {
      throw new NotImplementedException();
    }

    protected virtual ObjectBinder<ProjectInfo> CreateProjectInfoColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectInfoColumns();

    protected virtual ObjectBinder<ProjectInfo> CreateProjectWatermarkColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectWatermarkColumns();

    protected virtual ObjectBinder<ProjectInfo> CreateProjectHistoryColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectHistoryColumns();

    protected SqlParameter BindProjectName(string parameterName, string projectName) => this.BindString(parameterName, DBPath.UserToDatabasePath(projectName), (int) byte.MaxValue, false, SqlDbType.NVarChar);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ProjectComponent.s_sqlExceptionFactories;
  }
}
