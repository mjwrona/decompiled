// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.BoolQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Nest;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class BoolQueryBuilder
  {
    private readonly string m_rawQueryString;
    private readonly string m_rawFilterString;

    public BoolQueryBuilder(string rawQueryString, string rawFilterString)
    {
      if (rawQueryString == null)
        throw new ArgumentNullException(nameof (rawQueryString));
      if (rawFilterString == null)
        throw new ArgumentNullException(nameof (rawFilterString));
      this.m_rawQueryString = rawQueryString;
      this.m_rawFilterString = rawFilterString;
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<BoolQueryDescriptor<object>>(new BoolQueryDescriptor<object>().Must(new Func<QueryContainerDescriptor<object>, QueryContainer>(this.BoolQuery<object>)))).PrettyJson();

    internal QueryContainer BoolQuery<T>(QueryContainerDescriptor<T> queryDescriptor) where T : class => queryDescriptor.Raw(this.CreateBoolQueryString());

    private string CreateBoolQueryString()
    {
      if (!string.IsNullOrWhiteSpace(this.m_rawQueryString) && !string.IsNullOrWhiteSpace(this.m_rawFilterString))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                              \"{1}\": {2},\r\n                              \"{3}\": {4}\r\n                            }}\r\n                    }}", (object) "bool", (object) "must", (object) this.m_rawQueryString, (object) "filter", (object) this.m_rawFilterString);
      if (!string.IsNullOrWhiteSpace(this.m_rawQueryString))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                              \"{1}\": {2}\r\n                            }}\r\n                    }}", (object) "bool", (object) "must", (object) this.m_rawQueryString);
      return !string.IsNullOrWhiteSpace(this.m_rawFilterString) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                              \"{1}\": {2}\r\n                            }}\r\n                    }}", (object) "bool", (object) "filter", (object) this.m_rawFilterString) : string.Empty;
    }
  }
}
