// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetGetNuspecsRequest
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetGetNuspecsRequest : 
    PackageNameRequest<VssNuGetPackageName, IImmutableList<VssNuGetPackageVersion>>
  {
    public NuGetGetNuspecsRequest(
      IPackageNameRequest<VssNuGetPackageName> packageRequest,
      IEnumerable<VssNuGetPackageVersion> versions)
      : base(packageRequest, (IImmutableList<VssNuGetPackageVersion>) (versions != null ? versions.ToImmutableArray<VssNuGetPackageVersion>() : ImmutableArray<VssNuGetPackageVersion>.Empty))
    {
    }

    public IImmutableList<VssNuGetPackageVersion> Versions => this.AdditionalData;
  }
}
