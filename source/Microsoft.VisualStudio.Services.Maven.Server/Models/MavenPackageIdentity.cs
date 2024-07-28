// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenPackageIdentity : 
    PackageIdentityBase<MavenPackageName, MavenPackageVersion>,
    IEquatable<MavenPackageIdentity>
  {
    public MavenPackageIdentity(MavenPackageName name, MavenPackageVersion version)
      : base(name, version)
    {
    }

    public MavenPackageIdentity(string groupId, string artifactId, string version)
      : this(new MavenPackageName(groupId, artifactId), new MavenPackageVersion(version))
    {
    }

    public override string DisplayStringForMessages => MavenIdentityUtility.GAV(this.Name, this.Version);

    public override int GetHashCode() => this.Name.NormalizedName.GetHashCode() ^ this.Version.NormalizedVersion.GetHashCode();

    public bool Equals(MavenPackageIdentity other) => other != null && this.Name.NormalizedName.Equals(other.Name.NormalizedName) && this.Version.NormalizedVersion.Equals(other.Version.NormalizedVersion);

    public override bool Equals(object obj) => this.Equals(obj as MavenPackageIdentity);
  }
}
