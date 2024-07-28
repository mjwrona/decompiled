// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.RecognitionException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  [Serializable]
  internal class RecognitionException : Exception
  {
    private const long serialVersionUID = -3861826954750022374;
    [Nullable]
    private readonly IRecognizer recognizer;
    [Nullable]
    private readonly RuleContext ctx;
    [Nullable]
    private readonly IIntStream input;
    private IToken offendingToken;
    private int offendingState = -1;

    public RecognitionException(Lexer lexer, ICharStream input)
    {
      this.recognizer = (IRecognizer) lexer;
      this.input = (IIntStream) input;
      this.ctx = (RuleContext) null;
    }

    public RecognitionException(IRecognizer recognizer, IIntStream input, ParserRuleContext ctx)
    {
      this.recognizer = recognizer;
      this.input = input;
      this.ctx = (RuleContext) ctx;
      if (recognizer == null)
        return;
      this.offendingState = recognizer.State;
    }

    public RecognitionException(
      string message,
      IRecognizer recognizer,
      IIntStream input,
      ParserRuleContext ctx)
      : base(message)
    {
      this.recognizer = recognizer;
      this.input = input;
      this.ctx = (RuleContext) ctx;
      if (recognizer == null)
        return;
      this.offendingState = recognizer.State;
    }

    public int OffendingState
    {
      get => this.offendingState;
      protected set => this.offendingState = value;
    }

    [return: Nullable]
    public virtual IntervalSet GetExpectedTokens() => this.recognizer != null ? this.recognizer.Atn.GetExpectedTokens(this.offendingState, this.ctx) : (IntervalSet) null;

    public virtual RuleContext Context => this.ctx;

    public virtual IIntStream InputStream => this.input;

    public IToken OffendingToken
    {
      get => this.offendingToken;
      protected set => this.offendingToken = value;
    }

    public virtual IRecognizer Recognizer => this.recognizer;
  }
}
