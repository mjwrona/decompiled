// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenMetadataEntryExtensions
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  internal static class MavenMetadataEntryExtensions
  {
    public static MavenPackage ToPackage(
      this IMavenMetadataEntry packageMetadataEntry,
      IVssRequestContext requestContext,
      FeedCore feed,
      string fileName,
      bool includePom = false)
    {
      ArgumentUtility.CheckForNull<IMavenMetadataEntry>(packageMetadataEntry, nameof (packageMetadataEntry));
      ArgumentUtility.CheckForNull<MavenPackageIdentity>(packageMetadataEntry.PackageIdentity, "PackageIdentity");
      ArgumentUtility.CheckForEmptyGuid(feed.Id, "Id");
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      MavenPackageName name = packageMetadataEntry.PackageIdentity.Name;
      MavenPackageVersion version = packageMetadataEntry.PackageIdentity.Version;
      MavenPackage package = MavenIdentityUtility.ToPackage(name, version, feed, requestContext);
      if (includePom)
        package.Pom = packageMetadataEntry.Pom ?? new MavenPomMetadata();
      package.DeletedDate = packageMetadataEntry.DeletedDate;
      if (!string.IsNullOrWhiteSpace(fileName))
        package.RequestedFile = MavenUrlUtility.GetUrlForFile(requestContext, feed, name, version, fileName);
      foreach (MavenPackageFileNew packageFile in (IEnumerable<MavenPackageFileNew>) packageMetadataEntry.PackageFiles)
        package.Files.AddLink(packageFile.Path, MavenUrlUtility.GetUrlForFile(requestContext, feed, name, version, packageFile.Path).Href);
      return package;
    }

    public static MavenPackageVersionDeletionState ToPackageVersionDeletionState(
      this IMavenMetadataEntry packageMetadataEntry)
    {
      ArgumentUtility.CheckForNull<IMavenMetadataEntry>(packageMetadataEntry, nameof (packageMetadataEntry));
      ArgumentUtility.CheckForNull<MavenPackageIdentity>(packageMetadataEntry.PackageIdentity, "PackageIdentity");
      if (!packageMetadataEntry.DeletedDate.HasValue)
        throw new ArgumentException("DeletedDate");
      MavenPackageName name = packageMetadataEntry.PackageIdentity.Name;
      MavenPackageVersion version = packageMetadataEntry.PackageIdentity.Version;
      return new MavenPackageVersionDeletionState(name.GroupId, name.ArtifactId, version.DisplayVersion, packageMetadataEntry.DeletedDate.Value);
    }

    public static IMavenMetadataEntry ThrowIfNotActive(
      this IMavenMetadataEntry entry,
      FeedCore feed)
    {
      return entry != null && !entry.IsDeleted() ? entry : throw ExceptionHelper.PackageNotFound((IPackageIdentity) entry.PackageIdentity, feed);
    }
  }
}
