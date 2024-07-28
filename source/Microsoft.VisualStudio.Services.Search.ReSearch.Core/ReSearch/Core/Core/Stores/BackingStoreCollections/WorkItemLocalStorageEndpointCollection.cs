// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Stores.BackingStoreCollections.WorkItemLocalStorageEndpointCollection
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Stores.BackingStoreCollections
{
  [Export(typeof (IStorageEndpointCollection))]
  public class WorkItemLocalStorageEndpointCollection : LocalStorageEndpointCollection
  {
    public WorkItemLocalStorageEndpointCollection()
    {
    }

    public WorkItemLocalStorageEndpointCollection(string workingDirectoryName)
      : base(workingDirectoryName)
    {
    }

    public WorkItemLocalStorageEndpointCollection(
      string workingDirectoryName,
      bool storeShouldExist)
      : base(workingDirectoryName, storeShouldExist)
    {
    }

    public WorkItemLocalStorageEndpointCollection(
      string workingDirectoryName,
      bool storeShouldExist,
      IFileSystem fileSystem)
      : base(workingDirectoryName, storeShouldExist, fileSystem)
    {
    }

    public override IList<IEntityType> SupportedEntityTypes => (IList<IEntityType>) new List<IEntityType>()
    {
      (IEntityType) WorkItemEntityType.GetInstance()
    };

    public override IStorageEndpoint CrawlStore
    {
      get
      {
        if (this.m_crawlStore == null)
          this.m_crawlStore = (IStorageEndpoint) new WorkItemLooseFileStorageEndpoint(Path.Combine(this.WorkingDirectory.FullPath, nameof (CrawlStore)), this.FileSystem);
        return this.m_crawlStore;
      }
    }
  }
}
