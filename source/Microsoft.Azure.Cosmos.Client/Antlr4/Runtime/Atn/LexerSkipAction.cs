// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerSkipAction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class LexerSkipAction : ILexerAction
  {
    public static readonly LexerSkipAction Instance = new LexerSkipAction();

    private LexerSkipAction()
    {
    }

    public LexerActionType ActionType => LexerActionType.Skip;

    public bool IsPositionDependent => false;

    public void Execute(Lexer lexer) => lexer.Skip();

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Initialize(), (int) this.ActionType), 1);

    public override bool Equals(object obj) => obj == this;

    public override string ToString() => "skip";
  }
}
