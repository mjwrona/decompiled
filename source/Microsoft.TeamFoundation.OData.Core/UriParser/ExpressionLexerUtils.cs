// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpressionLexerUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  internal sealed class ExpressionLexerUtils
  {
    private const char SingleSuffixLower = 'f';
    private const char SingleSuffixUpper = 'F';

    internal static bool IsNumeric(ExpressionTokenKind id) => id == ExpressionTokenKind.IntegerLiteral || id == ExpressionTokenKind.DecimalLiteral || id == ExpressionTokenKind.DoubleLiteral || id == ExpressionTokenKind.Int64Literal || id == ExpressionTokenKind.SingleLiteral;

    internal static bool IsInfinityOrNaNDouble(string tokenText)
    {
      if (tokenText.Length == 3)
      {
        if ((int) tokenText[0] == (int) "INF"[0])
          return ExpressionLexerUtils.IsInfinityLiteralDouble(tokenText);
        if ((int) tokenText[0] == (int) "NaN"[0])
          return string.CompareOrdinal(tokenText, 0, "NaN", 0, 3) == 0;
      }
      return false;
    }

    internal static bool IsInfinityLiteralDouble(string text) => string.CompareOrdinal(text, 0, "INF", 0, text.Length) == 0;

    internal static bool IsInfinityOrNanSingle(string tokenText)
    {
      if (tokenText.Length == 4)
      {
        if ((int) tokenText[0] == (int) "INF"[0])
          return ExpressionLexerUtils.IsInfinityLiteralSingle(tokenText);
        if ((int) tokenText[0] == (int) "NaN"[0] && (tokenText[3] == 'f' || tokenText[3] == 'F'))
          return string.CompareOrdinal(tokenText, 0, "NaN", 0, 3) == 0;
      }
      return false;
    }

    internal static bool IsInfinityLiteralSingle(string text) => text.Length == 4 && (text[3] == 'f' || text[3] == 'F') && string.CompareOrdinal(text, 0, "INF", 0, 3) == 0;
  }
}
