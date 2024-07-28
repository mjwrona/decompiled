// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.DataModels.ComponentModels;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent2 : ExternalConnectionSqlComponent
  {
    private static readonly SqlMetaData[] typ_ExternalRepositoryMetadataTable = new SqlMetaData[8]
    {
      new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ExternalId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RepositoryName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("IsPrivate", SqlDbType.Bit),
      new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("WebUrl", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("AdditionalProperties", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("Metadata", SqlDbType.NVarChar, -1L)
    };

    public override ExternalConnectionDataset GetExternalConnectionDataset(
      Guid projectId,
      string providerKey,
      string connectionName,
      bool includeRepoProviderData = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnection");
      this.BindProjectId(projectId);
      this.BindString("@providerKey", providerKey, 400, false, SqlDbType.NVarChar);
      this.BindString("@connectionName", connectionName, 100, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeRepoProviderData", includeRepoProviderData);
      return this.ExecuteUnknown<ExternalConnectionDataset>((System.Func<IDataReader, ExternalConnectionDataset>) (reader =>
      {
        if (!reader.Read())
          return (ExternalConnectionDataset) null;
        ExternalConnectionDataset dataset = this.GetExternalConnectionDatasetBinder().Bind(reader);
        reader.NextResult();
        List<ExternalConnectionRepository> list1 = this.GetExternalRepositoryBinder().BindAll(reader).Where<ExternalConnectionRepository>((System.Func<ExternalConnectionRepository, bool>) (m => m.ConnectionId == dataset.ConnectionId)).ToList<ExternalConnectionRepository>();
        reader.NextResult();
        List<ExternalRepositoryData> list2 = this.GetRepoDataBinder().BindAll(reader).Where<ExternalRepositoryData>((System.Func<ExternalRepositoryData, bool>) (r => r.ConnectionId == dataset.ConnectionId)).ToList<ExternalRepositoryData>();
        dataset.Repos = list1.GroupJoin((IEnumerable<ExternalRepositoryData>) list2, (System.Func<ExternalConnectionRepository, Guid>) (repo => repo.RepositoryId), (System.Func<ExternalRepositoryData, Guid>) (data => data.RepositoryId), (repo, data) => new
        {
          repo = repo,
          data = data
        }).SelectMany(repoLeftJoinData => repoLeftJoinData.data.DefaultIfEmpty<ExternalRepositoryData>(), (repo, data) =>
        {
          repo.repo.Metadata = data?.Metadata;
          return repo.repo;
        });
        return dataset;
      }));
    }

    public override IEnumerable<ExternalConnectionDataset> GetExternalConnectionsDataset(
      Guid? projectId,
      string providerKey,
      bool includeDeleted = false,
      bool includeRepoProviderData = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnections");
      if (projectId.HasValue)
        this.BindProjectId(projectId.Value);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindBoolean("@includeRepoProviderData", includeRepoProviderData);
      this.BindString("@providerKey", providerKey, 400, true, SqlDbType.NVarChar);
      return this.ExecuteUnknown<IEnumerable<ExternalConnectionDataset>>((System.Func<IDataReader, IEnumerable<ExternalConnectionDataset>>) (reader =>
      {
        List<ExternalConnectionDataset> list = this.GetExternalConnectionDatasetBinder().BindAll(reader).ToList<ExternalConnectionDataset>();
        reader.NextResult();
        Dictionary<Guid, IEnumerable<ExternalConnectionRepository>> dictionary1 = this.GetExternalRepositoryBinder().BindAll(reader).GroupBy<ExternalConnectionRepository, Guid>((System.Func<ExternalConnectionRepository, Guid>) (m => m.ConnectionId)).ToDictionary<IGrouping<Guid, ExternalConnectionRepository>, Guid, IEnumerable<ExternalConnectionRepository>>((System.Func<IGrouping<Guid, ExternalConnectionRepository>, Guid>) (mg => mg.Key), (System.Func<IGrouping<Guid, ExternalConnectionRepository>, IEnumerable<ExternalConnectionRepository>>) (mg => mg.AsEnumerable<ExternalConnectionRepository>()));
        reader.NextResult();
        Dictionary<Guid, IEnumerable<ExternalRepositoryData>> dictionary2 = this.GetRepoDataBinder().BindAll(reader).GroupBy<ExternalRepositoryData, Guid>((System.Func<ExternalRepositoryData, Guid>) (r => r.ConnectionId)).ToDictionary<IGrouping<Guid, ExternalRepositoryData>, Guid, IEnumerable<ExternalRepositoryData>>((System.Func<IGrouping<Guid, ExternalRepositoryData>, Guid>) (rd => rd.Key), (System.Func<IGrouping<Guid, ExternalRepositoryData>, IEnumerable<ExternalRepositoryData>>) (rd => rd.AsEnumerable<ExternalRepositoryData>()));
        foreach (ExternalConnectionDataset connectionDataset in list)
        {
          IEnumerable<ExternalConnectionRepository> outer;
          dictionary1.TryGetValue(connectionDataset.ConnectionId, out outer);
          outer = outer ?? Enumerable.Empty<ExternalConnectionRepository>();
          IEnumerable<ExternalRepositoryData> inner;
          dictionary2.TryGetValue(connectionDataset.ConnectionId, out inner);
          inner = inner ?? Enumerable.Empty<ExternalRepositoryData>();
          connectionDataset.Repos = outer.GroupJoin(inner, (System.Func<ExternalConnectionRepository, Guid>) (repo => repo.RepositoryId), (System.Func<ExternalRepositoryData, Guid>) (data => data.RepositoryId), (repo, data) => new
          {
            repo = repo,
            data = data
          }).SelectMany(repoLeftJoinData => repoLeftJoinData.data.DefaultIfEmpty<ExternalRepositoryData>(), (repo, data) =>
          {
            repo.repo.Metadata = data?.Metadata;
            return repo.repo;
          });
        }
        return (IEnumerable<ExternalConnectionDataset>) list;
      }));
    }

    public override void SaveExternalRepositoryData(
      IEnumerable<ExternalConnectionRepository> externalConnectionRepos)
    {
      this.PrepareStoredProcedure("prc_SaveExternalRepositoryData");
      this.BindExternalRepositoryMetadataTable("@externalRepoData", externalConnectionRepos);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<ExternalConnectionRepository> GetExternalRepositories(
      IEnumerable<Guid> repositoryIds = null)
    {
      this.PrepareStoredProcedure("prc_GetExternalRepositories");
      this.BindGuidTable("@repositoryIds", repositoryIds);
      return this.ExecuteUnknown<IEnumerable<ExternalConnectionRepository>>((System.Func<IDataReader, IEnumerable<ExternalConnectionRepository>>) (reader => (IEnumerable<ExternalConnectionRepository>) this.GetExternalRepositoryBinder().BindAll(reader).ToList<ExternalConnectionRepository>() ?? Enumerable.Empty<ExternalConnectionRepository>()));
    }

    public override IEnumerable<ExternalProvider> GetExternalProviders()
    {
      this.PrepareStoredProcedure("prc_GetExternalProviders");
      return this.ExecuteUnknown<IEnumerable<ExternalProvider>>((System.Func<IDataReader, IEnumerable<ExternalProvider>>) (reader => (IEnumerable<ExternalProvider>) this.GetExternalProviderBinder().BindAll(reader).ToList<ExternalProvider>() ?? Enumerable.Empty<ExternalProvider>()));
    }

    public override ExternalProvider SaveExternalProvider(
      Guid? providerId,
      string providerKey,
      string providerType)
    {
      this.PrepareStoredProcedure("prc_SaveExternalProvider");
      if (providerId.HasValue)
        this.BindGuid("@providerId", providerId.Value);
      this.BindString("@providerKey", providerKey, 400, false, SqlDbType.NVarChar);
      this.BindString("@providerType", providerType, 100, false, SqlDbType.NVarChar);
      return this.ExecuteUnknown<ExternalProvider>((System.Func<IDataReader, ExternalProvider>) (reader => reader.Read() ? this.GetExternalProviderBinder().Bind(reader) : (ExternalProvider) null));
    }

    public override void DeleteExternalConnection(
      Guid projectId,
      string providerKey,
      string connectionName)
    {
      this.PrepareStoredProcedure("prc_DeleteExternalConnection");
      this.BindProjectId(projectId);
      this.BindString("@connectionName", connectionName, 100, false, SqlDbType.NVarChar);
      this.BindString("@providerKey", providerKey, 400, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    protected virtual ExternalConnectionSqlComponent2.RepoDataBinder GetRepoDataBinder() => new ExternalConnectionSqlComponent2.RepoDataBinder();

    protected override void BindExternalRepositoryTable(
      string parameterName,
      IEnumerable<ExternalGitRepo> externalRepos)
    {
      new ExternalConnectionSqlComponent2.ExternalRepositoryTable2(parameterName, externalRepos).BindTable((ExternalConnectionSqlComponent) this);
    }

    protected override void BindExternalRepositoryMetadataTable(
      string parameterName,
      IEnumerable<ExternalConnectionRepository> externalConnectionRepos)
    {
      new ExternalConnectionSqlComponent2.ExternalRepositoryMetadataTable(parameterName, externalConnectionRepos).BindTable((ExternalConnectionSqlComponent) this);
    }

    protected override ExternalConnectionSqlComponent.ExternalRepositoryBinder GetExternalRepositoryBinder() => (ExternalConnectionSqlComponent.ExternalRepositoryBinder) new ExternalConnectionSqlComponent2.ExternalRepositoryBinder2();

    protected virtual ExternalConnectionSqlComponent2.ExternalProviderBinder GetExternalProviderBinder() => new ExternalConnectionSqlComponent2.ExternalProviderBinder();

    protected override ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder GetExternalConnectionDatasetBinder() => (ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder) new ExternalConnectionSqlComponent2.ExternalConnectionDatasetBinder2();

    public override string ExternalRepositoryTableType => "typ_ExternalRepositoryTable2";

    public class ExternalRepositoryTable2 : ITableValuedParameter
    {
      private string m_parameterName;
      private IEnumerable<ExternalGitRepo> m_externalRepos;

      public ExternalRepositoryTable2(
        string parameterName,
        IEnumerable<ExternalGitRepo> externalRepos)
      {
        this.m_parameterName = parameterName;
        this.m_externalRepos = externalRepos;
      }

      public void BindTable(ExternalConnectionSqlComponent component) => component.Bind<ExternalGitRepo>(this.m_parameterName, this.m_externalRepos, ExternalConnectionSqlComponent2.typ_ExternalRepositoryMetadataTable, component.ExternalRepositoryTableType, (Action<SqlDataRecord, ExternalGitRepo>) ((record, repo) =>
      {
        if (!string.IsNullOrEmpty(repo.Id))
          record.SetString(1, repo.Id);
        else
          record.SetDBNull(1);
        if (!string.IsNullOrEmpty(repo.Name))
          record.SetString(2, repo.Name);
        else
          record.SetDBNull(2);
        record.SetBoolean(3, repo.IsPrivate);
        if (!string.IsNullOrEmpty(repo.Url))
          record.SetString(4, repo.Url);
        else
          record.SetDBNull(4);
        if (!string.IsNullOrEmpty(repo.WebUrl))
          record.SetString(5, repo.WebUrl);
        else
          record.SetDBNull(5);
        if (repo.AdditionalProperties != null)
          record.SetString(6, JsonConvert.SerializeObject((object) repo.AdditionalProperties));
        else
          record.SetDBNull(6);
      }));
    }

    public class ExternalRepositoryMetadataTable : ITableValuedParameter
    {
      private string m_parameterName;
      private IEnumerable<ExternalConnectionRepository> m_externalConnectionRepo;

      public ExternalRepositoryMetadataTable(
        string parameterName,
        IEnumerable<ExternalConnectionRepository> externalConnectionRepos)
      {
        this.m_parameterName = parameterName;
        this.m_externalConnectionRepo = externalConnectionRepos;
      }

      public void BindTable(ExternalConnectionSqlComponent component) => component.Bind<ExternalConnectionRepository>(this.m_parameterName, this.m_externalConnectionRepo, ExternalConnectionSqlComponent2.typ_ExternalRepositoryMetadataTable, component.ExternalRepositoryTableType, (Action<SqlDataRecord, ExternalConnectionRepository>) ((record, repo) =>
      {
        record.SetGuid(0, repo.RepositoryId);
        if (!string.IsNullOrEmpty(repo.ExternalId))
          record.SetString(1, repo.ExternalId);
        else
          record.SetDBNull(1);
        if (!string.IsNullOrEmpty(repo.RepositoryName))
          record.SetString(2, repo.RepositoryName);
        else
          record.SetDBNull(2);
        record.SetBoolean(3, repo.IsPrivate);
        if (!string.IsNullOrEmpty(repo.Url))
          record.SetString(4, repo.Url);
        else
          record.SetDBNull(4);
        if (!string.IsNullOrEmpty(repo.WebUrl))
          record.SetString(5, repo.WebUrl);
        else
          record.SetDBNull(5);
        if (repo.AdditionalProperties != null)
          record.SetString(6, JsonConvert.SerializeObject((object) repo.AdditionalProperties));
        else
          record.SetDBNull(6);
        if (!string.IsNullOrEmpty(repo.Metadata))
          record.SetString(7, repo.Metadata);
        else
          record.SetDBNull(7);
      }));
    }

    protected class ExternalConnectionDatasetBinder2 : 
      ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder
    {
      protected SqlColumnBinder m_providerKey = new SqlColumnBinder("ProviderKey");

      public override ExternalConnectionDataset Bind(IDataReader reader) => new ExternalConnectionDataset()
      {
        DataspaceId = this.m_dataspaceId.GetInt32(reader),
        ConnectionId = this.m_connectionId.GetGuid(reader),
        ProviderId = this.m_providerId.GetGuid(reader),
        ProviderKey = this.m_providerKey.GetString(reader, false),
        ConnectionName = this.m_connectionName.GetString(reader, false),
        ServiceEndpointId = this.m_serviceEndpointId.GetGuid(reader),
        IsValid = this.m_isValid.GetBoolean(reader),
        IsDeleted = this.m_isDeleted.GetBoolean(reader),
        CreatedDate = this.m_createdDate.GetDateTime(reader),
        CreatedBy = this.m_createdBy.GetGuid(reader, false),
        UpdatedDate = this.m_updatedDate.GetDateTime(reader)
      };
    }

    protected class ExternalRepositoryBinder2 : 
      ExternalConnectionSqlComponent.ExternalRepositoryBinder
    {
      protected SqlColumnBinder m_connectionName = new SqlColumnBinder("ConnectionName");
      protected SqlColumnBinder m_providerKey = new SqlColumnBinder("ProviderKey");
      protected SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");

      public override ExternalConnectionRepository Bind(IDataReader reader) => new ExternalConnectionRepository()
      {
        ConnectionId = this.m_connectionId.GetGuid(reader, true),
        ConnectionName = this.m_connectionName.GetString(reader, true),
        RepositoryId = this.m_repositoryId.GetGuid(reader),
        ProviderId = this.m_providerId.GetGuid(reader),
        ProviderKey = this.m_providerKey.GetString(reader, false),
        ExternalId = this.m_externalId.GetString(reader, false),
        RepositoryName = this.m_repositoryName.GetString(reader, false),
        IsPrivate = this.m_isPrivate.GetBoolean(reader),
        Url = this.m_url.GetString(reader, true),
        WebUrl = this.m_webUrl.GetString(reader, true),
        UpdatedDate = this.m_updatedDate.GetDateTime(reader),
        AdditionalProperties = this.m_additionalProperties.GetString(reader, true)
      };
    }

    protected class RepoDataBinder : WorkItemTrackingObjectBinder<ExternalRepositoryData>
    {
      protected SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");
      protected SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      protected SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");
      protected SqlColumnBinder m_updatedDate = new SqlColumnBinder("UpdatedDate");

      public override ExternalRepositoryData Bind(IDataReader reader) => new ExternalRepositoryData()
      {
        ConnectionId = this.m_connectionId.GetGuid(reader),
        RepositoryId = this.m_repositoryId.GetGuid(reader),
        Metadata = this.m_metadata.GetString(reader, false),
        UpdatedDate = this.m_updatedDate.GetDateTime(reader)
      };
    }

    protected class ExternalProviderBinder : WorkItemTrackingObjectBinder<ExternalProvider>
    {
      protected SqlColumnBinder m_providerId = new SqlColumnBinder("ProviderId");
      protected SqlColumnBinder m_providerKey = new SqlColumnBinder("ProviderKey");
      protected SqlColumnBinder m_providerType = new SqlColumnBinder("ProviderType");
      protected SqlColumnBinder m_provisionDate = new SqlColumnBinder("ProvisionDate");
      protected SqlColumnBinder m_updatedDate = new SqlColumnBinder("UpdatedDate");

      public override ExternalProvider Bind(IDataReader reader) => new ExternalProvider()
      {
        ProviderId = this.m_providerId.GetGuid(reader),
        ProviderKey = this.m_providerKey.GetString(reader, false),
        ProviderType = this.m_providerType.GetString(reader, false),
        ProvisionDate = this.m_provisionDate.GetDateTime(reader),
        UpdatedDate = this.m_updatedDate.GetDateTime(reader)
      };
    }

    protected class SaveConnectionBinder : WorkItemTrackingObjectBinder<SaveConnectionResult>
    {
      protected SqlColumnBinder m_status = new SqlColumnBinder("CreationStatus");
      protected SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");

      public override SaveConnectionResult Bind(IDataReader reader) => new SaveConnectionResult()
      {
        CreationStatus = this.m_status.GetInt32(reader),
        ConnectionId = this.m_connectionId.GetGuid(reader, false)
      };
    }

    protected class TelemetryDataBinder : 
      WorkItemTrackingObjectBinder<ExternalConnectionTelemetryData>
    {
      protected SqlColumnBinder m_externalConnectionCount = new SqlColumnBinder("ExternalConnectionCount");
      protected SqlColumnBinder m_externalRepoCount = new SqlColumnBinder("ExternalRepoCount");
      protected SqlColumnBinder m_externalRepoWithActivityCount = new SqlColumnBinder("ExternalRepoWithActivityCount");
      protected SqlColumnBinder m_githubPullRequestLinkCount = new SqlColumnBinder("GitHubPullRequestLinkCount");
      protected SqlColumnBinder m_githubCommitLinkCount = new SqlColumnBinder("GitHubCommitLinkCount");

      public override ExternalConnectionTelemetryData Bind(IDataReader reader) => new ExternalConnectionTelemetryData()
      {
        ExternalConnectionCount = this.m_externalConnectionCount.GetInt32(reader),
        ExternalRepoCount = this.m_externalRepoCount.GetInt32(reader),
        ExternalRepoWithActivityCount = this.m_externalRepoWithActivityCount.GetInt32(reader),
        GitHubPullRequestLinkCount = this.m_githubPullRequestLinkCount.GetInt32(reader),
        GitHubCommitLinkCount = this.m_githubCommitLinkCount.GetInt32(reader)
      };
    }
  }
}
