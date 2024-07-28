// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.DFASerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Dfa
{
  internal class DFASerializer
  {
    [NotNull]
    private readonly DFA dfa;
    [NotNull]
    private readonly IVocabulary vocabulary;
    [Nullable]
    internal readonly string[] ruleNames;
    [Nullable]
    internal readonly ATN atn;

    public DFASerializer(DFA dfa, IVocabulary vocabulary)
      : this(dfa, vocabulary, (string[]) null, (ATN) null)
    {
    }

    public DFASerializer(DFA dfa, IRecognizer parser)
      : this(dfa, parser != null ? parser.Vocabulary : (IVocabulary) Vocabulary.EmptyVocabulary, parser?.RuleNames, parser?.Atn)
    {
    }

    public DFASerializer(DFA dfa, IVocabulary vocabulary, string[] ruleNames, ATN atn)
    {
      this.dfa = dfa;
      this.vocabulary = vocabulary;
      this.ruleNames = ruleNames;
      this.atn = atn;
    }

    public override string ToString()
    {
      if (this.dfa.s0 == null)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      if (this.dfa.states != null)
      {
        List<DFAState> dfaStateList = new List<DFAState>((IEnumerable<DFAState>) this.dfa.states.Values);
        dfaStateList.Sort((Comparison<DFAState>) ((x, y) => x.stateNumber - y.stateNumber));
        foreach (DFAState s in dfaStateList)
        {
          int length = s.edges != null ? s.edges.Length : 0;
          for (int i = 0; i < length; ++i)
          {
            DFAState edge = s.edges[i];
            if (edge != null && edge.stateNumber != int.MaxValue)
            {
              stringBuilder.Append(this.GetStateString(s));
              string edgeLabel = this.GetEdgeLabel(i);
              stringBuilder.Append("-");
              stringBuilder.Append(edgeLabel);
              stringBuilder.Append("->");
              stringBuilder.Append(this.GetStateString(edge));
              stringBuilder.Append('\n');
            }
          }
        }
      }
      string str = stringBuilder.ToString();
      return str.Length == 0 ? (string) null : str;
    }

    protected internal virtual string GetContextLabel(int i)
    {
      if (i == PredictionContext.EMPTY_RETURN_STATE)
        return "ctx:EMPTY";
      if (this.atn != null && i > 0 && i <= this.atn.states.Count)
      {
        int ruleIndex = this.atn.states[i].ruleIndex;
        if (this.ruleNames != null && ruleIndex >= 0 && ruleIndex < this.ruleNames.Length)
          return "ctx:" + i.ToString() + "(" + this.ruleNames[ruleIndex] + ")";
      }
      return "ctx:" + i.ToString();
    }

    protected internal virtual string GetEdgeLabel(int i) => this.vocabulary.GetDisplayName(i - 1);

    internal virtual string GetStateString(DFAState s)
    {
      if (s == ATNSimulator.ERROR)
        return "ERROR";
      int stateNumber = s.stateNumber;
      string stateString = (s.isAcceptState ? ":" : "") + nameof (s) + stateNumber.ToString() + (s.requiresFullContext ? "^" : "");
      if (!s.isAcceptState)
        return stateString;
      return s.predicates != null ? stateString + "=>" + Arrays.ToString<PredPrediction>(s.predicates) : stateString + "=>" + s.prediction.ToString();
    }
  }
}
