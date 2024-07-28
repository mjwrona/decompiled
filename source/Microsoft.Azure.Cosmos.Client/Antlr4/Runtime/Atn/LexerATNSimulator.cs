// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerATNSimulator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class LexerATNSimulator : ATNSimulator
  {
    public readonly bool debug;
    public readonly bool dfa_debug;
    public static readonly int MIN_DFA_EDGE = 0;
    public static readonly int MAX_DFA_EDGE = (int) sbyte.MaxValue;
    protected readonly Lexer recog;
    protected int startIndex = -1;
    protected int thisLine = 1;
    protected int charPositionInLine;
    public readonly DFA[] decisionToDFA;
    protected int mode;
    private readonly SimState prevAccept = new SimState();
    public static int match_calls = 0;

    public LexerATNSimulator(
      ATN atn,
      DFA[] decisionToDFA,
      PredictionContextCache sharedContextCache)
      : this((Lexer) null, atn, decisionToDFA, sharedContextCache)
    {
    }

    public LexerATNSimulator(
      Lexer recog,
      ATN atn,
      DFA[] decisionToDFA,
      PredictionContextCache sharedContextCache)
      : base(atn, sharedContextCache)
    {
      this.decisionToDFA = decisionToDFA;
      this.recog = recog;
    }

    public void CopyState(LexerATNSimulator simulator)
    {
      this.charPositionInLine = simulator.charPositionInLine;
      this.thisLine = simulator.thisLine;
      this.mode = simulator.mode;
      this.startIndex = simulator.startIndex;
    }

    public int Match(ICharStream input, int mode)
    {
      ++LexerATNSimulator.match_calls;
      this.mode = mode;
      int marker = input.Mark();
      try
      {
        this.startIndex = input.Index;
        this.prevAccept.Reset();
        DFA dfa = this.decisionToDFA[mode];
        return dfa.s0 == null ? this.MatchATN(input) : this.ExecATN(input, dfa.s0);
      }
      finally
      {
        input.Release(marker);
      }
    }

    public override void Reset()
    {
      this.prevAccept.Reset();
      this.startIndex = -1;
      this.thisLine = 1;
      this.charPositionInLine = 0;
      this.mode = 0;
    }

    public override void ClearDFA()
    {
      for (int decision = 0; decision < this.decisionToDFA.Length; ++decision)
        this.decisionToDFA[decision] = new DFA(this.atn.GetDecisionState(decision), decision);
    }

    protected int MatchATN(ICharStream input)
    {
      ATNState p = (ATNState) this.atn.modeToStartState[this.mode];
      if (this.debug)
        this.ConsoleWriteLine("matchATN mode " + this.mode.ToString() + " start: " + p?.ToString());
      int mode = this.mode;
      ATNConfigSet startState = this.ComputeStartState(input, p);
      int num1 = startState.hasSemanticContext ? 1 : 0;
      startState.hasSemanticContext = false;
      DFAState ds0 = this.AddDFAState(startState);
      if (num1 == 0)
        this.decisionToDFA[this.mode].s0 = ds0;
      int num2 = this.ExecATN(input, ds0);
      if (!this.debug)
        return num2;
      this.ConsoleWriteLine("DFA after matchATN: " + this.decisionToDFA[mode].ToString());
      return num2;
    }

    protected int ExecATN(ICharStream input, DFAState ds0)
    {
      if (this.debug)
        this.ConsoleWriteLine("start state closure=" + ds0.configSet?.ToString());
      if (ds0.isAcceptState)
        this.CaptureSimState(this.prevAccept, input, ds0);
      int t = input.LA(1);
      DFAState s = ds0;
      while (true)
      {
        if (this.debug)
          this.ConsoleWriteLine("execATN loop starting closure: " + s.configSet?.ToString());
        DFAState dfaState = this.GetExistingTargetState(s, t) ?? this.ComputeTargetState(input, s, t);
        if (dfaState != ATNSimulator.ERROR)
        {
          if (t != -1)
            this.Consume(input);
          if (dfaState.isAcceptState)
          {
            this.CaptureSimState(this.prevAccept, input, dfaState);
            if (t == -1)
              break;
          }
          t = input.LA(1);
          s = dfaState;
        }
        else
          break;
      }
      return this.FailOrAccept(this.prevAccept, input, s.configSet, t);
    }

    protected DFAState GetExistingTargetState(DFAState s, int t)
    {
      if (s.edges == null || t < LexerATNSimulator.MIN_DFA_EDGE || t > LexerATNSimulator.MAX_DFA_EDGE)
        return (DFAState) null;
      DFAState edge = s.edges[t - LexerATNSimulator.MIN_DFA_EDGE];
      if (this.debug && edge != null)
        this.ConsoleWriteLine("reuse state " + s.stateNumber.ToString() + " edge to " + edge.stateNumber.ToString());
      return edge;
    }

    protected DFAState ComputeTargetState(ICharStream input, DFAState s, int t)
    {
      ATNConfigSet atnConfigSet = (ATNConfigSet) new OrderedATNConfigSet();
      this.GetReachableConfigSet(input, s.configSet, atnConfigSet, t);
      if (!atnConfigSet.Empty)
        return this.AddDFAEdge(s, t, atnConfigSet);
      if (!atnConfigSet.hasSemanticContext)
        this.AddDFAEdge(s, t, ATNSimulator.ERROR);
      return ATNSimulator.ERROR;
    }

    protected int FailOrAccept(SimState prevAccept, ICharStream input, ATNConfigSet reach, int t)
    {
      if (prevAccept.dfaState != null)
      {
        LexerActionExecutor lexerActionExecutor = prevAccept.dfaState.lexerActionExecutor;
        this.Accept(input, lexerActionExecutor, this.startIndex, prevAccept.index, prevAccept.line, prevAccept.charPos);
        return prevAccept.dfaState.prediction;
      }
      if (t == -1 && input.Index == this.startIndex)
        return -1;
      throw new LexerNoViableAltException(this.recog, input, this.startIndex, reach);
    }

    protected void GetReachableConfigSet(
      ICharStream input,
      ATNConfigSet closure,
      ATNConfigSet reach,
      int t)
    {
      int num = 0;
      foreach (ATNConfig config in (List<ATNConfig>) closure.configs)
      {
        bool currentAltReachedAcceptState = config.alt == num;
        if (!currentAltReachedAcceptState || !((LexerATNConfig) config).hasPassedThroughNonGreedyDecision())
        {
          if (this.debug)
            this.ConsoleWriteLine("testing " + this.GetTokenName(t) + " at " + config.ToString((IRecognizer) this.recog, true));
          int numberOfTransitions = config.state.NumberOfTransitions;
          for (int i = 0; i < numberOfTransitions; ++i)
          {
            ATNState reachableTarget = this.GetReachableTarget(config.state.Transition(i), t);
            if (reachableTarget != null)
            {
              LexerActionExecutor lexerActionExecutor = ((LexerATNConfig) config).getLexerActionExecutor();
              if (lexerActionExecutor != null)
                lexerActionExecutor = lexerActionExecutor.FixOffsetBeforeMatch(input.Index - this.startIndex);
              bool treatEofAsEpsilon = t == -1;
              if (this.Closure(input, new LexerATNConfig((LexerATNConfig) config, reachableTarget, lexerActionExecutor), reach, currentAltReachedAcceptState, true, treatEofAsEpsilon))
              {
                num = config.alt;
                break;
              }
            }
          }
        }
      }
    }

    protected void Accept(
      ICharStream input,
      LexerActionExecutor lexerActionExecutor,
      int startIndex,
      int index,
      int line,
      int charPos)
    {
      if (this.debug)
        this.ConsoleWriteLine("ACTION " + lexerActionExecutor?.ToString());
      input.Seek(index);
      this.thisLine = line;
      this.charPositionInLine = charPos;
      if (lexerActionExecutor == null || this.recog == null)
        return;
      lexerActionExecutor.Execute(this.recog, input, startIndex);
    }

    protected ATNState GetReachableTarget(Transition trans, int t) => trans.Matches(t, 0, 1114111) ? trans.target : (ATNState) null;

    protected ATNConfigSet ComputeStartState(ICharStream input, ATNState p)
    {
      PredictionContext empty = (PredictionContext) PredictionContext.EMPTY;
      ATNConfigSet configs = (ATNConfigSet) new OrderedATNConfigSet();
      for (int i = 0; i < p.NumberOfTransitions; ++i)
      {
        LexerATNConfig config = new LexerATNConfig(p.Transition(i).target, i + 1, empty);
        this.Closure(input, config, configs, false, false, false);
      }
      return configs;
    }

    protected bool Closure(
      ICharStream input,
      LexerATNConfig config,
      ATNConfigSet configs,
      bool currentAltReachedAcceptState,
      bool speculative,
      bool treatEofAsEpsilon)
    {
      if (this.debug)
        this.ConsoleWriteLine("closure(" + config.ToString((IRecognizer) this.recog, true) + ")");
      if (config.state is RuleStopState)
      {
        if (this.debug)
        {
          if (this.recog != null)
            this.ConsoleWriteLine("closure at " + this.recog.RuleNames[config.state.ruleIndex] + " rule stop " + config?.ToString());
          else
            this.ConsoleWriteLine("closure at rule stop " + config?.ToString());
        }
        if (config.context == null || config.context.HasEmptyPath)
        {
          if (config.context == null || config.context.IsEmpty)
          {
            configs.Add((ATNConfig) config);
            return true;
          }
          configs.Add((ATNConfig) new LexerATNConfig(config, config.state, (PredictionContext) PredictionContext.EMPTY));
          currentAltReachedAcceptState = true;
        }
        if (config.context != null && !config.context.IsEmpty)
        {
          for (int index = 0; index < config.context.Size; ++index)
          {
            if (config.context.GetReturnState(index) != PredictionContext.EMPTY_RETURN_STATE)
            {
              PredictionContext parent = config.context.GetParent(index);
              ATNState state = this.atn.states[config.context.GetReturnState(index)];
              LexerATNConfig config1 = new LexerATNConfig(config, state, parent);
              currentAltReachedAcceptState = this.Closure(input, config1, configs, currentAltReachedAcceptState, speculative, treatEofAsEpsilon);
            }
          }
        }
        return currentAltReachedAcceptState;
      }
      if (!config.state.OnlyHasEpsilonTransitions && (!currentAltReachedAcceptState || !config.hasPassedThroughNonGreedyDecision()))
        configs.Add((ATNConfig) config);
      ATNState state1 = config.state;
      for (int i = 0; i < state1.NumberOfTransitions; ++i)
      {
        Transition t = state1.Transition(i);
        LexerATNConfig epsilonTarget = this.GetEpsilonTarget(input, config, t, configs, speculative, treatEofAsEpsilon);
        if (epsilonTarget != null)
          currentAltReachedAcceptState = this.Closure(input, epsilonTarget, configs, currentAltReachedAcceptState, speculative, treatEofAsEpsilon);
      }
      return currentAltReachedAcceptState;
    }

    protected LexerATNConfig GetEpsilonTarget(
      ICharStream input,
      LexerATNConfig config,
      Transition t,
      ATNConfigSet configs,
      bool speculative,
      bool treatEofAsEpsilon)
    {
      LexerATNConfig epsilonTarget = (LexerATNConfig) null;
      switch (t.TransitionType)
      {
        case TransitionType.EPSILON:
          epsilonTarget = new LexerATNConfig(config, t.target);
          break;
        case TransitionType.RANGE:
        case TransitionType.ATOM:
        case TransitionType.SET:
          if (treatEofAsEpsilon && t.Matches(-1, 0, 1114111))
          {
            epsilonTarget = new LexerATNConfig(config, t.target);
            break;
          }
          break;
        case TransitionType.RULE:
          RuleTransition ruleTransition = (RuleTransition) t;
          PredictionContext context = (PredictionContext) new SingletonPredictionContext(config.context, ruleTransition.followState.stateNumber);
          epsilonTarget = new LexerATNConfig(config, t.target, context);
          break;
        case TransitionType.PREDICATE:
          PredicateTransition predicateTransition = (PredicateTransition) t;
          if (this.debug)
            this.ConsoleWriteLine("EVAL rule " + predicateTransition.ruleIndex.ToString() + ":" + predicateTransition.predIndex.ToString());
          configs.hasSemanticContext = true;
          if (this.EvaluatePredicate(input, predicateTransition.ruleIndex, predicateTransition.predIndex, speculative))
          {
            epsilonTarget = new LexerATNConfig(config, t.target);
            break;
          }
          break;
        case TransitionType.ACTION:
          if (config.context == null || config.context.HasEmptyPath)
          {
            LexerActionExecutor lexerActionExecutor = LexerActionExecutor.Append(config.getLexerActionExecutor(), this.atn.lexerActions[((ActionTransition) t).actionIndex]);
            epsilonTarget = new LexerATNConfig(config, t.target, lexerActionExecutor);
            break;
          }
          epsilonTarget = new LexerATNConfig(config, t.target);
          break;
        case TransitionType.PRECEDENCE:
          throw new Exception("Precedence predicates are not supported in lexers.");
      }
      return epsilonTarget;
    }

    protected bool EvaluatePredicate(
      ICharStream input,
      int ruleIndex,
      int predIndex,
      bool speculative)
    {
      if (this.recog == null)
        return true;
      if (!speculative)
        return this.recog.Sempred((RuleContext) null, ruleIndex, predIndex);
      int charPositionInLine = this.charPositionInLine;
      int thisLine = this.thisLine;
      int index = input.Index;
      int marker = input.Mark();
      try
      {
        this.Consume(input);
        return this.recog.Sempred((RuleContext) null, ruleIndex, predIndex);
      }
      finally
      {
        this.charPositionInLine = charPositionInLine;
        this.thisLine = thisLine;
        input.Seek(index);
        input.Release(marker);
      }
    }

    protected void CaptureSimState(SimState settings, ICharStream input, DFAState dfaState)
    {
      settings.index = input.Index;
      settings.line = this.thisLine;
      settings.charPos = this.charPositionInLine;
      settings.dfaState = dfaState;
    }

    protected DFAState AddDFAEdge(DFAState from, int t, ATNConfigSet q)
    {
      int num = q.hasSemanticContext ? 1 : 0;
      q.hasSemanticContext = false;
      DFAState q1 = this.AddDFAState(q);
      if (num != 0)
        return q1;
      this.AddDFAEdge(from, t, q1);
      return q1;
    }

    protected void AddDFAEdge(DFAState p, int t, DFAState q)
    {
      if (t < LexerATNSimulator.MIN_DFA_EDGE || t > LexerATNSimulator.MAX_DFA_EDGE)
        return;
      if (this.debug)
        this.ConsoleWriteLine("EDGE " + p?.ToString() + " -> " + q?.ToString() + " upon " + ((char) t).ToString());
      lock (p)
      {
        if (p.edges == null)
          p.edges = new DFAState[LexerATNSimulator.MAX_DFA_EDGE - LexerATNSimulator.MIN_DFA_EDGE + 1];
        p.edges[t - LexerATNSimulator.MIN_DFA_EDGE] = q;
      }
    }

    protected DFAState AddDFAState(ATNConfigSet configSet)
    {
      DFAState key1 = new DFAState(configSet);
      ATNConfig atnConfig = (ATNConfig) null;
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (config.state is RuleStopState)
        {
          atnConfig = config;
          break;
        }
      }
      if (atnConfig != null)
      {
        key1.isAcceptState = true;
        key1.lexerActionExecutor = ((LexerATNConfig) atnConfig).getLexerActionExecutor();
        key1.prediction = this.atn.ruleToTokenType[atnConfig.state.ruleIndex];
      }
      DFA dfa = this.decisionToDFA[this.mode];
      lock (dfa.states)
      {
        DFAState dfaState;
        if (dfa.states.TryGetValue(key1, out dfaState))
          return dfaState;
        DFAState key2 = key1;
        key2.stateNumber = dfa.states.Count;
        configSet.IsReadOnly = true;
        key2.configSet = configSet;
        dfa.states[key2] = key2;
        return key2;
      }
    }

    public DFA GetDFA(int mode) => this.decisionToDFA[mode];

    public string GetText(ICharStream input) => input.GetText(Interval.Of(this.startIndex, input.Index - 1));

    public int Line
    {
      get => this.thisLine;
      set => this.thisLine = value;
    }

    public int Column
    {
      get => this.charPositionInLine;
      set => this.charPositionInLine = value;
    }

    public void Consume(ICharStream input)
    {
      if (input.LA(1) == 10)
      {
        ++this.thisLine;
        this.charPositionInLine = 0;
      }
      else
        ++this.charPositionInLine;
      input.Consume();
    }

    public string GetTokenName(int t) => t == -1 ? "EOF" : "'" + ((char) t).ToString() + "'";
  }
}
