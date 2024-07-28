// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientRouteProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientRouteProviderService : IClientRouteProviderService, IVssFrameworkService
  {
    private const string c_sharedDataKey = "_routes";
    private const string c_area = "ClientRouteProviderService";
    private const string c_layer = "IClientRouteProviderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddRoute(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string routeId)
    {
      Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, routeId);
      if (contribution != null && contribution.Type.Equals("ms.vss-web.route", StringComparison.OrdinalIgnoreCase))
      {
        if (contribution.Properties != null)
        {
          string[] strArray;
          if (contribution.Properties.TryGetValue<string[]>("routeTemplates", out strArray) && strArray != null)
          {
            object obj;
            WebSdkMetadataDictionary<string, string[]> metadataDictionary;
            if (sharedData.TryGetValue("_routes", out obj) && obj is WebSdkMetadataDictionary<string, string[]>)
            {
              metadataDictionary = obj as WebSdkMetadataDictionary<string, string[]>;
            }
            else
            {
              metadataDictionary = new WebSdkMetadataDictionary<string, string[]>();
              sharedData.Add("_routes", (object) metadataDictionary);
            }
            if (metadataDictionary.ContainsKey(routeId))
              return;
            metadataDictionary.Add(routeId, strArray);
          }
          else
            requestContext.Trace(100136310, TraceLevel.Error, nameof (ClientRouteProviderService), "IClientRouteProviderService", "Unable find contributed route templates: {0}", (object) routeId);
        }
        else
          requestContext.Trace(100136316, TraceLevel.Error, nameof (ClientRouteProviderService), "IClientRouteProviderService", "Unable find contributed route properties: {0}", (object) routeId);
      }
      else
        requestContext.Trace(100136305, TraceLevel.Error, nameof (ClientRouteProviderService), "IClientRouteProviderService", "Unable find contributed route: {0}", (object) routeId);
    }
  }
}
