// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATNDeserializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Antlr4.Runtime.Atn
{
  internal class ATNDeserializer
  {
    public static readonly int SerializedVersion = 3;
    private static readonly Guid BaseSerializedUuid = new Guid("AADB8D7E-AEEF-4415-AD2B-8204D6CF042E");
    private static readonly Guid AddedUnicodeSmp = new Guid("59627784-3BE5-417A-B9EB-8131A7286089");
    private static readonly IList<Guid> SupportedUuids = (IList<Guid>) new List<Guid>();
    public static readonly Guid SerializedUuid;
    [NotNull]
    private readonly ATNDeserializationOptions deserializationOptions;
    private Guid uuid;
    private char[] data;
    private int p;

    static ATNDeserializer()
    {
      ATNDeserializer.SupportedUuids.Add(ATNDeserializer.BaseSerializedUuid);
      ATNDeserializer.SupportedUuids.Add(ATNDeserializer.AddedUnicodeSmp);
      ATNDeserializer.SerializedUuid = ATNDeserializer.AddedUnicodeSmp;
    }

    public ATNDeserializer()
      : this(ATNDeserializationOptions.Default)
    {
    }

    public ATNDeserializer(ATNDeserializationOptions deserializationOptions)
    {
      if (deserializationOptions == null)
        deserializationOptions = ATNDeserializationOptions.Default;
      this.deserializationOptions = deserializationOptions;
    }

    protected internal virtual bool IsFeatureSupported(Guid feature, Guid actualUuid)
    {
      int num = ATNDeserializer.SupportedUuids.IndexOf(feature);
      return num >= 0 && ATNDeserializer.SupportedUuids.IndexOf(actualUuid) >= num;
    }

    public virtual ATN Deserialize(char[] data)
    {
      this.Reset(data);
      this.CheckVersion();
      this.CheckUUID();
      ATN atn = this.ReadATN();
      this.ReadStates(atn);
      this.ReadRules(atn);
      this.ReadModes(atn);
      IList<IntervalSet> sets = (IList<IntervalSet>) new List<IntervalSet>();
      this.ReadSets(atn, sets, new Func<int>(this.ReadInt));
      if (this.IsFeatureSupported(ATNDeserializer.AddedUnicodeSmp, this.uuid))
        this.ReadSets(atn, sets, new Func<int>(this.ReadInt32));
      this.ReadEdges(atn, sets);
      this.ReadDecisions(atn);
      this.ReadLexerActions(atn);
      this.MarkPrecedenceDecisions(atn);
      if (this.deserializationOptions.VerifyAtn)
        this.VerifyATN(atn);
      if (this.deserializationOptions.GenerateRuleBypassTransitions && atn.grammarType == ATNType.Parser)
        this.GenerateRuleBypassTransitions(atn);
      if (this.deserializationOptions.Optimize)
        this.OptimizeATN(atn);
      ATNDeserializer.IdentifyTailCalls(atn);
      return atn;
    }

    protected internal virtual void OptimizeATN(ATN atn)
    {
      int num1;
      int num2;
      do
      {
        num1 = 0 + ATNDeserializer.InlineSetRules(atn) + ATNDeserializer.CombineChainedEpsilons(atn);
        bool preserveOrder = atn.grammarType == ATNType.Lexer;
        num2 = ATNDeserializer.OptimizeSets(atn, preserveOrder);
      }
      while (num1 + num2 != 0);
      if (!this.deserializationOptions.VerifyAtn)
        return;
      this.VerifyATN(atn);
    }

    protected internal virtual void GenerateRuleBypassTransitions(ATN atn)
    {
      atn.ruleToTokenType = new int[atn.ruleToStartState.Length];
      for (int index = 0; index < atn.ruleToStartState.Length; ++index)
        atn.ruleToTokenType[index] = atn.maxTokenType + index + 1;
      for (int index = 0; index < atn.ruleToStartState.Length; ++index)
      {
        BasicBlockStartState basicBlockStartState = new BasicBlockStartState();
        basicBlockStartState.ruleIndex = index;
        atn.AddState((ATNState) basicBlockStartState);
        BlockEndState blockEndState = new BlockEndState();
        blockEndState.ruleIndex = index;
        atn.AddState((ATNState) blockEndState);
        basicBlockStartState.endState = blockEndState;
        atn.DefineDecisionState((DecisionState) basicBlockStartState);
        blockEndState.startState = (BlockStartState) basicBlockStartState;
        Transition transition1 = (Transition) null;
        ATNState target1;
        if (atn.ruleToStartState[index].isPrecedenceRule)
        {
          target1 = (ATNState) null;
          foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
          {
            if (state.ruleIndex == index && state is StarLoopEntryState)
            {
              ATNState target2 = state.Transition(state.NumberOfTransitions - 1).target;
              if (target2 is LoopEndState && target2.epsilonOnlyTransitions && target2.Transition(0).target is RuleStopState)
              {
                target1 = state;
                break;
              }
            }
          }
          transition1 = target1 != null ? ((StarLoopEntryState) target1).loopBackState.Transition(0) : throw new NotSupportedException("Couldn't identify final state of the precedence rule prefix section.");
        }
        else
          target1 = (ATNState) atn.ruleToStopState[index];
        foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
        {
          foreach (Transition transition2 in state.transitions)
          {
            if (transition2 != transition1 && transition2.target == target1)
              transition2.target = (ATNState) blockEndState;
          }
        }
        while (atn.ruleToStartState[index].NumberOfTransitions > 0)
        {
          Transition e = atn.ruleToStartState[index].Transition(atn.ruleToStartState[index].NumberOfTransitions - 1);
          atn.ruleToStartState[index].RemoveTransition(atn.ruleToStartState[index].NumberOfTransitions - 1);
          basicBlockStartState.AddTransition(e);
        }
        atn.ruleToStartState[index].AddTransition((Transition) new EpsilonTransition((ATNState) basicBlockStartState));
        blockEndState.AddTransition((Transition) new EpsilonTransition(target1));
        ATNState atnState = (ATNState) new BasicState();
        atn.AddState(atnState);
        atnState.AddTransition((Transition) new AtomTransition((ATNState) blockEndState, atn.ruleToTokenType[index]));
        basicBlockStartState.AddTransition((Transition) new EpsilonTransition(atnState));
      }
      if (!this.deserializationOptions.VerifyAtn)
        return;
      this.VerifyATN(atn);
    }

    protected internal virtual void ReadLexerActions(ATN atn)
    {
      if (atn.grammarType != ATNType.Lexer)
        return;
      atn.lexerActions = new ILexerAction[this.ReadInt()];
      for (int index = 0; index < atn.lexerActions.Length; ++index)
      {
        LexerActionType type = (LexerActionType) this.ReadInt();
        int data1 = this.ReadInt();
        if (data1 == (int) ushort.MaxValue)
          data1 = -1;
        int data2 = this.ReadInt();
        if (data2 == (int) ushort.MaxValue)
          data2 = -1;
        ILexerAction lexerAction = this.LexerActionFactory(type, data1, data2);
        atn.lexerActions[index] = lexerAction;
      }
    }

    protected internal virtual void ReadDecisions(ATN atn)
    {
      int length = this.ReadInt();
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = this.ReadInt();
        DecisionState state = (DecisionState) atn.states[index2];
        atn.decisionToState.Add(state);
        state.decision = index1;
      }
      atn.decisionToDFA = new DFA[length];
      for (int index = 0; index < length; ++index)
        atn.decisionToDFA[index] = new DFA(atn.decisionToState[index], index);
    }

    protected internal virtual void ReadEdges(ATN atn, IList<IntervalSet> sets)
    {
      int num1 = this.ReadInt();
      for (int index = 0; index < num1; ++index)
      {
        int num2 = this.ReadInt();
        int trg = this.ReadInt();
        TransitionType type = (TransitionType) this.ReadInt();
        int num3 = this.ReadInt();
        int num4 = this.ReadInt();
        int num5 = this.ReadInt();
        Transition e = this.EdgeFactory(atn, type, num2, trg, num3, num4, num5, sets);
        atn.states[num2].AddTransition(e);
      }
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        for (int i = 0; i < state.NumberOfTransitions; ++i)
        {
          Transition transition = state.Transition(i);
          if (transition is RuleTransition)
          {
            RuleTransition ruleTransition = (RuleTransition) transition;
            int outermostPrecedenceReturn = -1;
            if (atn.ruleToStartState[ruleTransition.target.ruleIndex].isPrecedenceRule && ruleTransition.precedence == 0)
              outermostPrecedenceReturn = ruleTransition.target.ruleIndex;
            EpsilonTransition e = new EpsilonTransition(ruleTransition.followState, outermostPrecedenceReturn);
            atn.ruleToStopState[ruleTransition.target.ruleIndex].AddTransition((Transition) e);
          }
        }
      }
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        switch (state)
        {
          case BlockStartState _:
            if (((BlockStartState) state).endState == null)
              throw new InvalidOperationException();
            if (((BlockStartState) state).endState.startState != null)
              throw new InvalidOperationException();
            ((BlockStartState) state).endState.startState = (BlockStartState) state;
            continue;
          case PlusLoopbackState _:
            PlusLoopbackState plusLoopbackState = (PlusLoopbackState) state;
            for (int i = 0; i < plusLoopbackState.NumberOfTransitions; ++i)
            {
              ATNState target = plusLoopbackState.Transition(i).target;
              if (target is PlusBlockStartState)
                ((PlusBlockStartState) target).loopBackState = plusLoopbackState;
            }
            continue;
          case StarLoopbackState _:
            StarLoopbackState starLoopbackState = (StarLoopbackState) state;
            for (int i = 0; i < starLoopbackState.NumberOfTransitions; ++i)
            {
              ATNState target = starLoopbackState.Transition(i).target;
              if (target is StarLoopEntryState)
                ((StarLoopEntryState) target).loopBackState = starLoopbackState;
            }
            continue;
          default:
            continue;
        }
      }
    }

    protected internal virtual void ReadSets(
      ATN atn,
      IList<IntervalSet> sets,
      Func<int> readUnicode)
    {
      int num1 = this.ReadInt();
      for (int index1 = 0; index1 < num1; ++index1)
      {
        IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
        sets.Add(intervalSet);
        int num2 = this.ReadInt();
        if (this.ReadInt() != 0)
          intervalSet.Add(-1);
        for (int index2 = 0; index2 < num2; ++index2)
          intervalSet.Add(readUnicode(), readUnicode());
      }
    }

    protected internal virtual void ReadModes(ATN atn)
    {
      int length = this.ReadInt();
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = this.ReadInt();
        atn.modeToStartState.Add((TokensStartState) atn.states[index2]);
      }
      atn.modeToDFA = new DFA[length];
      for (int index = 0; index < length; ++index)
        atn.modeToDFA[index] = new DFA((DecisionState) atn.modeToStartState[index]);
    }

    protected internal virtual void ReadRules(ATN atn)
    {
      int length = this.ReadInt();
      if (atn.grammarType == ATNType.Lexer)
        atn.ruleToTokenType = new int[length];
      atn.ruleToStartState = new RuleStartState[length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = this.ReadInt();
        RuleStartState state = (RuleStartState) atn.states[index2];
        atn.ruleToStartState[index1] = state;
        if (atn.grammarType == ATNType.Lexer)
        {
          int num = this.ReadInt();
          if (num == (int) ushort.MaxValue)
            num = -1;
          atn.ruleToTokenType[index1] = num;
        }
      }
      atn.ruleToStopState = new RuleStopState[length];
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        if (state is RuleStopState)
        {
          RuleStopState ruleStopState = (RuleStopState) state;
          atn.ruleToStopState[state.ruleIndex] = ruleStopState;
          atn.ruleToStartState[state.ruleIndex].stopState = ruleStopState;
        }
      }
    }

    protected internal virtual void ReadStates(ATN atn)
    {
      IList<Tuple<LoopEndState, int>> tupleList1 = (IList<Tuple<LoopEndState, int>>) new List<Tuple<LoopEndState, int>>();
      IList<Tuple<BlockStartState, int>> tupleList2 = (IList<Tuple<BlockStartState, int>>) new List<Tuple<BlockStartState, int>>();
      int num1 = this.ReadInt();
      for (int index = 0; index < num1; ++index)
      {
        StateType type = (StateType) this.ReadInt();
        if (type == StateType.InvalidType)
        {
          atn.AddState((ATNState) null);
        }
        else
        {
          int ruleIndex = this.ReadInt();
          if (ruleIndex == (int) ushort.MaxValue)
            ruleIndex = -1;
          ATNState state = this.StateFactory(type, ruleIndex);
          if (type == StateType.LoopEnd)
          {
            int num2 = this.ReadInt();
            tupleList1.Add(Tuple.Create<LoopEndState, int>((LoopEndState) state, num2));
          }
          else if (state is BlockStartState)
          {
            int num3 = this.ReadInt();
            tupleList2.Add(Tuple.Create<BlockStartState, int>((BlockStartState) state, num3));
          }
          atn.AddState(state);
        }
      }
      foreach (Tuple<LoopEndState, int> tuple in (IEnumerable<Tuple<LoopEndState, int>>) tupleList1)
        tuple.Item1.loopBackState = atn.states[tuple.Item2];
      foreach (Tuple<BlockStartState, int> tuple in (IEnumerable<Tuple<BlockStartState, int>>) tupleList2)
        tuple.Item1.endState = (BlockEndState) atn.states[tuple.Item2];
      int num4 = this.ReadInt();
      for (int index1 = 0; index1 < num4; ++index1)
      {
        int index2 = this.ReadInt();
        ((DecisionState) atn.states[index2]).nonGreedy = true;
      }
      int num5 = this.ReadInt();
      for (int index3 = 0; index3 < num5; ++index3)
      {
        int index4 = this.ReadInt();
        ((RuleStartState) atn.states[index4]).isPrecedenceRule = true;
      }
    }

    protected internal virtual ATN ReadATN() => new ATN((ATNType) this.ReadInt(), this.ReadInt());

    protected internal virtual void CheckUUID()
    {
      this.uuid = this.ReadUUID();
      if (!ATNDeserializer.SupportedUuids.Contains(this.uuid))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Could not deserialize ATN with UUID {0} (expected {1} or a legacy UUID).", (object) this.uuid, (object) ATNDeserializer.SerializedUuid));
    }

    protected internal virtual void CheckVersion()
    {
      int num = this.ReadInt();
      if (num != ATNDeserializer.SerializedVersion)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Could not deserialize ATN with version {0} (expected {1}).", (object) num, (object) ATNDeserializer.SerializedVersion));
    }

    protected internal virtual void Reset(char[] data)
    {
      this.data = new char[data.Length];
      this.data[0] = data[0];
      for (int index = 1; index < data.Length; ++index)
        this.data[index] = (char) ((uint) data[index] - 2U);
      this.p = 0;
    }

    protected internal virtual void MarkPrecedenceDecisions(ATN atn)
    {
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        if (state is StarLoopEntryState && atn.ruleToStartState[state.ruleIndex].isPrecedenceRule)
        {
          ATNState target = state.Transition(state.NumberOfTransitions - 1).target;
          if (target is LoopEndState && target.epsilonOnlyTransitions && target.Transition(0).target is RuleStopState)
            ((StarLoopEntryState) state).isPrecedenceDecision = true;
        }
      }
    }

    protected internal virtual void VerifyATN(ATN atn)
    {
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        if (state != null)
        {
          this.CheckCondition(state.OnlyHasEpsilonTransitions || state.NumberOfTransitions <= 1);
          if (state is PlusBlockStartState)
            this.CheckCondition(((PlusBlockStartState) state).loopBackState != null);
          if (state is StarLoopEntryState)
          {
            StarLoopEntryState starLoopEntryState = (StarLoopEntryState) state;
            this.CheckCondition(starLoopEntryState.loopBackState != null);
            this.CheckCondition(starLoopEntryState.NumberOfTransitions == 2);
            if (starLoopEntryState.Transition(0).target is StarBlockStartState)
            {
              this.CheckCondition(starLoopEntryState.Transition(1).target is LoopEndState);
              this.CheckCondition(!starLoopEntryState.nonGreedy);
            }
            else
            {
              if (!(starLoopEntryState.Transition(0).target is LoopEndState))
                throw new InvalidOperationException();
              this.CheckCondition(starLoopEntryState.Transition(1).target is StarBlockStartState);
              this.CheckCondition(starLoopEntryState.nonGreedy);
            }
          }
          if (state is StarLoopbackState)
          {
            this.CheckCondition(state.NumberOfTransitions == 1);
            this.CheckCondition(state.Transition(0).target is StarLoopEntryState);
          }
          if (state is LoopEndState)
            this.CheckCondition(((LoopEndState) state).loopBackState != null);
          if (state is RuleStartState)
            this.CheckCondition(((RuleStartState) state).stopState != null);
          if (state is BlockStartState)
            this.CheckCondition(((BlockStartState) state).endState != null);
          if (state is BlockEndState)
            this.CheckCondition(((BlockEndState) state).startState != null);
          if (state is DecisionState)
          {
            DecisionState decisionState = (DecisionState) state;
            this.CheckCondition(decisionState.NumberOfTransitions <= 1 || decisionState.decision >= 0);
          }
          else
            this.CheckCondition(state.NumberOfTransitions <= 1 || state is RuleStopState);
        }
      }
    }

    protected internal virtual void CheckCondition(bool condition) => this.CheckCondition(condition, (string) null);

    protected internal virtual void CheckCondition(bool condition, string message)
    {
      if (!condition)
        throw new InvalidOperationException(message);
    }

    private static int InlineSetRules(ATN atn)
    {
      int num = 0;
      Transition[] transitionArray = new Transition[atn.ruleToStartState.Length];
      for (int index = 0; index < atn.ruleToStartState.Length; ++index)
      {
        ATNState target1 = (ATNState) atn.ruleToStartState[index];
        while (target1.OnlyHasEpsilonTransitions && target1.NumberOfOptimizedTransitions == 1 && target1.GetOptimizedTransition(0).TransitionType == TransitionType.EPSILON)
          target1 = target1.GetOptimizedTransition(0).target;
        if (target1.NumberOfOptimizedTransitions == 1)
        {
          Transition optimizedTransition = target1.GetOptimizedTransition(0);
          ATNState target2 = optimizedTransition.target;
          if (!optimizedTransition.IsEpsilon && target2.OnlyHasEpsilonTransitions && target2.NumberOfOptimizedTransitions == 1 && target2.GetOptimizedTransition(0).target is RuleStopState)
          {
            switch (optimizedTransition.TransitionType)
            {
              case TransitionType.RANGE:
              case TransitionType.ATOM:
              case TransitionType.SET:
                transitionArray[index] = optimizedTransition;
                continue;
              default:
                continue;
            }
          }
        }
      }
      for (int index1 = 0; index1 < atn.states.Count; ++index1)
      {
        ATNState state = atn.states[index1];
        if (state.ruleIndex >= 0)
        {
          IList<Transition> transitionList = (IList<Transition>) null;
          for (int i = 0; i < state.NumberOfOptimizedTransitions; ++i)
          {
            Transition optimizedTransition = state.GetOptimizedTransition(i);
            if (!(optimizedTransition is RuleTransition))
            {
              transitionList?.Add(optimizedTransition);
            }
            else
            {
              RuleTransition ruleTransition = (RuleTransition) optimizedTransition;
              Transition transition = transitionArray[ruleTransition.target.ruleIndex];
              if (transition == null)
              {
                transitionList?.Add(optimizedTransition);
              }
              else
              {
                if (transitionList == null)
                {
                  transitionList = (IList<Transition>) new List<Transition>();
                  for (int index2 = 0; index2 < i; ++index2)
                    transitionList.Add(state.GetOptimizedTransition(i));
                }
                ++num;
                ATNState followState = ruleTransition.followState;
                ATNState atnState = (ATNState) new BasicState();
                atnState.SetRuleIndex(followState.ruleIndex);
                atn.AddState(atnState);
                transitionList.Add((Transition) new EpsilonTransition(atnState));
                switch (transition.TransitionType)
                {
                  case TransitionType.RANGE:
                    atnState.AddTransition((Transition) new RangeTransition(followState, ((RangeTransition) transition).from, ((RangeTransition) transition).to));
                    continue;
                  case TransitionType.ATOM:
                    atnState.AddTransition((Transition) new AtomTransition(followState, ((AtomTransition) transition).token));
                    continue;
                  case TransitionType.SET:
                    atnState.AddTransition((Transition) new SetTransition(followState, transition.Label));
                    continue;
                  default:
                    throw new NotSupportedException();
                }
              }
            }
          }
          if (transitionList != null)
          {
            if (state.IsOptimized)
            {
              while (state.NumberOfOptimizedTransitions > 0)
                state.RemoveOptimizedTransition(state.NumberOfOptimizedTransitions - 1);
            }
            foreach (Transition e in (IEnumerable<Transition>) transitionList)
              state.AddOptimizedTransition(e);
          }
        }
      }
      return num;
    }

    private static int CombineChainedEpsilons(ATN atn)
    {
      int num = 0;
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        if (state.OnlyHasEpsilonTransitions && !(state is RuleStopState))
        {
          IList<Transition> transitionList = (IList<Transition>) null;
label_21:
          for (int i1 = 0; i1 < state.NumberOfOptimizedTransitions; ++i1)
          {
            Transition optimizedTransition = state.GetOptimizedTransition(i1);
            ATNState target1 = optimizedTransition.target;
            if (optimizedTransition.TransitionType != TransitionType.EPSILON || ((EpsilonTransition) optimizedTransition).OutermostPrecedenceReturn != -1 || target1.StateType != StateType.Basic || !target1.OnlyHasEpsilonTransitions)
            {
              transitionList?.Add(optimizedTransition);
            }
            else
            {
              for (int i2 = 0; i2 < target1.NumberOfOptimizedTransitions; ++i2)
              {
                if (target1.GetOptimizedTransition(i2).TransitionType != TransitionType.EPSILON || ((EpsilonTransition) target1.GetOptimizedTransition(i2)).OutermostPrecedenceReturn != -1)
                {
                  if (transitionList != null)
                  {
                    transitionList.Add(optimizedTransition);
                    goto label_21;
                  }
                  else
                    goto label_21;
                }
              }
              ++num;
              if (transitionList == null)
              {
                transitionList = (IList<Transition>) new List<Transition>();
                for (int i3 = 0; i3 < i1; ++i3)
                  transitionList.Add(state.GetOptimizedTransition(i3));
              }
              for (int i4 = 0; i4 < target1.NumberOfOptimizedTransitions; ++i4)
              {
                ATNState target2 = target1.GetOptimizedTransition(i4).target;
                transitionList.Add((Transition) new EpsilonTransition(target2));
              }
            }
          }
          if (transitionList != null)
          {
            if (state.IsOptimized)
            {
              while (state.NumberOfOptimizedTransitions > 0)
                state.RemoveOptimizedTransition(state.NumberOfOptimizedTransitions - 1);
            }
            foreach (Transition e in (IEnumerable<Transition>) transitionList)
              state.AddOptimizedTransition(e);
          }
        }
      }
      return num;
    }

    private static int OptimizeSets(ATN atn, bool preserveOrder)
    {
      if (preserveOrder)
        return 0;
      int num = 0;
      foreach (DecisionState decisionState in (IEnumerable<DecisionState>) atn.decisionToState)
      {
        IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
        for (int index = 0; index < decisionState.NumberOfOptimizedTransitions; ++index)
        {
          Transition optimizedTransition1 = decisionState.GetOptimizedTransition(index);
          if (optimizedTransition1 is EpsilonTransition && optimizedTransition1.target.NumberOfOptimizedTransitions == 1)
          {
            Transition optimizedTransition2 = optimizedTransition1.target.GetOptimizedTransition(0);
            if (optimizedTransition2.target is BlockEndState)
            {
              switch (optimizedTransition2)
              {
                case AtomTransition _:
                case RangeTransition _:
                case SetTransition _:
                  intervalSet.Add(index);
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        if (intervalSet.Count > 1)
        {
          IList<Transition> transitionList = (IList<Transition>) new List<Transition>();
          for (int index = 0; index < decisionState.NumberOfOptimizedTransitions; ++index)
          {
            if (!intervalSet.Contains(index))
              transitionList.Add(decisionState.GetOptimizedTransition(index));
          }
          ATNState target = decisionState.GetOptimizedTransition(intervalSet.MinElement).target.GetOptimizedTransition(0).target;
          IntervalSet set = new IntervalSet(Array.Empty<int>());
          for (int index = 0; index < intervalSet.GetIntervals().Count; ++index)
          {
            Interval interval = intervalSet.GetIntervals()[index];
            for (int a = interval.a; a <= interval.b; ++a)
            {
              Transition optimizedTransition = decisionState.GetOptimizedTransition(a).target.GetOptimizedTransition(0);
              if (optimizedTransition is NotSetTransition)
                throw new NotSupportedException("Not yet implemented.");
              set.AddAll((IIntSet) optimizedTransition.Label);
            }
          }
          Transition e1;
          if (set.GetIntervals().Count == 1)
          {
            if (set.Count == 1)
            {
              e1 = (Transition) new AtomTransition(target, set.MinElement);
            }
            else
            {
              Interval interval = set.GetIntervals()[0];
              e1 = (Transition) new RangeTransition(target, interval.a, interval.b);
            }
          }
          else
            e1 = (Transition) new SetTransition(target, set);
          ATNState atnState = (ATNState) new BasicState();
          atnState.SetRuleIndex(decisionState.ruleIndex);
          atn.AddState(atnState);
          atnState.AddTransition(e1);
          transitionList.Add((Transition) new EpsilonTransition(atnState));
          num += decisionState.NumberOfOptimizedTransitions - transitionList.Count;
          if (decisionState.IsOptimized)
          {
            while (decisionState.NumberOfOptimizedTransitions > 0)
              decisionState.RemoveOptimizedTransition(decisionState.NumberOfOptimizedTransitions - 1);
          }
          foreach (Transition e2 in (IEnumerable<Transition>) transitionList)
            decisionState.AddOptimizedTransition(e2);
        }
      }
      return num;
    }

    private static void IdentifyTailCalls(ATN atn)
    {
      foreach (ATNState state in (IEnumerable<ATNState>) atn.states)
      {
        foreach (Transition transition1 in state.transitions)
        {
          if (transition1 is RuleTransition)
          {
            RuleTransition transition2 = (RuleTransition) transition1;
            transition2.tailCall = ATNDeserializer.TestTailCall(atn, transition2, false);
            transition2.optimizedTailCall = ATNDeserializer.TestTailCall(atn, transition2, true);
          }
        }
        if (state.IsOptimized)
        {
          foreach (Transition optimizedTransition in state.optimizedTransitions)
          {
            if (optimizedTransition is RuleTransition)
            {
              RuleTransition transition = (RuleTransition) optimizedTransition;
              transition.tailCall = ATNDeserializer.TestTailCall(atn, transition, false);
              transition.optimizedTailCall = ATNDeserializer.TestTailCall(atn, transition, true);
            }
          }
        }
      }
    }

    private static bool TestTailCall(ATN atn, RuleTransition transition, bool optimizedPath)
    {
      if (!optimizedPath && transition.tailCall || optimizedPath && transition.optimizedTailCall)
        return true;
      BitSet bitSet = new BitSet(atn.states.Count);
      Stack<ATNState> atnStateStack = new Stack<ATNState>();
      atnStateStack.Push(transition.followState);
      while (atnStateStack.Count > 0)
      {
        ATNState atnState = atnStateStack.Pop();
        if (!bitSet.Get(atnState.stateNumber) && !(atnState is RuleStopState))
        {
          if (!atnState.OnlyHasEpsilonTransitions)
            return false;
          foreach (Transition transition1 in optimizedPath ? (IEnumerable<Transition>) atnState.optimizedTransitions : (IEnumerable<Transition>) atnState.transitions)
          {
            if (transition1.TransitionType != TransitionType.EPSILON)
              return false;
            atnStateStack.Push(transition1.target);
          }
        }
      }
      return true;
    }

    protected internal int ReadInt() => (int) this.data[this.p++];

    protected internal int ReadInt32() => (int) this.data[this.p++] | (int) this.data[this.p++] << 16;

    protected internal long ReadLong() => (long) this.ReadInt32() & (long) uint.MaxValue | (long) this.ReadInt32() << 32;

    protected internal Guid ReadUUID()
    {
      byte[] bytes = BitConverter.GetBytes(this.ReadLong());
      if (BitConverter.IsLittleEndian)
        Array.Reverse((Array) bytes);
      short c = (short) this.ReadInt();
      short b = (short) this.ReadInt();
      return new Guid(this.ReadInt32(), b, c, bytes);
    }

    [return: NotNull]
    protected internal virtual Transition EdgeFactory(
      ATN atn,
      TransitionType type,
      int src,
      int trg,
      int arg1,
      int arg2,
      int arg3,
      IList<IntervalSet> sets)
    {
      ATNState state = atn.states[trg];
      switch (type)
      {
        case TransitionType.EPSILON:
          return (Transition) new EpsilonTransition(state);
        case TransitionType.RANGE:
          return arg3 != 0 ? (Transition) new RangeTransition(state, -1, arg2) : (Transition) new RangeTransition(state, arg1, arg2);
        case TransitionType.RULE:
          return (Transition) new RuleTransition((RuleStartState) atn.states[arg1], arg2, arg3, state);
        case TransitionType.PREDICATE:
          return (Transition) new PredicateTransition(state, arg1, arg2, arg3 != 0);
        case TransitionType.ATOM:
          return arg3 != 0 ? (Transition) new AtomTransition(state, -1) : (Transition) new AtomTransition(state, arg1);
        case TransitionType.ACTION:
          return (Transition) new ActionTransition(state, arg1, arg2, arg3 != 0);
        case TransitionType.SET:
          return (Transition) new SetTransition(state, sets[arg1]);
        case TransitionType.NOT_SET:
          return (Transition) new NotSetTransition(state, sets[arg1]);
        case TransitionType.WILDCARD:
          return (Transition) new WildcardTransition(state);
        case TransitionType.PRECEDENCE:
          return (Transition) new PrecedencePredicateTransition(state, arg1);
        default:
          throw new ArgumentException("The specified transition type is not valid.");
      }
    }

    protected internal virtual ATNState StateFactory(StateType type, int ruleIndex)
    {
      ATNState atnState;
      switch (type)
      {
        case StateType.InvalidType:
          return (ATNState) null;
        case StateType.Basic:
          atnState = (ATNState) new BasicState();
          break;
        case StateType.RuleStart:
          atnState = (ATNState) new RuleStartState();
          break;
        case StateType.BlockStart:
          atnState = (ATNState) new BasicBlockStartState();
          break;
        case StateType.PlusBlockStart:
          atnState = (ATNState) new PlusBlockStartState();
          break;
        case StateType.StarBlockStart:
          atnState = (ATNState) new StarBlockStartState();
          break;
        case StateType.TokenStart:
          atnState = (ATNState) new TokensStartState();
          break;
        case StateType.RuleStop:
          atnState = (ATNState) new RuleStopState();
          break;
        case StateType.BlockEnd:
          atnState = (ATNState) new BlockEndState();
          break;
        case StateType.StarLoopBack:
          atnState = (ATNState) new StarLoopbackState();
          break;
        case StateType.StarLoopEntry:
          atnState = (ATNState) new StarLoopEntryState();
          break;
        case StateType.PlusLoopBack:
          atnState = (ATNState) new PlusLoopbackState();
          break;
        case StateType.LoopEnd:
          atnState = (ATNState) new LoopEndState();
          break;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The specified state type {0} is not valid.", (object) type));
      }
      atnState.ruleIndex = ruleIndex;
      return atnState;
    }

    protected internal virtual ILexerAction LexerActionFactory(
      LexerActionType type,
      int data1,
      int data2)
    {
      switch (type)
      {
        case LexerActionType.Channel:
          return (ILexerAction) new LexerChannelAction(data1);
        case LexerActionType.Custom:
          return (ILexerAction) new LexerCustomAction(data1, data2);
        case LexerActionType.Mode:
          return (ILexerAction) new LexerModeAction(data1);
        case LexerActionType.More:
          return (ILexerAction) LexerMoreAction.Instance;
        case LexerActionType.PopMode:
          return (ILexerAction) LexerPopModeAction.Instance;
        case LexerActionType.PushMode:
          return (ILexerAction) new LexerPushModeAction(data1);
        case LexerActionType.Skip:
          return (ILexerAction) LexerSkipAction.Instance;
        case LexerActionType.Type:
          return (ILexerAction) new LexerTypeAction(data1);
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The specified lexer action type {0} is not valid.", (object) type));
      }
    }
  }
}
