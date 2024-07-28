// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationProjectScopeLocationServiceExtensions
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TeamFoundationProjectScopeLocationServiceExtensions
  {
    public static Uri GetResourceUri(
      this ILocationService locationService,
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      Guid projectId,
      object routeValues,
      Guid serviceOwner = default (Guid))
    {
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues);
      if (projectId != Guid.Empty)
        routeDictionary["project"] = (object) projectId;
      return locationService.GetLocationData(requestContext, serviceOwner).GetResourceUri(requestContext, serviceType, identifier, (object) routeDictionary);
    }
  }
}
