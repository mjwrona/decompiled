// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent6
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationComponent6 : HostMigrationComponent5
  {
    public override TargetHostMigration CreateTargetMigration(TargetHostMigration entry)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateTargetMigration");
      this.BindGuid("@migrationId", entry.MigrationId);
      this.BindGuid("@hostId", entry.HostProperties.Id);
      this.BindBoolean(this.m_storageOnlyName, entry.StorageOnly);
      this.BindStorageMigrationTable(this.m_storageMigrationName, (IEnumerable<StorageMigration>) entry.StorageResources);
      this.BindInt("@storageAccountId", entry.StorageAccountId);
      this.BindGuid("@sourceInstanceId", entry.SourceServiceInstanceId);
      if (!object.Equals((object) entry.ParentMigrationId, (object) Guid.Empty))
        this.BindGuid("@parentMigrationId", entry.ParentMigrationId);
      this.BindShardingInfoTable("@shardingInfo", (IEnumerable<ShardingInfo>) entry.ShardingInfo);
      this.BindInt("@hostType", (int) entry.HostType);
      this.BindBoolean("@onlineBlobCopy", entry.OnlineBlobCopy);
      return this.ReadTargetMigration();
    }

    public override SourceHostMigration CreateSourceMigration(SourceHostMigration migrationRequest)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateSourceMigration");
      this.BindGuid("@migrationId", migrationRequest.MigrationId);
      this.BindGuid("@hostId", migrationRequest.HostId);
      this.BindBoolean(this.m_storageOnlyName, migrationRequest.StorageOnly);
      this.BindGuid("@targetServiceInstanceId", migrationRequest.TargetServiceInstanceId);
      if (!object.Equals((object) migrationRequest.ParentMigrationId, (object) Guid.Empty))
        this.BindGuid("@parentMigrationId", migrationRequest.ParentMigrationId);
      this.BindInt("@hostType", (int) migrationRequest.HostType);
      this.BindBoolean("@onlineBlobCopy", migrationRequest.OnlineBlobCopy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SourceHostMigration>((ObjectBinder<SourceHostMigration>) this.CreateSourceMigrationColumns());
        return resultCollection.GetCurrent<SourceHostMigration>().Items.FirstOrDefault<SourceHostMigration>();
      }
    }
  }
}
