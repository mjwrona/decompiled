// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.InternalEndpointAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using System;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class InternalEndpointAuthorizer : IEndpointAuthorizer
  {
    private readonly IVssRequestContext _requestContext;
    private readonly Microsoft.VisualStudio.Services.Identity.Identity _identity;
    private readonly string _endpointUrl;
    private readonly string _endpoint;
    private readonly TimeSpan TokenDuration = TimeSpan.FromMinutes(10.0);

    public bool SupportsAbsoluteEndpoint => false;

    public InternalEndpointAuthorizer(
      IVssRequestContext requestContext,
      string endpoint,
      string endpointUrl)
    {
      this._requestContext = requestContext;
      this._endpointUrl = endpointUrl;
      this._endpoint = endpoint;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      this._identity = vssRequestContext.GetService<IdentityService>().GetIdentity(vssRequestContext, requestContext.UserContext);
    }

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      if (!string.IsNullOrEmpty(resourceUrl))
        throw new InvalidOperationException(ServiceEndpointSdkResources.ResourceUrlNotSupported((object) this._endpointUrl, (object) string.Empty));
      SessionToken personalAccessToken = this.GeneratePersonalAccessToken(this._requestContext, this.TokenDuration, this._identity);
      if (personalAccessToken == null)
        return;
      string str = "Bearer " + personalAccessToken.Token;
      request.Headers.Add(HttpRequestHeader.Authorization, str);
    }

    public string GetEndpointUrl() => this._endpointUrl;

    public string GetServiceEndpointType() => this._endpoint;

    private SessionToken GeneratePersonalAccessToken(
      IVssRequestContext requestContext,
      TimeSpan tokenDuration,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.TraceEnter("WebApiProxy", nameof (GeneratePersonalAccessToken));
      if (identity == null)
        return (SessionToken) null;
      DateTime dateTime = DateTime.UtcNow.Add(tokenDuration);
      IVssRequestContext context = requestContext.Elevate();
      IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
      SessionToken sessionToken;
      try
      {
        requestContext.TraceVerbose("WebApiProxy", "Generating session token for userId: {0}, name: {1}, validity : {2}", (object) identity.Id, (object) identity.DisplayName, (object) dateTime);
        IDelegatedAuthorizationService authorizationService = service;
        IVssRequestContext requestContext1 = context;
        Guid? nullable1 = new Guid?(identity.Id);
        string displayName = identity.DisplayName;
        DateTime? nullable2 = new DateTime?(dateTime);
        Guid? clientId = new Guid?();
        Guid? userId = nullable1;
        string name = displayName;
        DateTime? validTo = nullable2;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        SessionTokenResult sessionTokenResult = authorizationService.IssueSessionToken(requestContext1, clientId, userId, name, validTo, authorizationId: authorizationId, accessId: accessId);
        if (sessionTokenResult != null)
        {
          if (sessionTokenResult.HasError)
          {
            requestContext.TraceError(10015005, "WebApiProxy", "Unable to generate a personal access token for service identity {0} ({1}). Error {2}", (object) identity.DisplayName, (object) identity.Id, (object) sessionTokenResult.SessionTokenError);
            throw new TeamFoundationInvalidAuthenticationException(ServiceEndpointSdkResources.FailedToGenerateToken((object) identity.DisplayName, (object) identity.Id, (object) sessionTokenResult.SessionTokenError));
          }
          sessionToken = sessionTokenResult.SessionToken;
        }
        else
        {
          requestContext.TraceError(10015006, "WebApiProxy", "Received a null result from the call to IDelegatedAuthorizationService.IssueSessionToken for service identity {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
          throw new TeamFoundationServiceException(ServiceEndpointSdkResources.NullSessionToken((object) identity.DisplayName, (object) identity.Id));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException("WebApiProxy", ex);
        throw;
      }
      requestContext.TraceInfo("WebApiProxy", "Successfully fetched PAT token for service identity {0} ({1}).", (object) identity.DisplayName, (object) identity.Id);
      return sessionToken;
    }
  }
}
