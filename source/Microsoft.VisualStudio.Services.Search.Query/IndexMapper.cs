// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.IndexMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class IndexMapper : IIndexMapper
  {
    private readonly IEntityType m_entityType;

    public IndexMapper(IEntityType entityType) => this.m_entityType = entityType;

    public IEnumerable<IndexInfo> GetIndexInfo(
      IVssRequestContext requestContext,
      EntitySearchQuery searchQuery)
    {
      return requestContext.GetService<CodeQueryScopingCacheService>().GetScopedQueryIndexInfo(this.m_entityType, searchQuery, requestContext);
    }

    public IEnumerable<IndexInfo> GetIndexInfo(IVssRequestContext requestContext) => requestContext.GetService<IDocumentContractTypeService>().GetQueryIndexInfo(this.m_entityType);

    public IEnumerable<IndexInfo> GetIndexInfoForABTesting(IVssRequestContext requestContext) => requestContext.GetService<DocumentContractTypeService>().GetQueryIndexInfoForABTesting();

    public DocumentContractType GetDocumentContractType(IVssRequestContext requestContext) => requestContext.GetService<IDocumentContractTypeService>().GetSupportedQueryDocumentContractType(requestContext, this.m_entityType);

    public string GetESConnectionString(IVssRequestContext requestContext) => requestContext.GetService<IDocumentContractTypeService>().GetQueryESConnectionString(this.m_entityType);

    public IEnumerable<IndexInfo> GetTenantCodeQueryAlias(string accountName) => (IEnumerable<IndexInfo>) new List<IndexInfo>()
    {
      new IndexInfo() { IndexName = "Tenant_A@" + accountName }
    };

    public IEnumerable<IndexInfo> GetTenantWikiQueryAlias(string collectionName) => (IEnumerable<IndexInfo>) new List<IndexInfo>()
    {
      new IndexInfo()
      {
        IndexName = "Tenant_Wiki@" + collectionName
      }
    };
  }
}
