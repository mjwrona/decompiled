// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.CommonTokenStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime
{
  internal class CommonTokenStream : BufferedTokenStream
  {
    protected internal int channel;

    public CommonTokenStream(ITokenSource tokenSource)
      : base(tokenSource)
    {
    }

    public CommonTokenStream(ITokenSource tokenSource, int channel)
      : this(tokenSource)
    {
      this.channel = channel;
    }

    protected internal override int AdjustSeekIndex(int i) => this.NextTokenOnChannel(i, this.channel);

    protected internal override IToken Lb(int k)
    {
      if (k == 0 || this.p - k < 0)
        return (IToken) null;
      int index1 = this.p;
      for (int index2 = 1; index2 <= k; ++index2)
        index1 = this.PreviousTokenOnChannel(index1 - 1, this.channel);
      return index1 < 0 ? (IToken) null : this.tokens[index1];
    }

    public override IToken LT(int k)
    {
      this.LazyInit();
      if (k == 0)
        return (IToken) null;
      if (k < 0)
        return this.Lb(-k);
      int index1 = this.p;
      for (int index2 = 1; index2 < k; ++index2)
      {
        if (this.Sync(index1 + 1))
          index1 = this.NextTokenOnChannel(index1 + 1, this.channel);
      }
      return this.tokens[index1];
    }

    public virtual int GetNumberOfOnChannelTokens()
    {
      int ofOnChannelTokens = 0;
      this.Fill();
      for (int index = 0; index < this.tokens.Count; ++index)
      {
        IToken token = this.tokens[index];
        if (token.Channel == this.channel)
          ++ofOnChannelTokens;
        if (token.Type == -1)
          break;
      }
      return ofOnChannelTokens;
    }
  }
}
