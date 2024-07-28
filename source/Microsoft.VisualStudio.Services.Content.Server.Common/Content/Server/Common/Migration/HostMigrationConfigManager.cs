// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Migration.HostMigrationConfigManager
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.Migration
{
  public class HostMigrationConfigManager
  {
    private IServiceHostAccess m_acc;
    private Guid m_hostId;

    public HostMigrationConfigManager(IServiceHostAccess access, Guid hostId)
    {
      this.m_acc = access;
      this.m_hostId = hostId;
    }

    public ParallelHostMigrationConfig GetConfig()
    {
      int int32_1 = this.GetInt32(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_TotalStorageAccountGroups, 1);
      int int32_2 = this.GetInt32(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_DelayInMillisecBetweenJobs, 0);
      int int32_3 = this.GetInt32(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_BlobCopyInContainerParallelism, 1);
      int int32_4 = this.GetInt32(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_MaxConcurrentJobsPerJobAgent, 1);
      int int32_5 = this.GetInt32(new Guid?(), FrameworkServerConstants.MigrationMaxContainerParallelism, 8);
      bool flag1 = this.GetBool(new Guid?(), FrameworkServerConstants.PreMigrationStopOnPendingCopy, true);
      bool flag2 = this.GetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_BlobCopyTracing, false);
      int int32_6 = this.GetInt32(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_BlobMetadataTablePrefixParallelism, 1);
      bool flag3 = this.GetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_EnableBlobTableMigrationCheckpoints, false);
      bool flag4 = this.GetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_EnableBlobTablePrefixParallelism, false);
      bool flag5 = this.GetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_EnableBlobMigrationParallelism, false);
      return new ParallelHostMigrationConfig()
      {
        TotalParallelJobsPerShardingGroup = new int?(int32_1),
        DelayInMilliSecondsBetweenCopyJobs = new int?(int32_2),
        InContainerParallelism = new int?(int32_3),
        MaxCopyThreadsPerCopyJob = new int?(int32_5),
        MaxConcurrentJobsPerJobAgent = new int?(int32_4),
        PreMigrationStopOnPendingCopy = new bool?(flag1),
        Tracing = new bool?(flag2),
        BlobMetadataTablePrefixParallelism = new int?(int32_6),
        EnableTableMigrationCheckpoints = new bool?(flag3),
        EnableTablePrefixParallelism = new bool?(flag4),
        EnableBlobMigrationParallelism = new bool?(flag5)
      };
    }

    public void SetConfig(ParallelHostMigrationConfig conf, Action<TraceLevel, string> log)
    {
      if (!conf.Validate(this.m_acc.IsProduction, log))
        throw new IllegalHostMigrationSettingException();
      List<string> toDelete = conf.UnsetUnspecified ? new List<string>() : (List<string>) null;
      this.SetInt(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_TotalStorageAccountGroups, conf.TotalParallelJobsPerShardingGroup, toDelete);
      this.SetInt(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_DelayInMillisecBetweenJobs, conf.DelayInMilliSecondsBetweenCopyJobs, toDelete);
      this.SetInt(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_BlobCopyInContainerParallelism, conf.InContainerParallelism, toDelete);
      this.SetInt(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_MaxConcurrentJobsPerJobAgent, conf.MaxConcurrentJobsPerJobAgent, toDelete);
      this.SetInt(new Guid?(), FrameworkServerConstants.MigrationMaxContainerParallelism, conf.MaxCopyThreadsPerCopyJob, toDelete);
      this.SetBool(new Guid?(), FrameworkServerConstants.PreMigrationStopOnPendingCopy, conf.PreMigrationStopOnPendingCopy, toDelete);
      this.SetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_BlobCopyTracing, conf.Tracing, toDelete);
      this.SetInt(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_BlobMetadataTablePrefixParallelism, conf.BlobMetadataTablePrefixParallelism, toDelete);
      this.SetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_EnableBlobTableMigrationCheckpoints, conf.EnableTableMigrationCheckpoints, toDelete);
      this.SetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_EnableBlobTablePrefixParallelism, conf.EnableTablePrefixParallelism, toDelete);
      this.SetBool(new Guid?(this.m_hostId), FrameworkServerConstants.ParallelMigration_EnableBlobMigrationParallelism, conf.EnableBlobMigrationParallelism, toDelete);
      if (toDelete == null || toDelete.Count <= 0)
        return;
      this.m_acc.DeleteRegistries(toDelete.ToArray());
    }

    public string GetPath(Guid? hostId, string key) => !hostId.HasValue ? key : HostSpecificRegistryUtil.GetPerHostRegistryPath(hostId.Value, FrameworkServerConstants.ParallelMigrationRoot, key);

    private int GetInt32(Guid? hostId, string key, int defaultValue) => this.m_acc.GetRegistry(this.GetPath(hostId, key), defaultValue);

    private void SetInt(Guid? hostId, string key, int? nullable, List<string> toDelete)
    {
      string path = this.GetPath(hostId, key);
      if (nullable.HasValue)
        this.m_acc.SetRegistry(path, nullable.Value);
      else
        toDelete?.Add(path);
    }

    private bool GetBool(Guid? hostId, string key, bool defaultVal) => this.m_acc.GetRegistry(this.GetPath(hostId, key), defaultVal);

    private void SetBool(Guid? hostId, string key, bool? nullable, List<string> toDelete)
    {
      string path = this.GetPath(hostId, key);
      if (nullable.HasValue)
        this.m_acc.SetRegistry(path, nullable.Value);
      else
        toDelete?.Add(path);
    }
  }
}
