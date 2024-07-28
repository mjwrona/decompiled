// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.NuGetServiceIndexGenerator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3
{
  public class NuGetServiceIndexGenerator
  {
    private const string PackagePublishV2Service = "PackagePublish/2.0.0";
    private const string LegacyGalleryV2Service = "LegacyGallery/2.0.0";
    private const string PackageBaseAddress = "PackageBaseAddress/3.0.0";
    private const string RegistrationsBaseUrl = "RegistrationsBaseUrl/3.0.0-beta";
    private const string RegistrationsBaseUrlSemVer2 = "RegistrationsBaseUrl/3.6.0";
    private const string RegistrationsBaseUrlSemVer2Versioned = "RegistrationsBaseUrl/Versioned";
    private const string SearchQueryService = "SearchQueryService/3.0.0-beta";
    private const string TypeVersion = "3.0.0";
    private readonly ILocationFacade locationService;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IProvenanceFacade provenanceService;
    private readonly IOrgLevelPackagingSetting<bool> matchUserSuppliedUriSetting;
    private readonly IOrgLevelPackagingSetting<bool> enableColossalPackagesSetting;
    private readonly IOrgLevelPackagingSetting<bool> enableCommitLogControllerSetting;

    public NuGetServiceIndexGenerator(
      ILocationFacade locationService,
      IExecutionEnvironment executionEnvironment,
      IProvenanceFacade provenanceService,
      IOrgLevelPackagingSetting<bool> matchUserSuppliedUriSetting,
      IOrgLevelPackagingSetting<bool> enableColossalPackagesSetting,
      IOrgLevelPackagingSetting<bool> enableCommitLogControllerSetting)
    {
      this.locationService = locationService;
      this.executionEnvironment = executionEnvironment;
      this.provenanceService = provenanceService;
      this.matchUserSuppliedUriSetting = matchUserSuppliedUriSetting;
      this.enableColossalPackagesSetting = enableColossalPackagesSetting;
      this.enableCommitLogControllerSetting = enableCommitLogControllerSetting;
    }

    public ServiceIndex GetFeedIndex(IFeedRequest feedRequest)
    {
      Uri hostBaseUri = this.locationService.GetHostBaseUri();
      List<ServiceEntry> serviceEntryList = new List<ServiceEntry>();
      PackagingUriNamePreference namePreference = this.matchUserSuppliedUriSetting.Get() ? PackagingUriNamePreference.PreferUserSuppliedNameOrId : PackagingUriNamePreference.PreferCanonicalId;
      string overrideFeedId = feedRequest.GetFeedNameOrIdForUri(namePreference);
      Guid id = feedRequest.Feed.Id;
      SessionId sessionId;
      int num = this.provenanceService.TryGetSessionId(out sessionId) ? 1 : 0;
      if (num != 0)
      {
        overrideFeedId = sessionId.Name;
        id = sessionId.Id;
      }
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.V2BaseResourceId, "PackagePublish/2.0.0", overrideFeedId: overrideFeedId));
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.V2BaseResourceId, "LegacyGallery/2.0.0"));
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.Registrations2BaseResourceId, "RegistrationsBaseUrl/3.0.0-beta"));
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.Registrations2BaseResourceIdSemVer, "RegistrationsBaseUrl/3.6.0", "This base URL includes SemVer 2.0.0 packages."));
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.Registrations2BaseResourceIdSemVer, "RegistrationsBaseUrl/Versioned", "This base URL includes SemVer 2.0.0 packages."));
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.Query2ResourceId, "SearchQueryService/3.0.0-beta"));
      serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.FlatContainer2GetFileResourceId, "PackageBaseAddress/3.0.0"));
      if (this.executionEnvironment.IsHosted())
        serviceEntryList.Add(new ServiceEntry(hostBaseUri.AbsoluteUri, "VssBaseUrl"));
      serviceEntryList.Add(new ServiceEntry(NuGetServiceIndexGenerator.GetUuidUrn(id), "VssFeedId")
      {
        Label = feedRequest.Feed.Name
      });
      if (num == 0)
      {
        serviceEntryList.Add(new ServiceEntry(NuGetServiceIndexGenerator.GetFeedViewPrivateUri(feedRequest.Feed), "VssQualifiedFeedViewId")
        {
          Label = feedRequest.Feed.FullyQualifiedName
        });
        if (feedRequest.Feed.ViewId.HasValue)
          serviceEntryList.Add(new ServiceEntry(NuGetServiceIndexGenerator.GetUuidUrn(feedRequest.Feed.ViewId.Value), "VssFeedViewId")
          {
            Label = feedRequest.Feed.ViewName
          });
      }
      if (feedRequest.Feed.Project != (ProjectReference) null)
        serviceEntryList.Add(new ServiceEntry(NuGetServiceIndexGenerator.GetUuidUrn(feedRequest.Feed.Project.Id), "AzureDevOpsProjectId")
        {
          Label = feedRequest.Feed.Project.Name
        });
      if (this.enableColossalPackagesSetting.Get())
        serviceEntryList.Add(new ServiceEntry(NuGetServiceIndexGenerator.GetUuidUrn(Guid.Empty), "VssStorageInfo"));
      if (this.enableCommitLogControllerSetting.Get())
        serviceEntryList.Add(this.CreateServiceEntryFromLocationService(feedRequest, namePreference, ResourceIds.CommitLogResourceId, "VssCommitLog"));
      return new ServiceIndex()
      {
        Version = "3.0.0",
        Resources = (IEnumerable<ServiceEntry>) serviceEntryList
      };
    }

    private static string GetFeedViewPrivateUri(FeedCore feed) => "com.visualstudio.feeds.feedview:" + feed.FullyQualifiedId;

    private static string GetUuidUrn(Guid uuid) => "urn:uuid:" + uuid.ToString("d");

    private ServiceEntry CreateServiceEntryFromLocationService(
      IFeedRequest feedRequest,
      PackagingUriNamePreference namePreference,
      Guid resourceId,
      string type,
      string comment = null,
      string overrideFeedId = null)
    {
      return new ServiceEntry(this.locationService.GetResourceUri("nuget", resourceId, feedRequest, namePreference, !string.IsNullOrWhiteSpace(overrideFeedId) ? (object) new
      {
        feed = overrideFeedId,
        feedId = overrideFeedId
      } : (object) null).EnsurePathEndsInSlash().AbsoluteUri, type)
      {
        Comment = comment
      };
    }
  }
}
