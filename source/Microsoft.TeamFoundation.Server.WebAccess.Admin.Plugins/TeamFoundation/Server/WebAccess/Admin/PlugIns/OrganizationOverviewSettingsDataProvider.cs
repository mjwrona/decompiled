// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationOverviewSettingsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.GeographyManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class OrganizationOverviewSettingsDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.OrganizationAdminOverview";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new string[4]
      {
        "SystemProperty.TimeZone",
        "SystemProperty.OrganizationActivationTime",
        "SystemProperty.Description",
        "SystemProperty.PrivacyUrl"
      });
      ILocationService service = requestContext.GetService<ILocationService>();
      string locationServiceUrl = service.GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.PublicAccessMappingMoniker);
      string forVssRegionCode = VssRegionHelper.GetLocalizedNameForVssRegionCode(requestContext, collection.PreferredRegion);
      string geographyName = this.GetGeographyName(requestContext, collection);
      string json = collection.Properties.GetValue<string>("SystemProperty.TimeZone", TimeZoneInfo.Local.Id.Serialize<string>());
      string id = TimeZoneInfo.Local.Id;
      string str1 = id;
      try
      {
        id = JsonUtilities.Deserialize<string>(json);
        str1 = TimeZoneInfo.FindSystemTimeZoneById(id).DisplayName;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050001, "OrganizationAdminOverview", "Service", ex);
      }
      TimeZoneData timeZoneData = new TimeZoneData();
      timeZoneData.DisplayName = str1;
      timeZoneData.Id = id;
      string str2 = collection.Properties.GetValue<string>("SystemProperty.Description", string.Empty);
      string str3 = collection.Properties.GetValue<string>("SystemProperty.PrivacyUrl", string.Empty);
      string str4 = "";
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationOverview.EditableOrganizationAvatar"))
        str4 = service.GetLocationData(requestContext, OrganizationResourceIds.ResourceAreaId).GetResourceUri(requestContext, "Organization", OrganizationResourceIds.CollectionAvatarResourceLocationId, (object) new
        {
          collectionId = collection.Id.ToString()
        }).AbsoluteUri;
      return (object) new OrganizationOverviewSettingsData()
      {
        Id = collection.Id.ToString(),
        Name = collection.Name,
        TimeZone = timeZoneData,
        Description = str2,
        PrivacyUrl = str3,
        Readonly = requestContext.IsOrganizationInReadOnlyMode(),
        Region = forVssRegionCode,
        Geography = geographyName,
        Url = locationServiceUrl,
        HasModifyPermissions = (OrganizationAdminDataProviderHelper.GetModifyPermissionBits(requestContext, collection.Id) == 16),
        AvatarUrl = str4
      };
    }

    private string GetGeographyName(IVssRequestContext collectionContext, Collection collection)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment).Elevate();
      IGeographyLocalizationService service1 = vssRequestContext.GetService<IGeographyLocalizationService>();
      if (!collectionContext.IsFeatureEnabled("OrganizationOverview.ShowGeographies"))
        return string.Empty;
      if (!collectionContext.IsFeatureEnabled("OrganizationOverview.BackfillGeographies.AdHoc"))
        return service1.GetLocalizedNameForRegionCode(vssRequestContext, collection.PreferredRegion);
      if (collection.PreferredGeography.IsNullOrEmpty<char>())
      {
        IVssRequestContext context = collectionContext.Elevate();
        ICollectionService service2 = context.GetService<ICollectionService>();
        Region region = vssRequestContext.GetService<IRegionManagementService>().GetRegion(vssRequestContext, collection.PreferredRegion);
        IVssRequestContext collectionContext1 = context;
        string geographyCode = region.GeographyCode;
        collection = service2.BackfillPreferredGeography(collectionContext1, geographyCode);
      }
      return service1.GetLocalizedNameForGeographyCode(vssRequestContext, collection.PreferredGeography);
    }
  }
}
