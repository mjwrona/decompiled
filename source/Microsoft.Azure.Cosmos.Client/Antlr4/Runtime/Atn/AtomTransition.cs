// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.AtomTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class AtomTransition : Transition
  {
    public readonly int token;

    public AtomTransition(ATNState target, int token)
      : base(target)
    {
      this.token = token;
    }

    public override TransitionType TransitionType => TransitionType.ATOM;

    public override IntervalSet Label => IntervalSet.Of(this.token);

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => this.token == symbol;

    [return: NotNull]
    public override string ToString() => this.token.ToString();
  }
}
