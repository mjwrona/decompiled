// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy.HttpRequestProxyFactory
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy
{
  internal static class HttpRequestProxyFactory
  {
    public static HttpRequestProxy GetProxy(
      IVssRequestContext requestContext,
      IDictionary<string, string> requestParameters,
      string scope,
      string endpointId,
      string url,
      string resourceUrl,
      string selector,
      string resultTemplate,
      JToken replacementContext,
      string keySelector = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(scope, nameof (scope));
      ArgumentUtility.CheckForNull<string>(endpointId, nameof (endpointId));
      ArgumentUtility.CheckForNull<string>(url, nameof (url));
      ArgumentUtility.CheckForNull<string>(selector, nameof (selector));
      IEndpointAuthorizer endpointAuthorizer = EndpointAuthorizerFactory.GetEndpointAuthorizer(requestContext, scope, endpointId);
      ResponseSelector responseSelector = ResponseSelectorFactory.GetResponseSelector(selector, keySelector, resultTemplate, replacementContext, (string) null, (string) null);
      return new HttpRequestProxy(requestContext, scope, endpointId, url, requestParameters, endpointAuthorizer, resourceUrl, responseSelector);
    }
  }
}
