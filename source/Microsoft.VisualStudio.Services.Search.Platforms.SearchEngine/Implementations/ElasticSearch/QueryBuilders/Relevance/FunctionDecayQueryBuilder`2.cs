// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Relevance.FunctionDecayQueryBuilder`2
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Relevance
{
  internal class FunctionDecayQueryBuilder<TOrigin, TScale> : IRelevanceQueryBuilder
  {
    private static readonly IReadOnlyDictionary<DecayFunction, string> s_functionToEsFunctionString = (IReadOnlyDictionary<DecayFunction, string>) new Dictionary<DecayFunction, string>()
    {
      [DecayFunction.BellCurve] = "gauss",
      [DecayFunction.Exponential] = "exp",
      [DecayFunction.Linear] = "linear"
    };

    public string Build(string baseQuery, IRelevanceExpression expression)
    {
      if (!(expression is FunctionDecayExpression<TOrigin, TScale> functionDecayExpression))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Patameter {0} is not of type {1}", (object) nameof (expression), (object) typeof (FunctionDecayExpression<TOrigin, TScale>))));
      List<string> values = new List<string>();
      values.Add(FormattableString.Invariant(FormattableStringFactory.Create("\"decay\":{0}", (object) JsonConvert.SerializeObject((object) functionDecayExpression.Decay))));
      string str1;
      if (Type.GetTypeCode(typeof (TOrigin)) == TypeCode.DateTime)
      {
        DateTime dateTime = DateTime.UtcNow;
        if (!EqualityComparer<TOrigin>.Default.Equals(functionDecayExpression.Origin, default (TOrigin)))
          dateTime = (DateTime) Convert.ChangeType((object) functionDecayExpression.Origin, typeof (DateTime), (IFormatProvider) CultureInfo.InvariantCulture);
        str1 = dateTime.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture) + "Z";
      }
      else
        str1 = functionDecayExpression.Origin.ToString();
      values.Add(FormattableString.Invariant(FormattableStringFactory.Create("\"origin\":{0}", (object) JsonConvert.SerializeObject((object) str1))));
      if ((object) functionDecayExpression.Scale != null)
        values.Add(FormattableString.Invariant(FormattableStringFactory.Create("\"scale\":{0}", (object) JsonConvert.SerializeObject((object) functionDecayExpression.Scale))));
      if ((object) functionDecayExpression.Offset != null)
        values.Add(FormattableString.Invariant(FormattableStringFactory.Create("\"offset\":{0}", (object) JsonConvert.SerializeObject((object) functionDecayExpression.Offset))));
      string str2 = JsonConvert.SerializeObject((object) FunctionDecayQueryBuilder<TOrigin, TScale>.s_functionToEsFunctionString[functionDecayExpression.Function]);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"function_score\": {{\r\n                        \"query\": {0},\r\n                        {1}: {{\r\n                           {2}: {{\r\n                             {3}\r\n                            }}\r\n                        }}\r\n                    }}\r\n                }}", (object) baseQuery, (object) str2, (object) JsonConvert.SerializeObject((object) functionDecayExpression.Field), (object) string.Join(FormattableString.Invariant(FormattableStringFactory.Create(",{0}", (object) Environment.NewLine)), (IEnumerable<string>) values)));
    }
  }
}
