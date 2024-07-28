// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContainerMigrationProgress
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class ContainerMigrationProgress
  {
    [DataMember]
    public Guid MigrationId { get; set; }

    [DataMember]
    public Guid ParallelBlobMigrationCoordinatorJobId { get; set; }

    [DataMember]
    public Guid TfsBlobMigrationJobId { get; set; }

    [DataMember]
    public int JobNo { get; set; }

    [DataMember]
    public int TotalJobs { get; set; }

    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public string Prefix { get; set; }

    [DataMember]
    public long BlobsCopied { get; set; }

    [DataMember]
    public long BlobsSkipped { get; set; }

    [DataMember]
    public long BlobsFailed { get; set; }

    [DataMember]
    public bool Completed { get; set; }

    [DataMember]
    public DateTime ChangedDate { get; set; }

    [DataMember]
    public string StatusMessage { get; set; }
  }
}
