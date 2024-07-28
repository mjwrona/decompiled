// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.InterpreterRuleContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime
{
  internal class InterpreterRuleContext : ParserRuleContext
  {
    private readonly int ruleIndex;

    public InterpreterRuleContext(ParserRuleContext parent, int invokingStateNumber, int ruleIndex)
      : base(parent, invokingStateNumber)
    {
      this.ruleIndex = ruleIndex;
    }

    public override int RuleIndex => this.ruleIndex;
  }
}
