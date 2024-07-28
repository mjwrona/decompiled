// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeTokenFilters
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (UnionListFormatter<AnalyzeTokenFilters, string, ITokenFilter>))]
  public class AnalyzeTokenFilters : List<Union<string, ITokenFilter>>
  {
    public AnalyzeTokenFilters()
    {
    }

    public AnalyzeTokenFilters(List<Union<string, ITokenFilter>> tokenFilters)
    {
      if (tokenFilters == null)
        return;
      foreach (Union<string, ITokenFilter> tokenFilter in tokenFilters)
        this.AddIfNotNull<Union<string, ITokenFilter>>(tokenFilter);
    }

    public AnalyzeTokenFilters(string[] tokenFilters)
    {
      if (tokenFilters == null)
        return;
      foreach (string tokenFilter in tokenFilters)
        this.AddIfNotNull<Union<string, ITokenFilter>>((Union<string, ITokenFilter>) tokenFilter);
    }

    public void Add(ITokenFilter filter) => this.Add(new Union<string, ITokenFilter>(filter));

    public static implicit operator AnalyzeTokenFilters(TokenFilterBase tokenFilter)
    {
      if (tokenFilter == null)
        return (AnalyzeTokenFilters) null;
      return new AnalyzeTokenFilters()
      {
        (ITokenFilter) tokenFilter
      };
    }

    public static implicit operator AnalyzeTokenFilters(string tokenFilter)
    {
      if (tokenFilter == null)
        return (AnalyzeTokenFilters) null;
      AnalyzeTokenFilters analyzeTokenFilters = new AnalyzeTokenFilters();
      analyzeTokenFilters.Add((Union<string, ITokenFilter>) tokenFilter);
      return analyzeTokenFilters;
    }

    public static implicit operator AnalyzeTokenFilters(string[] tokenFilters) => tokenFilters != null ? new AnalyzeTokenFilters(tokenFilters) : (AnalyzeTokenFilters) null;
  }
}
