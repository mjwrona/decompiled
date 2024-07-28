// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.ExchangeAppTokenValidator
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class ExchangeAppTokenValidator
  {
    private const string Area = "DelegatedAuthorizationService";
    private const string Layer = "PlatformDelegatedAuthorizationService";

    public static TokenError ValidateAppTokenClaims(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      ClaimsPrincipal appTokenClaims,
      out Guid appTokenClientId,
      out Guid appTokenAuthorizationId,
      out Guid appTokenIdentityId)
    {
      appTokenClientId = Guid.Empty;
      appTokenAuthorizationId = Guid.Empty;
      appTokenIdentityId = Guid.Empty;
      if (appToken == null)
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "AppToken is null.");
        return TokenError.InvalidAuthorizationGrant;
      }
      if (appTokenClaims == null)
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "AppTokenClaims is null.");
        return TokenError.InvalidAuthorizationGrant;
      }
      if (appTokenClaims.FindFirst(DelegatedAuthorizationTokenClaims.AuthorizationId) == null || string.IsNullOrWhiteSpace(appTokenClaims.FindFirst(DelegatedAuthorizationTokenClaims.AuthorizationId).Value))
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "AuthorizationId is missing from grant.");
        return TokenError.InvalidAuthorizationGrant;
      }
      if (!Guid.TryParse(appTokenClaims.FindFirst(DelegatedAuthorizationTokenClaims.AuthorizationId).Value, out appTokenAuthorizationId))
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "AuthorizationId Id is not in guid format.");
        return TokenError.InvalidAuthorizationGrant;
      }
      if (appTokenClaims.FindFirst("aud") == null || string.IsNullOrWhiteSpace(appTokenClaims.FindFirst("aud").Value))
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Client Id is missing from grant.");
        return TokenError.InvalidAuthorizationGrant;
      }
      if (!Guid.TryParse(appTokenClaims.FindFirst("aud").Value, out appTokenClientId))
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Grant client Id is not in guid format.");
        return TokenError.InvalidAuthorizationGrant;
      }
      if (appToken.ValidTo.ToUniversalTime() < DateTime.UtcNow)
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("App token is expired - {0}", (object) appToken.ValidTo));
        return TokenError.AuthorizationGrantExpired;
      }
      if (appTokenClaims.Identity?.Name == null)
      {
        requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "AppToken does not have assigned identity.");
        return TokenError.InvalidAuthorizationGrant;
      }
      string name = appTokenClaims.Identity.Name;
      if (Guid.TryParse(name, out appTokenIdentityId))
        return TokenError.None;
      requestContext.Trace(1049110, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Identity Id " + name + " is not in guid format.");
      return TokenError.InvalidAuthorizationGrant;
    }

    public static TokenError ValidateClientSecretClaims(
      IVssRequestContext requestContext,
      JsonWebToken clientSecret,
      ClaimsPrincipal clientSecretClaims,
      Guid appTokenClientId,
      out Registration registration)
    {
      registration = (Registration) null;
      if (clientSecret == null)
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "client secret is null.");
        return TokenError.InvalidClientSecret;
      }
      if (clientSecretClaims == null)
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "clientSecretClaims is null.");
        return TokenError.InvalidClientSecret;
      }
      if (clientSecretClaims.FindFirst(DelegatedAuthorizationTokenClaims.ClientId) == null || string.IsNullOrWhiteSpace(clientSecretClaims.FindFirst(DelegatedAuthorizationTokenClaims.ClientId).Value))
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Client Id is missing from client secret.");
        return TokenError.InvalidClientSecret;
      }
      Guid result1 = Guid.Empty;
      if (!Guid.TryParse(clientSecretClaims.FindFirst(DelegatedAuthorizationTokenClaims.ClientId).Value, out result1))
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Client secret clientId is not in guid format.");
        return TokenError.InvalidClientId;
      }
      if (clientSecretClaims.FindFirst(DelegatedAuthorizationTokenClaims.ClientSecretVersionId) == null || string.IsNullOrWhiteSpace(clientSecretClaims.FindFirst(DelegatedAuthorizationTokenClaims.ClientSecretVersionId).Value))
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Client secretVersionId is missing from client secret.");
        return TokenError.InvalidClientSecret;
      }
      Guid result2 = Guid.Empty;
      if (!Guid.TryParse(clientSecretClaims.FindFirst(DelegatedAuthorizationTokenClaims.ClientSecretVersionId).Value, out result2))
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Client secretVersionId is not in guid format.");
        return TokenError.InvalidClientSecret;
      }
      if (clientSecret.ValidTo.ToUniversalTime() < DateTime.UtcNow)
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Client Secret  is expired - {0}", (object) clientSecret.ValidTo));
        return TokenError.ClientSecretExpired;
      }
      if (appTokenClientId != result1)
      {
        requestContext.Trace(1049111, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Grant client Id - {0} and client secret client id {1} should match.", (object) appTokenClientId, (object) result1);
        return TokenError.InvalidAuthorizationGrant;
      }
      try
      {
        using (DelegatedAuthorizationComponent component = requestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.GetRegistration(Guid.Empty, result1);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1049111, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", ex);
      }
      if (registration == null)
      {
        requestContext.Trace(1049111, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("ClientId {0} does not exists.", (object) result1));
        return TokenError.InvalidClientId;
      }
      if (registration.ClientType != ClientType.FullTrust && registration.ClientType != ClientType.HighTrust)
      {
        requestContext.Trace(1049112, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("ClientId {0} is not full trust or high trust, passed client type - {1}", (object) result1, (object) registration.ClientType));
        return TokenError.InvalidClient;
      }
      if (registration.SecretVersionId != result2)
      {
        requestContext.Trace(1049113, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Client Id {0} given secret does not match {1}.", (object) result1, (object) result2));
        return TokenError.InvalidClient;
      }
      JsonWebToken jsonWebToken = (JsonWebToken) null;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        jsonWebToken = vssRequestContext.GetService<IDelegatedAuthorizationRegistrationService>().GetSecret(vssRequestContext, registration);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1049112, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", ex);
      }
      if (jsonWebToken == null)
      {
        requestContext.Trace(1049112, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Client {0} secret not found.", (object) result1));
        return TokenError.InvalidClientSecret;
      }
      if (string.Equals(jsonWebToken.EncodedToken, clientSecret.EncodedToken))
        return TokenError.None;
      requestContext.Trace(1049114, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Client {0} secret does not match with input clientSecret.", (object) result1));
      return TokenError.InvalidClientSecret;
    }

    public static TokenError ValidateAppTokenAuthorization(
      IVssRequestContext requestContext,
      Guid appTokenClientId,
      Guid appTokenAuthorizationId,
      Guid appTokenIdentityId,
      out Authorization authorization)
    {
      authorization = (Authorization) null;
      try
      {
        using (DelegatedAuthorizationComponent component = requestContext.CreateComponent<DelegatedAuthorizationComponent>())
          authorization = component.GetAuthorization(appTokenAuthorizationId);
      }
      catch
      {
      }
      if (authorization == null)
      {
        requestContext.Trace(1049116, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("AuthorizationId {0} does not exists.", (object) appTokenAuthorizationId));
        return TokenError.InvalidAuthorizationGrant;
      }
      if (authorization.ValidTo.ToUniversalTime() < (DateTimeOffset) DateTime.UtcNow)
      {
        requestContext.Trace(1049116, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Authorization Id is expired - {0}", (object) appTokenAuthorizationId));
        return TokenError.AuthorizationGrantExpired;
      }
      if (authorization.RegistrationId != appTokenClientId)
      {
        requestContext.Trace(1049117, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Grant registration {0} does not match with input authorization registration {1}", (object) appTokenClientId, (object) authorization.RegistrationId));
        return TokenError.InvalidAuthorizationGrant;
      }
      if (!(authorization.IdentityId != appTokenIdentityId))
        return TokenError.None;
      requestContext.Trace(1049118, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Grant identity {0} does not match with input authorization identityId {1}", (object) appTokenIdentityId, (object) authorization.IdentityId));
      return TokenError.InvalidAuthorizationGrant;
    }

    public static TokenError ValidateAppTokenIdentity(
      IVssRequestContext requestContext,
      Guid grantIdentityId,
      out Microsoft.VisualStudio.Services.Identity.Identity targetIdentity)
    {
      targetIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(vssRequestContext, (IList<Guid>) new List<Guid>()
      {
        grantIdentityId
      }, QueryMembership.None, (IEnumerable<string>) null);
      if (source.Count != 1)
      {
        requestContext.Trace(1049119, TraceLevel.Info, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Identity {0} does not exists in our system", (object) grantIdentityId));
        return TokenError.InvalidAuthorizationGrant;
      }
      targetIdentity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) targetIdentity))
      {
        requestContext.Trace(1049120, TraceLevel.Error, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", string.Format("Identity {0} is service identity.", (object) grantIdentityId));
        return TokenError.InvalidAuthorizationGrant;
      }
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (!service.IsMember(requestContext, readIdentity.Descriptor, targetIdentity.Descriptor))
        return TokenError.None;
      requestContext.Trace(1049121, TraceLevel.Warning, "DelegatedAuthorizationService", "PlatformDelegatedAuthorizationService", "Cannot issue a token for deployment administrator - {0}.", (object) targetIdentity.Id);
      return TokenError.InvalidUserId;
    }
  }
}
