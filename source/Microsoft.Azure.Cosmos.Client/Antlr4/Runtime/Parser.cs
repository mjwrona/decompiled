// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Parser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Pattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Antlr4.Runtime
{
  internal abstract class Parser : Recognizer<IToken, ParserATNSimulator>
  {
    private static readonly IDictionary<string, ATN> bypassAltsAtnCache = (IDictionary<string, ATN>) new Dictionary<string, ATN>();
    [NotNull]
    private IAntlrErrorStrategy _errHandler = (IAntlrErrorStrategy) new DefaultErrorStrategy();
    private ITokenStream _input;
    private readonly List<int> _precedenceStack = new List<int>()
    {
      0
    };
    private ParserRuleContext _ctx;
    private bool _buildParseTrees = true;
    private Parser.TraceListener _tracer;
    [Nullable]
    private IList<IParseTreeListener> _parseListeners;
    private int _syntaxErrors;
    protected readonly TextWriter Output;
    protected readonly TextWriter ErrorOutput;

    public Parser(ITokenStream input)
      : this(input, Console.Out, Console.Error)
    {
    }

    public Parser(ITokenStream input, TextWriter output, TextWriter errorOutput)
    {
      this.TokenStream = input;
      this.Output = output;
      this.ErrorOutput = errorOutput;
    }

    public virtual void Reset()
    {
      if ((ITokenStream) this.InputStream != null)
        this.InputStream.Seek(0);
      this._errHandler.Reset(this);
      this._ctx = (ParserRuleContext) null;
      this._syntaxErrors = 0;
      this.Trace = false;
      this._precedenceStack.Clear();
      this._precedenceStack.Add(0);
      this.Interpreter?.Reset();
    }

    [return: NotNull]
    public virtual IToken Match(int ttype)
    {
      IToken badToken = this.CurrentToken;
      if (badToken.Type == ttype)
      {
        this._errHandler.ReportMatch(this);
        this.Consume();
      }
      else
      {
        badToken = this._errHandler.RecoverInline(this);
        if (this._buildParseTrees && badToken.TokenIndex == -1)
          this._ctx.AddErrorNode(badToken);
      }
      return badToken;
    }

    [return: NotNull]
    public virtual IToken MatchWildcard()
    {
      IToken badToken = this.CurrentToken;
      if (badToken.Type > 0)
      {
        this._errHandler.ReportMatch(this);
        this.Consume();
      }
      else
      {
        badToken = this._errHandler.RecoverInline(this);
        if (this._buildParseTrees && badToken.TokenIndex == -1)
          this._ctx.AddErrorNode(badToken);
      }
      return badToken;
    }

    public virtual bool BuildParseTree
    {
      get => this._buildParseTrees;
      set => this._buildParseTrees = value;
    }

    public virtual bool TrimParseTree
    {
      get => this.ParseListeners.Contains((IParseTreeListener) Parser.TrimToSizeListener.Instance);
      set
      {
        if (value)
        {
          if (this.TrimParseTree)
            return;
          this.AddParseListener((IParseTreeListener) Parser.TrimToSizeListener.Instance);
        }
        else
          this.RemoveParseListener((IParseTreeListener) Parser.TrimToSizeListener.Instance);
      }
    }

    public virtual IList<IParseTreeListener> ParseListeners => this._parseListeners ?? (IList<IParseTreeListener>) Antlr4.Runtime.Sharpen.Collections.EmptyList<IParseTreeListener>();

    public virtual void AddParseListener(IParseTreeListener listener)
    {
      if (listener == null)
        throw new ArgumentNullException(nameof (listener));
      if (this._parseListeners == null)
        this._parseListeners = (IList<IParseTreeListener>) new List<IParseTreeListener>();
      this._parseListeners.Add(listener);
    }

    public virtual void RemoveParseListener(IParseTreeListener listener)
    {
      if (this._parseListeners == null || !this._parseListeners.Remove(listener) || this._parseListeners.Count != 0)
        return;
      this._parseListeners = (IList<IParseTreeListener>) null;
    }

    public virtual void RemoveParseListeners() => this._parseListeners = (IList<IParseTreeListener>) null;

    protected internal virtual void TriggerEnterRuleEvent()
    {
      foreach (IParseTreeListener parseListener in (IEnumerable<IParseTreeListener>) this._parseListeners)
      {
        parseListener.EnterEveryRule(this._ctx);
        this._ctx.EnterRule(parseListener);
      }
    }

    protected internal virtual void TriggerExitRuleEvent()
    {
      if (this._parseListeners == null)
        return;
      for (int index = this._parseListeners.Count - 1; index >= 0; --index)
      {
        IParseTreeListener parseListener = this._parseListeners[index];
        this._ctx.ExitRule(parseListener);
        parseListener.ExitEveryRule(this._ctx);
      }
    }

    public virtual int NumberOfSyntaxErrors => this._syntaxErrors;

    public virtual ITokenFactory TokenFactory => this._input.TokenSource.TokenFactory;

    [return: NotNull]
    public virtual ATN GetATNWithBypassAlts()
    {
      string serializedAtn = this.SerializedAtn;
      if (serializedAtn == null)
        throw new NotSupportedException("The current parser does not support an ATN with bypass alternatives.");
      lock (Parser.bypassAltsAtnCache)
      {
        ATN atnWithBypassAlts = Parser.bypassAltsAtnCache.Get<string, ATN>(serializedAtn);
        if (atnWithBypassAlts == null)
        {
          atnWithBypassAlts = new ATNDeserializer(new ATNDeserializationOptions()
          {
            GenerateRuleBypassTransitions = true
          }).Deserialize(serializedAtn.ToCharArray());
          Parser.bypassAltsAtnCache.Put<string, ATN>(serializedAtn, atnWithBypassAlts);
        }
        return atnWithBypassAlts;
      }
    }

    public virtual ParseTreePattern CompileParseTreePattern(string pattern, int patternRuleIndex)
    {
      ITokenSource tokenSource = (ITokenStream) this.InputStream != null ? ((ITokenStream) this.InputStream).TokenSource : throw new NotSupportedException("Parser can't discover a lexer to use");
      if (tokenSource is Lexer)
      {
        Lexer lexer = (Lexer) tokenSource;
        return this.CompileParseTreePattern(pattern, patternRuleIndex, lexer);
      }
    }

    public virtual ParseTreePattern CompileParseTreePattern(
      string pattern,
      int patternRuleIndex,
      Lexer lexer)
    {
      return new ParseTreePatternMatcher(lexer, this).Compile(pattern, patternRuleIndex);
    }

    public virtual IAntlrErrorStrategy ErrorHandler
    {
      get => this._errHandler;
      set => this._errHandler = value;
    }

    public override IIntStream InputStream => (IIntStream) this._input;

    public ITokenStream TokenStream
    {
      get => this._input;
      set
      {
        this._input = (ITokenStream) null;
        this.Reset();
        this._input = value;
      }
    }

    public virtual IToken CurrentToken => this._input.LT(1);

    public void NotifyErrorListeners(string msg) => this.NotifyErrorListeners(this.CurrentToken, msg, (RecognitionException) null);

    public virtual void NotifyErrorListeners(
      IToken offendingToken,
      string msg,
      RecognitionException e)
    {
      ++this._syntaxErrors;
      int line = -1;
      int charPositionInLine = -1;
      if (offendingToken != null)
      {
        line = offendingToken.Line;
        charPositionInLine = offendingToken.Column;
      }
      this.ErrorListenerDispatch.SyntaxError(this.ErrorOutput, (IRecognizer) this, offendingToken, line, charPositionInLine, msg, e);
    }

    public virtual IToken Consume()
    {
      IToken currentToken = this.CurrentToken;
      if (currentToken.Type != -1)
        this.InputStream.Consume();
      if (this._buildParseTrees | (this._parseListeners != null && this._parseListeners.Count != 0))
      {
        if (this._errHandler.InErrorRecoveryMode(this))
        {
          IErrorNode node = this._ctx.AddErrorNode(currentToken);
          if (this._parseListeners != null)
          {
            foreach (IParseTreeListener parseListener in (IEnumerable<IParseTreeListener>) this._parseListeners)
              parseListener.VisitErrorNode(node);
          }
        }
        else
        {
          ITerminalNode node = this._ctx.AddChild(currentToken);
          if (this._parseListeners != null)
          {
            foreach (IParseTreeListener parseListener in (IEnumerable<IParseTreeListener>) this._parseListeners)
              parseListener.VisitTerminal(node);
          }
        }
      }
      return currentToken;
    }

    protected internal virtual void AddContextToParseTree() => ((ParserRuleContext) this._ctx.Parent)?.AddChild((Antlr4.Runtime.RuleContext) this._ctx);

    public virtual void EnterRule(ParserRuleContext localctx, int state, int ruleIndex)
    {
      this.State = state;
      this._ctx = localctx;
      this._ctx.Start = this._input.LT(1);
      if (this._buildParseTrees)
        this.AddContextToParseTree();
      if (this._parseListeners == null)
        return;
      this.TriggerEnterRuleEvent();
    }

    public virtual void EnterLeftFactoredRule(ParserRuleContext localctx, int state, int ruleIndex)
    {
      this.State = state;
      if (this._buildParseTrees)
      {
        ParserRuleContext child = (ParserRuleContext) this._ctx.GetChild(this._ctx.ChildCount - 1);
        this._ctx.RemoveLastChild();
        child.Parent = (Antlr4.Runtime.RuleContext) localctx;
        localctx.AddChild((Antlr4.Runtime.RuleContext) child);
      }
      this._ctx = localctx;
      this._ctx.Start = this._input.LT(1);
      if (this._buildParseTrees)
        this.AddContextToParseTree();
      if (this._parseListeners == null)
        return;
      this.TriggerEnterRuleEvent();
    }

    public virtual void ExitRule()
    {
      this._ctx.Stop = this._input.LT(-1);
      if (this._parseListeners != null)
        this.TriggerExitRuleEvent();
      this.State = this._ctx.invokingState;
      this._ctx = (ParserRuleContext) this._ctx.Parent;
    }

    public virtual void EnterOuterAlt(ParserRuleContext localctx, int altNum)
    {
      localctx.setAltNumber(altNum);
      if (this._buildParseTrees && this._ctx != localctx)
      {
        ParserRuleContext parent = (ParserRuleContext) this._ctx.Parent;
        if (parent != null)
        {
          parent.RemoveLastChild();
          parent.AddChild((Antlr4.Runtime.RuleContext) localctx);
        }
      }
      this._ctx = localctx;
    }

    public int Precedence => this._precedenceStack.Count == 0 ? -1 : this._precedenceStack[this._precedenceStack.Count - 1];

    [Obsolete("UseEnterRecursionRule(ParserRuleContext, int, int, int) instead.")]
    public virtual void EnterRecursionRule(ParserRuleContext localctx, int ruleIndex) => this.EnterRecursionRule(localctx, this.Atn.ruleToStartState[ruleIndex].stateNumber, ruleIndex, 0);

    public virtual void EnterRecursionRule(
      ParserRuleContext localctx,
      int state,
      int ruleIndex,
      int precedence)
    {
      this.State = state;
      this._precedenceStack.Add(precedence);
      this._ctx = localctx;
      this._ctx.Start = this._input.LT(1);
      if (this._parseListeners == null)
        return;
      this.TriggerEnterRuleEvent();
    }

    public virtual void PushNewRecursionContext(
      ParserRuleContext localctx,
      int state,
      int ruleIndex)
    {
      ParserRuleContext ctx = this._ctx;
      ctx.Parent = (Antlr4.Runtime.RuleContext) localctx;
      ctx.invokingState = state;
      ctx.Stop = this._input.LT(-1);
      this._ctx = localctx;
      this._ctx.Start = ctx.Start;
      if (this._buildParseTrees)
        this._ctx.AddChild((Antlr4.Runtime.RuleContext) ctx);
      if (this._parseListeners == null)
        return;
      this.TriggerEnterRuleEvent();
    }

    public virtual void UnrollRecursionContexts(ParserRuleContext _parentctx)
    {
      this._precedenceStack.RemoveAt(this._precedenceStack.Count - 1);
      this._ctx.Stop = this._input.LT(-1);
      ParserRuleContext ctx = this._ctx;
      if (this._parseListeners != null)
      {
        for (; this._ctx != _parentctx; this._ctx = (ParserRuleContext) this._ctx.Parent)
          this.TriggerExitRuleEvent();
      }
      else
        this._ctx = _parentctx;
      ctx.Parent = (Antlr4.Runtime.RuleContext) _parentctx;
      if (!this._buildParseTrees || _parentctx == null)
        return;
      _parentctx.AddChild((Antlr4.Runtime.RuleContext) ctx);
    }

    public virtual ParserRuleContext GetInvokingContext(int ruleIndex)
    {
      for (ParserRuleContext invokingContext = this._ctx; invokingContext != null; invokingContext = (ParserRuleContext) invokingContext.Parent)
      {
        if (invokingContext.RuleIndex == ruleIndex)
          return invokingContext;
      }
      return (ParserRuleContext) null;
    }

    public virtual ParserRuleContext Context
    {
      get => this._ctx;
      set => this._ctx = value;
    }

    public override bool Precpred(Antlr4.Runtime.RuleContext localctx, int precedence) => precedence >= this._precedenceStack[this._precedenceStack.Count - 1];

    public IParserErrorListener ErrorListenerDispatch => (IParserErrorListener) new ProxyParserErrorListener((ICollection<IAntlrErrorListener<IToken>>) this.ErrorListeners);

    public virtual bool InContext(string context) => false;

    public virtual bool IsExpectedToken(int symbol)
    {
      ATN atn = this.Interpreter.atn;
      ParserRuleContext parserRuleContext = this._ctx;
      ATNState state = atn.states[this.State];
      IntervalSet intervalSet = atn.NextTokens(state);
      if (intervalSet.Contains(symbol))
        return true;
      if (!intervalSet.Contains(-2))
        return false;
      for (; parserRuleContext != null && parserRuleContext.invokingState >= 0 && intervalSet.Contains(-2); parserRuleContext = (ParserRuleContext) parserRuleContext.Parent)
      {
        RuleTransition ruleTransition = (RuleTransition) atn.states[parserRuleContext.invokingState].Transition(0);
        intervalSet = atn.NextTokens(ruleTransition.followState);
        if (intervalSet.Contains(symbol))
          return true;
      }
      return intervalSet.Contains(-2) && symbol == -1;
    }

    [return: NotNull]
    public virtual IntervalSet GetExpectedTokens() => this.Atn.GetExpectedTokens(this.State, (Antlr4.Runtime.RuleContext) this.Context);

    [return: NotNull]
    public virtual IntervalSet GetExpectedTokensWithinCurrentRule()
    {
      ATN atn = this.Interpreter.atn;
      return atn.NextTokens(atn.states[this.State]);
    }

    public virtual int GetRuleIndex(string ruleName)
    {
      int num;
      return this.RuleIndexMap.TryGetValue(ruleName, out num) ? num : -1;
    }

    public virtual ParserRuleContext RuleContext => this._ctx;

    public virtual IList<string> GetRuleInvocationStack() => this.GetRuleInvocationStack((Antlr4.Runtime.RuleContext) this._ctx);

    public virtual string GetRuleInvocationStackAsString()
    {
      StringBuilder stringBuilder = new StringBuilder("[");
      foreach (string ruleInvocation in (IEnumerable<string>) this.GetRuleInvocationStack())
      {
        stringBuilder.Append(ruleInvocation);
        stringBuilder.Append(", ");
      }
      stringBuilder.Length -= 2;
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }

    public virtual IList<string> GetRuleInvocationStack(Antlr4.Runtime.RuleContext p)
    {
      string[] ruleNames = this.RuleNames;
      IList<string> ruleInvocationStack = (IList<string>) new List<string>();
      for (; p != null; p = p.Parent)
      {
        int ruleIndex = p.RuleIndex;
        if (ruleIndex < 0)
          ruleInvocationStack.Add("n/a");
        else
          ruleInvocationStack.Add(ruleNames[ruleIndex]);
      }
      return ruleInvocationStack;
    }

    public virtual IList<string> GetDFAStrings()
    {
      IList<string> dfaStrings = (IList<string>) new List<string>();
      for (int index = 0; index < this.Interpreter.atn.decisionToDFA.Length; ++index)
      {
        DFA dfa = this.Interpreter.atn.decisionToDFA[index];
        dfaStrings.Add(dfa.ToString(this.Vocabulary));
      }
      return dfaStrings;
    }

    public virtual void DumpDFA()
    {
      bool flag = false;
      for (int index = 0; index < this.Interpreter.decisionToDFA.Length; ++index)
      {
        DFA dfa = this.Interpreter.decisionToDFA[index];
        if (dfa.states.Count > 0)
        {
          if (flag)
            this.Output.WriteLine();
          this.Output.WriteLine("Decision " + dfa.decision.ToString() + ":");
          this.Output.Write(dfa.ToString(this.Vocabulary));
          flag = true;
        }
      }
    }

    public virtual string SourceName => this._input.SourceName;

    public override ParseInfo ParseInfo
    {
      get
      {
        ParserATNSimulator interpreter = this.Interpreter;
        return interpreter is ProfilingATNSimulator ? new ParseInfo((ProfilingATNSimulator) interpreter) : (ParseInfo) null;
      }
    }

    public virtual bool Profile
    {
      set
      {
        int num = value ? 1 : 0;
        ParserATNSimulator interpreter = this.Interpreter;
        if (num != 0)
        {
          if (interpreter is ProfilingATNSimulator)
            return;
          this.Interpreter = (ParserATNSimulator) new ProfilingATNSimulator(this);
        }
        else
        {
          if (!(interpreter is ProfilingATNSimulator))
            return;
          this.Interpreter = new ParserATNSimulator(this, this.Atn, (DFA[]) null, (PredictionContextCache) null);
        }
      }
    }

    public virtual bool Trace
    {
      get
      {
        foreach (IParseTreeListener parseListener in (IEnumerable<IParseTreeListener>) this.ParseListeners)
        {
          if (parseListener is Parser.TraceListener)
            return true;
        }
        return false;
      }
      set
      {
        if (!value)
        {
          this.RemoveParseListener((IParseTreeListener) this._tracer);
          this._tracer = (Parser.TraceListener) null;
        }
        else
        {
          if (this._tracer != null)
            this.RemoveParseListener((IParseTreeListener) this._tracer);
          else
            this._tracer = new Parser.TraceListener(this);
          this.AddParseListener((IParseTreeListener) this._tracer);
        }
      }
    }

    internal class TraceListener : IParseTreeListener
    {
      private readonly TextWriter Output;
      private readonly Parser _enclosing;

      public TraceListener(TextWriter output) => this.Output = output;

      public virtual void EnterEveryRule(ParserRuleContext ctx) => this.Output.WriteLine("enter   " + this._enclosing.RuleNames[ctx.RuleIndex] + ", LT(1)=" + this._enclosing._input.LT(1).Text);

      public virtual void ExitEveryRule(ParserRuleContext ctx) => this.Output.WriteLine("exit    " + this._enclosing.RuleNames[ctx.RuleIndex] + ", LT(1)=" + this._enclosing._input.LT(1).Text);

      public virtual void VisitErrorNode(IErrorNode node)
      {
      }

      public virtual void VisitTerminal(ITerminalNode node)
      {
        ParserRuleContext ruleContext = (ParserRuleContext) node.Parent.RuleContext;
        this.Output.WriteLine("consume " + node.Symbol?.ToString() + " rule " + this._enclosing.RuleNames[ruleContext.RuleIndex]);
      }

      internal TraceListener(Parser _enclosing) => this._enclosing = _enclosing;
    }

    internal class TrimToSizeListener : IParseTreeListener
    {
      public static readonly Parser.TrimToSizeListener Instance = new Parser.TrimToSizeListener();

      public virtual void VisitTerminal(ITerminalNode node)
      {
      }

      public virtual void VisitErrorNode(IErrorNode node)
      {
      }

      public virtual void EnterEveryRule(ParserRuleContext ctx)
      {
      }

      public virtual void ExitEveryRule(ParserRuleContext ctx)
      {
        if (!(ctx.children is List<IParseTree>))
          return;
        ((List<IParseTree>) ctx.children).TrimExcess();
      }
    }
  }
}
