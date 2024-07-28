// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.PredicateTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class PredicateTransition : AbstractPredicateTransition
  {
    public readonly int ruleIndex;
    public readonly int predIndex;
    public readonly bool isCtxDependent;

    public PredicateTransition(ATNState target, int ruleIndex, int predIndex, bool isCtxDependent)
      : base(target)
    {
      this.ruleIndex = ruleIndex;
      this.predIndex = predIndex;
      this.isCtxDependent = isCtxDependent;
    }

    public override TransitionType TransitionType => TransitionType.PREDICATE;

    public override bool IsEpsilon => true;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    public SemanticContext.Predicate Predicate => new SemanticContext.Predicate(this.ruleIndex, this.predIndex, this.isCtxDependent);

    [return: NotNull]
    public override string ToString()
    {
      int num = this.ruleIndex;
      string str1 = num.ToString();
      num = this.predIndex;
      string str2 = num.ToString();
      return "pred_" + str1 + ":" + str2;
    }
  }
}
