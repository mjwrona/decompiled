// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore.PackagingItemStoreBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl;
using Microsoft.VisualStudio.Services.ItemStore.AzureStorage;
using Microsoft.VisualStudio.Services.ItemStore.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore
{
  public abstract class PackagingItemStoreBase : ItemStoreService
  {
    internal void UseMemoryItemProvider() => this.ItemProvider = (IItemProvider) new ItemTableProvider((ITableClientFactory) new MemoryTableClientFactory(new MemoryTableStorage("inmemoryaccount"), this.GetType().Name.ToLowerInvariant()));
  }
}
