// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders.ElasticsearchQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders
{
  internal static class ElasticsearchQueryBuilder
  {
    private const string TermQueryStringFormat = "{{\r\n                \"term\": {{\r\n                    \"{0}\": {1}\r\n                }}\r\n            }}";
    private const string WildcardQueryStringFormat = "{{\r\n                \"wildcard\": {{\r\n                    \"{0}\" : {{\r\n                        \"value\" : {1},\r\n                        \"rewrite\" : \"{2}\"\r\n                    }}\r\n                }}\r\n            }}";
    private const string FastWildcardQueryStringFormat = "{{\r\n                \"fast_wildcard\": {{\r\n                    \"{0}\" : {{\r\n                        \"value\" : {1}\r\n                    }}\r\n                }}\r\n            }}";
    private const string MatchPhraseQueryStringFormat = "{{\r\n                    \"match_phrase\": {{\r\n                        \"{0}\": {{\r\n                            \"query\" : {1}\r\n                        }}\r\n                    }}\r\n                }}";
    private const string MatchPhraseWithAGivenAnalyzerQueryStringFormat = "{{\r\n                    \"match_phrase\": {{\r\n                        \"{0}\": {{\r\n                            \"query\" : {1},\r\n                            \"analyzer\" : \"{2}\"\r\n                        }}\r\n                    }}\r\n                }}";
    private const string TermQueryWithBoostFormat = "{{\r\n                    \"term\": {{\r\n                        \"{0}\": {{\r\n                            \"value\": {1},\r\n                            \"boost\": {2}\r\n                        }}\r\n                    }}\r\n              }}";
    private const string BoolShouldQueryFormat = "{{\r\n                     \"bool\": {{\r\n                         \"should\": [{0}]\r\n                     }}\r\n                }}";
    private const string RegexpQueryStringFormat = "{{\r\n\t\t\t        \"regexp\": {{\r\n\t\t\t\t        \"{0}\": {1}\r\n\t\t\t            }}\r\n\t            }}";
    private const string SpanQueryStringFormat = "{{\r\n                    \"span_near\": {{\r\n                        \"clauses\": [\r\n                            {{\r\n                                \"span_term\": {{\r\n                                    \"{0}\": {1}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"span_term\": {{\r\n                                    \"{0}\": {2}\r\n                            }}\r\n                        }}\r\n                    ],\r\n                    \"slop\":{3},\r\n                    \"in_order\": {4}\r\n                }}\r\n            }}";

    public static string BuildTermQuery(string fieldName, string value) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                \"term\": {{\r\n                    \"{0}\": {1}\r\n                }}\r\n            }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value));

    public static string BuildSpanNearQuery(
      string fieldName,
      string value1,
      string value2,
      int slopValue)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"span_near\": {{\r\n                        \"clauses\": [\r\n                            {{\r\n                                \"span_term\": {{\r\n                                    \"{0}\": {1}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"span_term\": {{\r\n                                    \"{0}\": {2}\r\n                            }}\r\n                        }}\r\n                    ],\r\n                    \"slop\":{3},\r\n                    \"in_order\": {4}\r\n                }}\r\n            }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value1), (object) JsonConvert.SerializeObject((object) value2), (object) slopValue, (object) "false");
    }

    public static string BuildSpanBeforeQuery(
      string fieldName,
      string value1,
      string value2,
      int slopValue)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"span_near\": {{\r\n                        \"clauses\": [\r\n                            {{\r\n                                \"span_term\": {{\r\n                                    \"{0}\": {1}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"span_term\": {{\r\n                                    \"{0}\": {2}\r\n                            }}\r\n                        }}\r\n                    ],\r\n                    \"slop\":{3},\r\n                    \"in_order\": {4}\r\n                }}\r\n            }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value1), (object) JsonConvert.SerializeObject((object) value2), (object) slopValue, (object) "true");
    }

    public static string BuildWildcardQuery(
      string fieldName,
      string value,
      string queryRewriteMethod = "top_terms_boost_100")
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                \"wildcard\": {{\r\n                    \"{0}\" : {{\r\n                        \"value\" : {1},\r\n                        \"rewrite\" : \"{2}\"\r\n                    }}\r\n                }}\r\n            }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value), (object) queryRewriteMethod);
    }

    public static string BuildFastWildcardQuery(string fieldName, string value) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                \"fast_wildcard\": {{\r\n                    \"{0}\" : {{\r\n                        \"value\" : {1}\r\n                    }}\r\n                }}\r\n            }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value));

    public static string BuildRegexpQuery(string fieldName, string value) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n\t\t\t        \"regexp\": {{\r\n\t\t\t\t        \"{0}\": {1}\r\n\t\t\t            }}\r\n\t            }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value));

    public static string BuildMatchPhraseQuery(string fieldName, string value)
    {
      if (value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal) && value.Length > 1)
        value = value.Substring(1, value.Length - 2);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"match_phrase\": {{\r\n                        \"{0}\": {{\r\n                            \"query\" : {1}\r\n                        }}\r\n                    }}\r\n                }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value));
    }

    public static string BuildMatchPhraseQueryWithContentAnalyzer(string fieldName, string value)
    {
      if (value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal) && value.Length > 1)
        value = value.Substring(1, value.Length - 2);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"match_phrase\": {{\r\n                        \"{0}\": {{\r\n                            \"query\" : {1},\r\n                            \"analyzer\" : \"{2}\"\r\n                        }}\r\n                    }}\r\n                }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) value), (object) "contentanalyzer");
    }

    public static string BuildTermQueryWithBoost(string fieldName, string fieldValue, double boost) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"term\": {{\r\n                        \"{0}\": {{\r\n                            \"value\": {1},\r\n                            \"boost\": {2}\r\n                        }}\r\n                    }}\r\n              }}", (object) fieldName, (object) JsonConvert.SerializeObject((object) fieldValue), (object) boost);

    public static string BuildBoolShouldQuery(List<string> childQueries) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                     \"bool\": {{\r\n                         \"should\": [{0}]\r\n                     }}\r\n                }}", (object) string.Join(",", (IEnumerable<string>) childQueries));

    internal static string NormalizeBackslashAndDoubeQuote(string text) => text.Replace("\\", "\\\\").Replace("\\\\\"", "\\\"");

    internal static string TrimDoubleQuotesPadding(string value)
    {
      if (!string.IsNullOrWhiteSpace(value))
        value = value.Length > 1 ? value.Substring(1, value.Length - 2) : value;
      return value;
    }
  }
}
