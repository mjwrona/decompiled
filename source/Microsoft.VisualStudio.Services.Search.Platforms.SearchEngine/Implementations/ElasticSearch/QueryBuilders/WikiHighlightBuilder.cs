// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WikiHighlightBuilder
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
  internal class WikiHighlightBuilder
  {
    private const string WikiHighlighterType = "wiki-unified";
    private const string HighlightStartTag = "<highlighthit>";
    private const string HighlightEndTag = "</highlighthit>";
    private const int DefaultMaxNumberOfFragments = 3;
    private IReadOnlyCollection<string> m_highlightFields;
    private readonly IExpression m_queryParseTree;
    private bool m_enableHighlighting;
    private const string ContentLinksLowerField = "contentLinks.lower";

    public WikiHighlightBuilder(
      IExpression queryParseTree,
      IReadOnlyCollection<string> highlightFields,
      bool enableHighlighting)
    {
      this.m_queryParseTree = queryParseTree;
      this.m_enableHighlighting = enableHighlighting;
      if (!this.m_enableHighlighting)
        return;
      if (highlightFields == null || highlightFields.Count <= 0)
        throw new ArgumentException("Highlighting is enabled but highlight field is missing", nameof (highlightFields));
      foreach (string highlightField in (IEnumerable<string>) highlightFields)
      {
        if (string.IsNullOrWhiteSpace(highlightField))
          throw new ArgumentException("Highlight field contains whitespace or null value", nameof (highlightFields));
      }
      this.m_highlightFields = highlightFields;
    }

    internal Func<HighlightDescriptor<T>, IHighlight> Highlight<T>(IVssRequestContext requestContext) where T : class
    {
      IReadOnlyCollection<string> highlightFields = this.GetHighlightFields(this.m_queryParseTree);
      if (!this.m_enableHighlighting)
        return (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h);
      List<Func<HighlightFieldDescriptor<T>, IHighlightField>> descriptors = new List<Func<HighlightFieldDescriptor<T>, IHighlightField>>();
      foreach (string platformFieldName in (IEnumerable<string>) highlightFields)
        descriptors.Add(this.GetDescriptorsForField<T>(requestContext, platformFieldName));
      return (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h.PreTags("<highlighthit>").PostTags("</highlighthit>").Fields(descriptors.ToArray()));
    }

    public string ToString(IVssRequestContext requestContext) => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Highlight(this.Highlight<object>(requestContext)))).PrettyJson();

    private Func<HighlightFieldDescriptor<T>, IHighlightField> GetDescriptorsForField<T>(
      IVssRequestContext requestContext,
      string platformFieldName)
      where T : class
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxHighlightFragmentSize = service.GetValue<int>(requestContext, (RegistryQuery) "/service/ALMSearch/Settings/WikiHighlightFragmentSize", true, 0);
      int noMatchSize = service.GetValue<int>(requestContext, (RegistryQuery) "/service/ALMSearch/Settings/WikiNoMatchSize", true, 85);
      if ("content".Equals(platformFieldName, StringComparison.Ordinal))
        return (Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) platformFieldName).NumberOfFragments(new int?(this.GetHighlightFragmentCount(requestContext, platformFieldName))).FragmentSize(new int?(maxHighlightFragmentSize)).Type("wiki-unified").RequireFieldMatch(new bool?(false)).NoMatchSize(new int?(noMatchSize)));
      bool requireFieldMatch = !"contentLinks.lower".Equals(platformFieldName, StringComparison.Ordinal);
      return (Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) platformFieldName).NumberOfFragments(new int?(this.GetHighlightFragmentCount(requestContext, platformFieldName))).FragmentSize(new int?(maxHighlightFragmentSize)).Type("wiki-unified").RequireFieldMatch(new bool?(requireFieldMatch)));
    }

    private int GetHighlightFragmentCount(IVssRequestContext requestContext, string field)
    {
      switch (field)
      {
        case "fileNames":
        case "tags":
        case "contentLinks.lower":
        case "projectName.search":
        case "collectionName.search":
          return 0;
        case "content":
          return requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/service/ALMSearch/Settings/WikiMaxNumberOfFragmentsForContentField", true, 3);
        default:
          return 3;
      }
    }

    private IReadOnlyCollection<string> GetHighlightFields(IExpression queryParseTree)
    {
      if (queryParseTree == null || !this.HasOnlyContentLinksInQueryTree(queryParseTree))
        return this.m_highlightFields;
      return (IReadOnlyCollection<string>) new List<string>()
      {
        "contentLinks.lower"
      };
    }

    private bool HasOnlyContentLinksInQueryTree(IExpression queryParseTree)
    {
      foreach (IExpression expression in (IEnumerable<IExpression>) queryParseTree)
      {
        if (expression is TermExpression termExpression && (termExpression.IsOfType("*") || !termExpression.Type.Equals("contentLinks.lower", StringComparison.Ordinal)))
          return false;
      }
      return true;
    }
  }
}
