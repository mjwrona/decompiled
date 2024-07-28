// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.StringUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class StringUtil
  {
    public static readonly int defaultConsoleWidth = 80;
    private static int s_maxCommentDisplayLength = 120;
    public static readonly string s_ellipsis = "...";
    private static readonly string[] s_lineEndings = new string[3]
    {
      "\r\n",
      "\r",
      "\n"
    };
    private static readonly char[] s_newlineCharacters = new char[2]
    {
      '\r',
      '\n'
    };
    private static Dictionary<int, Dictionary<string, string>> s_resourceTokensCache = new Dictionary<int, Dictionary<string, string>>();
    private static readonly object s_resourceTokensLock = new object();

    public static string[] ParseCommandLine(string cmdLine)
    {
      int startPos = 0;
      bool flag = false;
      for (; startPos < cmdLine.Length; ++startPos)
      {
        char ch = cmdLine[startPos];
        if (ch == '"')
          flag = !flag;
        else if (!flag && (ch == ' ' || ch == '\t'))
          break;
      }
      List<string> stringList = new List<string>();
      while (startPos < cmdLine.Length)
      {
        string commandLineArgument = StringUtil.ParseCommandLineArgument(cmdLine, ref startPos);
        if (commandLineArgument != null)
          stringList.Add(commandLineArgument);
      }
      return stringList.ToArray();
    }

    public static string ParseCommandLineArgument(string cmdLine, ref int startPos)
    {
      int index = startPos;
      bool flag = false;
      string commandLineArgument = (string) null;
      while (index < cmdLine.Length && (cmdLine[index] == ' ' || cmdLine[index] == '\t'))
        ++index;
      if (index < cmdLine.Length)
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (; index < cmdLine.Length; ++index)
        {
          char ch = cmdLine[index];
          if (ch == '"')
          {
            if (flag && index + 1 < cmdLine.Length && cmdLine[index + 1] == '"')
            {
              ++index;
            }
            else
            {
              flag = !flag;
              continue;
            }
          }
          else if (!flag && (ch == ' ' || ch == '\t'))
            break;
          stringBuilder.Append(ch);
        }
        commandLineArgument = stringBuilder.ToString();
      }
      startPos = index;
      return commandLineArgument;
    }

    public static string FormatColumnOutput(
      string[] headers,
      string[,] contents,
      string indent,
      int lineWidth,
      Encoding encoding)
    {
      if (headers.Length < 2)
        throw new ArgumentOutOfRangeException(nameof (headers));
      int length1 = contents.GetLength(0);
      int length2 = contents.GetLength(1);
      if (headers.Length == 0 || length1 == 0 || length2 == 0)
        return string.Empty;
      if (headers.Length != length2)
        throw new ArgumentOutOfRangeException(TFCommonResources.HeadersAndContentsDontMatch((object) headers.Length, (object) length2));
      int[] maxWidths = new int[length2];
      for (int index = 0; index < length2 - 1; ++index)
        maxWidths[index] = StringUtil.CalculateWidth(headers[index], encoding);
      for (int index1 = 0; index1 < length1; ++index1)
      {
        for (int index2 = 0; index2 < length2 - 1; ++index2)
          maxWidths[index2] = Math.Max(maxWidths[index2], StringUtil.CalculateWidth(contents[index1, index2], encoding));
      }
      int width1 = StringUtil.CalculateWidth(indent, encoding);
      for (int index = 0; index < length2 - 1; ++index)
        width1 += maxWidths[index] + 1;
      maxWidths[maxWidths.Length - 1] = lineWidth - width1 - 1;
      int width2 = StringUtil.CalculateWidth(headers[headers.Length - 1], encoding);
      if (maxWidths[maxWidths.Length - 1] < width2)
        maxWidths[maxWidths.Length - 1] = width2;
      return StringUtil.FormatColumnOutput(headers, contents, indent, maxWidths, true, encoding);
    }

    public static string FormatColumnOutput(
      string[] headers,
      string[,] contents,
      string indent,
      int[] maxWidths,
      bool padLastColumn,
      Encoding encoding)
    {
      if (headers.Length < 2)
        throw new ArgumentOutOfRangeException(nameof (headers));
      int length1 = contents.GetLength(0);
      int length2 = contents.GetLength(1);
      if (length1 == 0 || length2 == 0)
        return string.Empty;
      if (headers.Length != length2)
        throw new ArgumentOutOfRangeException(TFCommonResources.HeadersAndContentsDontMatch((object) headers.Length, (object) length2));
      StringBuilder stringBuilder = new StringBuilder((contents.Length + 2) * StringUtil.defaultConsoleWidth);
      for (int index = 0; index < (padLastColumn ? length2 : length2 - 1); ++index)
        headers[index] = headers[index].PadRight(maxWidths[index] - (StringUtil.CalculateWidth(headers[index], encoding) - headers[index].Length));
      stringBuilder.Append(indent);
      stringBuilder.AppendLine(string.Join(" ", headers));
      string[] strArray1 = new string[length2];
      for (int index = 0; index < strArray1.Length; ++index)
        strArray1[index] = string.Empty.PadRight(maxWidths[index], '-');
      stringBuilder.Append(indent);
      stringBuilder.AppendLine(string.Join(" ", strArray1));
      string[] strArray2 = new string[length2];
      for (int index1 = 0; index1 < length1; ++index1)
      {
        for (int index2 = 0; index2 < length2 - 1; ++index2)
          strArray2[index2] = contents[index1, index2];
        strArray2[length2 - 1] = StringUtil.Truncate(contents[index1, length2 - 1], maxWidths[length2 - 1], encoding);
        string[] strArray3 = new string[length2];
        for (int index3 = 0; index3 < length2; ++index3)
        {
          string text = strArray2[index3] ?? string.Empty;
          int num = padLastColumn || index3 < length2 - 1 ? maxWidths[index3] - (StringUtil.CalculateWidth(text, encoding) - text.Length) : 0;
          strArray3[index3] = "{" + index3.ToString() + ",-" + num.ToString() + "}";
        }
        string format = indent + string.Join(" ", strArray3);
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object[]) strArray2));
      }
      return stringBuilder.ToString();
    }

    public static string GetRowFormat(string[] headers, string indent, Encoding encoding)
    {
      string rowTemplate = TFCommonResources.Manager.GetString("AColonB");
      return StringUtil.GetRowFormat(headers, indent, rowTemplate, encoding);
    }

    public static string GetRowFormat(
      string[] headers,
      string indent,
      string rowTemplate,
      Encoding encoding)
    {
      int num = 0;
      foreach (string header in headers)
        num = Math.Max(num, StringUtil.CalculateWidth(header, encoding));
      return StringUtil.GetRowFormat(headers, indent, rowTemplate, num, encoding);
    }

    public static string GetRowFormat(
      string[] headers,
      string indent,
      string rowTemplate,
      int headerMaxWidth,
      Encoding encoding)
    {
      StringBuilder stringBuilder = new StringBuilder(headers.Length * StringUtil.defaultConsoleWidth);
      for (int index = 0; index < headers.Length; ++index)
      {
        stringBuilder.Append(indent);
        string str = headers[index].PadRight(headerMaxWidth - (StringUtil.CalculateWidth(headers[index], encoding) - headers[index].Length));
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, rowTemplate, (object) str, (object) ("{" + index.ToString() + "}")));
      }
      return stringBuilder.ToString();
    }

    public static string GetSecureStringContent(SecureString sec)
    {
      string secureStringContent = (string) null;
      if (sec != null)
      {
        IntPtr bstr = Marshal.SecureStringToBSTR(sec);
        try
        {
          secureStringContent = Marshal.PtrToStringUni(bstr);
        }
        finally
        {
          Marshal.ZeroFreeBSTR(bstr);
        }
      }
      return secureStringContent;
    }

    public static SecureString CreateSecureString(string str)
    {
      SecureString secureString = new SecureString();
      if (!string.IsNullOrEmpty(str))
      {
        foreach (char c in str)
          secureString.AppendChar(c);
      }
      return secureString;
    }

    public static string Truncate(string text, int maximumCharacters, bool addEllipsis)
    {
      if (!string.IsNullOrEmpty(text) && text.Length > maximumCharacters)
      {
        int num = maximumCharacters;
        if (addEllipsis)
          num -= StringUtil.s_ellipsis.Length;
        int index = 0;
        TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(text);
        while (elementEnumerator.MoveNext() && elementEnumerator.ElementIndex <= num)
          index = elementEnumerator.ElementIndex;
        int length = index + StringInfo.GetNextTextElement(text, index).Length;
        if (length > num)
          length = index;
        text = text.Substring(0, length);
        if (addEllipsis && text.Length <= num)
          text += StringUtil.s_ellipsis;
      }
      return text;
    }

    public static string Truncate(string text, int width, Encoding encoding)
    {
      if (!string.IsNullOrEmpty(text))
      {
        if (StringUtil.CalculateWidth(text, encoding) > width)
        {
          int num = 0;
          char[] chars = new char[1];
          int index;
          for (index = 0; index < text.Length; ++index)
          {
            chars[0] = text[index];
            num += StringUtil.CalculateWidth(chars, encoding);
            if (num != width)
            {
              if (num > width)
              {
                --index;
                break;
              }
            }
            else
              break;
          }
          text = text.Substring(0, index + 1);
        }
        foreach (char oldChar in Environment.NewLine)
          text = text.Replace(oldChar, ' ');
      }
      return text;
    }

    public static string TruncateToFirstLine(string value, int maxLength, out bool isTruncated)
    {
      if (string.IsNullOrEmpty(value))
      {
        isTruncated = false;
        return value;
      }
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      isTruncated = false;
      for (int index1 = 0; index1 < value.Length; ++index1)
      {
        if (flag || !char.IsWhiteSpace(value[index1]))
        {
          flag = true;
          if (value[index1] == '\n')
          {
            for (int index2 = index1 + 1; index2 < value.Length; ++index2)
            {
              if (!char.IsWhiteSpace(value[index2]))
                isTruncated = true;
            }
            break;
          }
          stringBuilder.Append(value[index1]);
        }
      }
      for (int index = stringBuilder.Length - 1; index >= 0 && char.IsWhiteSpace(stringBuilder[index]); --index)
        stringBuilder.Remove(index, 1);
      string firstLine = StringUtil.Truncate(stringBuilder.ToString(), maxLength, false);
      isTruncated |= stringBuilder.Length > maxLength;
      return firstLine;
    }

    public static string TruncateLeft(string text, int width, Encoding encoding)
    {
      if (!string.IsNullOrEmpty(text))
      {
        if (StringUtil.CalculateWidth(text, encoding) > width)
        {
          int num = 0;
          char[] chars = new char[1];
          int index;
          for (index = text.Length - 1; index >= 0; --index)
          {
            chars[0] = text[index];
            num += StringUtil.CalculateWidth(chars, encoding);
            if (num != width)
            {
              if (num > width)
              {
                --index;
                break;
              }
            }
            else
              break;
          }
          text = text.Substring(text.Length - index);
        }
        foreach (char oldChar in Environment.NewLine)
          text = text.Replace(oldChar, ' ');
      }
      return text;
    }

    public static int[] CalculateColumnsWidth(
      string[] headers,
      string[,] contents,
      Encoding encoding)
    {
      if (headers.Length != contents.GetLength(1))
        throw new ArgumentOutOfRangeException(nameof (headers));
      int[] columnsWidth = new int[headers.Length];
      for (int index = 0; index < headers.Length; ++index)
        columnsWidth[index] = StringUtil.CalculateWidth(headers[index], encoding);
      for (int index1 = 0; index1 < headers.Length - 1; ++index1)
      {
        int index2 = 0;
        for (int length = contents.GetLength(0); index2 < length; ++index2)
          columnsWidth[index1] = Math.Max(columnsWidth[index1], StringUtil.CalculateWidth(contents[index2, index1], encoding));
      }
      return columnsWidth;
    }

    public static int CalculateWidth(string text, Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      if (text == null)
        return 0;
      return !encoding.IsSingleByte ? encoding.GetByteCount(text) : text.Length;
    }

    public static int CalculateWidth(char[] chars, Encoding encoding)
    {
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      if (chars == null)
        return 0;
      return !encoding.IsSingleByte ? encoding.GetByteCount(chars) : chars.Length;
    }

    public static string Format(string stringFormat, params object[] args)
    {
      if (args == null)
        return stringFormat;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, stringFormat, args);
    }

    public static string FormatInvariant(string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);

    public static string[] SplitOnNewlines(string lines, StringSplitOptions options)
    {
      if (lines == null)
        throw new ArgumentNullException(nameof (lines));
      return lines.Split(StringUtil.s_lineEndings, options);
    }

    public static bool HasNewlines(string value) => -1 != value.IndexOfAny(StringUtil.s_newlineCharacters);

    public static string FormatCommentForOneLine(string comment) => StringUtil.FormatCommentForOneLine(comment, StringUtil.s_maxCommentDisplayLength);

    public static string FormatCommentForOneLine(string comment, int maxCommentDisplayLength)
    {
      if (string.IsNullOrEmpty(comment))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in comment.Split(StringUtil.s_newlineCharacters))
      {
        if (str.Length > 0)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(' ');
          stringBuilder.Append(str);
          if (stringBuilder.Length > maxCommentDisplayLength)
            break;
        }
      }
      stringBuilder.Replace("\t", "    ");
      return stringBuilder.Length > maxCommentDisplayLength ? StringUtil.Truncate(stringBuilder.ToString(), maxCommentDisplayLength, true) : stringBuilder.ToString();
    }

    public static bool ContainsIllegalCharacters(string stringToValidate, string validCharacters)
    {
      bool flag = false;
      foreach (char ch in stringToValidate)
      {
        if (validCharacters.IndexOf(ch) == -1)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static bool ContainsIllegalCharacters(
      string stringToValidate,
      HashSet<char> validCharacters)
    {
      bool flag = false;
      foreach (char ch in stringToValidate)
      {
        if (!validCharacters.Contains(ch))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static string QuoteName(string text) => StringUtil.QuoteName(text, '[');

    public static string QuoteName(string text, char quoteCharacter)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      if (text.Length > 128)
        return (string) null;
      switch (quoteCharacter)
      {
        case '"':
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) text.Replace("\"", "\"\""));
        case '\'':
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) text.Replace("'", "''"));
        case '[':
        case ']':
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) text.Replace("]", "]]"));
        default:
          return (string) null;
      }
    }

    public static string ReplaceResources(string text, out bool replacedAll) => StringUtil.ReplaceTokens(text, "##", "##", StringUtil.\u003C\u003EO.\u003C0\u003E__CachingResourceTokenProvider ?? (StringUtil.\u003C\u003EO.\u003C0\u003E__CachingResourceTokenProvider = new Func<string, string>(StringUtil.CachingResourceTokenProvider)), out replacedAll);

    public static string ReplaceResources(
      string text,
      Func<string, string> escapeFunction,
      out bool replacedAll)
    {
      ArgumentUtility.CheckForNull<Func<string, string>>(escapeFunction, nameof (escapeFunction));
      return StringUtil.ReplaceTokens(text, "##", "##", (Func<string, string>) (resourceName =>
      {
        string str = StringUtil.CachingResourceTokenProvider(resourceName);
        if (!string.IsNullOrEmpty(str))
          str = escapeFunction(str);
        return str;
      }), out replacedAll);
    }

    public static string ReplaceTokens(
      string text,
      string tokenPrefix,
      string tokenSuffix,
      IDictionary<string, string> tokens,
      out bool replacedAll)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(tokens, nameof (tokens));
      return StringUtil.ReplaceTokens(text, tokenPrefix, tokenSuffix, (Func<string, string>) (token =>
      {
        string str;
        if (tokens.TryGetValue(token, out str) && str == null)
          str = "";
        return str;
      }), out replacedAll);
    }

    public static string ReplaceTokens(
      string text,
      string tokenPrefix,
      string tokenSuffix,
      IDictionary<string, string> tokens,
      Func<string, string> escapeFunction,
      out bool replacedAll)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(tokens, nameof (tokens));
      ArgumentUtility.CheckForNull<Func<string, string>>(escapeFunction, nameof (escapeFunction));
      return StringUtil.ReplaceTokens(text, tokenPrefix, tokenSuffix, (Func<string, string>) (token =>
      {
        string str;
        if (tokens.TryGetValue(token, out str) && str == null)
          str = "";
        if (!string.IsNullOrEmpty(str))
          str = escapeFunction(str);
        return str;
      }), out replacedAll);
    }

    public static string ReplaceTokens(
      string text,
      string tokenPrefix,
      string tokenSuffix,
      Func<string, string> tokenProvider,
      out bool replacedAll)
    {
      ArgumentUtility.CheckForNull<string>(text, nameof (text));
      ArgumentUtility.CheckStringForNullOrEmpty(tokenPrefix, nameof (tokenPrefix));
      ArgumentUtility.CheckStringForNullOrEmpty(tokenSuffix, nameof (tokenSuffix));
      ArgumentUtility.CheckForNull<Func<string, string>>(tokenProvider, nameof (tokenProvider));
      replacedAll = true;
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex1 = 0;
      int startIndex2;
      while ((startIndex2 = text.IndexOf(tokenPrefix, startIndex1, StringComparison.Ordinal)) >= 0)
      {
        int num = text.IndexOf(tokenSuffix, startIndex2 + tokenPrefix.Length, StringComparison.Ordinal);
        if (num >= 0)
        {
          stringBuilder.Append(text, startIndex1, startIndex2 - startIndex1);
          string str1 = text.Substring(startIndex2 + tokenPrefix.Length, num - startIndex2 - tokenPrefix.Length);
          string str2 = tokenProvider(str1);
          startIndex1 = num + tokenSuffix.Length;
          if (str2 != null)
          {
            stringBuilder.Append(str2);
          }
          else
          {
            replacedAll = false;
            stringBuilder.Append(text, startIndex2, startIndex1 - startIndex2);
          }
        }
        else
          break;
      }
      if (startIndex1 == 0)
        return text;
      stringBuilder.Append(text, startIndex1, text.Length - startIndex1);
      return stringBuilder.ToString();
    }

    public static int CountSubstrings(string value, string substring)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      ArgumentUtility.CheckStringForNullOrEmpty(substring, nameof (substring));
      int num1 = 0;
      int num2;
      for (int startIndex = 0; (num2 = value.IndexOf(substring, startIndex)) >= 0; startIndex = num2 + substring.Length)
        ++num1;
      return num1;
    }

    public static string ConvertToHex(byte[] byteArray)
    {
      ArgumentUtility.CheckForNull<byte[]>(byteArray, nameof (byteArray));
      return "0x" + HexConverter.ToString(byteArray);
    }

    public static byte[] ConvertFromHexString(string hexString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hexString, nameof (hexString));
      byte[] bytes;
      if (hexString.StartsWith("0x") && hexString.Length % 2 == 0 && HexConverter.TryToByteArray(hexString, 2, hexString.Length - 2, out bytes))
        return bytes;
      throw new ArgumentException(TFCommonResources.InvalidHexString());
    }

    public static string ToQuotedStringList<T>(this IEnumerable<T> values, char quoteCharacter = '\'')
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(values, nameof (values));
      return string.Join(", ", values.Select<T, string>((Func<T, string>) (x => StringUtil.QuoteName(x.ToString(), quoteCharacter))));
    }

    public static string ToQuotedStringListOrNullStringLiteral<T>(
      this IEnumerable<T> values,
      char quoteCharacter = '\'')
    {
      return values == null ? "null" : values.ToQuotedStringList<T>(quoteCharacter);
    }

    public static string ConvertToAlphaNumericString(string input) => new string(input.Where<char>((Func<char, bool>) (c => char.IsLetterOrDigit(c))).ToArray<char>());

    public static bool IsAlphaNumericOnlyString(string input) => input.All<char>((Func<char, bool>) (c => char.IsLetterOrDigit(c)));

    private static string CachingResourceTokenProvider(string fullyQualifiedResource)
    {
      lock (StringUtil.s_resourceTokensLock)
      {
        string str = (string) null;
        Dictionary<string, string> dictionary;
        if (!StringUtil.s_resourceTokensCache.TryGetValue(CultureInfo.CurrentCulture.LCID, out dictionary))
        {
          dictionary = new Dictionary<string, string>();
          StringUtil.s_resourceTokensCache.Add(CultureInfo.CurrentCulture.LCID, dictionary);
        }
        dictionary.TryGetValue(fullyQualifiedResource, out str);
        if (str == null)
        {
          str = StringUtil.ResourceTokenProvider(fullyQualifiedResource);
          if (str != null)
            dictionary.Add(fullyQualifiedResource, str);
        }
        return str;
      }
    }

    private static string ResourceTokenProvider(string fullyQualifiedResource)
    {
      int length = fullyQualifiedResource.LastIndexOf('.');
      if (length >= 0)
      {
        try
        {
          string str = fullyQualifiedResource.Substring(0, length);
          string name = fullyQualifiedResource.Substring(length + 1);
          Type type = Type.GetType(str);
          if (type == (Type) null)
          {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
              type = assembly.GetType(str);
              if (type != (Type) null)
                break;
            }
          }
          if (type == (Type) null)
          {
            TeamFoundationTrace.Error("Failed to resolve resource {0}. Could not find the following type: {1}.", (object) fullyQualifiedResource, (object) str);
            return (string) null;
          }
          MethodInfo methodInfo = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null);
          if (methodInfo == (MethodInfo) null)
          {
            PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, (Binder) null, (Type) null, Type.EmptyTypes, (ParameterModifier[]) null);
            if (property != (PropertyInfo) null)
              methodInfo = property.GetGetMethod(true);
          }
          if (methodInfo != (MethodInfo) null)
            return methodInfo.Invoke((object) null, (object[]) null)?.ToString();
          FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
          if (field != (FieldInfo) null)
            return field.GetValue((object) null)?.ToString();
          TeamFoundationTrace.Error("Failed to resolve resource {0}. The specified type does not have a parameterless static method, property, or field called {1}.", (object) fullyQualifiedResource, (object) name);
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.Error("Failed to resolve resource {0} - exception = {1}", (object) fullyQualifiedResource, (object) ex.Message);
        }
      }
      return (string) null;
    }
  }
}
