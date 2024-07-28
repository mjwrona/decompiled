// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.VssNuGetPackageName
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public sealed class VssNuGetPackageName : IPackageName, IEquatable<VssNuGetPackageName>
  {
    public VssNuGetPackageName(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.DisplayName = name;
      this.NormalizedName = name.ToLowerInvariant();
    }

    public string DisplayName { get; }

    public string NormalizedName { get; }

    public IProtocol Protocol => (IProtocol) Microsoft.VisualStudio.Services.NuGet.Server.Protocol.NuGet;

    public override string ToString() => this.DisplayName;

    public bool Equals(VssNuGetPackageName other) => PackageNameComparer.NormalizedName.Equals((IPackageName) this, (IPackageName) other);

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj is VssNuGetPackageName other && this.Equals(other);
    }

    public override int GetHashCode() => PackageNameComparer.NormalizedName.GetHashCode((IPackageName) this);
  }
}
