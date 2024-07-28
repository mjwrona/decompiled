// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Lexer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Antlr4.Runtime
{
  internal abstract class Lexer : Recognizer<int, LexerATNSimulator>, ITokenSource
  {
    public const int DEFAULT_MODE = 0;
    public const int DefaultTokenChannel = 0;
    public const int Hidden = 1;
    public const int MinCharValue = 0;
    public const int MaxCharValue = 1114111;
    private ICharStream _input;
    protected readonly TextWriter Output;
    protected readonly TextWriter ErrorOutput;
    private Tuple<ITokenSource, ICharStream> _tokenFactorySourcePair;
    private ITokenFactory _factory = CommonTokenFactory.Default;
    private IToken _token;
    private int _tokenStartCharIndex = -1;
    private int _tokenStartLine;
    private int _tokenStartColumn;
    private bool _hitEOF;
    private int _channel;
    private int _type;
    private readonly Stack<int> _modeStack = new Stack<int>();
    private int _mode;
    private string _text;

    public Lexer(ICharStream input)
      : this(input, Console.Out, Console.Error)
    {
    }

    public Lexer(ICharStream input, TextWriter output, TextWriter errorOutput)
    {
      this._input = input;
      this.Output = output;
      this.ErrorOutput = errorOutput;
      this._tokenFactorySourcePair = Tuple.Create<ITokenSource, ICharStream>((ITokenSource) this, input);
    }

    public virtual void Reset()
    {
      if (this._input != null)
        this._input.Seek(0);
      this._token = (IToken) null;
      this._type = 0;
      this._channel = 0;
      this._tokenStartCharIndex = -1;
      this._tokenStartColumn = -1;
      this._tokenStartLine = -1;
      this._text = (string) null;
      this._hitEOF = false;
      this._mode = 0;
      this._modeStack.Clear();
      this.Interpreter.Reset();
    }

    public virtual IToken NextToken()
    {
      int marker = this._input != null ? this._input.Mark() : throw new InvalidOperationException("nextToken requires a non-null input stream.");
label_3:
      try
      {
        while (!this._hitEOF)
        {
          this._token = (IToken) null;
          this._channel = 0;
          this._tokenStartCharIndex = this._input.Index;
          this._tokenStartColumn = this.Interpreter.Column;
          this._tokenStartLine = this.Interpreter.Line;
          this._text = (string) null;
          do
          {
            this._type = 0;
            int num;
            try
            {
              num = this.Interpreter.Match(this._input, this._mode);
            }
            catch (LexerNoViableAltException ex)
            {
              this.NotifyListeners(ex);
              this.Recover(ex);
              num = -3;
            }
            if (this._input.LA(1) == -1)
              this._hitEOF = true;
            if (this._type == 0)
              this._type = num;
            if (this._type == -3)
              goto label_3;
          }
          while (this._type == -2);
          if (this._token == null)
            this.Emit();
          return this._token;
        }
        this.EmitEOF();
        return this._token;
      }
      finally
      {
        this._input.Release(marker);
      }
    }

    public virtual void Skip() => this._type = -3;

    public virtual void More() => this._type = -2;

    public virtual void Mode(int m) => this._mode = m;

    public virtual void PushMode(int m)
    {
      this._modeStack.Push(this._mode);
      this.Mode(m);
    }

    public virtual int PopMode()
    {
      if (this._modeStack.Count == 0)
        throw new InvalidOperationException();
      this.Mode(this._modeStack.Pop());
      return this._mode;
    }

    public virtual ITokenFactory TokenFactory
    {
      get => this._factory;
      set => this._factory = value;
    }

    public virtual void SetInputStream(ICharStream input)
    {
      this._input = (ICharStream) null;
      this._tokenFactorySourcePair = Tuple.Create<ITokenSource, ICharStream>((ITokenSource) this, this._input);
      this.Reset();
      this._input = input;
      this._tokenFactorySourcePair = Tuple.Create<ITokenSource, ICharStream>((ITokenSource) this, this._input);
    }

    public virtual string SourceName => this._input.SourceName;

    public override IIntStream InputStream => (IIntStream) this._input;

    ICharStream ITokenSource.InputStream => this._input;

    public virtual void Emit(IToken token) => this._token = token;

    public virtual IToken Emit()
    {
      IToken token = this._factory.Create(this._tokenFactorySourcePair, this._type, this._text, this._channel, this._tokenStartCharIndex, this.CharIndex - 1, this._tokenStartLine, this._tokenStartColumn);
      this.Emit(token);
      return token;
    }

    public virtual IToken EmitEOF()
    {
      IToken token = this._factory.Create(this._tokenFactorySourcePair, -1, (string) null, 0, this._input.Index, this._input.Index - 1, this.Line, this.Column);
      this.Emit(token);
      return token;
    }

    public virtual int Line
    {
      get => this.Interpreter.Line;
      set => this.Interpreter.Line = value;
    }

    public virtual int Column
    {
      get => this.Interpreter.Column;
      set => this.Interpreter.Column = value;
    }

    public virtual int CharIndex => this._input.Index;

    public virtual int TokenStartCharIndex => this._tokenStartCharIndex;

    public virtual int TokenStartLine => this._tokenStartLine;

    public virtual int TokenStartColumn => this._tokenStartColumn;

    public virtual string Text
    {
      get => this._text != null ? this._text : this.Interpreter.GetText(this._input);
      set => this._text = value;
    }

    public virtual IToken Token
    {
      get => this._token;
      set => this._token = value;
    }

    public virtual int Type
    {
      get => this._type;
      set => this._type = value;
    }

    public virtual int Channel
    {
      get => this._channel;
      set => this._channel = value;
    }

    public virtual Stack<int> ModeStack => this._modeStack;

    public virtual int CurrentMode
    {
      get => this._mode;
      set => this._mode = value;
    }

    public virtual bool HitEOF
    {
      get => this._hitEOF;
      set => this._hitEOF = value;
    }

    public virtual string[] ChannelNames => (string[]) null;

    public virtual string[] ModeNames => (string[]) null;

    public virtual IList<IToken> GetAllTokens()
    {
      IList<IToken> allTokens = (IList<IToken>) new List<IToken>();
      for (IToken token = this.NextToken(); token.Type != -1; token = this.NextToken())
        allTokens.Add(token);
      return allTokens;
    }

    public virtual void Recover(LexerNoViableAltException e)
    {
      if (this._input.LA(1) == -1)
        return;
      this.Interpreter.Consume(this._input);
    }

    public virtual void NotifyListeners(LexerNoViableAltException e) => this.ErrorListenerDispatch.SyntaxError(this.ErrorOutput, (IRecognizer) this, 0, this._tokenStartLine, this._tokenStartColumn, "token recognition error at: '" + this.GetErrorDisplay(this._input.GetText(Interval.Of(this._tokenStartCharIndex, this._input.Index))) + "'", (RecognitionException) e);

    public virtual string GetErrorDisplay(string s)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int utf32;
      for (int index = 0; index < s.Length; index += utf32 > (int) ushort.MaxValue ? 2 : 1)
      {
        utf32 = char.ConvertToUtf32(s, index);
        stringBuilder.Append(this.GetErrorDisplay(utf32));
      }
      return stringBuilder.ToString();
    }

    public virtual string GetErrorDisplay(int c)
    {
      string errorDisplay;
      switch (c)
      {
        case -1:
          errorDisplay = "<EOF>";
          break;
        case 9:
          errorDisplay = "\\t";
          break;
        case 10:
          errorDisplay = "\\n";
          break;
        case 13:
          errorDisplay = "\\r";
          break;
        default:
          errorDisplay = char.ConvertFromUtf32(c);
          break;
      }
      return errorDisplay;
    }

    public virtual string GetCharErrorDisplay(int c) => "'" + this.GetErrorDisplay(c) + "'";

    public virtual void Recover(RecognitionException re) => this._input.Consume();
  }
}
