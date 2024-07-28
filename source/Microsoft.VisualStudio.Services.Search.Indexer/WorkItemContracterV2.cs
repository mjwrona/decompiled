// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.WorkItemContracterV2
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class WorkItemContracterV2 : EntityContracter
  {
    private bool m_enableIndexingOfIdentityFields;

    private Dictionary<string, WorkItemField> Fields { get; set; }

    public WorkItemContracterV2(
      IndexingExecutionContext indexingExecutionContext,
      IndexInfo indexInfo)
      : base(indexingExecutionContext.ServiceSettings.ProvisionerSettings, indexInfo.GetDocumentContractType(indexingExecutionContext), (IRoutingDataProvider) new WorkItemEntityRoutingProvider(WorkItemEntityRoutingProvider.GetRoutingFromIndexInfo(indexingExecutionContext)))
    {
    }

    protected void Initialize(IndexingExecutionContext indexingExecutionContext)
    {
      if (this.Fields == null)
        this.Fields = JsonConvert.DeserializeObject<List<WorkItemField>>(this.MetaDataStore["WorkItemFields"]).ToDictionary<WorkItemField, string, WorkItemField>((Func<WorkItemField, string>) (x => x.ReferenceName), (Func<WorkItemField, WorkItemField>) (x => x));
      this.m_enableIndexingOfIdentityFields = indexingExecutionContext.RequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableIndexingIdentityFields", TeamFoundationHostType.ProjectCollection);
    }

    internal bool RetrieveCrawledItem(
      IMetaDataStoreItem documentItem,
      string contentKey,
      out IObjectStoreItem crawledItem)
    {
      return this.CrawlStore.TryGet(new ContentId(new Hash(documentItem.ContentId.RawHash), contentKey, documentItem.DocumentId), out crawledItem);
    }

    public override AbstractSearchDocumentContract GetContractDocumentInternal(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem documentItem)
    {
      this.Initialize(indexingExecutionContext);
      byte[] numArray1 = this.RetrieveItemBlob(documentItem, "snapshot");
      byte[] numArray2 = this.RetrieveItemBlob(documentItem, "discussion");
      if ((numArray1 != null || numArray2 != null ? (this.OptimalDocumentSizeExceeded(indexingExecutionContext, (numArray1 != null ? numArray1.Length : 0) + (numArray2 != null ? numArray2.Length : 0)) ? 1 : 0) : 1) != 0)
        return (AbstractSearchDocumentContract) null;
      try
      {
        WorkItemContract contractDocument = this.GetContractDocument();
        contractDocument.IsDiscussionsOnlyDocument = numArray1 == null && numArray2 != null;
        WorkItemContract workItemContract = contractDocument;
        IVssRequestContext requestContext = indexingExecutionContext.RequestContext;
        Dictionary<string, WorkItemField> fields = this.Fields;
        IMetaDataStore metaDataStore = this.MetaDataStore;
        ParsedData workItemParsedData = new ParsedData();
        workItemParsedData.Content = numArray1;
        ParsedData discussionsParsedData = new ParsedData();
        discussionsParsedData.Content = numArray2;
        IMetaDataStoreItem documentItem1 = documentItem;
        bool? indexIdentityFields = new bool?(this.m_enableIndexingOfIdentityFields);
        workItemContract.PopulateFileContractDetails(requestContext, (object) fields, metaDataStore, workItemParsedData, discussionsParsedData, documentItem1, indexIdentityFields);
        return (AbstractSearchDocumentContract) contractDocument;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080230, "Indexing Pipeline", "Indexer", ex.ToString());
        return (AbstractSearchDocumentContract) null;
      }
    }

    internal WorkItemContract GetContractDocument() => new WorkItemContract();

    internal virtual bool OptimalDocumentSizeExceeded(
      IndexingExecutionContext indexingExecutionContext,
      int currentDocContentLength)
    {
      long sizeToFeedInBytes = indexingExecutionContext.FeederSettings.MaxDocumentSizeToFeedInBytes;
      return sizeToFeedInBytes > 0L && (long) currentDocContentLength > sizeToFeedInBytes;
    }

    internal virtual byte[] RetrieveItemBlob(IMetaDataStoreItem documentItem, string contentKey)
    {
      IObjectStoreItem crawledItem;
      this.RetrieveCrawledItem(documentItem, contentKey, out crawledItem);
      if (crawledItem == null)
        return (byte[]) null;
      byte[] blob = crawledItem.Blob;
      return blob != null && blob.Length != 0 ? blob : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WorkItemContracter.AddDocumentContractToBatch : No Content found for metadata key with Content Id {0} and Path {1}", (object) documentItem.ContentId.HexHash, (object) documentItem.Path));
    }
  }
}
