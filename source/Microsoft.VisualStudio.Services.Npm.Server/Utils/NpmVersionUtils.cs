// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.NpmVersionUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public static class NpmVersionUtils
  {
    public static IDictionary<string, string> GetLocalDistTags(
      this MetadataDocument<INpmMetadataEntry> doc)
    {
      return (doc.Properties?.NameMetadata is NpmLocalAndUpstreamSharedPackageMetadata nameMetadata ? nameMetadata.LocalDistributionTags : (IDictionary<string, string>) null) ?? (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public static IDictionary<string, string> GetUpstreamDistTags(
      this MetadataDocument<INpmMetadataEntry> doc)
    {
      return (doc.Properties?.NameMetadata is NpmLocalAndUpstreamSharedPackageMetadata nameMetadata ? nameMetadata.UpstreamDistributionTags : (IDictionary<string, string>) null) ?? (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public static IDictionary<string, string> GetMergedDistTags(
      this MetadataDocument<INpmMetadataEntry> doc)
    {
      IEnumerable<SemanticVersion> unpublishedVersions = doc.Entries.Where<INpmMetadataEntry>((Func<INpmMetadataEntry, bool>) (e => e.IsDeleted() || e.IsPermanentlyDeleted())).Select<INpmMetadataEntry, SemanticVersion>((Func<INpmMetadataEntry, SemanticVersion>) (e => e.PackageIdentity.Version));
      return (IDictionary<string, string>) new DistTagsUtils().MergeDistTags(doc.GetLocalDistTags(), doc.GetUpstreamDistTags(), (IEnumerable<IPackageVersion>) unpublishedVersions, (IEnumerable<IPackageVersion>) doc.Entries.Select<INpmMetadataEntry, SemanticVersion>((Func<INpmMetadataEntry, SemanticVersion>) (e => e.PackageIdentity.Version)));
    }

    public static MetadataDocument<INpmMetadataEntry> UpdateSharedMetadata(
      this MetadataDocument<INpmMetadataEntry> doc,
      object sharedMetadata)
    {
      MetadataDocumentProperties properties = new MetadataDocumentProperties()
      {
        NameMetadata = sharedMetadata,
        UpstreamsConfigurationHash = doc?.Properties?.UpstreamsConfigurationHash,
        SourceChainMap = doc?.Properties?.SourceChainMap,
        UpstreamsLastRefreshedUtc = (DateTime?) doc?.Properties?.UpstreamsLastRefreshedUtc
      };
      return new MetadataDocument<INpmMetadataEntry>(doc?.Entries, (IMetadataDocumentProperties) properties);
    }

    public static SemanticVersion ParseNpmPackageVersion(string rawVersion)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(rawVersion, nameof (rawVersion));
      rawVersion = NpmVersionUtils.SanitizeVersion(rawVersion);
      SemanticVersion semanticVersion = SemanticVersion.Parse(rawVersion);
      return new SemanticVersion(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, semanticVersion.Prerelease);
    }

    public static string GetRevision(this MetadataDocument<INpmMetadataEntry> doc) => (doc.Properties?.NameMetadata is NpmLocalAndUpstreamSharedPackageMetadata nameMetadata ? nameMetadata.Revision : (string) null) ?? Guid.Empty.ToString();

    public static bool TryParseNpmPackageVersion(string rawVersion, out SemanticVersion version)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(rawVersion, nameof (rawVersion));
      rawVersion = NpmVersionUtils.SanitizeVersion(rawVersion);
      SemanticVersion parsedVersion;
      if (!SemanticVersion.TryParse(rawVersion, out parsedVersion))
      {
        version = (SemanticVersion) null;
        return false;
      }
      version = new SemanticVersion(parsedVersion.Major, parsedVersion.Minor, parsedVersion.Patch, parsedVersion.Prerelease);
      return true;
    }

    private static string SanitizeVersion(string rawVersion)
    {
      string[] strArray = rawVersion.TrimStart('=', 'v').Split(new char[1]
      {
        '-'
      }, 2);
      string str;
      try
      {
        str = string.Join<int>(".", ((IEnumerable<string>) strArray[0].Split('.')).Select<string, int>((Func<string, int>) (t => int.Parse(t))));
      }
      catch
      {
        return rawVersion;
      }
      if (strArray.Length > 1)
        str = string.Format("{0}-{1}", (object) str, (object) strArray[1]);
      return str;
    }
  }
}
