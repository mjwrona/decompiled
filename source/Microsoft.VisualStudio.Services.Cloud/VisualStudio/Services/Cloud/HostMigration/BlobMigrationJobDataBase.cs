// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.BlobMigrationJobDataBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public abstract class BlobMigrationJobDataBase : 
    IMigrationDataSetter,
    IMigrationData,
    IParallelMigrationOverallSettings
  {
    private int m_inContainerParallelism = 1;

    [XmlAttribute("totalGroups")]
    public int TotalGroups { get; set; }

    [XmlAttribute("inContainerParallelism")]
    public int InContainerParallelism
    {
      get => this.m_inContainerParallelism;
      set
      {
        if (value < 16)
          this.m_inContainerParallelism = 1;
        else if (value < 256)
          this.m_inContainerParallelism = 16;
        else
          this.m_inContainerParallelism = 256;
      }
    }

    [XmlAttribute("hostId")]
    public Guid HostId { get; set; }

    [XmlAttribute("hostName")]
    public string HostName { get; set; }

    [XmlAttribute("migrationId")]
    public Guid MigrationId { get; set; }

    [XmlAttribute("storageOnly")]
    public bool StorageOnly { get; set; }

    [XmlAttribute("storageType")]
    public StorageType StorageType { get; set; }

    internal void SetHostInfo(TargetHostMigration migrationEntry)
    {
      if (migrationEntry == null)
        return;
      if (this.HostName == null)
        this.HostName = migrationEntry.HostProperties?.Name;
      if (!(this.HostId == new Guid()))
        return;
      this.HostId = migrationEntry.HostId;
    }
  }
}
