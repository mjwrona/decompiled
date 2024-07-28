// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationsPageDetailsHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RegistrationsPageDetailsHandler : 
    TracingHandler<PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>, JObject>
  {
    private readonly ILocationFacade locationFacade;
    private readonly IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> contentRedirectorSourceUriHandler;
    private readonly IRegistrationsPackageFrameCreator pageFrameCreator;
    private readonly IResourceUriBinder packageVersionRegistrationUriBinder;
    private readonly IResourceUriBinder packageRegistrationUriBinder;
    private readonly IResourceUriBinder commitLogUriBinder;
    private readonly INuGetLicenseUriCalculator licenseUriCalculator;
    private readonly INuGetIconUriCalculator iconUriCalculator;

    public RegistrationsPageDetailsHandler(
      ILocationFacade locationFacade,
      IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> contentRedirectorSourceUriHandler,
      IRegistrationsPackageFrameCreator pageFrameCreator,
      ITracerService tracerService,
      INuGetLicenseUriCalculator licenseUriCalculator,
      INuGetIconUriCalculator iconUriCalculator)
      : base(tracerService)
    {
      this.locationFacade = locationFacade;
      this.contentRedirectorSourceUriHandler = contentRedirectorSourceUriHandler;
      this.pageFrameCreator = pageFrameCreator;
      this.licenseUriCalculator = licenseUriCalculator;
      this.iconUriCalculator = iconUriCalculator;
      this.packageVersionRegistrationUriBinder = locationFacade.GetUnboundResourceUri("nuget", ResourceIds.Registrations2PackageVersionResourceId);
      this.packageRegistrationUriBinder = locationFacade.GetUnboundResourceUri("nuget", ResourceIds.Registrations2PackageResourceId);
      this.commitLogUriBinder = locationFacade.GetUnboundResourceUri("nuget", ResourceIds.CommitLogResourceId);
    }

    public override Task<JObject> Handle(
      PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>> request,
      ITracerBlock tracerBlock)
    {
      IDictionary<IPackageIdentity, Uri> packageToUriMap = this.contentRedirectorSourceUriHandler.Handle(new PackageIdentityBatchRequest((IFeedRequest) request, (ICollection<IPackageIdentity>) request.AdditionalData.Select<INuGetMetadataEntry, IPackageIdentity>((Func<INuGetMetadataEntry, IPackageIdentity>) (m => (IPackageIdentity) m.PackageIdentity)).ToList<IPackageIdentity>()));
      List<INuGetMetadataEntry> additionalData = request.AdditionalData;
      Uri registrationUri = this.GetPackageNewRegistrationUri((IPackageNameRequest<VssNuGetPackageName>) request);
      JArray content = new JArray((object) additionalData.Select<INuGetMetadataEntry, JObject>((Func<INuGetMetadataEntry, JObject>) (x => this.GetVersionDetails((IFeedRequest) request, x, registrationUri, packageToUriMap))));
      JObject result = new JObject(new object[2]
      {
        (object) new JProperty("count", (object) content.Count),
        (object) new JProperty("items", (object) content)
      });
      if (content.Count > 0)
      {
        VssNuGetPackageVersion lowerVersion = additionalData.Min<INuGetMetadataEntry, VssNuGetPackageVersion>((Func<INuGetMetadataEntry, VssNuGetPackageVersion>) (x => x.Metadata.Identity.Version));
        VssNuGetPackageVersion upperVersion = additionalData.Max<INuGetMetadataEntry, VssNuGetPackageVersion>((Func<INuGetMetadataEntry, VssNuGetPackageVersion>) (x => x.Metadata.Identity.Version));
        JObject packagePageFrame = this.pageFrameCreator.GetPackagePageFrame((IPackageNameRequest<VssNuGetPackageName>) request, lowerVersion, upperVersion, registrationUri);
        result.Merge((object) packagePageFrame);
      }
      return Task.FromResult<JObject>(result);
    }

    private JObject GetVersionDetails(
      IFeedRequest feedRequest,
      INuGetMetadataEntry metadataEntry,
      Uri packageNewRegistrationUri,
      IDictionary<IPackageIdentity, Uri> packageToUriMap)
    {
      NuGetPackageMetadata metadata = metadataEntry.Metadata;
      PackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata> request = feedRequest.WithPackage<VssNuGetPackageIdentity>(metadata.Identity).WithData<VssNuGetPackageIdentity, NuGetPackageMetadata>(metadata);
      Uri catalogUri = this.GetCommitLogEntryUri(feedRequest, metadataEntry);
      List<object> list = metadata.DependencyGroups.Select<NuGetDependencyGroup, object>((Func<NuGetDependencyGroup, object>) (metadataGroup => this.GetDependencyGroup(feedRequest, metadataGroup, catalogUri))).ToList<object>();
      string content1 = metadata.Identity.Version.NuGetVersion.HasMetadata ? metadata.Identity.Version.NuGetVersion.ToFullString() : metadata.Identity.Version.NormalizedOriginalCaseVersion;
      JObject content2 = new JObject(new object[17]
      {
        (object) new JProperty("@id", (object) catalogUri.AbsoluteUri),
        (object) new JProperty("@type", (object) "PackageDetails"),
        (object) new JProperty("authors", (object) (metadata.Authors ?? string.Empty)),
        (object) new JProperty("description", (object) (metadata.Description ?? string.Empty)),
        (object) new JProperty("iconUrl", (object) this.iconUriCalculator.GetIconUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request)),
        (object) new JProperty("id", (object) metadata.Identity.Name.DisplayName),
        (object) new JProperty("language", (object) (metadata.Language ?? string.Empty)),
        (object) new JProperty("licenseUrl", (object) this.licenseUriCalculator.GetLicenseUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request)),
        (object) new JProperty("listed", (object) metadata.Listed),
        (object) new JProperty("minClientVersion", (object) (metadata.MinClientVersion ?? string.Empty)),
        (object) new JProperty("projectUrl", (object) (metadata.ProjectUrl ?? string.Empty)),
        (object) new JProperty("published", (object) metadataEntry.CreatedDate),
        (object) new JProperty("requireLicenseAcceptance", (object) metadata.RequireLicenseAcceptance.GetValueOrDefault()),
        (object) new JProperty("summary", (object) (metadata.Summary ?? string.Empty)),
        (object) new JProperty("tags", metadata.Tags.IsNullOrEmpty<string>() ? (object) ImmutableList.Create<string>(string.Empty) : (object) metadata.Tags),
        (object) new JProperty("title", (object) (metadata.Title ?? string.Empty)),
        (object) new JProperty("version", (object) content1)
      });
      if (list.Count > 0)
        content2.Add("dependencyGroups", (JToken) new JArray((object) list));
      if (!string.IsNullOrWhiteSpace(metadata.LicenseExpression))
        content2.Add("licenseExpression", (JToken) metadata.LicenseExpression);
      return new JObject(new object[5]
      {
        (object) new JProperty("@id", (object) this.GetPackageVersionNewRegistrationUri(feedRequest.WithPackage<VssNuGetPackageIdentity>(metadata.Identity)).AbsoluteUri),
        (object) new JProperty("@type", (object) "Package"),
        (object) new JProperty("catalogEntry", (object) content2),
        (object) new JProperty("packageContent", (object) packageToUriMap[(IPackageIdentity) metadata.Identity].AbsoluteUri),
        (object) new JProperty("registration", (object) packageNewRegistrationUri.AbsoluteUri)
      });
    }

    private object GetDependencyGroup(
      IFeedRequest feedRequest,
      NuGetDependencyGroup metadataGroup,
      Uri catalogUri)
    {
      string str = "#dependencyGroup";
      if (!string.IsNullOrWhiteSpace(metadataGroup.TargetFramework))
        str = str + "/" + metadataGroup.TargetFramework.ToLowerInvariant();
      string groupUriString = catalogUri.AbsoluteUri + str;
      List<object> list = metadataGroup.Dependencies.Select<NuGetDependency, object>((Func<NuGetDependency, object>) (arg => this.GetDependency(feedRequest, arg, groupUriString))).ToList<object>();
      JObject dependencyGroup = new JObject(new object[2]
      {
        (object) new JProperty("@id", (object) groupUriString),
        (object) new JProperty("@type", (object) "PackageDependencyGroup")
      });
      if (!string.IsNullOrWhiteSpace(metadataGroup.TargetFramework))
        dependencyGroup.Add("targetFramework", (JToken) metadataGroup.TargetFramework);
      if (list.Count > 0)
        dependencyGroup.Add("dependencies", (JToken) new JArray((object) list));
      return (object) dependencyGroup;
    }

    private object GetDependency(
      IFeedRequest feedRequest,
      NuGetDependency metadataDependency,
      string groupUriString)
    {
      return (object) new JObject(new object[5]
      {
        (object) new JProperty("@id", (object) (groupUriString + "/" + metadataDependency.Name.NormalizedName)),
        (object) new JProperty("@type", (object) "PackageDependency"),
        (object) new JProperty("id", (object) metadataDependency.Name.DisplayName),
        (object) new JProperty("range", (object) metadataDependency.Range.ToNormalizedString()),
        (object) new JProperty("registration", (object) this.GetPackageNewRegistrationUri(feedRequest.WithPackageName<VssNuGetPackageName>(metadataDependency.Name)))
      });
    }

    private Uri GetCommitLogEntryUri(IFeedRequest feedRequest, INuGetMetadataEntry commitLogEntry) => this.commitLogUriBinder.Bind(feedRequest, PackagingUriNamePreference.PreferCanonicalId, (object) new Dictionary<string, object>()
    {
      {
        "commitId",
        (object) commitLogEntry.CommitId
      }
    });

    private Uri GetPackageVersionNewRegistrationUri(IPackageRequest<VssNuGetPackageIdentity> request) => this.packageVersionRegistrationUriBinder.Bind((IFeedRequest) request, PackagingUriNamePreference.PreferCanonicalId, (object) new Dictionary<string, object>()
    {
      {
        "packageId",
        (object) request.PackageId.Name.NormalizedName
      },
      {
        "packageVersion",
        (object) request.PackageId.Version.NormalizedVersion
      }
    });

    private Uri GetPackageNewRegistrationUri(IPackageNameRequest<VssNuGetPackageName> request) => this.packageRegistrationUriBinder.Bind((IFeedRequest) request, PackagingUriNamePreference.PreferCanonicalId, (object) new Dictionary<string, object>()
    {
      {
        "packageId",
        (object) request.PackageName.NormalizedName
      }
    });
  }
}
