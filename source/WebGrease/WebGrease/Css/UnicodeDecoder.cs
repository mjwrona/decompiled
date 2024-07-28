// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.UnicodeDecoder
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.IO;
using System.Text;
using WebGrease.Css.Ast;

namespace WebGrease.Css
{
  public class UnicodeDecoder
  {
    private readonly TextReader _reader;
    private char _currentChar;
    private string _readAhead;

    private UnicodeDecoder(TextReader reader) => this._reader = reader;

    public static string Decode(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return text;
      using (StringReader reader = new StringReader(text))
        return new UnicodeDecoder((TextReader) reader).GetUnicode();
    }

    private static int HValue(char ch)
    {
      int num = 0;
      if ('0' <= ch && ch <= '9')
        num = (int) ch - 48;
      else if ('a' <= ch && ch <= 'f')
        num = (int) ch - 97 + 10;
      else if ('A' <= ch && ch <= 'F')
        num = (int) ch - 65 + 10;
      return num;
    }

    private static bool IsH(char ch)
    {
      if ('0' <= ch && ch <= '9' || 'a' <= ch && ch <= 'f')
        return true;
      return 'A' <= ch && ch <= 'F';
    }

    private static bool IsSpace(char ch)
    {
      switch (ch)
      {
        case '\t':
        case '\n':
        case '\f':
        case '\r':
        case ' ':
          return true;
        default:
          return false;
      }
    }

    private string GetUnicode()
    {
      this.NextChar();
      StringBuilder stringBuilder = new StringBuilder();
      while (this._currentChar != char.MinValue)
      {
        if (this._currentChar == '\\' && UnicodeDecoder.IsH(this.PeekChar()))
        {
          int utf32 = this.GetUnicodeEncodingValue();
          if (utf32 >= 55296 && utf32 <= 56319)
          {
            this.NextChar();
            int num = utf32;
            if (this._currentChar != '\\' || !UnicodeDecoder.IsH(this.PeekChar()))
              throw new AstException("High surrogate should be followed by the low surrogate.");
            int unicodeEncodingValue = this.GetUnicodeEncodingValue();
            if (unicodeEncodingValue < 56320 || unicodeEncodingValue > 57343)
              throw new AstException("Invalid low surrogate.");
            utf32 = 65536 + (num - 55296) * 1024 + (unicodeEncodingValue - 56320);
          }
          stringBuilder.Append(char.ConvertFromUtf32(utf32));
        }
        else
          stringBuilder.Append(this._currentChar);
        this.NextChar();
      }
      return stringBuilder.ToString();
    }

    private int GetUnicodeEncodingValue()
    {
      int unicodeEncodingValue = 0;
      int num = 0;
      while (num++ < 6 && UnicodeDecoder.IsH(this.PeekChar()))
      {
        this.NextChar();
        unicodeEncodingValue = unicodeEncodingValue * 16 + UnicodeDecoder.HValue(this._currentChar);
      }
      if (UnicodeDecoder.IsSpace(this.PeekChar()))
        this.NextChar();
      return unicodeEncodingValue;
    }

    private void NextChar()
    {
      if (this._readAhead != null)
      {
        this._currentChar = this._readAhead[0];
        this._readAhead = this._readAhead.Length == 1 ? (string) null : this._readAhead.Substring(1);
      }
      else
      {
        int num = this._reader.Read();
        if (num < 0)
          this._currentChar = char.MinValue;
        else
          this._currentChar = (char) num;
      }
    }

    private char PeekChar()
    {
      if (this._readAhead != null)
        return this._readAhead[0];
      int num = this._reader.Peek();
      return num < 0 ? char.MinValue : (char) num;
    }
  }
}
