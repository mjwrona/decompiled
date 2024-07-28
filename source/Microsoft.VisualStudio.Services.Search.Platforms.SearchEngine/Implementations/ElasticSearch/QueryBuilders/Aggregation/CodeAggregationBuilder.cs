// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation.CodeAggregationBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation
{
  internal abstract class CodeAggregationBuilder : IAggregationBuilder
  {
    protected internal readonly IExpression QueryParseTree;
    protected internal readonly bool EnableAggregations;
    protected internal readonly IEnumerable<string> SelectedCEFilters;
    protected internal readonly IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> SelectedTermFilters;
    protected internal readonly Dictionary<string, List<string>> CodeTermFilterIdMap;
    protected internal readonly CodeFileContract contract;

    public CodeAggregationBuilder(
      DocumentContractType contractType,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> selectedSearchFilters,
      bool enableAggregations)
    {
      this.contract = CodeFileContract.GetContractInstance(contractType);
      if (queryParseTree == null)
        throw new ArgumentNullException(nameof (queryParseTree));
      if (selectedSearchFilters == null)
        throw new ArgumentNullException(nameof (selectedSearchFilters));
      if (selectedSearchFilters.ContainsKey("CodeElementFilters") && !selectedSearchFilters["CodeElementFilters"].Any<string>())
        throw new ArgumentException("selectedSearchFilter contains code element filter category with no filters", nameof (selectedSearchFilters));
      if (selectedSearchFilters.ContainsKey("AccountFilters") && !selectedSearchFilters["AccountFilters"].Any<string>())
        throw new ArgumentException("selectedSearchFilter contains account filter category with no filters", nameof (selectedSearchFilters));
      if (selectedSearchFilters.ContainsKey("CollectionFilters") && !selectedSearchFilters["CollectionFilters"].Any<string>())
        throw new ArgumentException("selectedSearchFilter contains collection filter category with no filters", nameof (selectedSearchFilters));
      if (selectedSearchFilters.ContainsKey("ProjectFilters") && !selectedSearchFilters["ProjectFilters"].Any<string>())
        throw new ArgumentException("selectedSearchFilter contains project filter category with no filters", nameof (selectedSearchFilters));
      if (selectedSearchFilters.ContainsKey("RepositoryFilters") && !selectedSearchFilters["RepositoryFilters"].Any<string>())
        throw new ArgumentException("selectedSearchFilter contains repository filter category with no filters", nameof (selectedSearchFilters));
      if (selectedSearchFilters.ContainsKey("PathFilters") && !selectedSearchFilters["PathFilters"].Any<string>())
        throw new ArgumentException("selectedSearchFilter contains path filter category with no filters", nameof (selectedSearchFilters));
      this.QueryParseTree = queryParseTree;
      this.EnableAggregations = enableAggregations;
      if (!this.EnableAggregations)
        return;
      if (!selectedSearchFilters.TryGetValue("CodeElementFilters", out this.SelectedCEFilters))
        this.SelectedCEFilters = Enumerable.Empty<string>();
      this.SelectedTermFilters = (IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>) new Dictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>();
      if (selectedSearchFilters.ContainsKey("AccountFilters"))
        this.SelectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.Account, selectedSearchFilters["AccountFilters"]);
      if (selectedSearchFilters.ContainsKey("CollectionFilters"))
        this.SelectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.CollectionName, selectedSearchFilters["CollectionFilters"]);
      if (selectedSearchFilters.ContainsKey("ProjectFilters"))
        this.SelectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.ProjectName, selectedSearchFilters["ProjectFilters"]);
      if (selectedSearchFilters.ContainsKey("RepositoryFilters"))
        this.SelectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.RepoName, selectedSearchFilters["RepositoryFilters"]);
      if (selectedSearchFilters.ContainsKey("PathFilters"))
        this.SelectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.FilePath, selectedSearchFilters["PathFilters"]);
      this.CodeTermFilterIdMap = this.ExtractCodeTermsInQueryExpression();
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Aggregations(new Func<AggregationContainerDescriptor<object>, IAggregationContainer>(this.Aggregates<object>)))).PrettyJson();

    public AggregationContainerDescriptor<T> Aggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      if (this.EnableAggregations)
      {
        if (this.SelectedTermFilters.TryGetValue(CodeFileContract.CodeContractQueryableElement.Account, out IEnumerable<string> _))
          aggDescriptor = this.AccountAggregationDescriptor<T>(aggDescriptor);
        else if (this.SelectedTermFilters.TryGetValue(CodeFileContract.CodeContractQueryableElement.CollectionName, out IEnumerable<string> _))
        {
          aggDescriptor = this.CollectionAggregates<T>(aggDescriptor);
        }
        else
        {
          if (this.ShouldComputeCEAggregations())
            aggDescriptor = this.CodeElementAggregates<T>(aggDescriptor);
          aggDescriptor = this.ProjectAndRepositoryAggregates<T>(aggDescriptor);
        }
      }
      return aggDescriptor;
    }

    protected internal abstract bool ShouldComputeCEAggregations();

    protected internal abstract AggregationContainerDescriptor<T> CodeElementAggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class;

    protected internal abstract void ProcessCodeTerms(
      Dictionary<string, List<string>> codeTermFilterIdMap);

    private static AggregationContainerDescriptor<T> TermAggregationDescriptor<T>(
      AggregationContainerDescriptor<T> aggDescriptor,
      string aggregateTermName,
      string aggregateName)
      where T : class
    {
      return aggDescriptor.Terms(aggregateName, (Func<TermsAggregationDescriptor<T>, ITermsAggregation>) (tad => (ITermsAggregation) tad.Field((Field) aggregateTermName).Size(new int?(CommonConstants.MaxNumberOfBucketsInTermsAggregations))));
    }

    private AggregationContainerDescriptor<T> ProjectAndRepositoryAggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      if (this.SelectedCEFilters.Any<string>() && this.ShouldComputeCEAggregations())
      {
        CodeFilterBuilderV3 codeElementFilterBuilder = new CodeFilterBuilderV3(this.QueryParseTree, (IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>) new Dictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>(), this.SelectedCEFilters, this.contract.ContractType);
        aggDescriptor = aggDescriptor.Filter("term_aggs", (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter(new Func<QueryContainerDescriptor<T>, QueryContainer>(codeElementFilterBuilder.CodeElementFilters<T>)).Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (aggDesc => (IAggregationContainer) this.ProjectAggregationDescriptor<T>(this.RepoAggregationDescriptor<T>(aggDesc))))));
      }
      else
        aggDescriptor = this.ProjectAggregationDescriptor<T>(this.RepoAggregationDescriptor<T>(aggDescriptor));
      return aggDescriptor;
    }

    private AggregationContainerDescriptor<T> CollectionAggregates<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      if (this.SelectedCEFilters.Any<string>() && this.ShouldComputeCEAggregations())
      {
        CodeFilterBuilderV3 codeElementFilterBuilder = new CodeFilterBuilderV3(this.QueryParseTree, (IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>) new Dictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>(), this.SelectedCEFilters, this.contract.ContractType);
        aggDescriptor = aggDescriptor.Filter("term_aggs", (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (f => (IFilterAggregation) f.Filter(new Func<QueryContainerDescriptor<T>, QueryContainer>(codeElementFilterBuilder.CodeElementFilters<T>)).Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (aggDesc => (IAggregationContainer) this.CollectionAggregationDescriptor<T>(aggDesc)))));
      }
      else
        aggDescriptor = this.CollectionAggregationDescriptor<T>(aggDescriptor);
      return aggDescriptor;
    }

    private AggregationContainerDescriptor<T> AccountAggregationDescriptor<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      AggregationContainerDescriptor<T> aggDescriptor1 = aggDescriptor;
      CodeContractField storedFieldForType = this.contract.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.Account);
      IEnumerable<CodeFileContract.CodeContractQueryableElement> filterFieldNames = Enumerable.Empty<CodeFileContract.CodeContractQueryableElement>();
      CodeContractField aggregateFieldName = storedFieldForType;
      return this.FieldAggregationDescriptor<T>(aggDescriptor1, filterFieldNames, aggregateFieldName, "account_aggs", "filtered_account_aggs");
    }

    private AggregationContainerDescriptor<T> CollectionAggregationDescriptor<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      AggregationContainerDescriptor<T> aggDescriptor1 = aggDescriptor;
      CodeContractField storedFieldForType = this.contract.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.CollectionName);
      IEnumerable<CodeFileContract.CodeContractQueryableElement> filterFieldNames = Enumerable.Empty<CodeFileContract.CodeContractQueryableElement>();
      CodeContractField aggregateFieldName = storedFieldForType;
      return this.FieldAggregationDescriptor<T>(aggDescriptor1, filterFieldNames, aggregateFieldName, "collection_aggs", "filtered_collection_aggs");
    }

    private AggregationContainerDescriptor<T> ProjectAggregationDescriptor<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      AggregationContainerDescriptor<T> aggDescriptor1 = aggDescriptor;
      CodeContractField storedFieldForType = this.contract.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.ProjectName);
      CodeFileContract.CodeContractQueryableElement[] filterFieldNames = new CodeFileContract.CodeContractQueryableElement[2]
      {
        CodeFileContract.CodeContractQueryableElement.RepoName,
        CodeFileContract.CodeContractQueryableElement.FilePath
      };
      CodeContractField aggregateFieldName = storedFieldForType;
      return this.FieldAggregationDescriptor<T>(aggDescriptor1, (IEnumerable<CodeFileContract.CodeContractQueryableElement>) filterFieldNames, aggregateFieldName, "project_aggs", "filtered_project_aggs");
    }

    private AggregationContainerDescriptor<T> RepoAggregationDescriptor<T>(
      AggregationContainerDescriptor<T> aggDescriptor)
      where T : class
    {
      IEnumerable<string> source;
      if (!this.SelectedTermFilters.TryGetValue(CodeFileContract.CodeContractQueryableElement.ProjectName, out source) || source.Count<string>() != 1)
        return aggDescriptor;
      AggregationContainerDescriptor<T> aggDescriptor1 = aggDescriptor;
      CodeContractField storedFieldForType = this.contract.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.RepoName);
      CodeFileContract.CodeContractQueryableElement[] filterFieldNames = new CodeFileContract.CodeContractQueryableElement[2]
      {
        CodeFileContract.CodeContractQueryableElement.ProjectName,
        CodeFileContract.CodeContractQueryableElement.FilePath
      };
      CodeContractField aggregateFieldName = storedFieldForType;
      return this.FieldAggregationDescriptor<T>(aggDescriptor1, (IEnumerable<CodeFileContract.CodeContractQueryableElement>) filterFieldNames, aggregateFieldName, "repository_aggs", "filtered_repository_aggs");
    }

    private AggregationContainerDescriptor<T> FieldAggregationDescriptor<T>(
      AggregationContainerDescriptor<T> aggDescriptor,
      IEnumerable<CodeFileContract.CodeContractQueryableElement> filterFieldNames,
      CodeContractField aggregateFieldName,
      string aggregateName,
      string filteredAggregateName)
      where T : class
    {
      if (!this.SelectedTermFilters.Keys.Intersect<CodeFileContract.CodeContractQueryableElement>(filterFieldNames).Any<CodeFileContract.CodeContractQueryableElement>())
        return CodeAggregationBuilder.TermAggregationDescriptor<T>(aggDescriptor, aggregateFieldName.ElasticsearchFieldName, aggregateName);
      CodeFilterBuilder termFilterBuilder = new CodeFilterBuilder(this.QueryParseTree, (IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>) this.SelectedTermFilters.Where<KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>>((Func<KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>, bool>) (m => filterFieldNames.Contains<CodeFileContract.CodeContractQueryableElement>(m.Key))).ToDictionary<KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>, CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>((Func<KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>, CodeFileContract.CodeContractQueryableElement>) (x => x.Key), (Func<KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>, IEnumerable<string>>) (x => x.Value)), this.contract.ContractType);
      return aggDescriptor.Filter(filteredAggregateName, (Func<FilterAggregationDescriptor<T>, IFilterAggregation>) (fad => (IFilterAggregation) fad.Filter(new Func<QueryContainerDescriptor<T>, QueryContainer>(termFilterBuilder.Filters<T>)).Aggregations(closure_0 ?? (closure_0 = (Func<AggregationContainerDescriptor<T>, IAggregationContainer>) (aggDesc => (IAggregationContainer) CodeAggregationBuilder.TermAggregationDescriptor<T>(aggDesc, aggregateFieldName.ElasticsearchFieldName, aggregateName))))));
    }

    private Dictionary<string, List<string>> ExtractCodeTermsInQueryExpression()
    {
      Dictionary<string, List<string>> inQueryExpression = new Dictionary<string, List<string>>();
      Queue<IExpression> expressionQueue = new Queue<IExpression>();
      expressionQueue.Enqueue(this.QueryParseTree);
      while (expressionQueue.Count > 0)
      {
        IExpression expression = expressionQueue.Dequeue();
        if (expression is TermExpression termExpression)
        {
          if (termExpression.IsOfType("*") || CodeSearchFilters.CEFilterIds.Contains<string>(termExpression.Type, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          {
            if (this.ShouldComputeAggregation(termExpression))
            {
              if (!inQueryExpression.ContainsKey(termExpression.Value))
              {
                if (inQueryExpression.Count == 0)
                {
                  inQueryExpression.Add(termExpression.Value, new List<string>()
                  {
                    termExpression.Type
                  });
                }
                else
                {
                  inQueryExpression.Clear();
                  break;
                }
              }
              else
                inQueryExpression[termExpression.Value].Add(termExpression.Type);
            }
            else
            {
              inQueryExpression.Clear();
              break;
            }
          }
        }
        else
        {
          foreach (IExpression child in expression.Children)
            expressionQueue.Enqueue(child);
        }
      }
      return inQueryExpression;
    }

    private bool ShouldComputeAggregation(TermExpression termExpression) => !termExpression.Value.ContainsWhitespaceOrSpecialCharacters() && !RegularExpressions.PrefixWildcardRegex.Match(termExpression.Value).Success && (!termExpression.IsOfType("*") || !RegularExpressions.WildcardRegex.Match(termExpression.Value).Success);
  }
}
