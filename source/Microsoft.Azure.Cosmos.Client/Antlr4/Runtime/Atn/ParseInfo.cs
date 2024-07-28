// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ParseInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class ParseInfo
  {
    protected readonly ProfilingATNSimulator atnSimulator;

    public ParseInfo(ProfilingATNSimulator atnSimulator) => this.atnSimulator = atnSimulator;

    public DecisionInfo[] getDecisionInfo() => this.atnSimulator.getDecisionInfo();

    public List<int> getLLDecisions()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      List<int> llDecisions = new List<int>();
      for (int index = 0; index < decisionInfo.Length; ++index)
      {
        if (decisionInfo[index].LL_Fallback > 0L)
          llDecisions.Add(index);
      }
      return llDecisions;
    }

    public long getTotalTimeInPrediction()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      long timeInPrediction = 0;
      for (int index = 0; index < decisionInfo.Length; ++index)
        timeInPrediction += decisionInfo[index].timeInPrediction;
      return timeInPrediction;
    }

    public long getTotalSLLLookaheadOps()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      long totalSllLookaheadOps = 0;
      for (int index = 0; index < decisionInfo.Length; ++index)
        totalSllLookaheadOps += decisionInfo[index].SLL_TotalLook;
      return totalSllLookaheadOps;
    }

    public long getTotalLLLookaheadOps()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      long totalLlLookaheadOps = 0;
      for (int index = 0; index < decisionInfo.Length; ++index)
        totalLlLookaheadOps += decisionInfo[index].LL_TotalLook;
      return totalLlLookaheadOps;
    }

    public long getTotalSLLATNLookaheadOps()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      long sllatnLookaheadOps = 0;
      for (int index = 0; index < decisionInfo.Length; ++index)
        sllatnLookaheadOps += decisionInfo[index].SLL_ATNTransitions;
      return sllatnLookaheadOps;
    }

    public long getTotalLLATNLookaheadOps()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      long llatnLookaheadOps = 0;
      for (int index = 0; index < decisionInfo.Length; ++index)
        llatnLookaheadOps += decisionInfo[index].LL_ATNTransitions;
      return llatnLookaheadOps;
    }

    public long getTotalATNLookaheadOps()
    {
      DecisionInfo[] decisionInfo = this.atnSimulator.getDecisionInfo();
      long totalAtnLookaheadOps = 0;
      for (int index = 0; index < decisionInfo.Length; ++index)
        totalAtnLookaheadOps = totalAtnLookaheadOps + decisionInfo[index].SLL_ATNTransitions + decisionInfo[index].LL_ATNTransitions;
      return totalAtnLookaheadOps;
    }

    public int getDFASize()
    {
      int dfaSize = 0;
      DFA[] decisionToDfa = this.atnSimulator.decisionToDFA;
      for (int decision = 0; decision < decisionToDfa.Length; ++decision)
        dfaSize += this.getDFASize(decision);
      return dfaSize;
    }

    public int getDFASize(int decision) => this.atnSimulator.decisionToDFA[decision].states.Count;
  }
}
