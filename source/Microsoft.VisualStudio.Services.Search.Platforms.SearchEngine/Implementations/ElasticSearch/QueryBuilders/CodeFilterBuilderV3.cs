// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.CodeFilterBuilderV3
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
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class CodeFilterBuilderV3 : CodeFilterBuilder
  {
    private IEnumerable<string> m_selectedCeFilterIds;

    public CodeFilterBuilderV3(
      IExpression queryParseTree,
      IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> selectedTermFilters,
      IEnumerable<string> selectedCEFilters,
      DocumentContractType contractType)
      : base(queryParseTree, selectedTermFilters, contractType)
    {
      if (selectedCEFilters == null)
        throw new ArgumentNullException(nameof (selectedCEFilters));
      this.NormalizeCodeElementFilters(selectedCEFilters);
    }

    public CodeFilterBuilderV3(
      IExpression queryParseTree,
      DocumentContractType contractType,
      IDictionary<string, IEnumerable<string>> selectedSearchFilters)
      : base(queryParseTree, contractType, selectedSearchFilters)
    {
      IEnumerable<string> selectedCEFilterIds;
      if (!selectedSearchFilters.TryGetValue("CodeElementFilters", out selectedCEFilterIds))
        selectedCEFilterIds = Enumerable.Empty<string>();
      this.NormalizeCodeElementFilters(selectedCEFilterIds);
    }

    internal override QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor)
    {
      QueryContainer queryContainer = base.Filters<T>(filterDescriptor);
      return queryContainer ? queryContainer & this.CodeElementFilters<T>(filterDescriptor) : queryContainer;
    }

    public override IExpression GetQueryFilterExpression() => (IExpression) new AndExpression(new IExpression[2]
    {
      this.GetCEFilterExpression(),
      base.GetQueryFilterExpression()
    });

    internal virtual QueryContainer CodeElementFilters<T>(
      QueryContainerDescriptor<T> filterDescriptor)
      where T : class
    {
      IList<int> allSelectedCEFacetTokenKinds = (IList<int>) this.m_selectedCeFilterIds.SelectMany<string, int>((Func<string, IEnumerable<int>>) (f => CodeSearchFilters.CEFilterAttributeMap[f].TokenIds)).ToList<int>();
      Dictionary<string, HashSet<string>> codeTokenToCodeFilterMap = this.GetCodeTokenToCodeElementFilltersMap();
      return allSelectedCEFacetTokenKinds.Count > 0 ? filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bs => (IBoolQuery) bs.Should(codeTokenToCodeFilterMap.Select<KeyValuePair<string, HashSet<string>>, Func<QueryContainerDescriptor<T>, QueryContainer>>((Func<KeyValuePair<string, HashSet<string>>, Func<QueryContainerDescriptor<T>, QueryContainer>>) (tokenFilterPair =>
      {
        IList<int> list = (IList<int>) tokenFilterPair.Value.SelectMany<string, int>((Func<string, IEnumerable<int>>) (c => CodeSearchFilters.CEFilterAttributeMap[c].TokenIds)).ToList<int>();
        IEnumerable<int> codeTokenIds = list.Count > 0 ? allSelectedCEFacetTokenKinds.Intersect<int>((IEnumerable<int>) list) : (IEnumerable<int>) allSelectedCEFacetTokenKinds;
        return this.GetQueryContainer<T>(tokenFilterPair.Key, codeTokenIds);
      })).ToArray<Func<QueryContainerDescriptor<T>, QueryContainer>>()))) : (QueryContainer) null;
    }

    private IExpression GetCEFilterExpression()
    {
      IList<int> list1 = (IList<int>) this.m_selectedCeFilterIds.SelectMany<string, int>((Func<string, IEnumerable<int>>) (f => CodeSearchFilters.CEFilterAttributeMap[f].TokenIds)).ToList<int>();
      Dictionary<string, HashSet<string>> elementFilltersMap = this.GetCodeTokenToCodeElementFilltersMap();
      List<IExpression> source = new List<IExpression>();
      if (list1.Count <= 0)
        return (IExpression) new EmptyExpression();
      foreach (KeyValuePair<string, HashSet<string>> keyValuePair in elementFilltersMap)
      {
        IList<int> list2 = (IList<int>) keyValuePair.Value.SelectMany<string, int>((Func<string, IEnumerable<int>>) (c => CodeSearchFilters.CEFilterAttributeMap[c].TokenIds)).ToList<int>();
        IEnumerable<int> tokenIds = list2.Count > 0 ? list1.Intersect<int>((IEnumerable<int>) list2) : (IEnumerable<int>) list1;
        source.Add((IExpression) new CodeElementFilterExpression(keyValuePair.Key, tokenIds));
      }
      IExpression filterExpression = (IExpression) new EmptyExpression();
      if (source.Count > 0)
        filterExpression = source.Count == 1 ? source[0] : source.Aggregate<IExpression>((Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new OrExpression(new IExpression[2]
        {
          current,
          filter
        })));
      return filterExpression;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Func<QueryContainerDescriptor<T>, QueryContainer> GetQueryContainer<T>(
      string tokenValue,
      IEnumerable<int> codeTokenIds)
      where T : class
    {
      return (Func<QueryContainerDescriptor<T>, QueryContainer>) (fd => fd.Raw(this.CodeFileContract.CreateCodeElementQueryString(tokenValue, codeTokenIds, false, string.Empty)));
    }

    internal IEnumerable<string> GetSelectedCodeElementFilters() => this.m_selectedCeFilterIds;

    private void NormalizeCodeElementFilters(IEnumerable<string> selectedCEFilterIds)
    {
      List<string> stringList = new List<string>();
      foreach (string selectedCeFilterId in selectedCEFilterIds)
        stringList.Add(selectedCeFilterId.NormalizeString());
      this.m_selectedCeFilterIds = (IEnumerable<string>) stringList;
    }
  }
}
