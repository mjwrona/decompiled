// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeCharFilters
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (UnionListFormatter<AnalyzeCharFilters, string, ICharFilter>))]
  public class AnalyzeCharFilters : List<Union<string, ICharFilter>>
  {
    public AnalyzeCharFilters()
    {
    }

    public AnalyzeCharFilters(List<Union<string, ICharFilter>> tokenFilters)
    {
      if (tokenFilters == null)
        return;
      foreach (Union<string, ICharFilter> tokenFilter in tokenFilters)
        this.AddIfNotNull<Union<string, ICharFilter>>(tokenFilter);
    }

    public AnalyzeCharFilters(string[] tokenFilters)
    {
      if (tokenFilters == null)
        return;
      foreach (string tokenFilter in tokenFilters)
        this.AddIfNotNull<Union<string, ICharFilter>>((Union<string, ICharFilter>) tokenFilter);
    }

    public void Add(ICharFilter filter) => this.Add(new Union<string, ICharFilter>(filter));

    public void Add(string filter) => this.Add(new Union<string, ICharFilter>(filter));

    public static implicit operator AnalyzeCharFilters(CharFilterBase tokenFilter)
    {
      if (tokenFilter == null)
        return (AnalyzeCharFilters) null;
      return new AnalyzeCharFilters()
      {
        (ICharFilter) tokenFilter
      };
    }

    public static implicit operator AnalyzeCharFilters(string tokenFilter)
    {
      if (tokenFilter == null)
        return (AnalyzeCharFilters) null;
      return new AnalyzeCharFilters() { tokenFilter };
    }

    public static implicit operator AnalyzeCharFilters(string[] tokenFilters) => tokenFilters != null ? new AnalyzeCharFilters(tokenFilters) : (AnalyzeCharFilters) null;
  }
}
