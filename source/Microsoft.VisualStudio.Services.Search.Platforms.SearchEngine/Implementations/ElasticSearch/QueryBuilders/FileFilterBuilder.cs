// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.FileFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class FileFilterBuilder
  {
    private Dictionary<string, List<string>> m_selectedTermFilters;

    public FileFilterBuilder(
      IDictionary<string, IEnumerable<string>> selectedTermFilters)
    {
      if (selectedTermFilters == null)
        throw new ArgumentNullException(nameof (selectedTermFilters));
      this.SetupPrivateFields(selectedTermFilters);
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Filter(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.Filters<object>)))).PrettyJson();

    internal QueryContainer Filters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => this.TermFilters<T>(filterDescriptor);

    internal QueryContainer TermFilters<T>(QueryContainerDescriptor<T> filterDescriptor) where T : class => filterDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bm => (IBoolQuery) bm.Must(this.m_selectedTermFilters.Select<KeyValuePair<string, List<string>>, QueryContainer>((Func<KeyValuePair<string, List<string>>, QueryContainer>) (fCat => Query<T>.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (bs => (IBoolQuery) bs.Should(fCat.Value.Select<string, QueryContainer>((Func<string, QueryContainer>) (fVal => Query<T>.Term((Field) fCat.Key, (object) fVal))).ToArray<QueryContainer>()))))).ToArray<QueryContainer>())));

    private void SetupPrivateFields(
      IDictionary<string, IEnumerable<string>> selectedTermFilters)
    {
      this.m_selectedTermFilters = new Dictionary<string, List<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> selectedTermFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) selectedTermFilters)
      {
        this.m_selectedTermFilters.Add(selectedTermFilter.Key, new List<string>());
        foreach (string text in selectedTermFilter.Value)
          this.m_selectedTermFilters[selectedTermFilter.Key].Add(text.NormalizeString());
      }
    }
  }
}
