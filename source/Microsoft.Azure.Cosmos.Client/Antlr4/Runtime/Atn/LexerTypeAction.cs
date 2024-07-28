// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerTypeAction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal class LexerTypeAction : ILexerAction
  {
    private readonly int type;

    public LexerTypeAction(int type) => this.type = type;

    public virtual int Type => this.type;

    public virtual LexerActionType ActionType => LexerActionType.Type;

    public virtual bool IsPositionDependent => false;

    public virtual void Execute(Lexer lexer) => lexer.Type = this.type;

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(), (int) this.ActionType), this.type), 2);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      return obj is LexerTypeAction && this.type == ((LexerTypeAction) obj).type;
    }

    public override string ToString() => string.Format("type({0})", (object) this.type);
  }
}
