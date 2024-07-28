// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.NpmBlobUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.Readme;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class NpmBlobUtils
  {
    private const string NpmBlobScope = "npm";

    public static IdBlobReference GetPackageBlobReference(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      ArgumentUtility.CheckForNull<NpmPackageName>(packageName, nameof (packageName));
      ArgumentUtility.CheckForNull<SemanticVersion>(packageVersion, nameof (packageVersion));
      return new IdBlobReference(NpmBlobUtils.GetPackageBlobReferenceId(feedId, packageName, packageVersion), "npm");
    }

    public static IdBlobReference GetFileInPackageBlobReference(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      string filePath)
    {
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      ArgumentUtility.CheckForNull<NpmPackageName>(packageName, nameof (packageName));
      ArgumentUtility.CheckForNull<SemanticVersion>(packageVersion, nameof (packageVersion));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filePath, nameof (filePath));
      return new IdBlobReference(NpmBlobUtils.GetFileInPackageBlobReferenceId(feedId, packageName, packageVersion, filePath), "npm");
    }

    public static IEnumerable<IdBlobReference> GetFileInPackageBlobReferencesToDelete(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      string filePath)
    {
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      ArgumentUtility.CheckForNull<NpmPackageName>(packageName, nameof (packageName));
      ArgumentUtility.CheckForNull<SemanticVersion>(packageVersion, nameof (packageVersion));
      yield return new IdBlobReference(NpmBlobUtils.GetFileInPackageBlobReferenceId(feedId, packageName, packageVersion, filePath), "npm");
      if (NpmReadmeUtils.IsReadmeFilePath(filePath))
        yield return new IdBlobReference(NpmBlobUtils.GetOldReadmeBlobReferenceId(feedId, packageName, packageVersion), "npm");
    }

    public static IEnumerable<BlobReferenceIdentifier> GetExtraAssetsBlobRefIds(
      Guid feedId,
      NpmPackageIdentity packageIdentity,
      PackageManifest manifest)
    {
      return ((IEnumerable<KeyValuePair<string, PackageFileMetadata>>) manifest?.FilesMetadata ?? Enumerable.Empty<KeyValuePair<string, PackageFileMetadata>>()).SelectMany<KeyValuePair<string, PackageFileMetadata>, BlobReferenceIdentifier>((Func<KeyValuePair<string, PackageFileMetadata>, IEnumerable<BlobReferenceIdentifier>>) (x => NpmBlobUtils.GetReferencesForFile(feedId, packageIdentity.Name, packageIdentity.Version, x.Key, x.Value?.BlobIdentifier)));
    }

    private static IEnumerable<BlobReferenceIdentifier> GetReferencesForFile(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      string filePath,
      BlobIdentifier blobId)
    {
      return blobId == (BlobIdentifier) null ? Enumerable.Empty<BlobReferenceIdentifier>() : NpmBlobUtils.GetFileInPackageBlobReferencesToDelete(feedId, packageName, packageVersion, filePath).Select<IdBlobReference, BlobReferenceIdentifier>((Func<IdBlobReference, BlobReferenceIdentifier>) (refId => new BlobReferenceIdentifier(blobId, refId.Name, refId.Scope)));
    }

    private static string GetPackageBlobReferenceId(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      return string.Format("feed/{0}/{1}-{2}.tgz", (object) feedId, (object) packageName.FullName, (object) packageVersion);
    }

    private static string GetFileInPackageBlobReferenceId(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      string filePath)
    {
      return string.Format("feed/{0}/{1}-{2}/files/{3}", (object) feedId, (object) packageName.FullName, (object) packageVersion, (object) filePath);
    }

    private static string GetOldReadmeBlobReferenceId(
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      return string.Format("feed/{0}/{1}-{2}{3}", (object) feedId, (object) packageName.FullName, (object) packageVersion, (object) "/readme.md");
    }
  }
}
