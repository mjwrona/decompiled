// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ATNState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Antlr4.Runtime.Atn
{
  internal abstract class ATNState
  {
    public const int InitialNumTransitions = 4;
    public static readonly ReadOnlyCollection<string> serializationNames = new ReadOnlyCollection<string>(Arrays.AsList<string>("INVALID", "BASIC", "RULE_START", "BLOCK_START", "PLUS_BLOCK_START", "STAR_BLOCK_START", "TOKEN_START", "RULE_STOP", "BLOCK_END", "STAR_LOOP_BACK", "STAR_LOOP_ENTRY", "PLUS_LOOP_BACK", "LOOP_END"));
    public const int InvalidStateNumber = -1;
    public ATN atn;
    public int stateNumber = -1;
    public int ruleIndex;
    public bool epsilonOnlyTransitions;
    protected internal readonly List<Antlr4.Runtime.Atn.Transition> transitions = new List<Antlr4.Runtime.Atn.Transition>(4);
    protected internal List<Antlr4.Runtime.Atn.Transition> optimizedTransitions;
    public IntervalSet nextTokenWithinRule;

    public virtual int NonStopStateNumber => this.stateNumber;

    public override int GetHashCode() => this.stateNumber;

    public override bool Equals(object o)
    {
      if (o == this)
        return true;
      return o is ATNState && this.stateNumber == ((ATNState) o).stateNumber;
    }

    public virtual bool IsNonGreedyExitState => false;

    public override string ToString() => this.stateNumber.ToString();

    public virtual Antlr4.Runtime.Atn.Transition[] TransitionsArray => this.transitions.ToArray();

    public virtual int NumberOfTransitions => this.transitions.Count;

    public virtual void AddTransition(Antlr4.Runtime.Atn.Transition e) => this.AddTransition(this.transitions.Count, e);

    public virtual void AddTransition(int index, Antlr4.Runtime.Atn.Transition e)
    {
      if (this.transitions.Count == 0)
        this.epsilonOnlyTransitions = e.IsEpsilon;
      else if (this.epsilonOnlyTransitions != e.IsEpsilon)
      {
        Console.Error.WriteLine("ATN state {0} has both epsilon and non-epsilon transitions.", (object) this.stateNumber);
        this.epsilonOnlyTransitions = false;
      }
      this.transitions.Insert(index, e);
    }

    public virtual Antlr4.Runtime.Atn.Transition Transition(int i) => this.transitions[i];

    public virtual void SetTransition(int i, Antlr4.Runtime.Atn.Transition e) => this.transitions[i] = e;

    public virtual void RemoveTransition(int index) => this.transitions.RemoveAt(index);

    public abstract StateType StateType { get; }

    public bool OnlyHasEpsilonTransitions => this.epsilonOnlyTransitions;

    public virtual void SetRuleIndex(int ruleIndex) => this.ruleIndex = ruleIndex;

    public virtual bool IsOptimized => this.optimizedTransitions != this.transitions;

    public virtual int NumberOfOptimizedTransitions => this.optimizedTransitions.Count;

    public virtual Antlr4.Runtime.Atn.Transition GetOptimizedTransition(int i) => this.optimizedTransitions[i];

    public virtual void AddOptimizedTransition(Antlr4.Runtime.Atn.Transition e)
    {
      if (!this.IsOptimized)
        this.optimizedTransitions = new List<Antlr4.Runtime.Atn.Transition>();
      this.optimizedTransitions.Add(e);
    }

    public virtual void SetOptimizedTransition(int i, Antlr4.Runtime.Atn.Transition e)
    {
      if (!this.IsOptimized)
        throw new InvalidOperationException();
      this.optimizedTransitions[i] = e;
    }

    public virtual void RemoveOptimizedTransition(int i)
    {
      if (!this.IsOptimized)
        throw new InvalidOperationException();
      this.optimizedTransitions.RemoveAt(i);
    }

    public ATNState() => this.optimizedTransitions = this.transitions;
  }
}
