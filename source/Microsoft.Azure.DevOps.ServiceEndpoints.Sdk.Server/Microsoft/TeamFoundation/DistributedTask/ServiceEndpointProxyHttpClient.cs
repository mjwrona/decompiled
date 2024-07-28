// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.ServiceEndpointProxyHttpClient
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask
{
  public class ServiceEndpointProxyHttpClient : VssHttpClientBase
  {
    public ServiceEndpointProxyHttpClient(
      Uri baseUri,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUri, pipeline, disposeHandler)
    {
    }

    public bool SendRequest(
      HttpRequestMessage message,
      out HttpResponseMessage response,
      out HttpStatusCode code,
      out string errorMessage)
    {
      bool flag = false;
      try
      {
        Task<HttpResponseMessage> task = this.SendAsync(message, HttpCompletionOption.ResponseContentRead);
        response = Microsoft.VisualStudio.Services.WebApi.TaskExtensions.SyncResult(task);
        code = response.StatusCode;
        errorMessage = (string) null;
        flag = true;
      }
      catch (VssServiceException ex)
      {
        response = (HttpResponseMessage) null;
        code = this.LastResponseContext.HttpStatusCode;
        errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
      }
      return flag;
    }
  }
}
