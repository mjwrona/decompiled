// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent9
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent9 : ExternalConnectionSqlComponent8
  {
    public override IEnumerable<ExternalConnectionRepository> GetConnectedExternalRepositories(
      Guid projectId,
      int limit,
      string fromRepositoryName,
      string filter)
    {
      this.PrepareStoredProcedure("prc_GetConnectedExternalRepositories");
      this.BindProjectId(projectId);
      this.BindInt("@limit", limit);
      this.BindString("@fromRepositoryName", fromRepositoryName, 400, true, SqlDbType.NVarChar);
      this.BindString("@filter", filter, -1, true, SqlDbType.NVarChar);
      return this.ExecuteUnknown<IEnumerable<ExternalConnectionRepository>>((System.Func<IDataReader, IEnumerable<ExternalConnectionRepository>>) (reader => (IEnumerable<ExternalConnectionRepository>) this.GetExternalRepositoryBinder().BindAll(reader).ToList<ExternalConnectionRepository>() ?? Enumerable.Empty<ExternalConnectionRepository>()));
    }
  }
}
