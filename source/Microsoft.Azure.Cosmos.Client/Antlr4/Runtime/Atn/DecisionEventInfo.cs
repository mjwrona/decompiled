// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.DecisionEventInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal class DecisionEventInfo
  {
    public readonly int decision;
    [Nullable]
    public readonly SimulatorState state;
    [NotNull]
    public readonly ITokenStream input;
    public readonly int startIndex;
    public readonly int stopIndex;
    public readonly bool fullCtx;

    public DecisionEventInfo(
      int decision,
      SimulatorState state,
      ITokenStream input,
      int startIndex,
      int stopIndex,
      bool fullCtx)
    {
      this.decision = decision;
      this.fullCtx = fullCtx;
      this.stopIndex = stopIndex;
      this.input = input;
      this.startIndex = startIndex;
      this.state = state;
    }
  }
}
