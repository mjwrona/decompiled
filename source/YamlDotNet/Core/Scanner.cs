// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Scanner
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
  [Serializable]
  public class Scanner : IScanner
  {
    private const int MaxVersionNumberLength = 9;
    private const int MaxBufferLength = 8;
    private static readonly IDictionary<char, char> simpleEscapeCodes = (IDictionary<char, char>) new SortedDictionary<char, char>()
    {
      {
        '0',
        char.MinValue
      },
      {
        'a',
        '\a'
      },
      {
        'b',
        '\b'
      },
      {
        't',
        '\t'
      },
      {
        '\t',
        '\t'
      },
      {
        'n',
        '\n'
      },
      {
        'v',
        '\v'
      },
      {
        'f',
        '\f'
      },
      {
        'r',
        '\r'
      },
      {
        'e',
        '\u001B'
      },
      {
        ' ',
        ' '
      },
      {
        '"',
        '"'
      },
      {
        '\'',
        '\''
      },
      {
        '\\',
        '\\'
      },
      {
        'N',
        '\u0085'
      },
      {
        '_',
        ' '
      },
      {
        'L',
        '\u2028'
      },
      {
        'P',
        '\u2029'
      }
    };
    private readonly Stack<int> indents = new Stack<int>();
    private readonly InsertionQueue<Token> tokens = new InsertionQueue<Token>();
    private readonly Stack<SimpleKey> simpleKeys = new Stack<SimpleKey>();
    private readonly CharacterAnalyzer<LookAheadBuffer> analyzer;
    private readonly Cursor cursor;
    private bool streamStartProduced;
    private bool streamEndProduced;
    private int indent = -1;
    private bool simpleKeyAllowed;
    private int flowLevel;
    private int tokensParsed;
    private bool tokenAvailable;
    private Token previous;

    public bool SkipComments { get; private set; }

    public Token Current { get; private set; }

    public Scanner(TextReader input, bool skipComments = true)
    {
      this.analyzer = new CharacterAnalyzer<LookAheadBuffer>(new LookAheadBuffer(input, 8));
      this.cursor = new Cursor();
      this.SkipComments = skipComments;
    }

    public Mark CurrentPosition => this.cursor.Mark();

    public bool MoveNext()
    {
      if (this.Current != null)
        this.ConsumeCurrent();
      return this.MoveNextWithoutConsuming();
    }

    public bool MoveNextWithoutConsuming()
    {
      if (!this.tokenAvailable && !this.streamEndProduced)
        this.FetchMoreTokens();
      if (this.tokens.Count > 0)
      {
        this.Current = this.tokens.Dequeue();
        this.tokenAvailable = false;
        return true;
      }
      this.Current = (Token) null;
      return false;
    }

    public void ConsumeCurrent()
    {
      ++this.tokensParsed;
      this.tokenAvailable = false;
      this.previous = this.Current;
      this.Current = (Token) null;
    }

    private char ReadCurrentCharacter()
    {
      int num = (int) this.analyzer.Peek(0);
      this.Skip();
      return (char) num;
    }

    private char ReadLine()
    {
      if (this.analyzer.Check("\r\n\u0085"))
      {
        this.SkipLine();
        return '\n';
      }
      int num = (int) this.analyzer.Peek(0);
      this.SkipLine();
      return (char) num;
    }

    private void FetchMoreTokens()
    {
      while (true)
      {
        bool flag = false;
        if (this.tokens.Count == 0)
        {
          flag = true;
        }
        else
        {
          this.StaleSimpleKeys();
          foreach (SimpleKey simpleKey in this.simpleKeys)
          {
            if (simpleKey.IsPossible && simpleKey.TokenNumber == this.tokensParsed)
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          this.FetchNextToken();
        else
          break;
      }
      this.tokenAvailable = true;
    }

    private static bool StartsWith(StringBuilder what, char start) => what.Length > 0 && (int) what[0] == (int) start;

    private void StaleSimpleKeys()
    {
      foreach (SimpleKey simpleKey in this.simpleKeys)
      {
        if (simpleKey.IsPossible && (simpleKey.Line < this.cursor.Line || simpleKey.Index + 1024 < this.cursor.Index))
        {
          if (simpleKey.IsRequired)
          {
            Mark mark = this.cursor.Mark();
            throw new SyntaxErrorException(mark, mark, "While scanning a simple key, could not find expected ':'.");
          }
          simpleKey.IsPossible = false;
        }
      }
    }

    private void FetchNextToken()
    {
      if (!this.streamStartProduced)
      {
        this.FetchStreamStart();
      }
      else
      {
        this.ScanToNextToken();
        this.StaleSimpleKeys();
        this.UnrollIndent(this.cursor.LineOffset);
        this.analyzer.Buffer.Cache(4);
        if (this.analyzer.Buffer.EndOfInput)
          this.FetchStreamEnd();
        else if (this.cursor.LineOffset == 0 && this.analyzer.Check('%'))
          this.FetchDirective();
        else if ((this.cursor.LineOffset != 0 || !this.analyzer.Check('-') || !this.analyzer.Check('-', 1) || !this.analyzer.Check('-', 2) ? 0 : (this.analyzer.IsWhiteBreakOrZero(3) ? 1 : 0)) != 0)
          this.FetchDocumentIndicator(true);
        else if ((this.cursor.LineOffset != 0 || !this.analyzer.Check('.') || !this.analyzer.Check('.', 1) || !this.analyzer.Check('.', 2) ? 0 : (this.analyzer.IsWhiteBreakOrZero(3) ? 1 : 0)) != 0)
          this.FetchDocumentIndicator(false);
        else if (this.analyzer.Check('['))
          this.FetchFlowCollectionStart(true);
        else if (this.analyzer.Check('{'))
          this.FetchFlowCollectionStart(false);
        else if (this.analyzer.Check(']'))
          this.FetchFlowCollectionEnd(true);
        else if (this.analyzer.Check('}'))
          this.FetchFlowCollectionEnd(false);
        else if (this.analyzer.Check(','))
          this.FetchFlowEntry();
        else if (this.analyzer.Check('-') && this.analyzer.IsWhiteBreakOrZero(1))
          this.FetchBlockEntry();
        else if (this.analyzer.Check('?') && (this.flowLevel > 0 || this.analyzer.IsWhiteBreakOrZero(1)))
          this.FetchKey();
        else if (this.analyzer.Check(':') && (this.flowLevel > 0 || this.analyzer.IsWhiteBreakOrZero(1)))
          this.FetchValue();
        else if (this.analyzer.Check('*'))
          this.FetchAnchor(true);
        else if (this.analyzer.Check('&'))
          this.FetchAnchor(false);
        else if (this.analyzer.Check('!'))
          this.FetchTag();
        else if (this.analyzer.Check('|') && this.flowLevel == 0)
          this.FetchBlockScalar(true);
        else if (this.analyzer.Check('>') && this.flowLevel == 0)
          this.FetchBlockScalar(false);
        else if (this.analyzer.Check('\''))
          this.FetchFlowScalar(true);
        else if (this.analyzer.Check('"'))
          this.FetchFlowScalar(false);
        else if (((this.analyzer.IsWhiteBreakOrZero() ? 1 : (this.analyzer.Check("-?:,[]{}#&*!|>'\"%@`") ? 1 : 0)) == 0 || this.analyzer.Check('-') && !this.analyzer.IsWhite(1) ? 1 : (this.flowLevel != 0 || !this.analyzer.Check("?:") ? 0 : (!this.analyzer.IsWhiteBreakOrZero(1) ? 1 : 0))) != 0)
        {
          this.FetchPlainScalar();
        }
        else
        {
          Mark start = this.cursor.Mark();
          this.Skip();
          Mark end = this.cursor.Mark();
          throw new SyntaxErrorException(start, end, "While scanning for the next token, find character that cannot start any token.");
        }
      }
    }

    private bool CheckWhiteSpace()
    {
      if (this.analyzer.Check(' '))
        return true;
      return (this.flowLevel > 0 || !this.simpleKeyAllowed) && this.analyzer.Check('\t');
    }

    private bool IsDocumentIndicator() => this.cursor.LineOffset == 0 && this.analyzer.IsWhiteBreakOrZero(3) && ((!this.analyzer.Check('-') || !this.analyzer.Check('-', 1) ? 0 : (this.analyzer.Check('-', 2) ? 1 : 0)) | (!this.analyzer.Check('.') || !this.analyzer.Check('.', 1) ? (false ? 1 : 0) : (this.analyzer.Check('.', 2) ? 1 : 0))) != 0;

    private void Skip()
    {
      this.cursor.Skip();
      this.analyzer.Buffer.Skip(1);
    }

    private void SkipLine()
    {
      if (this.analyzer.IsCrLf())
      {
        this.cursor.SkipLineByOffset(2);
        this.analyzer.Buffer.Skip(2);
      }
      else if (this.analyzer.IsBreak())
      {
        this.cursor.SkipLineByOffset(1);
        this.analyzer.Buffer.Skip(1);
      }
      else if (!this.analyzer.IsZero())
        throw new InvalidOperationException("Not at a break.");
    }

    private void ScanToNextToken()
    {
      while (true)
      {
        do
        {
          while (this.CheckWhiteSpace())
            this.Skip();
          this.ProcessComment();
          if (this.analyzer.IsBreak())
            this.SkipLine();
          else
            goto label_6;
        }
        while (this.flowLevel != 0);
        this.simpleKeyAllowed = true;
      }
label_6:;
    }

    private void ProcessComment()
    {
      if (!this.analyzer.Check('#'))
        return;
      Mark start = this.cursor.Mark();
      this.Skip();
      while (this.analyzer.IsSpace())
        this.Skip();
      StringBuilder stringBuilder = new StringBuilder();
      while (!this.analyzer.IsBreakOrZero())
        stringBuilder.Append(this.ReadCurrentCharacter());
      if (this.SkipComments)
        return;
      bool isInline = this.previous != null && this.previous.End.Line == start.Line && this.previous.End.Column != 1 && !(this.previous is StreamStart);
      this.tokens.Enqueue((Token) new Comment(stringBuilder.ToString(), isInline, start, this.cursor.Mark()));
    }

    private void FetchStreamStart()
    {
      this.simpleKeys.Push(new SimpleKey());
      this.simpleKeyAllowed = true;
      this.streamStartProduced = true;
      Mark mark = this.cursor.Mark();
      this.tokens.Enqueue((Token) new StreamStart(mark, mark));
    }

    private void UnrollIndent(int column)
    {
      if (this.flowLevel != 0)
        return;
      for (; this.indent > column; this.indent = this.indents.Pop())
      {
        Mark mark = this.cursor.Mark();
        this.tokens.Enqueue((Token) new BlockEnd(mark, mark));
      }
    }

    private void FetchStreamEnd()
    {
      this.cursor.ForceSkipLineAfterNonBreak();
      this.UnrollIndent(-1);
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = false;
      this.streamEndProduced = true;
      Mark mark = this.cursor.Mark();
      this.tokens.Enqueue((Token) new StreamEnd(mark, mark));
    }

    private void FetchDirective()
    {
      this.UnrollIndent(-1);
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = false;
      this.tokens.Enqueue(this.ScanDirective());
    }

    private Token ScanDirective()
    {
      Mark start = this.cursor.Mark();
      this.Skip();
      Token token;
      switch (this.ScanDirectiveName(start))
      {
        case "YAML":
          token = this.ScanVersionDirectiveValue(start);
          break;
        case "TAG":
          token = this.ScanTagDirectiveValue(start);
          break;
        default:
          throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a directive, find uknown directive name.");
      }
      while (this.analyzer.IsWhite())
        this.Skip();
      this.ProcessComment();
      if (!this.analyzer.IsBreakOrZero())
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a directive, did not find expected comment or line break.");
      if (this.analyzer.IsBreak())
        this.SkipLine();
      return token;
    }

    private void FetchDocumentIndicator(bool isStartToken)
    {
      this.UnrollIndent(-1);
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = false;
      Mark mark = this.cursor.Mark();
      this.Skip();
      this.Skip();
      this.Skip();
      this.tokens.Enqueue(isStartToken ? (Token) new DocumentStart(mark, this.cursor.Mark()) : (Token) new DocumentEnd(mark, mark));
    }

    private void FetchFlowCollectionStart(bool isSequenceToken)
    {
      this.SaveSimpleKey();
      this.IncreaseFlowLevel();
      this.simpleKeyAllowed = true;
      Mark mark = this.cursor.Mark();
      this.Skip();
      this.tokens.Enqueue(!isSequenceToken ? (Token) new FlowMappingStart(mark, mark) : (Token) new FlowSequenceStart(mark, mark));
    }

    private void IncreaseFlowLevel()
    {
      this.simpleKeys.Push(new SimpleKey());
      ++this.flowLevel;
    }

    private void FetchFlowCollectionEnd(bool isSequenceToken)
    {
      this.RemoveSimpleKey();
      this.DecreaseFlowLevel();
      this.simpleKeyAllowed = false;
      Mark mark = this.cursor.Mark();
      this.Skip();
      this.tokens.Enqueue(!isSequenceToken ? (Token) new FlowMappingEnd(mark, mark) : (Token) new FlowSequenceEnd(mark, mark));
    }

    private void DecreaseFlowLevel()
    {
      if (this.flowLevel <= 0)
        return;
      --this.flowLevel;
      this.simpleKeys.Pop();
    }

    private void FetchFlowEntry()
    {
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = true;
      Mark start = this.cursor.Mark();
      this.Skip();
      this.tokens.Enqueue((Token) new FlowEntry(start, this.cursor.Mark()));
    }

    private void FetchBlockEntry()
    {
      if (this.flowLevel == 0)
      {
        if (!this.simpleKeyAllowed)
        {
          Mark mark = this.cursor.Mark();
          throw new SyntaxErrorException(mark, mark, "Block sequence entries are not allowed in this context.");
        }
        this.RollIndent(this.cursor.LineOffset, -1, true, this.cursor.Mark());
      }
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = true;
      Mark start = this.cursor.Mark();
      this.Skip();
      this.tokens.Enqueue((Token) new BlockEntry(start, this.cursor.Mark()));
    }

    private void FetchKey()
    {
      if (this.flowLevel == 0)
      {
        if (!this.simpleKeyAllowed)
        {
          Mark mark = this.cursor.Mark();
          throw new SyntaxErrorException(mark, mark, "Mapping keys are not allowed in this context.");
        }
        this.RollIndent(this.cursor.LineOffset, -1, false, this.cursor.Mark());
      }
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = this.flowLevel == 0;
      Mark start = this.cursor.Mark();
      this.Skip();
      this.tokens.Enqueue((Token) new Key(start, this.cursor.Mark()));
    }

    private void FetchValue()
    {
      SimpleKey simpleKey = this.simpleKeys.Peek();
      if (simpleKey.IsPossible)
      {
        this.tokens.Insert(simpleKey.TokenNumber - this.tokensParsed, (Token) new Key(simpleKey.Mark, simpleKey.Mark));
        this.RollIndent(simpleKey.LineOffset, simpleKey.TokenNumber, false, simpleKey.Mark);
        simpleKey.IsPossible = false;
        this.simpleKeyAllowed = false;
      }
      else
      {
        if (this.flowLevel == 0)
        {
          if (!this.simpleKeyAllowed)
          {
            Mark mark = this.cursor.Mark();
            throw new SyntaxErrorException(mark, mark, "Mapping values are not allowed in this context.");
          }
          this.RollIndent(this.cursor.LineOffset, -1, false, this.cursor.Mark());
        }
        this.simpleKeyAllowed = this.flowLevel == 0;
      }
      Mark start = this.cursor.Mark();
      this.Skip();
      this.tokens.Enqueue((Token) new Value(start, this.cursor.Mark()));
    }

    private void RollIndent(int column, int number, bool isSequence, Mark position)
    {
      if (this.flowLevel > 0 || this.indent >= column)
        return;
      this.indents.Push(this.indent);
      this.indent = column;
      Token token = !isSequence ? (Token) new BlockMappingStart(position, position) : (Token) new BlockSequenceStart(position, position);
      if (number == -1)
        this.tokens.Enqueue(token);
      else
        this.tokens.Insert(number - this.tokensParsed, token);
    }

    private void FetchAnchor(bool isAlias)
    {
      this.SaveSimpleKey();
      this.simpleKeyAllowed = false;
      this.tokens.Enqueue(this.ScanAnchor(isAlias));
    }

    private Token ScanAnchor(bool isAlias)
    {
      Mark start = this.cursor.Mark();
      this.Skip();
      StringBuilder stringBuilder = new StringBuilder();
      while (this.analyzer.IsAlphaNumericDashOrUnderscore())
        stringBuilder.Append(this.ReadCurrentCharacter());
      if (stringBuilder.Length == 0 || !this.analyzer.IsWhiteBreakOrZero() && !this.analyzer.Check("?:,]}%@`"))
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning an anchor or alias, did not find expected alphabetic or numeric character.");
      return isAlias ? (Token) new AnchorAlias(stringBuilder.ToString(), start, this.cursor.Mark()) : (Token) new Anchor(stringBuilder.ToString(), start, this.cursor.Mark());
    }

    private void FetchTag()
    {
      this.SaveSimpleKey();
      this.simpleKeyAllowed = false;
      this.tokens.Enqueue(this.ScanTag());
    }

    private Token ScanTag()
    {
      Mark start = this.cursor.Mark();
      string handle;
      string suffix;
      if (this.analyzer.Check('<', 1))
      {
        handle = string.Empty;
        this.Skip();
        this.Skip();
        suffix = this.ScanTagUri((string) null, start);
        if (!this.analyzer.Check('>'))
          throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a tag, did not find the expected '>'.");
        this.Skip();
      }
      else
      {
        string head = this.ScanTagHandle(false, start);
        if (head.Length > 1 && head[0] == '!' && head[head.Length - 1] == '!')
        {
          handle = head;
          suffix = this.ScanTagUri((string) null, start);
        }
        else
        {
          suffix = this.ScanTagUri(head, start);
          handle = "!";
          if (suffix.Length == 0)
          {
            suffix = handle;
            handle = string.Empty;
          }
        }
      }
      if (!this.analyzer.IsWhiteBreakOrZero())
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a tag, did not find expected whitespace or line break.");
      return (Token) new Tag(handle, suffix, start, this.cursor.Mark());
    }

    private void FetchBlockScalar(bool isLiteral)
    {
      this.RemoveSimpleKey();
      this.simpleKeyAllowed = true;
      this.tokens.Enqueue(this.ScanBlockScalar(isLiteral));
    }

    private Token ScanBlockScalar(bool isLiteral)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder what = new StringBuilder();
      StringBuilder breaks = new StringBuilder();
      int num1 = 0;
      int num2 = 0;
      int currentIndent1 = 0;
      bool flag1 = false;
      Mark start = this.cursor.Mark();
      this.Skip();
      if (this.analyzer.Check("+-"))
      {
        num1 = this.analyzer.Check('+') ? 1 : -1;
        this.Skip();
        if (this.analyzer.IsDigit())
        {
          num2 = !this.analyzer.Check('0') ? this.analyzer.AsDigit() : throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a block scalar, find an indentation indicator equal to 0.");
          this.Skip();
        }
      }
      else if (this.analyzer.IsDigit())
      {
        num2 = !this.analyzer.Check('0') ? this.analyzer.AsDigit() : throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a block scalar, find an indentation indicator equal to 0.");
        this.Skip();
        if (this.analyzer.Check("+-"))
        {
          num1 = this.analyzer.Check('+') ? 1 : -1;
          this.Skip();
        }
      }
      while (this.analyzer.IsWhite())
        this.Skip();
      this.ProcessComment();
      if (!this.analyzer.IsBreakOrZero())
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a block scalar, did not find expected comment or line break.");
      if (this.analyzer.IsBreak())
        this.SkipLine();
      Mark end = this.cursor.Mark();
      if (num2 != 0)
        currentIndent1 = this.indent >= 0 ? this.indent + num2 : num2;
      for (int currentIndent2 = this.ScanBlockScalarBreaks(currentIndent1, breaks, start, ref end); this.cursor.LineOffset == currentIndent2 && !this.analyzer.IsZero(); currentIndent2 = this.ScanBlockScalarBreaks(currentIndent2, breaks, start, ref end))
      {
        bool flag2 = this.analyzer.IsWhite();
        if (!isLiteral && Scanner.StartsWith(what, '\n') && !flag1 && !flag2)
        {
          if (breaks.Length == 0)
            stringBuilder.Append(' ');
          what.Length = 0;
        }
        else
        {
          stringBuilder.Append(what.ToString());
          what.Length = 0;
        }
        stringBuilder.Append(breaks.ToString());
        breaks.Length = 0;
        flag1 = this.analyzer.IsWhite();
        while (!this.analyzer.IsBreakOrZero())
          stringBuilder.Append(this.ReadCurrentCharacter());
        char ch = this.ReadLine();
        if (ch != char.MinValue)
          what.Append(ch);
      }
      if (num1 != -1)
        stringBuilder.Append((object) what);
      if (num1 == 1)
        stringBuilder.Append((object) breaks);
      ScalarStyle style = isLiteral ? ScalarStyle.Literal : ScalarStyle.Folded;
      return (Token) new Scalar(stringBuilder.ToString(), style, start, end);
    }

    private int ScanBlockScalarBreaks(
      int currentIndent,
      StringBuilder breaks,
      Mark start,
      ref Mark end)
    {
      int val1 = 0;
      end = this.cursor.Mark();
      while (true)
      {
        while (currentIndent != 0 && this.cursor.LineOffset >= currentIndent || !this.analyzer.IsSpace())
        {
          if (this.cursor.LineOffset > val1)
            val1 = this.cursor.LineOffset;
          if ((currentIndent == 0 || this.cursor.LineOffset < currentIndent) && this.analyzer.IsTab())
            throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a block scalar, find a tab character where an indentation space is expected.");
          if (this.analyzer.IsBreak())
          {
            breaks.Append(this.ReadLine());
            end = this.cursor.Mark();
          }
          else
          {
            if (currentIndent == 0)
              currentIndent = Math.Max(val1, Math.Max(this.indent + 1, 1));
            return currentIndent;
          }
        }
        this.Skip();
      }
    }

    private void FetchFlowScalar(bool isSingleQuoted)
    {
      this.SaveSimpleKey();
      this.simpleKeyAllowed = false;
      this.tokens.Enqueue(this.ScanFlowScalar(isSingleQuoted));
    }

    private Token ScanFlowScalar(bool isSingleQuoted)
    {
      Mark start = this.cursor.Mark();
      this.Skip();
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      StringBuilder what = new StringBuilder();
      StringBuilder stringBuilder3 = new StringBuilder();
      while (!this.IsDocumentIndicator())
      {
        if (this.analyzer.IsZero())
          throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a quoted scalar, find unexpected end of stream.");
        bool flag = false;
        while (!this.analyzer.IsWhiteBreakOrZero())
        {
          if (isSingleQuoted && this.analyzer.Check('\'') && this.analyzer.Check('\'', 1))
          {
            stringBuilder1.Append('\'');
            this.Skip();
            this.Skip();
          }
          else if (!this.analyzer.Check(isSingleQuoted ? '\'' : '"'))
          {
            if (!isSingleQuoted && this.analyzer.Check('\\') && this.analyzer.IsBreak(1))
            {
              this.Skip();
              this.SkipLine();
              flag = true;
              break;
            }
            if (!isSingleQuoted && this.analyzer.Check('\\'))
            {
              int num = 0;
              char key = this.analyzer.Peek(1);
              switch (key)
              {
                case 'U':
                  num = 8;
                  break;
                case 'u':
                  num = 4;
                  break;
                case 'x':
                  num = 2;
                  break;
                default:
                  char ch;
                  if (!Scanner.simpleEscapeCodes.TryGetValue(key, out ch))
                    throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a quoted scalar, find unknown escape character.");
                  stringBuilder1.Append(ch);
                  break;
              }
              this.Skip();
              this.Skip();
              if (num > 0)
              {
                int utf32 = 0;
                for (int offset = 0; offset < num; ++offset)
                {
                  if (!this.analyzer.IsHex(offset))
                    throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a quoted scalar, did not find expected hexdecimal number.");
                  utf32 = (utf32 << 4) + this.analyzer.AsHex(offset);
                }
                if (utf32 >= 55296 && utf32 <= 57343 || utf32 > 1114111)
                  throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a quoted scalar, find invalid Unicode character escape code.");
                stringBuilder1.Append(char.ConvertFromUtf32(utf32));
                for (int index = 0; index < num; ++index)
                  this.Skip();
              }
            }
            else
              stringBuilder1.Append(this.ReadCurrentCharacter());
          }
          else
            break;
        }
        if (!this.analyzer.Check(isSingleQuoted ? '\'' : '"'))
        {
          while (this.analyzer.IsWhite() || this.analyzer.IsBreak())
          {
            if (this.analyzer.IsWhite())
            {
              if (!flag)
                stringBuilder2.Append(this.ReadCurrentCharacter());
              else
                this.Skip();
            }
            else if (!flag)
            {
              stringBuilder2.Length = 0;
              what.Append(this.ReadLine());
              flag = true;
            }
            else
              stringBuilder3.Append(this.ReadLine());
          }
          if (flag)
          {
            if (Scanner.StartsWith(what, '\n'))
            {
              if (stringBuilder3.Length == 0)
                stringBuilder1.Append(' ');
              else
                stringBuilder1.Append(stringBuilder3.ToString());
            }
            else
            {
              stringBuilder1.Append(what.ToString());
              stringBuilder1.Append(stringBuilder3.ToString());
            }
            what.Length = 0;
            stringBuilder3.Length = 0;
          }
          else
          {
            stringBuilder1.Append(stringBuilder2.ToString());
            stringBuilder2.Length = 0;
          }
        }
        else
        {
          this.Skip();
          return (Token) new Scalar(stringBuilder1.ToString(), isSingleQuoted ? ScalarStyle.SingleQuoted : ScalarStyle.DoubleQuoted, start, this.cursor.Mark());
        }
      }
      throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a quoted scalar, find unexpected document indicator.");
    }

    private void FetchPlainScalar()
    {
      this.SaveSimpleKey();
      this.simpleKeyAllowed = false;
      this.tokens.Enqueue(this.ScanPlainScalar());
    }

    private Token ScanPlainScalar()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      StringBuilder what = new StringBuilder();
      StringBuilder stringBuilder3 = new StringBuilder();
      bool flag = false;
      int num = this.indent + 1;
      Mark start = this.cursor.Mark();
      Mark end = start;
      while (!this.IsDocumentIndicator() && !this.analyzer.Check('#'))
      {
        while (!this.analyzer.IsWhiteBreakOrZero())
        {
          if (this.flowLevel > 0 && this.analyzer.Check(':') && !this.analyzer.IsWhiteBreakOrZero(1))
            throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a plain scalar, find unexpected ':'.");
          if ((!this.analyzer.Check(':') || !this.analyzer.IsWhiteBreakOrZero(1)) && (this.flowLevel <= 0 || !this.analyzer.Check(",:?[]{}")))
          {
            if (flag || stringBuilder2.Length > 0)
            {
              if (flag)
              {
                if (Scanner.StartsWith(what, '\n'))
                {
                  if (stringBuilder3.Length == 0)
                    stringBuilder1.Append(' ');
                  else
                    stringBuilder1.Append((object) stringBuilder3);
                }
                else
                {
                  stringBuilder1.Append((object) what);
                  stringBuilder1.Append((object) stringBuilder3);
                }
                what.Length = 0;
                stringBuilder3.Length = 0;
                flag = false;
              }
              else
              {
                stringBuilder1.Append((object) stringBuilder2);
                stringBuilder2.Length = 0;
              }
            }
            stringBuilder1.Append(this.ReadCurrentCharacter());
            end = this.cursor.Mark();
          }
          else
            break;
        }
        if (this.analyzer.IsWhite() || this.analyzer.IsBreak())
        {
          while (this.analyzer.IsWhite() || this.analyzer.IsBreak())
          {
            if (this.analyzer.IsWhite())
            {
              if (flag && this.cursor.LineOffset < num && this.analyzer.IsTab())
                throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a plain scalar, find a tab character that violate indentation.");
              if (!flag)
                stringBuilder2.Append(this.ReadCurrentCharacter());
              else
                this.Skip();
            }
            else if (!flag)
            {
              stringBuilder2.Length = 0;
              what.Append(this.ReadLine());
              flag = true;
            }
            else
              stringBuilder3.Append(this.ReadLine());
          }
          if (this.flowLevel == 0 && this.cursor.LineOffset < num)
            break;
        }
        else
          break;
      }
      if (flag)
        this.simpleKeyAllowed = true;
      return (Token) new Scalar(stringBuilder1.ToString(), ScalarStyle.Plain, start, end);
    }

    private void RemoveSimpleKey()
    {
      SimpleKey simpleKey = this.simpleKeys.Peek();
      if (simpleKey.IsPossible && simpleKey.IsRequired)
        throw new SyntaxErrorException(simpleKey.Mark, simpleKey.Mark, "While scanning a simple key, could not find expected ':'.");
      simpleKey.IsPossible = false;
    }

    private string ScanDirectiveName(Mark start)
    {
      StringBuilder stringBuilder = new StringBuilder();
      while (this.analyzer.IsAlphaNumericDashOrUnderscore())
        stringBuilder.Append(this.ReadCurrentCharacter());
      if (stringBuilder.Length == 0)
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a directive, could not find expected directive name.");
      if (!this.analyzer.IsWhiteBreakOrZero())
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a directive, find unexpected non-alphabetical character.");
      return stringBuilder.ToString();
    }

    private void SkipWhitespaces()
    {
      while (this.analyzer.IsWhite())
        this.Skip();
    }

    private Token ScanVersionDirectiveValue(Mark start)
    {
      this.SkipWhitespaces();
      int major = this.ScanVersionDirectiveNumber(start);
      if (!this.analyzer.Check('.'))
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %YAML directive, did not find expected digit or '.' character.");
      this.Skip();
      int minor = this.ScanVersionDirectiveNumber(start);
      return (Token) new VersionDirective(new Version(major, minor), start, start);
    }

    private Token ScanTagDirectiveValue(Mark start)
    {
      this.SkipWhitespaces();
      string handle = this.ScanTagHandle(true, start);
      if (!this.analyzer.IsWhite())
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %TAG directive, did not find expected whitespace.");
      this.SkipWhitespaces();
      string str = this.ScanTagUri((string) null, start);
      if (!this.analyzer.IsWhiteBreakOrZero())
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %TAG directive, did not find expected whitespace or line break.");
      string prefix = str;
      Mark start1 = start;
      Mark end = start;
      return (Token) new TagDirective(handle, prefix, start1, end);
    }

    private string ScanTagUri(string head, Mark start)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (head != null && head.Length > 1)
        stringBuilder.Append(head.Substring(1));
      while (this.analyzer.IsAlphaNumericDashOrUnderscore() || this.analyzer.Check(";/?:@&=+$,.!~*'()[]%"))
      {
        if (this.analyzer.Check('%'))
          stringBuilder.Append(this.ScanUriEscapes(start));
        else if (this.analyzer.Check('+'))
        {
          stringBuilder.Append(' ');
          this.Skip();
        }
        else
          stringBuilder.Append(this.ReadCurrentCharacter());
      }
      return stringBuilder.Length != 0 ? stringBuilder.ToString() : throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, did not find expected tag URI.");
    }

    private string ScanUriEscapes(Mark start)
    {
      byte[] bytes = (byte[]) null;
      int count = 0;
      int length = 0;
      while (this.analyzer.Check('%') && this.analyzer.IsHex(1) && this.analyzer.IsHex(2))
      {
        int num = (this.analyzer.AsHex(1) << 4) + this.analyzer.AsHex(2);
        if (length == 0)
        {
          length = (num & 128) == 0 ? 1 : ((num & 224) == 192 ? 2 : ((num & 240) == 224 ? 3 : ((num & 248) == 240 ? 4 : 0)));
          bytes = length != 0 ? new byte[length] : throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, find an incorrect leading UTF-8 octet.");
        }
        else if ((num & 192) != 128)
          throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, find an incorrect trailing UTF-8 octet.");
        bytes[count++] = (byte) num;
        this.Skip();
        this.Skip();
        this.Skip();
        if (--length <= 0)
        {
          string str = Encoding.UTF8.GetString(bytes, 0, count);
          return str.Length != 0 && str.Length <= 2 ? str : throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, find an incorrect UTF-8 sequence.");
        }
      }
      throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag, did not find URI escaped octet.");
    }

    private string ScanTagHandle(bool isDirective, Mark start)
    {
      if (!this.analyzer.Check('!'))
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a tag, did not find expected '!'.");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.ReadCurrentCharacter());
      while (this.analyzer.IsAlphaNumericDashOrUnderscore())
        stringBuilder.Append(this.ReadCurrentCharacter());
      if (this.analyzer.Check('!'))
        stringBuilder.Append(this.ReadCurrentCharacter());
      else if (isDirective && (stringBuilder.Length != 1 || stringBuilder[0] != '!'))
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While parsing a tag directive, did not find expected '!'.");
      return stringBuilder.ToString();
    }

    private int ScanVersionDirectiveNumber(Mark start)
    {
      int num1 = 0;
      int num2 = 0;
      while (this.analyzer.IsDigit())
      {
        if (++num2 > 9)
          throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %YAML directive, find extremely long version number.");
        num1 = num1 * 10 + this.analyzer.AsDigit();
        this.Skip();
      }
      if (num2 == 0)
        throw new SyntaxErrorException(start, this.cursor.Mark(), "While scanning a %YAML directive, did not find expected version number.");
      return num1;
    }

    private void SaveSimpleKey()
    {
      bool isRequired = this.flowLevel == 0 && this.indent == this.cursor.LineOffset;
      if (!this.simpleKeyAllowed)
        return;
      SimpleKey simpleKey = new SimpleKey(true, isRequired, this.tokensParsed + this.tokens.Count, this.cursor);
      this.RemoveSimpleKey();
      this.simpleKeys.Pop();
      this.simpleKeys.Push(simpleKey);
    }
  }
}
