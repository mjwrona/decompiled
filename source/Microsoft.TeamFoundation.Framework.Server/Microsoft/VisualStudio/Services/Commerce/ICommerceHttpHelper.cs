// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ICommerceHttpHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (CommerceHttpHelper))]
  public interface ICommerceHttpHelper : IVssFrameworkService
  {
    HttpClient GetHttpClientWithJwtTokenAuth(
      IVssRequestContext requestContext,
      JwtSecurityToken jwtToken,
      int requestTimeOut);

    HttpClient GetHttpClientWithJwtTokenAuth(
      IVssRequestContext requestContext,
      string serviceName,
      JwtSecurityToken jwtToken,
      int requestTimeOut);

    HttpClient GetHttpClientWithCertificate(
      IVssRequestContext requestContext,
      X509Certificate2 certificateThumbprint,
      int requestTimeOut);

    HttpResponseMessage GetHttpResponseMessage(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      Uri serviceUri,
      HttpMethod httpMethod,
      int slowRequestThresholdMilliseconds,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      Action<HttpResponseMessage> failureAction = null);

    HttpResponseMessage GetHttpResponseMessage(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      Uri serviceUri,
      HttpMethod httpMethod,
      object content,
      int slowRequestThresholdMilliseconds,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      Action<HttpResponseMessage> failureAction = null);
  }
}
