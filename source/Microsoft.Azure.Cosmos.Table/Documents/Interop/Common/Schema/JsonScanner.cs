// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.JsonScanner
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Documents.Interop.Common.Schema
{
  internal sealed class JsonScanner
  {
    private readonly JsonScanner.BufferReader reader;
    private JsonScanner.ScanState state;
    private JsonToken currentToken;

    public JsonScanner(string buffer)
    {
      this.reader = new JsonScanner.BufferReader(buffer);
      this.state = JsonScanner.ScanState.Initial;
    }

    public bool IsEof => this.reader.IsEof;

    public JsonToken GetCurrentToken()
    {
      if (this.state != JsonScanner.ScanState.HasValue)
        throw new InvalidOperationException();
      return this.currentToken;
    }

    public bool ScanNext()
    {
      this.state = this.state == JsonScanner.ScanState.Initial || this.state == JsonScanner.ScanState.HasValue ? this.ScanNextPrivate() : throw new InvalidOperationException();
      return this.state == JsonScanner.ScanState.HasValue;
    }

    private JsonScanner.ScanState ScanNextPrivate()
    {
      this.reader.StartNewAtom();
      char c;
      if (!this.reader.ReadNext(out c))
        return JsonScanner.ScanState.Error;
      switch (c)
      {
        case '"':
        case '\'':
          return this.ScanDelimitedString(c);
        case ',':
          this.currentToken = JsonToken.Comma;
          return JsonScanner.ScanState.HasValue;
        case '-':
          this.reader.UndoRead();
          return this.ScanDecimal();
        case '.':
          if (!this.reader.CheckNext(new Func<char, bool>(char.IsDigit)))
            return JsonScanner.ScanState.Error;
          this.reader.UndoRead();
          return this.ScanDecimal();
        case ':':
          this.currentToken = JsonToken.Colon;
          return JsonScanner.ScanState.HasValue;
        case '[':
          this.currentToken = JsonToken.BeginArray;
          return JsonScanner.ScanState.HasValue;
        case ']':
          this.currentToken = JsonToken.EndArray;
          return JsonScanner.ScanState.HasValue;
        case '{':
          this.currentToken = JsonToken.BeginObject;
          return JsonScanner.ScanState.HasValue;
        case '}':
          this.currentToken = JsonToken.EndObject;
          return JsonScanner.ScanState.HasValue;
        default:
          if (char.IsWhiteSpace(c))
          {
            this.reader.AdvanceWhile(new Func<char, bool>(char.IsWhiteSpace), true);
            return this.ScanNextPrivate();
          }
          if (!char.IsDigit(c))
            return this.ScanUnquotedString();
          if (c == '0' && this.reader.ReadNextIfEquals('x', 'X'))
            return this.ScanHexNumber();
          this.reader.UndoRead();
          return this.ScanDecimal();
      }
    }

    private JsonScanner.ScanState ScanDelimitedString(char quotationChar)
    {
      StringBuilder stringBuilder = new StringBuilder();
      JsonScanner.ScanStringState scanStringState = JsonScanner.ScanStringState.Continue;
      JsonScanner.UnicodeCharacter unicodeCharacter = new JsonScanner.UnicodeCharacter();
      char c;
      while (this.reader.ReadNext(out c))
      {
        switch (scanStringState)
        {
          case JsonScanner.ScanStringState.Continue:
            if ((int) c == (int) quotationChar)
            {
              scanStringState = JsonScanner.ScanStringState.Done;
              break;
            }
            if (c == '\\')
            {
              scanStringState = JsonScanner.ScanStringState.ReadEscapedCharacter;
              break;
            }
            break;
          case JsonScanner.ScanStringState.ReadEscapedCharacter:
            switch (c)
            {
              case '"':
                c = '"';
                break;
              case '\'':
                c = '\'';
                break;
              case '/':
                c = '/';
                break;
              case '\\':
                c = '\\';
                break;
              case 'b':
                c = '\b';
                break;
              case 'f':
                c = '\f';
                break;
              case 'n':
                c = '\n';
                break;
              case 'r':
                c = '\r';
                break;
              case 't':
                c = '\t';
                break;
              case 'u':
                unicodeCharacter = new JsonScanner.UnicodeCharacter();
                scanStringState = JsonScanner.ScanStringState.ReadUnicodeCharacter;
                goto label_22;
            }
            scanStringState = JsonScanner.ScanStringState.Continue;
            break;
          case JsonScanner.ScanStringState.ReadUnicodeCharacter:
            if (SchemaUtil.IsHexCharacter(c))
            {
              unicodeCharacter.Value <<= 4;
              unicodeCharacter.Value += SchemaUtil.GetHexValue(c);
              ++unicodeCharacter.DigitCount;
              if (unicodeCharacter.DigitCount == 4)
              {
                c = (char) unicodeCharacter.Value;
                scanStringState = JsonScanner.ScanStringState.Continue;
                break;
              }
              break;
            }
            scanStringState = JsonScanner.ScanStringState.Error;
            break;
        }
label_22:
        if (scanStringState == JsonScanner.ScanStringState.Continue)
          stringBuilder.Append(c);
        else if (scanStringState == JsonScanner.ScanStringState.Done || scanStringState == JsonScanner.ScanStringState.Error)
          break;
      }
      if (scanStringState != JsonScanner.ScanStringState.Done)
        return JsonScanner.ScanState.Error;
      this.currentToken = JsonToken.StringToken(stringBuilder.ToString(), this.reader.GetAtomText());
      return JsonScanner.ScanState.HasValue;
    }

    private JsonScanner.ScanState ScanUnquotedString()
    {
      this.reader.AdvanceWhile(new Func<char, bool>(SchemaUtil.IsIdentifierCharacter), true);
      switch (this.reader.GetAtomText())
      {
        case "Infinity":
          this.currentToken = JsonToken.Infinity;
          return JsonScanner.ScanState.HasValue;
        case "NaN":
          this.currentToken = JsonToken.NaN;
          return JsonScanner.ScanState.HasValue;
        case "true":
          this.currentToken = JsonToken.True;
          return JsonScanner.ScanState.HasValue;
        case "false":
          this.currentToken = JsonToken.False;
          return JsonScanner.ScanState.HasValue;
        case "null":
          this.currentToken = JsonToken.Null;
          return JsonScanner.ScanState.HasValue;
        default:
          return JsonScanner.ScanState.Error;
      }
    }

    private JsonScanner.ScanState ScanDecimal()
    {
      this.reader.ReadNextIfEquals('-');
      this.reader.AdvanceWhile(new Func<char, bool>(char.IsDigit), true);
      if (this.reader.ReadNextIfEquals('.'))
        this.reader.AdvanceWhile(new Func<char, bool>(char.IsDigit), true);
      if (this.reader.ReadNextIfEquals('e', 'E'))
      {
        this.reader.ReadNextIfEquals('+', '-');
        if (this.reader.AdvanceWhile(new Func<char, bool>(char.IsDigit), true) <= 0)
          return JsonScanner.ScanState.Error;
      }
      double number;
      if (this.reader.AdvanceWhile(new Func<char, bool>(SchemaUtil.IsIdentifierCharacter), true) > 0 || !this.reader.TryParseAtomAsDecimal(out number))
        return JsonScanner.ScanState.Error;
      this.currentToken = JsonToken.NumberToken(number, this.reader.GetAtomText());
      return JsonScanner.ScanState.HasValue;
    }

    private JsonScanner.ScanState ScanHexNumber()
    {
      this.reader.AdvanceWhile(new Func<char, bool>(SchemaUtil.IsHexCharacter), true);
      double number;
      if (this.reader.AdvanceWhile(new Func<char, bool>(SchemaUtil.IsIdentifierCharacter), true) > 0 || !this.reader.TryParseAtomAsHex(out number))
        return JsonScanner.ScanState.Error;
      this.currentToken = JsonToken.NumberToken(number, this.reader.GetAtomText());
      return JsonScanner.ScanState.HasValue;
    }

    private sealed class BufferReader
    {
      private readonly string buffer;
      private int atomStartIndex;
      private int atomEndIndex;

      public BufferReader(string buffer) => this.buffer = buffer != null ? buffer : throw new ArgumentNullException(nameof (buffer));

      public int AtomLength => this.atomEndIndex - this.atomStartIndex;

      public bool IsEof => this.atomEndIndex >= this.buffer.Length;

      public bool CheckNext(Func<char, bool> predicate) => !this.IsEof && predicate(this.buffer[this.atomEndIndex]);

      public bool ReadNext(out char c)
      {
        if (!this.IsEof)
        {
          c = this.buffer[this.atomEndIndex++];
          return true;
        }
        c = char.MinValue;
        return false;
      }

      public bool ReadNextIfEquals(char c)
      {
        if (this.IsEof || (int) c != (int) this.buffer[this.atomEndIndex])
          return false;
        ++this.atomEndIndex;
        return true;
      }

      public bool ReadNextIfEquals(char c1, char c2)
      {
        if (this.IsEof || (int) c1 != (int) this.buffer[this.atomEndIndex] && (int) c2 != (int) this.buffer[this.atomEndIndex])
          return false;
        ++this.atomEndIndex;
        return true;
      }

      public int AdvanceWhile(Func<char, bool> predicate, bool condition)
      {
        int atomEndIndex = this.atomEndIndex;
        while (this.atomEndIndex < this.buffer.Length && predicate(this.buffer[this.atomEndIndex]) == condition)
          ++this.atomEndIndex;
        return this.atomEndIndex - atomEndIndex;
      }

      public bool UndoRead()
      {
        if (this.atomEndIndex <= this.atomStartIndex)
          return false;
        --this.atomEndIndex;
        return true;
      }

      public void StartNewAtom() => this.atomStartIndex = this.atomEndIndex;

      public bool TryParseAtomAsDecimal(out double number)
      {
        if (double.TryParse(this.GetAtomText(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out number))
          return true;
        number = 0.0;
        return false;
      }

      public bool TryParseAtomAsHex(out double number)
      {
        string atomText = this.GetAtomText();
        if (!string.IsNullOrEmpty(atomText) && atomText.Length >= 2 && atomText[0] == '0' && atomText[1] != 'x')
        {
          int num = (int) atomText[1];
        }
        long result;
        if (long.TryParse(atomText.Substring(2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        {
          number = (double) result;
          return true;
        }
        number = 0.0;
        return false;
      }

      public string GetAtomText()
      {
        if (this.atomEndIndex > this.buffer.Length)
          throw new InvalidOperationException();
        return this.buffer.Substring(this.atomStartIndex, this.AtomLength);
      }
    }

    private enum ScanState
    {
      Error,
      HasValue,
      Initial,
    }

    private enum ScanStringState
    {
      Continue,
      Done,
      Error,
      ReadEscapedCharacter,
      ReadUnicodeCharacter,
    }

    private struct UnicodeCharacter
    {
      public int Value;
      public int DigitCount;
    }
  }
}
