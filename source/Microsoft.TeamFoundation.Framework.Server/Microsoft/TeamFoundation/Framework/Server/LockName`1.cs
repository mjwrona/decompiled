// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockName`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockName<T> : ILockName, IEquatable<ILockName>, IComparable<ILockName> where T : IComparable<T>, IEquatable<T>
  {
    private readonly int hashCode;
    private readonly LockLevel lockLevel;
    private readonly T nameValue;
    private readonly string toString;
    private INamedLockObject cachedLockObject;

    public LockName(T nameValue, LockLevel lockLevel)
    {
      this.nameValue = nameValue;
      this.lockLevel = lockLevel;
      this.hashCode = (int) (lockLevel ^ (LockLevel) nameValue.GetHashCode());
      if (lockLevel == LockLevel.Last)
        this.toString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Last {0}", (object) nameValue);
      else
        this.toString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) lockLevel, (object) nameValue);
    }

    public LockLevel LockLevel => this.lockLevel;

    public INamedLockObject CachedLockObject
    {
      get => this.cachedLockObject;
      set => this.cachedLockObject = value;
    }

    public T NameValue => this.nameValue;

    public bool Equals(ILockName other)
    {
      if (this == other)
        return true;
      return this.lockLevel == other.LockLevel && other is LockName<T> lockName && this.hashCode == lockName.GetHashCode() && this.nameValue.Equals(lockName.NameValue);
    }

    public int CompareTo(ILockName other)
    {
      if (this == other)
        return 0;
      int num = ((int) this.lockLevel).CompareTo((int) other.LockLevel);
      if (num == 0)
      {
        if (!(other is LockName<T> lockName))
          LockManager.RetailAssert(false, "must not have locks of different types having the same level\nthis: {0}\nother: {1}", (object) this, (object) other);
        num = this.nameValue.CompareTo(lockName.NameValue);
      }
      return num;
    }

    public override int GetHashCode() => this.hashCode;

    public override string ToString() => this.toString;
  }
}
