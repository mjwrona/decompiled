// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LL1Analyzer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class LL1Analyzer
  {
    public const int HitPred = 0;
    [NotNull]
    public readonly ATN atn;

    public LL1Analyzer(ATN atn) => this.atn = atn;

    [return: Nullable]
    public virtual IntervalSet[] GetDecisionLookahead(ATNState s)
    {
      if (s == null)
        return (IntervalSet[]) null;
      IntervalSet[] decisionLookahead = new IntervalSet[s.NumberOfTransitions];
      for (int i = 0; i < s.NumberOfTransitions; ++i)
      {
        decisionLookahead[i] = new IntervalSet(Array.Empty<int>());
        HashSet<ATNConfig> lookBusy = new HashSet<ATNConfig>();
        bool seeThruPreds = false;
        this.Look(s.Transition(i).target, (ATNState) null, (PredictionContext) PredictionContext.EMPTY, decisionLookahead[i], lookBusy, new BitSet(), seeThruPreds, false);
        if (decisionLookahead[i].Count == 0 || decisionLookahead[i].Contains(0))
          decisionLookahead[i] = (IntervalSet) null;
      }
      return decisionLookahead;
    }

    [return: NotNull]
    public virtual IntervalSet Look(ATNState s, RuleContext ctx) => this.Look(s, (ATNState) null, ctx);

    [return: NotNull]
    public virtual IntervalSet Look(ATNState s, ATNState stopState, RuleContext ctx)
    {
      IntervalSet look = new IntervalSet(Array.Empty<int>());
      bool seeThruPreds = true;
      PredictionContext ctx1 = ctx != null ? PredictionContext.FromRuleContext(s.atn, ctx) : (PredictionContext) null;
      this.Look(s, stopState, ctx1, look, new HashSet<ATNConfig>(), new BitSet(), seeThruPreds, true);
      return look;
    }

    protected internal virtual void Look(
      ATNState s,
      ATNState stopState,
      PredictionContext ctx,
      IntervalSet look,
      HashSet<ATNConfig> lookBusy,
      BitSet calledRuleStack,
      bool seeThruPreds,
      bool addEOF)
    {
      ATNConfig atnConfig = new ATNConfig(s, 0, ctx);
      if (!lookBusy.Add(atnConfig))
        return;
      if (s == stopState)
      {
        if (ctx == null)
        {
          look.Add(-2);
          return;
        }
        if (ctx.IsEmpty & addEOF)
        {
          look.Add(-1);
          return;
        }
      }
      if (s is RuleStopState)
      {
        if (ctx == null)
        {
          look.Add(-2);
          return;
        }
        if (ctx.IsEmpty & addEOF)
        {
          look.Add(-1);
          return;
        }
        if (ctx != PredictionContext.EMPTY)
        {
          for (int index = 0; index < ctx.Size; ++index)
          {
            ATNState state = this.atn.states[ctx.GetReturnState(index)];
            bool flag = calledRuleStack.Get(state.ruleIndex);
            try
            {
              calledRuleStack.Clear(state.ruleIndex);
              this.Look(state, stopState, ctx.GetParent(index), look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
            }
            finally
            {
              if (flag)
                calledRuleStack.Set(state.ruleIndex);
            }
          }
          return;
        }
      }
      int numberOfTransitions = s.NumberOfTransitions;
      for (int i = 0; i < numberOfTransitions; ++i)
      {
        Transition transition = s.Transition(i);
        if (transition is RuleTransition)
        {
          RuleTransition ruleTransition = (RuleTransition) transition;
          if (!calledRuleStack.Get(ruleTransition.ruleIndex))
          {
            PredictionContext ctx1 = SingletonPredictionContext.Create(ctx, ruleTransition.followState.stateNumber);
            try
            {
              calledRuleStack.Set(ruleTransition.target.ruleIndex);
              this.Look(transition.target, stopState, ctx1, look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
            }
            finally
            {
              calledRuleStack.Clear(ruleTransition.target.ruleIndex);
            }
          }
        }
        else if (transition is AbstractPredicateTransition)
        {
          if (seeThruPreds)
            this.Look(transition.target, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
          else
            look.Add(0);
        }
        else if (transition.IsEpsilon)
          this.Look(transition.target, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
        else if (transition is WildcardTransition)
        {
          look.AddAll((IIntSet) IntervalSet.Of(1, this.atn.maxTokenType));
        }
        else
        {
          IntervalSet set = transition.Label;
          if (set != null)
          {
            if (transition is NotSetTransition)
              set = set.Complement((IIntSet) IntervalSet.Of(1, this.atn.maxTokenType));
            look.AddAll((IIntSet) set);
          }
        }
      }
    }
  }
}
