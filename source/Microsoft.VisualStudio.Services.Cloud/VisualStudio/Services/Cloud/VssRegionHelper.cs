// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VssRegionHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class VssRegionHelper
  {
    private const string s_area = "RegionManagement";
    private const string s_layer = "VssRegionHelper";
    private const string c_regionResourceNamePrefix = "DCRegion";

    public static string GetLocalizedNameForVssRegionCode(
      IVssRequestContext requestContext,
      string regionCode)
    {
      string forVssRegionCode = HostingResources.Manager.GetString("DCRegion" + regionCode, CultureInfo.CurrentUICulture);
      if (forVssRegionCode == null)
      {
        forVssRegionCode = regionCode;
        requestContext.Trace(17984169, TraceLevel.Error, "RegionManagement", nameof (VssRegionHelper), "Failed to find the localized friendly name for region {0}.", (object) regionCode);
      }
      return forVssRegionCode;
    }

    public static string GetAzureRegion(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Service/AzureRegion", string.Empty);
    }
  }
}
