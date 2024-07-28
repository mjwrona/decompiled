// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent7
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent7 : ExternalConnectionSqlComponent6
  {
    public override void UpdateExternalRepositories(
      string providerKey,
      IEnumerable<ExternalGitRepo> externalRepos)
    {
      this.PrepareStoredProcedure("prc_SaveExternalRepositories");
      this.BindExternalRepositoryTable("@externalRepos", externalRepos);
      this.BindString("@providerKey", providerKey, 400, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override void RemoveExternalRepositoriesFromConnections(
      string providerKey,
      IEnumerable<string> externalRepoIds)
    {
      this.PrepareStoredProcedure("prc_RemoveExternalRepositoriesFromConnections");
      this.BindStringTable("@externalRepoIds", externalRepoIds);
      this.BindString("@providerKey", providerKey, 400, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
