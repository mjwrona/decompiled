// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.OrExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal class OrExpressionQueryBuilder : IPlatformQueryBuilder
  {
    public string Build(
      IVssRequestContext requestContext,
      IExpression expression,
      IEntityType entityType,
      DocumentContractType contractType,
      bool enableRanking,
      bool allowSpellingErrors,
      string requestId,
      ResultsCountPlatformRequest request)
    {
      IExpression[] children = expression.Children;
      int termExpressionsCount = 0;
      string stringToUpdate = string.Empty;
      for (int index = 0; index < children.Length; ++index)
        stringToUpdate = QueryStringGenerationHelper.FormatQueryStringIgnoringEmptyExpression(children[index].ToElasticSearchQuery(requestContext, entityType, contractType, enableRanking, allowSpellingErrors, requestId, request), stringToUpdate, ref termExpressionsCount);
      if (termExpressionsCount > 1)
        stringToUpdate = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"bool\": {{\r\n                            \"should\": [\r\n                                {0}\r\n                            ]\r\n                        }}\r\n                    }}", (object) stringToUpdate);
      return stringToUpdate;
    }
  }
}
