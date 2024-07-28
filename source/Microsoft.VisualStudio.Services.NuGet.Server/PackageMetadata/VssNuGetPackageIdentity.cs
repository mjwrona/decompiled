// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.VssNuGetPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using NuGet.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public class VssNuGetPackageIdentity : 
    PackageIdentityBase<VssNuGetPackageName, VssNuGetPackageVersion>,
    IEquatable<VssNuGetPackageIdentity>
  {
    public VssNuGetPackageIdentity(VssNuGetPackageName name, VssNuGetPackageVersion version)
      : base(name, version)
    {
    }

    public VssNuGetPackageIdentity(string id, string version)
      : this(new VssNuGetPackageName(id), new VssNuGetPackageVersion(version))
    {
    }

    public VssNuGetPackageIdentity(RawPackageIdentity rawPackageIdentity)
      : this(rawPackageIdentity.Id, rawPackageIdentity.Version)
    {
    }

    public VssNuGetPackageIdentity(string id, NuGetVersion version)
      : this(new VssNuGetPackageName(id), new VssNuGetPackageVersion(version))
    {
    }

    public override int GetHashCode() => this.Name.NormalizedName.GetHashCode() ^ this.Version.NormalizedVersion.GetHashCode();

    public bool Equals(VssNuGetPackageIdentity other) => other != null && this.Name.NormalizedName.Equals(other.Name.NormalizedName) && this.Version.NormalizedVersion.Equals(other.Version.NormalizedVersion);

    public override bool Equals(object obj) => this.Equals(obj as VssNuGetPackageIdentity);

    public string ToNupkgFilePath() => this.Name.NormalizedName + "." + this.Version.NormalizedVersion + ".nupkg";
  }
}
