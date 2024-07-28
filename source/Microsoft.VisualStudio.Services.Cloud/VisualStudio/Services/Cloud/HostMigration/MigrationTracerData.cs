// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.MigrationTracerData
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class MigrationTracerData : IMigrationData
  {
    public Guid HostId { get; }

    public string HostName { get; }

    public Guid MigrationId { get; }

    public bool StorageOnly { get; }

    public StorageType StorageType { get; }

    public MigrationTracerData(
      Guid hostId,
      string hostName,
      Guid migrationId,
      bool storageOnly,
      StorageType storageType)
    {
      this.HostId = hostId;
      this.HostName = hostName;
      this.MigrationId = migrationId;
      this.StorageOnly = storageOnly;
      this.StorageType = storageType;
    }
  }
}
