// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.Client.MicrosoftGraphHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.MSGraph.Client
{
  [ClientCircuitBreakerSettings(1, 50)]
  [ClientCancellationTimeout(2)]
  public class MicrosoftGraphHttpClient : HttpClient
  {
    protected const string Area = "VisualStudio.Services.MSGraph";
    private const string Layer = "MicrosoftGraphHttpClient";

    public MicrosoftGraphHttpClient(string accessToken, string baseUri, string version)
    {
      this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
      this.BaseAddress = new Uri(baseUri + (!string.IsNullOrWhiteSpace(version) ? "/" + version : ""));
    }

    public InvitationResult CreateInvitation(Invitation invitation)
    {
      string requestUri = this.BaseAddress.ToString() + "/" + "invitations";
      StringContent content = new StringContent(JsonConvert.SerializeObject((object) invitation));
      content.Headers.Add("ContentType", "application/json");
      return MicrosoftGraphHttpClient.ParseResponse<InvitationResult>(this.PostAsync(requestUri, (HttpContent) content).Result);
    }

    protected static TResult ParseResponse<TResult>(HttpResponseMessage response)
    {
      string result = response.Content.ReadAsStringAsync().Result;
      switch (response.StatusCode)
      {
        case HttpStatusCode.OK:
        case HttpStatusCode.Created:
        case HttpStatusCode.Accepted:
          return !string.IsNullOrWhiteSpace(result) ? JsonConvert.DeserializeObject<TResult>(result) : default (TResult);
        case HttpStatusCode.NoContent:
          return default (TResult);
        default:
          ErrorResult errorResult;
          Exception parseException;
          throw MicrosoftGraphHttpClient.TryParseErrorResult(result, out errorResult, out parseException) ? new MicrosoftGraphClientException(response.StatusCode, errorResult, retryAfter: response.Headers.RetryAfter) : new MicrosoftGraphClientException(response.StatusCode, result, parseException, response.Headers.RetryAfter);
      }
    }

    private static bool TryParseErrorResult(
      string responseContent,
      out ErrorResult errorResult,
      out Exception parseException)
    {
      try
      {
        errorResult = JsonConvert.DeserializeObject<ErrorResult>(responseContent);
        parseException = (Exception) null;
        return true;
      }
      catch (Exception ex)
      {
        errorResult = (ErrorResult) null;
        parseException = ex;
        return false;
      }
    }
  }
}
