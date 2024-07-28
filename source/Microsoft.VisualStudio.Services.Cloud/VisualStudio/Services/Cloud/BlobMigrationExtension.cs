// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobMigrationExtension
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [InheritedExport]
  public abstract class BlobMigrationExtension
  {
    public List<MigrateStorageInfo> GetStorageInfos(
      IVssRequestContext requestContext,
      Action<string> log = null)
    {
      return this.GetStorageInfosExtension(requestContext, log ?? new Action<string>(this.NullLog));
    }

    public List<StorageMigration> GetContainerLists(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      Action<string> log = null)
    {
      return this.GetContainerListsExtension(requestContext, hostProperties, log ?? new Action<string>(this.NullLog));
    }

    protected abstract List<MigrateStorageInfo> GetStorageInfosExtension(
      IVssRequestContext requestContext,
      Action<string> log);

    protected abstract List<StorageMigration> GetContainerListsExtension(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      Action<string> log);

    public virtual List<ShardingInfo> GetShardingInfo(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      Action<string> log)
    {
      return new List<ShardingInfo>(0);
    }

    private void NullLog(string message)
    {
    }

    public virtual long ExecuteShardedCopy(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      StorageMigration shardedResource,
      string prefix,
      Dictionary<string, AzureProvider> sourceProviders,
      Action<string> log)
    {
      return 0;
    }

    public ConcurrentDictionary<string, object> SharedContextForShardedCopy { get; protected internal set; }
  }
}
