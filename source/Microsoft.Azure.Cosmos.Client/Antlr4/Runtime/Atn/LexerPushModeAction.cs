// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerPushModeAction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class LexerPushModeAction : ILexerAction
  {
    private readonly int mode;

    public LexerPushModeAction(int mode) => this.mode = mode;

    public int Mode => this.mode;

    public LexerActionType ActionType => LexerActionType.PushMode;

    public bool IsPositionDependent => false;

    public void Execute(Lexer lexer) => lexer.PushMode(this.mode);

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(), (int) this.ActionType), this.mode), 2);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      return obj is LexerPushModeAction && this.mode == ((LexerPushModeAction) obj).mode;
    }

    public override string ToString() => string.Format("pushMode({0})", (object) this.mode);
  }
}
