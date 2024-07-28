// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SasTokenRequestController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "SasTokenRequest")]
  public class SasTokenRequestController : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      }
    };
    private static readonly TimeSpan s_maxExpiration = TimeSpan.FromDays(14.0);

    [HttpPost]
    [ClientResponseType(typeof (SasTokenInfo), null, null)]
    public SasTokenInfo CreateSasToken(SasTokenInfo request)
    {
      ArgumentUtility.CheckForNull<SasTokenInfo>(request, nameof (request), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<string>(request.ResourceUri, "ResourceUri", this.TfsRequestContext.ServiceName);
      Uri result;
      if (!Uri.TryCreate(request.ResourceUri, UriKind.Absolute, out result))
        throw new ArgumentException(HostingResources.InvalidSASTokenRequest_InvalidUriFormat()).Expected(this.TfsRequestContext.ServiceName);
      if (request.Expiration < TimeSpan.Zero)
        throw new ArgumentException(HostingResources.InvalidSASTokenRequest_InvalidExpiration()).Expected(this.TfsRequestContext.ServiceName);
      if (request.Expiration > SasTokenRequestController.s_maxExpiration)
        request.Expiration = SasTokenRequestController.s_maxExpiration;
      string sasToken = this.TfsRequestContext.GetService<ISasTokenRequestService>().GetSasToken(this.TfsRequestContext, result, request.Permissions, request.Expiration);
      request.SasToken = sasToken;
      return request;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) SasTokenRequestController.s_httpExceptions;

    public override string TraceArea => "SasTokenRequest";

    public override string ActivityLogArea => "HttpController";
  }
}
