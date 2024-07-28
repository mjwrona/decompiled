// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ServiceDefinitionsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Location", ResourceName = "ServiceDefinitions")]
  public class ServiceDefinitionsController : TfsApiController
  {
    [HttpPatch]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateServiceDefinitions(
      VssJsonCollectionWrapper<IEnumerable<ServiceDefinition>> serviceDefinitions)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serviceDefinitions.Value, nameof (serviceDefinitions));
      SecuredLocationManager service = this.TfsRequestContext.GetService<SecuredLocationManager>();
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions.Value)
      {
        if (string.IsNullOrEmpty(serviceDefinition.ToolId))
          serviceDefinition.ToolId = "Framework";
      }
      service.SaveServiceDefinitions(this.TfsRequestContext, serviceDefinitions.Value);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteServiceDefinition(string serviceType, Guid identifier)
    {
      SecuredLocationManager service = this.TfsRequestContext.GetService<SecuredLocationManager>();
      ServiceDefinition serviceDefinition = new ServiceDefinition();
      serviceDefinition.ServiceType = serviceType;
      serviceDefinition.Identifier = identifier;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ServiceDefinition[] serviceDefinitionArray = new ServiceDefinition[1]
      {
        serviceDefinition
      };
      service.RemoveServiceDefinitions(tfsRequestContext, (IEnumerable<ServiceDefinition>) serviceDefinitionArray);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpGet]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public IQueryable<ServiceDefinition> GetServiceDefinitions(string serviceType = null) => this.TfsRequestContext.GetService<SecuredLocationManager>().QueryServices(this.TfsRequestContext, serviceType).AsQueryable<ServiceDefinition>();

    [HttpGet]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public ServiceDefinition GetServiceDefinition(
      string serviceType,
      Guid identifier,
      bool allowFaultIn = true,
      bool previewFaultIn = false)
    {
      return identifier == LocationServiceConstants.ApplicationIdentifier && LocationServiceHelper.ShouldRemoveApplicationDefinitionForDev12(this.TfsRequestContext) ? (ServiceDefinition) null : this.TfsRequestContext.GetService<SecuredLocationManager>().QueryService(this.TfsRequestContext, serviceType, identifier, allowFaultIn, previewFaultIn);
    }

    public override string TraceArea => "LocationService";

    public override string ActivityLogArea => "Framework";
  }
}
