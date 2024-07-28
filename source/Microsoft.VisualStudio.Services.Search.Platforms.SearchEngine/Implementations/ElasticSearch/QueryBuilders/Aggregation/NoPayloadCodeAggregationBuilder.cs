// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.NoPayloadCodeAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  internal class NoPayloadCodeAggregationBuilder : CodeAggregationBuilder
  {
    public NoPayloadCodeAggregationBuilder(
      DocumentContractType contractType,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> selectedSearchFilters,
      bool enableAggregations)
      : base(contractType, queryParseTree, selectedSearchFilters, enableAggregations)
    {
    }

    protected internal override bool ShouldComputeCEAggregations() => false;

    protected internal override AggregationContainerDescriptor<T> CodeElementAggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
    {
      throw new NotSupportedException();
    }

    protected internal override void ProcessCodeTerms(
      Dictionary<string, List<string>> codeTermFilterIdMap)
    {
      throw new NotSupportedException();
    }
  }
}
