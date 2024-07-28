// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.CodeFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class CodeFilterBuilder
  {
    private Dictionary<string, HashSet<string>> m_codeTokenToCodeFilterMap;
    private Dictionary<CodeFileContract.CodeContractQueryableElement, List<string>> m_selectedTermFilters;

    protected CodeFileContract CodeFileContract { get; }

    public CodeFilterBuilder(
      IExpression queryParseTree,
      IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> selectedTermFilters,
      DocumentContractType contractType)
    {
      if (queryParseTree == null)
        throw new ArgumentNullException(nameof (queryParseTree));
      if (selectedTermFilters == null)
        throw new ArgumentNullException(nameof (selectedTermFilters));
      this.CodeFileContract = CodeFileContract.GetContractInstance(contractType);
      this.SetupPrivateFields(queryParseTree, selectedTermFilters);
    }

    public CodeFilterBuilder(
      IExpression queryParseTree,
      DocumentContractType contractType,
      IDictionary<string, IEnumerable<string>> selectedSearchFilters)
    {
      if (queryParseTree == null)
        throw new ArgumentNullException(nameof (queryParseTree));
      if (selectedSearchFilters == null)
        throw new ArgumentNullException(nameof (selectedSearchFilters));
      this.CodeFileContract = CodeFileContract.GetContractInstance(contractType);
      Dictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> selectedTermFilters = new Dictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>();
      IEnumerable<string> source1;
      if (selectedSearchFilters.TryGetValue("ProjectFilters", out source1))
      {
        List<string> stringList = new List<string>(source1.Count<string>());
        foreach (string text in source1)
          stringList.Add(text.NormalizeStringAndReplaceTurkishDottedI());
        IEnumerable<string> strings = (IEnumerable<string>) stringList;
        selectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.ProjectName, strings);
      }
      IEnumerable<string> source2;
      if (selectedSearchFilters.TryGetValue("RepositoryFilters", out source2))
      {
        List<string> stringList = new List<string>(source2.Count<string>());
        foreach (string text in source2)
          stringList.Add(text.NormalizeStringAndReplaceTurkishDottedI());
        IEnumerable<string> strings = (IEnumerable<string>) stringList;
        selectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.RepoName, strings);
      }
      IEnumerable<string> strings1;
      if (selectedSearchFilters.TryGetValue("PathFilters", out strings1))
        selectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.FilePath, strings1);
      IEnumerable<string> source3;
      if (selectedSearchFilters.TryGetValue("BranchFilters", out source3))
      {
        List<string> list = source3.ToList<string>();
        bool flag = list.Contains("#Default#");
        if (!list.Contains("#All#"))
        {
          if (flag)
          {
            list.RemoveAll((Predicate<string>) (branch => branch.Equals("#Default#", StringComparison.Ordinal)));
            selectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch, (IEnumerable<string>) new List<string>()
            {
              "true"
            });
          }
          selectedTermFilters.Add(CodeFileContract.CodeContractQueryableElement.BranchName, (IEnumerable<string>) list);
        }
      }
      this.SetupPrivateFields(queryParseTree, (IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>) selectedTermFilters);
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    public virtual IExpression GetQueryFilterExpression()
    {
      List<IExpression> source = new List<IExpression>();
      foreach (KeyValuePair<CodeFileContract.CodeContractQueryableElement, List<string>> selectedTermFilter in this.m_selectedTermFilters)
        source.Add((IExpression) new TermsExpression(this.CodeFileContract.GetSearchFieldForType(selectedTermFilter.Key.InlineFilterName()), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.In, selectedTermFilter.Value.Select<string, string>((Func<string, string>) (x => x.ToString((IFormatProvider) CultureInfo.InvariantCulture).ToLowerInvariant()))));
      IExpression filterExpression = (IExpression) new EmptyExpression();
      if (source.Count > 0)
        filterExpression = source.Count == 1 ? source[0] : source.Aggregate<IExpression>((Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
        {
          current,
          filter
        })));
      return filterExpression;
    }

    internal virtual QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_selectedTermFilters.Select<KeyValuePair<CodeFileContract.CodeContractQueryableElement, List<string>>, QueryContainer>((Func<KeyValuePair<CodeFileContract.CodeContractQueryableElement, List<string>>, QueryContainer>) (fCat => Query<T>.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bs => (IBoolQuery) bs.Should(fCat.Value.Select<string, QueryContainer>((Func<string, QueryContainer>) (fVal => Query<T>.Term((Field) this.CodeFileContract.GetSearchFieldForType(fCat.Key.InlineFilterName()), (object) fVal))).ToArray<QueryContainer>()))))).ToArray<QueryContainer>())));

    internal Dictionary<string, HashSet<string>> GetCodeTokenToCodeElementFilltersMap() => this.m_codeTokenToCodeFilterMap;

    internal IDictionary<CodeFileContract.CodeContractQueryableElement, List<string>> GetSelectedTermFilters() => (IDictionary<CodeFileContract.CodeContractQueryableElement, List<string>>) this.m_selectedTermFilters;

    private void GetAllCodeTokensInQuery(IExpression expression)
    {
      if (expression is TermExpression termExpression)
      {
        if (termExpression.IsOfType("*"))
        {
          if (this.m_codeTokenToCodeFilterMap.ContainsKey(termExpression.Value))
            return;
          this.m_codeTokenToCodeFilterMap.Add(termExpression.Value, new HashSet<string>());
        }
        else
        {
          if (!CodeSearchFilters.CEFilterIds.Contains<string>(termExpression.Type, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            return;
          if (this.m_codeTokenToCodeFilterMap.ContainsKey(termExpression.Value))
            this.m_codeTokenToCodeFilterMap[termExpression.Value].Add(termExpression.Type);
          else
            this.m_codeTokenToCodeFilterMap.Add(termExpression.Value, new HashSet<string>()
            {
              termExpression.Type
            });
        }
      }
      else
      {
        for (int index = 0; index < expression.Children.Length; ++index)
          this.GetAllCodeTokensInQuery(expression.Children[index]);
      }
    }

    private void NormalizeTermFilters(
      IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> selectedTermFilters)
    {
      this.m_selectedTermFilters = new Dictionary<CodeFileContract.CodeContractQueryableElement, List<string>>();
      foreach (KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> selectedTermFilter in (IEnumerable<KeyValuePair<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>>>) selectedTermFilters)
      {
        List<string> stringList = new List<string>();
        foreach (string text in selectedTermFilter.Value)
          stringList.Add(text.NormalizeString());
        this.m_selectedTermFilters.Add(selectedTermFilter.Key, stringList);
      }
    }

    private void SetupPrivateFields(
      IExpression queryParseTree,
      IDictionary<CodeFileContract.CodeContractQueryableElement, IEnumerable<string>> selectedTermFilters)
    {
      this.NormalizeTermFilters(selectedTermFilters);
      this.m_codeTokenToCodeFilterMap = new Dictionary<string, HashSet<string>>();
      this.GetAllCodeTokensInQuery(queryParseTree);
    }
  }
}
