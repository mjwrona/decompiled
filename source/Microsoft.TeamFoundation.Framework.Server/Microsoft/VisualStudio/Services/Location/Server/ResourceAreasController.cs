// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ResourceAreasController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Location", ResourceName = "ResourceAreas")]
  public class ResourceAreasController : TfsApiController
  {
    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public IEnumerable<ResourceAreaInfo> GetResourceAreas()
    {
      ILocationDataProvider locationData = this.TfsRequestContext.GetService<ILocationService>().GetLocationData(this.TfsRequestContext, ServiceInstanceTypes.SPS);
      Dictionary<Guid, string> urlCache = new Dictionary<Guid, string>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      return (IEnumerable<ResourceAreaInfo>) locationData.FindServiceDefinitions(tfsRequestContext, "LocationService2").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (sd => sd.IsResourceArea())).Select<ServiceDefinition, ResourceAreaInfo>((Func<ServiceDefinition, ResourceAreaInfo>) (sd => sd.ToResourceAreaInfo(this.TfsRequestContext, (IDictionary<Guid, string>) urlCache))).Where<ResourceAreaInfo>((Func<ResourceAreaInfo, bool>) (ra => ra.LocationUrl != null)).ToList<ResourceAreaInfo>();
    }

    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public ResourceAreaInfo GetResourceArea(Guid areaId)
    {
      ServiceDefinition serviceDefinition = this.TfsRequestContext.GetService<ILocationService>().GetLocationData(this.TfsRequestContext, ServiceInstanceTypes.SPS).FindServiceDefinition(this.TfsRequestContext, "LocationService2", areaId);
      if (serviceDefinition == null)
        return (ResourceAreaInfo) null;
      ResourceAreaInfo resourceAreaInfo = serviceDefinition.ToResourceAreaInfo(this.TfsRequestContext);
      return resourceAreaInfo == null || resourceAreaInfo.LocationUrl == null ? (ResourceAreaInfo) null : resourceAreaInfo;
    }
  }
}
