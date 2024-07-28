// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TargetHostMigration
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class TargetHostMigration : IMigrationEntry
  {
    [DataMember]
    public Guid MigrationId { get; set; }

    [DataMember]
    public MigrationHostProperties HostProperties { get; set; }

    [DataMember]
    public TargetMigrationState State { get; set; }

    [DataMember]
    public string StatusMessage { get; set; }

    [DataMember]
    public DateTime StatusChangedDate { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public int SourceDatabaseId { get; set; }

    [DataMember]
    public int TargetDatabaseId { get; set; }

    [DataMember]
    public StorageMigration[] StorageResources { get; set; }

    [DataMember]
    public SqlConnectionInfoWrapper ConnectionInfo { get; set; }

    [DataMember]
    public Guid ParentMigrationId { get; set; }

    [DataMember]
    public bool StorageOnly { get; set; }

    [DataMember]
    public int StorageAccountId { get; set; }

    [DataMember]
    public Guid SourceServiceInstanceId { get; set; }

    [DataMember]
    public Microsoft.VisualStudio.Services.Cloud.ShardingInfo[] ShardingInfo { get; set; }

    [DataMember]
    public TeamFoundationHostType HostType { get; set; }

    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    public bool OnlineBlobCopy
    {
      get => (this.Options & HostMigrationOptions.OnlineBlobCopy) == HostMigrationOptions.OnlineBlobCopy;
      set
      {
        if (value)
          this.Options |= HostMigrationOptions.OnlineBlobCopy;
        else
          this.Options &= HostMigrationOptions.Resume | HostMigrationOptions.LiveHost | HostMigrationOptions.UsePreConnectedDb | HostMigrationOptions.NoPartitionCopy | HostMigrationOptions.MigrationCertSigning | HostMigrationOptions.MigrateAdHocJobs | HostMigrationOptions.PermanentStorageOnly;
      }
    }

    [DataMember]
    public HostMigrationOptions Options { get; set; }

    [DataMember]
    public bool PerformDatabaseCopyValidation { get; set; }

    public static TargetHostMigration FromSourceMigration(
      SourceHostMigration sourceMigration,
      Guid sourceInstanceId)
    {
      return new TargetHostMigration()
      {
        MigrationId = sourceMigration.MigrationId,
        HostProperties = sourceMigration.HostProperties,
        StorageResources = sourceMigration.StorageMigrations,
        ConnectionInfo = sourceMigration.ConnectionInfo,
        ParentMigrationId = sourceMigration.ParentMigrationId,
        StorageOnly = sourceMigration.StorageOnly,
        SourceServiceInstanceId = sourceInstanceId,
        ShardingInfo = sourceMigration.ShardingInfo,
        HostId = sourceMigration.HostId,
        HostType = sourceMigration.HostType,
        Options = sourceMigration.Options
      };
    }

    public List<StorageMigration> GetBlobContainers() => ((IEnumerable<StorageMigration>) this.StorageResources).Where<StorageMigration>((Func<StorageMigration, bool>) (sr => sr.StorageType == StorageType.Blob)).ToList<StorageMigration>();

    public List<StorageMigration> GetAzureTables() => ((IEnumerable<StorageMigration>) this.StorageResources).Where<StorageMigration>((Func<StorageMigration, bool>) (sr => sr.StorageType == StorageType.Table)).ToList<StorageMigration>();

    public List<Microsoft.VisualStudio.Services.Cloud.ShardingInfo> GetBlobContainerShardingInfo() => ((IEnumerable<Microsoft.VisualStudio.Services.Cloud.ShardingInfo>) this.ShardingInfo).Where<Microsoft.VisualStudio.Services.Cloud.ShardingInfo>((Func<Microsoft.VisualStudio.Services.Cloud.ShardingInfo, bool>) (sm => sm.StorageType == StorageType.Blob)).ToList<Microsoft.VisualStudio.Services.Cloud.ShardingInfo>();

    public List<Microsoft.VisualStudio.Services.Cloud.ShardingInfo> GetTableShardingInfo() => ((IEnumerable<Microsoft.VisualStudio.Services.Cloud.ShardingInfo>) this.ShardingInfo).Where<Microsoft.VisualStudio.Services.Cloud.ShardingInfo>((Func<Microsoft.VisualStudio.Services.Cloud.ShardingInfo, bool>) (sm => sm.StorageType == StorageType.Table)).ToList<Microsoft.VisualStudio.Services.Cloud.ShardingInfo>();

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      int totalWidth = 30;
      sb.AppendLine("---------- TargetHostMigration ----------");
      sb.AppendLine("MigrationId:".PadRight(totalWidth) + this.MigrationId.ToString());
      sb.AppendLine("State:".PadRight(totalWidth) + this.State.ToString());
      sb.AppendLine("CreatedDate:".PadRight(totalWidth) + this.CreatedDate.ToString());
      sb.AppendLine("StatusMessage:".PadRight(totalWidth) + this.StatusMessage);
      sb.AppendLine("StatusChangedDate:".PadRight(totalWidth) + this.StatusChangedDate.ToString());
      sb.AppendLine("SourceDatabaseId:".PadRight(totalWidth) + this.SourceDatabaseId.ToString());
      sb.AppendLine("TargetDatabaseId:".PadRight(totalWidth) + this.TargetDatabaseId.ToString());
      sb.AppendLine("ParentMigrationId:".PadRight(totalWidth) + this.ParentMigrationId.ToString());
      sb.AppendLine("ConnectionInfo:".PadRight(totalWidth) + (this.ConnectionInfo != null ? "Exists" : "Not set"));
      sb.AppendLine("StorageAccountId:".PadRight(totalWidth) + this.StorageAccountId.ToString());
      sb.AppendLine("StorageOnly:".PadRight(totalWidth) + this.StorageOnly.ToString());
      sb.AppendLine("Options:".PadRight(totalWidth) + this.Options.ToString());
      sb.AppendLine("PerformDatabaseCopyValidation".PadRight(totalWidth) + this.PerformDatabaseCopyValidation.ToString());
      sb.AppendLine("StorageMigrations:");
      if (this.StorageResources == null || this.StorageResources.Length == 0)
      {
        sb.AppendLine("        none");
      }
      else
      {
        foreach (StorageMigration storageResource in this.StorageResources)
          sb.AppendLine("        " + storageResource.ToString());
      }
      if (this.ShardingInfo == null || this.ShardingInfo.Length == 0)
      {
        sb.AppendLine("        none");
      }
      else
      {
        foreach (Microsoft.VisualStudio.Services.Cloud.ShardingInfo shardingInfo in this.ShardingInfo)
          sb.AppendLine("        " + shardingInfo.ToString());
      }
      sb.AppendLine("    HostProperties:");
      if (this.HostProperties == null)
      {
        sb.AppendLine("        none");
        sb.AppendLine("HostId:".PadRight(totalWidth) + this.HostId.ToString());
        sb.AppendLine("HostType:".PadRight(totalWidth) + this.HostType.ToString());
      }
      else
        this.HostProperties.ToStringBuilder(sb);
      sb.AppendLine("-----------------------------------------");
      return sb.ToString();
    }
  }
}
