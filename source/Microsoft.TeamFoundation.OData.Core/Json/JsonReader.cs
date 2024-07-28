// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.OData.Json
{
  [DebuggerDisplay("{NodeType}: {Value}")]
  internal class JsonReader : IJsonStreamReader, IJsonReader, IDisposable
  {
    private const int InitialCharacterBufferSize = 2040;
    private readonly TextReader reader;
    private readonly bool isIeee754Compatible;
    private readonly Stack<JsonReader.Scope> scopes;
    private readonly bool allowAnnotations;
    private bool endOfInputReached;
    private bool readingStream;
    private bool canStream;
    private char streamOpeningQuoteCharacter = '"';
    private char[] characterBuffer;
    private int storedCharacterCount;
    private int tokenStartIndex;
    private int balancedQuoteCount;
    private JsonNodeType nodeType;
    private object nodeValue;
    private StringBuilder stringValueBuilder;

    public JsonReader(TextReader reader, bool isIeee754Compatible)
    {
      this.nodeType = JsonNodeType.None;
      this.nodeValue = (object) null;
      this.reader = reader;
      this.storedCharacterCount = 0;
      this.tokenStartIndex = 0;
      this.endOfInputReached = false;
      this.isIeee754Compatible = isIeee754Compatible;
      this.allowAnnotations = true;
      this.scopes = new Stack<JsonReader.Scope>();
      this.scopes.Push(new JsonReader.Scope(JsonReader.ScopeType.Root));
    }

    public ICharArrayPool ArrayPool { get; set; }

    public virtual object Value
    {
      get
      {
        if (this.readingStream)
          throw JsonReaderExtensions.CreateException(Strings.JsonReader_CannotAccessValueInStreamState);
        if (this.canStream)
        {
          if (this.nodeType != JsonNodeType.Property)
            this.canStream = false;
          if (this.nodeType == JsonNodeType.PrimitiveValue)
            this.nodeValue = this.characterBuffer[this.tokenStartIndex] != 'n' ? (object) this.ParseStringPrimitiveValue(out bool _) : this.ParseNullPrimitiveValue();
        }
        return this.nodeValue;
      }
    }

    public virtual JsonNodeType NodeType => this.nodeType;

    public virtual bool IsIeee754Compatible => this.isIeee754Compatible;

    public bool CanStream() => this.canStream;

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not really feasible to extract code to methods without introducing unnecessary complexity.")]
    public virtual bool Read()
    {
      if (this.readingStream)
        throw JsonReaderExtensions.CreateException(Strings.JsonReader_CannotCallReadInStreamState);
      if (this.canStream)
      {
        this.canStream = false;
        if (this.nodeType == JsonNodeType.PrimitiveValue)
        {
          if (this.characterBuffer[this.tokenStartIndex] == 'n')
            this.ParseNullPrimitiveValue();
          else
            this.ParseStringPrimitiveValue();
        }
      }
      this.nodeValue = (object) null;
      if (!this.SkipWhitespaces())
        return this.EndOfInput();
      JsonReader.Scope scope = this.scopes.Peek();
      bool flag = false;
      if (this.characterBuffer[this.tokenStartIndex] == ',')
      {
        flag = true;
        ++this.tokenStartIndex;
        if (!this.SkipWhitespaces())
          return this.EndOfInput();
      }
      switch (scope.Type)
      {
        case JsonReader.ScopeType.Root:
          if (flag)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedComma((object) JsonReader.ScopeType.Root));
          if (scope.ValueCount > 0)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_MultipleTopLevelValues);
          this.nodeType = this.ParseValue();
          break;
        case JsonReader.ScopeType.Array:
          if (flag && scope.ValueCount == 0)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedComma((object) JsonReader.ScopeType.Array));
          if (this.characterBuffer[this.tokenStartIndex] == ']')
          {
            ++this.tokenStartIndex;
            if (flag)
              throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedComma((object) JsonReader.ScopeType.Array));
            this.PopScope();
            this.nodeType = JsonNodeType.EndArray;
            break;
          }
          if (!flag && scope.ValueCount > 0)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_MissingComma((object) JsonReader.ScopeType.Array));
          this.nodeType = this.ParseValue();
          break;
        case JsonReader.ScopeType.Object:
          if (flag && scope.ValueCount == 0)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedComma((object) JsonReader.ScopeType.Object));
          if (this.characterBuffer[this.tokenStartIndex] == '}')
          {
            ++this.tokenStartIndex;
            if (flag)
              throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedComma((object) JsonReader.ScopeType.Object));
            this.PopScope();
            this.nodeType = JsonNodeType.EndObject;
            break;
          }
          if (!flag && scope.ValueCount > 0)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_MissingComma((object) JsonReader.ScopeType.Object));
          this.nodeType = this.ParseProperty();
          break;
        case JsonReader.ScopeType.Property:
          if (flag)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedComma((object) JsonReader.ScopeType.Property));
          this.nodeType = this.ParseValue();
          break;
        default:
          throw JsonReaderExtensions.CreateException(Strings.General_InternalError((object) InternalErrorCodes.JsonReader_Read));
      }
      return true;
    }

    public Stream CreateReadStream()
    {
      this.canStream = this.canStream ? false : throw JsonReaderExtensions.CreateException(Strings.JsonReader_CannotCreateReadStream);
      if ((this.streamOpeningQuoteCharacter = this.characterBuffer[this.tokenStartIndex]) == 'n')
      {
        this.ParseNullPrimitiveValue();
        ++this.scopes.Peek().ValueCount;
        this.Read();
        return (Stream) new ODataBinaryStreamReader((Func<char[], int, int, int>) ((a, b, c) => 0));
      }
      ++this.tokenStartIndex;
      this.readingStream = true;
      return (Stream) new ODataBinaryStreamReader(new Func<char[], int, int, int>(this.ReadChars));
    }

    public TextReader CreateTextReader()
    {
      this.canStream = this.canStream ? false : throw JsonReaderExtensions.CreateException(Strings.JsonReader_CannotCreateTextReader);
      this.SkipWhitespaces();
      if ((this.streamOpeningQuoteCharacter = this.characterBuffer[this.tokenStartIndex]) == 'n')
      {
        this.ParseNullPrimitiveValue();
        ++this.scopes.Peek().ValueCount;
        this.Read();
        return (TextReader) new ODataTextStreamReader((Func<char[], int, int, int>) ((a, b, c) => 0));
      }
      if (this.NodeType == JsonNodeType.PrimitiveValue)
        ++this.tokenStartIndex;
      this.readingStream = true;
      return (TextReader) new ODataTextStreamReader(new Func<char[], int, int, int>(this.ReadChars));
    }

    public void Dispose()
    {
      if (this.ArrayPool == null || this.characterBuffer == null)
        return;
      BufferUtils.ReturnToBuffer(this.ArrayPool, this.characterBuffer);
      this.characterBuffer = (char[]) null;
    }

    private static bool IsWhitespaceCharacter(char character) => character <= ' ' && (character == ' ' || character == '\t' || character == '\n' || character == '\r');

    private JsonNodeType ParseValue()
    {
      ++this.scopes.Peek().ValueCount;
      char c = this.characterBuffer[this.tokenStartIndex];
      switch (c)
      {
        case '"':
        case '\'':
          this.canStream = true;
          break;
        case '[':
          this.PushScope(JsonReader.ScopeType.Array);
          ++this.tokenStartIndex;
          this.SkipWhitespaces();
          this.canStream = this.characterBuffer[this.tokenStartIndex] == '"' || this.characterBuffer[this.tokenStartIndex] == '\'' || this.characterBuffer[this.tokenStartIndex] == 'n';
          return JsonNodeType.StartArray;
        case 'f':
        case 't':
          this.nodeValue = this.ParseBooleanPrimitiveValue();
          break;
        case 'n':
          this.canStream = true;
          break;
        case '{':
          this.PushScope(JsonReader.ScopeType.Object);
          ++this.tokenStartIndex;
          return JsonNodeType.StartObject;
        default:
          if (!char.IsDigit(c) && c != '-' && c != '.')
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedToken);
          this.nodeValue = this.ParseNumberPrimitiveValue();
          break;
      }
      this.TryPopPropertyScope();
      return JsonNodeType.PrimitiveValue;
    }

    private JsonNodeType ParseProperty()
    {
      ++this.scopes.Peek().ValueCount;
      this.PushScope(JsonReader.ScopeType.Property);
      this.nodeValue = (object) this.ParseName();
      if (string.IsNullOrEmpty((string) this.nodeValue))
        throw JsonReaderExtensions.CreateException(Strings.JsonReader_InvalidPropertyNameOrUnexpectedComma((object) (string) this.nodeValue));
      if (!this.SkipWhitespaces() || this.characterBuffer[this.tokenStartIndex] != ':')
        throw JsonReaderExtensions.CreateException(Strings.JsonReader_MissingColon((object) (string) this.nodeValue));
      ++this.tokenStartIndex;
      this.SkipWhitespaces();
      this.canStream = this.characterBuffer[this.tokenStartIndex] == '{' || this.characterBuffer[this.tokenStartIndex] == '[';
      return JsonNodeType.Property;
    }

    private string ParseStringPrimitiveValue() => this.ParseStringPrimitiveValue(out bool _);

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
    private string ParseStringPrimitiveValue(out bool hasLeadingBackslash)
    {
      hasLeadingBackslash = false;
      char ch1 = this.characterBuffer[this.tokenStartIndex];
      ++this.tokenStartIndex;
      StringBuilder stringBuilder = (StringBuilder) null;
      int characterCount = 0;
      while (this.tokenStartIndex + characterCount < this.storedCharacterCount || this.ReadInput())
      {
        char ch2 = this.characterBuffer[this.tokenStartIndex + characterCount];
        if (ch2 == '\\')
        {
          if (characterCount == 0 && stringBuilder == null)
            hasLeadingBackslash = true;
          if (stringBuilder == null)
          {
            if (this.stringValueBuilder == null)
              this.stringValueBuilder = new StringBuilder();
            else
              this.stringValueBuilder.Length = 0;
            stringBuilder = this.stringValueBuilder;
          }
          stringBuilder.Append(this.ConsumeTokenToString(characterCount));
          characterCount = 0;
          if (!this.EnsureAvailableCharacters(2))
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) "\\"));
          char ch3 = this.characterBuffer[this.tokenStartIndex + 1];
          this.tokenStartIndex += 2;
          switch (ch3)
          {
            case '"':
            case '\'':
            case '/':
            case '\\':
              stringBuilder.Append(ch3);
              continue;
            case 'b':
              stringBuilder.Append('\b');
              continue;
            case 'f':
              stringBuilder.Append('\f');
              continue;
            case 'n':
              stringBuilder.Append('\n');
              continue;
            case 'r':
              stringBuilder.Append('\r');
              continue;
            case 't':
              stringBuilder.Append('\t');
              continue;
            case 'u':
              if (!this.EnsureAvailableCharacters(4))
                throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) "\\uXXXX"));
              string s = this.ConsumeTokenToString(4);
              int result;
              if (!int.TryParse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result))
                throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) ("\\u" + s)));
              stringBuilder.Append((char) result);
              continue;
            default:
              throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) ("\\" + ch3.ToString())));
          }
        }
        else
        {
          if ((int) ch2 == (int) ch1)
          {
            string stringPrimitiveValue = this.ConsumeTokenToString(characterCount);
            ++this.tokenStartIndex;
            if (stringBuilder != null)
            {
              stringBuilder.Append(stringPrimitiveValue);
              stringPrimitiveValue = stringBuilder.ToString();
            }
            return stringPrimitiveValue;
          }
          ++characterCount;
        }
      }
      throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedEndOfString);
    }

    private object ParseNullPrimitiveValue()
    {
      string name = this.ParseName();
      if (!string.Equals(name, "null", StringComparison.Ordinal))
        throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedToken((object) name));
      return (object) null;
    }

    private object ParseBooleanPrimitiveValue()
    {
      string name = this.ParseName();
      if (string.Equals(name, "false", StringComparison.Ordinal))
        return (object) false;
      if (string.Equals(name, "true", StringComparison.Ordinal))
        return (object) true;
      throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedToken((object) name));
    }

    private object ParseNumberPrimitiveValue()
    {
      int characterCount;
      for (characterCount = 1; this.tokenStartIndex + characterCount < this.storedCharacterCount || this.ReadInput(); ++characterCount)
      {
        char c = this.characterBuffer[this.tokenStartIndex + characterCount];
        if (!char.IsDigit(c) && c != '.' && c != 'E' && c != 'e' && c != '-' && c != '+')
          break;
      }
      string str = this.ConsumeTokenToString(characterCount);
      int result1;
      if (int.TryParse(str, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
        return (object) result1;
      Decimal result2;
      if (!this.isIeee754Compatible && Decimal.TryParse(str, NumberStyles.Number, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
        return (object) result2;
      double result3;
      if (double.TryParse(str, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result3))
        return (object) result3;
      throw JsonReaderExtensions.CreateException(Strings.JsonReader_InvalidNumberFormat((object) str));
    }

    private string ParseName()
    {
      switch (this.characterBuffer[this.tokenStartIndex])
      {
        case '"':
        case '\'':
          return this.ParseStringPrimitiveValue();
        default:
          int characterCount = 0;
          do
          {
            char c = this.characterBuffer[this.tokenStartIndex + characterCount];
            if (c == '_' || char.IsLetterOrDigit(c) || c == '$' || this.allowAnnotations && (c == '.' || c == '@'))
              ++characterCount;
            else
              break;
          }
          while (this.tokenStartIndex + characterCount < this.storedCharacterCount || this.ReadInput());
          return this.ConsumeTokenToString(characterCount);
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
    private int ReadChars(char[] chars, int offset, int maxLength)
    {
      if (!this.readingStream)
        return 0;
      int num = 0;
      while (num < maxLength && (this.tokenStartIndex < this.storedCharacterCount || this.ReadInput()))
      {
        char ch = this.characterBuffer[this.tokenStartIndex];
        bool flag = true;
        if ((int) ch == (int) JsonReader.GetClosingQuoteCharacter(this.streamOpeningQuoteCharacter) && --this.balancedQuoteCount < 1)
        {
          if (ch != '"')
          {
            chars[num + offset] = ch;
            ++num;
            this.scopes.Pop();
          }
          ++this.tokenStartIndex;
          this.readingStream = false;
          ++this.scopes.Peek().ValueCount;
          this.Read();
          return num;
        }
        if ((int) ch == (int) this.streamOpeningQuoteCharacter)
          ++this.balancedQuoteCount;
        if (ch == '\\')
        {
          ++this.tokenStartIndex;
          if (!this.EnsureAvailableCharacters(1))
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) "\\uXXXX"));
          ch = this.characterBuffer[this.tokenStartIndex];
          switch (ch)
          {
            case '"':
            case '\'':
            case '/':
            case '\\':
              break;
            case 'b':
              ch = '\b';
              break;
            case 'f':
              ch = '\f';
              break;
            case 'n':
              ch = '\n';
              break;
            case 'r':
              ch = '\r';
              break;
            case 't':
              ch = '\t';
              break;
            case 'u':
              ++this.tokenStartIndex;
              if (!this.EnsureAvailableCharacters(4))
                throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) "\\uXXXX"));
              string s = this.ConsumeTokenToString(4);
              int result;
              if (!int.TryParse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result))
                throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) ("\\u" + s)));
              ch = (char) result;
              flag = false;
              break;
            default:
              throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnrecognizedEscapeSequence((object) ("\\" + ch.ToString())));
          }
        }
        chars[num + offset] = ch;
        ++num;
        if (flag)
          ++this.tokenStartIndex;
      }
      return num >= maxLength ? num : throw JsonReaderExtensions.CreateException(Strings.JsonReader_UnexpectedEndOfString);
    }

    private static char GetClosingQuoteCharacter(char openingCharacter)
    {
      if (openingCharacter == '[')
        return ']';
      return openingCharacter == '{' ? '}' : openingCharacter;
    }

    private bool EndOfInput()
    {
      if (this.scopes.Count > 1)
        throw JsonReaderExtensions.CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
      this.nodeType = JsonNodeType.EndOfInput;
      if (this.ArrayPool != null)
      {
        BufferUtils.ReturnToBuffer(this.ArrayPool, this.characterBuffer);
        this.characterBuffer = (char[]) null;
      }
      return false;
    }

    private void PushScope(JsonReader.ScopeType newScopeType) => this.scopes.Push(new JsonReader.Scope(newScopeType));

    private void PopScope()
    {
      this.scopes.Pop();
      this.TryPopPropertyScope();
    }

    private void TryPopPropertyScope()
    {
      if (this.scopes.Peek().Type != JsonReader.ScopeType.Property)
        return;
      this.scopes.Pop();
    }

    private bool SkipWhitespaces()
    {
      do
      {
        for (; this.tokenStartIndex < this.storedCharacterCount; ++this.tokenStartIndex)
        {
          if (!JsonReader.IsWhitespaceCharacter(this.characterBuffer[this.tokenStartIndex]))
            return true;
        }
      }
      while (this.ReadInput());
      return false;
    }

    private bool EnsureAvailableCharacters(int characterCountAfterTokenStart)
    {
      while (this.tokenStartIndex + characterCountAfterTokenStart > this.storedCharacterCount)
      {
        if (!this.ReadInput())
          return false;
      }
      return true;
    }

    private string ConsumeTokenToString(int characterCount)
    {
      string str = new string(this.characterBuffer, this.tokenStartIndex, characterCount);
      this.tokenStartIndex += characterCount;
      return str;
    }

    private bool ReadInput()
    {
      if (this.endOfInputReached)
        return false;
      if (this.characterBuffer == null)
        this.characterBuffer = BufferUtils.RentFromBuffer(this.ArrayPool, 2040);
      if (this.tokenStartIndex == this.storedCharacterCount)
      {
        this.tokenStartIndex = 0;
        this.storedCharacterCount = 0;
      }
      else if (this.storedCharacterCount == this.characterBuffer.Length)
      {
        if (this.tokenStartIndex < this.characterBuffer.Length / 4)
        {
          if (this.characterBuffer.Length == int.MaxValue)
            throw JsonReaderExtensions.CreateException(Strings.JsonReader_MaxBufferReached);
          int num = this.characterBuffer.Length * 2;
          char[] destinationArray = BufferUtils.RentFromBuffer(this.ArrayPool, num < 0 ? int.MaxValue : num);
          Array.Copy((Array) this.characterBuffer, this.tokenStartIndex, (Array) destinationArray, 0, this.storedCharacterCount - this.tokenStartIndex);
          this.storedCharacterCount -= this.tokenStartIndex;
          this.tokenStartIndex = 0;
          BufferUtils.ReturnToBuffer(this.ArrayPool, this.characterBuffer);
          this.characterBuffer = destinationArray;
        }
        else
        {
          Array.Copy((Array) this.characterBuffer, this.tokenStartIndex, (Array) this.characterBuffer, 0, this.storedCharacterCount - this.tokenStartIndex);
          this.storedCharacterCount -= this.tokenStartIndex;
          this.tokenStartIndex = 0;
        }
      }
      int num1 = this.reader.Read(this.characterBuffer, this.storedCharacterCount, this.characterBuffer.Length - this.storedCharacterCount);
      if (num1 == 0)
      {
        this.endOfInputReached = true;
        return false;
      }
      this.storedCharacterCount += num1;
      return true;
    }

    private enum ScopeType
    {
      Root,
      Array,
      Object,
      Property,
    }

    private sealed class Scope
    {
      private readonly JsonReader.ScopeType type;

      public Scope(JsonReader.ScopeType type) => this.type = type;

      public int ValueCount { get; set; }

      public JsonReader.ScopeType Type => this.type;
    }
  }
}
