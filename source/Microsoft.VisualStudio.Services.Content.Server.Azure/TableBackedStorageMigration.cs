// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableBackedStorageMigration
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class TableBackedStorageMigration : IStorageMigration
  {
    public bool IsSharded => true;

    public StorageType StorageType => StorageType.Table;

    public string VsoArea { get; private set; }

    public int? ShardIndex => new int?();

    public string Id => (string) null;

    public ITable Table { get; private set; }

    internal TableBackedStorageMigration(string saName, ITable table)
    {
      this.VsoArea = saName;
      this.Table = table;
    }
  }
}
