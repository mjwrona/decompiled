// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSScanner
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public sealed class JSScanner
  {
    private static readonly JSKeyword[] s_Keywords = JSKeyword.InitKeywords();
    private static readonly OperatorPrecedence[] s_OperatorsPrec = JSScanner.InitOperatorsPrec();
    private string m_strSourceCode;
    private int m_endPos;
    private StringBuilder m_identifier;
    private bool m_literalIssues;
    private Dictionary<string, string> m_defines;
    private int m_startLinePosition;
    private int m_currentPosition;
    private int m_currentLine;
    private int m_lastPosOnBuilder;
    private int m_ifDirectiveLevel;
    private int m_conditionalCompilationIfLevel;
    private bool m_conditionalCompilationOn;
    private bool m_inConditionalComment;
    private bool m_inSingleLineComment;
    private bool m_inMultipleLineComment;
    private bool m_mightBeKeyword;
    private string m_decodedString;
    private Context m_currentToken;

    internal ICollection<string> DebugLookupCollection { get; set; }

    public bool UsePreprocessorDefines { get; set; }

    public bool IgnoreConditionalCompilation { get; set; }

    public bool AllowEmbeddedAspNetBlocks { get; set; }

    public bool StripDebugCommentBlocks { get; set; }

    public bool SuppressErrors { get; set; }

    public int CurrentLine => this.m_currentLine;

    public bool IsEndOfFile => this.m_currentPosition >= this.m_endPos;

    public int StartLinePosition => this.m_startLinePosition;

    public bool LiteralHasIssues => this.m_literalIssues;

    public string StringLiteralValue => this.m_decodedString;

    public string Identifier => this.m_identifier.Length <= 0 ? this.m_currentToken.Code : this.m_identifier.ToString();

    public Context CurrentToken => this.m_currentToken;

    private bool IsAtEndOfLine => this.IsEndLineOrEOF(this.GetChar(this.m_currentPosition), 0);

    public event EventHandler<GlobalDefineEventArgs> GlobalDefine;

    public event EventHandler<NewModuleEventArgs> NewModule;

    public JSScanner(DocumentContext sourceContext)
    {
      this.m_currentToken = sourceContext != null ? new Context(sourceContext)
      {
        EndPosition = 0
      } : throw new ArgumentNullException(nameof (sourceContext));
      this.m_currentLine = 1;
      this.m_strSourceCode = sourceContext.Source;
      this.m_endPos = sourceContext.Source.Length;
      this.UsePreprocessorDefines = true;
      this.StripDebugCommentBlocks = true;
      this.m_identifier = new StringBuilder(128);
    }

    private JSScanner(IDictionary<string, string> defines)
    {
      this.SetPreprocessorDefines(defines);
      this.m_decodedString = (string) null;
      this.m_identifier = new StringBuilder(128);
      this.DebugLookupCollection = (ICollection<string>) new HashSet<string>();
    }

    public JSScanner Clone() => new JSScanner((IDictionary<string, string>) this.m_defines)
    {
      AllowEmbeddedAspNetBlocks = this.AllowEmbeddedAspNetBlocks,
      IgnoreConditionalCompilation = this.IgnoreConditionalCompilation,
      m_conditionalCompilationIfLevel = this.m_conditionalCompilationIfLevel,
      m_conditionalCompilationOn = this.m_conditionalCompilationOn,
      m_currentLine = this.m_currentLine,
      m_currentPosition = this.m_currentPosition,
      m_currentToken = this.m_currentToken.Clone(),
      m_endPos = this.m_endPos,
      m_ifDirectiveLevel = this.m_ifDirectiveLevel,
      m_inConditionalComment = this.m_inConditionalComment,
      m_inMultipleLineComment = this.m_inMultipleLineComment,
      m_inSingleLineComment = this.m_inSingleLineComment,
      m_lastPosOnBuilder = this.m_lastPosOnBuilder,
      m_startLinePosition = this.m_startLinePosition,
      m_strSourceCode = this.m_strSourceCode,
      UsePreprocessorDefines = this.UsePreprocessorDefines,
      StripDebugCommentBlocks = this.StripDebugCommentBlocks
    };

    public void SetPreprocessorDefines(IDictionary<string, string> defines)
    {
      if (defines != null && defines.Count > 0)
      {
        this.m_defines = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string> define in (IEnumerable<KeyValuePair<string, string>>) defines)
        {
          if (JSScanner.IsValidIdentifier(define.Key) && !this.m_defines.ContainsKey(define.Key))
            this.m_defines.Add(define.Key, define.Value);
        }
      }
      else
        this.m_defines = (Dictionary<string, string>) null;
    }

    public Context ScanNextToken()
    {
      this.m_currentToken.StartPosition = this.m_currentPosition;
      this.m_currentToken.StartLineNumber = this.m_currentLine;
      this.m_currentToken.StartLinePosition = this.m_startLinePosition;
      this.m_identifier.Length = 0;
      this.m_mightBeKeyword = false;
      char ch1 = this.GetChar(this.m_currentPosition);
      JSToken jsToken;
      switch (ch1)
      {
        case '\t':
        case '\v':
        case '\f':
        case ' ':
          jsToken = JSToken.WhiteSpace;
          while (JSScanner.IsBlankSpace(this.GetChar(++this.m_currentPosition)))
            ;
          break;
        case '\n':
        case '\r':
          jsToken = this.ScanLineTerminator(ch1);
          break;
        case '!':
          jsToken = JSToken.LogicalNot;
          if ('=' == this.GetChar(++this.m_currentPosition))
          {
            jsToken = JSToken.NotEqual;
            if ('=' == this.GetChar(++this.m_currentPosition))
            {
              ++this.m_currentPosition;
              jsToken = JSToken.StrictNotEqual;
              break;
            }
            break;
          }
          break;
        case '"':
        case '\'':
          jsToken = JSToken.StringLiteral;
          this.ScanString(ch1);
          break;
        case '#':
          ++this.m_currentPosition;
          jsToken = this.IllegalCharacter();
          break;
        case '$':
        case '_':
          jsToken = this.ScanIdentifier(true);
          break;
        case '%':
          jsToken = JSToken.Modulo;
          if ('=' == this.GetChar(++this.m_currentPosition))
          {
            ++this.m_currentPosition;
            jsToken = JSToken.ModuloAssign;
            break;
          }
          break;
        case '&':
          jsToken = JSToken.BitwiseAnd;
          char ch2 = this.GetChar(++this.m_currentPosition);
          if ('&' == ch2)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.LogicalAnd;
            break;
          }
          if ('=' == ch2)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.BitwiseAndAssign;
            break;
          }
          break;
        case '(':
          jsToken = JSToken.LeftParenthesis;
          ++this.m_currentPosition;
          break;
        case ')':
          jsToken = JSToken.RightParenthesis;
          ++this.m_currentPosition;
          break;
        case '*':
          jsToken = JSToken.Multiply;
          if ('=' == this.GetChar(++this.m_currentPosition))
          {
            ++this.m_currentPosition;
            jsToken = JSToken.MultiplyAssign;
            break;
          }
          break;
        case '+':
          jsToken = JSToken.FirstBinaryOperator;
          char ch3 = this.GetChar(++this.m_currentPosition);
          if ('+' == ch3)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.Increment;
            break;
          }
          if ('=' == ch3)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.PlusAssign;
            break;
          }
          break;
        case ',':
          jsToken = JSToken.Comma;
          ++this.m_currentPosition;
          break;
        case '-':
          jsToken = JSToken.Minus;
          char ch4 = this.GetChar(++this.m_currentPosition);
          if ('-' == ch4)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.Decrement;
            break;
          }
          if ('=' == ch4)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.MinusAssign;
            break;
          }
          break;
        case '.':
          jsToken = JSToken.AccessField;
          char character = this.GetChar(++this.m_currentPosition);
          if (character == '.' && this.GetChar(++this.m_currentPosition) == '.')
          {
            jsToken = JSToken.RestSpread;
            ++this.m_currentPosition;
            break;
          }
          if (JSScanner.IsDigit(character))
          {
            jsToken = this.ScanNumber('.');
            break;
          }
          break;
        case '/':
          jsToken = JSToken.Divide;
          switch (this.GetChar(++this.m_currentPosition))
          {
            case '*':
              this.m_inMultipleLineComment = true;
              if (this.GetChar(++this.m_currentPosition) == '@' && !this.IgnoreConditionalCompilation)
              {
                if (!this.m_conditionalCompilationOn && !this.CheckSubstring(this.m_currentPosition + 1, "cc_on"))
                {
                  this.SkipMultilineComment();
                  jsToken = JSToken.MultipleLineComment;
                  break;
                }
                if (!JSScanner.IsValidIdentifierStart(this.m_strSourceCode, this.m_currentPosition + 1))
                  ++this.m_currentPosition;
                this.m_inConditionalComment = true;
                jsToken = JSToken.ConditionalCommentStart;
                break;
              }
              this.SkipMultilineComment();
              jsToken = JSToken.MultipleLineComment;
              break;
            case '/':
              jsToken = JSToken.SingleLineComment;
              this.m_inSingleLineComment = true;
              switch (this.GetChar(++this.m_currentPosition))
              {
                case '/':
                  if (this.GetChar(++this.m_currentPosition) == '#')
                  {
                    jsToken = JSToken.PreprocessorDirective;
                    if (this.ScanPreprocessingDirective())
                      break;
                    goto label_143;
                  }
                  else
                    break;
                case '@':
                  if (!this.IgnoreConditionalCompilation && (this.m_conditionalCompilationOn || this.CheckSubstring(this.m_currentPosition + 1, "cc_on")))
                  {
                    if (!JSScanner.IsValidIdentifierStart(this.m_strSourceCode, this.m_currentPosition + 1))
                      ++this.m_currentPosition;
                    this.m_inConditionalComment = true;
                    jsToken = JSToken.ConditionalCommentStart;
                    goto label_143;
                  }
                  else
                    break;
              }
              this.SkipSingleLineComment();
              if (!this.m_inMultipleLineComment && this.m_inConditionalComment)
              {
                this.m_inConditionalComment = false;
                jsToken = JSToken.ConditionalCommentEnd;
                break;
              }
              break;
            case '=':
              ++this.m_currentPosition;
              jsToken = JSToken.DivideAssign;
              break;
          }
          break;
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
          ++this.m_currentPosition;
          jsToken = this.ScanNumber(ch1);
          break;
        case ':':
          jsToken = JSToken.Colon;
          ++this.m_currentPosition;
          break;
        case ';':
          jsToken = JSToken.Semicolon;
          ++this.m_currentPosition;
          break;
        case '<':
          if (this.AllowEmbeddedAspNetBlocks && '%' == this.GetChar(++this.m_currentPosition))
          {
            jsToken = this.ScanAspNetBlock();
            break;
          }
          jsToken = JSToken.LessThan;
          if ('<' == this.GetChar(++this.m_currentPosition))
          {
            ++this.m_currentPosition;
            jsToken = JSToken.LeftShift;
          }
          if ('=' == this.GetChar(this.m_currentPosition))
          {
            ++this.m_currentPosition;
            jsToken = jsToken == JSToken.LessThan ? JSToken.LessThanEqual : JSToken.LeftShiftAssign;
            break;
          }
          break;
        case '=':
          jsToken = JSToken.Assign;
          if ('=' == this.GetChar(++this.m_currentPosition))
          {
            jsToken = JSToken.Equal;
            if ('=' == this.GetChar(++this.m_currentPosition))
            {
              ++this.m_currentPosition;
              jsToken = JSToken.StrictEqual;
              break;
            }
            break;
          }
          if (this.GetChar(this.m_currentPosition) == '>')
          {
            ++this.m_currentPosition;
            jsToken = JSToken.ArrowFunction;
            break;
          }
          break;
        case '>':
          jsToken = JSToken.GreaterThan;
          if ('>' == this.GetChar(++this.m_currentPosition))
          {
            jsToken = JSToken.RightShift;
            if ('>' == this.GetChar(++this.m_currentPosition))
            {
              ++this.m_currentPosition;
              jsToken = JSToken.UnsignedRightShift;
            }
          }
          if ('=' == this.GetChar(this.m_currentPosition))
          {
            ++this.m_currentPosition;
            int num;
            switch (jsToken)
            {
              case JSToken.RightShift:
                num = 83;
                break;
              case JSToken.UnsignedRightShift:
                num = 84;
                break;
              case JSToken.GreaterThan:
                num = 67;
                break;
              default:
                num = 122;
                break;
            }
            jsToken = (JSToken) num;
            break;
          }
          break;
        case '?':
          jsToken = JSToken.ConditionalIf;
          ++this.m_currentPosition;
          break;
        case '@':
          if (this.IgnoreConditionalCompilation)
          {
            ++this.m_currentPosition;
            jsToken = this.IllegalCharacter();
            break;
          }
          int startIndex = ++this.m_currentPosition;
          int num1 = (int) this.ScanIdentifier(false);
          switch (this.m_currentPosition - startIndex)
          {
            case 0:
              if ('*' == this.GetChar(this.m_currentPosition) && '/' == this.GetChar(this.m_currentPosition + 1))
              {
                this.m_currentPosition += 2;
                this.m_inMultipleLineComment = false;
                this.m_inConditionalComment = false;
                jsToken = JSToken.ConditionalCommentEnd;
                goto label_143;
              }
              else
              {
                jsToken = this.IllegalCharacter();
                goto label_143;
              }
            case 2:
              if (this.CheckSubstring(startIndex, "if"))
              {
                jsToken = JSToken.ConditionalCompilationIf;
                ++this.m_conditionalCompilationIfLevel;
                if (!this.m_inConditionalComment && !this.m_conditionalCompilationOn)
                {
                  this.m_conditionalCompilationOn = true;
                  goto label_143;
                }
                else
                  goto label_143;
              }
              else
                break;
            case 3:
              if (this.CheckSubstring(startIndex, "set"))
              {
                jsToken = JSToken.ConditionalCompilationSet;
                if (!this.m_inConditionalComment && !this.m_conditionalCompilationOn)
                {
                  this.m_conditionalCompilationOn = true;
                  goto label_143;
                }
                else
                  goto label_143;
              }
              else if (this.CheckSubstring(startIndex, "end"))
              {
                jsToken = JSToken.ConditionalCompilationEnd;
                if (this.m_conditionalCompilationIfLevel > 0)
                {
                  --this.m_conditionalCompilationIfLevel;
                  goto label_143;
                }
                else
                {
                  this.HandleError(JSError.CCInvalidEnd);
                  goto label_143;
                }
              }
              else
                break;
            case 4:
              if (this.CheckSubstring(startIndex, "else"))
              {
                jsToken = JSToken.ConditionalCompilationElse;
                if (this.m_conditionalCompilationIfLevel <= 0)
                {
                  this.HandleError(JSError.CCInvalidElse);
                  goto label_143;
                }
                else
                  goto label_143;
              }
              else if (this.CheckSubstring(startIndex, "elif"))
              {
                jsToken = JSToken.ConditionalCompilationElseIf;
                if (this.m_conditionalCompilationIfLevel <= 0)
                {
                  this.HandleError(JSError.CCInvalidElseIf);
                  goto label_143;
                }
                else
                  goto label_143;
              }
              else
                break;
            case 5:
              if (this.CheckSubstring(startIndex, "cc_on"))
              {
                this.m_conditionalCompilationOn = true;
                jsToken = JSToken.ConditionalCompilationOn;
                goto label_143;
              }
              else
                break;
          }
          if (!this.m_conditionalCompilationOn)
            this.HandleError(JSError.CCOff);
          jsToken = JSToken.ConditionalCompilationVariable;
          break;
        case 'A':
        case 'B':
        case 'C':
        case 'D':
        case 'E':
        case 'F':
        case 'G':
        case 'H':
        case 'I':
        case 'J':
        case 'K':
        case 'L':
        case 'M':
        case 'N':
        case 'O':
        case 'P':
        case 'Q':
        case 'R':
        case 'S':
        case 'T':
        case 'U':
        case 'V':
        case 'W':
        case 'X':
        case 'Y':
        case 'Z':
          jsToken = this.ScanIdentifier(true);
          break;
        case '[':
          jsToken = JSToken.LeftBracket;
          ++this.m_currentPosition;
          break;
        case '\\':
          jsToken = this.ScanIdentifier(true);
          if (jsToken != JSToken.Identifier)
          {
            if (this.GetChar(this.m_currentPosition + 1) == 'u')
            {
              int currentPosition = this.m_currentPosition;
              JSScanner.PeekUnicodeEscape(this.m_strSourceCode, ref this.m_currentPosition);
              if (this.m_currentPosition - currentPosition > 1)
              {
                this.HandleError(JSError.IllegalChar);
                break;
              }
              jsToken = this.ScanIdentifier(true);
              this.HandleError(JSError.BadHexEscapeSequence);
              break;
            }
            if (JSScanner.IsValidIdentifierStart(this.m_strSourceCode, this.m_currentPosition + 1))
            {
              ++this.m_currentPosition;
              jsToken = this.ScanIdentifier(true);
              break;
            }
            ++this.m_currentPosition;
            this.HandleError(JSError.IllegalChar);
            break;
          }
          break;
        case ']':
          jsToken = JSToken.RightBracket;
          ++this.m_currentPosition;
          break;
        case '^':
          jsToken = JSToken.BitwiseXor;
          if ('=' == this.GetChar(++this.m_currentPosition))
          {
            ++this.m_currentPosition;
            jsToken = JSToken.BitwiseXorAssign;
            break;
          }
          break;
        case '`':
          jsToken = this.ScanTemplateLiteral(ch1);
          break;
        case 'a':
        case 'b':
        case 'c':
        case 'd':
        case 'e':
        case 'f':
        case 'g':
        case 'h':
        case 'i':
        case 'j':
        case 'k':
        case 'l':
        case 'm':
        case 'n':
        case 'o':
        case 'p':
        case 'q':
        case 'r':
        case 's':
        case 't':
        case 'u':
        case 'v':
        case 'w':
        case 'x':
        case 'y':
        case 'z':
          this.m_mightBeKeyword = true;
          jsToken = this.ScanKeyword(JSScanner.s_Keywords[(int) ch1 - 97]);
          break;
        case '{':
          jsToken = JSToken.LeftCurly;
          ++this.m_currentPosition;
          break;
        case '|':
          jsToken = JSToken.BitwiseOr;
          char ch5 = this.GetChar(++this.m_currentPosition);
          if ('|' == ch5)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.LogicalOr;
            break;
          }
          if ('=' == ch5)
          {
            ++this.m_currentPosition;
            jsToken = JSToken.BitwiseOrAssign;
            break;
          }
          break;
        case '}':
          jsToken = JSToken.RightCurly;
          ++this.m_currentPosition;
          break;
        case '~':
          jsToken = JSToken.BitwiseNot;
          ++this.m_currentPosition;
          break;
        default:
          if (ch1 == char.MinValue)
          {
            if (this.IsEndOfFile)
            {
              jsToken = JSToken.EndOfFile;
              if (this.m_conditionalCompilationIfLevel > 0)
              {
                this.m_currentToken.EndLineNumber = this.m_currentLine;
                this.m_currentToken.EndLinePosition = this.m_startLinePosition;
                this.m_currentToken.EndPosition = this.m_currentPosition;
                this.HandleError(JSError.NoCCEnd);
                break;
              }
              break;
            }
            ++this.m_currentPosition;
            jsToken = this.IllegalCharacter();
            break;
          }
          if (ch1 == '\u2028' || ch1 == '\u2029')
          {
            jsToken = this.ScanLineTerminator(ch1);
            break;
          }
          if ('\uD800' <= ch1 && ch1 <= '\uDBFF')
          {
            char ch6 = this.GetChar(this.m_currentPosition + 1);
            if ('\uDC00' <= ch6 && ch6 <= '\uDFFF')
            {
              jsToken = this.ScanIdentifier(true);
              if (jsToken != JSToken.Identifier)
              {
                this.m_currentPosition += 2;
                jsToken = this.IllegalCharacter();
                break;
              }
              break;
            }
            ++this.m_currentPosition;
            jsToken = this.IllegalCharacter();
            break;
          }
          if (JSScanner.IsValidIdentifierStart(this.m_strSourceCode, this.m_currentPosition))
          {
            jsToken = this.ScanIdentifier(true);
            break;
          }
          if (JSScanner.IsBlankSpace(ch1))
          {
            jsToken = JSToken.WhiteSpace;
            while (JSScanner.IsBlankSpace(this.GetChar(++this.m_currentPosition)))
              ;
            break;
          }
          ++this.m_currentPosition;
          jsToken = this.IllegalCharacter();
          break;
      }
