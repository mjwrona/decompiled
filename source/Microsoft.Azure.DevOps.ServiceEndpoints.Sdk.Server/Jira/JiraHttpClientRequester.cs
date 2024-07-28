// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraHttpClientRequester
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  internal sealed class JiraHttpClientRequester : 
    HttpClient,
    IExternalProviderHttpRequester,
    IDisposable
  {
    public JiraHttpClientRequester(HttpMessageHandler httpMessageHandler, TimeSpan timeout)
      : base(httpMessageHandler)
    {
      this.Timeout = timeout;
    }

    public bool SendRequest(
      HttpRequestMessage message,
      HttpCompletionOption option,
      out HttpResponseMessage response,
      out HttpStatusCode code,
      out string errorMessage)
    {
      response = this.SendAsync(message, option).SyncResult();
      code = response.StatusCode;
      errorMessage = (string) null;
      return response.IsSuccessStatusCode;
    }

    public async Task<HttpRequestResult> SendRequestAsync(
      HttpRequestMessage message,
      HttpCompletionOption option)
    {
      HttpResponseMessage httpResponseMessage = await this.SendAsync(message, option);
      return new HttpRequestResult()
      {
        Response = httpResponseMessage,
        Code = httpResponseMessage.StatusCode,
        ErrorMessage = (string) null,
        Success = httpResponseMessage.IsSuccessStatusCode
      };
    }
  }
}
