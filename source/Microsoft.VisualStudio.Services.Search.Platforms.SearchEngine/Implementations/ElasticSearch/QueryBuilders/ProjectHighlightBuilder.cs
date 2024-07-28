// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.ProjectHighlightBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class ProjectHighlightBuilder
  {
    private IList<string> m_highlightFields;
    private bool m_enableHighlighting;
    public const string HighlightStartTag = "<highlighthit>";
    public const string HighlightEndTag = "</highlighthit>";
    public const string HighlighterTypeV5 = "unified";
    public const int MaxNumberOfHighlightFragments = 3;
    public const int MaxHighlightFragmentSize = 150;

    public ProjectHighlightBuilder(IEnumerable<string> highlightFields, bool enableHighlighting)
    {
      this.m_highlightFields = (IList<string>) new List<string>();
      this.m_enableHighlighting = enableHighlighting;
      if (!this.m_enableHighlighting)
        return;
      foreach (string highlightField in highlightFields)
      {
        if (string.IsNullOrWhiteSpace(highlightField))
          throw new ArgumentException("Highlight field contains whitespace or null value", nameof (highlightFields));
        this.m_highlightFields.Add(highlightField);
      }
      if (this.m_highlightFields.Count == 0)
        throw new ArgumentException("Highlighting is enabled but highlight fields are missing", nameof (highlightFields));
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<ProjectContract>>(new SearchDescriptor<ProjectContract>().Highlight(this.Highlight<ProjectContract>()))).PrettyJson();

    internal Func<HighlightDescriptor<T>, IHighlight> Highlight<T>() where T : class => this.m_enableHighlighting ? (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h.PreTags("<highlighthit>").PostTags("</highlighthit>").RequireFieldMatch().Fields(this.GetHighlightFields<T>())) : (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h);

    private Func<HighlightFieldDescriptor<T>, IHighlightField>[] GetHighlightFields<T>() where T : class
    {
      string highlighterType = "unified";
      Func<HighlightFieldDescriptor<T>, IHighlightField>[] highlightFields = new Func<HighlightFieldDescriptor<T>, IHighlightField>[this.m_highlightFields.Count];
      int num = 0;
      foreach (string highlightField in (IEnumerable<string>) this.m_highlightFields)
      {
        string field = highlightField;
        highlightFields[num++] = (Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) field).NumberOfFragments(new int?(this.GetHighlightFragmentCount(field))).FragmentSize(new int?(150)).Type(highlighterType));
      }
      return highlightFields;
    }

    private int GetHighlightFragmentCount(string field) => field == "name" || field == "tags" || field == "tags.lower" || field == "languages.analysed" ? 0 : 3;
  }
}
