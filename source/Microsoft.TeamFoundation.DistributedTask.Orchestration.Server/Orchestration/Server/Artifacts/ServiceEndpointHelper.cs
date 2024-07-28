// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ServiceEndpointHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class ServiceEndpointHelper
  {
    public static Func<IVssRequestContext, Guid, Guid, ServiceEndpoint> GetServiceEndpoint = (Func<IVssRequestContext, Guid, Guid, ServiceEndpoint>) ((requestContext, projectId, endpointId) =>
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (new Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.MethodScope(requestContext, nameof (GetServiceEndpoint), nameof (GetServiceEndpoint)))
      {
        if (!requestContext.GetClient<SecurityHttpClient>(ServiceInstanceTypes.TFS).HasPermissionAsync(ServiceEndpointSecurity.NamespaceId, ServiceEndpointSecurity.GetServiceEndPointToken(projectId.ToString("D"), endpointId.ToString("D")), 1, true).SyncResult<bool>())
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          throw new AccessCheckException(userIdentity.Descriptor, userIdentity.DisplayName, ServiceEndpointSecurity.GetServiceEndPointToken(projectId.ToString("D"), endpointId.ToString("D")), 1, ServiceEndpointSecurity.NamespaceId, TaskResources.EndpointAccessDeniedForUseOperation());
        }
        return requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext.Elevate(), projectId, endpointId);
      }
    });

    public static Guid GetEndpointId(string endpointIdValue)
    {
      Guid result;
      if (Guid.TryParse(endpointIdValue, out result))
        return result;
      throw new InvalidRequestException(TaskResources.InvalidServiceEndpointId((object) endpointIdValue));
    }

    public static IEnumerable<ServiceEndpoint> GetServiceEndpoints(
      IVssRequestContext requestContext,
      Guid projectId,
      string endpointType)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(endpointType))
        throw new ArgumentNullException(nameof (endpointType));
      string[] authSchemes = (string[]) null;
      string[] strArray = endpointType.Split(':');
      if (strArray.Length == 2)
      {
        endpointType = strArray[0];
        authSchemes = strArray[1].Split(',');
      }
      using (new Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.MethodScope(requestContext, "CustomArtifact", nameof (GetServiceEndpoints)))
        return (IEnumerable<ServiceEndpoint>) requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(requestContext, projectId, endpointType, (IEnumerable<string>) authSchemes, (IEnumerable<Guid>) null, (string) null, false);
    }

    public static ServiceEndpointType GetServiceEndpointType(
      IVssRequestContext requestContext,
      Guid projectId,
      string endpointType)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string[] strArray = !string.IsNullOrWhiteSpace(endpointType) ? endpointType.Split(':') : throw new ArgumentNullException(nameof (endpointType));
      if (strArray.Length == 2)
        endpointType = strArray[0];
      using (new Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.MethodScope(requestContext, "CustomArtifact", nameof (GetServiceEndpointType)))
        return requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, endpointType, (string) null).ToList<ServiceEndpointType>().FirstOrDefault<ServiceEndpointType>();
    }

    public static IList<ServiceEndpoint> GetServiceEndpointsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Guid> endpointIds,
      bool includeFailed)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (new Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.MethodScope(requestContext, nameof (GetServiceEndpointsByIds), nameof (GetServiceEndpointsByIds)))
        return (IList<ServiceEndpoint>) requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) endpointIds, (string) null, includeFailed);
    }

    public static IList<InputValue> ExecuteServiceEndpointRequest(
      IVssRequestContext context,
      Guid scopeIdentifier,
      DataSourceBinding dataSourceBinding,
      IDictionary<string, string> inputParameters,
      IDictionary<string, string> currentInputValues)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      ServiceEndpointRequest serviceEndpointRequest = ServiceEndpointHelper.PrepareServiceEndpointRequest(dataSourceBinding, inputParameters);
      ServiceEndpointHelper.RemoveReservedParameters(serviceEndpointRequest);
      ServiceEndpointRequestResult endpointRequestResult = context.GetService<IServiceEndpointProxyService2>().ExecuteServiceEndpointRequest(context, scopeIdentifier, dataSourceBinding.EndpointId, serviceEndpointRequest);
      JArray jarray = ServiceEndpointHelper.IsSuccessStatusCode(endpointRequestResult.StatusCode) ? (JArray) endpointRequestResult.Result : throw new ExecuteServiceEndpointRequestFailedException(endpointRequestResult.ErrorMessage);
      List<InputValue> source = new List<InputValue>();
      foreach (JToken jtoken in jarray)
      {
        switch (jtoken.Type)
        {
          case JTokenType.Object:
            source.Add(jtoken.ToObject<InputValue>());
            continue;
          case JTokenType.String:
            source.Add(JsonConvert.DeserializeObject<InputValue>(jtoken.ToString()));
            continue;
          default:
            continue;
        }
      }
      if (currentInputValues != null)
      {
        currentInputValues["callbackRequired"] = endpointRequestResult.CallbackRequired.ToString();
        if (endpointRequestResult.CallbackRequired)
        {
          if (endpointRequestResult.CallbackContextParameters == null)
            throw new InvalidRequestException("Context can not be null if the callback required is true.");
          foreach (KeyValuePair<string, string> contextParameter in (IEnumerable<KeyValuePair<string, string>>) endpointRequestResult.CallbackContextParameters)
            currentInputValues[contextParameter.Key] = contextParameter.Value;
        }
      }
      return (IList<InputValue>) source.ToList<InputValue>();
    }

    private static void RemoveReservedParameters(ServiceEndpointRequest serviceEndpointRequest)
    {
      if (!serviceEndpointRequest.DataSourceDetails.Parameters.ContainsKey("endpoint"))
        return;
      serviceEndpointRequest.DataSourceDetails.Parameters.Remove("endpoint");
    }

    private static ServiceEndpointRequest PrepareServiceEndpointRequest(
      DataSourceBinding dataSourceBinding,
      IDictionary<string, string> inputParameters)
    {
      if (dataSourceBinding == null)
        throw new ArgumentNullException(nameof (dataSourceBinding));
      if (inputParameters == null)
        throw new ArgumentNullException(nameof (inputParameters));
      DataSourceDetails dataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = dataSourceBinding.DataSourceName,
        DataSourceUrl = dataSourceBinding.EndpointUrl,
        RequestVerb = dataSourceBinding.RequestVerb,
        RequestContent = dataSourceBinding.RequestContent,
        ResultSelector = dataSourceBinding.ResultSelector,
        InitialContextTemplate = dataSourceBinding.InitialContextTemplate
      };
      inputParameters.Copy<string, string>((IDictionary<string, string>) dataSourceDetails.Parameters);
      ResultTransformationDetails transformationDetails = new ResultTransformationDetails()
      {
        ResultTemplate = dataSourceBinding.ResultTemplate,
        CallbackContextTemplate = dataSourceBinding.CallbackContextTemplate,
        CallbackRequiredTemplate = dataSourceBinding.CallbackRequiredTemplate
      };
      return new ServiceEndpointRequest()
      {
        DataSourceDetails = dataSourceDetails,
        ResultTransformationDetails = transformationDetails
      };
    }

    private static bool IsSuccessStatusCode(HttpStatusCode statusCode) => statusCode >= HttpStatusCode.OK && statusCode <= (HttpStatusCode) 299;
  }
}
