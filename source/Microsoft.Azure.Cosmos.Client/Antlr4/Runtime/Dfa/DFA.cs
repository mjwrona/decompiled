// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.DFA
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Dfa
{
  internal class DFA
  {
    public Dictionary<DFAState, DFAState> states = new Dictionary<DFAState, DFAState>();
    public DFAState s0;
    public int decision;
    public DecisionState atnStartState;
    private bool precedenceDfa;

    public DFA(DecisionState atnStartState)
      : this(atnStartState, 0)
    {
    }

    public DFA(DecisionState atnStartState, int decision)
    {
      this.atnStartState = atnStartState;
      this.decision = decision;
      this.precedenceDfa = false;
      if (!(atnStartState is StarLoopEntryState) || !((StarLoopEntryState) atnStartState).isPrecedenceDecision)
        return;
      this.precedenceDfa = true;
      this.s0 = new DFAState(new ATNConfigSet())
      {
        edges = new DFAState[0],
        isAcceptState = false,
        requiresFullContext = false
      };
    }

    public bool IsPrecedenceDfa => this.precedenceDfa;

    public DFAState GetPrecedenceStartState(int precedence)
    {
      if (!this.IsPrecedenceDfa)
        throw new Exception("Only precedence DFAs may contain a precedence start state.");
      return precedence < 0 || precedence >= this.s0.edges.Length ? (DFAState) null : this.s0.edges[precedence];
    }

    public void SetPrecedenceStartState(int precedence, DFAState startState)
    {
      if (!this.IsPrecedenceDfa)
        throw new Exception("Only precedence DFAs may contain a precedence start state.");
      if (precedence < 0)
        return;
      lock (this.s0)
      {
        if (precedence >= this.s0.edges.Length)
          this.s0.edges = Arrays.CopyOf<DFAState>(this.s0.edges, precedence + 1);
        this.s0.edges[precedence] = startState;
      }
    }

    public List<DFAState> GetStates()
    {
      List<DFAState> states = new List<DFAState>((IEnumerable<DFAState>) this.states.Keys);
      states.Sort((Comparison<DFAState>) ((x, y) => x.stateNumber - y.stateNumber));
      return states;
    }

    public override string ToString() => this.ToString((IVocabulary) Vocabulary.EmptyVocabulary);

    public string ToString(IVocabulary vocabulary) => this.s0 == null ? "" : new DFASerializer(this, vocabulary).ToString();

    public string ToLexerString() => this.s0 == null ? "" : new LexerDFASerializer(this).ToString();
  }
}
