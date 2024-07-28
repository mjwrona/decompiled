// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component69
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component69 : Build2Component68
  {
    public override async Task<RetentionLease> AddRetentionLease(
      Guid projectId,
      string owner,
      int buildId,
      int definitionId,
      DateTime validUntil,
      bool highPriority,
      int maxLeases)
    {
      Build2Component69 build2Component69 = this;
      RetentionLease retentionLease;
      using (build2Component69.TraceScope(method: nameof (AddRetentionLease)))
      {
        build2Component69.PrepareStoredProcedure("Build.prc_AddRetentionLease");
        build2Component69.BindInt("@dataspaceId", build2Component69.GetDataspaceId(projectId));
        build2Component69.BindString("@ownerId", owner, 400, false, SqlDbType.NVarChar);
        build2Component69.BindInt("@runId", buildId);
        build2Component69.BindInt("@definitionId", definitionId);
        build2Component69.BindDate("@validUntil", validUntil);
        build2Component69.BindBoolean("@highPriority", highPriority);
        build2Component69.BindInt("@maxLeases", maxLeases);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component69.ExecuteReaderAsync(), build2Component69.ProcedureName, build2Component69.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component69.GetRetentionLeaseBinder());
          retentionLease = resultCollection.GetCurrent<RetentionLease>().Items.SingleOrDefault<RetentionLease>();
        }
      }
      return retentionLease;
    }
  }
}
