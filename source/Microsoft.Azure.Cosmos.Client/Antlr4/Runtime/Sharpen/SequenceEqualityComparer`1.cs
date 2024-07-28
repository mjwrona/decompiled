// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.SequenceEqualityComparer`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Linq;

namespace Antlr4.Runtime.Sharpen
{
  internal class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>
  {
    private static readonly SequenceEqualityComparer<T> _default = new SequenceEqualityComparer<T>();
    private readonly IEqualityComparer<T> _elementEqualityComparer = (IEqualityComparer<T>) EqualityComparer<T>.Default;

    public SequenceEqualityComparer()
      : this((IEqualityComparer<T>) null)
    {
    }

    public SequenceEqualityComparer(IEqualityComparer<T> elementComparer) => this._elementEqualityComparer = elementComparer ?? (IEqualityComparer<T>) EqualityComparer<T>.Default;

    public static SequenceEqualityComparer<T> Default => SequenceEqualityComparer<T>._default;

    public override bool Equals(IEnumerable<T> x, IEnumerable<T> y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.SequenceEqual<T>(y, this._elementEqualityComparer);
    }

    public override int GetHashCode(IEnumerable<T> obj)
    {
      if (obj == null)
        return 0;
      int hashCode = 1;
      foreach (T obj1 in obj)
        hashCode = 31 * hashCode + this._elementEqualityComparer.GetHashCode(obj1);
      return hashCode;
    }
  }
}
