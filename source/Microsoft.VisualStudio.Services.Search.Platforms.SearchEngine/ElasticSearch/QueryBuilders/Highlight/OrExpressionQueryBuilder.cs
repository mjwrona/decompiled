// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight.OrExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight
{
  internal class OrExpressionQueryBuilder : IHighlightQueryBuilder
  {
    public string Build(IExpression expression, string fieldName, out bool isFilteredQuery)
    {
      IExpression[] children = expression.Children;
      int num = 0;
      isFilteredQuery = false;
      string str = string.Empty;
      for (int index = 0; index < children.Length; ++index)
      {
        bool isFilteredQuery1;
        string searchHighlightQuery = children[index].ToElasticSearchHighlightQuery(fieldName, out isFilteredQuery1);
        isFilteredQuery |= isFilteredQuery1;
        string stringToUpdate = str;
        ref int local = ref num;
        str = QueryStringGenerationHelper.FormatQueryStringIgnoringEmptyExpression(searchHighlightQuery, stringToUpdate, ref local);
      }
      if (num > 1)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{{\r\n                          \"bool\": {{{{\r\n                              \"should\": [\r\n                                  {0}\r\n                              ]\r\n                          }}}}\r\n                      }}}}", (object) str);
      return str;
    }
  }
}
