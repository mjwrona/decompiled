// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ParserInterpreter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antlr4.Runtime
{
  internal class ParserInterpreter : Parser
  {
    private readonly string _grammarFileName;
    private readonly ATN _atn;
    private readonly DFA[] _decisionToDFA;
    protected internal readonly BitSet pushRecursionContextStates;
    private readonly string[] _ruleNames;
    [NotNull]
    private readonly IVocabulary vocabulary;
    private readonly Stack<Tuple<ParserRuleContext, int>> _parentContextStack = new Stack<Tuple<ParserRuleContext, int>>();

    public ParserInterpreter(
      string grammarFileName,
      IVocabulary vocabulary,
      IEnumerable<string> ruleNames,
      ATN atn,
      ITokenStream input)
      : base(input)
    {
      this._grammarFileName = grammarFileName;
      this._atn = atn;
      this._ruleNames = ruleNames.ToArray<string>();
      this.vocabulary = vocabulary;
      this.pushRecursionContextStates = new BitSet(atn.states.Count);
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        if (state is StarLoopEntryState && ((StarLoopEntryState) state).isPrecedenceDecision)
          this.pushRecursionContextStates.Set(state.stateNumber);
      }
      int numberOfDecisions = atn.NumberOfDecisions;
      this._decisionToDFA = new DFA[numberOfDecisions];
      for (int decision = 0; decision < numberOfDecisions; ++decision)
      {
        DecisionState decisionState = atn.GetDecisionState(decision);
        this._decisionToDFA[decision] = new DFA(decisionState, decision);
      }
      this.Interpreter = new ParserATNSimulator((Parser) this, atn, this._decisionToDFA, (PredictionContextCache) null);
    }

    public override ATN Atn => this._atn;

    public override IVocabulary Vocabulary => this.vocabulary;

    public override string[] RuleNames => this._ruleNames;

    public override string GrammarFileName => this._grammarFileName;

    public virtual ParserRuleContext Parse(int startRuleIndex)
    {
      RuleStartState ruleStartState = this._atn.ruleToStartState[startRuleIndex];
      InterpreterRuleContext localctx = new InterpreterRuleContext((ParserRuleContext) null, -1, startRuleIndex);
      if (ruleStartState.isPrecedenceRule)
        this.EnterRecursionRule((ParserRuleContext) localctx, ruleStartState.stateNumber, startRuleIndex, 0);
      else
        this.EnterRule((ParserRuleContext) localctx, ruleStartState.stateNumber, startRuleIndex);
      while (true)
      {
        ATNState atnState = this.AtnState;
        if (atnState.StateType == StateType.RuleStop)
        {
          if (!this.RuleContext.IsEmpty)
            this.VisitRuleStopState(atnState);
          else
            break;
        }
        else
        {
          try
          {
            this.VisitState(atnState);
          }
          catch (RecognitionException ex)
          {
            this.State = this._atn.ruleToStopState[atnState.ruleIndex].stateNumber;
            this.Context.exception = ex;
            this.ErrorHandler.ReportError((Parser) this, ex);
            this.ErrorHandler.Recover((Parser) this, ex);
          }
        }
      }
      if (ruleStartState.isPrecedenceRule)
      {
        ParserRuleContext ruleContext = this.RuleContext;
        this.UnrollRecursionContexts(this._parentContextStack.Pop().Item1);
        return ruleContext;
      }
      this.ExitRule();
      return (ParserRuleContext) localctx;
    }

    public override void EnterRecursionRule(
      ParserRuleContext localctx,
      int state,
      int ruleIndex,
      int precedence)
    {
      this._parentContextStack.Push(Tuple.Create<ParserRuleContext, int>(this.RuleContext, localctx.invokingState));
      base.EnterRecursionRule(localctx, state, ruleIndex, precedence);
    }

    protected internal virtual ATNState AtnState => this._atn.states[this.State];

    protected internal virtual void VisitState(ATNState p)
    {
      int num;
      if (p.NumberOfTransitions > 1)
      {
        this.ErrorHandler.Sync((Parser) this);
        num = this.Interpreter.AdaptivePredict(this.TokenStream, ((DecisionState) p).decision, this.RuleContext);
      }
      else
        num = 1;
      Transition transition = p.Transition(num - 1);
      switch (transition.TransitionType)
      {
        case TransitionType.EPSILON:
          if (this.pushRecursionContextStates.Get(p.stateNumber) && !(transition.target is LoopEndState))
          {
            this.PushNewRecursionContext((ParserRuleContext) new InterpreterRuleContext(this._parentContextStack.Peek().Item1, this._parentContextStack.Peek().Item2, this.RuleContext.RuleIndex), this._atn.ruleToStartState[p.ruleIndex].stateNumber, this.RuleContext.RuleIndex);
            break;
          }
          break;
        case TransitionType.RANGE:
        case TransitionType.SET:
        case TransitionType.NOT_SET:
          if (!transition.Matches(this.TokenStream.LA(1), 1, (int) ushort.MaxValue))
            this.ErrorHandler.RecoverInline((Parser) this);
          this.MatchWildcard();
          break;
        case TransitionType.RULE:
          RuleStartState target = (RuleStartState) transition.target;
          int ruleIndex = target.ruleIndex;
          InterpreterRuleContext localctx = new InterpreterRuleContext(this.RuleContext, p.stateNumber, ruleIndex);
          if (target.isPrecedenceRule)
          {
            this.EnterRecursionRule((ParserRuleContext) localctx, target.stateNumber, ruleIndex, ((RuleTransition) transition).precedence);
            break;
          }
          this.EnterRule((ParserRuleContext) localctx, transition.target.stateNumber, ruleIndex);
          break;
        case TransitionType.PREDICATE:
          PredicateTransition predicateTransition = (PredicateTransition) transition;
          if (!this.Sempred((Antlr4.Runtime.RuleContext) this.RuleContext, predicateTransition.ruleIndex, predicateTransition.predIndex))
            throw new FailedPredicateException((Parser) this);
          break;
        case TransitionType.ATOM:
          this.Match(((AtomTransition) transition).token);
          break;
        case TransitionType.ACTION:
          ActionTransition actionTransition = (ActionTransition) transition;
          this.Action((Antlr4.Runtime.RuleContext) this.RuleContext, actionTransition.ruleIndex, actionTransition.actionIndex);
          break;
        case TransitionType.WILDCARD:
          this.MatchWildcard();
          break;
        case TransitionType.PRECEDENCE:
          if (!this.Precpred((Antlr4.Runtime.RuleContext) this.RuleContext, ((PrecedencePredicateTransition) transition).precedence))
            throw new FailedPredicateException((Parser) this, string.Format("precpred(_ctx, {0})", (object) ((PrecedencePredicateTransition) transition).precedence));
          break;
        default:
          throw new NotSupportedException("Unrecognized ATN transition type.");
      }
      this.State = transition.target.stateNumber;
    }

    protected internal virtual void VisitRuleStopState(ATNState p)
    {
      if (this._atn.ruleToStartState[p.ruleIndex].isPrecedenceRule)
      {
        Tuple<ParserRuleContext, int> tuple = this._parentContextStack.Pop();
        this.UnrollRecursionContexts(tuple.Item1);
        this.State = tuple.Item2;
      }
      else
        this.ExitRule();
      this.State = ((RuleTransition) this._atn.states[this.State].Transition(0)).followState.stateNumber;
    }
  }
}
