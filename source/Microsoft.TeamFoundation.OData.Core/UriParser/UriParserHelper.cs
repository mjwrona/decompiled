// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriParserHelper
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class UriParserHelper
  {
    internal static bool IsCharHexDigit(char c)
    {
      if (c >= '0' && c <= '9' || c >= 'a' && c <= 'f')
        return true;
      return c >= 'A' && c <= 'F';
    }

    internal static bool TryRemovePrefix(string prefix, ref string text) => UriParserHelper.TryRemoveLiteralPrefix(prefix, ref text);

    internal static bool TryRemoveQuotes(ref string text)
    {
      if (text.Length < 2)
        return false;
      char ch = text[0];
      if (ch != '\'' || (int) text[text.Length - 1] != (int) ch)
        return false;
      string str = text.Substring(1, text.Length - 2);
      int startIndex = 0;
      while (true)
      {
        int num = str.IndexOf(ch, startIndex);
        if (num >= 0)
        {
          str = str.Remove(num, 1);
          if (str.Length >= num + 1 && (int) str[num] == (int) ch)
            startIndex = num + 1;
          else
            break;
        }
        else
          goto label_9;
      }
      return false;
label_9:
      text = str;
      return true;
    }

    internal static string RemoveQuotes(string text)
    {
      char ch = text[0];
      string str = text.Substring(1, text.Length - 2);
      int startIndex1 = 0;
      while (true)
      {
        int startIndex2 = str.IndexOf(ch, startIndex1);
        if (startIndex2 >= 0)
        {
          str = str.Remove(startIndex2, 1);
          startIndex1 = startIndex2 + 1;
        }
        else
          break;
      }
      return str;
    }

    internal static bool TryRemoveLiteralSuffix(string suffix, ref string text)
    {
      text = text.Trim();
      if (text.Length <= suffix.Length || UriParserHelper.IsValidNumericConstant(text) || !text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        return false;
      text = text.Substring(0, text.Length - suffix.Length);
      return true;
    }

    internal static bool TryRemoveLiteralPrefix(string prefix, ref string text)
    {
      if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        return false;
      text = text.Remove(0, prefix.Length);
      return true;
    }

    internal static void ValidatePrefixLiteral(string typePrefixLiteralName)
    {
      if (!((IEnumerable<char>) typePrefixLiteralName.ToCharArray()).All<char>((Func<char, bool>) (x => char.IsLetter(x) || x == '.')))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.OData.Strings.UriParserHelper_InvalidPrefixLiteral((object) typePrefixLiteralName)));
    }

    internal static bool IsUriValueQuoted(string text)
    {
      if (text.Length < 2 || text[0] != '\'' || text[text.Length - 1] != '\'')
        return false;
      int num;
      for (int startIndex = 1; startIndex < text.Length - 1; startIndex = num + 2)
      {
        num = text.IndexOf('\'', startIndex, text.Length - startIndex - 1);
        if (num != -1)
        {
          if (num == text.Length - 2 || text[num + 1] != '\'')
            return false;
        }
        else
          break;
      }
      return true;
    }

    internal static IEdmTypeReference GetLiteralEdmTypeReference(ExpressionTokenKind tokenKind)
    {
      switch (tokenKind)
      {
        case ExpressionTokenKind.BooleanLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(false);
        case ExpressionTokenKind.StringLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetString(true);
        case ExpressionTokenKind.IntegerLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false);
        case ExpressionTokenKind.Int64Literal:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetInt64(false);
        case ExpressionTokenKind.SingleLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetSingle(false);
        case ExpressionTokenKind.DateTimeOffsetLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetDateTimeOffset(false);
        case ExpressionTokenKind.DurationLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false);
        case ExpressionTokenKind.DecimalLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false);
        case ExpressionTokenKind.DoubleLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetDouble(false);
        case ExpressionTokenKind.GuidLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetGuid(false);
        case ExpressionTokenKind.BinaryLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetBinary(true);
        case ExpressionTokenKind.GeographyLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
        case ExpressionTokenKind.GeometryLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, false);
        case ExpressionTokenKind.QuotedLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetString(true);
        case ExpressionTokenKind.DateLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetDate(false);
        case ExpressionTokenKind.TimeOfDayLiteral:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetTimeOfDay(false);
        default:
          return (IEdmTypeReference) null;
      }
    }

    internal static bool IsAnnotation(string identifier) => !string.IsNullOrEmpty(identifier) && identifier[0] == '@' && identifier.Contains(".");

    private static bool IsValidNumericConstant(string text) => string.Equals(text, "INF", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "-INF", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "NaN", StringComparison.OrdinalIgnoreCase);
  }
}
