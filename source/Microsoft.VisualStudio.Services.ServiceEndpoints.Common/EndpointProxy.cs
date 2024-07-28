// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.EndpointProxy
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class EndpointProxy
  {
    private static readonly int TimeoutInSeconds = 100000;

    public IEnumerable<string> QueryEndpoint(
      ServiceEndpoint serviceEndpoint,
      string endpointUrl,
      string resourceUrl,
      string resultSelector,
      string resultTemplate,
      List<AuthorizationHeader> authorizationHeaders,
      Dictionary<string, string> parameters)
    {
      JToken replacementContext = EndpointMustacheHelper.CreateReplacementContext(serviceEndpoint, parameters);
      EndpointStringResolver endpointStringResolver = new EndpointStringResolver(replacementContext);
      if (string.IsNullOrEmpty(endpointUrl))
      {
        string template = endpointStringResolver.ResolveVariablesInDollarFormat(serviceEndpoint, resultTemplate, parameters);
        return (IEnumerable<string>) new List<string>()
        {
          endpointStringResolver.ResolveVariablesInMustacheFormat(template)
        };
      }
      if (!endpointUrl.StartsWith("{{endpoint.url}}") && !endpointUrl.StartsWith("{{{endpoint.url}}}"))
        throw new ServiceEndpointException(Resources.ShouldStartWithEndpointUrlError());
      ResponseSelector responseSelector = ResponseSelectorFactory.GetResponseSelector(resultSelector, (string) null, resultTemplate, replacementContext, (string) null, (string) null);
      IEndpointAuthorizer endpointAuthorizer = SchemeBasedAuthorizerFactory.GetEndpointAuthorizer(serviceEndpoint, authorizationHeaders);
      endpointUrl = endpointStringResolver.ResolveVariablesInDollarFormat(serviceEndpoint, endpointUrl, parameters);
      endpointUrl = endpointStringResolver.ResolveVariablesInMustacheFormat(endpointUrl);
      return EndpointProxy.ExecuteRequest(endpointUrl, resourceUrl, responseSelector, endpointAuthorizer);
    }

    private static IEnumerable<string> ExecuteRequest(
      string endpointUrl,
      string resourceUrl,
      ResponseSelector selector,
      IEndpointAuthorizer authorizer)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri(endpointUrl));
      httpWebRequest.Method = "GET";
      selector.AddHeaders(httpWebRequest);
      authorizer.AuthorizeRequest(httpWebRequest, resourceUrl);
      httpWebRequest.Timeout = EndpointProxy.TimeoutInSeconds;
      HttpResponseMessage httpResponseMessage = (HttpResponseMessage) null;
      HttpResponseMessage result;
      try
      {
        HttpRequestMessage messageFromWebRequest = EndpointProxy.GetRequestMessageFromWebRequest(httpWebRequest);
        WebRequestHandler handler = new WebRequestHandler();
        handler.ClientCertificates.AddRange(httpWebRequest.ClientCertificates);
        result = new HttpClient((HttpMessageHandler) handler).SendAsync(messageFromWebRequest).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        throw new ServiceEndpointException(Resources.ExecuteServiceEndpointFailed() + "\n" + httpResponseMessage?.Content?.ToString(), ex);
      }
      ResponseSelectorResult responseSelectorResult = selector.Select(result);
      result.Dispose();
      return (IEnumerable<string>) responseSelectorResult.Result;
    }

    private static HttpRequestMessage GetRequestMessageFromWebRequest(HttpWebRequest webRequest)
    {
      HttpRequestMessage messageFromWebRequest = new HttpRequestMessage()
      {
        Method = new HttpMethod(webRequest.Method),
        RequestUri = webRequest.RequestUri
      };
      messageFromWebRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(webRequest.Accept));
      foreach (string name in ((IEnumerable<string>) webRequest.Headers.AllKeys).ToList<string>().Where<string>((Func<string, bool>) (x => !string.Equals(x, "Content-Type", StringComparison.OrdinalIgnoreCase) && !string.Equals(x, "Accept", StringComparison.OrdinalIgnoreCase))).ToList<string>())
        messageFromWebRequest.Headers.Add(name, webRequest.Headers[name]);
      return messageFromWebRequest;
    }
  }
}
