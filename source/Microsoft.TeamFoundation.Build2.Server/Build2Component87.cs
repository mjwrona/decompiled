// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component87
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
  internal class Build2Component87 : Build2Component86
  {
    public override async Task<RetentionLease> UpdateRetentionLease(
      Guid projectId,
      int leaseId,
      DateTime? validUntil,
      bool? highPriority)
    {
      Build2Component87 build2Component87 = this;
      RetentionLease retentionLease;
      using (build2Component87.TraceScope(method: nameof (UpdateRetentionLease)))
      {
        build2Component87.PrepareStoredProcedure("Build.prc_UpdateRetentionLease");
        build2Component87.BindInt("@dataspaceId", build2Component87.GetDataspaceId(projectId));
        build2Component87.BindInt("@leaseId", leaseId);
        build2Component87.BindNullableDate("@validUntil", validUntil);
        build2Component87.BindNullableBoolean("@highPriority", highPriority);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component87.ExecuteReaderAsync(), build2Component87.ProcedureName, build2Component87.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component87.GetRetentionLeaseBinder());
          retentionLease = resultCollection.GetCurrent<RetentionLease>().Items.SingleOrDefault<RetentionLease>();
        }
      }
      return retentionLease;
    }

    protected override sealed BuildQueryOrder CollapseToOldQueryOrderValues(
      BuildQueryOrder queryOrder)
    {
      return queryOrder;
    }
  }
}
