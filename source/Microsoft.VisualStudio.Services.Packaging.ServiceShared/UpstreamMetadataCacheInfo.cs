// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCacheInfo
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCacheInfo
  {
    public UpstreamMetadataCacheInfo()
      : this(Enumerable.Empty<IPackageName>())
    {
    }

    public UpstreamMetadataCacheInfo(IEnumerable<IPackageName> packageNames) => this.PackageNames = (ISet<IPackageName>) new SortedSet<IPackageName>(packageNames, (IComparer<IPackageName>) PackageNameComparer.NormalizedName);

    public ISet<IPackageName> PackageNames { get; }
  }
}
