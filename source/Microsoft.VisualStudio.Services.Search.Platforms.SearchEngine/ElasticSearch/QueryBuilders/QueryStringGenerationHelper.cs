// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.QueryStringGenerationHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders
{
  internal static class QueryStringGenerationHelper
  {
    internal static string FormatQueryStringIgnoringEmptyExpression(
      string expressionString,
      string stringToUpdate,
      ref int termExpressionsCount)
    {
      if (!string.IsNullOrEmpty(expressionString))
      {
        ++termExpressionsCount;
        stringToUpdate = string.IsNullOrEmpty(stringToUpdate) ? expressionString : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},\r\n                          {1}", (object) stringToUpdate, (object) expressionString);
      }
      return stringToUpdate;
    }
  }
}
