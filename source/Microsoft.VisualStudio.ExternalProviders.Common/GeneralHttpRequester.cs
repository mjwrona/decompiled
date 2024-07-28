// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.GeneralHttpRequester
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public class GeneralHttpRequester : VssHttpClientBase, IExternalProviderHttpRequester, IDisposable
  {
    public GeneralHttpRequester(HttpMessageHandler httpMessageHandler)
      : base((Uri) null, httpMessageHandler, true)
    {
    }

    public bool SendRequest(
      HttpRequestMessage message,
      HttpCompletionOption option,
      out HttpResponseMessage response,
      out HttpStatusCode code,
      out string errorMessage)
    {
      bool flag = false;
      try
      {
        Task<HttpResponseMessage> task = this.SendAsync(message, option);
        response = Microsoft.VisualStudio.Services.WebApi.TaskExtensions.SyncResult(task);
        code = response.StatusCode;
        errorMessage = (string) null;
        flag = true;
      }
      catch (VssServiceException ex)
      {
        response = (HttpResponseMessage) null;
        code = this.LastResponseContext.HttpStatusCode;
        errorMessage = ex.InnerException?.Message ?? ex.Message;
      }
      return flag;
    }

    public async Task<HttpRequestResult> SendRequestAsync(
      HttpRequestMessage message,
      HttpCompletionOption option)
    {
      GeneralHttpRequester generalHttpRequester = this;
      try
      {
        HttpResponseMessage httpResponseMessage = await generalHttpRequester.SendAsync(message, option);
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
          Response = (HttpResponseMessage) null,
          Code = __nonvirtual (generalHttpRequester.LastResponseContext).HttpStatusCode,
          ErrorMessage = ex.InnerException?.Message ?? ex.Message,
          Success = false
        };
      }
    }
  }
}
