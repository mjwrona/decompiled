// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.DefaultErrorStrategy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  internal class DefaultErrorStrategy : IAntlrErrorStrategy
  {
    protected internal bool errorRecoveryMode;
    protected internal int lastErrorIndex = -1;
    protected internal IntervalSet lastErrorStates;

    public virtual void Reset(Parser recognizer) => this.EndErrorCondition(recognizer);

    protected internal virtual void BeginErrorCondition(Parser recognizer) => this.errorRecoveryMode = true;

    public virtual bool InErrorRecoveryMode(Parser recognizer) => this.errorRecoveryMode;

    protected internal virtual void EndErrorCondition(Parser recognizer)
    {
      this.errorRecoveryMode = false;
      this.lastErrorStates = (IntervalSet) null;
      this.lastErrorIndex = -1;
    }

    public virtual void ReportMatch(Parser recognizer) => this.EndErrorCondition(recognizer);

    public virtual void ReportError(Parser recognizer, RecognitionException e)
    {
      if (this.InErrorRecoveryMode(recognizer))
        return;
      this.BeginErrorCondition(recognizer);
      switch (e)
      {
        case NoViableAltException _:
          this.ReportNoViableAlternative(recognizer, (NoViableAltException) e);
          break;
        case InputMismatchException _:
          this.ReportInputMismatch(recognizer, (InputMismatchException) e);
          break;
        case FailedPredicateException _:
          this.ReportFailedPredicate(recognizer, (FailedPredicateException) e);
          break;
        default:
          Console.Error.WriteLine("unknown recognition error type: " + e.GetType().FullName);
          this.NotifyErrorListeners(recognizer, e.Message, e);
          break;
      }
    }

    protected internal virtual void NotifyErrorListeners(
      Parser recognizer,
      string message,
      RecognitionException e)
    {
      recognizer.NotifyErrorListeners(e.OffendingToken, message, e);
    }

    public virtual void Recover(Parser recognizer, RecognitionException e)
    {
      if (this.lastErrorIndex == recognizer.InputStream.Index && this.lastErrorStates != null && this.lastErrorStates.Contains(recognizer.State))
        recognizer.Consume();
      this.lastErrorIndex = recognizer.InputStream.Index;
      if (this.lastErrorStates == null)
        this.lastErrorStates = new IntervalSet(Array.Empty<int>());
      this.lastErrorStates.Add(recognizer.State);
      IntervalSet errorRecoverySet = this.GetErrorRecoverySet(recognizer);
      this.ConsumeUntil(recognizer, errorRecoverySet);
    }

    public virtual void Sync(Parser recognizer)
    {
      ATNState state = recognizer.Interpreter.atn.states[recognizer.State];
      if (this.InErrorRecoveryMode(recognizer))
        return;
      int el = recognizer.InputStream.LA(1);
      IntervalSet intervalSet = recognizer.Atn.NextTokens(state);
      if (intervalSet.Contains(-2) || intervalSet.Contains(el))
        return;
      switch (state.StateType)
      {
        case StateType.BlockStart:
        case StateType.PlusBlockStart:
        case StateType.StarBlockStart:
        case StateType.StarLoopEntry:
          if (this.SingleTokenDeletion(recognizer) != null)
            break;
          throw new InputMismatchException(recognizer);
        case StateType.StarLoopBack:
        case StateType.PlusLoopBack:
          this.ReportUnwantedToken(recognizer);
          IntervalSet set = recognizer.GetExpectedTokens().Or((IIntSet) this.GetErrorRecoverySet(recognizer));
          this.ConsumeUntil(recognizer, set);
          break;
      }
    }

    protected internal virtual void ReportNoViableAlternative(
      Parser recognizer,
      NoViableAltException e)
    {
      ITokenStream inputStream = (ITokenStream) recognizer.InputStream;
      string message = "no viable alternative at input " + this.EscapeWSAndQuote(inputStream == null ? "<unknown input>" : (e.StartToken.Type != -1 ? inputStream.GetText(e.StartToken, e.OffendingToken) : "<EOF>"));
      this.NotifyErrorListeners(recognizer, message, (RecognitionException) e);
    }

    protected internal virtual void ReportInputMismatch(Parser recognizer, InputMismatchException e)
    {
      string message = "mismatched input " + this.GetTokenErrorDisplay(e.OffendingToken) + " expecting " + e.GetExpectedTokens().ToString(recognizer.Vocabulary);
      this.NotifyErrorListeners(recognizer, message, (RecognitionException) e);
    }

    protected internal virtual void ReportFailedPredicate(
      Parser recognizer,
      FailedPredicateException e)
    {
      string message = "rule " + recognizer.RuleNames[recognizer.RuleContext.RuleIndex] + " " + e.Message;
      this.NotifyErrorListeners(recognizer, message, (RecognitionException) e);
    }

    protected internal virtual void ReportUnwantedToken(Parser recognizer)
    {
      if (this.InErrorRecoveryMode(recognizer))
        return;
      this.BeginErrorCondition(recognizer);
      IToken currentToken = recognizer.CurrentToken;
      string msg = "extraneous input " + this.GetTokenErrorDisplay(currentToken) + " expecting " + this.GetExpectedTokens(recognizer).ToString(recognizer.Vocabulary);
      recognizer.NotifyErrorListeners(currentToken, msg, (RecognitionException) null);
    }

    protected internal virtual void ReportMissingToken(Parser recognizer)
    {
      if (this.InErrorRecoveryMode(recognizer))
        return;
      this.BeginErrorCondition(recognizer);
      IToken currentToken = recognizer.CurrentToken;
      string msg = "missing " + this.GetExpectedTokens(recognizer).ToString(recognizer.Vocabulary) + " at " + this.GetTokenErrorDisplay(currentToken);
      recognizer.NotifyErrorListeners(currentToken, msg, (RecognitionException) null);
    }

    public virtual IToken RecoverInline(Parser recognizer)
    {
      IToken token = this.SingleTokenDeletion(recognizer);
      if (token != null)
      {
        recognizer.Consume();
        return token;
      }
      return this.SingleTokenInsertion(recognizer) ? this.GetMissingSymbol(recognizer) : throw new InputMismatchException(recognizer);
    }

    protected internal virtual bool SingleTokenInsertion(Parser recognizer)
    {
      int el = recognizer.InputStream.LA(1);
      ATNState target = recognizer.Interpreter.atn.states[recognizer.State].Transition(0).target;
      if (!recognizer.Interpreter.atn.NextTokens(target, (RuleContext) recognizer.RuleContext).Contains(el))
        return false;
      this.ReportMissingToken(recognizer);
      return true;
    }

    [return: Nullable]
    protected internal virtual IToken SingleTokenDeletion(Parser recognizer)
    {
      int el = recognizer.InputStream.LA(2);
      if (!this.GetExpectedTokens(recognizer).Contains(el))
        return (IToken) null;
      this.ReportUnwantedToken(recognizer);
      recognizer.Consume();
      IToken currentToken = recognizer.CurrentToken;
      this.ReportMatch(recognizer);
      return currentToken;
    }

    [return: NotNull]
    protected internal virtual IToken GetMissingSymbol(Parser recognizer)
    {
      IToken currentToken = recognizer.CurrentToken;
      int minElement = this.GetExpectedTokens(recognizer).MinElement;
      string tokenText = minElement != -1 ? "<missing " + recognizer.Vocabulary.GetDisplayName(minElement) + ">" : "<missing EOF>";
      IToken current = currentToken;
      IToken token = ((ITokenStream) recognizer.InputStream).LT(-1);
      if (current.Type == -1 && token != null)
        current = token;
      return this.ConstructToken(((ITokenStream) recognizer.InputStream).TokenSource, minElement, tokenText, current);
    }

    protected internal virtual IToken ConstructToken(
      ITokenSource tokenSource,
      int expectedTokenType,
      string tokenText,
      IToken current)
    {
      return tokenSource.TokenFactory.Create(Tuple.Create<ITokenSource, ICharStream>(tokenSource, current.TokenSource.InputStream), expectedTokenType, tokenText, 0, -1, -1, current.Line, current.Column);
    }

    [return: NotNull]
    protected internal virtual IntervalSet GetExpectedTokens(Parser recognizer) => recognizer.GetExpectedTokens();

    protected internal virtual string GetTokenErrorDisplay(IToken t) => t == null ? "<no token>" : this.EscapeWSAndQuote(this.GetSymbolText(t) ?? (this.GetSymbolType(t) != -1 ? "<" + this.GetSymbolType(t).ToString() + ">" : "<EOF>"));

    protected internal virtual string GetSymbolText(IToken symbol) => symbol.Text;

    protected internal virtual int GetSymbolType(IToken symbol) => symbol.Type;

    [return: NotNull]
    protected internal virtual string EscapeWSAndQuote(string s)
    {
      s = s.Replace("\n", "\\n");
      s = s.Replace("\r", "\\r");
      s = s.Replace("\t", "\\t");
      return "'" + s + "'";
    }

    [return: NotNull]
    protected internal virtual IntervalSet GetErrorRecoverySet(Parser recognizer)
    {
      ATN atn = recognizer.Interpreter.atn;
      RuleContext ruleContext = (RuleContext) recognizer.RuleContext;
      IntervalSet errorRecoverySet = new IntervalSet(Array.Empty<int>());
      for (; ruleContext != null && ruleContext.invokingState >= 0; ruleContext = ruleContext.Parent)
      {
        RuleTransition ruleTransition = (RuleTransition) atn.states[ruleContext.invokingState].Transition(0);
        IntervalSet set = atn.NextTokens(ruleTransition.followState);
        errorRecoverySet.AddAll((IIntSet) set);
      }
      errorRecoverySet.Remove(-2);
      return errorRecoverySet;
    }

    protected internal virtual void ConsumeUntil(Parser recognizer, IntervalSet set)
    {
      for (int el = recognizer.InputStream.LA(1); el != -1 && !set.Contains(el); el = recognizer.InputStream.LA(1))
        recognizer.Consume();
    }
  }
}
