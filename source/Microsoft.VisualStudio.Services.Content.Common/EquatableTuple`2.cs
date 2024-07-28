// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.EquatableTuple`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class EquatableTuple<T1, T2> : Tuple<T1, T2>, IEquatable<EquatableTuple<T1, T2>>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
  {
    public EquatableTuple(T1 t1, T2 t2)
      : base(t1, t2)
    {
    }

    public bool Equals(EquatableTuple<T1, T2> other)
    {
      if ((object) other == null || ((object) this.Item1 == (object) other.Item1 ? 1 : ((object) this.Item1 == null ? 0 : (this.Item1.Equals(other.Item1) ? 1 : 0))) == 0)
        return false;
      if ((object) this.Item2 == (object) other.Item2)
        return true;
      return (object) this.Item2 != null && this.Item2.Equals(other.Item2);
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => this.Equals(obj as EquatableTuple<T1, T2>);

    public static bool operator ==(EquatableTuple<T1, T2> r1, EquatableTuple<T1, T2> r2) => (object) r1 != null ? r1.Equals(r2) : (object) r2 == null;

    public static bool operator !=(EquatableTuple<T1, T2> r1, EquatableTuple<T1, T2> r2) => !(r1 == r2);
  }
}