label_143:
      this.m_currentToken.EndLineNumber = this.m_currentLine;
      this.m_currentToken.EndLinePosition = this.m_startLinePosition;
      this.m_currentToken.EndPosition = this.m_currentPosition;
      this.m_currentToken.Token = jsToken;
      return this.m_currentToken;
    }

    public Context UpdateToken(UpdateHint updateHint)
    {
      if (updateHint == UpdateHint.RegularExpression)
      {
        if (this.m_currentToken.IsOne(JSToken.Divide, JSToken.DivideAssign))
        {
          this.m_currentToken.Token = this.ScanRegExp();
          goto label_7;
        }
      }
      if (updateHint == UpdateHint.TemplateLiteral && this.m_currentToken.Is(JSToken.RightCurly))
        this.m_currentToken.Token = this.ScanTemplateLiteral('}');
      else if (updateHint == UpdateHint.ReplacementToken && this.m_currentToken.Is(JSToken.Modulo))
        this.m_currentToken.Token = this.ScanReplacementToken();
label_7:
      this.m_currentToken.EndLineNumber = this.m_currentLine;
      this.m_currentToken.EndLinePosition = this.m_startLinePosition;
      this.m_currentToken.EndPosition = this.m_currentPosition;
      return this.m_currentToken;
    }

    public static bool IsDigit(char character) => '0' <= character && character <= '9';

    public static bool IsKeyword(string name, bool strictMode)
    {
      bool flag = false;
      if (name != null)
      {
        int num = (int) name[0] - 97;
        if (0 <= num && num < JSScanner.s_Keywords.Length)
        {
          JSKeyword keyword = JSScanner.s_Keywords[(int) name[0] - 97];
          if (keyword != null)
          {
            switch (keyword.GetKeyword(name, 0, name.Length))
            {
              case JSToken.Identifier:
              case JSToken.Get:
              case JSToken.Set:
                flag = false;
                break;
              case JSToken.Let:
              case JSToken.Implements:
              case JSToken.Interface:
              case JSToken.Package:
              case JSToken.Private:
              case JSToken.Protected:
              case JSToken.Public:
              case JSToken.Static:
              case JSToken.Yield:
                flag = strictMode;
                break;
              default:
                flag = true;
                break;
            }
          }
        }
      }
      return flag;
    }

    public static bool IsValidIdentifier(string name)
    {
      bool flag = false;
      if (name != null)
      {
        int startIndex = 0;
        if (JSScanner.IsValidIdentifierStart(name, ref startIndex))
        {
          flag = true;
          while (startIndex < name.Length)
          {
            if (!JSScanner.IsValidIdentifierPart(name, ref startIndex))
            {
              flag = false;
              break;
            }
          }
        }
      }
      return flag;
    }

    private static bool IsValidIdentifierStart(string text, int index) => JSScanner.IsValidIdentifierStart(text, ref index);

    private static bool IsValidIdentifierStart(string name, ref int startIndex)
    {
      bool flag = false;
      if (name != null && startIndex < name.Length)
      {
        int index = startIndex;
        char ch1 = name[index];
        if (ch1 == '\\')
        {
          name = JSScanner.PeekUnicodeEscape(name, ref index);
          if (name != null && JSScanner.IsValidIdentifierStart(name, 0, name.Length))
          {
            startIndex = index;
            flag = true;
          }
        }
        else
        {
          int num;
          if ('\uD800' <= ch1 && ch1 <= '\uDBFF')
          {
            char ch2 = name[num = index + 1];
            if ('\uDC00' <= ch2 && ch2 <= '\uDFFF')
              ++num;
          }
          else
            num = index + 1;
          if (JSScanner.IsValidIdentifierStart(name, startIndex, num - startIndex))
          {
            startIndex = num;
            flag = true;
          }
        }
      }
      return flag;
    }

    private static bool IsValidIdentifierPart(string text, int index) => JSScanner.IsValidIdentifierPart(text, ref index);

    private static bool IsValidIdentifierPart(string name, ref int startIndex)
    {
      bool flag = false;
      if (name != null && startIndex < name.Length)
      {
        int index = startIndex;
        char ch1 = name[index];
        if (ch1 == '\\')
        {
          name = JSScanner.PeekUnicodeEscape(name, ref index);
          if (name != null && JSScanner.IsValidIdentifierPart(name, 0, name.Length))
          {
            startIndex = index;
            flag = true;
          }
        }
        else
        {
          int num;
          if ('\uD800' <= ch1 && ch1 <= '\uDBFF')
          {
            char ch2 = name[num = index + 1];
            if ('\uDC00' <= ch2 && ch2 <= '\uDFFF')
              ++num;
          }
          else
            num = index + 1;
          if (JSScanner.IsValidIdentifierPart(name, startIndex, num - startIndex))
          {
            startIndex = num;
            flag = true;
          }
        }
      }
      return flag;
    }

    private static bool IsValidIdentifierStart(string text, int index, int length)
    {
      if (text != null)
      {
        char ch = text[index];
        if (length == 1 && ('a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_' || ch == '$' || ch == '�'))
          return true;
        switch (char.GetUnicodeCategory(text, index))
        {
          case UnicodeCategory.UppercaseLetter:
          case UnicodeCategory.LowercaseLetter:
          case UnicodeCategory.TitlecaseLetter:
          case UnicodeCategory.ModifierLetter:
          case UnicodeCategory.OtherLetter:
          case UnicodeCategory.LetterNumber:
            return true;
        }
      }
      return false;
    }

    private static bool IsValidIdentifierPart(string text, int index, int length)
    {
      if (text != null)
      {
        char ch = text[index];
        if (length == 1 && ('a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || ch == '$' || ch == '\u200C' || ch == '\u200D' || ch == '�'))
          return true;
        switch (char.GetUnicodeCategory(text, index))
        {
          case UnicodeCategory.UppercaseLetter:
          case UnicodeCategory.LowercaseLetter:
          case UnicodeCategory.TitlecaseLetter:
          case UnicodeCategory.ModifierLetter:
          case UnicodeCategory.OtherLetter:
          case UnicodeCategory.NonSpacingMark:
          case UnicodeCategory.SpacingCombiningMark:
          case UnicodeCategory.DecimalDigitNumber:
          case UnicodeCategory.LetterNumber:
          case UnicodeCategory.ConnectorPunctuation:
            return true;
        }
      }
      return false;
    }

    public static bool StartsWithValidIdentifierPart(string text)
    {
      bool flag = false;
      if (text != null)
      {
        if (text[0] == '\\')
        {
          int index = 0;
          string text1 = JSScanner.PeekUnicodeEscape(text, ref index);
          flag = text1 != null && JSScanner.IsValidIdentifierPart(text1, 0, text1.Length);
        }
        else
          flag = JSScanner.IsValidIdentifierPart(text, 0, 1);
      }
      return flag;
    }

    public static bool IsValidIdentifierPart(char letter) => JSScanner.IsValidIdentifierPart(new string(letter, 1), 0);

    public static bool IsAssignmentOperator(JSToken token) => JSToken.Assign <= token && token <= JSToken.UnsignedRightShiftAssign;

    public static bool IsRightAssociativeOperator(JSToken token) => JSToken.Assign <= token && token <= JSToken.ConditionalIf;

    public static bool IsSafeIdentifier(string name)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(name) && JSScanner.IsSafeIdentifierStart(name[0]))
      {
        for (int index = 1; index < name.Length; ++index)
        {
          if (!JSScanner.IsSafeIdentifierPart(name[index]))
            return false;
        }
        flag = true;
      }
      return flag;
    }

    public static bool IsSafeIdentifierStart(char letter) => 'a' <= letter && letter <= 'z' || 'A' <= letter && letter <= 'Z' || letter == '_' || letter == '$';

    public static bool IsSafeIdentifierPart(char letter) => 'a' <= letter && letter <= 'z' || 'A' <= letter && letter <= 'Z' || '0' <= letter && letter <= '9' || letter == '_' || letter == '$';

    private void OnGlobalDefine(string name)
    {
      if (this.GlobalDefine == null)
        return;
      this.GlobalDefine((object) this, new GlobalDefineEventArgs()
      {
        Name = name
      });
    }

    private void OnNewModule(string newModule)
    {
      if (this.NewModule == null)
        return;
      this.NewModule((object) this, new NewModuleEventArgs()
      {
        Module = newModule
      });
    }

    private JSToken ScanLineTerminator(char ch)
    {
      JSToken jsToken = JSToken.EndOfLine;
      if (this.m_inConditionalComment && this.m_inSingleLineComment)
      {
        jsToken = JSToken.ConditionalCommentEnd;
        this.m_inConditionalComment = this.m_inSingleLineComment = false;
      }
      else
      {
        ++this.m_currentPosition;
        if (ch == '\r' && this.GetChar(this.m_currentPosition) == '\n')
          ++this.m_currentPosition;
        ++this.m_currentLine;
        this.m_startLinePosition = this.m_currentPosition;
        while ((ch = this.GetChar(this.m_currentPosition)) == '\r' || ch == '\n' || ch == '\u2028' || ch == '\u2029')
        {
          if (ch == '\r')
          {
            if (this.GetChar(++this.m_currentPosition) == '\n')
              ++this.m_currentPosition;
          }
          else
            ++this.m_currentPosition;
          ++this.m_currentLine;
          this.m_startLinePosition = this.m_currentPosition;
        }
      }
      this.m_inSingleLineComment = false;
      return jsToken;
    }

    private JSToken ScanIdentifier(bool possibleTemplateLiteral)
    {
      bool flag = false;
      int startIndex = this.m_currentPosition;
      int currentPosition1 = this.m_currentPosition;
      char ch1 = this.GetChar(currentPosition1);
      if (ch1 == '\\')
      {
        this.m_mightBeKeyword = false;
        string text = JSScanner.PeekUnicodeEscape(this.m_strSourceCode, ref currentPosition1);
        if (text != null && JSScanner.IsValidIdentifierStart(text, 0, text.Length))
        {
          this.m_identifier.Append(text);
          startIndex = this.m_currentPosition = currentPosition1;
          flag = true;
        }
      }
      else
      {
        int num;
        if ('\uD800' <= ch1 && ch1 <= '\uDBFF')
        {
          this.m_mightBeKeyword = false;
          ch1 = this.GetChar(num = currentPosition1 + 1);
          if ('\uDC00' <= ch1 && ch1 <= '\uDFFF')
            ++num;
        }
        else
          num = currentPosition1 + 1;
        if (JSScanner.IsValidIdentifierStart(this.m_strSourceCode, this.m_currentPosition, num - this.m_currentPosition))
        {
          this.m_mightBeKeyword = this.m_mightBeKeyword && 'a' <= ch1 && ch1 <= 'z';
          this.m_currentPosition = num;
          flag = true;
        }
      }
      if (flag)
      {
        for (char ch2 = this.GetChar(this.m_currentPosition); ch2 != char.MinValue; ch2 = this.GetChar(this.m_currentPosition))
        {
          int currentPosition2 = this.m_currentPosition;
          if (ch2 == '\\')
          {
            this.m_mightBeKeyword = false;
            string text = JSScanner.PeekUnicodeEscape(this.m_strSourceCode, ref currentPosition2);
            if (text != null && JSScanner.IsValidIdentifierPart(text, 0, text.Length))
            {
              if (this.m_currentPosition > startIndex)
                this.m_identifier.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex);
              this.m_identifier.Append(text);
              startIndex = this.m_currentPosition = currentPosition2;
            }
            else
              break;
          }
          else
          {
            int num;
            if ('\uD800' <= ch2 && ch2 <= '\uDBFF')
            {
              this.m_mightBeKeyword = false;
              ch2 = this.GetChar(num = currentPosition2 + 1);
              if ('\uDC00' <= ch2 && ch2 <= '\uDFFF')
                ++num;
            }
            else
              num = currentPosition2 + 1;
            if (JSScanner.IsValidIdentifierPart(this.m_strSourceCode, this.m_currentPosition, num - this.m_currentPosition))
            {
              this.m_mightBeKeyword = this.m_mightBeKeyword && 'a' <= ch2 && ch2 <= 'z';
              this.m_currentPosition = num;
            }
            else
              break;
          }
        }
        if (this.AllowEmbeddedAspNetBlocks && this.CheckSubstring(this.m_currentPosition, "<%="))
        {
          ++this.m_currentPosition;
          int num = (int) this.ScanAspNetBlock();
        }
        if (this.m_identifier.Length > 0 && this.m_currentPosition - startIndex > 0)
          this.m_identifier.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex);
      }
      if (possibleTemplateLiteral && flag && this.GetChar(this.m_currentPosition) == '`')
        return this.ScanTemplateLiteral('`');
      return !flag ? JSToken.Error : JSToken.Identifier;
    }

    private JSToken ScanKeyword(JSKeyword keyword)
    {
      JSToken jsToken = this.ScanIdentifier(true);
      if (keyword != null && this.m_mightBeKeyword)
      {
        switch (jsToken)
        {
          case JSToken.Identifier:
            jsToken = keyword.GetKeyword(this.m_strSourceCode, this.m_currentToken.StartPosition, this.m_currentPosition - this.m_currentToken.StartPosition);
            break;
          case JSToken.TemplateLiteral:
            int num = this.m_strSourceCode.IndexOf('`', this.m_currentToken.StartPosition);
            JSToken keyword1 = keyword.GetKeyword(this.m_strSourceCode, this.m_currentToken.StartPosition, num - this.m_currentToken.StartPosition);
            if (keyword1 != JSToken.Identifier)
            {
              jsToken = keyword1;
              this.m_currentPosition = num;
              break;
            }
            break;
        }
      }
      return jsToken;
    }

    private JSToken ScanNumber(char leadChar)
    {
      bool flag1 = '.' == leadChar;
      JSToken token = flag1 ? JSToken.NumericLiteral : JSToken.IntegerLiteral;
      bool flag2 = false;
      this.m_literalIssues = false;
      if ('0' == leadChar)
      {
        char character = this.GetChar(this.m_currentPosition);
        if ('x' == character || 'X' == character)
        {
          if (JSScanner.IsHexDigit(this.GetChar(this.m_currentPosition + 1)))
          {
            while (JSScanner.IsHexDigit(this.GetChar(++this.m_currentPosition)))
              ;
          }
          return this.CheckForNumericBadEnding(token);
        }
        if ('b' == character || 'B' == character)
        {
          switch (this.GetChar(this.m_currentPosition + 1))
          {
            case '0':
            case '1':
              char ch;
              int num;
              do
              {
                int index = ++this.m_currentPosition;
                ch = (char) (num = (int) this.GetChar(index));
              }
              while (48 == num || ch == '1');
              break;
          }
          return this.CheckForNumericBadEnding(token);
        }
        if ('o' == character || 'O' == character)
        {
          char ch1 = this.GetChar(this.m_currentPosition + 1);
          if ('0' <= ch1 && ch1 <= '7')
          {
            char ch2;
            int num;
            do
            {
              int index = ++this.m_currentPosition;
              ch2 = (char) (num = (int) this.GetChar(index));
            }
            while (48 <= num && ch2 <= '7');
          }
          return this.CheckForNumericBadEnding(token);
        }
        if ('0' <= character && character <= '7')
        {
          while ('0' <= character && character <= '7')
            character = this.GetChar(++this.m_currentPosition);
          if (JSScanner.IsDigit(character) && '7' < character)
          {
            this.m_literalIssues = true;
            while ('0' <= character && character <= '9')
              character = this.GetChar(++this.m_currentPosition);
            this.HandleError(JSError.BadNumericLiteral);
          }
          this.m_literalIssues = true;
          this.HandleError(JSError.OctalLiteralsDeprecated);
          return token;
        }
        if (character != 'e' && character != 'E' && JSScanner.IsValidIdentifierStart(this.m_strSourceCode, this.m_currentPosition))
          return this.CheckForNumericBadEnding(token);
      }
      while (true)
      {
        char character = this.GetChar(this.m_currentPosition);
        if (!JSScanner.IsDigit(character))
        {
          if ('.' == character)
          {
            if (!flag1)
            {
              flag1 = true;
              token = JSToken.NumericLiteral;
            }
            else
              break;
          }
          else if ('e' == character || 'E' == character)
          {
            if (!flag2)
            {
              flag2 = flag1 = true;
              token = JSToken.NumericLiteral;
            }
            else
              break;
          }
          else if ('+' == character || '-' == character)
          {
            char ch = this.GetChar(this.m_currentPosition - 1);
            if ('e' != ch && 'E' != ch)
              break;
          }
          else
            break;
        }
        ++this.m_currentPosition;
      }
      char ch3 = this.GetChar(this.m_currentPosition - 1);
      if ('+' == ch3 || '-' == ch3)
      {
        --this.m_currentPosition;
        ch3 = this.GetChar(this.m_currentPosition - 1);
      }
      if ('e' == ch3 || 'E' == ch3)
      {
        --this.m_currentPosition;
        ch3 = this.GetChar(this.m_currentPosition - 1);
      }
      if (token == JSToken.NumericLiteral && ch3 == '.')
        token = JSToken.IntegerLiteral;
      return this.CheckForNumericBadEnding(token);
    }

    private JSToken ScanReplacementToken()
    {
      int currentPosition = this.m_currentPosition;
      int currentLine = this.m_currentLine;
      int startLinePosition = this.m_startLinePosition;
      char letter = this.GetChar(this.m_currentPosition);
      if (letter != '.')
      {
        while (JSScanner.IsValidIdentifierPart(letter) || letter == '.' || letter == '-')
          letter = this.GetChar(++this.m_currentPosition);
        if (letter == ':')
        {
          letter = this.GetChar(++this.m_currentPosition);
          while (JSScanner.IsValidIdentifierPart(letter))
            letter = this.GetChar(++this.m_currentPosition);
        }
      }
      if (letter == '%' && this.m_currentPosition > currentPosition + 1 && this.GetChar(this.m_currentPosition - 1) != '.')
      {
        ++this.m_currentPosition;
        return JSToken.ReplacementToken;
      }
      this.m_currentPosition = currentPosition;
      this.m_currentLine = currentLine;
      this.m_startLinePosition = startLinePosition;
      return this.m_currentToken.Token;
    }

    private JSToken ScanRegExp()
    {
      int currentPosition = this.m_currentPosition;
      int currentLine = this.m_currentLine;
      int startLinePosition = this.m_startLinePosition;
      bool flag1 = false;
      bool flag2 = false;
      while (true)
      {
        char ch;
        do
        {
          int index = this.m_currentPosition++;
          if (!this.IsEndLineOrEOF(ch = this.GetChar(index), 0))
          {
            if (!flag1)
            {
              if (ch != '[')
              {
                if (!flag2)
                  goto label_8;
              }
              else
                goto label_4;
            }
            else
              goto label_2;
          }
          else
            goto label_13;
        }
        while (ch != ']');
        goto label_7;
label_2:
        flag1 = false;
        continue;
label_4:
        flag2 = true;
        continue;
label_7:
        flag2 = false;
        continue;
label_8:
        switch (ch)
        {
          case '/':
            goto label_9;
          case '\\':
            flag1 = true;
            continue;
          default:
            continue;
        }
      }
label_9:
      if (currentPosition != this.m_currentPosition)
        return JSToken.RegularExpression;
label_13:
      this.m_currentPosition = currentPosition;
      this.m_currentLine = currentLine;
      this.m_startLinePosition = startLinePosition;
      return this.m_currentToken.Token;
    }

    private JSToken ScanAspNetBlock()
    {
      JSToken jsToken = JSToken.AspNetBlock;
      char ch = this.GetChar(++this.m_currentPosition);
      ++this.m_currentPosition;
      while (this.GetChar(this.m_currentPosition - 1) != '%' || this.GetChar(this.m_currentPosition) != '>' || this.IsEndOfFile)
        ++this.m_currentPosition;
      this.m_currentToken.EndPosition = this.m_currentPosition + 1;
      this.m_currentToken.EndLineNumber = this.m_currentLine;
      this.m_currentToken.EndLinePosition = this.m_startLinePosition;
      if (this.IsEndOfFile)
      {
        this.HandleError(JSError.UnterminatedAspNetBlock);
      }
      else
      {
        ++this.m_currentPosition;
        if (ch == '=')
        {
          jsToken = JSToken.Identifier;
          if (JSScanner.IsValidIdentifierPart(this.m_strSourceCode, this.m_currentPosition) || this.CheckSubstring(this.m_currentPosition, "<%="))
          {
            while (true)
            {
              while (!JSScanner.IsValidIdentifierPart(this.m_strSourceCode, ref this.m_currentPosition))
              {
                if (this.CheckSubstring(this.m_currentPosition, "<%="))
                {
                  this.m_currentPosition += 4;
                  while (this.GetChar(this.m_currentPosition - 1) != '%' || this.GetChar(this.m_currentPosition) != '>' || this.IsEndOfFile)
                    ++this.m_currentPosition;
                  this.m_currentToken.EndPosition = this.m_currentPosition + 1;
                  this.m_currentToken.EndLineNumber = this.m_currentLine;
                  this.m_currentToken.EndLinePosition = this.m_startLinePosition;
                  if (this.IsEndOfFile)
                    this.HandleError(JSError.UnterminatedAspNetBlock);
                  else
                    ++this.m_currentPosition;
                }
                else
                  goto label_17;
              }
              do
                ;
              while (JSScanner.IsValidIdentifierPart(this.m_strSourceCode, ref this.m_currentPosition));
              this.m_currentToken.EndPosition = this.m_currentPosition;
            }
          }
        }
      }
label_17:
      return jsToken;
    }

    private void ScanString(char delimiter)
    {
      int startIndex = ++this.m_currentPosition;
      this.m_decodedString = (string) null;
      this.m_literalIssues = false;
      StringBuilder stringBuilder = (StringBuilder) null;
      while (true)
      {
        int accumulator;
        do
        {
          char c;
          do
          {
            int index = this.m_currentPosition++;
            if ((int) (c = this.GetChar(index)) != (int) delimiter)
            {
              if (c != '\\')
              {
                if (!this.IsLineTerminator(c, 0))
                {
                  if (c == char.MinValue)
                  {
                    this.m_literalIssues = true;
                    if (this.IsEndOfFile)
                      goto label_7;
                  }
                  if (!this.AllowEmbeddedAspNetBlocks || c != '<' || this.GetChar(this.m_currentPosition) != '%')
                  {
                    if ('\uD800' <= c && c <= '\uDBFF')
                      goto label_11;
                  }
                  else
                    goto label_9;
                }
                else
                  goto label_3;
              }
              else
                goto label_25;
            }
            else
              goto label_62;
          }
          while ('\uDC00' > c || c > '\uDFFF');
          goto label_24;
label_11:
          char ch = this.GetChar(this.m_currentPosition);
          if ('\uDC00' > ch || ch > '\uDFFF')
          {
            if (ch == '\\' && this.GetChar(this.m_currentPosition + 1) == 'u')
            {
              if (stringBuilder == null)
                stringBuilder = new StringBuilder(128);
              if (this.m_currentPosition - startIndex > 0)
                stringBuilder.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex);
              if (this.ScanHexSequence(this.m_currentPosition += 2, 'u', out accumulator))
              {
                stringBuilder.Append((char) accumulator);
                startIndex = this.m_currentPosition;
              }
              else
                goto label_21;
            }
            else
              goto label_22;
          }
          else
            goto label_12;
        }
        while (accumulator >= 56320 && 57343 >= accumulator);
        goto label_20;
label_9:
        this.SkipAspNetReplacement();
        this.m_literalIssues = true;
        continue;
label_12:
        ++this.m_currentPosition;
        continue;
label_20:
        this.m_literalIssues = true;
        this.HandleError(JSError.HighSurrogate);
        continue;
label_21:
        this.m_literalIssues = true;
        this.HandleError(JSError.HighSurrogate);
        continue;
label_22:
        this.m_literalIssues = true;
        this.HandleError(JSError.HighSurrogate);
        continue;
label_24:
        this.m_literalIssues = true;
        this.HandleError(JSError.LowSurrogate);
        continue;
label_25:
        int num1 = 0;
        if (stringBuilder == null)
          stringBuilder = new StringBuilder(128);
        if (this.m_currentPosition - startIndex - 1 > 0)
          stringBuilder.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex - 1);
        bool flag = false;
        char hexType = this.GetChar(this.m_currentPosition++);
        switch (hexType)
        {
          case '\n':
          case '\u2028':
          case '\u2029':
            ++this.m_currentLine;
            this.m_startLinePosition = this.m_currentPosition;
            break;
          case '\r':
            if ('\n' == this.GetChar(this.m_currentPosition))
            {
              ++this.m_currentPosition;
              goto case '\n';
            }
            else
              goto case '\n';
          case '"':
            stringBuilder.Append('"');
            break;
          case '\'':
            stringBuilder.Append('\'');
            break;
          case '0':
          case '1':
          case '2':
          case '3':
            flag = true;
            num1 = (int) hexType - 48 << 6;
            goto case '4';
          case '4':
          case '5':
          case '6':
          case '7':
            this.m_literalIssues = true;
            if (!flag)
              num1 = (int) hexType - 48 << 3;
            char ch1 = this.GetChar(this.m_currentPosition++);
            if ('0' <= ch1 && ch1 <= '7')
            {
              if (flag)
              {
                int num2 = num1 | (int) ch1 - 48 << 3;
                char ch2 = this.GetChar(this.m_currentPosition++);
                if ('0' <= ch2 && ch2 <= '7')
                {
                  int num3 = num2 | (int) ch2 - 48;
                  stringBuilder.Append((char) num3);
                }
                else
                {
                  stringBuilder.Append((char) (num2 >> 3));
                  --this.m_currentPosition;
                }
              }
              else
              {
                int num4 = num1 | (int) ch1 - 48;
                stringBuilder.Append((char) num4);
              }
            }
            else
            {
              if (flag)
                stringBuilder.Append((char) (num1 >> 6));
              else
                stringBuilder.Append((char) (num1 >> 3));
              --this.m_currentPosition;
            }
            this.HandleError(JSError.OctalLiteralsDeprecated);
            break;
          case '\\':
            stringBuilder.Append('\\');
            break;
          case 'b':
            stringBuilder.Append('\b');
            break;
          case 'f':
            stringBuilder.Append('\f');
            break;
          case 'n':
            stringBuilder.Append('\n');
            break;
          case 'r':
            stringBuilder.Append('\r');
            break;
          case 't':
            stringBuilder.Append('\t');
            break;
          case 'u':
          case 'x':
            string unescaped;
            if (this.ScanHexEscape(hexType, out unescaped))
            {
              stringBuilder.Append(unescaped);
              break;
            }
            stringBuilder.Append(this.m_strSourceCode.Substring(this.m_currentPosition - 2, 2));
            this.m_literalIssues = true;
            this.HandleError(JSError.BadHexEscapeSequence);
            break;
          case 'v':
            this.m_literalIssues = true;
            stringBuilder.Append('\v');
            break;
          default:
            stringBuilder.Append(hexType);
            break;
        }
        startIndex = this.m_currentPosition;
      }
