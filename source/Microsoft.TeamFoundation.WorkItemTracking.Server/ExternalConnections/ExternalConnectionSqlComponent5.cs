// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.DataModels.ComponentModels;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent5 : ExternalConnectionSqlComponent4
  {
    protected virtual ExternalConnectionSqlComponent2.SaveConnectionBinder GetSaveConnectionBinder() => new ExternalConnectionSqlComponent2.SaveConnectionBinder();

    public override Guid SaveExternalConnection(
      Guid projectId,
      string providerKey,
      string connectionName,
      Guid serviceEndpointId,
      IEnumerable<ExternalGitRepo> externalRepos,
      Guid createdBy)
    {
      this.PrepareStoredProcedure("prc_SaveExternalConnection");
      this.BindProjectId(projectId);
      this.BindString("@connectionName", connectionName, 100, false, SqlDbType.NVarChar);
      this.BindGuid("@serviceEndpointId", serviceEndpointId);
      this.BindExternalRepositoryTable("@externalRepos", externalRepos);
      this.BindString("@providerKey", providerKey, 400, false, SqlDbType.NVarChar);
      this.BindGuid("@createdBy", createdBy);
      return this.ExecuteUnknown<Guid>((System.Func<IDataReader, Guid>) (reader =>
      {
        SaveConnectionResult connectionResult = reader.Read() ? this.GetSaveConnectionBinder().Bind(reader) : throw new InvalidProviderKeyException(providerKey);
        return connectionResult.CreationStatus != -1 ? connectionResult.ConnectionId : throw new InvalidProviderKeyException(providerKey);
      }));
    }

    public override void DeleteExternalConnection(Guid projectId, Guid connectionId)
    {
      this.PrepareStoredProcedure("prc_DeleteExternalConnection");
      this.BindProjectId(projectId);
      this.BindGuid("@connectionId", connectionId);
      this.ExecuteNonQuery();
    }

    public override ExternalConnectionDataset GetExternalConnectionDataset(
      Guid projectId,
      Guid connectionId,
      bool includeRepoProviderData = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnection");
      this.BindProjectId(projectId);
      this.BindGuid("@connectionId", connectionId);
      this.BindBoolean("@includeRepoProviderData", includeRepoProviderData);
      return this.ExecuteUnknown<ExternalConnectionDataset>((System.Func<IDataReader, ExternalConnectionDataset>) (reader =>
      {
        if (!reader.Read())
          return (ExternalConnectionDataset) null;
        ExternalConnectionDataset connectionDataset = this.GetExternalConnectionDatasetBinder().Bind(reader);
        reader.NextResult();
        List<ExternalConnectionRepository> list1 = this.GetExternalRepositoryBinder().BindAll(reader).ToList<ExternalConnectionRepository>();
        reader.NextResult();
        List<ExternalRepositoryData> list2 = this.GetRepoDataBinder().BindAll(reader).ToList<ExternalRepositoryData>();
        connectionDataset.Repos = list1.GroupJoin((IEnumerable<ExternalRepositoryData>) list2, (System.Func<ExternalConnectionRepository, Guid>) (repo => repo.RepositoryId), (System.Func<ExternalRepositoryData, Guid>) (data => data.RepositoryId), (repo, data) => new
        {
          repo = repo,
          data = data
        }).SelectMany(repoLeftJoinData => repoLeftJoinData.data.DefaultIfEmpty<ExternalRepositoryData>(), (repo, data) =>
        {
          repo.repo.Metadata = data?.Metadata;
          return repo.repo;
        });
        return connectionDataset;
      }));
    }
  }
}
