// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ReverseVersionComparer`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ReverseVersionComparer<T> : 
    Comparer<IPackageVersion>,
    IEqualityComparer<IPackageVersion>
    where T : class, IPackageVersion, IComparable<T>, IEquatable<T>
  {
    public override int Compare(IPackageVersion x, IPackageVersion y)
    {
      T other = (T) x;
      T obj = (T) y;
      if ((object) other == null && (object) obj == null)
        return 0;
      if ((object) obj == null)
        return -1;
      return (object) other == null ? 1 : obj.CompareTo(other);
    }

    public bool Equals(IPackageVersion x, IPackageVersion y) => ((T) x).Equals((T) y);

    public int GetHashCode(IPackageVersion obj) => ((T) obj).GetHashCode();
  }
}
