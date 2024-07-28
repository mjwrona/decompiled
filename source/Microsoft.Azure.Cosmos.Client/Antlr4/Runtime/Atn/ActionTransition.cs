// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ActionTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Atn
{
  internal sealed class ActionTransition : Transition
  {
    public readonly int ruleIndex;
    public readonly int actionIndex;
    public readonly bool isCtxDependent;

    public ActionTransition(ATNState target, int ruleIndex)
      : this(target, ruleIndex, -1, false)
    {
    }

    public ActionTransition(ATNState target, int ruleIndex, int actionIndex, bool isCtxDependent)
      : base(target)
    {
      this.ruleIndex = ruleIndex;
      this.actionIndex = actionIndex;
      this.isCtxDependent = isCtxDependent;
    }

    public override TransitionType TransitionType => TransitionType.ACTION;

    public override bool IsEpsilon => true;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    public override string ToString()
    {
      int num = this.ruleIndex;
      string str1 = num.ToString();
      num = this.actionIndex;
      string str2 = num.ToString();
      return "action_" + str1 + ":" + str2;
    }
  }
}
