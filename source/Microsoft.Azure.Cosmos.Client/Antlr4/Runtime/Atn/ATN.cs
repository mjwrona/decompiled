// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATN
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class ATN
  {
    public const int INVALID_ALT_NUMBER = 0;
    [NotNull]
    public readonly IList<ATNState> states = (IList<ATNState>) new List<ATNState>();
    [NotNull]
    public readonly IList<DecisionState> decisionToState = (IList<DecisionState>) new List<DecisionState>();
    public RuleStartState[] ruleToStartState;
    public RuleStopState[] ruleToStopState;
    [NotNull]
    public readonly IDictionary<string, TokensStartState> modeNameToStartState = (IDictionary<string, TokensStartState>) new Dictionary<string, TokensStartState>();
    public readonly ATNType grammarType;
    public readonly int maxTokenType;
    public int[] ruleToTokenType;
    public ILexerAction[] lexerActions;
    [NotNull]
    public readonly IList<TokensStartState> modeToStartState = (IList<TokensStartState>) new List<TokensStartState>();
    private readonly PredictionContextCache contextCache = new PredictionContextCache();
    [NotNull]
    public DFA[] decisionToDFA = new DFA[0];
    [NotNull]
    public DFA[] modeToDFA = new DFA[0];
    protected internal readonly ConcurrentDictionary<int, int> LL1Table = new ConcurrentDictionary<int, int>();

    public ATN(ATNType grammarType, int maxTokenType)
    {
      this.grammarType = grammarType;
      this.maxTokenType = maxTokenType;
    }

    public virtual PredictionContext GetCachedContext(PredictionContext context) => PredictionContext.GetCachedContext(context, this.contextCache, new PredictionContext.IdentityHashMap());

    [return: NotNull]
    public virtual IntervalSet NextTokens(ATNState s, RuleContext ctx) => new LL1Analyzer(this).Look(s, ctx);

    [return: NotNull]
    public virtual IntervalSet NextTokens(ATNState s)
    {
      if (s.nextTokenWithinRule != null)
        return s.nextTokenWithinRule;
      s.nextTokenWithinRule = this.NextTokens(s, (RuleContext) null);
      s.nextTokenWithinRule.SetReadonly(true);
      return s.nextTokenWithinRule;
    }

    public virtual void AddState(ATNState state)
    {
      if (state != null)
      {
        state.atn = this;
        state.stateNumber = this.states.Count;
      }
      this.states.Add(state);
    }

    public virtual void RemoveState(ATNState state) => this.states[state.stateNumber] = (ATNState) null;

    public virtual void DefineMode(string name, TokensStartState s)
    {
      this.modeNameToStartState[name] = s;
      this.modeToStartState.Add(s);
      this.modeToDFA = Arrays.CopyOf<DFA>(this.modeToDFA, this.modeToStartState.Count);
      this.modeToDFA[this.modeToDFA.Length - 1] = new DFA((DecisionState) s);
      this.DefineDecisionState((DecisionState) s);
    }

    public virtual int DefineDecisionState(DecisionState s)
    {
      this.decisionToState.Add(s);
      s.decision = this.decisionToState.Count - 1;
      this.decisionToDFA = Arrays.CopyOf<DFA>(this.decisionToDFA, this.decisionToState.Count);
      this.decisionToDFA[this.decisionToDFA.Length - 1] = new DFA(s, s.decision);
      return s.decision;
    }

    public virtual DecisionState GetDecisionState(int decision) => this.decisionToState.Count != 0 ? this.decisionToState[decision] : (DecisionState) null;

    public virtual int NumberOfDecisions => this.decisionToState.Count;

    [return: NotNull]
    public virtual IntervalSet GetExpectedTokens(int stateNumber, RuleContext context)
    {
      if (stateNumber < 0 || stateNumber >= this.states.Count)
        throw new ArgumentException("Invalid state number.");
      RuleContext ruleContext = context;
      IntervalSet set = this.NextTokens(this.states[stateNumber]);
      if (!set.Contains(-2))
        return set;
      IntervalSet expectedTokens = new IntervalSet(Array.Empty<int>());
      expectedTokens.AddAll((IIntSet) set);
      expectedTokens.Remove(-2);
      for (; ruleContext != null && ruleContext.invokingState >= 0 && set.Contains(-2); ruleContext = ruleContext.Parent)
      {
        set = this.NextTokens(((RuleTransition) this.states[ruleContext.invokingState].Transition(0)).followState);
        expectedTokens.AddAll((IIntSet) set);
        expectedTokens.Remove(-2);
      }
      if (set.Contains(-2))
        expectedTokens.Add(-1);
      return expectedTokens;
    }
  }
}
