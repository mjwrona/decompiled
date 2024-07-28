// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.HttpUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Microsoft.OData
{
  internal static class HttpUtils
  {
    internal static IList<KeyValuePair<string, string>> ReadMimeType(
      string contentType,
      out string mediaTypeName,
      out string mediaTypeCharset)
    {
      IList<KeyValuePair<ODataMediaType, string>> keyValuePairList = !string.IsNullOrEmpty(contentType) ? HttpUtils.ReadMediaTypes(contentType) : throw new ODataContentTypeException(Strings.HttpUtils_ContentTypeMissing);
      ODataMediaType mediaType = keyValuePairList.Count == 1 ? keyValuePairList[0].Key : throw new ODataContentTypeException(Strings.HttpUtils_NoOrMoreThanOneContentTypeSpecified((object) contentType));
      MediaTypeUtils.CheckMediaTypeForWildCards(mediaType);
      mediaTypeName = mediaType.FullTypeName;
      mediaTypeCharset = keyValuePairList[0].Value;
      return mediaType.Parameters == null ? (IList<KeyValuePair<string, string>>) null : (IList<KeyValuePair<string, string>>) mediaType.Parameters.ToList<KeyValuePair<string, string>>();
    }

    internal static string BuildContentType(ODataMediaType mediaType, Encoding encoding) => mediaType.ToText(encoding);

    internal static IList<KeyValuePair<ODataMediaType, string>> MediaTypesFromString(string text) => string.IsNullOrEmpty(text) ? (IList<KeyValuePair<ODataMediaType, string>>) null : HttpUtils.ReadMediaTypes(text);

    internal static bool CompareMediaTypeNames(string mediaTypeName1, string mediaTypeName2) => string.Equals(mediaTypeName1, mediaTypeName2, StringComparison.OrdinalIgnoreCase);

    internal static bool CompareMediaTypeParameterNames(
      string parameterName1,
      string parameterName2)
    {
      if (string.Equals(parameterName1, parameterName2, StringComparison.OrdinalIgnoreCase) || HttpUtils.IsMetadataParameter(parameterName1) && HttpUtils.IsMetadataParameter(parameterName2))
        return true;
      return HttpUtils.IsStreamingParameter(parameterName1) && HttpUtils.IsStreamingParameter(parameterName2);
    }

    internal static bool IsMetadataParameter(string parameterName) => string.Compare(parameterName, "odata.metadata", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(parameterName, "metadata", StringComparison.OrdinalIgnoreCase) == 0;

    internal static bool IsStreamingParameter(string parameterName) => string.Compare(parameterName, "odata.streaming", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(parameterName, "streaming", StringComparison.OrdinalIgnoreCase) == 0;

    internal static Encoding EncodingFromAcceptableCharsets(
      string acceptableCharsets,
      ODataMediaType mediaType,
      Encoding utf8Encoding,
      Encoding defaultEncoding)
    {
      Encoding encoding = (Encoding) null;
      if (!string.IsNullOrEmpty(acceptableCharsets))
      {
        foreach (KeyValuePair<int, HttpUtils.CharsetPart> keyValuePair in new List<HttpUtils.CharsetPart>(HttpUtils.AcceptCharsetParts(acceptableCharsets)).ToArray().StableSort<HttpUtils.CharsetPart>((Comparison<HttpUtils.CharsetPart>) ((x, y) => y.Quality - x.Quality)))
        {
          HttpUtils.CharsetPart charsetPart = keyValuePair.Value;
          if (charsetPart.Quality > 0)
          {
            if (string.Compare("utf-8", charsetPart.Charset, StringComparison.OrdinalIgnoreCase) == 0)
            {
              encoding = utf8Encoding;
              break;
            }
            encoding = HttpUtils.GetEncodingFromCharsetName(charsetPart.Charset);
            if (encoding != null)
              break;
          }
        }
      }
      if (encoding == null)
      {
        encoding = mediaType.SelectEncoding();
        if (encoding == null)
          return defaultEncoding;
      }
      return encoding;
    }

    internal static void ReadQualityValue(string text, ref int textIndex, out int qualityValue)
    {
      char p1 = text[textIndex++];
      switch (p1)
      {
        case '0':
          qualityValue = 0;
          break;
        case '1':
          qualityValue = 1;
          break;
        default:
          throw new ODataContentTypeException(Strings.HttpUtils_InvalidQualityValueStartChar((object) text, (object) p1));
      }
      if (textIndex < text.Length && text[textIndex] == '.')
      {
        ++textIndex;
        int num = 1000;
        while (num > 1 && textIndex < text.Length)
        {
          int int32 = HttpUtils.DigitToInt32(text[textIndex]);
          if (int32 >= 0)
          {
            ++textIndex;
            num /= 10;
            qualityValue *= 10;
            qualityValue += int32;
          }
          else
            break;
        }
        qualityValue *= num;
        if (qualityValue > 1000)
          throw new ODataContentTypeException(Strings.HttpUtils_InvalidQualityValue((object) (qualityValue / 1000), (object) text));
      }
      else
        qualityValue *= 1000;
    }

    internal static void ValidateHttpMethod(string httpMethodString)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(httpMethodString, nameof (httpMethodString));
      if (string.CompareOrdinal(httpMethodString, "GET") != 0 && string.CompareOrdinal(httpMethodString, "DELETE") != 0 && string.CompareOrdinal(httpMethodString, "PATCH") != 0 && string.CompareOrdinal(httpMethodString, "POST") != 0 && string.CompareOrdinal(httpMethodString, "PUT") != 0)
        throw new ODataException(Strings.HttpUtils_InvalidHttpMethodString((object) httpMethodString));
    }

    internal static bool IsQueryMethod(string httpMethod) => string.CompareOrdinal(httpMethod, "GET") == 0;

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is a large switch on all the Http response codes; no complexity here.")]
    internal static string GetStatusMessage(int statusCode)
    {
      switch (statusCode)
      {
        case 100:
          return "Continue";
        case 101:
          return "Switching Protocols";
        case 200:
          return "OK";
        case 201:
          return "Created";
        case 202:
          return "Accepted";
        case 203:
          return "Non-Authoritative Information";
        case 204:
          return "No Content";
        case 205:
          return "Reset Content";
        case 206:
          return "Partial Content";
        case 300:
          return "Multiple Choices";
        case 301:
          return "Moved Permanently";
        case 302:
          return "Found";
        case 303:
          return "See Other";
        case 304:
          return "Not Modified";
        case 305:
          return "Use Proxy";
        case 307:
          return "Temporary Redirect";
        case 400:
          return "Bad Request";
        case 401:
          return "Unauthorized";
        case 402:
          return "Payment Required";
        case 403:
          return "Forbidden";
        case 404:
          return "Not Found";
        case 405:
          return "Method Not Allowed";
        case 406:
          return "Not Acceptable";
        case 407:
          return "Proxy Authentication Required";
        case 408:
          return "Request Time-out";
        case 409:
          return "Conflict";
        case 410:
          return "Gone";
        case 411:
          return "Length Required";
        case 412:
          return "Precondition Failed";
        case 413:
          return "Request Entity Too Large";
        case 414:
          return "Request-URI Too Large";
        case 415:
          return "Unsupported Media Type";
        case 416:
          return "Requested range not satisfiable";
        case 417:
          return "Expectation Failed";
        case 500:
          return "Internal Server Error";
        case 501:
          return "Not Implemented";
        case 502:
          return "Bad Gateway";
        case 503:
          return "Service Unavailable";
        case 504:
          return "Gateway Time-out";
        case 505:
          return "HTTP Version not supported";
        default:
          return "Unknown Status Code";
      }
    }

    internal static Encoding GetEncodingFromCharsetName(string charsetName)
    {
      try
      {
        return Encoding.GetEncoding(charsetName);
      }
      catch (ArgumentException ex)
      {
        return (Encoding) null;
      }
    }

    internal static string ReadTokenOrQuotedStringValue(
      string headerName,
      string headerText,
      ref int textIndex,
      out bool isQuotedString,
      Func<string, Exception> createException)
    {
      StringBuilder stringBuilder = new StringBuilder();
      isQuotedString = false;
      if (textIndex < headerText.Length && headerText[textIndex] == '"')
      {
        ++textIndex;
        isQuotedString = true;
      }
      char minValue = char.MinValue;
      while (textIndex < headerText.Length)
      {
        minValue = headerText[textIndex];
        switch (minValue)
        {
          case '"':
          case '\\':
            if (!isQuotedString)
              throw createException(Strings.HttpUtils_EscapeCharWithoutQuotes((object) headerName, (object) headerText, (object) textIndex, (object) minValue));
            ++textIndex;
            if (minValue != '"')
            {
              if (textIndex >= headerText.Length)
                throw createException(Strings.HttpUtils_EscapeCharAtEnd((object) headerName, (object) headerText, (object) textIndex, (object) minValue));
              minValue = headerText[textIndex];
              break;
            }
            goto label_15;
          default:
            if (isQuotedString || HttpUtils.IsHttpToken(minValue))
            {
              if (isQuotedString && !HttpUtils.IsValidInQuotedHeaderValue(minValue))
                throw createException(Strings.HttpUtils_InvalidCharacterInQuotedParameterValue((object) headerName, (object) headerText, (object) textIndex, (object) minValue));
              break;
            }
            goto label_15;
        }
        stringBuilder.Append(minValue);
        ++textIndex;
      }
label_15:
      if (isQuotedString && minValue != '"')
        throw createException(Strings.HttpUtils_ClosingQuoteNotFound((object) headerName, (object) headerText, (object) textIndex));
      return stringBuilder.ToString();
    }

    internal static bool SkipWhitespace(string text, ref int textIndex)
    {
      while (textIndex < text.Length && char.IsWhiteSpace(text, textIndex))
        ++textIndex;
      return textIndex == text.Length;
    }

    private static IEnumerable<HttpUtils.CharsetPart> AcceptCharsetParts(string headerValue)
    {
      bool flag1 = false;
      int textIndex1 = 0;
      while (textIndex1 < headerValue.Length && !HttpUtils.SkipWhitespace(headerValue, ref textIndex1))
      {
        if (headerValue[textIndex1] == ',')
        {
          flag1 = false;
          ++textIndex1;
        }
        else
        {
          if (flag1)
            throw new ODataContentTypeException(Strings.HttpUtils_MissingSeparatorBetweenCharsets((object) headerValue));
          int startIndex = textIndex1;
          int textIndex2 = startIndex;
          bool flag2 = HttpUtils.ReadToken(headerValue, ref textIndex2);
          if (textIndex2 == textIndex1)
            throw new ODataContentTypeException(Strings.HttpUtils_InvalidCharsetName((object) headerValue));
          int headerEnd;
          int qualityValue;
          if (flag2)
          {
            qualityValue = 1000;
            headerEnd = textIndex2;
          }
          else
          {
            char c = headerValue[textIndex2];
            if (!HttpUtils.IsHttpSeparator(c))
              throw new ODataContentTypeException(Strings.HttpUtils_InvalidSeparatorBetweenCharsets((object) headerValue));
            if (c == ';')
            {
              if (HttpUtils.ReadLiteral(headerValue, textIndex2, ";q="))
                throw new ODataContentTypeException(Strings.HttpUtils_UnexpectedEndOfQValue((object) headerValue));
              headerEnd = textIndex2 + 3;
              HttpUtils.ReadQualityValue(headerValue, ref headerEnd, out qualityValue);
            }
            else
            {
              qualityValue = 1000;
              headerEnd = textIndex2;
            }
          }
          yield return new HttpUtils.CharsetPart(headerValue.Substring(startIndex, textIndex2 - startIndex), qualityValue);
          flag1 = true;
          textIndex1 = headerEnd;
        }
      }
    }

    private static IList<KeyValuePair<ODataMediaType, string>> ReadMediaTypes(string text)
    {
      List<KeyValuePair<string, string>> parameters = (List<KeyValuePair<string, string>>) null;
      List<KeyValuePair<ODataMediaType, string>> keyValuePairList = new List<KeyValuePair<ODataMediaType, string>>();
      int textIndex = 0;
      while (!HttpUtils.SkipWhitespace(text, ref textIndex))
      {
        string type;
        string subType;
        HttpUtils.ReadMediaTypeAndSubtype(text, ref textIndex, out type, out subType);
        string charset = (string) null;
        while (!HttpUtils.SkipWhitespace(text, ref textIndex))
        {
          if (text[textIndex] == ',')
          {
            ++textIndex;
            break;
          }
          if (text[textIndex] != ';')
            throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeRequiresSemicolonBeforeParameter((object) text));
          ++textIndex;
          if (!HttpUtils.SkipWhitespace(text, ref textIndex))
            HttpUtils.ReadMediaTypeParameter(text, ref textIndex, ref parameters, ref charset);
          else
            break;
        }
        keyValuePairList.Add(new KeyValuePair<ODataMediaType, string>(new ODataMediaType(type, subType, (IEnumerable<KeyValuePair<string, string>>) parameters), charset));
        parameters = (List<KeyValuePair<string, string>>) null;
      }
      return (IList<KeyValuePair<ODataMediaType, string>>) keyValuePairList;
    }

    private static void ReadMediaTypeParameter(
      string text,
      ref int textIndex,
      ref List<KeyValuePair<string, string>> parameters,
      ref string charset)
    {
      int startIndex = textIndex;
      bool flag = HttpUtils.ReadToken(text, ref textIndex);
      string str1 = text.Substring(startIndex, textIndex - startIndex);
      if (str1.Length == 0)
        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeMissingParameterName);
      if (flag)
        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeMissingParameterValue((object) str1));
      if (text[textIndex] != '=')
        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeMissingParameterValue((object) str1));
      ++textIndex;
      string str2 = HttpUtils.ReadTokenOrQuotedStringValue("Content-Type", text, ref textIndex, out bool _, (Func<string, Exception>) (message => (Exception) new ODataContentTypeException(message)));
      if (HttpUtils.CompareMediaTypeParameterNames(nameof (charset), str1))
      {
        charset = str2;
      }
      else
      {
        if (parameters == null)
          parameters = new List<KeyValuePair<string, string>>(1);
        parameters.Add(new KeyValuePair<string, string>(str1, str2));
      }
    }

    private static void ReadMediaTypeAndSubtype(
      string mediaTypeName,
      ref int textIndex,
      out string type,
      out string subType)
    {
      int startIndex1 = textIndex;
      if (HttpUtils.ReadToken(mediaTypeName, ref textIndex))
        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeUnspecified((object) mediaTypeName));
      if (mediaTypeName[textIndex] != '/')
        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeRequiresSlash((object) mediaTypeName));
      type = mediaTypeName.Substring(startIndex1, textIndex - startIndex1);
      ++textIndex;
      int startIndex2 = textIndex;
      HttpUtils.ReadToken(mediaTypeName, ref textIndex);
      if (textIndex == startIndex2)
        throw new ODataContentTypeException(Strings.HttpUtils_MediaTypeRequiresSubType((object) mediaTypeName));
      subType = mediaTypeName.Substring(startIndex2, textIndex - startIndex2);
    }

    private static bool IsHttpToken(char c) => c < '\u007F' && c > '\u001F' && !HttpUtils.IsHttpSeparator(c);

    private static bool IsValidInQuotedHeaderValue(char c)
    {
      int num = (int) c;
      return (num >= 32 || c == ' ' || c == '\t') && num != (int) sbyte.MaxValue;
    }

    private static bool IsHttpSeparator(char c) => c == '(' || c == ')' || c == '<' || c == '>' || c == '@' || c == ',' || c == ';' || c == ':' || c == '\\' || c == '"' || c == '/' || c == '[' || c == ']' || c == '?' || c == '=' || c == '{' || c == '}' || c == ' ' || c == '\t';

    private static bool ReadToken(string text, ref int textIndex)
    {
      while (textIndex < text.Length && HttpUtils.IsHttpToken(text[textIndex]))
        ++textIndex;
      return textIndex == text.Length;
    }

    private static int DigitToInt32(char c)
    {
      if (c >= '0' && c <= '9')
        return (int) c - 48;
      if (HttpUtils.IsHttpElementSeparator(c))
        return -1;
      throw new ODataException(Strings.HttpUtils_CannotConvertCharToInt((object) c));
    }

    private static bool IsHttpElementSeparator(char c) => c == ',' || c == ' ' || c == '\t';

    private static bool ReadLiteral(string text, int textIndex, string literal)
    {
      if (string.Compare(text, textIndex, literal, 0, literal.Length, StringComparison.Ordinal) != 0)
        throw new ODataException(Strings.HttpUtils_ExpectedLiteralNotFoundInString((object) literal, (object) textIndex, (object) text));
      return textIndex + literal.Length == text.Length;
    }

    private struct CharsetPart
    {
      internal readonly string Charset;
      internal readonly int Quality;

      internal CharsetPart(string charset, int quality)
      {
        this.Charset = charset;
        this.Quality = quality;
      }
    }
  }
}
