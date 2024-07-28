// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent4
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent4 : ExternalConnectionSqlComponent3
  {
    public override IEnumerable<ExternalConnectionDataset> GetExternalConnectionsDatasetByRepoExternalIds(
      Guid? projectId,
      string providerKey,
      IEnumerable<string> externalIds,
      bool includeDeleted = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnectionsByRepoExternalIds");
      if (projectId.HasValue)
        this.BindProjectId(projectId.Value);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindStringTable("@externalIds", externalIds);
      this.BindString("@providerKey", providerKey, 400, true, SqlDbType.NVarChar);
      return this.ExecuteUnknown<IEnumerable<ExternalConnectionDataset>>((System.Func<IDataReader, IEnumerable<ExternalConnectionDataset>>) (reader =>
      {
        List<ExternalConnectionDataset> list = this.GetExternalConnectionDatasetBinder().BindAll(reader).ToList<ExternalConnectionDataset>();
        reader.NextResult();
        Dictionary<Guid, IEnumerable<ExternalConnectionRepository>> dictionary = this.GetExternalRepositoryBinder().BindAll(reader).GroupBy<ExternalConnectionRepository, Guid>((System.Func<ExternalConnectionRepository, Guid>) (m => m.ConnectionId)).ToDictionary<IGrouping<Guid, ExternalConnectionRepository>, Guid, IEnumerable<ExternalConnectionRepository>>((System.Func<IGrouping<Guid, ExternalConnectionRepository>, Guid>) (mg => mg.Key), (System.Func<IGrouping<Guid, ExternalConnectionRepository>, IEnumerable<ExternalConnectionRepository>>) (mg => mg.AsEnumerable<ExternalConnectionRepository>()));
        foreach (ExternalConnectionDataset connectionDataset in list)
        {
          IEnumerable<ExternalConnectionRepository> connectionRepositories;
          dictionary.TryGetValue(connectionDataset.ConnectionId, out connectionRepositories);
          connectionDataset.Repos = connectionRepositories;
        }
        return (IEnumerable<ExternalConnectionDataset>) list;
      }));
    }

    public override IEnumerable<ExternalConnectionDataset> GetExternalConnectionsDatasetByRepoInternalIds(
      Guid? projectId,
      IEnumerable<Guid> internalIds,
      bool includeDeleted = false)
    {
      this.PrepareStoredProcedure("prc_GetExternalConnectionsByRepoInternalIds");
      if (projectId.HasValue)
        this.BindProjectId(projectId.Value);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindGuidTable("@internalIds", internalIds);
      return this.ExecuteUnknown<IEnumerable<ExternalConnectionDataset>>((System.Func<IDataReader, IEnumerable<ExternalConnectionDataset>>) (reader =>
      {
        List<ExternalConnectionDataset> list = this.GetExternalConnectionDatasetBinder().BindAll(reader).ToList<ExternalConnectionDataset>();
        reader.NextResult();
        Dictionary<Guid, IEnumerable<ExternalConnectionRepository>> dictionary = this.GetExternalRepositoryBinder().BindAll(reader).GroupBy<ExternalConnectionRepository, Guid>((System.Func<ExternalConnectionRepository, Guid>) (m => m.ConnectionId)).ToDictionary<IGrouping<Guid, ExternalConnectionRepository>, Guid, IEnumerable<ExternalConnectionRepository>>((System.Func<IGrouping<Guid, ExternalConnectionRepository>, Guid>) (mg => mg.Key), (System.Func<IGrouping<Guid, ExternalConnectionRepository>, IEnumerable<ExternalConnectionRepository>>) (mg => mg.AsEnumerable<ExternalConnectionRepository>()));
        foreach (ExternalConnectionDataset connectionDataset in list)
        {
          IEnumerable<ExternalConnectionRepository> connectionRepositories;
          dictionary.TryGetValue(connectionDataset.ConnectionId, out connectionRepositories);
          connectionDataset.Repos = connectionRepositories;
        }
        return (IEnumerable<ExternalConnectionDataset>) list;
      }));
    }
  }
}
