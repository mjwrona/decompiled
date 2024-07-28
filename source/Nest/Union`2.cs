// Decompiled with JetBrains decompiler
// Type: Nest.Union`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (UnionFormatter<,>))]
  public class Union<TFirst, TSecond>
  {
    internal readonly int Tag;
    internal readonly TFirst Item1;
    internal readonly TSecond Item2;

    public Union(TFirst item)
    {
      this.Item1 = item;
      this.Tag = 0;
    }

    public Union(TSecond item)
    {
      this.Item2 = item;
      this.Tag = 1;
    }

    public void Match(Action<TFirst> first, Action<TSecond> second)
    {
      switch (this.Tag)
      {
        case 0:
          first(this.Item1);
          break;
        case 1:
          second(this.Item2);
          break;
        default:
          throw new Exception(string.Format("Unrecognized tag value: {0}", (object) this.Tag));
      }
    }

    public T Match<T>(Func<TFirst, T> first, Func<TSecond, T> second)
    {
      switch (this.Tag)
      {
        case 0:
          return first(this.Item1);
        case 1:
          return second(this.Item2);
        default:
          throw new Exception(string.Format("Unrecognized tag value: {0}", (object) this.Tag));
      }
    }

    public static implicit operator Union<TFirst, TSecond>(TFirst first) => new Union<TFirst, TSecond>(first);

    public static implicit operator Union<TFirst, TSecond>(TSecond second) => new Union<TFirst, TSecond>(second);
  }
}
