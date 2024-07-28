// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ProfilingATNSimulator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using System;

namespace Antlr4.Runtime.Atn
{
  internal class ProfilingATNSimulator : ParserATNSimulator
  {
    protected readonly DecisionInfo[] decisions;
    protected int numDecisions;
    protected int sllStopIndex;
    protected int llStopIndex;
    protected int currentDecision;
    protected DFAState currentState;
    protected int conflictingAltResolvedBySLL;

    public ProfilingATNSimulator(Parser parser)
      : base(parser, parser.Interpreter.atn, parser.Interpreter.decisionToDFA, parser.Interpreter.getSharedContextCache())
    {
      this.numDecisions = this.atn.decisionToState.Count;
      this.decisions = new DecisionInfo[this.numDecisions];
      for (int decision = 0; decision < this.numDecisions; ++decision)
        this.decisions[decision] = new DecisionInfo(decision);
    }

    public override int AdaptivePredict(
      ITokenStream input,
      int decision,
      ParserRuleContext outerContext)
    {
      try
      {
        this.sllStopIndex = -1;
        this.llStopIndex = -1;
        this.currentDecision = decision;
        long fileTime1 = DateTime.Now.ToFileTime();
        int num = base.AdaptivePredict(input, decision, outerContext);
        long fileTime2 = DateTime.Now.ToFileTime();
        this.decisions[decision].timeInPrediction += fileTime2 - fileTime1;
        ++this.decisions[decision].invocations;
        int val2_1 = this.sllStopIndex - this.startIndex + 1;
        this.decisions[decision].SLL_TotalLook += (long) val2_1;
        this.decisions[decision].SLL_MinLook = this.decisions[decision].SLL_MinLook == 0L ? (long) val2_1 : Math.Min(this.decisions[decision].SLL_MinLook, (long) val2_1);
        if ((long) val2_1 > this.decisions[decision].SLL_MaxLook)
        {
          this.decisions[decision].SLL_MaxLook = (long) val2_1;
          this.decisions[decision].SLL_MaxLookEvent = new LookaheadEventInfo(decision, (SimulatorState) null, input, this.startIndex, this.sllStopIndex, false);
        }
        if (this.llStopIndex >= 0)
        {
          int val2_2 = this.llStopIndex - this.startIndex + 1;
          this.decisions[decision].LL_TotalLook += (long) val2_2;
          this.decisions[decision].LL_MinLook = this.decisions[decision].LL_MinLook == 0L ? (long) val2_2 : Math.Min(this.decisions[decision].LL_MinLook, (long) val2_2);
          if ((long) val2_2 > this.decisions[decision].LL_MaxLook)
          {
            this.decisions[decision].LL_MaxLook = (long) val2_2;
            this.decisions[decision].LL_MaxLookEvent = new LookaheadEventInfo(decision, (SimulatorState) null, input, this.startIndex, this.llStopIndex, true);
          }
        }
        return num;
      }
      finally
      {
        this.currentDecision = -1;
      }
    }

    protected override DFAState GetExistingTargetState(DFAState previousD, int t)
    {
      this.sllStopIndex = this.input.Index;
      DFAState existingTargetState = base.GetExistingTargetState(previousD, t);
      if (existingTargetState != null)
      {
        ++this.decisions[this.currentDecision].SLL_DFATransitions;
        if (existingTargetState == ATNSimulator.ERROR)
          this.decisions[this.currentDecision].errors.Add(new ErrorInfo(this.currentDecision, (SimulatorState) null, this.input, this.startIndex, this.sllStopIndex));
      }
      this.currentState = existingTargetState;
      return existingTargetState;
    }

    protected override DFAState ComputeTargetState(DFA dfa, DFAState previousD, int t)
    {
      DFAState targetState = base.ComputeTargetState(dfa, previousD, t);
      this.currentState = targetState;
      return targetState;
    }

    protected override ATNConfigSet ComputeReachSet(ATNConfigSet closure, int t, bool fullCtx)
    {
      if (fullCtx)
        this.llStopIndex = this.input.Index;
      ATNConfigSet reachSet = base.ComputeReachSet(closure, t, fullCtx);
      if (fullCtx)
      {
        ++this.decisions[this.currentDecision].LL_ATNTransitions;
        if (reachSet == null)
          this.decisions[this.currentDecision].errors.Add(new ErrorInfo(this.currentDecision, (SimulatorState) null, this.input, this.startIndex, this.llStopIndex));
      }
      else
      {
        ++this.decisions[this.currentDecision].SLL_ATNTransitions;
        if (reachSet == null)
          this.decisions[this.currentDecision].errors.Add(new ErrorInfo(this.currentDecision, (SimulatorState) null, this.input, this.startIndex, this.sllStopIndex));
      }
      return reachSet;
    }

    protected override bool EvalSemanticContext(
      SemanticContext pred,
      ParserRuleContext parserCallStack,
      int alt,
      bool fullCtx)
    {
      bool evalResult = base.EvalSemanticContext(pred, parserCallStack, alt, fullCtx);
      if (!(pred is SemanticContext.PrecedencePredicate))
        this.decisions[this.currentDecision].predicateEvals.Add(new PredicateEvalInfo((SimulatorState) null, this.currentDecision, this.input, this.startIndex, this.llStopIndex >= 0 ? this.llStopIndex : this.sllStopIndex, pred, evalResult, alt));
      return evalResult;
    }

    protected override void ReportAttemptingFullContext(
      DFA dfa,
      BitSet conflictingAlts,
      ATNConfigSet configs,
      int startIndex,
      int stopIndex)
    {
      this.conflictingAltResolvedBySLL = conflictingAlts == null ? configs.GetAlts().NextSetBit(0) : conflictingAlts.NextSetBit(0);
      ++this.decisions[this.currentDecision].LL_Fallback;
      base.ReportAttemptingFullContext(dfa, conflictingAlts, configs, startIndex, stopIndex);
    }

    protected override void ReportContextSensitivity(
      DFA dfa,
      int prediction,
      ATNConfigSet configs,
      int startIndex,
      int stopIndex)
    {
      if (prediction != this.conflictingAltResolvedBySLL)
        this.decisions[this.currentDecision].contextSensitivities.Add(new ContextSensitivityInfo(this.currentDecision, (SimulatorState) null, this.input, startIndex, stopIndex));
      base.ReportContextSensitivity(dfa, prediction, configs, startIndex, stopIndex);
    }

    protected override void ReportAmbiguity(
      DFA dfa,
      DFAState D,
      int startIndex,
      int stopIndex,
      bool exact,
      BitSet ambigAlts,
      ATNConfigSet configSet)
    {
      int num = ambigAlts == null ? configSet.GetAlts().NextSetBit(0) : ambigAlts.NextSetBit(0);
      if (configSet.fullCtx && num != this.conflictingAltResolvedBySLL)
        this.decisions[this.currentDecision].contextSensitivities.Add(new ContextSensitivityInfo(this.currentDecision, (SimulatorState) null, this.input, startIndex, stopIndex));
      this.decisions[this.currentDecision].ambiguities.Add(new AmbiguityInfo(this.currentDecision, (SimulatorState) null, this.input, startIndex, stopIndex));
      base.ReportAmbiguity(dfa, D, startIndex, stopIndex, exact, ambigAlts, configSet);
    }

    public DecisionInfo[] getDecisionInfo() => this.decisions;

    public DFAState getCurrentState() => this.currentState;
  }
}
