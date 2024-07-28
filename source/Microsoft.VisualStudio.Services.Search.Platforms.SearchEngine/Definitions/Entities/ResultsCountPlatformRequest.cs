// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ResultsCountPlatformRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities
{
  public class ResultsCountPlatformRequest
  {
    public ResultsCountPlatformRequest(
      string requestId,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> indexInfo,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression scopeFiltersExpression,
      DocumentContractType contractType,
      AbstractSearchDocumentContract docContract,
      int terminateAfter)
    {
      this.RequestId = requestId;
      this.IndexInfo = indexInfo;
      this.QueryParseTree = queryParseTree;
      this.SearchFilters = searchFilters;
      this.ScopeFiltersExpression = scopeFiltersExpression;
      this.ContractType = contractType;
      this.TerminateAfter = terminateAfter;
      this.DocumentContract = docContract;
    }

    protected ResultsCountPlatformRequest() => this.IndexInfo = Enumerable.Empty<Microsoft.VisualStudio.Services.Search.Common.IndexInfo>();

    public AbstractSearchDocumentContract DocumentContract { get; set; }

    public string RequestId { get; set; }

    public IExpression QueryParseTree { get; set; }

    public IExpression ScopeFiltersExpression { get; set; }

    public DocumentContractType ContractType { get; set; }

    public IDictionary<string, IEnumerable<string>> SearchFilters { get; set; }

    public IEnumerable<IndexName> Indices => this.IndexInfo.Select<Microsoft.VisualStudio.Services.Search.Common.IndexInfo, IndexName>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexInfo, IndexName>) (i => (IndexName) i.IndexName));

    public int MaxInnerHits { get; set; }

    public string[] Routing
    {
      get
      {
        if (this.IndexInfo.IsNullOrEmpty<Microsoft.VisualStudio.Services.Search.Common.IndexInfo>())
          return (string[]) null;
        if (this.IndexInfo.Count<Microsoft.VisualStudio.Services.Search.Common.IndexInfo>() == 1)
          return this.IndexInfo.First<Microsoft.VisualStudio.Services.Search.Common.IndexInfo>().Routing?.Split(',');
        HashSet<string> stringSet = new HashSet<string>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexInfo indexInfo in this.IndexInfo)
        {
          if (string.IsNullOrWhiteSpace(indexInfo.Routing))
            return (string[]) null;
          stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) indexInfo.Routing.Split(','));
        }
        return stringSet.ToArray<string>();
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> IndexInfo { get; set; }

    public int TerminateAfter { get; set; }

    public ResultsCountPlatformResponse Count(IVssRequestContext requestContext) => this.DocumentContract.Count(requestContext, this);
  }
}
