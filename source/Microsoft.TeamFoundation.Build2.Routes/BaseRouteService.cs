// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.BaseRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  public abstract class BaseRouteService : PipelinesRouteService
  {
    protected string GetResourceUrl(
      IVssRequestContext requestContext,
      string serviceArea,
      Guid locationId,
      object routeValues,
      Func<Uri, string> continuation = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Uri resourceUri;
      try
      {
        resourceUri = service.GetResourceUri(requestContext, serviceArea, locationId, routeValues);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build2", "BuildRouteService", ex);
        return string.Empty;
      }
      return continuation == null ? resourceUri.AbsoluteUri : continuation(resourceUri);
    }

    protected override string ServiceType => "build";
  }
}
