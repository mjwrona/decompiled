// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.RescoreBuilderBase
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal abstract class RescoreBuilderBase : IEntityRescoreBuilder
  {
    protected readonly bool m_enableRescore;
    protected readonly IExpression m_queryParseTree;

    public RescoreBuilderBase(IExpression queryParseTree, bool enableRescore)
    {
      this.m_enableRescore = enableRescore;
      this.m_queryParseTree = queryParseTree;
    }

    public virtual Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>> Rescore<T>(
      IVssRequestContext requestContext)
      where T : class
    {
      throw new NotImplementedException("Implement in the child class");
    }

    public string ToString(IVssRequestContext requestContext) => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Rescore(this.Rescore<object>(requestContext)))).PrettyJson();

    protected bool IsValidQueryForProximityRescore()
    {
      IExpression queryParseTree = this.m_queryParseTree;
      int num1;
      if (queryParseTree == null)
      {
        num1 = 0;
      }
      else
      {
        int? length = queryParseTree.Children?.Length;
        int num2 = 1;
        num1 = length.GetValueOrDefault() > num2 & length.HasValue ? 1 : 0;
      }
      bool flag = num1 != 0;
      return this.m_enableRescore && flag && !string.IsNullOrWhiteSpace(this.GetSearchText(this.m_queryParseTree));
    }

    protected string GetSearchText(IExpression queryParseTree)
    {
      IList<string> values = (IList<string>) new List<string>();
      foreach (IExpression child in queryParseTree.Children)
      {
        if (child is TermExpression termExpression)
          values.Add(termExpression.Value);
      }
      return string.Join(" ", (IEnumerable<string>) values);
    }
  }
}
