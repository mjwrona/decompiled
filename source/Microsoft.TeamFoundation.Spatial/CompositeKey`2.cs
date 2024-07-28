// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.CompositeKey`2
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;

namespace Microsoft.Spatial
{
  internal class CompositeKey<T1, T2> : IEquatable<CompositeKey<T1, T2>>
  {
    private readonly T1 first;
    private readonly T2 second;

    public CompositeKey(T1 first, T2 second)
    {
      this.first = first;
      this.second = second;
    }

    public static bool operator ==(CompositeKey<T1, T2> left, CompositeKey<T1, T2> right) => object.Equals((object) left, (object) right);

    public static bool operator !=(CompositeKey<T1, T2> left, CompositeKey<T1, T2> right) => !object.Equals((object) left, (object) right);

    public bool Equals(CompositeKey<T1, T2> other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      return object.Equals((object) other.first, (object) this.first) && object.Equals((object) other.second, (object) this.second);
    }

    public override bool Equals(object obj) => this.Equals(obj as CompositeKey<T1, T2>);

    public override int GetHashCode() => this.first.GetHashCode() * 397 ^ this.second.GetHashCode();
  }
}
