// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.UnbufferedCharStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.IO;
using System.Text;

namespace Antlr4.Runtime
{
  internal class UnbufferedCharStream : ICharStream, IIntStream
  {
    protected internal int[] data;
    protected internal int n;
    protected internal int p;
    protected internal int numMarkers;
    protected internal int lastChar = -1;
    protected internal int lastCharBufferStart;
    protected internal int currentCharIndex;
    protected internal TextReader input;
    public string name;

    public UnbufferedCharStream()
      : this(256)
    {
    }

    public UnbufferedCharStream(int bufferSize)
    {
      this.n = 0;
      this.data = new int[bufferSize];
    }

    public UnbufferedCharStream(Stream input)
      : this(input, 256)
    {
    }

    public UnbufferedCharStream(TextReader input)
      : this(input, 256)
    {
    }

    public UnbufferedCharStream(Stream input, int bufferSize)
      : this(bufferSize)
    {
      this.input = (TextReader) new StreamReader(input);
      this.Fill(1);
    }

    public UnbufferedCharStream(TextReader input, int bufferSize)
      : this(bufferSize)
    {
      this.input = input;
      this.Fill(1);
    }

    public virtual void Consume()
    {
      if (this.LA(1) == -1)
        throw new InvalidOperationException("cannot consume EOF");
      this.lastChar = this.data[this.p];
      if (this.p == this.n - 1 && this.numMarkers == 0)
      {
        this.n = 0;
        this.p = -1;
        this.lastCharBufferStart = this.lastChar;
      }
      ++this.p;
      ++this.currentCharIndex;
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
        if (this.n > 0 && this.data[this.n - 1] == -1)
          return index;
        int c = this.NextChar();
        if (c > (int) ushort.MaxValue || c == -1)
        {
          this.Add(c);
        }
        else
        {
          char ch1 = (char) c;
          if (char.IsLowSurrogate(ch1))
            throw new ArgumentException("Invalid UTF-16 (low surrogate with no preceding high surrogate)");
          if (char.IsHighSurrogate(ch1))
          {
            int num = this.NextChar();
            if (num > (int) ushort.MaxValue)
              throw new ArgumentException("Invalid UTF-16 (high surrogate followed by code point > U+FFFF");
            char ch2 = num != -1 ? (char) num : throw new ArgumentException("Invalid UTF-16 (low surrogate with no preceding high surrogate)");
            if (!char.IsLowSurrogate(ch2))
              throw new ArgumentException("Invalid UTF-16 (low surrogate with no preceding high surrogate)");
            this.Add(char.ConvertToUtf32(ch1, ch2));
          }
          else
            this.Add(c);
        }
      }
      return n;
    }

    protected internal virtual int NextChar() => this.input.Read();

    protected internal virtual void Add(int c)
    {
      if (this.n >= this.data.Length)
        this.data = Arrays.CopyOf<int>(this.data, this.data.Length * 2);
      this.data[this.n++] = c;
    }

    public virtual int LA(int i)
    {
      if (i == -1)
        return this.lastChar;
      this.Sync(i);
      int index = this.p + i - 1;
      if (index < 0)
        throw new ArgumentOutOfRangeException();
      return index >= this.n ? -1 : this.data[index];
    }

    public virtual int Mark()
    {
      if (this.numMarkers == 0)
        this.lastCharBufferStart = this.lastChar;
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
      if (this.numMarkers != 0 || this.p <= 0)
        return;
      Array.Copy((Array) this.data, this.p, (Array) this.data, 0, this.n - this.p);
      this.n -= this.p;
      this.p = 0;
      this.lastCharBufferStart = this.lastChar;
    }

    public virtual int Index => this.currentCharIndex;

    public virtual void Seek(int index)
    {
      if (index == this.currentCharIndex)
        return;
      if (index > this.currentCharIndex)
      {
        this.Sync(index - this.currentCharIndex);
        index = Math.Min(index, this.BufferStartIndex + this.n - 1);
      }
      int num = index - this.BufferStartIndex;
      if (num < 0)
        throw new ArgumentException("cannot seek to negative index " + index.ToString());
      this.p = num < this.n ? num : throw new NotSupportedException("seek to index outside buffer: " + index.ToString() + " not in " + this.BufferStartIndex.ToString() + ".." + (this.BufferStartIndex + this.n).ToString());
      this.currentCharIndex = index;
      if (this.p == 0)
        this.lastChar = this.lastCharBufferStart;
      else
        this.lastChar = this.data[this.p - 1];
    }

    public virtual int Size => throw new NotSupportedException("Unbuffered stream cannot know its size");

    public virtual string SourceName => string.IsNullOrEmpty(this.name) ? "<unknown>" : this.name;

    public virtual string GetText(Interval interval)
    {
      if (interval.a < 0 || interval.b < interval.a - 1)
        throw new ArgumentException("invalid interval");
      int bufferStartIndex = this.BufferStartIndex;
      if (this.n > 0 && this.data[this.n - 1] == -1 && interval.a + interval.Length > bufferStartIndex + this.n)
        throw new ArgumentException("the interval extends past the end of the stream");
      if (interval.a < bufferStartIndex || interval.b >= bufferStartIndex + this.n)
        throw new NotSupportedException("interval " + interval.ToString() + " outside buffer: " + bufferStartIndex.ToString() + ".." + (bufferStartIndex + this.n - 1).ToString());
      int num = interval.a - bufferStartIndex;
      StringBuilder stringBuilder = new StringBuilder(interval.Length);
      for (int index = 0; index < interval.Length; ++index)
        stringBuilder.Append(char.ConvertFromUtf32(this.data[num + index]));
      return stringBuilder.ToString();
    }

    protected internal int BufferStartIndex => this.currentCharIndex - this.p;
  }
}
