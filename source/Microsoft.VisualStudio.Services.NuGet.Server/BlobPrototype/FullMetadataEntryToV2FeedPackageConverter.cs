// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.FullMetadataEntryToV2FeedPackageConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class FullMetadataEntryToV2FeedPackageConverter : 
    IConverter<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>, ServerV2FeedPackage>,
    IHaveInputType<IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry>>,
    IHaveOutputType<ServerV2FeedPackage>
  {
    private readonly INuGetLicenseUriCalculator licenseUriCalculator;
    private readonly INuGetIconUriCalculator iconUriCalculator;

    public FullMetadataEntryToV2FeedPackageConverter(
      INuGetLicenseUriCalculator licenseUriCalculator,
      INuGetIconUriCalculator iconUriCalculator)
    {
      this.licenseUriCalculator = licenseUriCalculator;
      this.iconUriCalculator = iconUriCalculator;
    }

    public ServerV2FeedPackage Convert(
      IPackageRequest<VssNuGetPackageIdentity, INuGetMetadataEntry> request)
    {
      INuGetMetadataEntry additionalData = request.AdditionalData;
      NuGetPackageMetadata metadata = additionalData.Metadata;
      PackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata> request1 = request.WithData<VssNuGetPackageIdentity, NuGetPackageMetadata>(metadata);
      ServerV2FeedPackage serverV2FeedPackage1 = new ServerV2FeedPackage();
      serverV2FeedPackage1.PackageIdentity = metadata.Identity;
      serverV2FeedPackage1.PackageStorageId = additionalData.PackageStorageId;
      serverV2FeedPackage1.Id = metadata.Identity.Name.DisplayName;
      serverV2FeedPackage1.Version = metadata.Identity.Version.DisplayVersion;
      serverV2FeedPackage1.NormalizedVersion = metadata.Identity.Version.NormalizedOriginalCaseVersion;
      ServerV2FeedPackage serverV2FeedPackage2 = serverV2FeedPackage1;
      string str;
      if (metadata.Authors == null)
        str = (string) null;
      else
        str = string.Join(", ", new string[1]
        {
          metadata.Authors
        });
      serverV2FeedPackage2.Authors = str;
      serverV2FeedPackage1.Created = additionalData.CreatedDate;
      serverV2FeedPackage1.Copyright = metadata.Copyright;
      serverV2FeedPackage1.Dependencies = NuGetV2DependencyUtils.FlattenDependencies((IReadOnlyList<NuGetDependencyGroup>) metadata.DependencyGroups);
      serverV2FeedPackage1.Description = metadata.Description;
      serverV2FeedPackage1.DownloadCount = 0;
      serverV2FeedPackage1.IconUrl = this.iconUriCalculator.GetIconUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request1);
      serverV2FeedPackage1.IsPrerelease = metadata.Identity.Version.NuGetVersion.IsPrerelease;
      serverV2FeedPackage1.Language = metadata.Language;
      serverV2FeedPackage1.LastUpdated = additionalData.ModifiedDate;
      serverV2FeedPackage1.Published = additionalData.CreatedDate;
      serverV2FeedPackage1.PackageSize = additionalData.PackageSize;
      serverV2FeedPackage1.ProjectUrl = metadata.ProjectUrl;
      serverV2FeedPackage1.ReleaseNotes = metadata.ReleaseNotes;
      serverV2FeedPackage1.RequireLicenseAcceptance = metadata.RequireLicenseAcceptance.GetValueOrDefault();
      serverV2FeedPackage1.Summary = metadata.Summary;
      serverV2FeedPackage1.Tags = metadata.Tags != null ? string.Join(" ", (IEnumerable<string>) metadata.Tags) : (string) null;
      serverV2FeedPackage1.Title = metadata.Title;
      serverV2FeedPackage1.MinClientVersion = metadata.MinClientVersion;
      serverV2FeedPackage1.LastEdited = new DateTime?(additionalData.ModifiedDate);
      serverV2FeedPackage1.LicenseUrl = this.licenseUriCalculator.GetLicenseUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request1);
      serverV2FeedPackage1.Listed = metadata.Listed;
      serverV2FeedPackage1.IsLatestVersion = false;
      serverV2FeedPackage1.IsAbsoluteLatestVersion = false;
      return serverV2FeedPackage1;
    }
  }
}
