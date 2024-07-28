// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.EntityContracter
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public abstract class EntityContracter : IDocumentContracter
  {
    private IRoutingDataProvider m_routingDataProvider;

    internal EntityContracter(
      ProvisionerSettings settings,
      DocumentContractType contractType,
      IRoutingDataProvider routingDataProvider)
    {
      this.Settings = settings;
      this.DocumentContractType = contractType;
      this.m_routingDataProvider = routingDataProvider;
    }

    public void Initialize(
      IStorageEndpoint parseStore,
      IMetaDataStore metaDataStore,
      IStorageEndpoint crawlStore)
    {
      this.MetaDataStore = metaDataStore ?? throw new ArgumentNullException(nameof (metaDataStore));
      this.CrawlStore = crawlStore ?? throw new ArgumentNullException(nameof (crawlStore));
      this.ParseStore = parseStore;
    }

    public virtual AbstractSearchDocumentContract GetContractDocument(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem metaDataStoreItem)
    {
      AbstractSearchDocumentContract documentInternal = this.GetContractDocumentInternal(indexingExecutionContext, metaDataStoreItem);
      if (documentInternal != null)
        this.AssignRouting(indexingExecutionContext, documentInternal);
      return documentInternal;
    }

    public abstract AbstractSearchDocumentContract GetContractDocumentInternal(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem fileItem);

    private void AssignRouting(
      IndexingExecutionContext indexingExecutionContext,
      AbstractSearchDocumentContract abstractSearchDocumentContract)
    {
      abstractSearchDocumentContract.Routing = this.m_routingDataProvider.GetRouting(indexingExecutionContext, abstractSearchDocumentContract.Item);
    }

    internal bool ValidateDocumentContract(DocumentContractType contractType) => this.DocumentContractType == contractType;

    protected ProvisionerSettings Settings { get; }

    protected DocumentContractType DocumentContractType { get; set; }

    protected IStorageEndpoint ParseStore { get; set; }

    protected IMetaDataStore MetaDataStore { get; set; }

    protected IStorageEndpoint CrawlStore { get; set; }
  }
}
