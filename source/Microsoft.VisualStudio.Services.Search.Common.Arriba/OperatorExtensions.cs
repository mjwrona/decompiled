// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.OperatorExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba
{
  internal static class OperatorExtensions
  {
    private static IReadOnlyDictionary<Operator, string> s_syntaxString = (IReadOnlyDictionary<Operator, string>) new Dictionary<Operator, string>()
    {
      {
        Operator.Equals,
        " = "
      },
      {
        Operator.GreaterThan,
        " > "
      },
      {
        Operator.GreaterThanOrEqual,
        " >= "
      },
      {
        Operator.In,
        " IN "
      },
      {
        Operator.LessThan,
        " < "
      },
      {
        Operator.LessThanOrEqual,
        " <= "
      },
      {
        Operator.Matches,
        ":"
      },
      {
        Operator.MatchesExact,
        "::"
      },
      {
        Operator.NotEquals,
        " <> "
      },
      {
        Operator.StartsWith,
        " STARTSWITH "
      },
      {
        Operator.Near,
        " NEAR "
      },
      {
        Operator.Before,
        " BEFORE "
      },
      {
        Operator.After,
        " AFTER "
      }
    };

    internal static string ToSyntaxString(this Operator op) => OperatorExtensions.s_syntaxString[op];
  }
}
