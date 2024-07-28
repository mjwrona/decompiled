// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.GeographyLocalizationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.GeographyManagement;
using Microsoft.VisualStudio.Services.HostManagement;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class GeographyLocalizationService : IGeographyLocalizationService, IVssFrameworkService
  {
    private const string c_area = "GeographyManagement";
    private const string c_layer = "GeographyLocalizationService";
    private const string c_geographyResourceNamePrefix = "DCGeography";

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.ValidateRequestContext(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetLocalizedNameForRegionCode(
      IVssRequestContext deploymentContext,
      string regionCode)
    {
      this.ValidateRequestContext(deploymentContext);
      Region region = deploymentContext.GetService<IRegionManagementService>().GetRegion(deploymentContext, regionCode);
      if (region != null && !string.IsNullOrEmpty(region.GeographyCode))
        return this.GetLocalizedNameForGeographyCode(deploymentContext, region.GeographyCode);
      deploymentContext.TraceAlways(17984168, TraceLevel.Warning, "GeographyManagement", nameof (GeographyLocalizationService), "Failed to find geography for the region {0}.", (object) regionCode);
      return (string) null;
    }

    public string GetLocalizedNameForGeographyCode(
      IVssRequestContext deploymentContext,
      string geographyCode)
    {
      this.ValidateRequestContext(deploymentContext);
      if (string.IsNullOrWhiteSpace(geographyCode))
        return (string) null;
      string forGeographyCode = HostingResources.Manager.GetString("DCGeography" + geographyCode, CultureInfo.CurrentUICulture);
      if (forGeographyCode == null)
      {
        forGeographyCode = geographyCode;
        deploymentContext.TraceAlways(17984169, TraceLevel.Error, "GeographyManagement", nameof (GeographyLocalizationService), "Failed to find the localized friendly name for geography {0}.", (object) geographyCode);
      }
      return forGeographyCode;
    }

    private void ValidateRequestContext(IVssRequestContext deploymentContext)
    {
      deploymentContext.CheckHostedDeployment();
      deploymentContext.CheckDeploymentRequestContext();
    }
  }
}
