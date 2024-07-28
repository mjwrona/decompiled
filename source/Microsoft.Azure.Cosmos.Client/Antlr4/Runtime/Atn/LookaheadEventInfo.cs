// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LookaheadEventInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Atn
{
  internal class LookaheadEventInfo : DecisionEventInfo
  {
    public LookaheadEventInfo(
      int decision,
      SimulatorState state,
      ITokenStream input,
      int startIndex,
      int stopIndex,
      bool fullCtx)
      : base(decision, state, input, startIndex, stopIndex, fullCtx)
    {
    }
  }
}
