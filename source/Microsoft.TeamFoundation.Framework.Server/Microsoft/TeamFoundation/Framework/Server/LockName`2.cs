// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockName`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockName<T1, T2> : ILockName, IEquatable<ILockName>, IComparable<ILockName>
    where T1 : IComparable<T1>, IEquatable<T1>
    where T2 : IComparable<T2>, IEquatable<T2>
  {
    private readonly int hashCode;
    private readonly LockLevel lockLevel;
    private readonly T1 nameValue1;
    private readonly T2 nameValue2;
    private INamedLockObject cachedLockObject;
    private readonly string toString;

    internal LockName(T1 nameValue1, T2 nameValue2, LockLevel lockLevel)
    {
      this.nameValue1 = nameValue1;
      this.nameValue2 = nameValue2;
      this.lockLevel = lockLevel;
      this.hashCode = (int) (lockLevel ^ (LockLevel) nameValue1.GetHashCode() ^ (LockLevel) nameValue2.GetHashCode());
      if (this.LockLevel == LockLevel.Host)
        this.toString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Host:[{0},{1}]", (object) this.NameValue1, (object) this.NameValue2);
      else
        this.toString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:[{1},{2}]", (object) this.LockLevel, (object) this.NameValue1, (object) this.NameValue2);
    }

    public LockLevel LockLevel => this.lockLevel;

    public INamedLockObject CachedLockObject
    {
      get => this.cachedLockObject;
      set => this.cachedLockObject = value;
    }

    public T1 NameValue1 => this.nameValue1;

    public T2 NameValue2 => this.nameValue2;

    public bool Equals(ILockName other)
    {
      if (this == other)
        return true;
      return this.lockLevel == other.LockLevel && other is LockName<T1, T2> lockName && this.hashCode == lockName.GetHashCode() && this.nameValue1.Equals(lockName.NameValue1) && this.nameValue2.Equals(lockName.NameValue2);
    }

    public int CompareTo(ILockName other)
    {
      if (this == other)
        return 0;
      int num = ((int) this.lockLevel).CompareTo((int) other.LockLevel);
      if (num == 0)
      {
        if (!(other is LockName<T1, T2> lockName))
          LockManager.RetailAssert(false, "must not have locks of different types having the same level\nthis: {0}\nother: {1}", (object) this, (object) other);
        num = this.nameValue1.CompareTo(lockName.NameValue1);
        if (num == 0)
          num = this.nameValue2.CompareTo(lockName.NameValue2);
      }
      return num;
    }

    public override int GetHashCode() => this.hashCode;

    public override string ToString() => this.toString;
  }
}
