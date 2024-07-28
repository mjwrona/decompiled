// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.LegacyNpmSpecificBlockedPackageIdentitiesHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class LegacyNpmSpecificBlockedPackageIdentitiesHandler
  {
    private readonly Dictionary<string, List<SemanticVersion>> blockedIdentities = new Dictionary<string, List<SemanticVersion>>()
    {
      {
        "eslint-scope",
        new List<SemanticVersion>() { new SemanticVersion(3, 7, 2) }
      },
      {
        "eslint-config-eslint",
        new List<SemanticVersion>() { new SemanticVersion(5, 0, 2) }
      },
      {
        "event-stream",
        new List<SemanticVersion>() { new SemanticVersion(3, 3, 6) }
      },
      {
        "flatmap-stream",
        new List<SemanticVersion>()
        {
          new SemanticVersion(0, 1, 0),
          new SemanticVersion(0, 1, 1),
          new SemanticVersion(0, 1, 2)
        }
      }
    };

    public void ThrowIfBlocked(NpmPackageIdentity packageIdentity)
    {
      if (this.IsBlocked(packageIdentity))
        throw new PackageBlockedException(Resources.Error_PackageVersionBlocked((object) packageIdentity.Name, (object) packageIdentity.Version));
    }

    public bool IsBlocked(NpmPackageIdentity packageIdentity) => this.blockedIdentities.ContainsKey(packageIdentity.Name.FullName) && this.blockedIdentities[packageIdentity.Name.FullName].Contains(packageIdentity.Version);

    public List<SemanticVersion> GetBlockedVersions(NpmPackageName name) => !this.blockedIdentities.ContainsKey(name.FullName) ? (List<SemanticVersion>) null : this.blockedIdentities[name.FullName];
  }
}
