// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.PublicDeploymentRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class PublicDeploymentRequestRestrictionsAttribute : IPublicRequestRestrictionsAttribute
  {
    public AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      return PublicDeploymentRequestRestrictionsAttribute.RouteSupportsDeploymentLevelPublicAccess(requestContext) ? new AllowPublicAccessResult(true, true, Guid.Empty) : new AllowPublicAccessResult(false, false, Guid.Empty);
    }

    public static bool RouteSupportsDeploymentLevelPublicAccess(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        Contribution routedContribution = requestContext.GetService<IContributionRoutingService>().GetRoutedContribution(requestContext);
        if (routedContribution != null)
          flag = routedContribution.GetProperty<bool>("deploymentLevelAnonymous");
      }
      return flag;
    }
  }
}
