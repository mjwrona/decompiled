// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyServiceEndpointsControllerBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientInternalUseOnly(false)]
  public abstract class LegacyServiceEndpointsControllerBase : DistributedTaskProjectApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> SHttpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UriFormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DuplicateServiceConnectionException),
        HttpStatusCode.Conflict
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LegacyServiceEndpointsControllerBase.SHttpExceptions;

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpGet]
    public virtual async Task<IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>> GetServiceEndpoints(
      [ClientQueryParameter] string type = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string authSchemes = null,
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string endpointIds = null,
      [ClientQueryParameter] bool includeFailed = false)
    {
      LegacyServiceEndpointsControllerBase endpointsControllerBase = this;
      IServiceEndpointService2 service = endpointsControllerBase.TfsRequestContext.GetService<IServiceEndpointService2>();
      IList<string> stringList = ArtifactPropertyKinds.AsPropertyFilters(authSchemes);
      List<Guid> endpointIdsAsGuid = LegacyServiceEndpointsControllerBase.GetEndpointIdsAsGuid(endpointIds);
      IVssRequestContext tfsRequestContext = endpointsControllerBase.TfsRequestContext;
      Guid projectId = endpointsControllerBase.ProjectId;
      string type1 = type;
      IList<string> authSchemes1 = stringList;
      List<Guid> endpointIds1 = endpointIdsAsGuid;
      int num = includeFailed ? 1 : 0;
      return (await service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<Guid>) endpointIds1, (string) null, num != 0).ConfigureAwait(true)).Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint()));
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpGet]
    public virtual Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint GetServiceEndpointDetails(
      Guid endpointId)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = this.TfsRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpointId);
      return serviceEndpoint == null ? (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null : serviceEndpoint.ToLegacyServiceEndpoint();
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPost]
    public virtual Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint CreateServiceEndpoint(
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        throw new ArgumentNullException(nameof (endpoint));
      return this.TfsRequestContext.GetService<IServiceEndpointService2>().CreateServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpoint.ToServiceEndpoint()).ToLegacyServiceEndpoint();
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPut]
    public virtual Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint UpdateServiceEndpoint(
      Guid endpointId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      string operation = null)
    {
      if (endpoint == null)
        throw new ArgumentNullException(nameof (endpoint));
      IServiceEndpointService2 service = this.TfsRequestContext.GetService<IServiceEndpointService2>();
      endpoint.Id = endpointId;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = endpoint.ToServiceEndpoint();
      string operation1 = operation;
      return service.UpdateServiceEndpoint(tfsRequestContext, projectId, serviceEndpoint, operation1).ToLegacyServiceEndpoint();
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpDelete]
    public virtual void DeleteServiceEndpoint(Guid endpointId) => this.TfsRequestContext.GetService<IServiceEndpointService2>().DeleteServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpointId);

    private static List<Guid> GetEndpointIdsAsGuid(string endpointIds)
    {
      List<Guid> endpointIdsAsGuid = (List<Guid>) null;
      IList<string> stringList1 = ArtifactPropertyKinds.AsPropertyFilters(endpointIds);
      if (stringList1 != null)
      {
        endpointIdsAsGuid = new List<Guid>();
        List<string> stringList2 = new List<string>();
        foreach (string input in (IEnumerable<string>) stringList1)
        {
          Guid result;
          if (Guid.TryParse(input, out result))
            endpointIdsAsGuid.Add(result);
          else
            stringList2.Add(input);
        }
        if (stringList2.Any<string>())
          throw new ArgumentException(string.Format("{0}, endpointIds: {1}", (object) TFCommonResources.EntityModel_BadGuidFormat(), (object) string.Join(", ", (IEnumerable<string>) stringList2)));
      }
      return endpointIdsAsGuid;
    }

    public override string ActivityLogArea => "ServiceEndpoint";
  }
}
