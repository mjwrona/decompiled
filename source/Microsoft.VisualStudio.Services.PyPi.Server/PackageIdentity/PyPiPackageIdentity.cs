// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public class PyPiPackageIdentity : 
    PackageIdentityBase<PyPiPackageName, PyPiPackageVersion>,
    IEquatable<PyPiPackageIdentity>
  {
    public PyPiPackageIdentity(PyPiPackageName name, PyPiPackageVersion version)
      : base(name, version)
    {
    }

    public bool Equals(PyPiPackageIdentity other) => object.Equals((object) this.Name.NormalizedName, (object) other.Name.NormalizedName) && object.Equals((object) this.Version.NormalizedVersion, (object) other.Version.NormalizedVersion);

    public override bool Equals(object obj) => this == obj || this.Equals(obj as PyPiPackageIdentity);

    public override int GetHashCode() => this.Name.NormalizedName.GetHashCode() ^ this.Version.NormalizedVersion.GetHashCode();
  }
}
