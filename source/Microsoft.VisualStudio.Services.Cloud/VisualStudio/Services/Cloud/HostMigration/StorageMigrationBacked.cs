// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.StorageMigrationBacked
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class StorageMigrationBacked : IStorageMigration
  {
    private StorageMigration m_mig;

    public StorageMigrationBacked(StorageMigration mig) => this.m_mig = mig;

    public StorageMigration StorageMigration => this.m_mig;

    public bool IsSharded => this.m_mig.IsSharded;

    public StorageType StorageType => this.m_mig.StorageType;

    public string VsoArea => this.m_mig.VsoArea;

    public int? ShardIndex => this.m_mig.ShardIndex;

    public string Id => this.m_mig.Id;
  }
}
