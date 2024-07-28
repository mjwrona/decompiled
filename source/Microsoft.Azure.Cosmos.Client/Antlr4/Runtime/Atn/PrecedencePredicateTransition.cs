// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.PrecedencePredicateTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Atn
{
  internal sealed class PrecedencePredicateTransition : AbstractPredicateTransition
  {
    public readonly int precedence;

    public PrecedencePredicateTransition(ATNState target, int precedence)
      : base(target)
    {
      this.precedence = precedence;
    }

    public override TransitionType TransitionType => TransitionType.PRECEDENCE;

    public override bool IsEpsilon => true;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    public SemanticContext.PrecedencePredicate Predicate => new SemanticContext.PrecedencePredicate(this.precedence);

    public override string ToString() => this.precedence.ToString() + " >= _p";
  }
}
