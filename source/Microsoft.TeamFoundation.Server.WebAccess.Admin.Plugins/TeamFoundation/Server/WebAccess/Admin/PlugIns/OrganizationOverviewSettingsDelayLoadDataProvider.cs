// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationOverviewSettingsDelayLoadDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class OrganizationOverviewSettingsDelayLoadDataProvider : IExtensionDataProvider
  {
    private static VssRefreshCache<TimeZones> s_allTimeZones = new VssRefreshCache<TimeZones>(TimeSpan.FromMinutes(30.0), (Func<IVssRequestContext, TimeZones>) (context =>
    {
      List<TimeZoneData> timeZoneDataList = new List<TimeZoneData>();
      timeZoneDataList.AddRange(TimeZoneInfo.GetSystemTimeZones().Select<TimeZoneInfo, TimeZoneData>((Func<TimeZoneInfo, TimeZoneData>) (x => new TimeZoneData()
      {
        DisplayName = x.DisplayName,
        Id = x.Id
      })));
      return new TimeZones()
      {
        AllTimeZones = timeZoneDataList
      };
    }));

    public string Name => "Admin.OrganizationAdminOverviewDelayLoad";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IClientLocationProviderService service1 = requestContext.GetService<IClientLocationProviderService>();
      service1.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      service1.AddLocation(requestContext, providerContext.SharedData, AexServiceConstants.ServiceInstanceTypeId, new TeamFoundationHostType?(TeamFoundationHostType.Deployment));
      service1.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.Application));
      requestContext.GetService<IClientFeatureProviderService>().AddFeatureFlagState(requestContext, providerContext.SharedData, "VisualStudio.Services.AdminEngagement.OrganizationOverview.EditableOrganizationAvatar");
      ICollectionService service2 = requestContext.GetService<ICollectionService>();
      Collection collection = service2.GetCollection(requestContext, (IEnumerable<string>) null);
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IVssIdentityRetrievalService>().ResolveEligibleActorByMasterId(requestContext, collection.Owner);
      if (identity?.Properties == null)
      {
        string str = identity == null ? "ownerIdentity" : "ownerIdentity.Properties";
        requestContext.Trace(10050102, TraceLevel.Error, "OrganizationAdminOverview", "DataProvider", string.Format("GetData: {0} value is null; collection.Owner is {1}", (object) str, (object) collection.Owner));
        throw new ValueCannotBeNullException(str);
      }
      bool flag1 = false;
      bool flag2 = false;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationOverview.EnableTakeOverUI"))
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        flag1 = service2.IsEligibleForTakeOver(requestContext);
        flag2 = AadServiceUtils.IsAzureDevOpsAdministrator(requestContext, userIdentity.GetAadObjectId(), collection.TenantId);
      }
      Guid userId = requestContext.GetUserId();
      List<TimeZoneData> allTimeZones = OrganizationOverviewSettingsDelayLoadDataProvider.s_allTimeZones.Get(requestContext).AllTimeZones;
      ILocationService service3 = requestContext.GetService<ILocationService>();
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = requestContext.UseDevOpsDomainUrls();
      string accessMappingMoniker = flag5 ? AccessMappingConstants.VstsAccessMapping : AccessMappingConstants.DevOpsAccessMapping;
      string locationServiceUrl = service3.GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, accessMappingMoniker);
      if (locationServiceUrl == null)
      {
        requestContext.Trace(1005006, TraceLevel.Error, "OrganizationAdminOverview", "Service", "Failed to resolve target account Url, devopsDomainUrls={0}", (object) flag5);
      }
      else
      {
        try
        {
          ServicingOrchestrationRequestStatus orchestrationRequestStatus = requestContext.GetClient<NewDomainUrlOrchestrationHttpClient>().GetStatusAsync().SyncResult<ServicingOrchestrationRequestStatus>();
          if (orchestrationRequestStatus.Status == ServicingOrchestrationStatus.Running || orchestrationRequestStatus.Status == ServicingOrchestrationStatus.Queued)
            flag4 = true;
          flag3 = true;
        }
        catch (ServicingOrchestrationEntryDoesNotExistException ex)
        {
          flag3 = true;
        }
        catch (Exception ex)
        {
          flag3 = false;
        }
      }
      Owner owner = new Owner()
      {
        Name = identity.DisplayName,
        Id = identity.Id,
        Email = identity.Properties.ContainsKey("mail") ? identity.Properties["mail"].ToString() : ""
      };
      string str1 = "";
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationOverview.EditableOrganizationAvatar"))
        str1 = service3.GetLocationData(requestContext, OrganizationResourceIds.ResourceAreaId).GetResourceUri(requestContext, "Organization", OrganizationResourceIds.CollectionAvatarResourceLocationId, (object) new
        {
          collectionId = collection.Id.ToString()
        }).AbsoluteUri;
      return (object) new OrganizationOverviewSettingsDelayLoadData()
      {
        CurrentOwner = owner,
        IsOwner = identity.Id.Equals(userId),
        CurrentUserId = userId,
        AllTimeZones = allTimeZones,
        ShowDomainMigration = flag3,
        DisableDomainMigration = flag4,
        DevOpsDomainUrls = flag5,
        TargetDomainUrl = locationServiceUrl,
        HasDeletePermissions = (OrganizationAdminDataProviderHelper.GetDeletePermissionBits(requestContext, collection.Id) == 32),
        HasModifyPermissions = (OrganizationAdminDataProviderHelper.GetModifyPermissionBits(requestContext, collection.Id) == 16),
        OrganizationTakeover = new OrganizationTakeover()
        {
          IsAzureDevOpsAdministrator = flag2,
          IsEligibleForTakeOver = flag1
        },
        AvatarUrl = str1
      };
    }
  }
}
