// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.HttpExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public static class HttpExtensions
  {
    private const string c_authorizationHeaderKey = "Authorization";

    public static string BuildHttpRequestStringRepresentation(
      this HttpRequestMessage request,
      string requestBody,
      string confidentialUrl = null)
    {
      HttpRequestStringRepresentationBuilder representationBuilder = new HttpRequestStringRepresentationBuilder(request, requestBody);
      if (!string.IsNullOrWhiteSpace(confidentialUrl))
        representationBuilder.ConfidentialRequestUri = confidentialUrl;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Headers)
      {
        if (header.Key == "Authorization")
        {
          AuthenticationHeaderValue authorization = request.Headers.Authorization;
          string headerValue = string.Format("{0} {1}", (object) authorization.Scheme, (object) SecurityHelper.GetMaskedValue(authorization.Parameter));
          representationBuilder.AddHeader("Authorization", headerValue, false);
        }
        else
          representationBuilder.AddHeader(header.Key, header.Value, false);
      }
      return representationBuilder.ToString();
    }

    public static HttpClient SetBasicAuthentication(
      this HttpClient httpClient,
      string username,
      string password)
    {
      if (httpClient == null)
        throw new ArgumentNullException(nameof (httpClient));
      httpClient.DefaultRequestHeaders.Authorization = ServiceHooksHttpRequestMessage.CreateBasicAuthenticationHeader(username, password);
      return httpClient;
    }
  }
}
