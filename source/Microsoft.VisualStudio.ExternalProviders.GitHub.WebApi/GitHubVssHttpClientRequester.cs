// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubVssHttpClientRequester
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public class GitHubVssHttpClientRequester : 
    VssHttpClientBase,
    IExternalProviderHttpRequester,
    IDisposable
  {
    private readonly bool m_enableStrictPayloadValidation;

    public GitHubVssHttpClientRequester(
      HttpMessageHandler httpMessageHandler,
      bool enableStrictPayloadValidation)
      : base((Uri) null, httpMessageHandler, true)
    {
      this.m_enableStrictPayloadValidation = enableStrictPayloadValidation;
    }

    public virtual bool SendRequest(
      HttpRequestMessage message,
      HttpCompletionOption option,
      out HttpResponseMessage response,
      out HttpStatusCode code,
      out string errorMessage)
    {
      bool flag = false;
      try
      {
        if (this.m_enableStrictPayloadValidation)
          message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.speedy-preview+json"));
        Task<HttpResponseMessage> task = this.SendAsync(message, option);
        response = Microsoft.VisualStudio.Services.WebApi.TaskExtensions.SyncResult(task);
        code = response.StatusCode;
        errorMessage = (string) null;
        flag = true;
      }
      catch (VssServiceException ex)
      {
        response = this.CreateErrorResponse();
        code = this.LastResponseContext.HttpStatusCode;
        errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
      }
      return flag;
    }

    public virtual async Task<HttpRequestResult> SendRequestAsync(
      HttpRequestMessage message,
      HttpCompletionOption option)
    {
      GitHubVssHttpClientRequester httpClientRequester = this;
      try
      {
        if (httpClientRequester.m_enableStrictPayloadValidation)
          message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.speedy-preview+json"));
        HttpResponseMessage httpResponseMessage = await httpClientRequester.SendAsync(message, option);
        return new HttpRequestResult()
        {
          Response = httpResponseMessage,
          Code = httpResponseMessage.StatusCode,
          ErrorMessage = (string) null,
          Success = true
        };
      }
      catch (VssServiceException ex)
      {
        // ISSUE: explicit non-virtual call
        return new HttpRequestResult()
        {
          Response = httpClientRequester.CreateErrorResponse(),
          Code = __nonvirtual (httpClientRequester.LastResponseContext).HttpStatusCode,
          ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message,
          Success = false
        };
      }
    }

    private HttpResponseMessage CreateErrorResponse()
    {
      if (this.LastResponseContext.Headers == null)
        return (HttpResponseMessage) null;
      HttpResponseMessage errorResponse = new HttpResponseMessage(this.LastResponseContext.HttpStatusCode);
      this.LastResponseContext.Headers.ForEach<KeyValuePair<string, IEnumerable<string>>>((Action<KeyValuePair<string, IEnumerable<string>>>) (h => errorResponse.Headers.Add(h.Key, h.Value)));
      return errorResponse;
    }
  }
}
