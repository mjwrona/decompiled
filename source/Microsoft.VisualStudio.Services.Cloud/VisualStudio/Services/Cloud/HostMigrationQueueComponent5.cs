// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationQueueComponent5
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationQueueComponent5 : HostMigrationQueueComponent4
  {
    public override List<HostMigrationRequest> GetSchedulableQueueRequestsSegmented(
      DateTime maxLastUserAccess,
      int numberOfRequests,
      int maxPriority,
      Guid minHostId,
      string[] excludedTargetInstances)
    {
      this.PrepareStoredProcedure("Migration.prc_GetSchedulableQueueRequestsSegmented");
      this.BindDateTime("@maxLastUserAccess", maxLastUserAccess);
      this.BindInt("@numberOfRequests", numberOfRequests);
      this.BindInt("@maxPriority", maxPriority);
      this.BindGuid("@minHostId", minHostId);
      this.BindStringTable("@excludedTargetInstanceNames", (IEnumerable<string>) excludedTargetInstances);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMigrationRequest>((ObjectBinder<HostMigrationRequest>) new HostMigrationRequestBinder());
        return resultCollection.GetCurrent<HostMigrationRequest>().Items;
      }
    }
  }
}
