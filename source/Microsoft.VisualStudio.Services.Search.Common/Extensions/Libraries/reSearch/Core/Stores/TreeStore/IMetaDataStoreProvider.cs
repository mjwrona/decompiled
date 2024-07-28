// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.IMetaDataStoreProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public interface IMetaDataStoreProvider
  {
    IMetaDataStore GetMetaDataStore();

    IEnumerable<IMetaDataStore> GetMetaDataStores();

    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "TEncoder is used in the implemented method")]
    IMetaDataStore<T> GetMetaDataStoreWriter<T, TEncoder>(StringComparer stringComparer)
      where T : IMetaDataStoreItem, new()
      where TEncoder : IMetaDataStoreEncoder<T>, new();
  }
}
