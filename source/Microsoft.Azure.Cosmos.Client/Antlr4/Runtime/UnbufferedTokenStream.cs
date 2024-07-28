// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.UnbufferedTokenStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Text;

namespace Antlr4.Runtime
{
  internal class UnbufferedTokenStream : ITokenStream, IIntStream
  {
    private ITokenSource _tokenSource;
    protected internal IToken[] tokens;
    protected internal int n;
    protected internal int p;
    protected internal int numMarkers;
    protected internal IToken lastToken;
    protected internal IToken lastTokenBufferStart;
    protected internal int currentTokenIndex;

    public UnbufferedTokenStream(ITokenSource tokenSource)
      : this(tokenSource, 256)
    {
    }

    public UnbufferedTokenStream(ITokenSource tokenSource, int bufferSize)
    {
      this.TokenSource = tokenSource;
      this.tokens = new IToken[bufferSize];
      this.n = 0;
      this.Fill(1);
    }

    public virtual IToken Get(int i)
    {
      int bufferStartIndex = this.GetBufferStartIndex();
      if (i < bufferStartIndex || i >= bufferStartIndex + this.n)
        throw new ArgumentOutOfRangeException("get(" + i.ToString() + ") outside buffer: " + bufferStartIndex.ToString() + ".." + (bufferStartIndex + this.n).ToString());
      return this.tokens[i - bufferStartIndex];
    }

    public virtual IToken LT(int i)
    {
      if (i == -1)
        return this.lastToken;
      this.Sync(i);
      int index = this.p + i - 1;
      if (index < 0)
        throw new ArgumentOutOfRangeException("LT(" + i.ToString() + ") gives negative index");
      return index >= this.n ? this.tokens[this.n - 1] : this.tokens[index];
    }

    public virtual int LA(int i) => this.LT(i).Type;

    public virtual ITokenSource TokenSource
    {
      get => this._tokenSource;
      set => this._tokenSource = value;
    }

    [return: NotNull]
    public virtual string GetText() => string.Empty;

    [return: NotNull]
    public virtual string GetText(RuleContext ctx) => this.GetText(ctx.SourceInterval);

    [return: NotNull]
    public virtual string GetText(IToken start, IToken stop)
    {
      if (start != null && stop != null)
        return this.GetText(Interval.Of(start.TokenIndex, stop.TokenIndex));
      throw new NotSupportedException("The specified start and stop symbols are not supported.");
    }

    public virtual void Consume()
    {
      if (this.LA(1) == -1)
        throw new InvalidOperationException("cannot consume EOF");
      this.lastToken = this.tokens[this.p];
      if (this.p == this.n - 1 && this.numMarkers == 0)
      {
        this.n = 0;
        this.p = -1;
        this.lastTokenBufferStart = this.lastToken;
      }
      ++this.p;
      ++this.currentTokenIndex;
      this.Sync(1);
    }

    protected internal virtual void Sync(int want)
    {
      int n = this.p + want - 1 - this.n + 1;
      if (n <= 0)
        return;
      this.Fill(n);
    }

    protected internal virtual int Fill(int n)
    {
      for (int index = 0; index < n; ++index)
      {
        if (this.n > 0 && this.tokens[this.n - 1].Type == -1)
          return index;
        this.Add(this.TokenSource.NextToken());
      }
      return n;
    }

    protected internal virtual void Add(IToken t)
    {
      if (this.n >= this.tokens.Length)
        this.tokens = Arrays.CopyOf<IToken>(this.tokens, this.tokens.Length * 2);
      if (t is IWritableToken)
        ((IWritableToken) t).TokenIndex = this.GetBufferStartIndex() + this.n;
      this.tokens[this.n++] = t;
    }

    public virtual int Mark()
    {
      if (this.numMarkers == 0)
        this.lastTokenBufferStart = this.lastToken;
      int num = -this.numMarkers - 1;
      ++this.numMarkers;
      return num;
    }

    public virtual void Release(int marker)
    {
      int num = -this.numMarkers;
      if (marker != num)
        throw new InvalidOperationException("release() called with an invalid marker.");
      --this.numMarkers;
      if (this.numMarkers != 0)
        return;
      if (this.p > 0)
      {
        Array.Copy((Array) this.tokens, this.p, (Array) this.tokens, 0, this.n - this.p);
        this.n -= this.p;
        this.p = 0;
      }
      this.lastTokenBufferStart = this.lastToken;
    }

    public virtual int Index => this.currentTokenIndex;

    public virtual void Seek(int index)
    {
      if (index == this.currentTokenIndex)
        return;
      if (index > this.currentTokenIndex)
      {
        this.Sync(index - this.currentTokenIndex);
        index = Math.Min(index, this.GetBufferStartIndex() + this.n - 1);
      }
      int bufferStartIndex = this.GetBufferStartIndex();
      int num = index - bufferStartIndex;
      if (num < 0)
        throw new ArgumentException("cannot seek to negative index " + index.ToString());
      this.p = num < this.n ? num : throw new NotSupportedException("seek to index outside buffer: " + index.ToString() + " not in " + bufferStartIndex.ToString() + ".." + (bufferStartIndex + this.n).ToString());
      this.currentTokenIndex = index;
      if (this.p == 0)
        this.lastToken = this.lastTokenBufferStart;
      else
        this.lastToken = this.tokens[this.p - 1];
    }

    public virtual int Size => throw new NotSupportedException("Unbuffered stream cannot know its size");

    public virtual string SourceName => this.TokenSource.SourceName;

    [return: NotNull]
    public virtual string GetText(Interval interval)
    {
      int bufferStartIndex = this.GetBufferStartIndex();
      int num1 = bufferStartIndex + this.tokens.Length - 1;
      int a = interval.a;
      int b = interval.b;
      if (a < bufferStartIndex || b > num1)
        throw new NotSupportedException("interval " + interval.ToString() + " not in token buffer window: " + bufferStartIndex.ToString() + ".." + num1.ToString());
      int num2 = a - bufferStartIndex;
      int num3 = b - bufferStartIndex;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = num2; index <= num3; ++index)
      {
        IToken token = this.tokens[index];
        stringBuilder.Append(token.Text);
      }
      return stringBuilder.ToString();
    }

    protected internal int GetBufferStartIndex() => this.currentTokenIndex - this.p;
  }
}
