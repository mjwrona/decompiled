// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.CodePayloadAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  internal class CodePayloadAggregationBuilder : CodeAggregationBuilder
  {
    private List<object> m_payloadTerms;

    public CodePayloadAggregationBuilder(
      DocumentContractType contractType,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> selectedSearchFilters,
      bool enableAggregations)
      : base(contractType, queryParseTree, selectedSearchFilters, enableAggregations)
    {
      if (!this.EnableAggregations)
        return;
      this.ProcessCodeTerms(this.CodeTermFilterIdMap);
    }

    protected internal override AggregationContainerDescriptor<T> CodeElementAggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
    {
      if (this.SelectedTermFilters.ContainsKey(CodeFileContract.CodeContractQueryableElement.ProjectName) || this.SelectedTermFilters.ContainsKey(CodeFileContract.CodeContractQueryableElement.RepoName) || this.SelectedTermFilters.ContainsKey(CodeFileContract.CodeContractQueryableElement.FilePath) || this.SelectedTermFilters.ContainsKey(CodeFileContract.CodeContractQueryableElement.BranchName))
      {
        CodeFilterBuilder filterBuilder = new CodeFilterBuilder(this.QueryParseTree, this.SelectedTermFilters, this.contract.ContractType);
        aggDescriptor = aggDescriptor.Filter("filtered_code_element_aggs", (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter(new Func<QueryContainerDescriptor<T>, QueryContainer>(filterBuilder.Filters<T>))));
      }
      else
        aggDescriptor = (AggregationContainerDescriptor<T>) null;
      return aggDescriptor;
    }

    protected internal override bool ShouldComputeCEAggregations() => this.m_payloadTerms.Any<object>();

    protected internal override sealed void ProcessCodeTerms(
      Dictionary<string, List<string>> codeTermFilterIdMap)
    {
      this.m_payloadTerms = new List<object>();
      foreach (KeyValuePair<string, List<string>> codeTermFilterId in codeTermFilterIdMap)
      {
        string key = codeTermFilterId.Key;
        foreach (string str in codeTermFilterId.Value)
        {
          if (this.m_payloadTerms.FirstOrDefault<object>() == null)
            this.m_payloadTerms.Add(new object());
        }
      }
    }
  }
}
