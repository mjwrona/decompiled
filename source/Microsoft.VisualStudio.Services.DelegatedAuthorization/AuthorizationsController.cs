// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationsController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "DelegatedAuth", ResourceName = "Authorizations")]
  public class AuthorizationsController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "AuthorizationsController";

    [HttpGet]
    [ClientResponseType(typeof (AuthorizationDescription), null, null)]
    public HttpResponseMessage InitiateAuthorization(
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      return this.InitiateAuthorization(Guid.Empty, responseType, clientId, redirectUri, scopes);
    }

    [HttpGet]
    [ClientResponseType(typeof (AuthorizationDescription), null, null)]
    public HttpResponseMessage InitiateAuthorization(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      try
      {
        return this.Request.CreateResponse<AuthorizationDescription>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().InitiateAuthorization(this.TfsRequestContext, userId, responseType, clientId, redirectUri, scopes));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (AuthorizationDecision), null, null)]
    public HttpResponseMessage Authorize(
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      return this.Authorize(Guid.Empty, responseType, clientId, redirectUri, scopes);
    }

    [HttpPost]
    [ClientResponseType(typeof (AuthorizationDecision), null, null)]
    public HttpResponseMessage Authorize(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      try
      {
        return this.Request.CreateResponse<AuthorizationDecision>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().Authorize(this.TfsRequestContext, userId, responseType, clientId, redirectUri, scopes));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RevokeAuthorization(Guid authorizationId, Guid? userId = null)
    {
      try
      {
        this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().Revoke(this.TfsRequestContext, userId ?? Guid.Empty, authorizationId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<AuthorizationDetails>), null, null)]
    public HttpResponseMessage GetAuthorizations(Guid? userId = null)
    {
      try
      {
        return this.Request.CreateResponse<IEnumerable<AuthorizationDetails>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IDelegatedAuthorizationService>().GetAuthorizations(this.TfsRequestContext, userId ?? Guid.Empty));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
