// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component93
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component93 : Build2Component92
  {
    public override IList<Guid> GetAllServiceConnectionsForRepoAndProject(
      Guid projectId,
      string repoId,
      string repoType,
      int triggerFilter)
    {
      using (this.TraceScope(method: nameof (GetAllServiceConnectionsForRepoAndProject)))
      {
        this.PrepareStoredProcedure("Build.prc_GetAllServiceConnectionsForRepoAndProject");
        this.BindString("@repoId", repoId, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@repoType", repoType, 40, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@triggerFilter", triggerFilter);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GuidBinder());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
    }
  }
}