label_3:
      this.HandleError(JSError.UnterminatedString);
      --this.m_currentPosition;
      if (this.GetChar(this.m_currentPosition - 1) == '\r')
      {
        --this.m_currentPosition;
        goto label_62;
      }
      else
        goto label_62;
label_7:
      --this.m_currentPosition;
      this.HandleError(JSError.UnterminatedString);
label_62:
      if (stringBuilder != null)
      {
        if (this.m_currentPosition - startIndex - 1 > 0)
          stringBuilder.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex - 1);
        this.m_decodedString = stringBuilder.ToString();
      }
      else if (this.m_currentPosition == this.m_currentToken.StartPosition + 1)
        this.m_decodedString = string.Empty;
      else
        this.m_decodedString = this.m_strSourceCode.Substring(this.m_currentToken.StartPosition + 1, this.m_currentPosition - this.m_currentToken.StartPosition - ((int) this.GetChar(this.m_currentPosition - 1) == (int) delimiter ? 2 : 1));
    }

    private bool ScanHexEscape(char hexType, out string unescaped)
    {
      int accumulator1;
      bool flag = this.ScanHexSequence(this.m_currentPosition, hexType, out accumulator1);
      if (flag)
      {
        if (55296 <= accumulator1 && accumulator1 <= 56319)
        {
          char ch = this.GetChar(this.m_currentPosition);
          if ('\uDC00' <= ch && ch <= '\uDFFF')
          {
            ++this.m_currentPosition;
            unescaped = new string(new char[2]
            {
              (char) accumulator1,
              ch
            });
            return true;
          }
          if (ch == '\\' && this.GetChar(this.m_currentPosition + 1) == 'u')
          {
            this.m_currentPosition += 2;
            int accumulator2;
            if (this.ScanHexSequence(this.m_currentPosition, hexType, out accumulator2))
            {
              unescaped = new string(new char[2]
              {
                (char) accumulator1,
                (char) accumulator2
              });
              return true;
            }
          }
          this.HandleError(JSError.HighSurrogate);
          this.m_literalIssues = true;
          unescaped = new string((char) accumulator1, 1);
          return true;
        }
        if (56320 <= accumulator1 && accumulator1 <= 57343)
        {
          this.HandleError(JSError.LowSurrogate);
          this.m_literalIssues = true;
          unescaped = new string((char) accumulator1, 1);
          return true;
        }
      }
      unescaped = flag ? char.ConvertFromUtf32(accumulator1) : (string) null;
      return flag;
    }

    private bool ScanHexSequence(int startOfDigits, char hexType, out int accumulator)
    {
      bool flag = true;
      int num1 = hexType == 'x' ? 2 : 4;
      if (hexType == 'u' && this.GetChar(this.m_currentPosition) == '{')
      {
        ++this.m_currentPosition;
        num1 = 6;
      }
      accumulator = 0;
      char ch;
      for (ch = this.GetChar(this.m_currentPosition); this.m_currentPosition - startOfDigits < num1 && JSScanner.IsHexDigit(ch); ch = this.GetChar(++this.m_currentPosition))
      {
        if (JSScanner.IsDigit(ch))
          accumulator = accumulator << 4 | (int) ch - 48;
        else if ('A' <= ch && ch <= 'F')
          accumulator = accumulator << 4 | (int) ch - 65 + 10;
        else if ('a' <= ch && ch <= 'f')
          accumulator = accumulator << 4 | (int) ch - 97 + 10;
      }
      int num2 = this.m_currentPosition - startOfDigits;
      if (num2 == 0 || num1 != 6 && num2 != num1 || num1 == 6 && ch != '}')
      {
        flag = false;
        this.m_currentPosition = startOfDigits;
      }
      else if (num1 == 6 && ch == '}')
        ++this.m_currentPosition;
      return flag;
    }

    private JSToken ScanTemplateLiteral(char ch)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = this.m_currentToken.StartPosition;
      if (ch == '`')
        ++this.m_currentPosition;
      ch = this.GetChar(this.m_currentPosition);
      while (true)
      {
        switch (ch)
        {
          case char.MinValue:
          case '`':
            goto label_30;
          case '$':
            if (this.GetChar(this.m_currentPosition + 1) != '{')
              break;
            goto label_4;
          case '\\':
            if (stringBuilder == null)
              stringBuilder = new StringBuilder(128);
            if (this.m_currentPosition > startIndex)
              stringBuilder.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex);
            ch = this.GetChar(++this.m_currentPosition);
            switch (ch)
            {
              case '\n':
              case '\u2028':
              case '\u2029':
                ++this.m_currentLine;
                this.m_startLinePosition = this.m_currentPosition;
                break;
              case '\r':
                if (this.GetChar(this.m_currentPosition + 1) == '\n')
                {
                  ++this.m_currentPosition;
                  goto case '\n';
                }
                else
                  goto case '\n';
              case '0':
                stringBuilder.Append(char.MinValue);
                break;
              case 'b':
                stringBuilder.Append('\b');
                break;
              case 'f':
                stringBuilder.Append('\f');
                break;
              case 'n':
                stringBuilder.Append('\n');
                break;
              case 'r':
                stringBuilder.Append('\r');
                break;
              case 't':
                stringBuilder.Append('\t');
                break;
              case 'u':
              case 'x':
                ++this.m_currentPosition;
                string unescaped;
                if (this.ScanHexEscape(ch, out unescaped))
                {
                  stringBuilder.Append(unescaped);
                }
                else
                {
                  stringBuilder.Append(this.m_strSourceCode.Substring(this.m_currentPosition - 2, 2));
                  this.m_literalIssues = true;
                  this.HandleError(JSError.BadHexEscapeSequence);
                }
                --this.m_currentPosition;
                break;
              case 'v':
                stringBuilder.Append('\v');
                break;
              default:
                stringBuilder.Append(ch);
                break;
            }
            startIndex = this.m_currentPosition + 1;
            break;
          default:
            if (this.IsLineTerminator(ch, 1))
            {
              ++this.m_currentPosition;
              ++this.m_currentLine;
              this.m_startLinePosition = this.m_currentPosition + 1;
              break;
            }
            break;
        }
        ch = this.GetChar(++this.m_currentPosition);
      }
