// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.BufferedTokenStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime
{
  internal class BufferedTokenStream : ITokenStream, IIntStream
  {
    [NotNull]
    private ITokenSource _tokenSource;
    protected internal IList<IToken> tokens = (IList<IToken>) new List<IToken>(100);
    protected internal int p = -1;
    protected internal bool fetchedEOF;

    public BufferedTokenStream(ITokenSource tokenSource) => this._tokenSource = tokenSource != null ? tokenSource : throw new ArgumentNullException("tokenSource cannot be null");

    public virtual ITokenSource TokenSource => this._tokenSource;

    public virtual int Index => this.p;

    public virtual int Mark() => 0;

    public virtual void Release(int marker)
    {
    }

    public virtual void Reset() => this.Seek(0);

    public virtual void Seek(int index)
    {
      this.LazyInit();
      this.p = this.AdjustSeekIndex(index);
    }

    public virtual int Size => this.tokens.Count;

    public virtual void Consume()
    {
      if ((this.p < 0 || !(!this.fetchedEOF ? this.p < this.tokens.Count : this.p < this.tokens.Count - 1)) && this.LA(1) == -1)
        throw new InvalidOperationException("cannot consume EOF");
      if (!this.Sync(this.p + 1))
        return;
      this.p = this.AdjustSeekIndex(this.p + 1);
    }

    protected internal virtual bool Sync(int i)
    {
      int n = i - this.tokens.Count + 1;
      return n <= 0 || this.Fetch(n) >= n;
    }

    protected internal virtual int Fetch(int n)
    {
      if (this.fetchedEOF)
        return 0;
      for (int index = 0; index < n; ++index)
      {
        IToken token = this._tokenSource.NextToken();
        if (token is IWritableToken)
          ((IWritableToken) token).TokenIndex = this.tokens.Count;
        this.tokens.Add(token);
        if (token.Type == -1)
        {
          this.fetchedEOF = true;
          return index + 1;
        }
      }
      return n;
    }

    public virtual IToken Get(int i)
    {
      if (i < 0 || i >= this.tokens.Count)
        throw new ArgumentOutOfRangeException("token index " + i.ToString() + " out of range 0.." + (this.tokens.Count - 1).ToString());
      return this.tokens[i];
    }

    public virtual IList<IToken> Get(int start, int stop)
    {
      if (start < 0 || stop < 0)
        return (IList<IToken>) null;
      this.LazyInit();
      IList<IToken> tokenList = (IList<IToken>) new List<IToken>();
      if (stop >= this.tokens.Count)
        stop = this.tokens.Count - 1;
      for (int index = start; index <= stop; ++index)
      {
        IToken token = this.tokens[index];
        if (token.Type != -1)
          tokenList.Add(token);
        else
          break;
      }
      return tokenList;
    }

    public virtual int LA(int i) => this.LT(i).Type;

    protected internal virtual IToken Lb(int k) => this.p - k < 0 ? (IToken) null : this.tokens[this.p - k];

    [return: NotNull]
    public virtual IToken LT(int k)
    {
      this.LazyInit();
      if (k == 0)
        return (IToken) null;
      if (k < 0)
        return this.Lb(-k);
      int num = this.p + k - 1;
      this.Sync(num);
      return num >= this.tokens.Count ? this.tokens[this.tokens.Count - 1] : this.tokens[num];
    }

    protected internal virtual int AdjustSeekIndex(int i) => i;

    protected internal void LazyInit()
    {
      if (this.p != -1)
        return;
      this.Setup();
    }

    protected internal virtual void Setup()
    {
      this.Sync(0);
      this.p = this.AdjustSeekIndex(0);
    }

    public virtual void SetTokenSource(ITokenSource tokenSource)
    {
      this._tokenSource = tokenSource;
      this.tokens.Clear();
      this.p = -1;
      this.fetchedEOF = false;
    }

    public virtual IList<IToken> GetTokens() => this.tokens;

    public virtual IList<IToken> GetTokens(int start, int stop) => this.GetTokens(start, stop, (BitSet) null);

    public virtual IList<IToken> GetTokens(int start, int stop, BitSet types)
    {
      this.LazyInit();
      if (start < 0 || stop >= this.tokens.Count || stop < 0 || start >= this.tokens.Count)
        throw new ArgumentOutOfRangeException("start " + start.ToString() + " or stop " + stop.ToString() + " not in 0.." + (this.tokens.Count - 1).ToString());
      if (start > stop)
        return (IList<IToken>) null;
      IList<IToken> tokens = (IList<IToken>) new List<IToken>();
      for (int index = start; index <= stop; ++index)
      {
        IToken token = this.tokens[index];
        if (types == null || types.Get(token.Type))
          tokens.Add(token);
      }
      if (tokens.Count == 0)
        tokens = (IList<IToken>) null;
      return tokens;
    }

    public virtual IList<IToken> GetTokens(int start, int stop, int ttype)
    {
      BitSet types = new BitSet(ttype);
      types.Set(ttype);
      return this.GetTokens(start, stop, types);
    }

    protected internal virtual int NextTokenOnChannel(int i, int channel)
    {
      this.Sync(i);
      if (i >= this.Size)
        return this.Size - 1;
      for (IToken token = this.tokens[i]; token.Channel != channel && token.Type != -1; token = this.tokens[i])
      {
        ++i;
        this.Sync(i);
      }
      return i;
    }

    protected internal virtual int PreviousTokenOnChannel(int i, int channel)
    {
      this.Sync(i);
      if (i >= this.Size)
        return this.Size - 1;
      for (; i >= 0; --i)
      {
        IToken token = this.tokens[i];
        if (token.Type == -1 || token.Channel == channel)
          return i;
      }
      return i;
    }

    public virtual IList<IToken> GetHiddenTokensToRight(int tokenIndex, int channel)
    {
      this.LazyInit();
      if (tokenIndex < 0 || tokenIndex >= this.tokens.Count)
        throw new ArgumentOutOfRangeException(tokenIndex.ToString() + " not in 0.." + (this.tokens.Count - 1).ToString());
      int num = this.NextTokenOnChannel(tokenIndex + 1, 0);
      return this.FilterForChannel(tokenIndex + 1, num != -1 ? num : this.Size - 1, channel);
    }

    public virtual IList<IToken> GetHiddenTokensToRight(int tokenIndex) => this.GetHiddenTokensToRight(tokenIndex, -1);

    public virtual IList<IToken> GetHiddenTokensToLeft(int tokenIndex, int channel)
    {
      this.LazyInit();
      if (tokenIndex < 0 || tokenIndex >= this.tokens.Count)
        throw new ArgumentOutOfRangeException(tokenIndex.ToString() + " not in 0.." + (this.tokens.Count - 1).ToString());
      if (tokenIndex == 0)
        return (IList<IToken>) null;
      int num = this.PreviousTokenOnChannel(tokenIndex - 1, 0);
      return num == tokenIndex - 1 ? (IList<IToken>) null : this.FilterForChannel(num + 1, tokenIndex - 1, channel);
    }

    public virtual IList<IToken> GetHiddenTokensToLeft(int tokenIndex) => this.GetHiddenTokensToLeft(tokenIndex, -1);

    protected internal virtual IList<IToken> FilterForChannel(int from, int to, int channel)
    {
      IList<IToken> tokenList = (IList<IToken>) new List<IToken>();
      for (int index = from; index <= to; ++index)
      {
        IToken token = this.tokens[index];
        if (channel == -1)
        {
          if (token.Channel != 0)
            tokenList.Add(token);
        }
        else if (token.Channel == channel)
          tokenList.Add(token);
      }
      return tokenList.Count == 0 ? (IList<IToken>) null : tokenList;
    }

    public virtual string SourceName => this._tokenSource.SourceName;

    [return: NotNull]
    public virtual string GetText()
    {
      this.Fill();
      return this.GetText(Interval.Of(0, this.Size - 1));
    }

    [return: NotNull]
    public virtual string GetText(Interval interval)
    {
      int a = interval.a;
      int num = interval.b;
      if (a < 0 || num < 0)
        return string.Empty;
      this.LazyInit();
      if (num >= this.tokens.Count)
        num = this.tokens.Count - 1;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = a; index <= num; ++index)
      {
        IToken token = this.tokens[index];
        if (token.Type != -1)
          stringBuilder.Append(token.Text);
        else
          break;
      }
      return stringBuilder.ToString();
    }

    [return: NotNull]
    public virtual string GetText(RuleContext ctx) => this.GetText(ctx.SourceInterval);

    [return: NotNull]
    public virtual string GetText(IToken start, IToken stop) => start != null && stop != null ? this.GetText(Interval.Of(start.TokenIndex, stop.TokenIndex)) : string.Empty;

    public virtual void Fill()
    {
      this.LazyInit();
      int n = 1000;
      do
        ;
      while (this.Fetch(n) >= n);
    }
  }
}
