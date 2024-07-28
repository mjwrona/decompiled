// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.PackageVersionContracter
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class PackageVersionContracter : EntityContracter
  {
    public PackageVersionContracter(
      IndexingExecutionContext indexingExecutionContext,
      IndexInfo indexInfo)
      : base(indexingExecutionContext.ServiceSettings.ProvisionerSettings, indexInfo.GetDocumentContractType(indexingExecutionContext), (IRoutingDataProvider) new PackageEntityRoutingProvider(indexInfo?.Routing))
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
      Array.Empty<byte>();
      Array.Empty<byte>();
      IObjectStoreItem crawledItem1;
      IObjectStoreItem crawledItem2;
      if (!this.RetrieveCrawledItem(item, "packageContent", out crawledItem1) || !this.RetrieveCrawledItem(item, "feedContent", out crawledItem2))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AddPackageVersionContractToBatch : Failed to get content for objectId: '{0}', path: '{1}' and contentKey: '{2}'", (object) item.ContentId.HexHash, (object) item.Path, crawledItem1 == null ? (object) "packageContent" : (object) "feedContent"));
      byte[] blob = crawledItem1.Blob;
      PackageVersionChangeEntityMetadata data = JsonConvert.DeserializeObject<PackageVersionChangeEntityMetadata>(Encoding.UTF8.GetString(crawledItem2.Blob));
      PackageVersionContract documentInternal = new PackageVersionContract();
      documentInternal.PopulateFileContractDetails(indexingExecutionContext.RequestContext, (object) data, new ParsedData()
      {
        Content = blob
      });
      return (AbstractSearchDocumentContract) documentInternal;
    }
  }
}