label_4:
      this.m_currentPosition += 2;
label_30:
      if (ch == '`')
        ++this.m_currentPosition;
      if (stringBuilder != null)
      {
        if (this.m_currentPosition > startIndex)
          stringBuilder.Append(this.m_strSourceCode, startIndex, this.m_currentPosition - startIndex);
        this.m_decodedString = stringBuilder.ToString();
      }
      else
        this.m_decodedString = this.m_strSourceCode.Substring(this.m_currentToken.StartPosition, this.m_currentPosition - this.m_currentToken.StartPosition);
      return JSToken.TemplateLiteral;
    }

    private void SkipAspNetReplacement()
    {
      ++this.m_currentPosition;
      char ch;
      do
      {
        int index = this.m_currentPosition++;
        if ((ch = this.GetChar(index)) == char.MinValue && this.IsEndOfFile)
          goto label_4;
      }
      while (ch != '%' || this.GetChar(this.m_currentPosition) != '>');
      ++this.m_currentPosition;
      return;
label_4:;
    }

    private void SkipSingleLineComment()
    {
      this.SkipToEndOfLine();
      this.m_inSingleLineComment = false;
      this.m_currentToken.EndPosition = this.m_currentPosition;
      this.m_currentToken.EndLinePosition = this.m_startLinePosition;
      this.m_currentToken.EndLineNumber = this.m_currentLine;
    }

    private void SkipToEndOfLine()
    {
      char ch = this.GetChar(this.m_currentPosition);
      while (ch != char.MinValue && ch != '\n' && ch != '\r' && ch != '\u2028' && ch != '\u2029')
        ch = this.GetChar(++this.m_currentPosition);
    }

    private void SkipOneLineTerminator()
    {
      switch (this.GetChar(this.m_currentPosition))
      {
        case '\n':
        case '\u2028':
        case '\u2029':
          ++this.m_currentPosition;
          ++this.m_currentLine;
          this.m_startLinePosition = this.m_currentPosition;
          break;
        case '\r':
          if (this.GetChar(++this.m_currentPosition) == '\n')
            ++this.m_currentPosition;
          ++this.m_currentLine;
          this.m_startLinePosition = this.m_currentPosition;
          break;
      }
    }

    private void SkipMultilineComment()
    {
      while (true)
      {
        char c = this.GetChar(this.m_currentPosition);
        while ('*' == c)
        {
          c = this.GetChar(++this.m_currentPosition);
          if ('/' == c)
          {
            ++this.m_currentPosition;
            this.m_inMultipleLineComment = false;
            this.m_currentToken.EndPosition = this.m_currentPosition;
            this.m_currentToken.EndLinePosition = this.m_startLinePosition;
            this.m_currentToken.EndLineNumber = this.m_currentLine;
            return;
          }
          if (c != char.MinValue)
          {
            if (this.IsLineTerminator(c, 1))
            {
              c = this.GetChar(++this.m_currentPosition);
              ++this.m_currentLine;
              this.m_startLinePosition = this.m_currentPosition + 1;
            }
          }
          else
            break;
        }
        if (c != char.MinValue || !this.IsEndOfFile)
        {
          if (this.IsLineTerminator(c, 1))
          {
            ++this.m_currentLine;
            this.m_startLinePosition = this.m_currentPosition + 1;
          }
          ++this.m_currentPosition;
        }
        else
          break;
      }
      this.m_currentToken.EndPosition = this.m_currentPosition;
      this.m_currentToken.EndLinePosition = this.m_startLinePosition;
      this.m_currentToken.EndLineNumber = this.m_currentLine;
      this.m_currentToken.HandleError(JSError.NoCommentEnd, true);
    }

    private void SkipBlanks()
    {
      char c = this.GetChar(this.m_currentPosition);
      while (JSScanner.IsBlankSpace(c))
        c = this.GetChar(++this.m_currentPosition);
    }

    private bool CheckSubstring(int startIndex, string target)
    {
      for (int index = 0; index < target.Length; ++index)
      {
        if ((int) target[index] != (int) this.GetChar(startIndex + index))
          return false;
      }
      return true;
    }

    private bool CheckCaseInsensitiveSubstring(string target)
    {
      int currentPosition = this.m_currentPosition;
      for (int index = 0; index < target.Length; ++index)
      {
        if ((int) target[index] != (int) char.ToUpperInvariant(this.GetChar(currentPosition + index)))
          return false;
      }
      this.m_currentPosition += target.Length;
      return true;
    }

    private JSToken CheckForNumericBadEnding(JSToken token)
    {
      bool flag = false;
      char ch = this.GetChar(this.m_currentPosition);
      if ('0' <= ch && ch <= '9')
      {
        ++this.m_currentPosition;
        flag = true;
      }
      else if (JSScanner.IsValidIdentifierStart(this.m_strSourceCode, ref this.m_currentPosition))
        flag = true;
      if (flag)
      {
        do
          ;
        while (JSScanner.IsValidIdentifierPart(this.m_strSourceCode, ref this.m_currentPosition));
        this.m_literalIssues = true;
        this.HandleError(JSError.BadNumericLiteral);
        token = JSToken.NumericLiteral;
      }
      return token;
    }

    private char GetChar(int index) => index < this.m_endPos ? this.m_strSourceCode[index] : char.MinValue;

    private static int GetHexValue(char hex) => '0' > hex || hex > '9' ? ('a' > hex || hex > 'f' ? (int) hex - 65 + 10 : (int) hex - 97 + 10) : (int) hex - 48;

    private static int DecodeOneUnicodeEscapeSequence(string text, ref int index)
    {
      int num1 = -1;
      if (text != null && index + 4 < text.Length && text[index] == '\\' && text[index + 1] == 'u')
      {
        if (text[index + 2] == '{')
        {
          char minValue = char.MinValue;
          int num2 = 0;
          index += 2;
          while (++index < text.Length && (minValue = text[index]) != '}' && JSScanner.IsHexDigit(minValue))
            num2 = num2 << 4 | JSScanner.GetHexValue(minValue);
          if (minValue == '}')
          {
            num1 = num2;
            ++index;
          }
        }
        else if (index + 5 < text.Length && JSScanner.IsHexDigit(text[index + 2]) && JSScanner.IsHexDigit(text[index + 3]) && JSScanner.IsHexDigit(text[index + 4]) && JSScanner.IsHexDigit(text[index + 5]))
        {
          num1 = JSScanner.GetHexValue(text[index + 2]) << 12 | JSScanner.GetHexValue(text[index + 3]) << 8 | JSScanner.GetHexValue(text[index + 4]) << 4 | JSScanner.GetHexValue(text[index + 5]);
          index += 6;
        }
        else
          ++index;
      }
      return num1;
    }

    private static string PeekUnicodeEscape(string text, ref int index)
    {
      int utf32 = JSScanner.DecodeOneUnicodeEscapeSequence(text, ref index);
      if (55296 <= utf32 && utf32 <= 56319)
      {
        int num1 = index;
        int num2 = JSScanner.DecodeOneUnicodeEscapeSequence(text, ref index);
        if (56320 <= num2 && num2 <= 57343)
          return new string(new char[2]
          {
            (char) utf32,
            (char) num2
          });
        index = num1;
        return new string(new char[1]{ (char) utf32 });
      }
      return 56320 <= utf32 && utf32 <= 57343 ? new string(new char[1]
      {
        (char) utf32
      }) : (utf32 < 0 || utf32 > 1114111 ? (string) null : char.ConvertFromUtf32(utf32));
    }

    private static bool IsHexDigit(char c)
    {
      if ('0' <= c && c <= '9' || 'A' <= c && c <= 'F')
        return true;
      return 'a' <= c && c <= 'f';
    }

    private bool IsLineTerminator(char c, int increment)
    {
      switch (c)
      {
        case '\n':
          return true;
        case '\r':
          if ('\n' == this.GetChar(this.m_currentPosition + increment))
            ++this.m_currentPosition;
          return true;
        case '\u2028':
          return true;
        case '\u2029':
          return true;
        default:
          return false;
      }
    }

    private bool IsEndLineOrEOF(char c, int increment)
    {
      if (this.IsLineTerminator(c, increment))
        return true;
      return c == char.MinValue && this.IsEndOfFile;
    }

    private static bool IsBlankSpace(char c)
    {
      switch (c)
      {
        case '\t':
        case '\v':
        case '\f':
        case ' ':
        case ' ':
        case '\uFEFF':
          return true;
        default:
          return c >= '\u0080' && char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
      }
    }

    internal static bool IsProcessableOperator(JSToken token) => JSToken.FirstBinaryOperator <= token && token <= JSToken.ConditionalIf;

    private string PPScanIdentifier(bool forceUpper)
    {
      string str = (string) null;
      int currentPosition = this.m_currentPosition;
      if (JSScanner.IsValidIdentifierStart(this.m_strSourceCode, ref this.m_currentPosition))
      {
        while (JSScanner.IsValidIdentifierPart(this.m_strSourceCode, ref this.m_currentPosition))
          ;
      }
      if (this.m_currentPosition > currentPosition)
      {
        str = this.m_strSourceCode.Substring(currentPosition, this.m_currentPosition - currentPosition);
        if (forceUpper)
          str = str.ToUpperInvariant();
      }
      return str;
    }

    private bool PPScanInteger(out int intValue)
    {
      int currentPosition = this.m_currentPosition;
      while (JSScanner.IsDigit(this.GetChar(this.m_currentPosition)))
        ++this.m_currentPosition;
      bool flag = false;
      if (this.m_currentPosition > currentPosition)
        flag = int.TryParse(this.m_strSourceCode.Substring(currentPosition, this.m_currentPosition - currentPosition), out intValue);
      else
        intValue = 0;
      return flag;
    }

    private int PPSkipToDirective(params string[] endStrings)
    {
      int currentPosition = this.m_currentPosition;
      int currentLine = this.m_currentLine;
      int startLinePosition = this.m_startLinePosition;
      do
      {
        do
        {
          char ch = this.GetChar(this.m_currentPosition++);
          if (ch <= '\n')
          {
            if (ch != char.MinValue)
            {
              if (ch == '\n')
              {
                ++this.m_currentLine;
                this.m_startLinePosition = this.m_currentPosition;
              }
            }
            else if (this.IsEndOfFile)
            {
              --this.m_currentPosition;
              this.m_currentToken.EndPosition = this.m_currentPosition;
              this.m_currentToken.EndLineNumber = this.m_currentLine;
              this.m_currentToken.EndLinePosition = this.m_startLinePosition;
              Context context = this.m_currentToken.Clone();
              context.EndPosition = currentPosition;
              context.EndLineNumber = currentLine;
              context.EndLinePosition = startLinePosition;
              context.HandleError(string.CompareOrdinal(endStrings[0], "#ENDDEBUG") == 0 ? JSError.NoEndDebugDirective : JSError.NoEndIfDirective);
              throw new EndOfStreamException();
            }
          }
          else if (ch != '\r')
          {
            if (ch != '/')
            {
              switch ((int) ch - 8232)
              {
                case 0:
                  ++this.m_currentLine;
                  this.m_startLinePosition = this.m_currentPosition;
                  continue;
                case 1:
                  ++this.m_currentLine;
                  this.m_startLinePosition = this.m_currentPosition;
                  continue;
                default:
                  continue;
              }
            }
          }
          else
          {
            if (this.GetChar(this.m_currentPosition) == '\n')
              ++this.m_currentPosition;
            ++this.m_currentLine;
            this.m_startLinePosition = this.m_currentPosition;
          }
        }
        while (!this.CheckSubstring(this.m_currentPosition, "//"));
        this.m_currentPosition += 2;
        if (this.CheckCaseInsensitiveSubstring("#IFDEF") || this.CheckCaseInsensitiveSubstring("#IFNDEF") || this.CheckCaseInsensitiveSubstring("#IF"))
        {
          this.PPSkipToDirective("#ENDIF");
        }
        else
        {
          for (int directive = 0; directive < endStrings.Length; ++directive)
          {
            if (this.CheckCaseInsensitiveSubstring(endStrings[directive]))
              return directive;
          }
        }
      }
      while (!this.CheckCaseInsensitiveSubstring("#END") || !JSScanner.IsBlankSpace(this.GetChar(this.m_currentPosition)) && !this.IsAtEndOfLine);
      return 0;
    }

    private bool ScanPreprocessingDirective()
    {
      if (this.CheckCaseInsensitiveSubstring("#GLOBALS"))
        return this.ScanGlobalsDirective();
      if (this.CheckCaseInsensitiveSubstring("#SOURCE"))
        return this.ScanSourceDirective();
      if (this.UsePreprocessorDefines)
      {
        if (this.CheckCaseInsensitiveSubstring("#DEBUG"))
          return this.ScanDebugDirective();
        if (this.CheckCaseInsensitiveSubstring("#IF"))
          return this.ScanIfDirective();
        if (this.CheckCaseInsensitiveSubstring("#ELSE") && this.m_ifDirectiveLevel > 0)
          return this.ScanElseDirective();
        if (this.CheckCaseInsensitiveSubstring("#ENDIF") && this.m_ifDirectiveLevel > 0)
          return this.ScanEndIfDirective();
        if (this.CheckCaseInsensitiveSubstring("#DEFINE"))
          return this.ScanDefineDirective();
        if (this.CheckCaseInsensitiveSubstring("#UNDEF"))
          return this.ScanUndefineDirective();
      }
      return true;
    }

    private bool ScanGlobalsDirective()
    {
      this.SkipBlanks();
      while (!this.IsAtEndOfLine)
      {
        string name = this.PPScanIdentifier(false);
        if (name != null)
          this.OnGlobalDefine(name);
        this.SkipBlanks();
      }
      return true;
    }

    private bool ScanSourceDirective()
    {
      this.SkipBlanks();
      int intValue1 = 0;
      int intValue2 = 0;
      if (this.PPScanInteger(out intValue1))
      {
        this.SkipBlanks();
        if (this.PPScanInteger(out intValue2))
        {
          this.SkipBlanks();
          int currentPosition = this.m_currentPosition;
          this.SkipToEndOfLine();
          if (this.m_currentPosition > currentPosition)
          {
            this.SkipOneLineTerminator();
            string str = this.m_strSourceCode.Substring(currentPosition, this.m_currentPosition - currentPosition).TrimEnd();
            this.m_currentToken.ChangeFileContext(str);
            this.m_currentLine = intValue1;
            this.m_startLinePosition = this.m_currentPosition - intValue2 + 1;
            this.m_currentToken.SourceOffsetStart = this.m_currentToken.SourceOffsetEnd = this.m_currentPosition;
            this.OnNewModule(str);
            return false;
          }
        }
      }
      return true;
    }

    private bool ScanIfDirective()
    {
      bool flag1 = this.CheckCaseInsensitiveSubstring("DEF");
      bool flag2 = !flag1 && this.CheckCaseInsensitiveSubstring("NDEF");
      this.SkipBlanks();
      if (!this.IsAtEndOfLine)
      {
        string key = this.PPScanIdentifier(true);
        if (!string.IsNullOrEmpty(key))
        {
          ++this.m_ifDirectiveLevel;
          bool flag3 = this.m_defines != null && this.m_defines.ContainsKey(key);
          this.SkipBlanks();
          if (flag1 || flag2 || this.IsAtEndOfLine)
          {
            if ((flag2 || !flag3) && (!flag2 || flag3))
            {
              if (this.PPSkipToDirective("#ENDIF", "#ELSE") == 0)
                --this.m_ifDirectiveLevel;
            }
          }
          else
          {
            Func<string, string, bool> func = this.CheckForOperator((SortedDictionary<string, Func<string, string, bool>>) JSScanner.PPOperators.Instance);
            if (func != null)
            {
              this.SkipBlanks();
              int currentPosition = this.m_currentPosition;
              if (!this.IsAtEndOfLine)
                this.SkipToEndOfLine();
              string str = this.m_strSourceCode.Substring(currentPosition, this.m_currentPosition - currentPosition);
              if (!flag3 || !func(this.m_defines[key], str.TrimEnd()))
              {
                if (this.PPSkipToDirective("#ENDIF", "#ELSE") == 0)
                  --this.m_ifDirectiveLevel;
              }
            }
          }
        }
      }
      return true;
    }

    private Func<string, string, bool> CheckForOperator(
      SortedDictionary<string, Func<string, string, bool>> operators)
    {
      foreach (KeyValuePair<string, Func<string, string, bool>> keyValuePair in operators)
      {
        if (this.CheckCaseInsensitiveSubstring(keyValuePair.Key))
          return keyValuePair.Value;
      }
      return (Func<string, string, bool>) null;
    }

    private bool ScanElseDirective()
    {
      --this.m_ifDirectiveLevel;
      this.PPSkipToDirective("#ENDIF");
      return true;
    }

    private bool ScanEndIfDirective()
    {
      --this.m_ifDirectiveLevel;
      return true;
    }

    private bool ScanDefineDirective()
    {
      this.SkipBlanks();
      if (!this.IsAtEndOfLine)
      {
        string key = this.PPScanIdentifier(true);
        if (!string.IsNullOrEmpty(key))
        {
          string str = string.Empty;
          this.SkipBlanks();
          if (this.GetChar(this.m_currentPosition) == '=')
          {
            int startIndex = ++this.m_currentPosition;
            this.SkipToEndOfLine();
            str = this.m_strSourceCode.Substring(startIndex, this.m_currentPosition - startIndex).Trim();
          }
          if (this.m_defines == null)
            this.m_defines = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (!this.m_defines.ContainsKey(key))
            this.m_defines.Add(key, str);
          else
            this.m_defines[key] = str;
        }
      }
      return true;
    }

    private bool ScanUndefineDirective()
    {
      this.SkipBlanks();
      if (!this.IsAtEndOfLine)
      {
        string key = this.PPScanIdentifier(true);
        if (!string.IsNullOrEmpty(key) && this.m_defines != null && this.m_defines.ContainsKey(key))
          this.m_defines.Remove(key);
      }
      return true;
    }

    private bool ScanDebugDirective()
    {
      if (this.GetChar(this.m_currentPosition) == '=')
      {
        ++this.m_currentPosition;
        string name = this.PPScanIdentifier(false);
        if (name == null)
        {
          this.DebugLookupCollection.Clear();
        }
        else
        {
          this.OnGlobalDefine(name);
          while (this.GetChar(this.m_currentPosition) == '.')
          {
            ++this.m_currentPosition;
            string str = this.PPScanIdentifier(false);
            if (str != null)
            {
              name = name + (object) '.' + str;
            }
            else
            {
              name = (string) null;
              break;
            }
          }
          if (name != null)
            this.DebugLookupCollection.Add(name);
        }
      }
      else if (this.StripDebugCommentBlocks && (this.m_defines == null || !this.m_defines.ContainsKey("DEBUG")))
        this.PPSkipToDirective("#ENDDEBUG");
      return true;
    }

    private void HandleError(JSError error)
    {
      this.m_currentToken.EndPosition = this.m_currentPosition;
      this.m_currentToken.EndLinePosition = this.m_startLinePosition;
      this.m_currentToken.EndLineNumber = this.m_currentLine;
      if (this.SuppressErrors)
        return;
      this.m_currentToken.HandleError(error);
    }

    private JSToken IllegalCharacter()
    {
      this.HandleError(JSError.IllegalChar);
      return JSToken.Error;
    }

    public static JSToken StripAssignment(JSToken assignOp)
    {
      if (JSScanner.IsAssignmentOperator(assignOp))
      {
        int num = (int) (assignOp - 73);
        if (num > 0)
          assignOp = (JSToken) (49 + num - 1);
      }
      return assignOp;
    }

    public static OperatorPrecedence GetOperatorPrecedence(Context op) => op != null && op.Token != JSToken.None ? JSScanner.s_OperatorsPrec[(int) (op.Token - 49)] : OperatorPrecedence.None;

    private static OperatorPrecedence[] InitOperatorsPrec()
    {
      OperatorPrecedence[] operatorPrecedenceArray = new OperatorPrecedence[38];
      operatorPrecedenceArray[0] = OperatorPrecedence.Additive;
      operatorPrecedenceArray[1] = OperatorPrecedence.Additive;
      operatorPrecedenceArray[20] = OperatorPrecedence.LogicalOr;
      operatorPrecedenceArray[19] = OperatorPrecedence.LogicalAnd;
      operatorPrecedenceArray[6] = OperatorPrecedence.BitwiseOr;
      operatorPrecedenceArray[7] = OperatorPrecedence.BitwiseXor;
      operatorPrecedenceArray[5] = OperatorPrecedence.BitwiseAnd;
      operatorPrecedenceArray[11] = OperatorPrecedence.Equality;
      operatorPrecedenceArray[12] = OperatorPrecedence.Equality;
      operatorPrecedenceArray[13] = OperatorPrecedence.Equality;
      operatorPrecedenceArray[14] = OperatorPrecedence.Equality;
      operatorPrecedenceArray[21] = OperatorPrecedence.Relational;
      operatorPrecedenceArray[22] = OperatorPrecedence.Relational;
      operatorPrecedenceArray[17] = OperatorPrecedence.Relational;
      operatorPrecedenceArray[15] = OperatorPrecedence.Relational;
      operatorPrecedenceArray[16] = OperatorPrecedence.Relational;
      operatorPrecedenceArray[18] = OperatorPrecedence.Relational;
      operatorPrecedenceArray[8] = OperatorPrecedence.Shift;
      operatorPrecedenceArray[9] = OperatorPrecedence.Shift;
      operatorPrecedenceArray[10] = OperatorPrecedence.Shift;
      operatorPrecedenceArray[2] = OperatorPrecedence.Multiplicative;
      operatorPrecedenceArray[3] = OperatorPrecedence.Multiplicative;
      operatorPrecedenceArray[4] = OperatorPrecedence.Multiplicative;
      operatorPrecedenceArray[24] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[25] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[26] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[27] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[28] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[30] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[31] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[32] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[29] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[33] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[34] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[35] = OperatorPrecedence.Assignment;
      operatorPrecedenceArray[36] = OperatorPrecedence.Conditional;
      operatorPrecedenceArray[37] = OperatorPrecedence.Conditional;
      operatorPrecedenceArray[23] = OperatorPrecedence.Comma;
      return operatorPrecedenceArray;
    }

    private sealed class PPOperators : SortedDictionary<string, Func<string, string, bool>>
    {
      private PPOperators()
        : base((IComparer<string>) new JSScanner.PPOperators.LengthComparer())
      {
        this.Add("==", new Func<string, string, bool>(JSScanner.PPOperators.PPIsEqual));
        this.Add("!=", new Func<string, string, bool>(JSScanner.PPOperators.PPIsNotEqual));
        this.Add("===", new Func<string, string, bool>(JSScanner.PPOperators.PPIsStrictEqual));
        this.Add("!==", new Func<string, string, bool>(JSScanner.PPOperators.PPIsNotStrictEqual));
        this.Add("<", new Func<string, string, bool>(JSScanner.PPOperators.PPIsLessThan));
        this.Add(">", new Func<string, string, bool>(JSScanner.PPOperators.PPIsGreaterThan));
        this.Add("<=", new Func<string, string, bool>(JSScanner.PPOperators.PPIsLessThanOrEqual));
        this.Add(">=", new Func<string, string, bool>(JSScanner.PPOperators.PPIsGreaterThanOrEqual));
      }

      public static JSScanner.PPOperators Instance => JSScanner.PPOperators.Nested.Instance;

      private static bool PPIsStrictEqual(string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;

      private static bool PPIsNotStrictEqual(string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase) != 0;

      private static bool PPIsEqual(string left, string right)
      {
        bool flag = string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        double leftNumeric;
        double rightNumeric;
        if (!flag && JSScanner.PPOperators.ConvertToNumeric(left, right, out leftNumeric, out rightNumeric))
          flag = leftNumeric == rightNumeric;
        return flag;
      }

      private static bool PPIsNotEqual(string left, string right)
      {
        bool flag = string.Compare(left, right, StringComparison.OrdinalIgnoreCase) != 0;
        double leftNumeric;
        double rightNumeric;
        if (flag && JSScanner.PPOperators.ConvertToNumeric(left, right, out leftNumeric, out rightNumeric))
          flag = leftNumeric != rightNumeric;
        return flag;
      }

      private static bool PPIsLessThan(string left, string right)
      {
        bool flag = false;
        double leftNumeric;
        double rightNumeric;
        if (JSScanner.PPOperators.ConvertToNumeric(left, right, out leftNumeric, out rightNumeric))
          flag = leftNumeric < rightNumeric;
        return flag;
      }

      private static bool PPIsGreaterThan(string left, string right)
      {
        bool flag = false;
        double leftNumeric;
        double rightNumeric;
        if (JSScanner.PPOperators.ConvertToNumeric(left, right, out leftNumeric, out rightNumeric))
          flag = leftNumeric > rightNumeric;
        return flag;
      }

      private static bool PPIsLessThanOrEqual(string left, string right)
      {
        bool flag = false;
        double leftNumeric;
        double rightNumeric;
        if (JSScanner.PPOperators.ConvertToNumeric(left, right, out leftNumeric, out rightNumeric))
          flag = leftNumeric <= rightNumeric;
        return flag;
      }

      private static bool PPIsGreaterThanOrEqual(string left, string right)
      {
        bool flag = false;
        double leftNumeric;
        double rightNumeric;
        if (JSScanner.PPOperators.ConvertToNumeric(left, right, out leftNumeric, out rightNumeric))
          flag = leftNumeric >= rightNumeric;
        return flag;
      }

      private static bool ConvertToNumeric(
        string left,
        string right,
        out double leftNumeric,
        out double rightNumeric)
      {
        rightNumeric = 0.0;
        return double.TryParse(left, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out leftNumeric) && double.TryParse(right, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out rightNumeric);
      }

      private static class Nested
      {
        internal static readonly JSScanner.PPOperators Instance = new JSScanner.PPOperators();
      }

      private class LengthComparer : IComparer<string>
      {
        public int Compare(string x, string y)
        {
          int num = x == null || y == null ? 0 : y.Length - x.Length;
          return num == 0 ? string.CompareOrdinal(x, y) : num;
        }
      }
    }
  }
}
