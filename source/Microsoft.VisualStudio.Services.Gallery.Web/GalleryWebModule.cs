// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryWebModule
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class GalleryWebModule : WebPlatformModule
  {
    public override PageContext CreatePageContext(RequestContext requestContext)
    {
      PageContext pageContext = base.CreatePageContext(requestContext);
      this.UpdateModuleLoaderConfiguration(pageContext.ModuleLoaderConfig, pageContext.WebContext);
      return pageContext;
    }

    private void UpdateModuleLoaderConfiguration(
      ModuleLoaderConfiguration moduleLoaderConfig,
      WebContext webContext)
    {
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts", "highcharts");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/highcharts-more", "highcharts-more");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/modules/accessibility", "highcharts-accessibility");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/modules/funnel", "highcharts-funnel");
      this.AddHighchartsFile(moduleLoaderConfig, webContext, "highcharts/modules/heatmap", "highcharts-heatmap");
      moduleLoaderConfig.Map["*"] = new Dictionary<string, string>()
      {
        {
          "office-ui-fabric-react/lib",
          "OfficeFabric"
        }
      };
    }

    private void AddHighchartsFile(
      ModuleLoaderConfiguration moduleLoaderConfig,
      WebContext webContext,
      string moduleName,
      string rootFilename)
    {
      string str1 = ".v9.0.1";
      string str2 = webContext.Diagnostics.DebugMode ? ".src" : "";
      string str3 = rootFilename + str1 + str2;
      moduleLoaderConfig.Paths[moduleName] = StaticResources.ThirdParty.Scripts.GetLocation(str3);
      moduleLoaderConfig.AddContributionPath(moduleName, ContributionPathType.ThirdParty, str3);
    }
  }
}
