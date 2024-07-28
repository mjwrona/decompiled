// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PackageStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public class PackageStore : IPackageStore
  {
    private Dictionary<string, List<PackageMetadata>> m_packages;

    public PackageStore(params PackageMetadata[] packages)
      : this((IEnumerable<PackageMetadata>) packages)
    {
    }

    public PackageStore(IEnumerable<PackageMetadata> packages = null, IPackageResolver resolver = null)
    {
      this.Resolver = resolver;
      this.m_packages = (packages != null ? packages.GroupBy<PackageMetadata, string>((Func<PackageMetadata, string>) (x => x.Type)).ToDictionary<IGrouping<string, PackageMetadata>, string, List<PackageMetadata>>((Func<IGrouping<string, PackageMetadata>, string>) (x => x.Key), (Func<IGrouping<string, PackageMetadata>, List<PackageMetadata>>) (x => x.ToList<PackageMetadata>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (Dictionary<string, List<PackageMetadata>>) null) ?? new Dictionary<string, List<PackageMetadata>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public IPackageResolver Resolver { get; }

    public PackageVersion GetLatestVersion(string packageType)
    {
      List<PackageMetadata> list;
      if (!this.m_packages.TryGetValue(packageType, out list))
      {
        IList<PackageMetadata> packages = this.Resolver?.GetPackages(packageType);
        if (packages != null && packages.Count > 0)
        {
          list = packages.ToList<PackageMetadata>();
          this.m_packages[packageType] = list;
        }
      }
      return list == null ? (PackageVersion) null : list.OrderByDescending<PackageMetadata, PackageVersion>((Func<PackageMetadata, PackageVersion>) (x => x.Version)).Select<PackageMetadata, PackageVersion>((Func<PackageMetadata, PackageVersion>) (x => x.Version)).FirstOrDefault<PackageVersion>();
    }
  }
}
