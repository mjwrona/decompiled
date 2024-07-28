// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenIdentityUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenIdentityUtility
  {
    public static MavenPackage ToPackage(
      MavenPackageName packageName,
      MavenPackageVersion packageVersion,
      FeedCore feed,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(packageName, nameof (packageName));
      ArgumentUtility.CheckForEmptyGuid(feed.Id, "Id");
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      MavenPackage package = new MavenPackage(packageName.GroupId, packageName.ArtifactId, packageVersion?.DisplayVersion);
      if (package.HasVersion())
      {
        package.ArtifactIndex = MavenUrlUtility.GetUrlForIndex(requestContext, feed, packageName, packageVersion);
        package.VersionsIndex = MavenUrlUtility.GetUrlForIndex(requestContext, feed, packageName);
        if (MavenIdentityUtility.IsSnapshotVersion(packageVersion))
          package.SnapshotMetadata = MavenUrlUtility.GetUrlForMetadataFile(requestContext, feed, packageName, packageVersion);
      }
      package.ArtifactMetadata = MavenUrlUtility.GetUrlForMetadataFile(requestContext, feed, packageName);
      return package;
    }

    public static string GetProtocolSpecificName(MavenPackageName name)
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(name, nameof (name));
      return "maven_" + name.NormalizedName;
    }

    public static string GAV(MavenPackageName name, MavenPackageVersion version)
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(name, nameof (name));
      return MavenIdentityUtility.GAV(name.NormalizedName, version?.NormalizedVersion);
    }

    public static string GAV(string normalizedName, string normalizedVersion)
    {
      ArgumentUtility.CheckStringForAnyWhiteSpace(normalizedName, nameof (normalizedName));
      IList<string> source = (IList<string>) new List<string>()
      {
        normalizedName
      };
      if (!string.IsNullOrWhiteSpace(normalizedVersion))
        source.Add(normalizedVersion);
      return string.Join(":", source.ToArray<string>());
    }

    public static string ToPath(this MavenPackageIdentity identity) => identity.Name.GroupId.Replace('.', '/') + "/" + identity.Name.ArtifactId + "/" + identity.Version.NormalizedVersion;

    public static string PomFileName(this MavenPackageIdentity identity) => identity.Name.ArtifactId + "-" + identity.Version.DisplayVersion + ".pom";

    public static string JarFileName(this MavenPackageIdentity identity) => identity.Name.ArtifactId + "-" + identity.Version.DisplayVersion + ".jar";

    public static bool IsSnapshotVersion(MavenPackageVersion packageVersion) => MavenIdentityUtility.IsSnapshotVersion(packageVersion?.DisplayVersion);

    public static bool IsSnapshotVersion(string packageVersion) => packageVersion != null && packageVersion.EndsWith("snapshot", StringComparison.OrdinalIgnoreCase);
  }
}
