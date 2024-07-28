// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent11
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
  public class HostMigrationComponent11 : HostMigrationComponent10
  {
    public override ContainerMigrationProgress CreateContainerMigrationProgress(
      Guid migrationId,
      Guid parallelBlobMigrationCoordinatorJobId,
      Guid tfsBlobMigrationJobId,
      int jobNo,
      int totalJobs,
      string uri,
      string prefix)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateContainerMigrationProgress");
      this.BindGuid("@migrationId", migrationId);
      this.BindGuid("@parallelBlobMigrationCoordinatorJobId", parallelBlobMigrationCoordinatorJobId);
      this.BindGuid("@tfsBlobMigrationJobId", tfsBlobMigrationJobId);
      this.BindInt("@jobNo", jobNo);
      this.BindInt("@totalJobs", totalJobs);
      this.BindString("@uri", uri, 4096, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@prefix", prefix, 5, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerMigrationProgress>((ObjectBinder<ContainerMigrationProgress>) new ContainerMigrationProgressColumns());
        return resultCollection.GetCurrent<ContainerMigrationProgress>().Items.FirstOrDefault<ContainerMigrationProgress>();
      }
    }

    public override ContainerMigrationProgress UpdateContainerMigrationProgress(
      ContainerMigrationProgress cmp)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateContainerMigrationProgress");
      this.BindGuid("@migrationId", cmp.MigrationId);
      this.BindGuid("@parallelBlobMigrationCoordinatorJobId", cmp.ParallelBlobMigrationCoordinatorJobId);
      this.BindGuid("@tfsBlobMigrationJobId", cmp.TfsBlobMigrationJobId);
      this.BindInt("@jobNo", cmp.JobNo);
      this.BindInt("@totalJobs", cmp.TotalJobs);
      this.BindString("@uri", cmp.Uri, 4096, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@prefix", cmp.Prefix, 5, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindLong("@blobsCopied", cmp.BlobsCopied);
      this.BindLong("@blobsSkipped", cmp.BlobsSkipped);
      this.BindLong("@blobsFailed", cmp.BlobsFailed);
      this.BindBoolean("@completed", cmp.Completed);
      this.BindString("@statusMessage", cmp.StatusMessage, 32768, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerMigrationProgress>((ObjectBinder<ContainerMigrationProgress>) new ContainerMigrationProgressColumns());
        return resultCollection.GetCurrent<ContainerMigrationProgress>().Items.FirstOrDefault<ContainerMigrationProgress>();
      }
    }

    public override List<ContainerMigrationProgress> QueryContainerMigrationProgressEntries(
      Guid migrationId,
      string uri)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryContainerMigrationProgressEntries");
      this.BindGuid("@migrationId", migrationId);
      this.BindString("@uri", uri, 4096, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerMigrationProgress>((ObjectBinder<ContainerMigrationProgress>) new ContainerMigrationProgressColumns());
        return resultCollection.GetCurrent<ContainerMigrationProgress>().Items;
      }
    }

    public override void DeleteContainerMigrationProgress(Guid migrationId)
    {
      this.PrepareStoredProcedure("Migration.prc_DeleteContainerMigrationProgressEntries");
      this.BindGuid("@migrationId", migrationId);
      this.ExecuteNonQuery();
    }
  }
}
