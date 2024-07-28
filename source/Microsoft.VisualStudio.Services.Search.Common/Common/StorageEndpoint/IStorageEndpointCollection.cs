// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint.IStorageEndpointCollection
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint
{
  public interface IStorageEndpointCollection : IDisposable
  {
    IList<IEntityType> SupportedEntityTypes { get; }

    IStorageEndpoint CrawlStore { get; }

    IStorageEndpoint ParseStore { get; }

    IMetaDataStoreProvider MetaDataStoreProvider { get; }
  }
}
