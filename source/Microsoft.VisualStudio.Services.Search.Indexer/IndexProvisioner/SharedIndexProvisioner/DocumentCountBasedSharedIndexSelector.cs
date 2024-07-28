// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner.DocumentCountBasedSharedIndexSelector
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner
{
  internal sealed class DocumentCountBasedSharedIndexSelector : EntitySharedIndexSelector
  {
    private readonly long m_maxDocsPerIndex;

    public DocumentCountBasedSharedIndexSelector(
      ExecutionContext executionContext,
      ProvisionerConfigAndConstantsProvider provider,
      ISearchPlatform searchPlatform,
      ISearchClusterManagementService searchClusterManagementService,
      long maxDocsPerIndex,
      IDataAccessFactory dataAccessFactory)
      : this(executionContext, provider, searchPlatform, searchClusterManagementService, maxDocsPerIndex, dataAccessFactory, new CreateSearchIndexHelper())
    {
    }

    public DocumentCountBasedSharedIndexSelector(
      ExecutionContext executionContext,
      ProvisionerConfigAndConstantsProvider provider,
      ISearchPlatform searchPlatform,
      ISearchClusterManagementService searchClusterManagementService,
      long maxDocsPerIndex,
      IDataAccessFactory dataAccessFactory,
      CreateSearchIndexHelper createSearchIndexHelper)
      : base(executionContext, provider, searchPlatform, searchClusterManagementService, dataAccessFactory, createSearchIndexHelper)
    {
      this.m_maxDocsPerIndex = maxDocsPerIndex;
    }

    internal override bool IsSharedIndexFull(
      IndexingExecutionContext indexingExecutionContext,
      IndexIdentity sharedIndexIdentity)
    {
      return this.GetIndexedDocumentCount(sharedIndexIdentity) >= this.m_maxDocsPerIndex;
    }

    internal long GetIndexedDocumentCount(IndexIdentity indexIdentity)
    {
      long indexedDocumentCount = -1;
      try
      {
        indexedDocumentCount = this.SearchPlatform.GetIndex(indexIdentity).GetIndexedDocumentCount();
      }
      catch (Exception ex)
      {
        Tracer.TraceException(1082247, "Indexing Pipeline", "Indexer", ex);
      }
      return indexedDocumentCount;
    }
  }
}
