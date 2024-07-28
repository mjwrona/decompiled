// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmPackageIdentity : PackageIdentityBase<NpmPackageName, SemanticVersion>
  {
    public NpmPackageIdentity(NpmPackageName name, SemanticVersion version)
      : base(name, version)
    {
    }

    public NpmPackageIdentity(string id, SemanticVersion version)
      : this(new NpmPackageName(id), version)
    {
    }

    public string ToTgzFilePath() => this.Name.UnscopedName + "-" + this.Version.NormalizedVersion + ".tgz";
  }
}
