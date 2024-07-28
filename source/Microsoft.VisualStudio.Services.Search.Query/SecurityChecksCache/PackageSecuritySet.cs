// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.PackageSecuritySet
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public class PackageSecuritySet
  {
    private readonly bool m_allPackageContainersAreAccessible;
    private readonly HashSet<PackageContainer> m_accessiblePackageContainers;

    public bool AllPackageContainersAreAccessible => this.m_allPackageContainersAreAccessible;

    public HashSet<PackageContainer> AccessiblePackageContainers => this.m_accessiblePackageContainers;

    public PackageSecuritySet(
      bool allPackageContainersAreAccessible,
      IEnumerable<PackageContainer> accessiblePackageContainers)
    {
      this.m_allPackageContainersAreAccessible = allPackageContainersAreAccessible;
      this.m_accessiblePackageContainers = new HashSet<PackageContainer>((IEqualityComparer<PackageContainer>) new PackageContainerIdComparator());
      if (this.m_allPackageContainersAreAccessible)
        return;
      this.m_accessiblePackageContainers.AddRange<PackageContainer, HashSet<PackageContainer>>(accessiblePackageContainers);
    }

    public override bool Equals(object obj)
    {
      PackageSecuritySet packageSecuritySet = obj as PackageSecuritySet;
      if (this == obj)
        return true;
      return packageSecuritySet != null && this.AllPackageContainersAreAccessible == packageSecuritySet.AllPackageContainersAreAccessible && this.ArePackageContainerSetEqual(this.AccessiblePackageContainers, packageSecuritySet.AccessiblePackageContainers);
    }

    public override int GetHashCode()
    {
      if (this.AllPackageContainersAreAccessible)
        return 1;
      int hashCode = 0;
      foreach (PackageContainer packageContainer in this.AccessiblePackageContainers)
        hashCode ^= packageContainer.GetHashCode();
      return hashCode;
    }

    private bool ArePackageContainerSetEqual(
      HashSet<PackageContainer> containerList1,
      HashSet<PackageContainer> containerList2)
    {
      if (containerList1 == null && containerList2 == null)
        return true;
      if (containerList1 == null || containerList2 == null || containerList1.GetType().FullName != containerList2.GetType().FullName || containerList1.Count != containerList2.Count)
        return false;
      foreach (PackageContainer packageContainer in containerList1)
      {
        if (!containerList2.Contains(packageContainer))
          return false;
      }
      return true;
    }
  }
}
