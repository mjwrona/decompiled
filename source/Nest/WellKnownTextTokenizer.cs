// Decompiled with JetBrains decompiler
// Type: Nest.WellKnownTextTokenizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Nest
{
  internal class WellKnownTextTokenizer : IDisposable
  {
    public const int CarriageReturn = 13;
    private const int CharacterTypesLength = 256;
    public const int Comma = 44;
    public const int Comment = 35;
    public const int Dot = 46;
    public const string Empty = "EMPTY";
    public const int Linefeed = 10;
    public const int LParen = 40;
    public const int Minus = 45;
    public const string NaN = "NAN";
    private const int NeedChar = 2147483647;
    public const int Plus = 43;
    public const int RParen = 41;
    private static readonly CharacterType[] CharacterTypes = new CharacterType[256];
    private readonly List<char> _buffer = new List<char>();
    private readonly TextReader _reader;
    private int _peekChar = int.MaxValue;
    private bool _pushed;

    static WellKnownTextTokenizer()
    {
      WellKnownTextTokenizer.Chars(97, 122, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(65, 90, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(160, (int) byte.MaxValue, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(48, 57, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(40, 41, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(43, 43, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(44, 44, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(45, 46, CharacterType.Alpha);
      WellKnownTextTokenizer.Chars(35, 35, CharacterType.Comment);
    }

    public WellKnownTextTokenizer(TextReader reader) => this._reader = reader ?? throw new ArgumentNullException(nameof (reader));

    public int LineNumber { get; private set; } = 1;

    public int Position { get; private set; }

    public TokenType TokenType { get; private set; }

    public string TokenValue { get; private set; }

    public void Dispose() => this._reader?.Dispose();

    private static void Chars(int low, int high, CharacterType type)
    {
      if (low < 0)
        low = 0;
      if (high >= 256)
        high = (int) byte.MaxValue;
      while (low <= high)
        WellKnownTextTokenizer.CharacterTypes[low++] = type;
    }

    public string TokenString()
    {
      switch (this.TokenType)
      {
        case TokenType.None:
          return "END-OF-STREAM";
        case TokenType.Word:
          return this.TokenValue;
        case TokenType.LParen:
          return "(";
        case TokenType.RParen:
          return ")";
        case TokenType.Comma:
          return ",";
        default:
          return string.Format("'{0}'", (object) (char) this._peekChar);
      }
    }

    private int Read()
    {
      ++this.Position;
      return this._reader.Read();
    }

    public TokenType PeekToken()
    {
      int position = this.Position;
      TokenType tokenType = this.NextToken();
      this.Position = position;
      this._pushed = true;
      return tokenType;
    }

    public TokenType NextToken()
    {
      if (this._pushed)
      {
        this._pushed = false;
        this.Position += !string.IsNullOrEmpty(this.TokenValue) ? 1 + this.TokenValue.Length : 1;
        return this.TokenType;
      }
      this.TokenValue = (string) null;
      int index = this._peekChar;
      if (index < 0)
        index = int.MaxValue;
      if (index == int.MaxValue)
      {
        index = this.Read();
        if (index < 0)
          return this.TokenType = TokenType.None;
      }
      this._peekChar = int.MaxValue;
      CharacterType characterType1;
      for (characterType1 = index < 256 ? WellKnownTextTokenizer.CharacterTypes[index] : CharacterType.Alpha; characterType1 == CharacterType.Whitespace; characterType1 = index < 256 ? WellKnownTextTokenizer.CharacterTypes[index] : CharacterType.Alpha)
      {
        switch (index)
        {
          case 10:
            ++this.LineNumber;
            this.Position = 0;
            goto default;
          case 13:
            ++this.LineNumber;
            this.Position = 0;
            index = this.Read();
            if (index == 10)
            {
              index = this.Read();
              break;
            }
            break;
          default:
            index = this.Read();
            break;
        }
        if (index < 0)
          return this.TokenType = TokenType.None;
      }
      switch (index)
      {
        case 40:
          return this.TokenType = TokenType.LParen;
        case 41:
          return this.TokenType = TokenType.RParen;
        case 44:
          return this.TokenType = TokenType.Comma;
        default:
          switch (characterType1)
          {
            case CharacterType.Alpha:
              int length = 0;
              CharacterType characterType2;
              do
              {
                this._buffer.Insert(length++, (char) index);
                index = this.Read();
                if (index < 0)
                  characterType2 = CharacterType.Whitespace;
                else if (index < 256)
                {
                  if (index != 40 && index != 41 && index != 44)
                    characterType2 = WellKnownTextTokenizer.CharacterTypes[index];
                  else
                    break;
                }
                else
                  characterType2 = CharacterType.Alpha;
              }
              while (characterType2 == CharacterType.Alpha);
              this._peekChar = index;
              this.TokenValue = new string(this._buffer.ToArray(), 0, length);
              return this.TokenType = TokenType.Word;
            case CharacterType.Comment:
              int num;
              do
                ;
              while ((num = this.Read()) != 10 && num != 13 && num >= 0);
              this._peekChar = num;
              return this.NextToken();
            default:
              return this.TokenType = TokenType.None;
          }
      }
    }
  }
}
