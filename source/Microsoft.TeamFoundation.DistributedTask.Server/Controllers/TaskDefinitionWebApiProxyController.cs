// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitionWebApiProxyController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "endpoint")]
  public class TaskDefinitionWebApiProxyController : DistributedTaskApiController
  {
    [HttpPost]
    public IList<string> QueryEndpoint(TaskDefinitionEndpoint endpoint)
    {
      ArgumentUtility.CheckForNull<TaskDefinitionEndpoint>(endpoint, nameof (endpoint));
      Guid result1;
      if (Guid.TryParse(endpoint.TaskId, out result1))
      {
        if (this.TfsRequestContext.IsFeatureEnabled("DistributedTask.VerifyPermissionsBeforeEndpointUse"))
        {
          if (!Guid.TryParse(endpoint.Scope, out Guid _))
            throw new EndpointNotFoundException(ServiceEndpointSdkResources.EndpointNotFound((object) endpoint.Scope));
          Guid result2;
          if (!Guid.TryParse(TaskDefinitionWebApiProxyController.ClearConnectionId(endpoint.ConnectionId), out result2))
            throw new EndpointNotFoundException(ServiceEndpointSdkResources.EndpointNotFound((object) endpoint.ConnectionId));
          bool flag1 = false;
          bool flag2 = new ServiceEndpointSecurity().HasPermission(this.TfsRequestContext, endpoint.Scope, result2.ToString("D"), 2, true);
          if (!flag2)
            flag1 = new ServiceEndpointSecurity().HasPermission(this.TfsRequestContext, endpoint.Scope, result2.ToString("D"), 1, true);
          if (!flag1 && !flag2)
            throw new EndpointNotFoundException(ServiceEndpointSdkResources.EndpointNotFound((object) result2.ToString("D")));
        }
        TaskDefinition taskDefinition = this.TfsRequestContext.GetService<IDistributedTaskService>().GetTaskDefinitions(this.TfsRequestContext, new Guid?(result1)).OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (td => td.Version)).FirstOrDefault<TaskDefinition>();
        if (taskDefinition != null)
        {
          HttpRequestProxy proxy = HttpRequestProxyFactory.GetProxy(this.TfsRequestContext.Elevate(), (IDictionary<string, string>) null, endpoint.Scope, endpoint.ConnectionId, endpoint.Url, (string) null, endpoint.Selector, (string) null, (JToken) null, endpoint.KeySelector);
          HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
          string endpointType = string.Empty;
          string endpointUrl = string.Empty;
          IList<string> stringList = (IList<string>) null;
          try
          {
            stringList = proxy.ExecuteRequest(taskDefinition.SourceDefinitions, (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) null, false, ref httpStatusCode, out string _, out endpointType, out endpointUrl, true).Result;
            CustomerIntelligenceHelper.PublishServiceEndpointProxyTelemetry(this.TfsRequestContext, httpStatusCode, "TaskDefinitionWebApiProxy", (string) null, (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null, endpoint, endpointType, (string) null, (string) null, (Exception) null);
          }
          catch (Exception ex)
          {
            CustomerIntelligenceHelper.PublishServiceEndpointProxyTelemetry(this.TfsRequestContext, httpStatusCode, "TaskDefinitionWebApiProxy", (string) null, (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null, endpoint, endpointType, (string) null, (string) null, ex);
          }
          return stringList;
        }
      }
      throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionIdNotFound((object) endpoint.TaskId));
    }

    private static string ClearConnectionId(string connectionId)
    {
      string str = "tfs:";
      int startIndex = connectionId.IndexOf(str);
      return startIndex >= 0 ? connectionId.Remove(startIndex, str.Length) : connectionId;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<EndpointNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskDefinitionNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointUntrustedHostException>(HttpStatusCode.Forbidden);
    }
  }
}
