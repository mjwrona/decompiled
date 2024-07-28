// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.HttpHeaderValueLexer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal abstract class HttpHeaderValueLexer
  {
    internal const string ElementSeparator = ",";
    internal const string ParameterSeparator = ";";
    internal const string ValueSeparator = "=";
    private readonly string httpHeaderName;
    private readonly string httpHeaderValue;
    private readonly int startIndexOfNextItem;
    private readonly string value;
    private readonly string originalText;

    private HttpHeaderValueLexer(
      string httpHeaderName,
      string httpHeaderValue,
      string value,
      string originalText,
      int startIndexOfNextItem)
    {
      this.httpHeaderName = httpHeaderName;
      this.httpHeaderValue = httpHeaderValue;
      this.value = value;
      this.originalText = originalText;
      if (this.httpHeaderValue != null)
        HttpUtils.SkipWhitespace(this.httpHeaderValue, ref startIndexOfNextItem);
      this.startIndexOfNextItem = startIndexOfNextItem;
    }

    internal string Value => this.value;

    internal string OriginalText => this.originalText;

    internal abstract HttpHeaderValueLexer.HttpHeaderValueItemType Type { get; }

    internal static HttpHeaderValueLexer Create(string httpHeaderName, string httpHeaderValue) => (HttpHeaderValueLexer) new HttpHeaderValueLexer.HttpHeaderStart(httpHeaderName, httpHeaderValue);

    internal HttpHeaderValue ToHttpHeaderValue()
    {
      HttpHeaderValueLexer lexer = this;
      HttpHeaderValue httpHeaderValue = new HttpHeaderValue();
      while (lexer.Type != HttpHeaderValueLexer.HttpHeaderValueItemType.End)
      {
        lexer = lexer.ReadNext();
        if (lexer.Type == HttpHeaderValueLexer.HttpHeaderValueItemType.Token)
        {
          HttpHeaderValueElement headerValueElement = HttpHeaderValueLexer.ReadHttpHeaderValueElement(ref lexer);
          if (!httpHeaderValue.ContainsKey(headerValueElement.Name))
            httpHeaderValue.Add(headerValueElement.Name, headerValueElement);
        }
      }
      return httpHeaderValue;
    }

    internal abstract HttpHeaderValueLexer ReadNext();

    private static HttpHeaderValueElement ReadHttpHeaderValueElement(ref HttpHeaderValueLexer lexer)
    {
      List<KeyValuePair<string, string>> source = new List<KeyValuePair<string, string>>()
      {
        HttpHeaderValueLexer.ReadKeyValuePair(ref lexer)
      };
      while (lexer.Type == HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator)
      {
        lexer = lexer.ReadNext();
        source.Add(HttpHeaderValueLexer.ReadKeyValuePair(ref lexer));
      }
      return new HttpHeaderValueElement(source[0].Key, source[0].Value, (IEnumerable<KeyValuePair<string, string>>) source.Skip<KeyValuePair<string, string>>(1).ToArray<KeyValuePair<string, string>>());
    }

    private static KeyValuePair<string, string> ReadKeyValuePair(ref HttpHeaderValueLexer lexer)
    {
      string originalText = lexer.OriginalText;
      string str = (string) null;
      lexer = lexer.ReadNext();
      if (lexer.Type == HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator)
      {
        lexer = lexer.ReadNext();
        str = lexer.OriginalText;
        lexer = lexer.ReadNext();
      }
      return new KeyValuePair<string, string>(originalText, str);
    }

    private bool EndOfHeaderValue() => this.startIndexOfNextItem == this.httpHeaderValue.Length;

    private HttpHeaderValueLexer ReadNextTokenOrQuotedString()
    {
      int startIndexOfNextItem = this.startIndexOfNextItem;
      bool isQuotedString;
      string str = HttpUtils.ReadTokenOrQuotedStringValue(this.httpHeaderName, this.httpHeaderValue, ref startIndexOfNextItem, out isQuotedString, (Func<string, Exception>) (message => (Exception) new ODataException(message)));
      if (startIndexOfNextItem == this.startIndexOfNextItem)
        throw new ODataException(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString((object) this.httpHeaderName, (object) this.httpHeaderValue, (object) this.startIndexOfNextItem));
      if (!isQuotedString)
        return (HttpHeaderValueLexer) new HttpHeaderValueLexer.HttpHeaderToken(this.httpHeaderName, this.httpHeaderValue, str, startIndexOfNextItem);
      string originalText = this.httpHeaderValue.Substring(this.startIndexOfNextItem, startIndexOfNextItem - this.startIndexOfNextItem);
      return (HttpHeaderValueLexer) new HttpHeaderValueLexer.HttpHeaderQuotedString(this.httpHeaderName, this.httpHeaderValue, str, originalText, startIndexOfNextItem);
    }

    private HttpHeaderValueLexer.HttpHeaderToken ReadNextToken()
    {
      HttpHeaderValueLexer headerValueLexer = this.ReadNextTokenOrQuotedString();
      return headerValueLexer.Type != HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString ? (HttpHeaderValueLexer.HttpHeaderToken) headerValueLexer : throw new ODataException(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString((object) this.httpHeaderName, (object) this.httpHeaderValue, (object) this.startIndexOfNextItem));
    }

    private HttpHeaderValueLexer.HttpHeaderSeparator ReadNextSeparator()
    {
      string p3 = this.httpHeaderValue.Substring(this.startIndexOfNextItem, 1);
      if (p3 != "," && p3 != ";" && p3 != "=")
        throw new ODataException(Strings.HttpHeaderValueLexer_UnrecognizedSeparator((object) this.httpHeaderName, (object) this.httpHeaderValue, (object) this.startIndexOfNextItem, (object) p3));
      return new HttpHeaderValueLexer.HttpHeaderSeparator(this.httpHeaderName, this.httpHeaderValue, p3, this.startIndexOfNextItem + 1);
    }

    internal enum HttpHeaderValueItemType
    {
      Start,
      Token,
      QuotedString,
      ElementSeparator,
      ParameterSeparator,
      ValueSeparator,
      End,
    }

    private sealed class HttpHeaderStart : HttpHeaderValueLexer
    {
      internal HttpHeaderStart(string httpHeaderName, string httpHeaderValue)
        : base(httpHeaderName, httpHeaderValue, (string) null, (string) null, 0)
      {
      }

      internal override HttpHeaderValueLexer.HttpHeaderValueItemType Type => HttpHeaderValueLexer.HttpHeaderValueItemType.Start;

      internal override HttpHeaderValueLexer ReadNext() => this.httpHeaderValue == null || this.EndOfHeaderValue() ? (HttpHeaderValueLexer) HttpHeaderValueLexer.HttpHeaderEnd.Instance : (HttpHeaderValueLexer) this.ReadNextToken();
    }

    private sealed class HttpHeaderToken : HttpHeaderValueLexer
    {
      internal HttpHeaderToken(
        string httpHeaderName,
        string httpHeaderValue,
        string value,
        int startIndexOfNextItem)
        : base(httpHeaderName, httpHeaderValue, value, value, startIndexOfNextItem)
      {
      }

      internal override HttpHeaderValueLexer.HttpHeaderValueItemType Type => HttpHeaderValueLexer.HttpHeaderValueItemType.Token;

      internal override HttpHeaderValueLexer ReadNext() => this.EndOfHeaderValue() ? (HttpHeaderValueLexer) HttpHeaderValueLexer.HttpHeaderEnd.Instance : (HttpHeaderValueLexer) this.ReadNextSeparator();
    }

    private sealed class HttpHeaderQuotedString : HttpHeaderValueLexer
    {
      internal HttpHeaderQuotedString(
        string httpHeaderName,
        string httpHeaderValue,
        string value,
        string originalText,
        int startIndexOfNextItem)
        : base(httpHeaderName, httpHeaderValue, value, originalText, startIndexOfNextItem)
      {
      }

      internal override HttpHeaderValueLexer.HttpHeaderValueItemType Type => HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString;

      internal override HttpHeaderValueLexer ReadNext()
      {
        if (this.EndOfHeaderValue())
          return (HttpHeaderValueLexer) HttpHeaderValueLexer.HttpHeaderEnd.Instance;
        HttpHeaderValueLexer.HttpHeaderSeparator httpHeaderSeparator = this.ReadNextSeparator();
        return httpHeaderSeparator.Value == "," || httpHeaderSeparator.Value == ";" ? (HttpHeaderValueLexer) httpHeaderSeparator : throw new ODataException(Strings.HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString((object) this.httpHeaderName, (object) this.httpHeaderValue, (object) this.startIndexOfNextItem, (object) httpHeaderSeparator.Value));
      }
    }

    private sealed class HttpHeaderSeparator : HttpHeaderValueLexer
    {
      internal HttpHeaderSeparator(
        string httpHeaderName,
        string httpHeaderValue,
        string value,
        int startIndexOfNextItem)
        : base(httpHeaderName, httpHeaderValue, value, value, startIndexOfNextItem)
      {
      }

      internal override HttpHeaderValueLexer.HttpHeaderValueItemType Type
      {
        get
        {
          switch (this.Value)
          {
            case ",":
              return HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator;
            case ";":
              return HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator;
            default:
              return HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator;
          }
        }
      }

      internal override HttpHeaderValueLexer ReadNext()
      {
        if (this.EndOfHeaderValue())
          throw new ODataException(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator((object) this.httpHeaderName, (object) this.httpHeaderValue, (object) this.startIndexOfNextItem, (object) this.originalText));
        return this.Value == "=" ? this.ReadNextTokenOrQuotedString() : (HttpHeaderValueLexer) this.ReadNextToken();
      }
    }

    private sealed class HttpHeaderEnd : HttpHeaderValueLexer
    {
      internal static readonly HttpHeaderValueLexer.HttpHeaderEnd Instance = new HttpHeaderValueLexer.HttpHeaderEnd();

      private HttpHeaderEnd()
        : base((string) null, (string) null, (string) null, (string) null, -1)
      {
      }

      internal override HttpHeaderValueLexer.HttpHeaderValueItemType Type => HttpHeaderValueLexer.HttpHeaderValueItemType.End;

      internal override HttpHeaderValueLexer ReadNext() => (HttpHeaderValueLexer) null;
    }
  }
}
