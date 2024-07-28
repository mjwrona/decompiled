// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Tools
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class Tools
  {
    public static void AppendName(StringBuilder builder, string name)
    {
      bool flag = name.Length == 0 || !char.IsLetter(name, 0);
      int length = name.Length;
      for (int index = 0; index < length && !flag; ++index)
      {
        if (!char.IsLetterOrDigit(name, index))
          flag = true;
      }
      builder.AppendFormat(flag ? "[{0}]" : "{0}", (object) name);
    }

    public static void AppendString(StringBuilder builder, string token) => builder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) string.Join("''", token.Split('\'')));

    public static bool IsNumericString(string value)
    {
      List<Node> lexems = Parser.ParseLexems(value);
      return lexems.Count == 1 && lexems[0].NodeType == NodeType.Number;
    }

    public static bool IsDateString(string dateToken, CultureInfo culture) => DateTime.TryParse(dateToken, (IFormatProvider) culture, DateTimeStyles.None, out DateTime _);

    public static bool IsBoolString(string value) => bool.TryParse(value, out bool _);

    public static bool IsGuidString(string value)
    {
      try
      {
        Guid guid = new Guid(value);
        return true;
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      return false;
    }

    public static bool TranslateBoolToken(string value, out bool result)
    {
      value = value.ToLowerInvariant();
      switch (value)
      {
        case "true":
          result = true;
          return true;
        case "false":
          result = false;
          return true;
        default:
          result = false;
          return false;
      }
    }

    public static void EnsureSyntax(bool condition, SyntaxError message, Node node)
    {
      if (!condition)
        throw new SyntaxException(node, message);
    }
  }
}
