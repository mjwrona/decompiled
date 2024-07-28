// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.AndExpressionQueryBuilder
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
  internal class AndExpressionQueryBuilder : IPlatformQueryBuilder
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
      string stringToUpdate1 = string.Empty;
      string stringToUpdate2 = string.Empty;
      int termExpressionsCount = 0;
      for (int index = 0; index < children.Length; ++index)
      {
        if (children[index] is NotExpression)
          stringToUpdate2 = QueryStringGenerationHelper.FormatQueryStringIgnoringEmptyExpression(children[index].Children[0].ToElasticSearchQuery(requestContext, entityType, contractType, enableRanking, allowSpellingErrors, requestId, request), stringToUpdate2, ref termExpressionsCount);
        else
          stringToUpdate1 = QueryStringGenerationHelper.FormatQueryStringIgnoringEmptyExpression(children[index].ToElasticSearchQuery(requestContext, entityType, contractType, enableRanking, allowSpellingErrors, requestId, request), stringToUpdate1, ref termExpressionsCount);
      }
      if (!string.IsNullOrEmpty(stringToUpdate1) && termExpressionsCount > 1)
        stringToUpdate1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"must\": [\r\n                        {0}\r\n                    ]", (object) stringToUpdate1);
      if (!string.IsNullOrEmpty(stringToUpdate2))
        stringToUpdate2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"must_not\": [\r\n                        {0}\r\n                    ]", (object) stringToUpdate2);
      string str = stringToUpdate1;
      if (!string.IsNullOrEmpty(stringToUpdate1) && !string.IsNullOrEmpty(stringToUpdate2))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"bool\": {{\r\n                            {0},\r\n                            {1}\r\n                        }}\r\n                    }}", (object) stringToUpdate1, (object) stringToUpdate2);
      else if (!string.IsNullOrEmpty(stringToUpdate1) && termExpressionsCount > 1)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"bool\": {{\r\n                            {0}\r\n                        }}\r\n                    }}", (object) stringToUpdate1);
      else if (!string.IsNullOrEmpty(stringToUpdate2))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"bool\": {{\r\n                            {0}\r\n                        }}\r\n                    }}", (object) stringToUpdate2);
      return str;
    }
  }
}
