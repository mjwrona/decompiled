// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.DecisionInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class DecisionInfo
  {
    public readonly int decision;
    public long invocations;
    public long timeInPrediction;
    public long SLL_TotalLook;
    public long SLL_MinLook;
    public long SLL_MaxLook;
    public LookaheadEventInfo SLL_MaxLookEvent;
    public long LL_TotalLook;
    public long LL_MinLook;
    public long LL_MaxLook;
    public LookaheadEventInfo LL_MaxLookEvent;
    public readonly List<ContextSensitivityInfo> contextSensitivities = new List<ContextSensitivityInfo>();
    public readonly List<ErrorInfo> errors = new List<ErrorInfo>();
    public readonly List<AmbiguityInfo> ambiguities = new List<AmbiguityInfo>();
    public readonly List<PredicateEvalInfo> predicateEvals = new List<PredicateEvalInfo>();
    public long SLL_ATNTransitions;
    public long SLL_DFATransitions;
    public long LL_Fallback;
    public long LL_ATNTransitions;
    public long LL_DFATransitions;

    public DecisionInfo(int decision) => this.decision = decision;

    public override string ToString() => "{decision=" + this.decision.ToString() + ", contextSensitivities=" + this.contextSensitivities.Count.ToString() + ", errors=" + this.errors.Count.ToString() + ", ambiguities=" + this.ambiguities.Count.ToString() + ", SLL_lookahead=" + this.SLL_TotalLook.ToString() + ", SLL_ATNTransitions=" + this.SLL_ATNTransitions.ToString() + ", SLL_DFATransitions=" + this.SLL_DFATransitions.ToString() + ", LL_Fallback=" + this.LL_Fallback.ToString() + ", LL_lookahead=" + this.LL_TotalLook.ToString() + ", LL_ATNTransitions=" + this.LL_ATNTransitions.ToString() + "}";
  }
}
