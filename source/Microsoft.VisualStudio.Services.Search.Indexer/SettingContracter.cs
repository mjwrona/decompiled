// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.SettingContracter
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class SettingContracter : EntityContracter
  {
    public SettingContracter(IndexingExecutionContext indexingExecutionContext, IndexInfo indexInfo)
      : base(indexingExecutionContext.ServiceSettings.ProvisionerSettings, indexInfo.GetDocumentContractType(indexingExecutionContext), (IRoutingDataProvider) new SettingEntityRoutingProvider(string.Empty))
    {
    }

    internal bool RetrieveCrawledItem(
      IMetaDataStoreItem item,
      string contentKey,
      out IObjectStoreItem crawledItem)
    {
      return this.CrawlStore.TryGet(new ContentId(new Hash(item.ContentId.RawHash), contentKey), out crawledItem);
    }

    public override AbstractSearchDocumentContract GetContractDocumentInternal(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem item)
    {
      IObjectStoreItem crawledItem;
      if (!this.RetrieveCrawledItem(item, "settingContent", out crawledItem))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AddSettingContractToBatch : Failed to get content for objectId: '{0}', path: '{1}' and contentKey: '{2}'", (object) item.ContentId.HexHash, (object) item.Path, (object) "settingContent"));
      byte[] blob = crawledItem.Blob;
      SettingContract documentInternal = new SettingContract();
      documentInternal.PopulateFileContractDetails(new ParsedData()
      {
        Content = blob
      });
      return (AbstractSearchDocumentContract) documentInternal;
    }
  }
}
