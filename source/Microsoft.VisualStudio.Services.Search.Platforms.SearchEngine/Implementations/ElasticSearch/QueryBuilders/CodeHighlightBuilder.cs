// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.CodeHighlightBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Nest;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class CodeHighlightBuilder
  {
    private string m_highlighterName;
    private string m_highlightField;
    private bool m_enableHighlighting;

    public CodeHighlightBuilder(
      string highlighterName,
      string highlightField,
      bool enableHighlighting)
    {
      this.m_highlighterName = highlighterName;
      this.m_enableHighlighting = enableHighlighting;
      if (!this.m_enableHighlighting)
        return;
      this.m_highlightField = !string.IsNullOrWhiteSpace(highlightField) ? highlightField : throw new ArgumentException("Highlighting is enabled but highlight field is missing", nameof (highlightField));
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Highlight(this.Highlight<object>()))).PrettyJson();

    internal Func<HighlightDescriptor<T>, IHighlight> Highlight<T>() where T : class => this.m_enableHighlighting ? (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h.Fields((Func<HighlightFieldDescriptor<T>, IHighlightField>) (f => (IHighlightField) f.Field((Field) this.m_highlightField).Type(this.m_highlighterName)))) : (Func<HighlightDescriptor<T>, IHighlight>) (h => (IHighlight) h);
  }
}
