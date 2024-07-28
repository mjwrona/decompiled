// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.WikiContracter
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class WikiContracter : EntityContracter
  {
    public WikiContracter(IndexingExecutionContext indexingExecutionContext, IndexInfo indexInfo)
      : base(indexingExecutionContext.ServiceSettings.ProvisionerSettings, indexInfo.GetDocumentContractType(indexingExecutionContext), (IRoutingDataProvider) new WikiEntityRoutingProvider(indexInfo?.Routing))
    {
    }

    internal bool RetrieveParsedItem(IMetaDataStoreItem item, out IObjectStoreItem parsedItem) => this.ParseStore.TryGet(new ContentId(new Hash(ParserUtils.AppendParserTypeToContentId(ParserType.MD, item.ContentId.RawHash))), out parsedItem);

    public override AbstractSearchDocumentContract GetContractDocumentInternal(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem item)
    {
      int num = item.UpdateType == MetaDataStoreUpdateType.UpdateMetaData ? 0 : (item.UpdateType != MetaDataStoreUpdateType.Delete ? 1 : 0);
      byte[] numArray = Array.Empty<byte>();
      if (num != 0)
      {
        IObjectStoreItem parsedItem;
        if (this.RetrieveParsedItem(item, out parsedItem))
        {
          numArray = parsedItem.Blob;
        }
        else
        {
          Tracer.TraceWarning(1080233, "Indexing Pipeline", "Indexer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AddWikiContractToBatch : Failed to get parse store file for objectId: '{0}' and path: '{1}'", (object) item.ContentId.HexHash, (object) item.Path));
          return (AbstractSearchDocumentContract) null;
        }
      }
      WikiContract documentContract = this.GetDocumentContract();
      documentContract.PopulateFileContractDetails(indexingExecutionContext.RequestContext, (object) item, this.MetaDataStore, new ParsedData()
      {
        Content = numArray
      });
      if ((long) documentContract.GetSize() > indexingExecutionContext.FeederSettings.MaxDocumentSizeToFeedInBytes)
      {
        documentContract = this.GetDocumentContract();
        documentContract.PopulateFileContractDetails(indexingExecutionContext.RequestContext, (object) item, this.MetaDataStore, new ParsedData()
        {
          Content = (byte[]) null
        });
      }
      return (AbstractSearchDocumentContract) documentContract;
    }

    internal WikiContract GetDocumentContract() => new WikiContract();
  }
}
