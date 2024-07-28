// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.BaseInputCharStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  internal abstract class BaseInputCharStream : ICharStream, IIntStream
  {
    public const int ReadBufferSize = 1024;
    public const int InitialBufferSize = 1024;
    protected internal int n;
    protected internal int p;
    public string name;

    public virtual void Reset() => this.p = 0;

    public virtual void Consume()
    {
      if (this.p >= this.n)
        throw new InvalidOperationException("cannot consume EOF");
      ++this.p;
    }

    public virtual int LA(int i)
    {
      if (i == 0)
        return 0;
      if (i < 0)
      {
        ++i;
        if (this.p + i - 1 < 0)
          return -1;
      }
      return this.p + i - 1 >= this.n ? -1 : this.ValueAt(this.p + i - 1);
    }

    public virtual int Lt(int i) => this.LA(i);

    public virtual int Index => this.p;

    public virtual int Size => this.n;

    public virtual int Mark() => -1;

    public virtual void Release(int marker)
    {
    }

    public virtual void Seek(int index)
    {
      if (index <= this.p)
      {
        this.p = index;
      }
      else
      {
        index = Math.Min(index, this.n);
        while (this.p < index)
          this.Consume();
      }
    }

    public virtual string GetText(Interval interval)
    {
      int a = interval.a;
      int num = interval.b;
      if (num >= this.n)
        num = this.n - 1;
      int count = num - a + 1;
      return a >= this.n ? string.Empty : this.ConvertDataToString(a, count);
    }

    protected abstract int ValueAt(int i);

    protected abstract string ConvertDataToString(int start, int count);

    public override sealed string ToString() => this.ConvertDataToString(0, this.n);

    public virtual string SourceName => string.IsNullOrEmpty(this.name) ? "<unknown>" : this.name;
  }
}
