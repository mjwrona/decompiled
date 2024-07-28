// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.SimulatorState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Dfa;

namespace Antlr4.Runtime.Atn
{
  internal class SimulatorState
  {
    public readonly ParserRuleContext outerContext;
    public readonly DFAState s0;
    public readonly bool useContext;
    public readonly ParserRuleContext remainingOuterContext;

    public SimulatorState(
      ParserRuleContext outerContext,
      DFAState s0,
      bool useContext,
      ParserRuleContext remainingOuterContext)
    {
      this.outerContext = outerContext != null ? outerContext : ParserRuleContext.EmptyContext;
      this.s0 = s0;
      this.useContext = useContext;
      this.remainingOuterContext = remainingOuterContext;
    }
  }
}
