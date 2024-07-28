// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SharePointConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;
using System.Security.Principal;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SharePointConstants
  {
    public const string WssTfsServiceClassName = "Microsoft.TeamFoundation.SharePoint.TeamFoundationIntegrationService";
    public const string WssTfsServiceAssemblyShortName = "Microsoft.TeamFoundation.SharePoint";
    public const string RegistryPathRoot = "/Configuration/SharePoint";
    public const string RegistryPathForCacheTimeout = "/Configuration/SharePoint/SiteCacheTimeoutMilliseconds";
    public const string SharePointWebAppDefaultRelativePath = "sites";
    public const int MaxSiteAddressLength = 1024;
    public const int MinimumSupportedExtensionMajorVersion = 2;
    public const string SiteAdminDirectory = "_layouts";
    public const string SiteAdminPagePath2 = "_layouts/webadmin.aspx";
    public const string SiteAdminPagePath3 = "_layouts/settings.aspx";
    public const string AdminWebServicePath = "_vti_adm/Admin.asmx";
    public const string ListsWebServicePath = "_vti_bin/Lists.asmx";
    public const string SiteDataWebServicePath = "_vti_bin/SiteData.asmx";
    public const string SitesWebServicePath = "_vti_bin/Sites.asmx";
    public const string WssTfsWebServicePath = "_vti_bin/TeamFoundationIntegrationService.asmx";
    public static readonly string[] KnownWebServicePaths = new string[5]
    {
      "_vti_adm/Admin.asmx",
      "_vti_bin/Lists.asmx",
      "_vti_bin/SiteData.asmx",
      "_vti_bin/Sites.asmx",
      "_vti_bin/TeamFoundationIntegrationService.asmx"
    };
    public static readonly string SharePointServiceAccountsGroupIdentifier = SidIdentityHelper.ConstructWellKnownSid(2U, 1U);
    public static readonly SecurityIdentifier SharePointServiceAccountsGroup = new SecurityIdentifier(SharePointConstants.SharePointServiceAccountsGroupIdentifier);
    public const string LocalGroupName_Normal = "WSS_WPG";
    public const string LocalGroupName_Admin = "WSS_ADMIN_WPG";
    public const string SolutionName_BaseIntegration = "Microsoft.TeamFoundation.SharePoint.wsp";
    public const string SolutionName_WebParts = "TswaWebPartCollection.wsp";
    public const string WebAppFeature_WebParts_Name = "Tswa";
    public static readonly Guid WebAppFeature_WebParts_Identifier = new Guid("60E22958-BA2A-47be-B995-820C756324B6");
    public const string SolutionName_Dashboards = "Microsoft.TeamFoundation.SharePoint.Dashboards.wsp";
    public const string SolutionName_Dashboards15 = "Microsoft.TeamFoundation.SharePoint.Dashboards15.wsp";
    public const string WebAppFeature_DashboardsUpdate_Name = "TfsDashboardUpdate";
    public static readonly Guid WebAppFeature_DashboardsUpdate_Identifier = new Guid("A875689A-3F2F-4a72-8CE9-60F67C18D96E");
    public const int SharePoint12MajorVersion = 12;
    public const int SharePoint14MajorVersion = 14;
    public const int SharePoint15MajorVersion = 15;
  }
}
