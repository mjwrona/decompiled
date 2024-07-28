// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.EpsilonTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class EpsilonTransition : Transition
  {
    private readonly int outermostPrecedenceReturn;

    public EpsilonTransition(ATNState target)
      : this(target, -1)
    {
    }

    public EpsilonTransition(ATNState target, int outermostPrecedenceReturn)
      : base(target)
    {
      this.outermostPrecedenceReturn = outermostPrecedenceReturn;
    }

    public int OutermostPrecedenceReturn => this.outermostPrecedenceReturn;

    public override TransitionType TransitionType => TransitionType.EPSILON;

    public override bool IsEpsilon => true;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    [return: NotNull]
    public override string ToString() => "epsilon";
  }
}
