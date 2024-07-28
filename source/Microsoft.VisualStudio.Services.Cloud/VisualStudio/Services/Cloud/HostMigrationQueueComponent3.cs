// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationQueueComponent3
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationQueueComponent3 : HostMigrationQueueComponent2
  {
    public override List<HostMigrationRequest> GetRunningRequests()
    {
      this.PrepareStoredProcedure("Migration.prc_GetRunningQueueRequests");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMigrationRequest>((ObjectBinder<HostMigrationRequest>) new HostMigrationRequestBinder());
        return resultCollection.GetCurrent<HostMigrationRequest>().Items;
      }
    }

    public override List<HostMigrationRequest> GetSchedulableQueueRequest(
      DateTime maxLastUserAccess,
      int numberOfRequests,
      string[] excludedTargetInstances)
    {
      this.PrepareStoredProcedure("Migration.prc_GetSchedulableQueueRequest");
      this.BindDateTime("@maxLastUserAccess", maxLastUserAccess);
      this.BindInt("@numberOfRequests", numberOfRequests);
      this.BindStringTable("@excludedTargetInstanceNames", (IEnumerable<string>) excludedTargetInstances);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMigrationRequest>((ObjectBinder<HostMigrationRequest>) new HostMigrationRequestBinder());
        return resultCollection.GetCurrent<HostMigrationRequest>().Items;
      }
    }
  }
}
