// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.PackageHighlightBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class PackageHighlightBuilder
  {
    public const string HighlighterTypev5 = "unified";
    public const string HighlighterTypev2 = "postings";
    public const string HighlightStartTag = "<highlighthit>";
    public const string HighlightEndTag = "</highlighthit>";
    public const int MaxHighlightFragmentSize = 85;
    public const int MaxNumberOfFragmentsForContentField = 3;
    public const int DefaultMaxNumberOfFragments = 3;
    private readonly IReadOnlyCollection<string> m_highlightFields = (IReadOnlyCollection<string>) new List<string>()
    {
      "name",
      "name.casechangeanalyzed",
      "description",
      "version"
    };
    private readonly bool m_enableHighlighting;

    public PackageHighlightBuilder(bool enableHighlighting) => this.m_enableHighlighting = enableHighlighting;

    internal Func<HighlightDescriptor<T>, IHighlight> Highlight<T>() where T : class
    {
      if (!this.m_enableHighlighting)
        return (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h);
      List<Func<HighlightFieldDescriptor<T>, IHighlightField>> descriptors = new List<Func<HighlightFieldDescriptor<T>, IHighlightField>>();
      foreach (string highlightField in (IEnumerable<string>) this.m_highlightFields)
        descriptors.Add(this.GetDescriptorsForField<T>(highlightField));
      return (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h.PreTags("<highlighthit>").PostTags("</highlighthit>").Fields(descriptors.ToArray()));
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Highlight(this.Highlight<object>()))).PrettyJson();

    private Func<HighlightFieldDescriptor<T>, IHighlightField> GetDescriptorsForField<T>(
      string platformFieldName)
      where T : class
    {
      string highlighterType = "unified";
      return (Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) platformFieldName).NumberOfFragments(new int?(this.GetHighlightFragmentCount())).FragmentSize(new int?(85)).Type(highlighterType).RequireFieldMatch());
    }

    private int GetHighlightFragmentCount() => 3;
  }
}
