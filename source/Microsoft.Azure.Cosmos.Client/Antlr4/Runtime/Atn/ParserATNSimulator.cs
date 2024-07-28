// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ParserATNSimulator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class ParserATNSimulator : ATNSimulator
  {
    public static readonly bool debug;
    public static readonly bool debug_list_atn_decisions;
    public static readonly bool dfa_debug;
    public static readonly bool retry_debug;
    protected readonly Parser parser;
    public readonly DFA[] decisionToDFA;
    private PredictionMode mode = PredictionMode.LL;
    protected MergeCache mergeCache;
    protected ITokenStream input;
    protected int startIndex;
    protected ParserRuleContext context;
    protected DFA thisDfa;

    public ParserATNSimulator(
      ATN atn,
      DFA[] decisionToDFA,
      PredictionContextCache sharedContextCache)
      : this((Parser) null, atn, decisionToDFA, sharedContextCache)
    {
    }

    public ParserATNSimulator(
      Parser parser,
      ATN atn,
      DFA[] decisionToDFA,
      PredictionContextCache sharedContextCache)
      : base(atn, sharedContextCache)
    {
      this.parser = parser;
      this.decisionToDFA = decisionToDFA;
    }

    public override void Reset()
    {
    }

    public override void ClearDFA()
    {
      for (int decision = 0; decision < this.decisionToDFA.Length; ++decision)
        this.decisionToDFA[decision] = new DFA(this.atn.GetDecisionState(decision), decision);
    }

    public virtual int AdaptivePredict(
      ITokenStream input,
      int decision,
      ParserRuleContext outerContext)
    {
      if (ParserATNSimulator.debug || ParserATNSimulator.debug_list_atn_decisions)
        this.ConsoleWriteLine("adaptivePredict decision " + decision.ToString() + " exec LA(1)==" + this.GetLookaheadName(input) + " line " + input.LT(1).Line.ToString() + ":" + input.LT(1).Column.ToString());
      this.input = input;
      this.startIndex = input.Index;
      this.context = outerContext;
      DFA dfa = this.decisionToDFA[decision];
      this.thisDfa = dfa;
      int marker = input.Mark();
      int startIndex = this.startIndex;
      try
      {
        DFAState dfaState = !dfa.IsPrecedenceDfa ? dfa.s0 : dfa.GetPrecedenceStartState(this.parser.Precedence);
        if (dfaState == null)
        {
          if (outerContext == null)
            outerContext = ParserRuleContext.EmptyContext;
          if (ParserATNSimulator.debug || ParserATNSimulator.debug_list_atn_decisions)
            this.ConsoleWriteLine("predictATN decision " + dfa.decision.ToString() + " exec LA(1)==" + this.GetLookaheadName(input) + ", outerContext=" + outerContext.ToString((IRecognizer) this.parser));
          bool fullCtx = false;
          ATNConfigSet startState = this.ComputeStartState((ATNState) dfa.atnStartState, (RuleContext) ParserRuleContext.EmptyContext, fullCtx);
          if (dfa.IsPrecedenceDfa)
          {
            dfa.s0.configSet = startState;
            ATNConfigSet configs = this.ApplyPrecedenceFilter(startState);
            dfaState = this.AddDFAState(dfa, new DFAState(configs));
            dfa.SetPrecedenceStartState(this.parser.Precedence, dfaState);
          }
          else
          {
            dfaState = this.AddDFAState(dfa, new DFAState(startState));
            dfa.s0 = dfaState;
          }
        }
        int num = this.ExecATN(dfa, dfaState, input, startIndex, outerContext);
        if (ParserATNSimulator.debug)
          this.ConsoleWriteLine("DFA after predictATN: " + dfa.ToString(this.parser.Vocabulary));
        return num;
      }
      finally
      {
        this.mergeCache = (MergeCache) null;
        this.thisDfa = (DFA) null;
        input.Seek(startIndex);
        input.Release(marker);
      }
    }

    protected int ExecATN(
      DFA dfa,
      DFAState s0,
      ITokenStream input,
      int startIndex,
      ParserRuleContext outerContext)
    {
      if (ParserATNSimulator.debug || ParserATNSimulator.debug_list_atn_decisions)
        this.ConsoleWriteLine("execATN decision " + dfa.decision.ToString() + " exec LA(1)==" + this.GetLookaheadName(input) + " line " + input.LT(1).Line.ToString() + ":" + input.LT(1).Column.ToString());
      DFAState previousD = s0;
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("s0 = " + s0?.ToString());
      int t = input.LA(1);
      while (true)
      {
        do
        {
          DFAState D = this.GetExistingTargetState(previousD, t) ?? this.ComputeTargetState(dfa, previousD, t);
          if (D == ATNSimulator.ERROR)
          {
            NoViableAltException viableAltException = this.NoViableAlt(input, outerContext, previousD.configSet, startIndex);
            input.Seek(startIndex);
            int decisionEntryRule = this.GetSynValidOrSemInvalidAltThatFinishedDecisionEntryRule(previousD.configSet, outerContext);
            return decisionEntryRule != 0 ? decisionEntryRule : throw viableAltException;
          }
          if (D.requiresFullContext && this.mode != PredictionMode.SLL)
          {
            BitSet conflictingAlts = D.configSet.conflictingAlts;
            if (D.predicates != null)
            {
              if (ParserATNSimulator.debug)
                this.ConsoleWriteLine("DFA state has preds in DFA sim LL failover");
              int index = input.Index;
              if (index != startIndex)
                input.Seek(startIndex);
              conflictingAlts = this.EvalSemanticContext(D.predicates, outerContext, true);
              if (conflictingAlts.Cardinality() == 1)
              {
                if (ParserATNSimulator.debug)
                  this.ConsoleWriteLine("Full LL avoided");
                return conflictingAlts.NextSetBit(0);
              }
              if (index != startIndex)
                input.Seek(index);
            }
            if (ParserATNSimulator.dfa_debug)
              this.ConsoleWriteLine("ctx sensitive state " + outerContext?.ToString() + " in " + D?.ToString());
            bool fullCtx = true;
            ATNConfigSet startState = this.ComputeStartState((ATNState) dfa.atnStartState, (RuleContext) outerContext, fullCtx);
            this.ReportAttemptingFullContext(dfa, conflictingAlts, D.configSet, startIndex, input.Index);
            return this.ExecATNWithFullContext(dfa, D, startState, input, startIndex, outerContext);
          }
          if (D.isAcceptState)
          {
            if (D.predicates == null)
              return D.prediction;
            int index = input.Index;
            input.Seek(startIndex);
            BitSet ambigAlts = this.EvalSemanticContext(D.predicates, outerContext, true);
            switch (ambigAlts.Cardinality())
            {
              case 0:
                throw this.NoViableAlt(input, outerContext, D.configSet, startIndex);
              case 1:
                return ambigAlts.NextSetBit(0);
              default:
                this.ReportAmbiguity(dfa, D, startIndex, index, false, ambigAlts, D.configSet);
                return ambigAlts.NextSetBit(0);
            }
          }
          else
            previousD = D;
        }
        while (t == -1);
        input.Consume();
        t = input.LA(1);
      }
    }

    protected virtual DFAState GetExistingTargetState(DFAState previousD, int t)
    {
      DFAState[] edges = previousD.edges;
      return edges == null || t + 1 < 0 || t + 1 >= edges.Length ? (DFAState) null : edges[t + 1];
    }

    protected virtual DFAState ComputeTargetState(DFA dfa, DFAState previousD, int t)
    {
      ATNConfigSet reachSet = this.ComputeReachSet(previousD.configSet, t, false);
      if (reachSet == null)
      {
        this.AddDFAEdge(dfa, previousD, t, ATNSimulator.ERROR);
        return ATNSimulator.ERROR;
      }
      DFAState dfaState = new DFAState(reachSet);
      int uniqueAlt = ParserATNSimulator.GetUniqueAlt(reachSet);
      if (ParserATNSimulator.debug)
      {
        ICollection<BitSet> conflictingAltSubsets = PredictionMode.GetConflictingAltSubsets((IEnumerable<ATNConfig>) reachSet.configs);
        this.ConsoleWriteLine("SLL altSubSets=" + conflictingAltSubsets?.ToString() + ", configs=" + reachSet?.ToString() + ", predict=" + uniqueAlt.ToString() + ", allSubsetsConflict=" + PredictionMode.AllSubsetsConflict((IEnumerable<BitSet>) conflictingAltSubsets).ToString() + ", conflictingAlts=" + this.GetConflictingAlts(reachSet)?.ToString());
      }
      if (uniqueAlt != 0)
      {
        dfaState.isAcceptState = true;
        dfaState.configSet.uniqueAlt = uniqueAlt;
        dfaState.prediction = uniqueAlt;
      }
      else if (PredictionMode.HasSLLConflictTerminatingPrediction(this.mode, reachSet))
      {
        dfaState.configSet.conflictingAlts = this.GetConflictingAlts(reachSet);
        dfaState.requiresFullContext = true;
        dfaState.isAcceptState = true;
        dfaState.prediction = dfaState.configSet.conflictingAlts.NextSetBit(0);
      }
      if (dfaState.isAcceptState && dfaState.configSet.hasSemanticContext)
      {
        this.PredicateDFAState(dfaState, this.atn.GetDecisionState(dfa.decision));
        if (dfaState.predicates != null)
          dfaState.prediction = 0;
      }
      return this.AddDFAEdge(dfa, previousD, t, dfaState);
    }

    protected void PredicateDFAState(DFAState dfaState, DecisionState decisionState)
    {
      int numberOfTransitions = decisionState.NumberOfTransitions;
      BitSet conflictingAltsOrUniqueAlt = this.GetConflictingAltsOrUniqueAlt(dfaState.configSet);
      SemanticContext[] predsForAmbigAlts = this.GetPredsForAmbigAlts(conflictingAltsOrUniqueAlt, dfaState.configSet, numberOfTransitions);
      if (predsForAmbigAlts != null)
      {
        dfaState.predicates = this.GetPredicatePredictions(conflictingAltsOrUniqueAlt, predsForAmbigAlts);
        dfaState.prediction = 0;
      }
      else
        dfaState.prediction = conflictingAltsOrUniqueAlt.NextSetBit(0);
    }

    protected int ExecATNWithFullContext(
      DFA dfa,
      DFAState D,
      ATNConfigSet s0,
      ITokenStream input,
      int startIndex,
      ParserRuleContext outerContext)
    {
      if (ParserATNSimulator.debug || ParserATNSimulator.debug_list_atn_decisions)
        this.ConsoleWriteLine("execATNWithFullContext " + s0?.ToString());
      bool fullCtx = true;
      bool exact = false;
      ATNConfigSet atnConfigSet = s0;
      input.Seek(startIndex);
      int t = input.LA(1);
      ATNConfigSet reachSet;
      int prediction;
      while (true)
      {
        do
        {
          reachSet = this.ComputeReachSet(atnConfigSet, t, fullCtx);
          if (reachSet == null)
          {
            NoViableAltException viableAltException = this.NoViableAlt(input, outerContext, atnConfigSet, startIndex);
            input.Seek(startIndex);
            int decisionEntryRule = this.GetSynValidOrSemInvalidAltThatFinishedDecisionEntryRule(atnConfigSet, outerContext);
            return decisionEntryRule != 0 ? decisionEntryRule : throw viableAltException;
          }
          ICollection<BitSet> conflictingAltSubsets = PredictionMode.GetConflictingAltSubsets((IEnumerable<ATNConfig>) reachSet.configs);
          if (ParserATNSimulator.debug)
            this.ConsoleWriteLine("LL altSubSets=" + conflictingAltSubsets?.ToString() + ", predict=" + PredictionMode.GetUniqueAlt((IEnumerable<BitSet>) conflictingAltSubsets).ToString() + ", ResolvesToJustOneViableAlt=" + PredictionMode.ResolvesToJustOneViableAlt((IEnumerable<BitSet>) conflictingAltSubsets).ToString());
          reachSet.uniqueAlt = ParserATNSimulator.GetUniqueAlt(reachSet);
          if (reachSet.uniqueAlt != 0)
          {
            prediction = reachSet.uniqueAlt;
            goto label_17;
          }
          else
          {
            if (this.mode != PredictionMode.LL_EXACT_AMBIG_DETECTION)
            {
              prediction = PredictionMode.ResolvesToJustOneViableAlt((IEnumerable<BitSet>) conflictingAltSubsets);
              if (prediction != 0)
                goto label_17;
            }
            else if (PredictionMode.AllSubsetsConflict((IEnumerable<BitSet>) conflictingAltSubsets) && PredictionMode.AllSubsetsEqual((IEnumerable<BitSet>) conflictingAltSubsets))
            {
              exact = true;
              prediction = PredictionMode.GetSingleViableAlt((IEnumerable<BitSet>) conflictingAltSubsets);
              goto label_17;
            }
            atnConfigSet = reachSet;
          }
        }
        while (t == -1);
        input.Consume();
        t = input.LA(1);
      }
label_17:
      if (reachSet.uniqueAlt != 0)
      {
        this.ReportContextSensitivity(dfa, prediction, reachSet, startIndex, input.Index);
        return prediction;
      }
      this.ReportAmbiguity(dfa, D, startIndex, input.Index, exact, reachSet.GetAlts(), reachSet);
      return prediction;
    }

    protected virtual ATNConfigSet ComputeReachSet(ATNConfigSet closure, int t, bool fullCtx)
    {
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("in computeReachSet, starting closure: " + closure?.ToString());
      if (this.mergeCache == null)
        this.mergeCache = new MergeCache();
      ATNConfigSet configSet = new ATNConfigSet(fullCtx);
      List<ATNConfig> atnConfigList = (List<ATNConfig>) null;
      foreach (ATNConfig config in (List<ATNConfig>) closure.configs)
      {
        if (ParserATNSimulator.debug)
          this.ConsoleWriteLine("testing " + this.GetTokenName(t) + " at " + config.ToString());
        if (config.state is RuleStopState)
        {
          if (fullCtx || t == -1)
          {
            if (atnConfigList == null)
              atnConfigList = new List<ATNConfig>();
            atnConfigList.Add(config);
          }
        }
        else
        {
          int numberOfTransitions = config.state.NumberOfTransitions;
          for (int i = 0; i < numberOfTransitions; ++i)
          {
            ATNState reachableTarget = this.GetReachableTarget(config.state.Transition(i), t);
            if (reachableTarget != null)
              configSet.Add(new ATNConfig(config, reachableTarget), this.mergeCache);
          }
        }
      }
      ATNConfigSet atnConfigSet = (ATNConfigSet) null;
      if (atnConfigList == null && t != -1)
      {
        if (configSet.Count == 1)
          atnConfigSet = configSet;
        else if (ParserATNSimulator.GetUniqueAlt(configSet) != 0)
          atnConfigSet = configSet;
      }
      if (atnConfigSet == null)
      {
        atnConfigSet = new ATNConfigSet(fullCtx);
        HashSet<ATNConfig> closureBusy = new HashSet<ATNConfig>();
        bool treatEofAsEpsilon = t == -1;
        foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
          this.Closure(config, atnConfigSet, closureBusy, false, fullCtx, treatEofAsEpsilon);
      }
      if (t == -1)
        atnConfigSet = this.RemoveAllConfigsNotInRuleStopState(atnConfigSet, atnConfigSet == configSet);
      if (atnConfigList != null && (!fullCtx || !PredictionMode.HasConfigInRuleStopState((IEnumerable<ATNConfig>) atnConfigSet.configs)))
      {
        foreach (ATNConfig config in atnConfigList)
          atnConfigSet.Add(config, this.mergeCache);
      }
      return atnConfigSet.Empty ? (ATNConfigSet) null : atnConfigSet;
    }

    protected ATNConfigSet RemoveAllConfigsNotInRuleStopState(
      ATNConfigSet configSet,
      bool lookToEndOfRule)
    {
      if (PredictionMode.AllConfigsInRuleStopStates((IEnumerable<ATNConfig>) configSet.configs))
        return configSet;
      ATNConfigSet atnConfigSet = new ATNConfigSet(configSet.fullCtx);
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (config.state is RuleStopState)
          atnConfigSet.Add(config, this.mergeCache);
        else if (lookToEndOfRule && config.state.OnlyHasEpsilonTransitions && this.atn.NextTokens(config.state).Contains(-2))
        {
          ATNState state = (ATNState) this.atn.ruleToStopState[config.state.ruleIndex];
          atnConfigSet.Add(new ATNConfig(config, state), this.mergeCache);
        }
      }
      return atnConfigSet;
    }

    protected ATNConfigSet ComputeStartState(ATNState p, RuleContext ctx, bool fullCtx)
    {
      PredictionContext context = PredictionContext.FromRuleContext(this.atn, ctx);
      ATNConfigSet configs = new ATNConfigSet(fullCtx);
      for (int i = 0; i < p.NumberOfTransitions; ++i)
      {
        ATNConfig config = new ATNConfig(p.Transition(i).target, i + 1, context);
        HashSet<ATNConfig> closureBusy = new HashSet<ATNConfig>();
        this.Closure(config, configs, closureBusy, true, fullCtx, false);
      }
      return configs;
    }

    protected ATNConfigSet ApplyPrecedenceFilter(ATNConfigSet configSet)
    {
      Dictionary<int, PredictionContext> dictionary = new Dictionary<int, PredictionContext>();
      ATNConfigSet atnConfigSet = new ATNConfigSet(configSet.fullCtx);
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (config.alt == 1)
        {
          SemanticContext semanticContext = config.semanticContext.EvalPrecedence<IToken, ParserATNSimulator>((Recognizer<IToken, ParserATNSimulator>) this.parser, (RuleContext) this.context);
          if (semanticContext != null)
          {
            dictionary[config.state.stateNumber] = config.context;
            if (semanticContext != config.semanticContext)
              atnConfigSet.Add(new ATNConfig(config, semanticContext), this.mergeCache);
            else
              atnConfigSet.Add(config, this.mergeCache);
          }
        }
      }
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        PredictionContext predictionContext;
        if (config.alt != 1 && (config.IsPrecedenceFilterSuppressed || !dictionary.TryGetValue(config.state.stateNumber, out predictionContext) || predictionContext == null || !predictionContext.Equals((object) config.context)))
          atnConfigSet.Add(config, this.mergeCache);
      }
      return atnConfigSet;
    }

    protected ATNState GetReachableTarget(Transition trans, int ttype) => trans.Matches(ttype, 0, this.atn.maxTokenType) ? trans.target : (ATNState) null;

    protected SemanticContext[] GetPredsForAmbigAlts(
      BitSet ambigAlts,
      ATNConfigSet configSet,
      int nalts)
    {
      SemanticContext[] array = new SemanticContext[nalts + 1];
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (ambigAlts[config.alt])
          array[config.alt] = SemanticContext.OrOp(array[config.alt], config.semanticContext);
      }
      int num = 0;
      for (int index = 1; index <= nalts; ++index)
      {
        if (array[index] == null)
          array[index] = SemanticContext.NONE;
        else if (array[index] != SemanticContext.NONE)
          ++num;
      }
      if (num == 0)
        array = (SemanticContext[]) null;
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("getPredsForAmbigAlts result " + Arrays.ToString<SemanticContext>(array));
      return array;
    }

    protected PredPrediction[] GetPredicatePredictions(
      BitSet ambigAlts,
      SemanticContext[] altToPred)
    {
      List<PredPrediction> predPredictionList = new List<PredPrediction>();
      bool flag = false;
      for (int index = 1; index < altToPred.Length; ++index)
      {
        SemanticContext pred = altToPred[index];
        if (ambigAlts != null && ambigAlts[index])
          predPredictionList.Add(new PredPrediction(pred, index));
        if (pred != SemanticContext.NONE)
          flag = true;
      }
      return !flag ? (PredPrediction[]) null : predPredictionList.ToArray();
    }

    protected int GetSynValidOrSemInvalidAltThatFinishedDecisionEntryRule(
      ATNConfigSet configs,
      ParserRuleContext outerContext)
    {
      Pair<ATNConfigSet, ATNConfigSet> semanticValidity = this.SplitAccordingToSemanticValidity(configs, outerContext);
      ATNConfigSet a = semanticValidity.a;
      ATNConfigSet b = semanticValidity.b;
      int decisionEntryRule1 = this.getAltThatFinishedDecisionEntryRule(a);
      if (decisionEntryRule1 != 0)
        return decisionEntryRule1;
      if (b.Count > 0)
      {
        int decisionEntryRule2 = this.getAltThatFinishedDecisionEntryRule(b);
        if (decisionEntryRule2 != 0)
          return decisionEntryRule2;
      }
      return 0;
    }

    protected int getAltThatFinishedDecisionEntryRule(ATNConfigSet configSet)
    {
      IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (config.OuterContextDepth > 0 || config.state is RuleStopState && config.context.HasEmptyPath)
          intervalSet.Add(config.alt);
      }
      return intervalSet.Count == 0 ? 0 : intervalSet.MinElement;
    }

    protected Pair<ATNConfigSet, ATNConfigSet> SplitAccordingToSemanticValidity(
      ATNConfigSet configSet,
      ParserRuleContext outerContext)
    {
      ATNConfigSet a = new ATNConfigSet(configSet.fullCtx);
      ATNConfigSet b = new ATNConfigSet(configSet.fullCtx);
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (config.semanticContext != SemanticContext.NONE)
        {
          if (this.EvalSemanticContext(config.semanticContext, outerContext, config.alt, configSet.fullCtx))
            a.Add(config);
          else
            b.Add(config);
        }
        else
          a.Add(config);
      }
      return new Pair<ATNConfigSet, ATNConfigSet>(a, b);
    }

    protected virtual BitSet EvalSemanticContext(
      PredPrediction[] predPredictions,
      ParserRuleContext outerContext,
      bool complete)
    {
      BitSet bitSet = new BitSet();
      foreach (PredPrediction predPrediction in predPredictions)
      {
        if (predPrediction.pred == SemanticContext.NONE)
        {
          bitSet[predPrediction.alt] = true;
          if (!complete)
            break;
        }
        else
        {
          bool fullCtx = false;
          bool flag = this.EvalSemanticContext(predPrediction.pred, outerContext, predPrediction.alt, fullCtx);
          if (ParserATNSimulator.debug || ParserATNSimulator.dfa_debug)
            this.ConsoleWriteLine("eval pred " + predPrediction?.ToString() + "=" + flag.ToString());
          if (flag)
          {
            if (ParserATNSimulator.debug || ParserATNSimulator.dfa_debug)
              this.ConsoleWriteLine("PREDICT " + predPrediction.alt.ToString());
            bitSet[predPrediction.alt] = true;
            if (!complete)
              break;
          }
        }
      }
      return bitSet;
    }

    protected virtual bool EvalSemanticContext(
      SemanticContext pred,
      ParserRuleContext parserCallStack,
      int alt,
      bool fullCtx)
    {
      return pred.Eval<IToken, ParserATNSimulator>((Recognizer<IToken, ParserATNSimulator>) this.parser, (RuleContext) parserCallStack);
    }

    protected void Closure(
      ATNConfig config,
      ATNConfigSet configs,
      HashSet<ATNConfig> closureBusy,
      bool collectPredicates,
      bool fullCtx,
      bool treatEofAsEpsilon)
    {
      int depth = 0;
      this.ClosureCheckingStopState(config, configs, closureBusy, collectPredicates, fullCtx, depth, treatEofAsEpsilon);
    }

    protected void ClosureCheckingStopState(
      ATNConfig config,
      ATNConfigSet configSet,
      HashSet<ATNConfig> closureBusy,
      bool collectPredicates,
      bool fullCtx,
      int depth,
      bool treatEofAsEpsilon)
    {
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("closure(" + config.ToString((IRecognizer) this.parser, true) + ")");
      if (config.state is RuleStopState)
      {
        if (!config.context.IsEmpty)
        {
          for (int index = 0; index < config.context.Size; ++index)
          {
            if (config.context.GetReturnState(index) == PredictionContext.EMPTY_RETURN_STATE)
            {
              if (fullCtx)
              {
                configSet.Add(new ATNConfig(config, config.state, (PredictionContext) PredictionContext.EMPTY), this.mergeCache);
              }
              else
              {
                if (ParserATNSimulator.debug)
                  this.ConsoleWriteLine("FALLING off rule " + this.GetRuleName(config.state.ruleIndex));
                this.Closure_(config, configSet, closureBusy, collectPredicates, fullCtx, depth, treatEofAsEpsilon);
              }
            }
            else
            {
              ATNState state = this.atn.states[config.context.GetReturnState(index)];
              PredictionContext parent = config.context.GetParent(index);
              int alt = config.alt;
              PredictionContext context = parent;
              SemanticContext semanticContext = config.semanticContext;
              this.ClosureCheckingStopState(new ATNConfig(state, alt, context, semanticContext)
              {
                reachesIntoOuterContext = config.OuterContextDepth
              }, configSet, closureBusy, collectPredicates, fullCtx, depth - 1, treatEofAsEpsilon);
            }
          }
          return;
        }
        if (fullCtx)
        {
          configSet.Add(config, this.mergeCache);
          return;
        }
        if (ParserATNSimulator.debug)
          this.ConsoleWriteLine("FALLING off rule " + this.GetRuleName(config.state.ruleIndex));
      }
      this.Closure_(config, configSet, closureBusy, collectPredicates, fullCtx, depth, treatEofAsEpsilon);
    }

    protected void Closure_(
      ATNConfig config,
      ATNConfigSet configs,
      HashSet<ATNConfig> closureBusy,
      bool collectPredicates,
      bool fullCtx,
      int depth,
      bool treatEofAsEpsilon)
    {
      ATNState state = config.state;
      if (!state.OnlyHasEpsilonTransitions)
        configs.Add(config, this.mergeCache);
      for (int i = 0; i < state.NumberOfTransitions; ++i)
      {
        if (i != 0 || !this.CanDropLoopEntryEdgeInLeftRecursiveRule(config))
        {
          Transition t = state.Transition(i);
          bool collectPredicates1 = !(t is Antlr4.Runtime.Atn.ActionTransition) & collectPredicates;
          ATNConfig epsilonTarget = this.GetEpsilonTarget(config, t, collectPredicates1, depth == 0, fullCtx, treatEofAsEpsilon);
          if (epsilonTarget != null)
          {
            int depth1 = depth;
            if (config.state is RuleStopState)
            {
              if (this.thisDfa != null && this.thisDfa.IsPrecedenceDfa && ((EpsilonTransition) t).OutermostPrecedenceReturn == this.thisDfa.atnStartState.ruleIndex)
                epsilonTarget.SetPrecedenceFilterSuppressed(true);
              ++epsilonTarget.reachesIntoOuterContext;
              if (closureBusy.Add(epsilonTarget))
              {
                configs.dipsIntoOuterContext = true;
                --depth1;
                if (ParserATNSimulator.debug)
                  this.ConsoleWriteLine("dips into outer ctx: " + epsilonTarget?.ToString());
              }
              else
                continue;
            }
            else if (t.IsEpsilon || closureBusy.Add(epsilonTarget))
            {
              if (t is Antlr4.Runtime.Atn.RuleTransition && depth1 >= 0)
                ++depth1;
            }
            else
              continue;
            this.ClosureCheckingStopState(epsilonTarget, configs, closureBusy, collectPredicates1, fullCtx, depth1, treatEofAsEpsilon);
          }
        }
      }
    }

    protected bool CanDropLoopEntryEdgeInLeftRecursiveRule(ATNConfig config)
    {
      ATNState state1 = config.state;
      if (state1.StateType != StateType.StarLoopEntry || !((StarLoopEntryState) state1).isPrecedenceDecision || config.context.IsEmpty || config.context.HasEmptyPath)
        return false;
      int size = config.context.Size;
      for (int index = 0; index < size; ++index)
      {
        if (this.atn.states[config.context.GetReturnState(index)].ruleIndex != state1.ruleIndex)
          return false;
      }
      BlockEndState state2 = (BlockEndState) this.atn.states[((BlockStartState) state1.Transition(0).target).endState.stateNumber];
      for (int index = 0; index < size; ++index)
      {
        ATNState state3 = this.atn.states[config.context.GetReturnState(index)];
        if (state3.NumberOfTransitions != 1 || !state3.Transition(0).IsEpsilon)
          return false;
        ATNState target = state3.Transition(0).target;
        if ((state3.StateType != StateType.BlockEnd || target != state1) && state3 != state2 && target != state2 && (target.StateType != StateType.BlockEnd || target.NumberOfTransitions != 1 || !target.Transition(0).IsEpsilon || target.Transition(0).target != state1))
          return false;
      }
      return true;
    }

    public string GetRuleName(int index) => this.parser != null && index >= 0 ? this.parser.RuleNames[index] : "<rule " + index.ToString() + ">";

    protected ATNConfig GetEpsilonTarget(
      ATNConfig config,
      Transition t,
      bool collectPredicates,
      bool inContext,
      bool fullCtx,
      bool treatEofAsEpsilon)
    {
      switch (t.TransitionType)
      {
        case TransitionType.EPSILON:
          return new ATNConfig(config, t.target);
        case TransitionType.RANGE:
        case TransitionType.ATOM:
        case TransitionType.SET:
          return treatEofAsEpsilon && t.Matches(-1, 0, 1) ? new ATNConfig(config, t.target) : (ATNConfig) null;
        case TransitionType.RULE:
          return this.RuleTransition(config, (Antlr4.Runtime.Atn.RuleTransition) t);
        case TransitionType.PREDICATE:
          return this.PredTransition(config, (PredicateTransition) t, collectPredicates, inContext, fullCtx);
        case TransitionType.ACTION:
          return this.ActionTransition(config, (Antlr4.Runtime.Atn.ActionTransition) t);
        case TransitionType.PRECEDENCE:
          return this.PrecedenceTransition(config, (PrecedencePredicateTransition) t, collectPredicates, inContext, fullCtx);
        default:
          return (ATNConfig) null;
      }
    }

    protected ATNConfig ActionTransition(ATNConfig config, Antlr4.Runtime.Atn.ActionTransition t)
    {
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("ACTION edge " + t.ruleIndex.ToString() + ":" + t.actionIndex.ToString());
      return new ATNConfig(config, t.target);
    }

    public ATNConfig PrecedenceTransition(
      ATNConfig config,
      PrecedencePredicateTransition pt,
      bool collectPredicates,
      bool inContext,
      bool fullCtx)
    {
      if (ParserATNSimulator.debug)
      {
        this.ConsoleWriteLine("PRED (collectPredicates=" + collectPredicates.ToString() + ") " + pt.precedence.ToString() + ">=_p, ctx dependent=true");
        if (this.parser != null)
          this.ConsoleWriteLine("context surrounding pred is " + this.parser.GetRuleInvocationStack()?.ToString());
      }
      ATNConfig atnConfig = (ATNConfig) null;
      if (collectPredicates & inContext)
      {
        if (fullCtx)
        {
          int index = this.input.Index;
          this.input.Seek(this.startIndex);
          int num = this.EvalSemanticContext((SemanticContext) pt.Predicate, this.context, config.alt, fullCtx) ? 1 : 0;
          this.input.Seek(index);
          if (num != 0)
            atnConfig = new ATNConfig(config, pt.target);
        }
        else
        {
          SemanticContext semanticContext = SemanticContext.AndOp(config.semanticContext, (SemanticContext) pt.Predicate);
          atnConfig = new ATNConfig(config, pt.target, semanticContext);
        }
      }
      else
        atnConfig = new ATNConfig(config, pt.target);
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("config from pred transition=" + atnConfig?.ToString());
      return atnConfig;
    }

    protected ATNConfig PredTransition(
      ATNConfig config,
      PredicateTransition pt,
      bool collectPredicates,
      bool inContext,
      bool fullCtx)
    {
      if (ParserATNSimulator.debug)
      {
        this.ConsoleWriteLine("PRED (collectPredicates=" + collectPredicates.ToString() + ") " + pt.ruleIndex.ToString() + ":" + pt.predIndex.ToString() + ", ctx dependent=" + pt.isCtxDependent.ToString());
        if (this.parser != null)
          this.ConsoleWriteLine("context surrounding pred is " + this.parser.GetRuleInvocationStack()?.ToString());
      }
      ATNConfig atnConfig = (ATNConfig) null;
      if (collectPredicates && (!pt.isCtxDependent || pt.isCtxDependent & inContext))
      {
        if (fullCtx)
        {
          int index = this.input.Index;
          this.input.Seek(this.startIndex);
          int num = this.EvalSemanticContext((SemanticContext) pt.Predicate, this.context, config.alt, fullCtx) ? 1 : 0;
          this.input.Seek(index);
          if (num != 0)
            atnConfig = new ATNConfig(config, pt.target);
        }
        else
        {
          SemanticContext semanticContext = SemanticContext.AndOp(config.semanticContext, (SemanticContext) pt.Predicate);
          atnConfig = new ATNConfig(config, pt.target, semanticContext);
        }
      }
      else
        atnConfig = new ATNConfig(config, pt.target);
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("config from pred transition=" + atnConfig?.ToString());
      return atnConfig;
    }

    protected ATNConfig RuleTransition(ATNConfig config, Antlr4.Runtime.Atn.RuleTransition t)
    {
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("CALL rule " + this.GetRuleName(t.target.ruleIndex) + ", ctx=" + config.context?.ToString());
      ATNState followState = t.followState;
      PredictionContext context = SingletonPredictionContext.Create(config.context, followState.stateNumber);
      return new ATNConfig(config, t.target, context);
    }

    protected BitSet GetConflictingAlts(ATNConfigSet configSet) => PredictionMode.GetAlts((IEnumerable<BitSet>) PredictionMode.GetConflictingAltSubsets((IEnumerable<ATNConfig>) configSet.configs));

    protected BitSet GetConflictingAltsOrUniqueAlt(ATNConfigSet configSet)
    {
      BitSet conflictingAltsOrUniqueAlt;
      if (configSet.uniqueAlt != 0)
      {
        conflictingAltsOrUniqueAlt = new BitSet();
        conflictingAltsOrUniqueAlt[configSet.uniqueAlt] = true;
      }
      else
        conflictingAltsOrUniqueAlt = configSet.conflictingAlts;
      return conflictingAltsOrUniqueAlt;
    }

    public string GetTokenName(int t)
    {
      if (t == -1)
        return "EOF";
      string displayName = (this.parser != null ? this.parser.Vocabulary : (IVocabulary) Vocabulary.EmptyVocabulary).GetDisplayName(t);
      return displayName.Equals(t.ToString()) ? displayName : displayName + "<" + t.ToString() + ">";
    }

    public string GetLookaheadName(ITokenStream input) => this.GetTokenName(input.LA(1));

    public void DumpDeadEndConfigs(NoViableAltException nvae)
    {
      Console.Error.WriteLine("dead end configs: ");
      foreach (ATNConfig config in (List<ATNConfig>) nvae.DeadEndConfigs.configs)
      {
        string str = "no edges";
        if (config.state.NumberOfTransitions > 0)
        {
          Transition transition = config.state.Transition(0);
          switch (transition)
          {
            case AtomTransition _:
              str = "Atom " + this.GetTokenName(((AtomTransition) transition).token);
              break;
            case SetTransition _:
              SetTransition setTransition = (SetTransition) transition;
              str = (setTransition is NotSetTransition ? "~" : "") + "Set " + setTransition.set.ToString();
              break;
          }
        }
        Console.Error.WriteLine(config.ToString((IRecognizer) this.parser, true) + ":" + str);
      }
    }

    protected NoViableAltException NoViableAlt(
      ITokenStream input,
      ParserRuleContext outerContext,
      ATNConfigSet configs,
      int startIndex)
    {
      return new NoViableAltException((IRecognizer) this.parser, input, input.Get(startIndex), input.LT(1), configs, outerContext);
    }

    protected static int GetUniqueAlt(ATNConfigSet configSet)
    {
      int uniqueAlt = 0;
      foreach (ATNConfig config in (List<ATNConfig>) configSet.configs)
      {
        if (uniqueAlt == 0)
          uniqueAlt = config.alt;
        else if (config.alt != uniqueAlt)
          return 0;
      }
      return uniqueAlt;
    }

    protected DFAState AddDFAEdge(DFA dfa, DFAState from, int t, DFAState to)
    {
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("EDGE " + from?.ToString() + " -> " + to?.ToString() + " upon " + this.GetTokenName(t));
      if (to == null)
        return (DFAState) null;
      to = this.AddDFAState(dfa, to);
      if (from == null || t < -1 || t > this.atn.maxTokenType)
        return to;
      lock (from)
      {
        if (from.edges == null)
          from.edges = new DFAState[this.atn.maxTokenType + 1 + 1];
        from.edges[t + 1] = to;
      }
      if (ParserATNSimulator.debug)
        this.ConsoleWriteLine("DFA=\n" + dfa.ToString(this.parser != null ? this.parser.Vocabulary : (IVocabulary) Vocabulary.EmptyVocabulary));
      return to;
    }

    protected DFAState AddDFAState(DFA dfa, DFAState D)
    {
      if (D == ATNSimulator.ERROR)
        return D;
      lock (dfa.states)
      {
        DFAState dfaState = dfa.states.Get<DFAState, DFAState>(D);
        if (dfaState != null)
          return dfaState;
        D.stateNumber = dfa.states.Count;
        if (!D.configSet.IsReadOnly)
        {
          D.configSet.OptimizeConfigs((ATNSimulator) this);
          D.configSet.IsReadOnly = true;
        }
        dfa.states.Put<DFAState, DFAState>(D, D);
        if (ParserATNSimulator.debug)
          this.ConsoleWriteLine("adding new DFA state: " + D?.ToString());
        return D;
      }
    }

    protected virtual void ReportAttemptingFullContext(
      DFA dfa,
      BitSet conflictingAlts,
      ATNConfigSet configs,
      int startIndex,
      int stopIndex)
    {
      if (ParserATNSimulator.debug || ParserATNSimulator.retry_debug)
      {
        Interval interval = Interval.Of(startIndex, stopIndex);
        this.ConsoleWriteLine("reportAttemptingFullContext decision=" + dfa.decision.ToString() + ":" + configs?.ToString() + ", input=" + this.parser.TokenStream.GetText(interval));
      }
      if (this.parser == null)
        return;
      this.parser.ErrorListenerDispatch.ReportAttemptingFullContext(this.parser, dfa, startIndex, stopIndex, conflictingAlts, (SimulatorState) null);
    }

    protected virtual void ReportContextSensitivity(
      DFA dfa,
      int prediction,
      ATNConfigSet configs,
      int startIndex,
      int stopIndex)
    {
      if (ParserATNSimulator.debug || ParserATNSimulator.retry_debug)
      {
        Interval interval = Interval.Of(startIndex, stopIndex);
        this.ConsoleWriteLine("ReportContextSensitivity decision=" + dfa.decision.ToString() + ":" + configs?.ToString() + ", input=" + this.parser.TokenStream.GetText(interval));
      }
      if (this.parser == null)
        return;
      this.parser.ErrorListenerDispatch.ReportContextSensitivity(this.parser, dfa, startIndex, stopIndex, prediction, (SimulatorState) null);
    }

    protected virtual void ReportAmbiguity(
      DFA dfa,
      DFAState D,
      int startIndex,
      int stopIndex,
      bool exact,
      BitSet ambigAlts,
      ATNConfigSet configs)
    {
      if (ParserATNSimulator.debug || ParserATNSimulator.retry_debug)
      {
        Interval interval = Interval.Of(startIndex, stopIndex);
        this.ConsoleWriteLine("ReportAmbiguity " + ambigAlts?.ToString() + ":" + configs?.ToString() + ", input=" + this.parser.TokenStream.GetText(interval));
      }
      if (this.parser == null)
        return;
      this.parser.ErrorListenerDispatch.ReportAmbiguity(this.parser, dfa, startIndex, stopIndex, exact, ambigAlts, configs);
    }

    public PredictionMode PredictionMode
    {
      get => this.mode;
      set => this.mode = value;
    }

    public Parser getParser() => this.parser;
  }
}
