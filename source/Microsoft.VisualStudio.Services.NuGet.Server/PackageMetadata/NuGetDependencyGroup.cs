// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.NuGetDependencyGroup
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public class NuGetDependencyGroup
  {
    public NuGetDependencyGroup(
      string targetFramework,
      bool isOldStyleDependencyList,
      IEnumerable<NuGetDependency> dependencies)
    {
      ArgumentUtility.CheckForNull<IEnumerable<NuGetDependency>>(dependencies, nameof (dependencies));
      this.Dependencies = dependencies.ToImmutableList<NuGetDependency>();
      this.TargetFramework = targetFramework;
      this.IsOldStyleDependencyList = isOldStyleDependencyList;
    }

    public string TargetFramework { get; }

    public bool IsOldStyleDependencyList { get; }

    public ImmutableList<NuGetDependency> Dependencies { get; }
  }
}
