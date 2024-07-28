// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryTableLoader
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  public class MemoryTableLoader : TableLoader
  {
    private Dictionary<string, MemoryTableStorage> storages;

    public MemoryTableLoader(IEnumerable<MemoryTableStorage> storages) => this.storages = storages.ToDictionary<MemoryTableStorage, string>((Func<MemoryTableStorage, string>) (storage => storage.AccountName));

    public ITable LoadSource(IVssRequestContext deploymentContext, StorageMigration sm)
    {
      string id = sm.Id;
      return new MemoryTableClientFactory(this.storages[sm.StorageAccountName], id).GetTable((string) null);
    }

    public ITable LoadTarget(
      IVssRequestContext deploymentContext,
      MigrateStorageInfo info,
      string name)
    {
      return new MemoryTableClientFactory(this.storages[info.Name], name).GetTable((string) null);
    }
  }
}
