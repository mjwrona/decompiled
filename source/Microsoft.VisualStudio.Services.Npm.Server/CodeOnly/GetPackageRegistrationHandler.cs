// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.GetPackageRegistrationHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class GetPackageRegistrationHandler : 
    IAsyncHandler<
    #nullable disable
    IPackageNameRequest<NpmPackageName>, string>,
    IHaveInputType<IPackageNameRequest<NpmPackageName>>,
    IHaveOutputType<string>
  {
    private const string createdTimeKey = "created";
    private const string modifiedTimeKey = "modified";
    internal static readonly string[] propertiesToRemove = new string[6]
    {
      "dist",
      "_inCache",
      "_npmOperationalData",
      "_resolved",
      "_shasum",
      "_id"
    };
    private readonly ITracerService tracerService;
    private readonly IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService;
    private readonly INpmUriBuilder uriBuilder;
    private readonly IAsyncHandler<IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>, IDictionary<string, string>> distTagProvider;

    public GetPackageRegistrationHandler(
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService,
      INpmUriBuilder uriBuilder,
      IAsyncHandler<IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>, IDictionary<string, string>> distTagProvider,
      ITracerService tracerService)
    {
      this.uriBuilder = uriBuilder;
      this.distTagProvider = distTagProvider;
      this.tracerService = tracerService ?? throw new ArgumentNullException(nameof (tracerService));
      this.metadataService = metadataService;
    }

    public async Task<string> Handle(
      IPackageNameRequest<NpmPackageName> packageNameRequest)
    {
      GetPackageRegistrationHandler sendInTheThisObject = this;
      string str;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        PackageNameQuery<INpmMetadataEntry> packageNameRequest1 = new PackageNameQuery<INpmMetadataEntry>((IPackageNameRequest) packageNameRequest);
        MetadataDocument<INpmMetadataEntry> packageDoc = await sendInTheThisObject.metadataService.GetPackageVersionStatesDocumentAsync(packageNameRequest1);
        if (packageDoc == null)
          throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) packageNameRequest.PackageName, (object) packageNameRequest.Feed.FullyQualifiedName));
        IDictionary<string, string> tags = await sendInTheThisObject.distTagProvider.Handle((IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>) packageNameRequest.WithData<NpmPackageName, MetadataDocument<INpmMetadataEntry>>(packageDoc));
        string revision = packageDoc.GetRevision();
        str = sendInTheThisObject.BuildPackageJsonFromMetadata(packageNameRequest, packageDoc.Entries, tags, revision).Json ?? throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) packageNameRequest.PackageName, (object) packageNameRequest.Feed.FullyQualifiedName));
      }
      return str;
    }

    private VersionMetadata BuildFromMetadata(
      IPackageNameRequest<NpmPackageName> packageNameRequest,
      INpmMetadataEntry entry)
    {
      string str = entry.PackageJson.Name + "@" + entry.PackageIdentity.Version.ToString();
      VersionMetadata versionMetadata1 = new VersionMetadata(entry.PackageJson);
      versionMetadata1.Version = entry.PackageIdentity.Version.ToString();
      versionMetadata1.Deprecated = entry.Deprecated;
      versionMetadata1.Id = str;
      versionMetadata1.IsPublished = entry.IsDeleted();
      VersionMetadata versionMetadata2 = versionMetadata1;
      if (versionMetadata2.AdditionalData != null)
      {
        foreach (string key in GetPackageRegistrationHandler.propertiesToRemove)
          versionMetadata2.AdditionalData.Remove(key);
      }
      versionMetadata2.Distribution = new Distribution()
      {
        Tarball = this.uriBuilder.GetPackageDownloadRedirectUri(packageNameRequest.WithPackageName<NpmPackageName>(new NpmPackageName(versionMetadata2.Name)), versionMetadata2.Version),
        ShaSum = entry.PackageSha1Sum
      };
      GetPackageRegistrationHandler.NormalizeBinariesKeys(versionMetadata2, packageNameRequest.PackageName);
      return versionMetadata2;
    }

    public static void NormalizeBinariesKeys(
      VersionMetadata versionMetadata,
      NpmPackageName packageName)
    {
      if (versionMetadata.Binaries == null)
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string key1 in versionMetadata.Binaries.Keys)
      {
        string key2 = string.IsNullOrWhiteSpace(key1) ? packageName.UnscopedName : key1;
        string binary = versionMetadata.Binaries[key1];
        dictionary.Add(key2, binary);
      }
      versionMetadata.Binaries = dictionary;
    }

    private NpmRegistration BuildPackageJsonFromMetadata(
      IPackageNameRequest<NpmPackageName> packageNameRequest,
      List<INpmMetadataEntry> allVersions,
      IDictionary<string, string> tags,
      string revision)
    {
      List<SemanticVersion> blockedVersions = new LegacyNpmSpecificBlockedPackageIdentitiesHandler().GetBlockedVersions(packageNameRequest.PackageName);
      Dictionary<string, VersionMetadata> source = new Dictionary<string, VersionMetadata>();
      Dictionary<string, object> time = new Dictionary<string, object>();
      time.Add("created", (object) DateTime.MaxValue.ToUniversalTime());
      time.Add("modified", (object) DateTime.MinValue.ToUniversalTime());
      for (int index = allVersions.Count - 1; index >= 0; --index)
      {
        INpmMetadataEntry allVersion = allVersions[index];
        this.UpdateCreated(time, allVersion.CreatedDate);
        this.UpdateModified(time, allVersion.ModifiedDate);
        this.UpdateModified(time, allVersion.CreatedDate);
        if ((blockedVersions == null || !blockedVersions.Contains(allVersion.PackageIdentity.Version)) && !allVersion.IsDeleted())
        {
          VersionMetadata versionMetadata = this.BuildFromMetadata(packageNameRequest, allVersion);
          source.Add(versionMetadata.Version, versionMetadata);
          time.Add(versionMetadata.Version, (object) allVersion.CreatedDate);
        }
      }
      if (!source.Any<KeyValuePair<string, VersionMetadata>>())
        return new NpmRegistration((string) null, (HashSet<SemanticVersion>) null);
      return new NpmRegistration(JsonConvert.SerializeObject((object) new Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata()
      {
        Id = packageNameRequest.PackageName.FullName,
        Name = packageNameRequest.PackageName.FullName,
        Revision = revision,
        Versions = (IDictionary<string, VersionMetadata>) source,
        DistributionTags = tags,
        Time = (IDictionary<string, object>) time
      }), (HashSet<SemanticVersion>) null);
    }

    private void UpdateModified(Dictionary<string, object> time, DateTime newTime)
    {
      newTime = newTime.ToUniversalTime();
      if (!((DateTime) time["modified"] < newTime))
        return;
      time["modified"] = (object) newTime;
    }

    private void UpdateCreated(Dictionary<string, object> time, DateTime newTime)
    {
      newTime = newTime.ToUniversalTime();
      if (!((DateTime) time["created"] > newTime))
        return;
      time["created"] = (object) newTime;
    }
  }
}
