// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.SetTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal class SetTransition : Transition
  {
    [NotNull]
    public readonly IntervalSet set;

    public SetTransition(ATNState target, IntervalSet set)
      : base(target)
    {
      if (set == null)
        set = IntervalSet.Of(0);
      this.set = set;
    }

    public override TransitionType TransitionType => TransitionType.SET;

    public override IntervalSet Label => this.set;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => this.set.Contains(symbol);

    [return: NotNull]
    public override string ToString() => this.set.ToString();
  }
}
