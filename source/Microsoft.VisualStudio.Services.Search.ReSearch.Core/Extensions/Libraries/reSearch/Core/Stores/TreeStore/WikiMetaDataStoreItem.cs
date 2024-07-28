// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.WikiMetaDataStoreItem
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public class WikiMetaDataStoreItem : BasicMetaDataStoreItem
  {
    public string WikiId { get; private set; }

    public string WikiName { get; private set; }

    public string MappedPath { get; private set; }

    public WikiMetaDataStoreItem()
    {
    }

    public WikiMetaDataStoreItem(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      string documentId,
      List<BranchInfo> branches,
      long? filePathId,
      string wikiId,
      string wikiName,
      string mappedPath)
      : base(contentId, path, updateType, documentId, branches, filePathId)
    {
      this.WikiId = wikiId;
      this.WikiName = wikiName;
      this.MappedPath = mappedPath;
    }

    public WikiMetaDataStoreItem(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      long size)
      : base(contentId, path, updateType, size)
    {
    }

    public void Initialize(string wikiId, string wikiName, string mappedPath)
    {
      this.WikiId = wikiId;
      this.WikiName = wikiName;
      this.MappedPath = mappedPath;
    }
  }
}
