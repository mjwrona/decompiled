// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CodeContracter
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class CodeContracter : EntityContracter
  {
    public CodeContracter(IndexingExecutionContext indexingExecutionContext, IndexInfo indexInfo)
      : base(indexingExecutionContext.ServiceSettings.ProvisionerSettings, indexInfo.GetDocumentContractType(indexingExecutionContext), (IRoutingDataProvider) new CodeEntityRoutingProvider(CodeEntityRoutingProvider.GetRoutingFromIndexInfo(indexingExecutionContext, indexInfo)))
    {
    }

    internal virtual bool RetrieveParsedItem(
      IMetaDataStoreItem fileItem,
      out IObjectStoreItem parsedItem)
    {
      try
      {
        return this.ParseStore.TryGet(new ContentId(new Hash(ParserUtils.AppendParserTypeToContentId(fileItem.GetParserType(), fileItem.ContentId.RawHash))), out parsedItem) || this.ParseStore.TryGet(new ContentId(new Hash(ParserUtils.AppendParserTypeToContentId(ParserType.Text, fileItem.ContentId.RawHash))), out parsedItem) || this.ParseStore.TryGet(new ContentId(new Hash(ParserUtils.AppendParserTypeToContentId(ParserType.None, fileItem.ContentId.RawHash))), out parsedItem);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080209, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Failed to read parse store, exception {0}", (object) ex)));
        parsedItem = (IObjectStoreItem) null;
        return false;
      }
    }

    public override AbstractSearchDocumentContract GetContractDocumentInternal(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem metaDataStoreItem)
    {
      int num = metaDataStoreItem.UpdateType == MetaDataStoreUpdateType.UpdateMetaData ? 0 : (metaDataStoreItem.UpdateType != MetaDataStoreUpdateType.Delete ? 1 : 0);
      byte[] contentBytes = (byte[]) null;
      byte[] numArray = Array.Empty<byte>();
      ParsedData parsedData1;
      if (num != 0)
      {
        IObjectStoreItem parsedItem;
        if (this.RetrieveParsedItem(metaDataStoreItem, out parsedItem))
        {
          byte[] blob = parsedItem.Blob;
          if (blob == null || blob.Length == 0)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080209, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("GetContractDocument : ParsedData was either null or empty for Content Id {0} and Path {1}", (object) metaDataStoreItem.ContentId.HexHash, (object) metaDataStoreItem.Path)));
            return (AbstractSearchDocumentContract) null;
          }
          parsedData1 = Serializers.FromByteArray<ParsedData>(blob);
          if ((indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.IndexOriginalCodeContent") || this.DocumentContractType.IsNoPayloadContract()) && !this.GetOriginalCodeContent(metaDataStoreItem, out contentBytes))
            return (AbstractSearchDocumentContract) null;
        }
        else
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080209, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("GetContractDocument : ParsedData not found for Content Id {0} and Path {1}", (object) metaDataStoreItem.ContentId.HexHash, (object) metaDataStoreItem.Path)));
          return (AbstractSearchDocumentContract) null;
        }
      }
      else
        parsedData1 = new ParsedData()
        {
          Content = numArray
        };
      CodeFileContract codeContract = CodeFileContract.CreateCodeContract(this.DocumentContractType, indexingExecutionContext.ProvisioningContext.SearchPlatform);
      codeContract.PopulateFileContractDetails(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit, (object) metaDataStoreItem, this.MetaDataStore, parsedData1, this.Settings, contentBytes);
      if ((long) codeContract.GetSize() > indexingExecutionContext.FeederSettings.MaxDocumentSizeToFeedInBytes)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080209, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Size of the document is {0} ", (object) codeContract.GetSize())) + FormattableString.Invariant(FormattableStringFactory.Create("which is more than the maximum supported size {0}.", (object) indexingExecutionContext.FeederSettings.MaxDocumentSizeToFeedInBytes)) + FormattableString.Invariant(FormattableStringFactory.Create("Hence, setting the empty content and original content.")));
        codeContract = CodeFileContract.CreateCodeContract(this.DocumentContractType, indexingExecutionContext.ProvisioningContext.SearchPlatform);
        CodeFileContract codeFileContract = codeContract;
        IVssRequestContext requestContext = indexingExecutionContext.RequestContext;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repositoryIndexingUnit = indexingExecutionContext.RepositoryIndexingUnit;
        IMetaDataStoreItem data = metaDataStoreItem;
        IMetaDataStore metaDataStore = this.MetaDataStore;
        ParsedData parsedData2 = new ParsedData();
        parsedData2.Content = (byte[]) null;
        ProvisionerSettings settings = this.Settings;
        codeFileContract.PopulateFileContractDetails(requestContext, repositoryIndexingUnit, (object) data, metaDataStore, parsedData2, settings);
      }
      return (AbstractSearchDocumentContract) codeContract;
    }

    internal virtual bool GetOriginalCodeContent(
      IMetaDataStoreItem fileItem,
      out byte[] contentBytes)
    {
      contentBytes = (byte[]) null;
      try
      {
        IObjectStoreItem objectStoreItem;
        if (!this.CrawlStore.TryGet(new ContentId(fileItem.ContentId), out objectStoreItem))
          return false;
        contentBytes = objectStoreItem.Blob;
        return true;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080209, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Failed to get content in CrawlStore for contentId = '{0}', Exception {1}", (object) fileItem.ContentId, (object) ex)));
        return false;
      }
    }
  }
}
