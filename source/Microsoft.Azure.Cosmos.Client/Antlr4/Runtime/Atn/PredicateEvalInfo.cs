// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.PredicateEvalInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Atn
{
  internal class PredicateEvalInfo : DecisionEventInfo
  {
    public readonly SemanticContext semctx;
    public readonly int predictedAlt;
    public readonly bool evalResult;

    public PredicateEvalInfo(
      SimulatorState state,
      int decision,
      ITokenStream input,
      int startIndex,
      int stopIndex,
      SemanticContext semctx,
      bool evalResult,
      int predictedAlt)
      : base(decision, state, input, startIndex, stopIndex, state.useContext)
    {
      this.semctx = semctx;
      this.evalResult = evalResult;
      this.predictedAlt = predictedAlt;
    }
  }
}
