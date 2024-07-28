// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.SessionTokenGenerator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class SessionTokenGenerator
  {
    private static string s_layer = "PersonalAccessTokenGenerator";

    public static SessionToken GenerateSelfDescribingJwt(
      IVssRequestContext requestContext,
      TimeSpan tokenDuration,
      Microsoft.VisualStudio.Services.Identity.Identity toBeResolvedIdentity,
      string scope = null,
      IDictionary<string, string> customClaims = null)
    {
      using (new MethodScope(requestContext, SessionTokenGenerator.s_layer, nameof (GenerateSelfDescribingJwt)))
      {
        if (toBeResolvedIdentity == null)
        {
          requestContext.TraceError(10016006, "Orchestration", "Identity is passed in as null for service host {0}", (object) requestContext.ServiceHost.InstanceId);
          return (SessionToken) null;
        }
        IVssRequestContext context = requestContext.Elevate();
        Microsoft.VisualStudio.Services.Identity.Identity identity = SessionTokenGenerator.ResolveIdentity(requestContext, toBeResolvedIdentity);
        if (identity == null)
        {
          requestContext.TraceError(10016005, "Orchestration", "Unable to resolve identity {0} ({1}) using service host {2}", (object) toBeResolvedIdentity.DisplayName, (object) toBeResolvedIdentity.Id, (object) requestContext.ServiceHost.InstanceId);
          return (SessionToken) null;
        }
        SessionToken selfDescribingJwt = (SessionToken) null;
        DateTime dateTime = DateTime.UtcNow.Add(tokenDuration);
        IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
        try
        {
          IDelegatedAuthorizationService authorizationService = service;
          IVssRequestContext requestContext1 = context;
          Guid? nullable1 = new Guid?(identity.Id);
          string displayName = identity.DisplayName;
          DateTime? nullable2 = new DateTime?(dateTime);
          string str = scope;
          IDictionary<string, string> dictionary = customClaims;
          Guid? clientId = new Guid?();
          Guid? userId = nullable1;
          string name = displayName;
          DateTime? validTo = nullable2;
          string scope1 = str;
          Guid? authorizationId = new Guid?();
          Guid? accessId = new Guid?();
          IDictionary<string, string> customClaims1 = dictionary;
          SessionTokenResult sessionTokenResult = authorizationService.IssueSessionToken(requestContext1, clientId, userId, name, validTo, scope1, authorizationId: authorizationId, accessId: accessId, customClaims: customClaims1);
          if (sessionTokenResult != null)
          {
            if (sessionTokenResult.HasError)
            {
              requestContext.TraceError(10016000, "Orchestration", "Unable to generate a personal access token for service identity {0} ({1}). Error {2}", (object) identity.DisplayName, (object) identity.Id, (object) sessionTokenResult.SessionTokenError);
            }
            else
            {
              selfDescribingJwt = sessionTokenResult.SessionToken;
              requestContext.TraceVerbose("Orchestration", "Successfully generated a personal access token for service identity {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
            }
          }
          else
            requestContext.TraceError(10016001, "Orchestration", "Received a null result from the call to IDelegatedAuthorizationService.IssueSessionToken for service identity {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10016002, "Orchestration", ex);
        }
        return selfDescribingJwt;
      }
    }

    public static SessionToken GenerateOidcToken(
      IVssRequestContext requestContext,
      Guid scope,
      IOidcFederationClaims federationClaims,
      TimeSpan tokenLifespan)
    {
      using (new MethodScope(requestContext, SessionTokenGenerator.s_layer, nameof (GenerateOidcToken)))
      {
        using (PerformanceTimer.StartMeasure(requestContext, "SessionTokenGenerator.GenerateOidcToken"))
        {
          ArgumentUtility.CheckForNull<IOidcFederationClaims>(federationClaims, nameof (federationClaims));
          Microsoft.VisualStudio.Services.Identity.Identity identity = SessionTokenGenerator.ResolveIdentityForIdTokenIssuance(requestContext, scope);
          Guid id = identity.Id;
          string displayName = identity.DisplayName;
          IDelegatedAuthorizationService service = requestContext.GetService<IDelegatedAuthorizationService>();
          SessionTokenResult sessionTokenResult;
          try
          {
            IDelegatedAuthorizationService authorizationService = service;
            IVssRequestContext requestContext1 = requestContext;
            Guid? nullable1 = new Guid?(id);
            string str = displayName;
            DateTime? nullable2 = new DateTime?(DateTime.UtcNow.Add(tokenLifespan));
            IDictionary<string, string> jsonClaims = federationClaims.JsonClaims;
            Guid? clientId = new Guid?();
            Guid? userId = nullable1;
            string name = str;
            DateTime? validTo = nullable2;
            Guid? authorizationId = new Guid?();
            Guid? accessId = new Guid?();
            IDictionary<string, string> customClaims = jsonClaims;
            sessionTokenResult = authorizationService.IssueSessionToken(requestContext1, clientId, userId, name, validTo, tokenType: SessionTokenType.OpenIdConnect, authorizationId: authorizationId, accessId: accessId, customClaims: customClaims);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016200, nameof (SessionTokenGenerator), ex);
            throw new SessionTokenCreateException("Failed to generate a session token for service identity.");
          }
          if (sessionTokenResult == null)
          {
            requestContext.TraceError(10016201, nameof (SessionTokenGenerator), "Received a null result from the call to IDelegatedAuthorizationService.IssueSessionToken for service identity {0} ({1})", (object) displayName, (object) id);
            throw new SessionTokenCreateException("Failed to generate a session token for service identity.");
          }
          if (sessionTokenResult.HasError)
          {
            requestContext.TraceError(10016202, nameof (SessionTokenGenerator), "Unable to generate a session token for service identity {0} ({1}). Error {2}", (object) displayName, (object) id, (object) sessionTokenResult.SessionTokenError);
            throw new SessionTokenCreateException("Failed to generate a session token for service identity.");
          }
          SessionToken sessionToken = sessionTokenResult.SessionToken;
          requestContext.TraceVerbose(nameof (SessionTokenGenerator), "Successfully generated a session token for service identity {0} ({1})", (object) displayName, (object) id);
          return sessionToken != null ? sessionToken : throw new SessionTokenCreateException("Failed to generate a session token for service identity. Session token was null.");
        }
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ResolveIdentityForIdTokenIssuance(
      IVssRequestContext requestContext,
      Guid scope)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (requestContext.IsSystemContext)
      {
        if (scope != new Guid())
          identity = SessionTokenGenerator.GetBuildIdentity(requestContext, scope.ToString());
        if (identity == null)
          identity = SessionTokenGenerator.GetBuildIdentity(requestContext, requestContext.ServiceHost.InstanceId.ToString());
      }
      else
        identity = requestContext.GetUserIdentity();
      return identity;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetBuildIdentity(
      IVssRequestContext requestContext,
      string identifier)
    {
      requestContext.CheckSystemRequestContext();
      return IdentityHelper.GetFrameworkIdentity(requestContext, FrameworkIdentityType.ServiceIdentity, "Build", identifier);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ResolveIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      using (new MethodScope(requestContext, SessionTokenGenerator.s_layer, nameof (ResolveIdentity)))
      {
        identity = requestContext.GetService<IdentityService>().GetIdentity(requestContext, identity.Descriptor);
        if (identity != null && !IdentityHelper.IsServiceIdentityType(identity.Descriptor))
          requestContext.TraceError(10015529, "Orchestration", "Expected a service identity type but instead got identity {0}", (object) identity.Descriptor);
        return identity;
      }
    }
  }
}
