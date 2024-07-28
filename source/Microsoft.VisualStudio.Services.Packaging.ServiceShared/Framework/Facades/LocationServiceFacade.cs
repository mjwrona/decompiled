// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.LocationServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades
{
  public class LocationServiceFacade : ILocationFacade
  {
    private readonly IVssRequestContext context;
    private readonly ILocationService locationService;

    public LocationServiceFacade(IVssRequestContext context)
    {
      this.context = context;
      this.locationService = context.GetService<ILocationService>();
    }

    public Uri GetResourceUri(string serviceType, Guid resourceId, object routeValues) => this.locationService.GetResourceUri(this.context, serviceType, resourceId, routeValues);

    public Uri GetResourceUri(
      string serviceType,
      Guid resourceId,
      Guid projectId,
      object routeValues)
    {
      return this.locationService.GetResourceUri(this.context, serviceType, resourceId, projectId, routeValues);
    }

    public IResourceUriBinder GetUnboundResourceUri(string serviceType, Guid identifier) => this.locationService.GetUnboundResourceUri(this.context, serviceType, identifier);

    public Guid InstanceType => this.locationService.InstanceType;

    public string GetLocationServiceUrl(Guid instanceType, string accessMappingMoniker) => this.locationService.GetLocationServiceUrl(this.context, instanceType, accessMappingMoniker);
  }
}
