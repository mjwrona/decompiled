// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.LexerActionExecutor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;

namespace Antlr4.Runtime.Atn
{
  internal class LexerActionExecutor
  {
    [NotNull]
    private readonly ILexerAction[] lexerActions;
    private readonly int hashCode;

    public LexerActionExecutor(ILexerAction[] lexerActions)
    {
      this.lexerActions = lexerActions;
      int hash = MurmurHash.Initialize();
      foreach (ILexerAction lexerAction in lexerActions)
        hash = MurmurHash.Update(hash, (object) lexerAction);
      this.hashCode = MurmurHash.Finish(hash, lexerActions.Length);
    }

    [return: NotNull]
    public static LexerActionExecutor Append(
      LexerActionExecutor lexerActionExecutor,
      ILexerAction lexerAction)
    {
      if (lexerActionExecutor == null)
        return new LexerActionExecutor(new ILexerAction[1]
        {
          lexerAction
        });
      ILexerAction[] lexerActions = Arrays.CopyOf<ILexerAction>(lexerActionExecutor.lexerActions, lexerActionExecutor.lexerActions.Length + 1);
      lexerActions[lexerActions.Length - 1] = lexerAction;
      return new LexerActionExecutor(lexerActions);
    }

    public virtual LexerActionExecutor FixOffsetBeforeMatch(int offset)
    {
      ILexerAction[] lexerActions = (ILexerAction[]) null;
      for (int index = 0; index < this.lexerActions.Length; ++index)
      {
        if (this.lexerActions[index].IsPositionDependent && !(this.lexerActions[index] is LexerIndexedCustomAction))
        {
          if (lexerActions == null)
            lexerActions = (ILexerAction[]) this.lexerActions.Clone();
          lexerActions[index] = (ILexerAction) new LexerIndexedCustomAction(offset, this.lexerActions[index]);
        }
      }
      return lexerActions == null ? this : new LexerActionExecutor(lexerActions);
    }

    [NotNull]
    public virtual ILexerAction[] LexerActions => this.lexerActions;

    public virtual void Execute(Lexer lexer, ICharStream input, int startIndex)
    {
      bool flag = false;
      int index = input.Index;
      try
      {
        foreach (ILexerAction lexerAction in this.lexerActions)
        {
          if (lexerAction is LexerIndexedCustomAction)
          {
            int offset = ((LexerIndexedCustomAction) lexerAction).Offset;
            input.Seek(startIndex + offset);
            lexerAction = ((LexerIndexedCustomAction) lexerAction).Action;
            flag = startIndex + offset != index;
          }
          else if (lexerAction.IsPositionDependent)
          {
            input.Seek(index);
            flag = false;
          }
          lexerAction.Execute(lexer);
        }
      }
      finally
      {
        if (flag)
          input.Seek(index);
      }
    }

    public override int GetHashCode() => this.hashCode;

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is LexerActionExecutor))
        return false;
      LexerActionExecutor lexerActionExecutor = (LexerActionExecutor) obj;
      return this.hashCode == lexerActionExecutor.hashCode && Arrays.Equals<ILexerAction>(this.lexerActions, lexerActionExecutor.lexerActions);
    }
  }
}
