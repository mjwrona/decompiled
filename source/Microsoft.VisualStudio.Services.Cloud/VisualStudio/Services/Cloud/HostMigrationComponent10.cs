// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent10
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationComponent10 : HostMigrationComponent9
  {
    public override Guid QueryLatestTargetMigrationByHostId(Guid hostId)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryLatestTargetMigrationByHostId");
      this.BindGuid("@hostId", hostId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder migrationId = new SqlColumnBinder("MigrationId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => migrationId.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items.FirstOrDefault<Guid>();
      }
    }
  }
}
