// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformDistributedTaskEnvironmentsRouteService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class PlatformDistributedTaskEnvironmentsRouteService : 
    IDistributedTaskEnvironmentsRouteService,
    IVssFrameworkService
  {
    public string GetEnvironmentWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      return this.ConstructWebLink(requestContext, projectId, "environments", environmentId);
    }

    protected string ConstructWebLink(
      IVssRequestContext requestContext,
      Guid projectId,
      string controllerName,
      int environmentId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      string str;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
        str = service.GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, accessMappingMoniker);
      }
      else
        str = service.LocationForAccessMapping(requestContext, "/", RelativeToSetting.Context, service.GetPublicAccessMapping(requestContext));
      if (!str.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        str += "/";
      return str + string.Join("/", (IEnumerable<string>) new List<string>()
      {
        projectId.ToString("D"),
        "_" + controllerName,
        string.Format("{0}", (object) environmentId)
      });
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
