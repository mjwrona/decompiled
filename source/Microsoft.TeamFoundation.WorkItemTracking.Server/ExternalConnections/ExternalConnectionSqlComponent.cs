// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[9]
    {
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent>(1, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent2>(2, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent3>(3, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent4>(4, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent5>(5, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent6>(6, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent7>(7, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent8>(8, false),
      (IComponentCreator) new ComponentCreator<ExternalConnectionSqlComponent9>(9, false)
    }, "ExternalConnection", "WorkItem");

    public virtual ExternalConnectionDataset GetExternalConnectionDataset(
      Guid projectId,
      Guid connectionId,
      bool includeRepoProviderData = false)
    {
      throw new NotImplementedException();
    }

    public virtual ExternalConnectionDataset GetExternalConnectionDataset(
      Guid projectId,
      string providerKey,
      string connectionName,
      bool includeRepoProviderData = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnection");
      this.BindProjectId(projectId);
      this.BindString("@connectionName", connectionName, 100, false, SqlDbType.NVarChar);
      return this.ExecuteUnknown<ExternalConnectionDataset>((System.Func<IDataReader, ExternalConnectionDataset>) (reader =>
      {
        if (!reader.Read())
          return (ExternalConnectionDataset) null;
        ExternalConnectionDataset dataset = this.GetExternalConnectionDatasetBinder().Bind(reader);
        reader.NextResult();
        List<ExternalConnectionRepository> list = this.GetExternalRepositoryBinder().BindAll(reader).Where<ExternalConnectionRepository>((System.Func<ExternalConnectionRepository, bool>) (m => m.ConnectionId == dataset.ConnectionId)).ToList<ExternalConnectionRepository>();
        dataset.Repos = (IEnumerable<ExternalConnectionRepository>) list;
        return dataset;
      }));
    }

    public virtual IEnumerable<ExternalConnectionDataset> GetExternalConnectionsDataset(
      Guid? projectId,
      string providerKey,
      bool includeDeleted = false,
      bool includeRepoProviderData = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnections");
      if (projectId.HasValue)
        this.BindProjectId(projectId.Value);
      this.BindBoolean("@includeDeleted", includeDeleted);
      return this.ExecuteUnknown<IEnumerable<ExternalConnectionDataset>>((System.Func<IDataReader, IEnumerable<ExternalConnectionDataset>>) (reader =>
      {
        List<ExternalConnectionDataset> list = this.GetExternalConnectionDatasetBinder().BindAll(reader).ToList<ExternalConnectionDataset>();
        reader.NextResult();
        Dictionary<Guid, IEnumerable<ExternalConnectionRepository>> dictionary = this.GetExternalRepositoryBinder().BindAll(reader).GroupBy<ExternalConnectionRepository, Guid>((System.Func<ExternalConnectionRepository, Guid>) (m => m.ConnectionId)).ToDictionary<IGrouping<Guid, ExternalConnectionRepository>, Guid, IEnumerable<ExternalConnectionRepository>>((System.Func<IGrouping<Guid, ExternalConnectionRepository>, Guid>) (mg => mg.Key), (System.Func<IGrouping<Guid, ExternalConnectionRepository>, IEnumerable<ExternalConnectionRepository>>) (mg => mg.AsEnumerable<ExternalConnectionRepository>()));
        foreach (ExternalConnectionDataset connectionDataset in list)
        {
          IEnumerable<ExternalConnectionRepository> connectionRepositories;
          dictionary.TryGetValue(connectionDataset.ConnectionId, out connectionRepositories);
          connectionDataset.Repos = connectionRepositories ?? Enumerable.Empty<ExternalConnectionRepository>();
        }
        return (IEnumerable<ExternalConnectionDataset>) list;
      }));
    }

    public virtual Guid SaveExternalConnection(
      Guid projectId,
      string providerKey,
      string connectionName,
      Guid serviceEndpointId,
      IEnumerable<ExternalGitRepo> externalRepos,
      Guid createdBy)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateConnectionMetadata(
      Guid projectId,
      Guid connectionId,
      string connectionMetadata)
    {
    }

    public virtual void DeleteExternalConnection(
      Guid projectId,
      string providerKey,
      string connectionName)
    {
      this.PrepareStoredProcedure("prc_DeleteExternalConnection");
      this.BindProjectId(projectId);
      this.BindString("@connectionName", connectionName, 100, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteExternalConnection(Guid projectId, Guid connectionId) => throw new NotImplementedException();

    public virtual IEnumerable<ExternalConnectionDataset> GetExternalConnectionsDatasetByRepoExternalIds(
      Guid? projectId,
      string providerKey,
      IEnumerable<string> externalIds,
      bool includeDeleted = false)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ExternalConnectionDataset> GetExternalConnectionsDatasetByRepoInternalIds(
      Guid? projectId,
      IEnumerable<Guid> internalIds,
      bool includeDeleted = false)
    {
      throw new NotImplementedException();
    }

    public virtual void SaveExternalRepositoryData(
      IEnumerable<ExternalConnectionRepository> externalConnectionRepo)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ExternalConnectionRepository> GetExternalRepositories(
      IEnumerable<Guid> repositoryIds = null)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ExternalConnectionRepository> GetConnectedExternalRepositories(
      Guid projectId,
      int limit,
      string fromRepositoryName,
      string filter)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ExternalProvider> GetExternalProviders() => throw new NotImplementedException();

    public virtual ExternalProvider SaveExternalProvider(
      Guid? providerId,
      string providerKey,
      string providerType)
    {
      throw new NotImplementedException();
    }

    public virtual ExternalConnectionTelemetryData CollectectExternalConnectionTelemetry(
      int dataCollectionTimeFrameInDays)
    {
      throw new NotImplementedException();
    }

    protected virtual ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder GetExternalConnectionDatasetBinder() => new ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder();

    protected virtual ExternalConnectionSqlComponent.ExternalRepositoryBinder GetExternalRepositoryBinder() => new ExternalConnectionSqlComponent.ExternalRepositoryBinder();

    protected virtual SqlParameter BindProjectId(Guid projectId) => this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));

    protected virtual void BindExternalRepositoryTable(
      string parameterName,
      IEnumerable<ExternalGitRepo> externalRepos)
    {
      new ExternalConnectionSqlComponent.ExternalRepositoryTable(parameterName, externalRepos).BindTable(this);
    }

    protected virtual void BindExternalRepositoryMetadataTable(
      string parameterName,
      IEnumerable<ExternalConnectionRepository> externalConnectionRepos)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateExternalRepositories(
      string providerKey,
      IEnumerable<ExternalGitRepo> externalRepos)
    {
      throw new NotImplementedException();
    }

    public virtual void RemoveExternalRepositoriesFromConnections(
      string providerKey,
      IEnumerable<string> externalRepoIds)
    {
      throw new NotImplementedException();
    }

    public virtual string ExternalRepositoryTableType => "typ_ExternalRepositoryTable";

    public class ExternalRepositoryTable : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_ExternalRepositoryTable = new SqlMetaData[6]
      {
        new SqlMetaData("ExternalId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("RepositoryName", SqlDbType.NVarChar, 400L),
        new SqlMetaData("IsPrivate", SqlDbType.Bit),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("WebUrl", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("AdditionalProperties", SqlDbType.NVarChar, 4000L)
      };
      private string m_parameterName;
      private IEnumerable<ExternalGitRepo> m_externalRepos;

      public ExternalRepositoryTable(
        string parameterName,
        IEnumerable<ExternalGitRepo> externalRepos)
      {
        this.m_parameterName = parameterName;
        this.m_externalRepos = externalRepos;
      }

      public void BindTable(ExternalConnectionSqlComponent component) => component.Bind<ExternalGitRepo>(this.m_parameterName, this.m_externalRepos, ExternalConnectionSqlComponent.ExternalRepositoryTable.typ_ExternalRepositoryTable, component.ExternalRepositoryTableType, (Action<SqlDataRecord, ExternalGitRepo>) ((record, repo) =>
      {
        if (!string.IsNullOrEmpty(repo.Id))
          record.SetString(0, repo.Id);
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(repo.Name))
          record.SetString(1, repo.Name);
        else
          record.SetDBNull(1);
        record.SetBoolean(2, repo.IsPrivate);
        if (!string.IsNullOrEmpty(repo.Url))
          record.SetString(3, repo.Url);
        else
          record.SetDBNull(3);
        if (!string.IsNullOrEmpty(repo.WebUrl))
          record.SetString(4, repo.WebUrl);
        else
          record.SetDBNull(4);
        if (repo.AdditionalProperties != null)
          record.SetString(5, JsonConvert.SerializeObject((object) repo.AdditionalProperties));
        else
          record.SetDBNull(5);
      }));
    }

    protected class ExternalConnectionDatasetBinder : 
      WorkItemTrackingObjectBinder<ExternalConnectionDataset>
    {
      protected SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      protected SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");
      protected SqlColumnBinder m_providerId = new SqlColumnBinder("ProviderId");
      protected SqlColumnBinder m_connectionName = new SqlColumnBinder("ConnectionName");
      protected SqlColumnBinder m_serviceEndpointId = new SqlColumnBinder("ServiceEndpointId");
      protected SqlColumnBinder m_isValid = new SqlColumnBinder("IsValid");
      protected SqlColumnBinder m_isDeleted = new SqlColumnBinder("IsDeleted");
      protected SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
      protected SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
      protected SqlColumnBinder m_updatedDate = new SqlColumnBinder("UpdatedDate");

      public override ExternalConnectionDataset Bind(IDataReader reader) => new ExternalConnectionDataset()
      {
        DataspaceId = this.m_dataspaceId.GetInt32(reader),
        ConnectionId = this.m_connectionId.GetGuid(reader),
        ProviderId = this.m_providerId.GetGuid(reader),
        ConnectionName = this.m_connectionName.GetString(reader, false),
        ServiceEndpointId = this.m_serviceEndpointId.GetGuid(reader),
        IsValid = this.m_isValid.GetBoolean(reader),
        IsDeleted = this.m_isDeleted.GetBoolean(reader),
        CreatedDate = this.m_createdDate.GetDateTime(reader),
        CreatedBy = this.m_createdBy.GetGuid(reader, false),
        UpdatedDate = this.m_updatedDate.GetDateTime(reader)
      };
    }

    protected class ExternalRepositoryBinder : 
      WorkItemTrackingObjectBinder<ExternalConnectionRepository>
    {
      protected SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");
      protected SqlColumnBinder m_providerId = new SqlColumnBinder("ProviderId");
      protected SqlColumnBinder m_externalId = new SqlColumnBinder("ExternalId");
      protected SqlColumnBinder m_repositoryName = new SqlColumnBinder("RepositoryName");
      protected SqlColumnBinder m_isPrivate = new SqlColumnBinder("IsPrivate");
      protected SqlColumnBinder m_url = new SqlColumnBinder("Url");
      protected SqlColumnBinder m_webUrl = new SqlColumnBinder("WebUrl");
      protected SqlColumnBinder m_additionalProperties = new SqlColumnBinder("AdditionalProperties");
      protected SqlColumnBinder m_updatedDate = new SqlColumnBinder("UpdatedDate");

      public override ExternalConnectionRepository Bind(IDataReader reader) => new ExternalConnectionRepository()
      {
        ConnectionId = this.m_connectionId.GetGuid(reader),
        ProviderId = this.m_providerId.GetGuid(reader),
        ExternalId = this.m_externalId.GetString(reader, false),
        RepositoryName = this.m_repositoryName.GetString(reader, false),
        IsPrivate = this.m_isPrivate.GetBoolean(reader),
        Url = this.m_url.GetString(reader, true),
        WebUrl = this.m_webUrl.GetString(reader, true),
        UpdatedDate = this.m_updatedDate.GetDateTime(reader),
        AdditionalProperties = this.m_additionalProperties.GetString(reader, true)
      };
    }
  }
}
