// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SourceHostMigration
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class SourceHostMigration : IMigrationEntry
  {
    [DataMember]
    public Guid MigrationId { get; set; }

    [DataMember]
    public MigrationHostProperties HostProperties { get; set; }

    [DataMember]
    public SourceMigrationState State { get; set; }

    [DataMember]
    public string StatusMessage { get; set; }

    [DataMember]
    public DateTime StatusChangedDate { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public Guid ParentMigrationId { get; set; }

    [DataMember]
    public Guid TargetServiceInstanceId { get; set; }

    [DataMember]
    public bool StorageOnly { get; set; }

    [DataMember]
    public StorageMigration[] StorageMigrations { get; set; }

    [DataMember]
    public Microsoft.VisualStudio.Services.Cloud.ShardingInfo[] ShardingInfo { get; set; }

    [DataMember]
    public bool OnlineBlobCopy
    {
      get => this.Options.HasFlag((Enum) HostMigrationOptions.OnlineBlobCopy);
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
    public SqlConnectionInfoWrapper ConnectionInfo { get; set; }

    [DataMember]
    public int HostsAffectedByTheMove { get; set; }

    [IgnoreDataMember]
    internal int CredentialId { get; set; }

    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    internal TeamFoundationHostType HostType { get; set; }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      int totalWidth = 30;
      sb.AppendLine("---------- SourceHostMigration ----------");
      sb.AppendLine("MigrationId:".PadRight(totalWidth) + this.MigrationId.ToString());
      sb.AppendLine("State:".PadRight(totalWidth) + this.State.ToString());
      sb.AppendLine("CreatedDate:".PadRight(totalWidth) + this.CreatedDate.ToString());
      sb.AppendLine("StatusMessage:".PadRight(totalWidth) + this.StatusMessage);
      sb.AppendLine("StatusChangedDate:".PadRight(totalWidth) + this.StatusChangedDate.ToString());
      sb.AppendLine("ParentMigrationId:".PadRight(totalWidth) + this.ParentMigrationId.ToString());
      sb.AppendLine("CredentialId:".PadRight(totalWidth) + this.CredentialId.ToString());
      sb.AppendLine("ConnectionInfo:".PadRight(totalWidth) + (this.ConnectionInfo != null ? "Exists" : "Not set"));
      sb.AppendLine("StorageOnly:".PadRight(totalWidth) + this.StorageOnly.ToString());
      sb.AppendLine("Options:".PadRight(totalWidth) + this.Options.ToString());
      sb.AppendLine("StorageMigrations:");
      if (this.StorageMigrations == null || this.StorageMigrations.Length == 0)
      {
        sb.AppendLine("        none");
      }
      else
      {
        foreach (StorageMigration storageMigration in this.StorageMigrations)
          sb.AppendLine("        " + storageMigration.ToString());
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
