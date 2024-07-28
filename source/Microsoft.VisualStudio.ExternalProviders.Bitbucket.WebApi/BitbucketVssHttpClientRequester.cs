// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketVssHttpClientRequester
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  internal sealed class BitbucketVssHttpClientRequester : 
    VssHttpClientBase,
    IExternalProviderHttpRequester,
    IDisposable
  {
    public BitbucketVssHttpClientRequester(HttpMessageHandler httpMessageHandler)
      : base((Uri) null, httpMessageHandler, true)
    {
    }

    protected override bool ShouldThrowError(HttpResponseMessage response) => false;

    public bool SendRequest(
      HttpRequestMessage message,
      HttpCompletionOption option,
      out HttpResponseMessage response,
      out HttpStatusCode code,
      out string errorMessage)
    {
      response = this.SendAsync(message, option).ConfigureAwait(false).GetAwaiter().GetResult();
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
