// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CssScanner
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  internal class CssScanner
  {
    private const string c_scanIncludes = "~=";
    private const string c_dashMatch = "|=";
    private const string c_prefixMatch = "^=";
    private const string c_suffixMatch = "$=";
    private const string c_substringMatch = "*=";
    private const string c_commentStart = "<!--";
    private const string c_commentEnd = "-->";
    private TextReader m_reader;
    private string m_readAhead;
    private char m_currentChar;
    private string m_rawNumber;
    private CssContext m_context;
    private static Regex s_leadingZeros = new Regex("^0*([0-9]+?)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex s_trailingZeros = new Regex("^([0-9]+?)0*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex s_sourceDirective = new Regex("#SOURCE\\s+(?<line>\\d+)\\s+(?<col>\\d+)\\s+(?<path>.*)\\s*\\*/", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private bool m_isAtEOF;

    public string RawNumber => this.m_rawNumber;

    public bool AllowEmbeddedAspNetBlocks { get; set; }

    public bool GotEndOfLine { get; set; }

    public bool EndOfFile => this.m_isAtEOF;

    public CssScanner(TextReader reader)
    {
      this.m_context = new CssContext();
      this.m_reader = reader;
      this.NextChar();
    }

    public CssToken NextToken()
    {
      this.GotEndOfLine = false;
      this.m_context.Advance();
      this.m_rawNumber = (string) null;
      CssToken cssToken = (CssToken) null;
      bool flag;
      do
      {
        flag = false;
        switch (this.m_currentChar)
        {
          case char.MinValue:
            this.m_isAtEOF = true;
            break;
          case '\t':
          case ' ':
            while (CssScanner.IsSpace(this.m_currentChar))
            {
              if (this.m_currentChar == '\r' || this.m_currentChar == '\n' || this.m_currentChar == '\f')
                this.GotEndOfLine = true;
              this.NextChar();
            }
            cssToken = new CssToken(TokenType.Space, ' ', this.m_context);
            break;
          case '\n':
          case '\f':
          case '\r':
            this.GotEndOfLine = true;
            goto case '\t';
          case '!':
            cssToken = this.ScanImportant();
            break;
          case '"':
          case '\'':
            cssToken = this.ScanString();
            break;
          case '#':
            cssToken = this.ScanHash();
            break;
          case '$':
            cssToken = this.ScanSuffixMatch();
            break;
          case '*':
            cssToken = this.ScanSubstringMatch();
            break;
          case '-':
            cssToken = this.ScanCDC();
            if (cssToken == null)
            {
              string ident = this.GetIdent();
              if (ident != null)
              {
                if (this.m_currentChar == '(')
                {
                  this.NextChar();
                  cssToken = new CssToken(TokenType.Function, "-" + ident + (object) '(', this.m_context);
                  break;
                }
                cssToken = new CssToken(TokenType.Identifier, "-" + ident, this.m_context);
                break;
              }
              cssToken = new CssToken(TokenType.Character, '-', this.m_context);
              break;
            }
            break;
          case '.':
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            cssToken = this.ScanNum();
            break;
          case '/':
            cssToken = this.ScanComment();
            if (cssToken == null)
            {
              flag = true;
              break;
            }
            break;
          case '<':
            cssToken = !this.AllowEmbeddedAspNetBlocks || this.PeekChar() != '%' ? this.ScanCDO() : this.ScanAspNetBlock();
            break;
          case '@':
            cssToken = this.ScanAtKeyword();
            break;
          case 'U':
          case 'u':
            cssToken = this.ScanUrl();
            break;
          case '^':
            cssToken = this.ScanPrefixMatch();
            break;
          case '|':
            cssToken = this.ScanDashMatch();
            break;
          case '~':
            cssToken = this.ScanIncludes();
            break;
          default:
            cssToken = this.ScanIdent();
            break;
        }
      }
      while (flag);
      return cssToken;
    }

    public CssToken ScanReplacementToken()
    {
      CssToken cssToken = (CssToken) null;
      string replacementToken = this.GetReplacementToken(false);
      if (!replacementToken.IsNullOrWhiteSpace())
        cssToken = new CssToken(TokenType.ReplacementToken, replacementToken, this.m_context);
      return cssToken;
    }

    private CssToken ScanComment()
    {
      CssToken cssToken = (CssToken) null;
      this.NextChar();
      if (this.m_currentChar == '*')
      {
        this.NextChar();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("/*");
        bool flag = false;
        while (this.m_currentChar != char.MinValue)
        {
          stringBuilder.Append(this.m_currentChar);
          if (this.m_currentChar == '*' && this.PeekChar() == '/')
          {
            stringBuilder.Append('/');
            this.NextChar();
            this.NextChar();
            if (stringBuilder.ToString() == "/*!/*/" && this.ReadString("/*/"))
              stringBuilder.Append("/*/");
            flag = true;
            break;
          }
          this.NextChar();
        }
        if (!flag)
          this.ReportError(0, CssErrorCode.UnterminatedComment);
        string str = stringBuilder.ToString();
        if (string.Compare(str, 2, "/#SOURCE", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
        {
          Match match = CssScanner.s_sourceDirective.Match(str);
          int result1;
          int result2;
          if (match != null && int.TryParse(match.Result("${line}"), out result1) && int.TryParse(match.Result("${col}"), out result2))
          {
            this.OnContextChange(match.Result("${path}"), result1, result2);
            this.SkipToNextLineWithoutUpdate();
            return (CssToken) null;
          }
        }
        cssToken = new CssToken(TokenType.Comment, str, this.m_context);
      }
      else if (this.m_currentChar == '/')
      {
        if (this.PeekChar() == '/')
        {
          this.NextChar();
          if (this.PeekChar() == '#')
          {
            this.NextChar();
            if (this.ReadString("#SOURCE"))
            {
              this.DirectiveSkipSpace();
              int line = this.DirectiveScanInteger();
              if (line > 0)
              {
                this.DirectiveSkipSpace();
                int column = this.DirectiveScanInteger();
                if (column > 0)
                {
                  this.DirectiveSkipSpace();
                  StringBuilder stringBuilder = new StringBuilder();
                  while (this.m_currentChar != '\n' && this.m_currentChar != '\r')
                  {
                    stringBuilder.Append(this.m_currentChar);
                    int num = (int) this.DirectiveNextChar();
                  }
                  string fileContext = stringBuilder.ToString().TrimEnd();
                  if (!string.IsNullOrEmpty(fileContext))
                  {
                    this.OnContextChange(fileContext, line, column);
                    this.SkipToNextLineWithoutUpdate();
                    return (CssToken) null;
                  }
                }
              }
            }
          }
        }
        while (this.m_currentChar != '\n' && this.m_currentChar != '\r' && this.m_currentChar != char.MinValue)
          this.NextChar();
        return (CssToken) null;
      }
      if (cssToken == null)
        cssToken = new CssToken(TokenType.Character, '/', this.m_context);
      return cssToken;
    }

    private void SkipToNextLineWithoutUpdate()
    {
      while (this.m_currentChar != '\n' && this.m_currentChar != '\r')
      {
        int num1 = (int) this.DirectiveNextChar();
      }
      if (this.m_currentChar == '\n' || this.m_currentChar == '\f')
      {
        int num2 = (int) this.DirectiveNextChar();
      }
      else
      {
        if (this.m_currentChar != '\r' || this.DirectiveNextChar() != '\n')
          return;
        int num3 = (int) this.DirectiveNextChar();
      }
    }

    private CssToken ScanAspNetBlock()
    {
      StringBuilder stringBuilder = new StringBuilder();
      char ch = ' ';
      while (this.m_currentChar != char.MinValue && (this.m_currentChar != '>' || ch != '%'))
      {
        stringBuilder.Append(this.m_currentChar);
        ch = this.m_currentChar;
        this.NextChar();
      }
      if (this.m_currentChar != char.MinValue)
      {
        stringBuilder.Append(this.m_currentChar);
        this.NextChar();
      }
      return new CssToken(TokenType.AspNetBlock, stringBuilder.ToString(), this.m_context);
    }

    private CssToken ScanCDO()
    {
      CssToken cssToken1 = (CssToken) null;
      this.NextChar();
      if (this.m_currentChar == '!' && this.PeekChar() == '-')
      {
        this.NextChar();
        if (this.PeekChar() == '-')
        {
          this.NextChar();
          this.NextChar();
          cssToken1 = new CssToken(TokenType.CommentOpen, "<!--", this.m_context);
        }
        else
          this.PushChar('!');
      }
      CssToken cssToken2;
      return cssToken1 ?? (cssToken2 = new CssToken(TokenType.Character, '<', this.m_context));
    }

    private CssToken ScanCDC()
    {
      CssToken cssToken = (CssToken) null;
      this.NextChar();
      if (this.m_currentChar == '-' && this.PeekChar() == '>')
      {
        this.NextChar();
        this.NextChar();
        cssToken = new CssToken(TokenType.CommentClose, "-->", this.m_context);
      }
      return cssToken;
    }

    private CssToken ScanIncludes()
    {
      CssToken cssToken = (CssToken) null;
      this.NextChar();
      if (this.m_currentChar == '=')
      {
        this.NextChar();
        cssToken = new CssToken(TokenType.Includes, "~=", this.m_context);
      }
      return cssToken ?? new CssToken(TokenType.Character, '~', this.m_context);
    }

    private CssToken ScanDashMatch()
    {
      CssToken cssToken;
      if (this.PeekChar() == '=')
      {
        this.NextChar();
        this.NextChar();
        cssToken = new CssToken(TokenType.DashMatch, "|=", this.m_context);
      }
      else
        cssToken = this.ScanIdent();
      if (cssToken == null)
      {
        this.NextChar();
        cssToken = new CssToken(TokenType.Character, '|', this.m_context);
      }
      return cssToken;
    }

    private CssToken ScanPrefixMatch()
    {
      CssToken cssToken = (CssToken) null;
      this.NextChar();
      if (this.m_currentChar == '=')
      {
        this.NextChar();
        cssToken = new CssToken(TokenType.PrefixMatch, "^=", this.m_context);
      }
      return cssToken ?? new CssToken(TokenType.Character, '^', this.m_context);
    }

    private CssToken ScanSuffixMatch()
    {
      CssToken cssToken = (CssToken) null;
      this.NextChar();
      if (this.m_currentChar == '=')
      {
        this.NextChar();
        cssToken = new CssToken(TokenType.SuffixMatch, "$=", this.m_context);
      }
      return cssToken ?? new CssToken(TokenType.Character, '$', this.m_context);
    }

    private CssToken ScanSubstringMatch()
    {
      CssToken cssToken;
      if (this.PeekChar() == '=')
      {
        this.NextChar();
        this.NextChar();
        cssToken = new CssToken(TokenType.SubstringMatch, "*=", this.m_context);
      }
      else
        cssToken = this.ScanIdent();
      if (cssToken == null)
      {
        this.NextChar();
        cssToken = new CssToken(TokenType.Character, '*', this.m_context);
      }
      return cssToken;
    }

    private CssToken ScanString()
    {
      string text = this.GetString();
      return new CssToken(text.Length >= 2 && (int) text[0] == (int) text[text.Length - 1] ? TokenType.String : TokenType.Error, text, this.m_context);
    }

    private CssToken ScanHash()
    {
      this.NextChar();
      string str = this.m_currentChar == '%' ? this.GetReplacementToken(true) : this.GetName();
      return str != null ? new CssToken(TokenType.Hash, '#'.ToString() + str, this.m_context) : new CssToken(TokenType.Character, '#', this.m_context);
    }

    private CssToken ScanAtKeyword()
    {
      this.NextChar();
      TokenType tokenType = TokenType.Character;
      bool flag = this.m_currentChar == '-';
      if (flag)
        this.NextChar();
      string str = this.GetIdent();
      if (str != null)
      {
        if (flag)
          str = '-'.ToString() + str;
        switch (str.ToUpperInvariant())
        {
          case "IMPORT":
            tokenType = TokenType.ImportSymbol;
            break;
          case "PAGE":
            tokenType = TokenType.PageSymbol;
            break;
          case "MEDIA":
            tokenType = TokenType.MediaSymbol;
            break;
          case "FONT-FACE":
            tokenType = TokenType.FontFaceSymbol;
            break;
          case "CHARSET":
            tokenType = TokenType.CharacterSetSymbol;
            break;
          case "NAMESPACE":
            tokenType = TokenType.NamespaceSymbol;
            break;
          case "TOP-LEFT-CORNER":
            tokenType = TokenType.TopLeftCornerSymbol;
            break;
          case "TOP-LEFT":
            tokenType = TokenType.TopLeftSymbol;
            break;
          case "TOP-CENTER":
            tokenType = TokenType.TopCenterSymbol;
            break;
          case "TOP-RIGHT":
            tokenType = TokenType.TopRightSymbol;
            break;
          case "TOP-RIGHT-CORNER":
            tokenType = TokenType.TopRightCornerSymbol;
            break;
          case "BOTTOM-LEFT-CORNER":
            tokenType = TokenType.BottomLeftCornerSymbol;
            break;
          case "BOTTOM-LEFT":
            tokenType = TokenType.BottomLeftSymbol;
            break;
          case "BOTTOM-CENTER":
            tokenType = TokenType.BottomCenterSymbol;
            break;
          case "BOTTOM-RIGHT":
            tokenType = TokenType.BottomRightSymbol;
            break;
          case "BOTTOM-RIGHT-CORNER":
            tokenType = TokenType.BottomRightCornerSymbol;
            break;
          case "LEFT-TOP":
            tokenType = TokenType.LeftTopSymbol;
            break;
          case "LEFT-MIDDLE":
            tokenType = TokenType.LeftMiddleSymbol;
            break;
          case "LEFT-BOTTOM":
            tokenType = TokenType.LeftBottomSymbol;
            break;
          case "RIGHT-TOP":
            tokenType = TokenType.RightTopSymbol;
            break;
          case "RIGHT-MIDDLE":
            tokenType = TokenType.RightMiddleSymbol;
            break;
          case "RIGHT-BOTTOM":
            tokenType = TokenType.RightBottomSymbol;
            break;
          case "KEYFRAMES":
          case "-MS-KEYFRAMES":
          case "-MOZ-KEYFRAMES":
          case "-WEBKIT-KEYFRAMES":
            tokenType = TokenType.KeyFramesSymbol;
            break;
          default:
            tokenType = TokenType.AtKeyword;
            break;
        }
      }
      else if (flag)
        this.PushChar('-');
      return new CssToken(tokenType, '@'.ToString() + (str == null ? (object) string.Empty : (object) str), this.m_context);
    }

    private CssToken ScanImportant()
    {
      CssToken cssToken = (CssToken) null;
      this.NextChar();
      string w = this.GetW();
      if (char.ToUpperInvariant(this.m_currentChar) == 'I' && this.ReadString("IMPORTANT"))
        cssToken = new CssToken(TokenType.ImportantSymbol, "!important", this.m_context);
      if (cssToken == null && w.Length > 0)
        this.PushChar(' ');
      return cssToken ?? new CssToken(TokenType.Character, '!', this.m_context);
    }

    private CssToken ScanUnicodeRange()
    {
      CssToken cssToken = (CssToken) null;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("U+");
      bool flag1 = false;
      int num1 = 0;
      bool flag2 = true;
      int num2 = 0;
      while (this.m_currentChar != char.MinValue && num1 < 6 && (this.m_currentChar == '?' || !flag1 && CssScanner.IsH(this.m_currentChar)))
      {
        if (flag2 && this.m_currentChar != '0')
          flag2 = false;
        if (this.m_currentChar == '?')
        {
          flag1 = true;
          num2 = num2 * 16 + CssScanner.HValue('F');
        }
        else
          num2 = num2 * 16 + CssScanner.HValue(this.m_currentChar);
        if (!flag2)
          stringBuilder.Append(this.m_currentChar);
        ++num1;
        this.NextChar();
      }
      if (num1 > 0)
      {
        if (num2 < 0 || 1114111 < num2)
          this.ReportError(0, CssErrorCode.InvalidUnicodeRange, (object) stringBuilder.ToString());
        if (flag2)
          stringBuilder.Append('0');
        if (flag1)
          cssToken = new CssToken(TokenType.UnicodeRange, stringBuilder.ToString(), this.m_context);
        else if (this.m_currentChar == '-')
        {
          stringBuilder.Append('-');
          this.NextChar();
          int num3 = 0;
          bool flag3 = true;
          int num4 = 0;
          while (this.m_currentChar != char.MinValue && num3 < 6 && CssScanner.IsH(this.m_currentChar))
          {
            if (flag3 && this.m_currentChar != '0')
              flag3 = false;
            num4 = num4 * 16 + CssScanner.HValue(this.m_currentChar);
            if (!flag3)
              stringBuilder.Append(this.m_currentChar);
            ++num3;
            this.NextChar();
          }
          if (num3 > 0)
          {
            if (flag3)
              stringBuilder.Append('0');
            if (num4 < 0 || 1114111 < num4 || num2 >= num4)
              this.ReportError(0, CssErrorCode.InvalidUnicodeRange, (object) stringBuilder.ToString());
            cssToken = new CssToken(TokenType.UnicodeRange, stringBuilder.ToString(), this.m_context);
          }
        }
        else
          cssToken = new CssToken(TokenType.UnicodeRange, stringBuilder.ToString(), this.m_context);
      }
      if (cssToken == null)
      {
        this.PushString(stringBuilder.ToString());
        cssToken = this.ScanIdent();
      }
      return cssToken;
    }

    private CssToken ScanUrl()
    {
      CssToken cssToken = (CssToken) null;
      if (this.PeekChar() == '+')
      {
        this.NextChar();
        this.NextChar();
        cssToken = this.ScanUnicodeRange();
      }
      else if (this.ReadString("URL("))
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("url(");
        this.GetW();
        string str = this.GetString() ?? this.GetUrl();
        if (str != null)
        {
          stringBuilder.Append(str);
          this.GetW();
          if (this.m_currentChar == ')')
          {
            stringBuilder.Append(')');
            this.NextChar();
            cssToken = new CssToken(TokenType.Uri, stringBuilder.ToString(), this.m_context);
          }
        }
      }
      return cssToken ?? this.ScanIdent();
    }

    private CssToken ScanNum()
    {
      CssToken cssToken = (CssToken) null;
      string num = this.GetNum();
      if (num != null)
      {
        if (this.m_currentChar == '%')
        {
          this.NextChar();
          cssToken = new CssToken(TokenType.Percentage, num + (object) '%', this.m_context);
          this.m_rawNumber += (string) (object) '%';
        }
        else
        {
          string ident = this.GetIdent();
          if (ident == null)
          {
            cssToken = new CssToken(TokenType.Number, num, this.m_context);
          }
          else
          {
            this.m_rawNumber += ident;
            TokenType tokenType = TokenType.Dimension;
            switch (ident.ToUpperInvariant())
            {
              case "EM":
              case "EX":
              case "CH":
              case "REM":
              case "VW":
              case "VH":
              case "VM":
              case "VMIN":
              case "VMAX":
              case "FR":
              case "GR":
              case "GD":
                tokenType = TokenType.RelativeLength;
                break;
              case "CM":
              case "MM":
              case "IN":
              case "PX":
              case "PT":
              case "PC":
                tokenType = TokenType.AbsoluteLength;
                break;
              case "DEG":
              case "GRAD":
              case "RAD":
              case "TURN":
                tokenType = TokenType.Angle;
                break;
              case "MS":
              case "S":
                tokenType = TokenType.Time;
                break;
              case "DPI":
              case "DPCM":
              case "DPPX":
                tokenType = TokenType.Resolution;
                break;
              case "HZ":
              case "KHZ":
                tokenType = TokenType.Frequency;
                break;
              case "DB":
              case "ST":
                tokenType = TokenType.Speech;
                break;
            }
            cssToken = !(num == "0") || tokenType == TokenType.Dimension || tokenType == TokenType.Angle || tokenType == TokenType.Time || tokenType == TokenType.Frequency || tokenType == TokenType.Resolution ? new CssToken(tokenType, num + ident, this.m_context) : new CssToken(TokenType.Number, num, this.m_context);
          }
        }
      }
      else if (this.m_currentChar == '.')
      {
        cssToken = new CssToken(TokenType.Character, '.', this.m_context);
        this.NextChar();
      }
      else
        this.ReportError(1, CssErrorCode.UnexpectedNumberCharacter, (object) this.m_currentChar);
      return cssToken;
    }

    private CssToken ScanIdent()
    {
      CssToken cssToken = (CssToken) null;
      string ident = this.GetIdent();
      if (ident != null)
      {
        if (this.m_currentChar == '(')
        {
          this.NextChar();
          cssToken = string.Compare(ident, "not", StringComparison.OrdinalIgnoreCase) != 0 ? new CssToken(TokenType.Function, ident + (object) '(', this.m_context) : new CssToken(TokenType.Not, ident + (object) '(', this.m_context);
        }
        else if (string.Compare(ident, "progid", StringComparison.OrdinalIgnoreCase) == 0 && this.m_currentChar == ':')
        {
          this.NextChar();
          cssToken = this.ScanProgId();
        }
        else
          cssToken = new CssToken(TokenType.Identifier, ident, this.m_context);
      }
      if (ident == null && this.m_currentChar != char.MinValue)
      {
        cssToken = new CssToken(TokenType.Character, this.m_currentChar, this.m_context);
        this.NextChar();
      }
      return cssToken;
    }

    private CssToken ScanProgId()
    {
      CssToken cssToken = (CssToken) null;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("progid:");
      for (string ident = this.GetIdent(); ident != null; ident = this.GetIdent())
      {
        stringBuilder.Append(ident);
        if (this.m_currentChar == '.')
        {
          stringBuilder.Append('.');
          this.NextChar();
        }
      }
      if (this.m_currentChar == '(')
      {
        stringBuilder.Append('(');
        this.NextChar();
        cssToken = new CssToken(TokenType.ProgId, stringBuilder.ToString(), this.m_context);
      }
      else
        this.ReportError(1, CssErrorCode.ExpectedOpenParenthesis);
      return cssToken;
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

    private static int HValue(char ch)
    {
      if ('0' <= ch && ch <= '9')
        return (int) ch - 48;
      if ('a' <= ch && ch <= 'f')
        return (int) ch - 97 + 10;
      return 'A' <= ch && ch <= 'F' ? (int) ch - 65 + 10 : 0;
    }

    public static bool IsH(char ch)
    {
      if ('0' <= ch && ch <= '9' || 'a' <= ch && ch <= 'f')
        return true;
      return 'A' <= ch && ch <= 'F';
    }

    private static bool IsD(char ch) => '0' <= ch && ch <= '9';

    private static bool IsNonAscii(char ch) => '\u0080' <= ch && ch <= char.MaxValue;

    internal static bool IsNmChar(char ch)
    {
      if (CssScanner.IsNonAscii(ch) || ch == '-' || ch == '_' || '0' <= ch && ch <= '9' || 'a' <= ch && ch <= 'z')
        return true;
      return 'A' <= ch && ch <= 'Z';
    }

    private string GetReplacementToken(bool advancePastDelimiter)
    {
      bool flag = false;
      char currentChar = this.m_currentChar;
      StringBuilder stringBuilder = new StringBuilder();
      if (advancePastDelimiter)
        this.NextChar();
      string name = this.GetName();
      while (name != null)
      {
        stringBuilder.Append(name);
        if (this.m_currentChar == '.')
        {
          stringBuilder.Append('.');
          this.NextChar();
          name = this.GetName();
          if (name != null)
            continue;
        }
        else if (this.m_currentChar == ':')
        {
          this.NextChar();
          stringBuilder.Append(':');
          stringBuilder.Append(this.GetName());
          if (this.m_currentChar == '%')
          {
            this.NextChar();
            stringBuilder.Append('%');
            flag = true;
            break;
          }
        }
        else if (this.m_currentChar == '%')
        {
          this.NextChar();
          stringBuilder.Append('%');
          flag = true;
          break;
        }
        this.PushString(stringBuilder.ToString());
        break;
      }
      if (!flag)
        this.m_currentChar = currentChar;
      return !flag ? (string) null : '%'.ToString() + stringBuilder.ToString();
    }

    private int GetUnicodeEncodingValue(out bool follwedByWhitespace)
    {
      int unicodeEncodingValue = 0;
      int num = 0;
      while (this.m_currentChar != char.MinValue && num++ < 6 && CssScanner.IsH(this.m_currentChar))
      {
        unicodeEncodingValue = unicodeEncodingValue * 16 + CssScanner.HValue(this.m_currentChar);
        this.NextChar();
      }
      follwedByWhitespace = CssScanner.IsSpace(this.m_currentChar);
      if (follwedByWhitespace)
        this.NextChar();
      return unicodeEncodingValue;
    }

    private string GetUnicode()
    {
      string unicode = (string) null;
      if (this.m_currentChar == '\\' && CssScanner.IsH(this.PeekChar()))
      {
        this.NextChar();
        bool follwedByWhitespace;
        int unicodeValue = this.GetUnicodeEncodingValue(out follwedByWhitespace);
        switch (unicodeValue)
        {
          case 32:
          case 92:
            unicode = (follwedByWhitespace ? "\\{0:x} " : "\\{0:x}").FormatInvariant((object) unicodeValue);
            break;
          default:
            if (55296 <= unicodeValue && unicodeValue <= 56319)
            {
              int num = unicodeValue;
              if (this.m_currentChar == '\\' && CssScanner.IsH(this.PeekChar()))
              {
                this.NextChar();
                int unicodeEncodingValue = this.GetUnicodeEncodingValue(out follwedByWhitespace);
                if (56320 <= unicodeEncodingValue && unicodeEncodingValue <= 57343)
                  unicodeValue = 65536 + (num - 55296) * 1024 + (unicodeEncodingValue - 56320);
                else
                  this.ReportError(0, CssErrorCode.InvalidLowSurrogate, (object) num, (object) unicodeEncodingValue);
              }
              else
                this.ReportError(0, CssErrorCode.HighSurrogateNoLow, (object) unicodeValue);
            }
            unicode = CssScanner.ConvertUtf32ToUtf16(unicodeValue);
            break;
        }
      }
      return unicode;
    }

    private static string ConvertUtf32ToUtf16(int unicodeValue) => char.ConvertFromUtf32(unicodeValue);

    private string GetEscape()
    {
      string unicode = this.GetUnicode();
      if (unicode == null && this.m_currentChar == '\\')
      {
        char ch = this.PeekChar();
        if (' ' <= ch && ch <= '~' || CssScanner.IsNonAscii(ch))
        {
          this.NextChar();
          this.NextChar();
          return "\\" + (object) ch;
        }
      }
      return unicode;
    }

    private string GetNmStart()
    {
      string escape = this.GetEscape();
      if (escape == null && (CssScanner.IsNonAscii(this.m_currentChar) || this.m_currentChar == '_' || 'a' <= this.m_currentChar && this.m_currentChar <= 'z' || 'A' <= this.m_currentChar && this.m_currentChar <= 'Z'))
      {
        if (this.m_currentChar == '_')
          this.ReportError(4, CssErrorCode.UnderscoreNotValid);
        escape = char.ToString(this.m_currentChar);
        this.NextChar();
      }
      return escape;
    }

    private string GetNmChar()
    {
      string escape = this.GetEscape();
      if (escape == null && CssScanner.IsNmChar(this.m_currentChar))
      {
        if (this.m_currentChar == '_')
          this.ReportError(4, CssErrorCode.UnderscoreNotValid);
        escape = char.ToString(this.m_currentChar);
        this.NextChar();
      }
      return escape;
    }

    private string GetString()
    {
      string str1 = (string) null;
      if (this.m_currentChar == '\'' || this.m_currentChar == '"')
      {
        char currentChar1 = this.m_currentChar;
        this.NextChar();
        StringBuilder sb = new StringBuilder();
        sb.Append(currentChar1);
        while (this.m_currentChar != char.MinValue && (int) this.m_currentChar != (int) currentChar1)
        {
          string str2 = this.GetEscape();
          if (str2 != null)
          {
            if (str2.Length == 1 && (int) str2[0] == (int) currentChar1)
              str2 = "\\" + (object) currentChar1;
            sb.Append(str2);
          }
          else if (CssScanner.IsNonAscii(this.m_currentChar))
          {
            sb.Append(this.m_currentChar);
            this.NextChar();
          }
          else if (this.m_currentChar == '\\')
          {
            this.NextChar();
            if (this.GetNewline() == null)
              this.ReportError(0, CssErrorCode.UnexpectedEscape, (object) this.m_currentChar);
          }
          else if (this.m_currentChar == ' ' || this.m_currentChar == '\t' || this.m_currentChar == '!' || this.m_currentChar == '#' || this.m_currentChar == '$' || this.m_currentChar == '%' || this.m_currentChar == '&' || '(' <= this.m_currentChar && this.m_currentChar <= '~' || (int) this.m_currentChar == (currentChar1 == '"' ? 39 : 34))
          {
            char currentChar2 = this.m_currentChar;
            sb.Append(this.m_currentChar);
            this.NextChar();
            if (this.AllowEmbeddedAspNetBlocks && currentChar2 == '<' && this.m_currentChar == '%')
              this.SkipAspNetBlock(sb);
          }
          else
          {
            if (this.m_currentChar == '\n' || this.m_currentChar == '\r')
            {
              this.GotEndOfLine = true;
              this.ReportError(0, CssErrorCode.UnterminatedString, (object) sb.ToString());
              sb.AppendLine();
              while (CssScanner.IsSpace(this.m_currentChar))
                this.NextChar();
              return sb.ToString();
            }
            this.ReportError(0, CssErrorCode.UnexpectedStringCharacter, (object) this.m_currentChar);
          }
        }
        if ((int) this.m_currentChar == (int) currentChar1)
        {
          sb.Append(currentChar1);
          this.NextChar();
        }
        str1 = sb.ToString();
      }
      return str1;
    }

    private void SkipAspNetBlock(StringBuilder sb)
    {
      sb.Append(this.m_currentChar);
      this.NextChar();
      bool flag = false;
      while (this.m_currentChar != char.MinValue)
      {
        if (this.m_currentChar == '%')
        {
          flag = true;
        }
        else
        {
          if (flag && this.m_currentChar == '>')
          {
            sb.Append(this.m_currentChar);
            this.NextChar();
            break;
          }
          flag = false;
        }
        sb.Append(this.m_currentChar);
        this.NextChar();
      }
    }

    private string GetIdent()
    {
      string nmStart = this.GetNmStart();
      if (nmStart != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(nmStart);
        string nmChar;
        while (this.m_currentChar != char.MinValue && (nmChar = this.GetNmChar()) != null)
          stringBuilder.Append(nmChar);
        nmStart = stringBuilder.ToString();
      }
      return nmStart;
    }

    private string GetName()
    {
      string nmChar1 = this.GetNmChar();
      if (nmChar1 != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(nmChar1);
        string nmChar2;
        while (this.m_currentChar != char.MinValue && (nmChar2 = this.GetNmChar()) != null)
          stringBuilder.Append(nmChar2);
        nmChar1 = stringBuilder.ToString();
      }
      return nmChar1;
    }

    private string GetNum()
    {
      string num = (string) null;
      string input1 = (string) null;
      string input2 = (string) null;
      bool flag = false;
      if (CssScanner.IsD(this.m_currentChar))
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.m_currentChar);
        this.NextChar();
        while (CssScanner.IsD(this.m_currentChar))
        {
          stringBuilder.Append(this.m_currentChar);
          this.NextChar();
        }
        input1 = stringBuilder.ToString();
      }
      if (this.m_currentChar == '.')
      {
        if (CssScanner.IsD(this.PeekChar()))
        {
          flag = true;
          this.NextChar();
          StringBuilder stringBuilder = new StringBuilder();
          while (CssScanner.IsD(this.m_currentChar))
          {
            stringBuilder.Append(this.m_currentChar);
            this.NextChar();
          }
          input2 = stringBuilder.ToString();
        }
        else if (input1 != null)
        {
          flag = true;
          this.ReportError(2, CssErrorCode.DecimalNoDigit);
          input2 = string.Empty;
          this.NextChar();
        }
      }
      if (input1 != null || input2 != null)
      {
        this.m_rawNumber = input1 ?? (flag ? "." : "") + input2 ?? "";
        if (input1 != null)
          input1 = CssScanner.s_leadingZeros.Replace(input1, "$1");
        if (input2 != null)
        {
          input2 = CssScanner.s_trailingZeros.Replace(input2, "$1");
          if (input2 == "0" || input2.Length == 0)
            input2 = (string) null;
        }
        if (input2 != null && input1 == "0")
          input1 = (string) null;
        num = input2 != null ? input1 + (object) '.' + input2 : input1 ?? "0";
      }
      return num;
    }

    private string GetUrl()
    {
      StringBuilder stringBuilder = new StringBuilder();
      while (this.m_currentChar != char.MinValue)
      {
        string escape = this.GetEscape();
        if (escape != null)
          stringBuilder.Append(escape);
        else if (CssScanner.IsNonAscii(this.m_currentChar) || this.m_currentChar == '!' || this.m_currentChar == '#' || this.m_currentChar == '$' || this.m_currentChar == '%' || this.m_currentChar == '&' || '*' <= this.m_currentChar && this.m_currentChar <= '~')
        {
          stringBuilder.Append(this.m_currentChar);
          this.NextChar();
        }
        else
          break;
      }
      return stringBuilder.ToString();
    }

    private string GetW()
    {
      string w = string.Empty;
      if (CssScanner.IsSpace(this.m_currentChar))
      {
        w = " ";
        this.NextChar();
        while (CssScanner.IsSpace(this.m_currentChar))
          this.NextChar();
      }
      return w;
    }

    private string GetNewline()
    {
      string newline = (string) null;
      switch (this.m_currentChar)
      {
        case '\n':
          this.NextChar();
          newline = "\n";
          break;
        case '\f':
          this.NextChar();
          newline = "\f";
          break;
        case '\r':
          this.NextChar();
          if (this.m_currentChar == '\n')
          {
            this.NextChar();
            newline = "\r\n";
            break;
          }
          newline = "\r";
          break;
      }
      return newline;
    }

    private void NextChar()
    {
      if (this.m_readAhead != null)
      {
        this.m_currentChar = this.m_readAhead[0];
        this.m_readAhead = this.m_readAhead.Length != 1 ? this.m_readAhead.Substring(1) : (string) null;
        this.m_context.End.NextChar();
      }
      else
      {
        int num = this.m_reader.Read();
        if (num < 0)
        {
          this.m_currentChar = char.MinValue;
        }
        else
        {
          this.m_currentChar = (char) num;
          switch (this.m_currentChar)
          {
            case '\n':
            case '\f':
              this.m_context.End.NextLine();
              break;
            case '\r':
              if (this.PeekChar() == '\n')
                break;
              this.m_context.End.NextLine();
              break;
            default:
              this.m_context.End.NextChar();
              break;
          }
        }
      }
    }

    public char PeekChar()
    {
      if (this.m_readAhead != null)
        return this.m_readAhead[0];
      int num = this.m_reader.Peek();
      return num < 0 ? char.MinValue : (char) num;
    }

    private bool ReadString(string str)
    {
      if ((int) char.ToUpperInvariant(this.m_currentChar) != (int) char.ToUpperInvariant(str[0]))
        return false;
      StringBuilder stringBuilder = (StringBuilder) null;
      for (int index = 1; index < str.Length; ++index)
      {
        if ((int) char.ToUpperInvariant(this.PeekChar()) != (int) char.ToUpperInvariant(str[index]))
        {
          if (index > 1 && stringBuilder != null)
            this.PushString(stringBuilder.ToString());
          return false;
        }
        if (stringBuilder == null)
          stringBuilder = new StringBuilder(str.Length);
        stringBuilder.Append(this.m_currentChar);
        this.NextChar();
      }
      this.NextChar();
      return true;
    }

    private void PushChar(char ch)
    {
      if (this.m_readAhead == null)
      {
        this.m_readAhead = char.ToString(this.m_currentChar);
        this.m_currentChar = ch;
      }
      else
      {
        this.m_readAhead = this.m_currentChar.ToString() + this.m_readAhead;
        this.m_currentChar = ch;
      }
      this.m_context.End.PreviousChar();
    }

    private void PushString(string str)
    {
      if (str.Length > 0)
      {
        this.m_readAhead = str.Length <= 1 ? this.m_currentChar.ToString() + this.m_readAhead : str.Substring(1) + (object) this.m_currentChar + this.m_readAhead;
        this.m_currentChar = str[0];
      }
      for (int index = 0; index < str.Length; ++index)
        this.m_context.End.PreviousChar();
    }

    private char DirectiveNextChar()
    {
      int num = this.m_reader.Read();
      this.m_currentChar = num < 0 ? char.MinValue : (char) num;
      return this.m_currentChar;
    }

    private void DirectiveSkipSpace()
    {
      while (this.m_currentChar == ' ' || this.m_currentChar == '\t')
        this.NextChar();
    }

    private int DirectiveScanInteger()
    {
      int num = 0;
      while ('0' <= this.m_currentChar && this.m_currentChar <= '9')
      {
        num = num * 10 + ((int) this.m_currentChar - 48);
        this.NextChar();
      }
      return num;
    }

    private void ReportError(int severity, CssErrorCode error, params object[] args)
    {
      string str = CssStrings.ResourceManager.GetString(error.ToString(), CssStrings.Culture).FormatInvariant(args);
      this.OnScannerError(new ContextError()
      {
        IsError = severity < 2,
        Severity = severity,
        Subcategory = CssStrings.ScannerSubsystem,
        File = "",
        ErrorNumber = (int) error,
        ErrorCode = "CSS{0}".FormatInvariant((object) (int) (error & (CssErrorCode) 65535)),
        StartLine = this.m_context.End.Line,
        StartColumn = this.m_context.End.Char,
        Message = str
      });
    }

    public event EventHandler<ContextErrorEventArgs> ScannerError;

    protected void OnScannerError(ContextError error)
    {
      if (this.ScannerError == null)
        return;
      this.ScannerError((object) this, new ContextErrorEventArgs()
      {
        Error = error
      });
    }

    public event EventHandler<CssScannerContextChangeEventArgs> ContextChange;

    protected void OnContextChange(string fileContext, int line, int column)
    {
      this.m_context.Reset(line, column);
      if (this.ContextChange == null)
        return;
      this.ContextChange((object) this, new CssScannerContextChangeEventArgs(fileContext));
    }
  }
}
