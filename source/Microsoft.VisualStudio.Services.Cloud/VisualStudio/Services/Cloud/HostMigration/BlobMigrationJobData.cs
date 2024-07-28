// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.BlobMigrationJobData
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class BlobMigrationJobData : 
    BlobMigrationJobDataBase,
    IParallelMigrationIndividualSettings,
    IParallelMigrationOverallSettings,
    IGroupIndexable
  {
    [XmlAttribute("index")]
    public int GroupIndex { get; set; }

    [XmlAttribute("jobName")]
    public string JobName { get; set; }

    [XmlAttribute("jobId")]
    public Guid JobId { get; set; }

    [XmlAttribute("maxConcurrentJobsPerJobAgent")]
    public int MaxConcurrentJobsPerJobAgent { get; set; }
  }
}
