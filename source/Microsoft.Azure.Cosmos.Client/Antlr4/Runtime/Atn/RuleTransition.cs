// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.RuleTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime.Atn
{
  internal sealed class RuleTransition : Transition
  {
    public readonly int ruleIndex;
    public readonly int precedence;
    [NotNull]
    public ATNState followState;
    public bool tailCall;
    public bool optimizedTailCall;

    [Obsolete("UseRuleTransition(RuleStartState, int, int, ATNState) instead.")]
    public RuleTransition(RuleStartState ruleStart, int ruleIndex, ATNState followState)
      : this(ruleStart, ruleIndex, 0, followState)
    {
    }

    public RuleTransition(
      RuleStartState ruleStart,
      int ruleIndex,
      int precedence,
      ATNState followState)
      : base((ATNState) ruleStart)
    {
      this.ruleIndex = ruleIndex;
      this.precedence = precedence;
      this.followState = followState;
    }

    public override TransitionType TransitionType => TransitionType.RULE;

    public override bool IsEpsilon => true;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;
  }
}
