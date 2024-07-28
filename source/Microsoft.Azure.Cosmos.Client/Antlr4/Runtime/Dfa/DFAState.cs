// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.DFAState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Dfa
{
  internal class DFAState
  {
    public int stateNumber = -1;
    public ATNConfigSet configSet = new ATNConfigSet();
    public DFAState[] edges;
    public bool isAcceptState;
    public int prediction;
    public LexerActionExecutor lexerActionExecutor;
    public bool requiresFullContext;
    public PredPrediction[] predicates;

    public DFAState()
    {
    }

    public DFAState(int stateNumber) => this.stateNumber = stateNumber;

    public DFAState(ATNConfigSet configs) => this.configSet = configs;

    public HashSet<int> getAltSet()
    {
      HashSet<int> intSet = new HashSet<int>();
      if (this.configSet != null)
      {
        foreach (ATNConfig config in (List<ATNConfig>) this.configSet.configs)
          intSet.Add(config.alt);
      }
      return intSet.Count == 0 ? (HashSet<int>) null : intSet;
    }

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Initialize(7), this.configSet.GetHashCode()), 1);

    public override bool Equals(object o)
    {
      if (this == o)
        return true;
      return o is DFAState && this.configSet.Equals((object) ((DFAState) o).configSet);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.stateNumber).Append(":").Append((object) this.configSet);
      if (this.isAcceptState)
      {
        stringBuilder.Append("=>");
        if (this.predicates != null)
          stringBuilder.Append(Arrays.ToString<PredPrediction>(this.predicates));
        else
          stringBuilder.Append(this.prediction);
      }
      return stringBuilder.ToString();
    }
  }
}
