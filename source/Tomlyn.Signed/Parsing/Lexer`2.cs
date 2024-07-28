// Decompiled with JetBrains decompiler
// Type: Tomlyn.Parsing.Lexer`2
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tomlyn.Helpers;
using Tomlyn.Syntax;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Parsing
{
  internal class Lexer<TSourceView, TCharReader> : ITokenProvider<TSourceView>
    where TSourceView : struct, ISourceView<TCharReader>
    where TCharReader : struct, CharacterIterator
  {
    private SyntaxTokenValue _token;
    private List<DiagnosticMessage>? _errors;
    private TCharReader _reader;
    private const int Eof = -1;
    private TSourceView _sourceView;
    private readonly StringBuilder _textBuilder;
    private readonly List<char32> _currentIdentifierChars;
    private LexerInternalState? _preview1;
    private LexerInternalState _current;

    public Lexer(TSourceView sourceView)
    {
      this._sourceView = sourceView;
      this._reader = sourceView.GetIterator();
      this._currentIdentifierChars = new List<char32>();
      this._textBuilder = new StringBuilder();
      this.Reset();
    }

    public TSourceView Source => this._sourceView;

    public bool HasErrors => this._errors != null && this._errors.Count > 0;

    public IEnumerable<DiagnosticMessage> Errors => (IEnumerable<DiagnosticMessage>) this._errors ?? Enumerable.Empty<DiagnosticMessage>();

    public bool MoveNext()
    {
      if (this._token.Kind == TokenKind.Eof)
        return false;
      if (this.State == LexerState.Key)
        this.NextTokenForKey();
      else
        this.NextTokenForValue();
      return true;
    }

    public SyntaxTokenValue Token => this._token;

    public LexerState State { get; set; }

    private TextPosition _position => this._current.Position;

    private char32 _c => this._current.CurrentChar;

    private void NextTokenForKey()
    {
      TextPosition position = this._position;
      switch ((int) this._c)
      {
        case -1:
          this._token = new SyntaxTokenValue(TokenKind.Eof, this._position, this._position);
          break;
        case 10:
          this._token = new SyntaxTokenValue(TokenKind.NewLine, position, position);
          this.NextChar();
          break;
        case 13:
          this.NextChar();
          if ((int) this._c == 10)
          {
            this._token = new SyntaxTokenValue(TokenKind.NewLine, position, this._position);
            this.NextChar();
            break;
          }
          this.AddError("Invalid \\r not followed by \\n", position, position);
          this._token = new SyntaxTokenValue(TokenKind.NewLine, position, position);
          break;
        case 34:
          this.ReadString(position, false);
          break;
        case 35:
          this.NextChar();
          this.ReadComment(position);
          break;
        case 39:
          this.ReadStringLiteral(position, false);
          break;
        case 46:
          this.NextChar();
          this._token = new SyntaxTokenValue(TokenKind.Dot, position, position);
          break;
        case 61:
          this.NextChar();
          this._token = new SyntaxTokenValue(TokenKind.Equal, position, position);
          break;
        case 91:
          this.NextChar();
          if ((int) this._c == 91)
          {
            this._token = new SyntaxTokenValue(TokenKind.OpenBracketDouble, position, this._position);
            this.NextChar();
            break;
          }
          this._token = new SyntaxTokenValue(TokenKind.OpenBracket, position, position);
          break;
        case 93:
          this.NextChar();
          if ((int) this._c == 93)
          {
            this._token = new SyntaxTokenValue(TokenKind.CloseBracketDouble, position, this._position);
            this.NextChar();
            break;
          }
          this._token = new SyntaxTokenValue(TokenKind.CloseBracket, position, position);
          break;
        case 123:
          this._token = new SyntaxTokenValue(TokenKind.OpenBrace, this._position, this._position);
          this.NextChar();
          break;
        case 125:
          this._token = new SyntaxTokenValue(TokenKind.CloseBrace, this._position, this._position);
          this.NextChar();
          break;
        default:
          if (this.ConsumeWhitespace())
            break;
          if (CharHelper.IsKeyStart(this._c))
          {
            this.ReadKey();
            break;
          }
          this._token = new SyntaxTokenValue(TokenKind.Invalid, this._position, this._position);
          this.NextChar();
          break;
      }
    }

    private void NextTokenForValue()
    {
      TextPosition position = this._position;
      switch ((int) this._c)
      {
        case -1:
          this._token = new SyntaxTokenValue(TokenKind.Eof, this._position, this._position);
          break;
        case 10:
          this._token = new SyntaxTokenValue(TokenKind.NewLine, position, this._position);
          this.NextChar();
          break;
        case 13:
          this.NextChar();
          if ((int) this._c == 10)
          {
            this._token = new SyntaxTokenValue(TokenKind.NewLine, position, this._position);
            this.NextChar();
            break;
          }
          this._token = new SyntaxTokenValue(TokenKind.NewLine, position, position);
          break;
        case 34:
          this.ReadString(position, true);
          break;
        case 35:
          this.NextChar();
          this.ReadComment(position);
          break;
        case 39:
          this.ReadStringLiteral(position, true);
          break;
        case 44:
          this._token = new SyntaxTokenValue(TokenKind.Comma, position, position);
          this.NextChar();
          break;
        case 91:
          this.NextChar();
          this._token = new SyntaxTokenValue(TokenKind.OpenBracket, position, position);
          break;
        case 93:
          this.NextChar();
          this._token = new SyntaxTokenValue(TokenKind.CloseBracket, position, position);
          break;
        case 123:
          this._token = new SyntaxTokenValue(TokenKind.OpenBrace, this._position, this._position);
          this.NextChar();
          break;
        case 125:
          this._token = new SyntaxTokenValue(TokenKind.CloseBrace, this._position, this._position);
          this.NextChar();
          break;
        default:
          if (this.ConsumeWhitespace())
            break;
          if ((int) this._c == 43 || (int) this._c == 45 || CharHelper.IsIdentifierStart(this._c))
          {
            this.ReadSpecialToken();
            break;
          }
          if (CharHelper.IsDigit(this._c))
          {
            this.ReadNumberOrDate();
            break;
          }
          this._token = new SyntaxTokenValue(TokenKind.Invalid, this._position, this._position);
          this.NextChar();
          break;
      }
    }

    private bool ConsumeWhitespace()
    {
      TextPosition position1 = this._position;
      TextPosition position2 = this._position;
      while (CharHelper.IsWhiteSpace(this._c))
      {
        position2 = this._position;
        this.NextChar();
      }
      if (!(position1 != this._position))
        return false;
      this._token = new SyntaxTokenValue(TokenKind.Whitespaces, position1, position2);
      return true;
    }

    private void ReadKey()
    {
      TextPosition position1 = this._position;
      TextPosition position2 = this._position;
      while (CharHelper.IsKeyContinue(this._c))
      {
        position2 = this._position;
        this.NextChar();
      }
      this._token = new SyntaxTokenValue(TokenKind.BasicKey, position1, position2);
    }

    private void ReadSpecialToken()
    {
      TextPosition position1 = this._position;
      TextPosition position2 = this._position;
      this._currentIdentifierChars.Clear();
      char32 c = this._c;
      this._currentIdentifierChars.Add(this._c);
      this.NextChar();
      if (((int) c == 43 || (int) c == 45) && CharHelper.IsDigit(this._c))
      {
        this._currentIdentifierChars.Clear();
        this.ReadNumberOrDate(new char32?(c), new TextPosition?(position1));
      }
      else
      {
        while (CharHelper.IsIdentifierContinue(this._c))
        {
          this._currentIdentifierChars.Add(this._c);
          position2 = this._position;
          this.NextChar();
        }
        this._token = !this.MatchCurrentIdentifier("true") ? (!this.MatchCurrentIdentifier("false") ? (!this.MatchCurrentIdentifier("inf") ? (!this.MatchCurrentIdentifier("+inf") ? (!this.MatchCurrentIdentifier("-inf") ? (!this.MatchCurrentIdentifier("nan") ? (!this.MatchCurrentIdentifier("+nan") ? (!this.MatchCurrentIdentifier("-nan") ? new SyntaxTokenValue(TokenKind.Invalid, position1, position2) : new SyntaxTokenValue(TokenKind.NegativeNan, position1, position2, BoxedValues.FloatNegativeNaN)) : new SyntaxTokenValue(TokenKind.PositiveNan, position1, position2, BoxedValues.FloatPositiveNaN)) : new SyntaxTokenValue(TokenKind.Nan, position1, position2, BoxedValues.FloatNan)) : new SyntaxTokenValue(TokenKind.NegativeInfinite, position1, position2, BoxedValues.FloatNegativeInfinity)) : new SyntaxTokenValue(TokenKind.PositiveInfinite, position1, position2, BoxedValues.FloatPositiveInfinity)) : new SyntaxTokenValue(TokenKind.Infinite, position1, position2, BoxedValues.FloatPositiveInfinity)) : new SyntaxTokenValue(TokenKind.False, position1, position2, BoxedValues.False)) : new SyntaxTokenValue(TokenKind.True, position1, position2, BoxedValues.True);
        this._currentIdentifierChars.Clear();
      }
    }

    private bool MatchCurrentIdentifier(string text)
    {
      if (this._currentIdentifierChars.Count != text.Length)
        return false;
      for (int index = 0; index < text.Length; ++index)
      {
        if ((int) this._currentIdentifierChars[index] != (int) text[index])
          return false;
      }
      return true;
    }

    private void ReadNumberOrDate(char32? signPrefix = null, TextPosition? signPrefixPos = null)
    {
      TextPosition textPosition = signPrefixPos ?? this._position;
      TextPosition position1 = this._position;
      bool flag1 = false;
      TextPosition position2 = this._position;
      bool hasValue = signPrefix.HasValue;
      bool flag2 = (int) this._c == 48;
      this._textBuilder.Length = 0;
      if (hasValue)
        this._textBuilder.AppendUtf32(signPrefix.Value);
      if (flag2)
      {
        this.NextChar();
        if (!hasValue && ((int) this._c == 120 || (int) this._c == 88 || (int) this._c == 111 || (int) this._c == 79 || (int) this._c == 98 || (int) this._c == 66))
        {
          string str1;
          string str2;
          string str3;
          Func<char32, bool> func1;
          Func<char32, int> func2;
          int num1;
          TokenKind kind;
          if ((int) this._c == 120 || (int) this._c == 88)
          {
            str1 = "hexadecimal";
            str2 = "[0-9a-zA-Z]";
            str3 = "0x";
            func1 = CharHelper.IsHexFunc;
            func2 = CharHelper.HexToDecFunc;
            num1 = 4;
            kind = TokenKind.IntegerHexa;
            if ((int) this._c == 88)
              this.AddError("Invalid capital X for hexadecimal. Use `x` instead.", this._position, this._position);
          }
          else if ((int) this._c == 111 || (int) this._c == 79)
          {
            str1 = "octal";
            str2 = "[0-7]";
            str3 = "0o";
            func1 = CharHelper.IsOctalFunc;
            func2 = CharHelper.OctalToDecFunc;
            num1 = 3;
            kind = TokenKind.IntegerOctal;
            if ((int) this._c == 79)
              this.AddError("Invalid capital O for octal. Use `o` instead.", this._position, this._position);
          }
          else
          {
            str1 = "binary";
            str2 = "0 or 1";
            str3 = "0b";
            func1 = CharHelper.IsBinaryFunc;
            func2 = CharHelper.BinaryToDecFunc;
            num1 = 1;
            kind = TokenKind.IntegerBinary;
            if ((int) this._c == 66)
              this.AddError("Invalid capital B for binary. Use `b` instead.", this._position, this._position);
          }
          TextPosition position3 = this._position;
          this.NextChar();
          int num2 = 64 / num1;
          int num3 = num2;
          bool flag3 = false;
          bool flag4 = false;
          ulong num4 = 0;
          while (true)
          {
            bool flag5 = false;
            if ((int) this._c == 95 || (flag5 = func1(this._c)))
            {
              bool flag6 = (int) this._c != 95;
              if (!flag4 && !flag6)
                this.AddError("An underscore must be surrounded by at least one " + str1 + " digit on each side", textPosition, textPosition);
              else if (flag6)
              {
                num4 = (num4 << num1) + (ulong) func2(this._c);
                --num3;
                if (num3 == -1)
                  this.AddError(string.Format("Invalid size of {0} integer. Expecting less than or equal {1} {2} digits", (object) str1, (object) num2, (object) str1), textPosition, textPosition);
              }
              flag4 = flag6;
              if (flag5)
                flag3 = true;
              position3 = this._position;
              this.NextChar();
            }
            else
              break;
          }
          if (!flag3)
          {
            this.AddError("Invalid " + str1 + " integer. Expecting at least one " + str2 + " after " + str3, textPosition, textPosition);
            this._token = new SyntaxTokenValue(TokenKind.Invalid, textPosition, position3);
            return;
          }
          if (!flag4)
          {
            this.AddError("Invalid " + str1 + " integer. Expecting a " + str2 + " after the last character", textPosition, textPosition);
            this._token = new SyntaxTokenValue(TokenKind.Invalid, textPosition, position3);
            return;
          }
          this._token = new SyntaxTokenValue(kind, textPosition, position3, (object) (long) num4);
          return;
        }
        this._textBuilder.Append('0');
      }
      TextPosition position4 = this._position;
      bool flag7 = false;
      bool isPreviousDigit = false;
      if (flag2)
      {
        int num = 0;
        isPreviousDigit = true;
        while ((int) this._c == 48 || (int) this._c == 95)
        {
          isPreviousDigit = (int) this._c == 48;
          if (isPreviousDigit)
          {
            this._textBuilder.Append((char) (int) this._c);
            ++num;
          }
          position1 = this._position;
          this.NextChar();
        }
        flag7 = num > 0;
      }
      this.ReadDigits(ref position1, isPreviousDigit);
      if ((int) this._c == 45 || (int) this._c == 58)
      {
        while (CharHelper.IsDateTime(this._c))
        {
          this._textBuilder.AppendUtf32(this._c);
          position1 = this._position;
          this.NextChar();
        }
        if (CharHelper.IsWhiteSpace(this._c) && CharHelper.IsDateTime(this.PeekChar()))
        {
          this._textBuilder.AppendUtf32(this._c);
          this.NextChar();
          while (CharHelper.IsDateTime(this._c))
          {
            this._textBuilder.AppendUtf32(this._c);
            position1 = this._position;
            this.NextChar();
          }
        }
        string str = this._textBuilder.ToString();
        if (hasValue)
        {
          this.AddError(string.Format("Invalid prefix `{0}` for the following offset/local date/time `{1}`", (object) signPrefix.Value, (object) str), textPosition, position1);
          str = str.Substring(1);
        }
        TomlDateTime time;
        if (DateTimeRFC3339.TryParseOffsetDateTime(str, out time))
          this._token = new SyntaxTokenValue(time.Kind == TomlDateTimeKind.OffsetDateTimeByZ ? TokenKind.OffsetDateTimeByZ : TokenKind.OffsetDateTimeByNumber, textPosition, position1, (object) time);
        else if (DateTimeRFC3339.TryParseLocalDateTime(str, out time))
          this._token = new SyntaxTokenValue(TokenKind.LocalDateTime, textPosition, position1, (object) time);
        else if (DateTimeRFC3339.TryParseLocalDate(str, out time))
          this._token = new SyntaxTokenValue(TokenKind.LocalDate, textPosition, position1, (object) time);
        else if (DateTimeRFC3339.TryParseLocalTime(str, out time))
        {
          this._token = new SyntaxTokenValue(TokenKind.LocalTime, textPosition, position1, (object) time);
        }
        else
        {
          DateTime result;
          if (DateTime.TryParse(str, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite, out result))
          {
            time = new TomlDateTime((DateTimeOffset) result, 0, TomlDateTimeKind.LocalDateTime);
            this._token = new SyntaxTokenValue(TokenKind.LocalDateTime, textPosition, position1, (object) time);
            this.AddError("Invalid format of date time/offset `" + str + "` not following RFC3339", textPosition, position1);
          }
          else
          {
            time = new TomlDateTime(DateTimeOffset.MinValue, 0, TomlDateTimeKind.LocalDateTime);
            this._token = new SyntaxTokenValue(TokenKind.LocalDateTime, textPosition, position1, (object) time);
            this.AddError("Unable to parse the date time/offset `" + str + "`", textPosition, position1);
          }
        }
      }
      else
      {
        if (flag7)
          this.AddError("Multiple leading 0 are not allowed", position4, position4);
        if ((int) this._c == 46)
        {
          this._textBuilder.Append('.');
          position1 = this._position;
          this.NextChar();
          if (!CharHelper.IsDigit(this._c))
          {
            this.AddError("Expecting at least one digit after the float dot .", this._position, this._position);
            this._token = new SyntaxTokenValue(TokenKind.Invalid, textPosition, position1);
            return;
          }
          flag1 = true;
          this.ReadDigits(ref position1, false);
        }
        if ((int) this._c == 101 || (int) this._c == 69)
        {
          flag1 = true;
          this._textBuilder.AppendUtf32(this._c);
          position1 = this._position;
          this.NextChar();
          if ((int) this._c == 43 || (int) this._c == 45)
          {
            this._textBuilder.AppendUtf32(this._c);
            position1 = this._position;
            this.NextChar();
          }
          if (!CharHelper.IsDigit(this._c))
          {
            this.AddError("Expecting at least one digit after the exponent", this._position, this._position);
            this._token = new SyntaxTokenValue(TokenKind.Invalid, textPosition, position1);
            return;
          }
          this.ReadDigits(ref position1, false);
        }
        string s = this._textBuilder.ToString();
        object obj1;
        if (flag1)
        {
          double result;
          if (!double.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            this.AddError("Unable to parse floating point `" + s + "`", textPosition, position1);
          if ((int) result != 0 & flag2)
            this.AddError("Unexpected leading zero (`0`) for float `" + s + "`", position2, position2);
          obj1 = result == 0.0 ? BoxedValues.FloatZero : (result == 1.0 ? BoxedValues.FloatOne : (object) result);
        }
        else
        {
          long result1;
          if (!long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
          {
            ulong result2;
            if (!ulong.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
              this.AddError("Unable to parse integer `" + s + "`", textPosition, position1);
            else
              result1 = (long) result2;
          }
          if (flag2 && result1 != 0L)
            this.AddError("Unexpected leading zero (`0`) for integer `" + s + "`", position2, position2);
          object obj2;
          switch (result1)
          {
            case 0:
              obj2 = BoxedValues.IntegerZero;
              break;
            case 1:
              obj2 = BoxedValues.IntegerOne;
              break;
            default:
              obj2 = (object) result1;
              break;
          }
          obj1 = obj2;
        }
        this._token = new SyntaxTokenValue(flag1 ? TokenKind.Float : TokenKind.Integer, textPosition, position1, obj1);
      }
    }

    private void ReadDigits(ref TextPosition end, bool isPreviousDigit)
    {
      bool flag;
      while ((flag = CharHelper.IsDigit(this._c)) || (int) this._c == 95)
      {
        if (flag)
        {
          this._textBuilder.AppendUtf32(this._c);
          isPreviousDigit = true;
        }
        else if (!isPreviousDigit)
          this.AddError("An underscore `_` must follow a digit and not another `_`", this._position, this._position);
        else
          isPreviousDigit = false;
        end = this._position;
        this.NextChar();
      }
      if (isPreviousDigit)
        return;
      this.AddError("Missing a digit after a trailing underscore `_`", this._position, this._position);
    }

    private void ReadString(TextPosition start, bool allowMultiline)
    {
      TextPosition position1 = this._position;
      bool flag = false;
      this.NextChar();
      if (allowMultiline && (int) this._c == 34)
      {
        TextPosition position2 = this._position;
        this.NextChar();
        if ((int) this._c == 34)
        {
          position1 = this._position;
          this.NextChar();
          flag = true;
          this.SkipImmediateNextLine();
        }
        else
        {
          this._token = new SyntaxTokenValue(TokenKind.String, start, position2, (object) string.Empty);
          return;
        }
      }
      this._textBuilder.Length = 0;
      while (true)
      {
        do
        {
          while ((int) this._c == 34 || (int) this._c == -1)
          {
            if (flag)
            {
              if ((int) this._c == 34)
              {
                int num = 0;
                while ((int) this._c == 34 && num < 5)
                {
                  ++num;
                  position1 = this._position;
                  this.NextChar();
                }
                if (num >= 3)
                {
                  for (int index = 0; index < num - 3; ++index)
                    this._textBuilder.Append('"');
                }
                else
                {
                  for (int index = 0; index < num; ++index)
                    this._textBuilder.Append('"');
                  continue;
                }
              }
              else
                this.AddError("Invalid End-Of-File found for multi-line string", position1, position1);
              this._token = new SyntaxTokenValue(TokenKind.StringMulti, start, position1, (object) this._textBuilder.ToString());
              return;
            }
            if ((int) this._c == 34)
            {
              position1 = this._position;
              this.NextChar();
            }
            else
              this.AddError("Invalid End-Of-File found on string literal", position1, position1);
            this._token = new SyntaxTokenValue(TokenKind.String, start, position1, (object) this._textBuilder.ToString());
            return;
          }
        }
        while (this.TryReadEscapeChar(ref position1));
        if (!flag && CharHelper.IsNewLine(this._c))
          this.AddError("Invalid newline in a string", this._position, this._position);
        else if (CharHelper.IsControlCharacter(this._c) && (!flag || !CharHelper.IsWhiteSpaceOrNewLine(this._c)))
          this.AddError("Invalid control character found " + ((char) (int) this._c).ToPrintableString(), start, start);
        this._textBuilder.AppendUtf32(this._c);
        position1 = this._position;
        this.NextChar();
      }
    }

    private void SkipImmediateNextLine()
    {
      if ((int) this._c == 13)
      {
        this.NextChar();
        if ((int) this._c != 10)
          return;
        this.NextChar();
      }
      else
      {
        if ((int) this._c != 10)
          return;
        this.NextChar();
      }
    }

    private bool TryReadEscapeChar(ref TextPosition end)
    {
      if ((int) this._c != 92)
        return false;
      end = this._position;
      this.NextChar();
      switch ((int) this._c)
      {
        case 9:
        case 10:
        case 13:
        case 32:
          bool flag = (int) this._c == 32;
          TextPosition position1 = this._position;
          while (CharHelper.IsWhiteSpaceOrNewLine(this._c))
          {
            end = this._position;
            this.NextChar();
          }
          if (flag && end.Line == position1.Line)
            this.AddError("Invalid escape `\\`. It must skip at least one line.", position1, position1);
          return true;
        case 34:
          this._textBuilder.Append('"');
          end = this._position;
          this.NextChar();
          return true;
        case 85:
        case 117:
          TextPosition position2 = this._position;
          end = this._position;
          int num1 = (int) this._c == 117 ? 4 : 8;
          this.NextChar();
          int num2 = 0;
          int num3 = 0;
          for (; CharHelper.IsHexFunc(this._c) && num2 < num1; ++num2)
          {
            num3 = (num3 << 4) + CharHelper.HexToDecimal(this._c);
            end = this._position;
            this.NextChar();
          }
          if (num2 == num1)
          {
            if (!CharHelper.IsValidUnicodeScalarValue((char32) num3))
              this.AddError(string.Format("Invalid Unicode scalar value [{0:X}]", (object) num3), position2, position2);
            this._textBuilder.AppendUtf32((char32) num3);
            return true;
          }
          break;
        case 92:
          this._textBuilder.Append('\\');
          end = this._position;
          this.NextChar();
          return true;
        case 98:
          this._textBuilder.Append('\b');
          end = this._position;
          this.NextChar();
          return true;
        case 102:
          this._textBuilder.Append('\f');
          end = this._position;
          this.NextChar();
          return true;
        case 110:
          this._textBuilder.Append('\n');
          end = this._position;
          this.NextChar();
          return true;
        case 114:
          this._textBuilder.Append('\r');
          end = this._position;
          this.NextChar();
          return true;
        case 116:
          this._textBuilder.Append('\t');
          end = this._position;
          this.NextChar();
          return true;
      }
      this.AddError(string.Format("Unexpected escape character [{0}] in string. Only b t n f r \\ \" u0000-uFFFF U00000000-UFFFFFFFF are allowed", (object) this._c), this._position, this._position);
      return false;
    }

    private void ReadStringLiteral(TextPosition start, bool allowMultiline)
    {
      TextPosition position1 = this._position;
      bool flag = false;
      this.NextChar();
      if (allowMultiline && (int) this._c == 39)
      {
        TextPosition position2 = this._position;
        this.NextChar();
        if ((int) this._c == 39)
        {
          position1 = this._position;
          this.NextChar();
          flag = true;
          this.SkipImmediateNextLine();
        }
        else
        {
          this._token = new SyntaxTokenValue(TokenKind.StringLiteral, start, position2, (object) string.Empty);
          return;
        }
      }
      this._textBuilder.Length = 0;
      while (true)
      {
        while ((int) this._c == 39 || (int) this._c == -1)
        {
          if (flag)
          {
            if ((int) this._c == 39)
            {
              int num = 0;
              while ((int) this._c == 39 && num < 5)
              {
                ++num;
                position1 = this._position;
                this.NextChar();
              }
              if (num >= 3)
              {
                for (int index = 0; index < num - 3; ++index)
                  this._textBuilder.Append('\'');
              }
              else
              {
                for (int index = 0; index < num; ++index)
                  this._textBuilder.Append('\'');
                continue;
              }
            }
            else
              this.AddError("Invalid End-Of-File found for multi-line literal string", position1, position1);
            this._token = new SyntaxTokenValue(TokenKind.StringLiteralMulti, start, position1, (object) this._textBuilder.ToString());
            return;
          }
          if ((int) this._c == 39)
          {
            position1 = this._position;
            this.NextChar();
          }
          else
            this.AddError("Invalid End-Of-File found on string literal", position1, position1);
          this._token = new SyntaxTokenValue(TokenKind.StringLiteral, start, position1, (object) this._textBuilder.ToString());
          return;
        }
        if (!flag && CharHelper.IsNewLine(this._c))
          this.AddError("Invalid newline in a string", this._position, this._position);
        else if (CharHelper.IsControlCharacter(this._c) && (!flag || !CharHelper.IsNewLine(this._c)))
          this.AddError("Invalid control character found " + ((char) (int) this._c).ToPrintableString(), start, start);
        this._textBuilder.AppendUtf32(this._c);
        position1 = this._position;
        this.NextChar();
      }
    }

    private void ReadComment(TextPosition start)
    {
      TextPosition end = start;
      while ((int) this._c != -1 && (int) this._c != 13 && (int) this._c != 10)
      {
        if ((int) this._c >= 0 && (int) this._c <= 8 || (int) this._c >= 10 && (int) this._c <= 31 || (int) this._c == (int) sbyte.MaxValue)
          this.AddError(string.Format("Invalid control character U+{0:X4} in comment", (object) this._c.Code), this._position, this._position);
        end = this._position;
        this.NextChar();
      }
      if ((int) this._c == 13 && (int) this.PeekChar() != 10)
        this.AddError(string.Format("Invalid control character U+{0:X4} in comment", (object) this._c.Code), this._position, this._position);
      this._token = new SyntaxTokenValue(TokenKind.Comment, start, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NextChar()
    {
      if (this._preview1.HasValue)
      {
        this._current = this._preview1.Value;
        this._preview1 = new LexerInternalState?();
      }
      else
      {
        this._current.Position = this._current.NextPosition;
        this._current.PreviousChar = this._current.CurrentChar;
        this._current.CurrentChar = this.NextCharFromReader();
      }
    }

    private char32 PeekChar()
    {
      if (!this._preview1.HasValue)
      {
        LexerInternalState current = this._current;
        this.NextChar();
        this._preview1 = new LexerInternalState?(this._current);
        this._current = current;
      }
      return this._preview1.Value.CurrentChar;
    }

    private char32 NextCharFromReader()
    {
      try
      {
        int offset = this._position.Offset;
        char32? next = this._reader.TryGetNext(ref offset);
        this._current.NextPosition.Offset = offset;
        if (next.HasValue)
        {
          char32 c = next.Value;
          if ((int) c == 10)
          {
            this._current.NextPosition.Column = 0;
            ++this._current.NextPosition.Line;
          }
          else
            ++this._current.NextPosition.Column;
          this.CheckCharacter(c);
          return c;
        }
      }
      catch (CharReaderException ex)
      {
        this.AddError(ex.Message, this._position, this._position);
      }
      return (char32) -1;
    }

    private void CheckCharacter(char32 c)
    {
      if (CharHelper.IsValidUnicodeScalarValue(c) && (int) c != 65533)
        return;
      this.AddError(string.Format("The character `{0}` is an invalid UTF8 character", (object) c), this._current.Position, this._current.Position);
    }

    private void AddError(string message, TextPosition start, TextPosition end)
    {
      if (this._errors == null)
        this._errors = new List<DiagnosticMessage>();
      this._errors.Add(new DiagnosticMessage(DiagnosticMessageKind.Error, new SourceSpan(this._sourceView.SourcePath, start, end), message));
    }

    private void Reset()
    {
      this._preview1 = new LexerInternalState?();
      this._current = new LexerInternalState()
      {
        Position = new TextPosition(this._reader.Start, 0, 0)
      };
      this._current.CurrentChar = this.NextCharFromReader();
      this._token = new SyntaxTokenValue();
      this._errors = (List<DiagnosticMessage>) null;
    }
  }
}
