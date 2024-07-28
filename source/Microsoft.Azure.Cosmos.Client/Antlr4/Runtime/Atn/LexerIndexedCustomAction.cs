// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerIndexedCustomAction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class LexerIndexedCustomAction : ILexerAction
  {
    private readonly int offset;
    private readonly ILexerAction action;

    public LexerIndexedCustomAction(int offset, ILexerAction action)
    {
      this.offset = offset;
      this.action = action;
    }

    public int Offset => this.offset;

    [NotNull]
    public ILexerAction Action => this.action;

    public LexerActionType ActionType => this.action.ActionType;

    public bool IsPositionDependent => true;

    public void Execute(Lexer lexer) => this.action.Execute(lexer);

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(), this.offset), (object) this.action), 2);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is LexerIndexedCustomAction))
        return false;
      LexerIndexedCustomAction indexedCustomAction = (LexerIndexedCustomAction) obj;
      return this.offset == indexedCustomAction.offset && this.action.Equals((object) indexedCustomAction.action);
    }
  }
}
