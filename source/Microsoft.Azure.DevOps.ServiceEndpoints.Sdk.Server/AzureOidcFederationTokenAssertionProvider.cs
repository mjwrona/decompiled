// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureOidcFederationTokenAssertionProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class AzureOidcFederationTokenAssertionProvider : 
    IAzureOidcFederationTokenAssertionProvider
  {
    private readonly Guid _scopeIdentifier;
    private readonly IOidcFederationClaims _federationClaims;
    private readonly TimeSpan _tokenLifetime;

    public AzureOidcFederationTokenAssertionProvider(
      Guid scopeIdentifier,
      IOidcFederationClaims federationClaims,
      TimeSpan tokenLifetime)
    {
      ArgumentUtility.CheckForNull<IOidcFederationClaims>(federationClaims, nameof (federationClaims));
      ArgumentUtility.CheckForDefault<TimeSpan>(tokenLifetime, nameof (tokenLifetime));
      this._scopeIdentifier = scopeIdentifier;
      this._federationClaims = federationClaims;
      this._tokenLifetime = tokenLifetime;
    }

    public string IssueOidcToken(IVssRequestContext requestContext)
    {
      requestContext.TraceAlways(34000225, TraceLevel.Verbose, "ServiceEndpoints", "WebApiProxy", "Generating OpenIdConnect token for subject {0}", (object) this._federationClaims.Subject);
      using (new MethodScope(requestContext, "WebApiProxy", nameof (IssueOidcToken)))
      {
        Guid identityId;
        string identityName;
        this.SelectIdentity(requestContext, out identityId, out identityName);
        DateTime dateTime = DateTime.UtcNow.Add(this._tokenLifetime);
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IDelegatedAuthorizationService service = requestContext.GetService<IDelegatedAuthorizationService>();
        SessionToken sessionToken = (SessionToken) null;
        try
        {
          IDelegatedAuthorizationService authorizationService = service;
          IVssRequestContext requestContext1 = vssRequestContext;
          Guid? nullable1 = new Guid?(identityId);
          string str = identityName;
          DateTime? nullable2 = new DateTime?(dateTime);
          IDictionary<string, string> jsonClaims = this._federationClaims.JsonClaims;
          Guid? clientId = new Guid?();
          Guid? userId = nullable1;
          string name = str;
          DateTime? validTo = nullable2;
          Guid? authorizationId = new Guid?();
          Guid? accessId = new Guid?();
          IDictionary<string, string> customClaims = jsonClaims;
          SessionTokenResult sessionTokenResult = authorizationService.IssueSessionToken(requestContext1, clientId, userId, name, validTo, tokenType: SessionTokenType.OpenIdConnect, authorizationId: authorizationId, accessId: accessId, customClaims: customClaims);
          if (sessionTokenResult != null)
          {
            if (sessionTokenResult.HasError)
            {
              requestContext.TraceWarning("WebApiProxy", "Unable to generate a personal access token for service identity {0} ({1}). Error {2}", (object) identityName, (object) identityId, (object) sessionTokenResult.SessionTokenError);
            }
            else
            {
              sessionToken = sessionTokenResult.SessionToken;
              requestContext.TraceInfo("WebApiProxy", "Successfully generated a personal access token for service identity {0} ({1})", (object) identityName, (object) identityId);
            }
          }
          else
            requestContext.TraceWarning("WebApiProxy", "Received a null result from the call to IDelegatedAuthorizationService.IssueSessionToken for service identity {0} ({1})", (object) identityName, (object) identityId);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("WebApiProxy", ex);
        }
        return sessionToken?.Token;
      }
    }

    private void SelectIdentity(
      IVssRequestContext requestContext,
      out Guid identityId,
      out string identityName)
    {
      if (AzureOidcFederationTokenAssertionProvider.ShouldUseBuildIdentity(requestContext))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        Guid guid;
        if (this._scopeIdentifier != new Guid())
        {
          IVssRequestContext requestContext1 = requestContext;
          guid = this._scopeIdentifier;
          string identifier = guid.ToString();
          identity = this.GetBuildIdentity(requestContext1, identifier);
        }
        if (identity == null)
        {
          IVssRequestContext requestContext2 = requestContext;
          guid = requestContext.ServiceHost.InstanceId;
          string identifier = guid.ToString();
          identity = this.GetBuildIdentity(requestContext2, identifier);
        }
        identityId = identity.Id;
        identityName = identity.DisplayName;
      }
      else
      {
        identityId = requestContext.GetUserId();
        identityName = requestContext.AuthenticatedUserName;
      }
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetBuildIdentity(
      IVssRequestContext requestContext,
      string identifier)
    {
      if (!requestContext.IsFeatureEnabled("ServiceEndpoints.UseUserIdentityToCheckSystemContext"))
        requestContext.CheckSystemRequestContext();
      return IdentityHelper.GetFrameworkIdentity(requestContext, FrameworkIdentityType.ServiceIdentity, "Build", identifier);
    }

    private static bool ShouldUseBuildIdentity(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("ServiceEndpoints.UseUserIdentityToCheckSystemContext"))
        return requestContext.IsSystemContext;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return !IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity);
    }
  }
}
