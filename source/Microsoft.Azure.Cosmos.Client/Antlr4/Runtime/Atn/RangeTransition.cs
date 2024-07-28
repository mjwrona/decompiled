// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.RangeTransition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal sealed class RangeTransition : Transition
  {
    public readonly int from;
    public readonly int to;

    public RangeTransition(ATNState target, int from, int to)
      : base(target)
    {
      this.from = from;
      this.to = to;
    }

    public override TransitionType TransitionType => TransitionType.RANGE;

    public override IntervalSet Label => IntervalSet.Of(this.from, this.to);

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => symbol >= this.from && symbol <= this.to;

    [return: NotNull]
    public override string ToString() => "'" + ((char) this.from).ToString() + "'..'" + ((char) this.to).ToString() + "'";
  }
}
