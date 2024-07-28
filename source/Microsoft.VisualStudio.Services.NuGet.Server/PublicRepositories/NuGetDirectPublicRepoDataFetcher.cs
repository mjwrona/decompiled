// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetDirectPublicRepoDataFetcher
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public class NuGetDirectPublicRepoDataFetcher : 
    IDirectPublicRepoDataFetcher<VssNuGetPackageName, NuGetPubCachePackageNameFile, NuGetCatalogCursor>
  {
    private readonly ITracerService tracerService;
    private readonly ITimeProvider timeProvider;
    private readonly IUpstreamNuGetClient directClient;

    public NuGetDirectPublicRepoDataFetcher(
      ITracerService tracerService,
      ITimeProvider timeProvider,
      IUpstreamNuGetClient directClient)
    {
      this.tracerService = tracerService;
      this.timeProvider = timeProvider;
      this.directClient = directClient;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public async Task<(NuGetPubCachePackageNameFile NewDoc, bool NeedSave)> FetchFromUpstreamAndApplyToDocAsync(
      VssNuGetPackageName packageName,
      NuGetPubCachePackageNameFile existingDoc,
      NuGetCatalogCursor? minValidCursor)
    {
      NuGetDirectPublicRepoDataFetcher sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (FetchFromUpstreamAndApplyToDocAsync)))
      {
        Timestamp nowTimestamp = Timestamp.FromDateTime(sendInTheThisObject.timeProvider.Now);
        Dictionary<VssNuGetPackageVersion, NuGetPubCacheVersionLevelInfo> existingVersionsByVersion = existingDoc.Versions.ToDictionary<NuGetPubCacheVersionLevelInfo, VssNuGetPackageVersion>((Func<NuGetPubCacheVersionLevelInfo, VssNuGetPackageVersion>) (x => x.Identity.Version), (IEqualityComparer<VssNuGetPackageVersion>) PackageVersionComparer.NormalizedVersion);
        IReadOnlyList<VssNuGetPackageVersion> allVersions = await sendInTheThisObject.FetchVersionListAsync(packageName);
        if (!allVersions.Any<VssNuGetPackageVersion>())
          return (BuildNewDoc((string) null, (IEnumerable<NuGetPubCacheVersionLevelInfo>) ImmutableArray<NuGetPubCacheVersionLevelInfo>.Empty, (NuGetCatalogCursor) null), true);
        NuGetPackageRegistrationState registrations = await sendInTheThisObject.directClient.GetRegistrationState(UnusableFeedRequest.Instance, packageName, (IEnumerable<VssNuGetPackageVersion>) allVersions);
        if ((object) minValidCursor != null && (object) registrations.CatalogCursorPosition != null && registrations.CatalogCursorPosition < minValidCursor)
          throw new UpstreamNotUpToDateException();
        IEnumerable<VssNuGetPackageVersion> second = existingVersionsByVersion.Where<KeyValuePair<VssNuGetPackageVersion, NuGetPubCacheVersionLevelInfo>>((Func<KeyValuePair<VssNuGetPackageVersion, NuGetPubCacheVersionLevelInfo>, bool>) (x => x.Value.Nuspec != null)).Select<KeyValuePair<VssNuGetPackageVersion, NuGetPubCacheVersionLevelInfo>, VssNuGetPackageVersion>((Func<KeyValuePair<VssNuGetPackageVersion, NuGetPubCacheVersionLevelInfo>, VssNuGetPackageVersion>) (x => x.Key));
        IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes> nuspecs = await sendInTheThisObject.directClient.GetNuspecs(UnusableFeedRequest.Instance, packageName, allVersions.Except<VssNuGetPackageVersion>(second));
        NuGetCatalogCursor generationCursorPosition = existingDoc.GenerationCursorPosition;
        if ((object) registrations.CatalogCursorPosition != null && (object) generationCursorPosition != null && registrations.CatalogCursorPosition < generationCursorPosition)
          return (existingDoc, false);
        List<NuGetPubCacheVersionLevelInfo> versionLevelInfoList = new List<NuGetPubCacheVersionLevelInfo>();
        foreach (VssNuGetPackageVersion key in (IEnumerable<VssNuGetPackageVersion>) allVersions)
        {
          NuGetPubCacheVersionLevelInfo versionLevelInfo1 = new NuGetPubCacheVersionLevelInfo()
          {
            DisplayName = packageName.DisplayName,
            DisplayVersion = key.DisplayVersion
          };
          NuGetPubCacheVersionLevelInfo versionLevelInfo2;
          if (existingVersionsByVersion.TryGetValue(key, out versionLevelInfo2) && versionLevelInfo2.Nuspec != null)
          {
            versionLevelInfo1.Nuspec = versionLevelInfo2.Nuspec;
          }
          else
          {
            ContentBytes contentBytes;
            if (nuspecs.TryGetValue(key, out contentBytes) && contentBytes != null)
            {
              byte[] numArray = contentBytes.AreBytesCompressed ? contentBytes.Content : CompressionHelper.DeflateByteArray(contentBytes.Content);
              versionLevelInfo1.Nuspec = new NuGetPubCacheVersionNuspec()
              {
                DeflatedBytes = ByteString.CopyFrom(numArray),
                FetchDate = nowTimestamp
              };
            }
          }
          NuGetRegistrationState registration;
          if (registrations.Versions.TryGetValue(key, out registration))
          {
            versionLevelInfo1.DisplayName = registration.CanonicalIdentity.Name.DisplayName;
            versionLevelInfo1.DisplayVersion = registration.CanonicalIdentity.Version.DisplayVersion;
            versionLevelInfo1.MutableInfo = NuGetDirectPublicRepoDataFetcher.ConvertRegistrationInfo(registration, nowTimestamp);
          }
          versionLevelInfoList.Add(versionLevelInfo1);
        }
        return (BuildNewDoc(versionLevelInfoList.MaxByOrDefault<NuGetPubCacheVersionLevelInfo, VssNuGetPackageVersion>((Func<NuGetPubCacheVersionLevelInfo, VssNuGetPackageVersion>) (x => x.Identity.Version))?.Identity.Name.DisplayName, (IEnumerable<NuGetPubCacheVersionLevelInfo>) versionLevelInfoList, registrations.CatalogCursorPosition), true);

        NuGetPubCachePackageNameFile BuildNewDoc(
          string? latestVersionDisplayName,
          IEnumerable<NuGetPubCacheVersionLevelInfo> versionEntries,
          NuGetCatalogCursor? catalogCursorPosition)
        {
          return new NuGetPubCachePackageNameFile()
          {
            DisplayName = latestVersionDisplayName ?? packageName.DisplayName,
            ModifiedDate = nowTimestamp,
            Versions = {
              versionEntries
            },
            DocumentVersion = existingDoc.DocumentVersion + 1L,
            GenerationCursorPosition = catalogCursorPosition
          };
        }
      }
    }

    private static NuGetPubCacheVersionMutableInfo ConvertRegistrationInfo(
      NuGetRegistrationState registration,
      Timestamp nowTimestamp)
    {
      NuGetPubCacheVersionMutableInfo versionMutableInfo = new NuGetPubCacheVersionMutableInfo()
      {
        Listed = registration.Listed,
        FetchDate = nowTimestamp,
        GenerationCursorPosition = registration.CatalogCursorPosition
      };
      if (registration.PublishDate.HasValue)
        versionMutableInfo.PublishDate = Timestamp.FromDateTime(registration.PublishDate.Value.ToUniversalTime());
      if ((object) registration.Deprecation != null)
      {
        NuGetPubCachePackageDeprecation packageDeprecation = new NuGetPubCachePackageDeprecation()
        {
          NullableMessage = registration.Deprecation.Message,
          Reasons = {
            (IEnumerable<string>) registration.Deprecation.Reasons
          }
        };
        if (registration.Deprecation.AlternatePackageName != null)
          packageDeprecation.AlternatePackage = new NuGetPubCacheAlternatePackage()
          {
            Id = registration.Deprecation.AlternatePackageName,
            NullableRange = registration.Deprecation.AlternatePackageRange
          };
        versionMutableInfo.Deprecation = packageDeprecation;
      }
      foreach (NuGetVulnerability vulnerability in (IEnumerable<NuGetVulnerability>) registration.Vulnerabilities)
        versionMutableInfo.Vulnerabilities.Add(new NuGetPubCacheVulnerability()
        {
          AdvisoryUrl = vulnerability.AdvisoryUrl,
          Severity = vulnerability.Severity
        });
      return versionMutableInfo;
    }

    private async Task<IReadOnlyList<VssNuGetPackageVersion>> FetchVersionListAsync(
      VssNuGetPackageName packageName)
    {
      try
      {
        return (IReadOnlyList<VssNuGetPackageVersion>) (await this.directClient.GetPackageVersions(UnusableFeedRequest.Instance, packageName)).Select<VersionWithSourceChain<VssNuGetPackageVersion>, VssNuGetPackageVersion>((Func<VersionWithSourceChain<VssNuGetPackageVersion>, VssNuGetPackageVersion>) (x => x.Version)).ToList<VssNuGetPackageVersion>();
      }
      catch (PackageNotFoundException ex)
      {
        return (IReadOnlyList<VssNuGetPackageVersion>) Array.Empty<VssNuGetPackageVersion>();
      }
    }
  }
}
