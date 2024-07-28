// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.NuGetSearchResultVersionSummary
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class NuGetSearchResultVersionSummary
  {
    public VssNuGetPackageIdentity PackageIdentity { get; }

    public bool IsDeleted { get; }

    public bool IsLocal { get; }

    public bool IsListed { get; }

    public bool IsPrerelease => this.PackageIdentity.Version.NuGetVersion.IsPrerelease;

    public bool IsSemVer2 => this.PackageIdentity.Version.NuGetVersion.IsSemVer2;

    public ImmutableList<Guid> Views { get; }

    public NuGetSearchResultVersionSummary(
      VssNuGetPackageIdentity packageIdentity,
      bool isListed,
      IEnumerable<Guid> viewIds,
      bool isDeleted,
      bool isLocal)
    {
      this.PackageIdentity = packageIdentity;
      this.IsListed = isListed;
      this.IsDeleted = isDeleted;
      this.IsLocal = isLocal;
      this.Views = (viewIds != null ? viewIds.ToImmutableList<Guid>() : (ImmutableList<Guid>) null) ?? ImmutableList<Guid>.Empty;
    }
  }
}
