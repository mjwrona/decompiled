// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerCustomAction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class LexerCustomAction : ILexerAction
  {
    private readonly int ruleIndex;
    private readonly int actionIndex;

    public LexerCustomAction(int ruleIndex, int actionIndex)
    {
      this.ruleIndex = ruleIndex;
      this.actionIndex = actionIndex;
    }

    public int RuleIndex => this.ruleIndex;

    public int ActionIndex => this.actionIndex;

    public LexerActionType ActionType => LexerActionType.Custom;

    public bool IsPositionDependent => true;

    public void Execute(Lexer lexer) => lexer.Action((RuleContext) null, this.ruleIndex, this.actionIndex);

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(), (int) this.ActionType), this.ruleIndex), this.actionIndex), 3);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is LexerCustomAction))
        return false;
      LexerCustomAction lexerCustomAction = (LexerCustomAction) obj;
      return this.ruleIndex == lexerCustomAction.ruleIndex && this.actionIndex == lexerCustomAction.actionIndex;
    }
  }
}
