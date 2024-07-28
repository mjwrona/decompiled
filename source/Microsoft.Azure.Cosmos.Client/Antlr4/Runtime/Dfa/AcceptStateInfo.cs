// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.AcceptStateInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;

namespace Antlr4.Runtime.Dfa
{
  internal class AcceptStateInfo
  {
    private readonly int prediction;
    private readonly LexerActionExecutor lexerActionExecutor;

    public AcceptStateInfo(int prediction)
    {
      this.prediction = prediction;
      this.lexerActionExecutor = (LexerActionExecutor) null;
    }

    public AcceptStateInfo(int prediction, LexerActionExecutor lexerActionExecutor)
    {
      this.prediction = prediction;
      this.lexerActionExecutor = lexerActionExecutor;
    }

    public virtual int Prediction => this.prediction;

    public virtual LexerActionExecutor LexerActionExecutor => this.lexerActionExecutor;
  }
}
