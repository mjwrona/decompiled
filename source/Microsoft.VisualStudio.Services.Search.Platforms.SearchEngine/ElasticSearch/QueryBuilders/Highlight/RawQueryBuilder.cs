// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight.RawQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight
{
  public class RawQueryBuilder
  {
    private string m_highlightRawQuery;

    public RawQueryBuilder(string rawQuery) => this.m_highlightRawQuery = rawQuery;

    internal QueryContainer HighlighterQuery<T>(QueryContainerDescriptor<T> queryDescriptor) where T : class => queryDescriptor.Raw(this.m_highlightRawQuery);
  }
}
