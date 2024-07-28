// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.MissingFieldExpressionQueryBuilder
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
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal class MissingFieldExpressionQueryBuilder : IPlatformQueryBuilder
  {
    public string Build(
      IVssRequestContext requestContext,
      IExpression expression,
      IEntityType type,
      DocumentContractType contractType,
      bool enableRanking,
      bool allowSpellingErrors,
      string requestId,
      ResultsCountPlatformRequest request)
    {
      if (!(expression is MissingFieldExpression missingFieldExpression))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Parameter {0} is not of type {1}", (object) nameof (expression), (object) typeof (MissingFieldExpression))));
      string str = string.Empty;
      if (!string.IsNullOrEmpty(missingFieldExpression.FieldName))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                    \"{0}\": {{\r\n                        \"{1}\": [\r\n                            {{\r\n                                \"{2}\": {{\r\n                                    \"{3}\": \"{4}\"\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) "bool", (object) "must_not", (object) "exists", (object) "field", (object) missingFieldExpression.FieldName);
      return str;
    }
  }
}
