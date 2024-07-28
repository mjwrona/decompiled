// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationService
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Exceptions;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Tokens;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class PlatformDelegatedAuthorizationService : 
    PlatformDelegatedAuthorizationServiceBase,
    IDelegatedAuthorizationService,
    IVssFrameworkService
  {
    private string issuer;
    private readonly Guid WellKnownRegistrationIDForAppToken = Guid.Empty;
    private readonly Guid TfsPatWebUiClientId = new Guid("99999999-9999-9999-9999-999999999999");
    private readonly IHostLevelConversionConfiguration hostLevelConversionConfiguration;
    private IOAuth2SettingsService settings;
    private const string DelegatedAuthorizationSecretsDrawerName = "DelegatedAuthorizationSecrets";
    private const string DelegatedAuthorizationFrameworkAccessTokenKeySecret = "FrameworkAccessTokenKeySecret";
    private const string JwtBearerTokenType = "jwt-bearer";
    private const string ChangePublisherMessageBufferSize = "/Service/DelegatedAuthorization/ChangePublisherMessageBufferSize";
    private static readonly Regex Base32NoPaddingFormatRegex = new Regex("^[A-Z2-7]{52}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private const string Area = "DelegatedAuthorizationService";
    private const string Layer = "PlatformDelegatedAuthorizationService";
    private const string prefix = "vso:";
    private const string seperator = "|";
    private const string colon = ":";
    private const int DefaultRSAKeySize = 2048;
    private const string OAuthWhitelistFeatureFlag = "VisualStudio.Services.OAuthWhitelist.AllowOAuthWhitelist";

    public PlatformDelegatedAuthorizationService()
      : this(HostLevelConversionConfiguration.Instance)
    {
    }

    public PlatformDelegatedAuthorizationService(
      IHostLevelConversionConfiguration hostLevelConversionConfiguration)
    {
      this.hostLevelConversionConfiguration = hostLevelConversionConfiguration;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>();
      this.settings = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IOAuth2SettingsService>();
      this.LoadExtensions(systemRequestContext);
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      ILocationService service = vssRequestContext.GetService<ILocationService>();
      Guid serviceAreaIdentifier = vssRequestContext.ExecutionEnvironment.IsHostedDeployment ? ServiceInstanceTypes.SPS : Guid.Empty;
      this.issuer = systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? vssRequestContext.ServiceHost.InstanceId.ToString() : new Uri(service.GetLocationServiceUrl(vssRequestContext, serviceAreaIdentifier, AccessMappingConstants.PublicAccessMappingMoniker)).Host;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.DisposeExtensions();

    public void HasImpersonatePermission(
      IVssRequestContext requestContext,
      int tracePoint,
      string message)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        requestContext.Trace(tracePoint, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), message);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission);
      }
    }

    public void TryAuthenticateTheCredentialsBeforeIssuingToken(IVssRequestContext requestContext) => this.TryAuthenticateTheCredentialsBeforeIssuingToken(requestContext, 1048002, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Alternate credentials cannot be used to request session token.");

    public Guid TryValidateIdentityBeforeIssuingToken(
      IVssRequestContext requestContext,
      Guid? userId = null)
    {
      return this.TryValidateIdentityBeforeIssuingToken(requestContext, userId, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService));
    }

    public void TryValidatingUserForInitiateAuthorization(
      IVssRequestContext requestContext,
      Guid userId)
    {
      this.TryValidatingUserForInitiateAuthorization(requestContext, userId, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService));
    }

    public void CheckIdentityIssueTokenIsImpersonating(
      IVssRequestContext requestContext,
      out Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      Guid? userId = null)
    {
      requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Entering CheckIdentityIssueTokenIsImpersonating");
      ref Guid? local1 = ref userId;
      IVssRequestContext requestContext1 = requestContext;
      Guid? nullable = userId;
      Guid? userId1 = new Guid?(nullable ?? Guid.Empty);
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      ref Microsoft.VisualStudio.Services.Identity.Identity local2 = ref identity;
      ref Microsoft.VisualStudio.Services.Identity.Identity local3 = ref targetIdentity;
      Guid userIdCheckForEmpty = this.GetUserIdCheckForEmpty(requestContext1, userId1, out local2, out local3, true);
      local1 = new Guid?(userIdCheckForEmpty);
      IVssRequestContext requestContext2 = requestContext;
      Guid guid1;
      string str1;
      if (!userId.HasValue)
      {
        str1 = "";
      }
      else
      {
        guid1 = userId.Value;
        str1 = guid1.ToString();
      }
      string str2 = targetIdentity == null ? " is null" : "is not null";
      requestContext2.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "After GetUserIdCheckForEmpty - identity {0}, target identity is {1}", (object) str1, (object) str2);
      if (!userId.HasValue || userId.Value == Guid.Empty)
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.UserIdRequired);
      if (targetIdentity == null)
      {
        if (userId.HasValue)
          requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Target identity {0} is not available in the system.", (object) userId.Value);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserId);
      }
      if (!IdentityHelper.IsUserIdentity(requestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) targetIdentity) && targetIdentity.Descriptor.IdentityType != "Microsoft.TeamFoundation.ServiceIdentity")
      {
        requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Cannot issue token for service identity - {0}.", (object) targetIdentity.Id);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserType);
      }
      this.CheckShardedIdentity(requestContext, targetIdentity, 2048002, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Cannot issue a token for deployment administrator - {0}.", (object) targetIdentity.Id));
      Guid guid2 = identity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
      guid1 = guid2;
      nullable = userId;
      if ((nullable.HasValue ? (guid1 != nullable.GetValueOrDefault() ? 1 : 0) : 1) == 0)
        return;
      requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity is not same as target identity for issue token.");
      this.HasImpersonatePermission(requestContext, 2048002, string.Format("Request identity {0} does not have framework impersonate permissions, target identity {1}.", (object) guid2, (object) userId));
      requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Impersonating identity {0}, impersonated identity - {1}.", (object) guid2, (object) userId);
    }

    public AuthorizationDescription InitiateAuthorization(
      IVssRequestContext requestContext,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      return this.InitiateAuthorization(requestContext, Guid.Empty, responseType, clientId, redirectUri, scopes);
    }

    public AuthorizationDescription InitiateAuthorization(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (InitiateAuthorization));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return this.ExecuteServiceMethods<AuthorizationDescription>(requestContext, (Func<IVssRequestContext, AuthorizationDescription>) (context => this.InitiateAuthorizationInternal(context, userId, responseType, clientId, redirectUri, scopes)), (Func<IVssRequestContext, AuthorizationDescription>) (context => this.InitiateAuthorizationRemote(context, userId, responseType, clientId, redirectUri, scopes)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (InitiateAuthorization));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (InitiateAuthorization));
      }
    }

    private AuthorizationDescription InitiateAuthorizationRemote(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      try
      {
        this.TryValidatingUserForInitiateAuthorization(requestContext, userId);
      }
      catch (PlatformDelegatedAuthorizationServiceException ex)
      {
        string message = ex.Error.ToString();
        ex.ToPlatformDelegatedAuthorizationException(message);
      }
      return requestContext.GetService<ITokenService>().InitiateAuthorization(requestContext, userId, responseType, clientId, redirectUri, scopes);
    }

    private AuthorizationDescription InitiateAuthorizationInternal(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      if (clientId == Guid.Empty)
        return new AuthorizationDescription()
        {
          InitiationError = InitiationError.ClientIdRequired
        };
      if (redirectUri == (Uri) null)
        return new AuthorizationDescription()
        {
          InitiationError = InitiationError.RedirectUriRequired
        };
      if (!RegistrationValidator.ValidateUriScheme(redirectUri, true))
        return new AuthorizationDescription()
        {
          InitiationError = InitiationError.InvalidRedirectUri
        };
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out requestIdentity, out targetIdentity);
      if (!this.IsUserAuthorized(requestContext, requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment), userId) || this.IsServicePrincipal(requestContext, targetIdentity))
      {
        requestContext.Trace(1048007, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity {0} is not authorized to InitiateAuthorization.", (object) userId);
        throw new PlatformDelegatedAuthorizationException("AccessDenied");
      }
      AuthorizationScopeConfiguration configuration = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(requestContext);
      if (configuration != null)
      {
        if (configuration.IsEnabled)
        {
          try
          {
            scopes = configuration.NormalizeScopes(scopes, out bool _, true);
          }
          catch
          {
            return new AuthorizationDescription()
            {
              InitiationError = string.IsNullOrWhiteSpace(scopes) ? InitiationError.ScopeRequired : InitiationError.InvalidScope
            };
          }
        }
      }
      Registration registration;
      try
      {
        using (DelegatedAuthorizationComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.InitiateAuthorization(userId, responseType, clientId, redirectUri, scopes);
      }
      catch (RegistrationNotFoundException ex)
      {
        return new AuthorizationDescription()
        {
          InitiationError = InitiationError.InvalidClientId
        };
      }
      catch (InvalidRegistrationException ex)
      {
        return new AuthorizationDescription()
        {
          InitiationError = InitiationError.InvalidClientId
        };
      }
      catch (InvalidRedirectUriException ex)
      {
        return new AuthorizationDescription()
        {
          InitiationError = InitiationError.InvalidRedirectUri
        };
      }
      catch (ResponseTypeNotSupportedException ex)
      {
        return new AuthorizationDescription()
        {
          InitiationError = responseType == ResponseType.None ? InitiationError.ResponseTypeRequired : InitiationError.ResponseTypeNotSupported
        };
      }
      catch (InvalidAuthorizationScopeException ex)
      {
        return new AuthorizationDescription()
        {
          InitiationError = string.IsNullOrWhiteSpace(scopes) ? InitiationError.ScopeRequired : InitiationError.InvalidScope
        };
      }
      string market = Thread.CurrentThread.CurrentCulture.Name;
      return new AuthorizationDescription()
      {
        ClientRegistration = registration,
        ScopeDescriptions = ((IEnumerable<AuthorizationScope>) configuration.GetScopes(scopes)).Select<AuthorizationScope, AuthorizationScopeDescription>((Func<AuthorizationScope, AuthorizationScopeDescription>) (scope => scope.GetDescription(market))).Where<AuthorizationScopeDescription>((Func<AuthorizationScopeDescription, bool>) (description => description != null)).ToArray<AuthorizationScopeDescription>()
      };
    }

    public AuthorizationDecision Authorize(
      IVssRequestContext requestContext,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null)
    {
      return this.Authorize(requestContext, Guid.Empty, responseType, clientId, redirectUri, scopes, authorizationId);
    }

    public AuthorizationDecision Authorize(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Authorize));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 2);
        if (!authorizationId.HasValue)
          authorizationId = new Guid?(Guid.NewGuid());
        return this.ExecuteServiceMethods<AuthorizationDecision>(requestContext, (Func<IVssRequestContext, AuthorizationDecision>) (context => this.AuthorizeInternal(context, userId, responseType, clientId, redirectUri, scopes, authorizationId)), (Func<IVssRequestContext, AuthorizationDecision>) (context => this.AuthorizeRemote(context, userId, responseType, clientId, redirectUri, scopes, authorizationId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Authorize));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Authorize));
      }
    }

    private AuthorizationDecision AuthorizeRemote(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null)
    {
      try
      {
        this.TryValidatingIfDelegateAuthIsAllowedForUser(requestContext, userId);
      }
      catch (PlatformDelegatedAuthorizationServiceException ex)
      {
        ex.ToAuthorizationDecision();
      }
      return requestContext.GetService<ITokenService>().Authorize(requestContext, userId, responseType, clientId, redirectUri, scopes, authorizationId);
    }

    private AuthorizationDecision AuthorizeInternal(
      IVssRequestContext requestContext,
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      Guid? authorizationId = null)
    {
      if (clientId == Guid.Empty)
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.ClientIdRequired
        };
      if (responseType == ResponseType.None)
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.ResponseTypeRequired
        };
      if (string.IsNullOrWhiteSpace(scopes))
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.ScopeRequired
        };
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out requestIdentity, out targetIdentity);
      if (userId == Guid.Empty || targetIdentity == null)
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidUserId
        };
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      Registration registration = (Registration) null;
      try
      {
        using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.GetRegistration(Guid.Empty, clientId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1048036, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
      }
      if (registration == null)
      {
        requestContext.Trace(1048035, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client registration not found, make sure the client id is correct.");
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidClientId
        };
      }
      IDelegatedAuthorizationConfigurationService service = requestContext.GetService<IDelegatedAuthorizationConfigurationService>();
      AuthorizationScopeConfiguration configuration = service.GetConfiguration(requestContext);
      if (configuration != null)
      {
        if (configuration.IsEnabled)
        {
          try
          {
            if (registration.ClientType == ClientType.FullTrust)
            {
              if (registration.Scopes.IndexOf("app_token", StringComparison.InvariantCultureIgnoreCase) != -1)
                goto label_23;
            }
            scopes = configuration.NormalizeScopes(scopes, out bool _);
          }
          catch
          {
            return new AuthorizationDecision()
            {
              AuthorizationError = AuthorizationError.InvalidScope
            };
          }
        }
      }
label_23:
      if (redirectUri == (Uri) null)
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.RedirectUriRequired
        };
      if (!RegistrationValidator.ValidateUriScheme(redirectUri, true))
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidRedirectUri
        };
      if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) targetIdentity))
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidUserType
        };
      if (ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()))
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidUserType
        };
      if (requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment) != userId)
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.AccessDenied
        };
      DelegatedAuthorizationSettings settings = service.GetSettings(requestContext);
      Authorization authorization = (Authorization) null;
      try
      {
        using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
          authorization = component.Authorize(userId, responseType, clientId, redirectUri, scopes, DateTime.UtcNow, DateTime.UtcNow.Add(settings.AuthorizationGrantLifetime), authorizationId: authorizationId);
      }
      catch (RegistrationNotFoundException ex)
      {
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidClientId
        };
      }
      catch (InvalidRegistrationException ex)
      {
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidClientId
        };
      }
      catch (ResponseTypeNotSupportedException ex)
      {
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.ResponseTypeNotSupported
        };
      }
      catch (InvalidRedirectUriException ex)
      {
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidRedirectUri
        };
      }
      catch (InvalidAuthorizationScopeException ex)
      {
        return new AuthorizationDecision()
        {
          AuthorizationError = AuthorizationError.InvalidScope
        };
      }
      return new AuthorizationDecision()
      {
        Authorization = authorization,
        AuthorizationGrant = (AuthorizationGrant) new JwtBearerAuthorizationGrant(this.GenerateAuthorizationGrant(requestContext, authorization, settings))
      };
    }

    public void Revoke(IVssRequestContext requestContext, Guid authorizationId) => this.Revoke(requestContext, Guid.Empty, authorizationId);

    public void Revoke(IVssRequestContext requestContext, Guid userId, Guid authorizationId)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Revoke));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        this.ExecuteServiceMethods<bool>(requestContext, (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeInternal(context, userId, authorizationId);
          return true;
        }), (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeRemote(context, userId, authorizationId);
          return true;
        }), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Revoke));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Revoke));
      }
    }

    private void RevokeRemote(IVssRequestContext requestContext, Guid userId, Guid authorizationId) => requestContext.GetService<ITokenService>().Revoke(requestContext, userId, authorizationId);

    private void RevokeInternal(
      IVssRequestContext requestContext,
      Guid userId,
      Guid authorizationId)
    {
      ArgumentUtility.CheckForEmptyGuid(authorizationId, nameof (authorizationId));
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out requestIdentity, out targetIdentity);
      if (!this.IsUserAuthorized(requestContext, requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment), userId) || this.IsServicePrincipal(requestContext, targetIdentity))
      {
        requestContext.Trace(1048007, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity {0} is not authorized to Revoke.", (object) userId);
        throw new PlatformDelegatedAuthorizationException("AccessDenied");
      }
      using (DelegatedAuthorizationComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<DelegatedAuthorizationComponent>())
        component.Revoke(userId, authorizationId);
    }

    public IEnumerable<AuthorizationDetails> GetAuthorizations(IVssRequestContext requestContext) => this.GetAuthorizations(requestContext, Guid.Empty);

    public IEnumerable<AuthorizationDetails> GetAuthorizations(
      IVssRequestContext requestContext,
      Guid userId)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetAuthorizations));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return (IEnumerable<AuthorizationDetails>) this.ExecuteListServiceMethods<AuthorizationDetails>(requestContext, (Func<IVssRequestContext, IEnumerable<AuthorizationDetails>>) (context => this.GetAuthorizationsInternal(context, userId)), (Func<IVssRequestContext, IEnumerable<AuthorizationDetails>>) (context => this.GetAuthorizationsRemote(context, userId)), "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetAuthorizations));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetAuthorizations));
      }
    }

    private string GetDictionaryKey(AuthorizationDetails authorization) => (authorization.Authorization == null ? string.Empty : authorization.Authorization.AuthorizationId.ToString()) + ":" + (authorization.ClientRegistration == null ? string.Empty : authorization.ClientRegistration.RegistrationId.ToString());

    private IDictionary<string, AuthorizationDetails> ToDictionary(
      IEnumerable<AuthorizationDetails> list)
    {
      return (IDictionary<string, AuthorizationDetails>) list.ToDictionary<AuthorizationDetails, string, AuthorizationDetails>((Func<AuthorizationDetails, string>) (a => this.GetDictionaryKey(a)), (Func<AuthorizationDetails, AuthorizationDetails>) (a => a));
    }

    private IEnumerable<AuthorizationDetails> GetAuthorizationsRemote(
      IVssRequestContext requestContext,
      Guid userId)
    {
      ITokenService service = requestContext.GetService<ITokenService>();
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out Microsoft.VisualStudio.Services.Identity.Identity _, out targetIdentity);
      userId = targetIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext1 = requestContext;
      Guid userId1 = userId;
      return service.GetAuthorizations(requestContext1, userId1);
    }

    private IEnumerable<AuthorizationDetails> GetAuthorizationsInternal(
      IVssRequestContext requestContext,
      Guid userId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out requestIdentity, out targetIdentity);
      if (!this.IsUserAuthorized(requestContext, requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment), userId) || this.IsServicePrincipal(requestContext, targetIdentity))
      {
        requestContext.Trace(1048007, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity {0} is not authorized to GetAuthorizations.", (object) userId);
        throw new PlatformDelegatedAuthorizationException("AccessDenied");
      }
      IEnumerable<AuthorizationDetails> authorizations;
      using (DelegatedAuthorizationComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<DelegatedAuthorizationComponent>())
        authorizations = component.GetAuthorizations(userId);
      if (authorizations != null)
      {
        string market = Thread.CurrentThread.CurrentCulture.Name;
        AuthorizationScopeConfiguration configuration = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetConfiguration(requestContext);
        foreach (AuthorizationDetails authorizationDetails in authorizations)
          authorizationDetails.ScopeDescriptions = ((IEnumerable<AuthorizationScope>) configuration.GetScopes(authorizationDetails.Authorization.Scopes)).Select<AuthorizationScope, AuthorizationScopeDescription>((Func<AuthorizationScope, AuthorizationScopeDescription>) (scope => scope.GetDescription(market))).ToArray<AuthorizationScopeDescription>();
      }
      return authorizations;
    }

    public virtual AccessToken GetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      AccessTokenResult accessTokenResult = this.Exchange(requestContext, accessTokenKey, isPublic);
      if (accessTokenResult.HasError)
      {
        switch (accessTokenResult.AccessTokenError)
        {
          case TokenError.InvalidAccessToken:
          case TokenError.InvalidAccessTokenKey:
            throw new InvalidPersonalAccessTokenException(accessTokenResult.AccessTokenError.ToString());
          case TokenError.AccessDenied:
            throw new AccessCheckException(accessTokenResult.AccessTokenError.ToString());
          case TokenError.FailedToGetAccessToken:
            throw new PlatformDelegatedAuthorizationException("Internal Server Error");
          case TokenError.InvalidPublicAccessTokenKey:
            throw new InvalidPublicKeyException(accessTokenResult.AccessTokenError.ToString());
          default:
            throw new ExchangeAccessTokenKeyException(accessTokenResult.AccessTokenError.ToString());
        }
      }
      else
      {
        AccessToken accessToken = new AccessToken()
        {
          AuthorizationId = accessTokenResult.AuthorizationId,
          Token = accessTokenResult.AccessToken,
          TokenType = accessTokenResult.TokenType
        };
        if (accessTokenResult.RefreshToken != null)
          accessToken.RefreshToken = accessTokenResult.RefreshToken.Jwt;
        return accessToken;
      }
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostid,
      string requestedScopes = null)
    {
      requestContext.TraceEnter(1048340, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      try
      {
        return this.ExecuteServiceMethods<AccessTokenResult>(requestContext, (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeInternal(context, registrationId, clientSecret, hostid, requestedScopes)), (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeRemote(context, registrationId, clientSecret, hostid, requestedScopes)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      }
      finally
      {
        requestContext.TraceLeave(1048319, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      }
    }

    private AccessTokenResult ExchangeRemote(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      string requestedScopes = null)
    {
      try
      {
        ITeamFoundationHostManagementService service1 = requestContext.GetService<ITeamFoundationHostManagementService>();
        Guid tenantId = Guid.Empty;
        ITokenService service2 = requestContext.GetService<ITokenService>();
        using (IVssRequestContext context = service1.BeginUserRequest(requestContext, hostId, requestContext.GetUserIdentity()))
          tenantId = context.GetOrganizationAadTenantId();
        AccessTokenResult accessTokenResult = service2.Exchange(requestContext, registrationId, clientSecret, hostId, tenantId, requestedScopes);
        if (accessTokenResult.IsFirstPartyClient && accessTokenResult.AccessTokenError == TokenError.HostAuthorizationNotFound)
        {
          requestContext.Trace(1048341, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Host authorization not found for first party client {0}, trying to authorize host {1}", (object) registrationId, (object) hostId));
          HostAuthorizationDecision hostAuthrorizationDecision = service2.AuthorizeHost(requestContext, registrationId, hostId);
          if (!hostAuthrorizationDecision.HasError)
          {
            bool flag = false;
            using (IVssRequestContext vssRequestContext = service1.BeginUserRequest(requestContext, hostId, requestContext.GetUserIdentity()))
              flag = this.CreateApplicationPrincipalIdentity(vssRequestContext.Elevate(), hostAuthrorizationDecision);
            if (flag)
            {
              requestContext.Trace(1048341, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Trying to acquire an access token again");
              accessTokenResult = service2.Exchange(requestContext, registrationId, clientSecret, hostId, tenantId, requestedScopes);
              if (accessTokenResult.IsFirstPartyClient && accessTokenResult.AccessTokenError == TokenError.HostAuthorizationNotFound)
                requestContext.Trace(1048341, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Host authorization not found for first party client {0}, trying to authorize host {1}", (object) registrationId, (object) hostId));
            }
          }
        }
        return accessTokenResult;
      }
      catch (Exception ex)
      {
        TokenError result;
        if (System.Enum.TryParse<TokenError>(ex.Message, out result))
          return new AccessTokenResult()
          {
            AccessTokenError = result
          };
        throw;
      }
    }

    private AccessTokenResult ExchangeInternal(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostid,
      string requestedScopes = null)
    {
      throw new NotSupportedException();
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Uri redirectUri = null,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1050810, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      Guid? userId = this.GetIdentityId(requestContext, grantType, grant, clientSecret);
      try
      {
        if (!accessId.HasValue)
          accessId = new Guid?(Guid.NewGuid());
        return this.ExecuteServiceMethods<AccessTokenResult>(requestContext, (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeInternal(context, grantType, grant, clientSecret, redirectUri, accessId)), (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeRemote(context, grantType, grant, clientSecret, redirectUri, accessId, userId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      }
      finally
      {
        requestContext.TraceLeave(1050819, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      }
    }

    private Guid? GetIdentityId(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret)
    {
      switch (grantType)
      {
        case GrantType.Implicit:
          IVssRequestContext context = requestContext.Elevate();
          IdentityService service = context.GetService<IdentityService>();
          Guid guid = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
          IVssRequestContext requestContext1 = context;
          return service.ReadIdentities(requestContext1, (IList<Guid>) new List<Guid>()
          {
            guid
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() == null ? new Guid?() : new Guid?(guid);
        case GrantType.ClientCredentials:
          Guid result1 = Guid.Empty;
          string subject = clientSecret.Subject;
          if (!string.IsNullOrEmpty(subject) && Guid.TryParse(subject, out result1))
          {
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
            Registration registration = vssRequestContext.GetService<IDelegatedAuthorizationRegistrationService>().Get(vssRequestContext, result1);
            if (registration != null && registration.IdentityId != Guid.Empty)
              return new Guid?(registration.IdentityId);
            break;
          }
          break;
        default:
          Guid result2;
          if (grant != null && Guid.TryParse(grant.NameIdentifier, out result2))
            return new Guid?(result2);
          break;
      }
      return new Guid?();
    }

    private AccessTokenResult ExchangeRemote(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Uri redirectUri = null,
      Guid? accessId = null,
      Guid? userId = null)
    {
      Guid hostId = requestContext.ServiceHost.InstanceId;
      Guid orgHostId = requestContext.ServiceHost.InstanceId;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        orgHostId = requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
      Uri tokenEndpointUrl = (Uri) null;
      if (grantType == GrantType.ClientCredentials)
      {
        IVssRequestContext rootContext = requestContext.RootContext;
        tokenEndpointUrl = rootContext.GetService<ILocationService>().GetResourceUri(rootContext, "oauth2", OAuth2ResourceIds.Token, (object) null);
      }
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AccessTokenResult>(requestContext, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), (Func<IVssRequestContext, bool, AccessTokenResult>) ((context, isImpersonating) =>
      {
        try
        {
          return context.GetService<ITokenService>().Exchange(context, grantType, grant, clientSecret, hostId, orgHostId, tokenEndpointUrl, redirectUri, accessId);
        }
        catch (Exception ex)
        {
          TokenError result;
          if (System.Enum.TryParse<TokenError>(ex.Message, out result))
            return new AccessTokenResult()
            {
              AccessTokenError = result
            };
          throw;
        }
      }), userId, impersonateUserAllowed: true);
    }

    private AccessTokenResult ExchangeInternal(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Uri redirectUri = null,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1050810, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeInternal));
      try
      {
        if (grantType == GrantType.None)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant is set to none.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.GrantTypeRequired
          };
        }
        if (grantType == GrantType.Implicit)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant is implicit type, entering in implicitAccessToken flow .");
          return this.GenerateImplicitAccessToken(requestContext, grant);
        }
        if (clientSecret == null)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "client secret is null.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.ClientSecretRequired
          };
        }
        if (grantType == GrantType.ClientCredentials)
          return this.GenerateClientCredentialsAccessToken(requestContext, clientSecret, accessId.HasValue ? accessId.Value : Guid.NewGuid());
        if (grant == null)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant is null.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AuthorizationGrantRequired
          };
        }
        DelegatedAuthorizationCertificateManager certificateManager = new DelegatedAuthorizationCertificateManager();
        IDelegatedAuthorizationConfigurationService service1 = requestContext.GetService<IDelegatedAuthorizationConfigurationService>();
        DelegatedAuthorizationSettings settings1 = service1.GetSettings(requestContext);
        requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Get certificate");
        IVssRequestContext requestContext1 = requestContext;
        DelegatedAuthorizationSettings settings2 = settings1;
        X509Certificate2 certificate = certificateManager.GetCertificate(requestContext1, settings2);
        requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Validate the grant.");
        ClaimsPrincipal claimsPrincipal1 = this.ValidateToken(grant, certificate);
        if (claimsPrincipal1 == null)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant principal is null.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAuthorizationGrant
          };
        }
        if (requestContext.IsTracing(1050817, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService)))
        {
          foreach (Claim claim in claimsPrincipal1.Claims)
            requestContext.Trace(1050817, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "grantPrincipal claim type: {0} | value: {1} ", (object) claim.Type, (object) claim.Value);
        }
        requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Validate the client secret.");
        ClaimsPrincipal claimsPrincipal2 = this.ValidateToken(clientSecret, certificate);
        if (claimsPrincipal2 == null)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client secret principal is null.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidClientSecret
          };
        }
        if (requestContext.IsTracing(1050817, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService)))
        {
          foreach (Claim claim in claimsPrincipal2.Claims)
            requestContext.Trace(1050817, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "clientSecretPrincipal claim type: {0} | value: {1} ", (object) claim.Type, (object) claim.Value);
        }
        if (claimsPrincipal2.FindFirst(DelegatedAuthorizationTokenClaims.ClientSecretVersionId) == null || string.IsNullOrWhiteSpace(claimsPrincipal2.FindFirst(DelegatedAuthorizationTokenClaims.ClientSecretVersionId).Value))
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "client secret principal does not have secret versionId.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidClientSecret
          };
        }
        if (redirectUri == (Uri) null)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "RedirectUri is null.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.RedirectUriRequired
          };
        }
        if (grantType != GrantType.RefreshToken && (claimsPrincipal1.FindFirst(DelegatedAuthorizationTokenClaims.AuthorizationId) == null || string.IsNullOrWhiteSpace(claimsPrincipal1.FindFirst(DelegatedAuthorizationTokenClaims.AuthorizationId).Value)))
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant principal does not have AuthorizationId.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAuthorizationGrant
          };
        }
        if (grantType == GrantType.RefreshToken && (claimsPrincipal1.FindFirst(DelegatedAuthorizationTokenClaims.AccessTokenId) == null || string.IsNullOrWhiteSpace(claimsPrincipal1.FindFirst(DelegatedAuthorizationTokenClaims.AccessTokenId).Value)))
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant principal does not have AccessTokenId.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidRefreshToken
          };
        }
        if (grant.ValidTo.ToUniversalTime() < DateTime.UtcNow)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant is expired - grant date {0} and current date {1}", (object) grant.ValidTo, (object) DateTime.UtcNow);
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AuthorizationGrantExpired
          };
        }
        if (clientSecret.ValidTo.ToUniversalTime() < DateTime.UtcNow)
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client secret is expired - client secret date {0}, current date {1}", (object) clientSecret.ValidTo, (object) DateTime.UtcNow);
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.ClientSecretExpired
          };
        }
        string scopes1 = grant.Scopes;
        if (grantType != GrantType.RefreshToken)
        {
          if (!grant.Scopes.Contains("vso.authorization_grant"))
          {
            requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant does not contain vso.authorization_grant scope which is required for token exchange");
            return new AccessTokenResult()
            {
              AccessTokenError = TokenError.AuthorizationGrantScopeMissing
            };
          }
          scopes1 = grant.Scopes.Replace("vso.authorization_grant", "");
        }
        AuthorizationScopeConfiguration configuration = service1.GetConfiguration(requestContext);
        string scopes2;
        try
        {
          scopes2 = configuration.NormalizeScopes(scopes1, out bool _);
        }
        catch
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "scopes validation fail, passed scope - {0}", (object) scopes1);
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAuthorizationScopes
          };
        }
        Guid clientSecretVersionId = Guid.Parse(claimsPrincipal2.FindFirst(DelegatedAuthorizationTokenClaims.ClientSecretVersionId).Value);
        IdentityService service2 = requestContext.GetService<IdentityService>();
        string name = claimsPrincipal1.Identity.Name;
        Guid result = Guid.Empty;
        if (!Guid.TryParse(name, out result))
        {
          requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Name id is not in correct format - {0}", (object) name);
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAuthorizationGrant
          };
        }
        AccessTokenResult accessTokenResult = new AccessTokenResult();
        try
        {
          IVssRequestContext requestContext2 = requestContext.Elevate();
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service2.ReadIdentities(requestContext2, (IList<Guid>) new List<Guid>()
          {
            result
          }, QueryMembership.None, (IEnumerable<string>) null);
          if (identityList.Count == 0)
          {
            requestContext.Trace(1050811, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Identity does not exists in system - {0}", (object) result);
            return new AccessTokenResult()
            {
              AccessTokenError = TokenError.InvalidAuthorizationGrant
            };
          }
          AccessTokenData accessTokenRawData = (AccessTokenData) null;
          using (DelegatedAuthorizationComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<DelegatedAuthorizationComponent>())
          {
            if (grantType == GrantType.RefreshToken)
            {
              Guid accessId1 = Guid.Parse(claimsPrincipal1.FindFirst(DelegatedAuthorizationTokenClaims.AccessTokenId).Value);
              requestContext.Trace(1050812, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant type is refreshToken, call RefreshAccessToken with accessId {0}", (object) accessId);
              accessTokenRawData = component.RefreshAccessToken(accessId1, clientSecretVersionId, redirectUri, DateTime.UtcNow, DateTime.UtcNow.Add(settings1.AccessTokenLifetime), accessId);
            }
            else
            {
              Guid authorizationId = Guid.Parse(claimsPrincipal1.FindFirst(DelegatedAuthorizationTokenClaims.AuthorizationId).Value);
              requestContext.Trace(1050812, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Grant type is not refreshToken, call CreateAccessToken with authorizationId {0}", (object) authorizationId);
              accessTokenRawData = component.CreateAccessToken(authorizationId, clientSecretVersionId, redirectUri, DateTime.UtcNow, DateTime.UtcNow.Add(settings1.AccessTokenLifetime), accessId);
            }
          }
          requestContext.Trace(1050813, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Generate tokens for access.");
          this.GenerateTokens(requestContext, accessTokenResult, accessTokenRawData, certificate, identityList[0].StorageKey(requestContext, TeamFoundationHostType.Deployment), scopes2);
        }
        catch (AuthorizationNotFoundException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "AuthorizationNotFoundException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AuthorizationNotFound
          };
        }
        catch (InvalidAuthorizationException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "InvalidAuthorizationException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAuthorizationGrant
          };
        }
        catch (AccessAlreadyIssuedException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "AccessAlreadyIssuedException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AccessAlreadyIssued
          };
        }
        catch (InvalidRedirectUriException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "InvalidRedirectUriException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidRedirectUri
          };
        }
        catch (AccessTokenNotFoundException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "AccessTokenNotFoundException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AccessTokenNotFound
          };
        }
        catch (InvalidAccessTokenException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "InvalidAccessTokenException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAccessToken
          };
        }
        catch (AccessTokenAlreadyRefreshedException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "AccessTokenAlreadyRefreshedException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AccessTokenAlreadyRefreshed
          };
        }
        catch (InvalidClientSecretException ex)
        {
          requestContext.Trace(1050814, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "InvalidClientSecretException");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidClientSecret
          };
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1050815, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
          throw;
        }
        return accessTokenResult;
      }
      finally
      {
        requestContext.TraceLeave(1050819, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeInternal));
      }
    }

    public AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048010, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      try
      {
        return this.ExecuteServiceMethods<AccessTokenResult>(requestContext, (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeInternal(context, accessTokenKey, isPublic)), (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeRemote(context, accessTokenKey, isPublic)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      }
      finally
      {
        requestContext.TraceLeave(1048019, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (Exchange));
      }
    }

    private AccessTokenResult ExchangeRemote(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      try
      {
        return this.ExchangeAccessToken(requestContext, accessTokenKey, isPublic);
      }
      catch (Exception ex)
      {
        TokenError result;
        bool flag = System.Enum.TryParse<TokenError>(ex.Message, out result);
        if (this.hostLevelConversionConfiguration.IsEnabledForExchangeTokenFlow(requestContext) && !isPublic && result != TokenError.InvalidAccessToken)
          return this.ExchangeGlobalOrMultiOrgPat(requestContext, accessTokenKey);
        if (flag)
          return new AccessTokenResult()
          {
            AccessTokenError = result
          };
        throw;
      }
    }

    private AccessTokenResult ExchangeGlobalOrMultiOrgPat(
      IVssRequestContext requestContext,
      string accessTokenKey)
    {
      try
      {
        return this.ExchangeAccessToken(requestContext.ToDeploymentHostContext(), accessTokenKey, false);
      }
      catch (Exception ex)
      {
        TokenError result;
        if (System.Enum.TryParse<TokenError>(ex.Message, out result))
          return new AccessTokenResult()
          {
            AccessTokenError = result
          };
        throw;
      }
    }

    private AccessTokenResult ExchangeAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic)
    {
      AccessToken accessToken = requestContext.GetService<ITokenService>().GetAccessToken(requestContext, accessTokenKey, isPublic);
      return new AccessTokenResult()
      {
        AccessToken = accessToken.Token,
        AuthorizationId = accessToken.AuthorizationId,
        TokenType = accessToken.TokenType,
        ValidTo = accessToken.ValidTo.DateTime,
        RefreshToken = accessToken.RefreshToken != null ? new RefreshTokenGrant(accessToken.RefreshToken) : (RefreshTokenGrant) null
      };
    }

    private AccessTokenResult ExchangeInternal(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048010, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeInternal));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (string.IsNullOrWhiteSpace(accessTokenKey))
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AccessTokenKeyRequired
          };
        if (!PlatformDelegatedAuthorizationService.Base32NoPaddingFormatRegex.IsMatch(accessTokenKey.Trim('=')))
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAccessTokenKey
          };
        if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        {
          requestContext.Trace(1048012, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "User does not have permission to exchange access token key.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.AccessDenied
          };
        }
        AccessTokenData accessTokenData = (AccessTokenData) null;
        try
        {
          using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
            accessTokenData = component.GetAccessTokenByKey(accessTokenKey, isPublic);
        }
        catch (InvalidAccessTokenKeyException ex)
        {
          requestContext.Trace(1048014, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Access token matching the given access token key is not found.");
          if (isPublic)
            return new AccessTokenResult()
            {
              AccessTokenError = TokenError.InvalidPublicAccessTokenKey
            };
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAccessTokenKey
          };
        }
        catch (InvalidAccessTokenException ex)
        {
          requestContext.Trace(1048014, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Access token key mapped to an invalid access token.");
          if (isPublic)
            return new AccessTokenResult()
            {
              AccessTokenError = TokenError.InvalidPublicAccessToken
            };
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.InvalidAccessToken
          };
        }
        if (accessTokenData == null)
        {
          requestContext.Trace(1048014, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Invalid access token key.");
          return new AccessTokenResult()
          {
            AccessTokenError = TokenError.FailedToGetAccessToken
          };
        }
        VssSigningCredentials signingCredentials1 = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSigningCredentials(vssRequestContext, true);
        AccessTokenResult accessTokenResult = new AccessTokenResult();
        IVssRequestContext requestContext1 = vssRequestContext;
        Guid identityId = accessTokenData.Authorization.IdentityId;
        Guid authorizationId = accessTokenData.AccessToken.AuthorizationId;
        string scopes = accessTokenData.Authorization.Scopes;
        DateTime utcDateTime1 = accessTokenData.AccessToken.ValidFrom.UtcDateTime;
        DateTime utcDateTime2 = accessTokenData.AccessToken.ValidTo.UtcDateTime;
        VssSigningCredentials signingCredentials2 = signingCredentials1;
        string audience = accessTokenData.Authorization.Audience;
        string source1 = accessTokenData.Authorization.Source;
        Guid? accessId = new Guid?();
        string source2 = source1;
        Guid? hostAuthorizationId = new Guid?();
        Guid? appId = new Guid?();
        accessTokenResult.AccessToken = this.GenerateAccessToken(requestContext1, identityId, authorizationId, scopes, utcDateTime1, utcDateTime2, signingCredentials2, audience, accessId: accessId, source: source2, hostAuthorizationId: hostAuthorizationId, appId: appId);
        accessTokenResult.TokenType = "jwt-bearer";
        accessTokenResult.ValidTo = accessTokenData.AccessToken.ValidTo.UtcDateTime;
        accessTokenResult.AuthorizationId = accessTokenData.Authorization.AuthorizationId;
        requestContext.Trace(1048015, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Generated access token for given access token key.");
        return accessTokenResult;
      }
      finally
      {
        requestContext.TraceLeave(1048019, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeInternal));
      }
    }

    public SessionToken GetSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048031, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetSessionToken));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        return this.ExecuteServiceMethods<SessionToken>(requestContext, (Func<IVssRequestContext, SessionToken>) (context => this.GetSessionTokenInternal(context, authorizationId, isPublic)), (Func<IVssRequestContext, SessionToken>) (context => this.GetSessionTokenRemote(context, authorizationId, isPublic)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetSessionToken));
      }
      finally
      {
        requestContext.TraceLeave(1048027, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetSessionToken));
      }
    }

    private SessionToken GetSessionTokenRemote(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      return requestContext.GetService<ITokenService>().GetSessionToken(requestContext, authorizationId, isPublic);
    }

    private SessionToken GetSessionTokenInternal(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      ArgumentUtility.CheckForEmptyGuid(authorizationId, nameof (authorizationId));
      requestContext.TraceEnter(1048050, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetSessionTokenInternal));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        AccessTokenKey accessTokenKey = new AccessTokenKey();
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          accessTokenKey = component.GetAccessTokenKey(authorizationId, isPublic);
        if (accessTokenKey == null)
        {
          string message = DelegatedAuthorizationResources.NoValidSessionTokenFound((object) authorizationId);
          requestContext.Trace(1048051, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), message);
          throw new SessionTokenNotFoundException(message);
        }
        Guid guid = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
        if (accessTokenKey.UserId != guid && !vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        {
          string getToken = DelegatedAuthorizationResources.NoPermissionToGetToken((object) guid, (object) authorizationId);
          requestContext.Trace(1048052, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), getToken);
          throw new AccessCheckException(getToken);
        }
        List<Guid> guidList = (List<Guid>) null;
        if (!string.IsNullOrEmpty(accessTokenKey.Audience))
          guidList = ((IEnumerable<string>) accessTokenKey.Audience.Split(new string[1]
          {
            "|"
          }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (x => x.StartsWith("vso:"))).Select<string, Guid>((Func<string, Guid>) (x =>
          {
            Guid result;
            Guid.TryParse(x.Substring("vso:".Length), out result);
            return result;
          })).Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>();
        return new SessionToken()
        {
          AccessId = accessTokenKey.AccessId,
          AuthorizationId = accessTokenKey.AuthorizationId,
          UserId = accessTokenKey.UserId,
          ValidFrom = accessTokenKey.ValidFrom,
          ValidTo = accessTokenKey.ValidTo,
          DisplayName = accessTokenKey.DisplayName,
          Scope = accessTokenKey.Scope,
          TargetAccounts = (IList<Guid>) guidList,
          IsValid = accessTokenKey.IsValid,
          IsPublic = accessTokenKey.IsPublic,
          PublicData = accessTokenKey.PublicData,
          Source = accessTokenKey.Source
        };
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1048058, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1048059, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetSessionTokenInternal));
      }
    }

    private IList<SessionToken> AccessTokenKeyListToSessionTokenList(
      IList<AccessTokenKey> accessTokenKey)
    {
      if (accessTokenKey == null)
        return (IList<SessionToken>) new List<SessionToken>();
      List<SessionToken> sessionTokenList = new List<SessionToken>();
      foreach (AccessTokenKey accessTokenKey1 in (IEnumerable<AccessTokenKey>) accessTokenKey)
      {
        List<Guid> guidList = (List<Guid>) null;
        if (!string.IsNullOrEmpty(accessTokenKey1.Audience))
          guidList = ((IEnumerable<string>) accessTokenKey1.Audience.Split(new string[1]
          {
            "|"
          }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (x => x.StartsWith("vso:"))).Select<string, Guid>((Func<string, Guid>) (x =>
          {
            Guid result;
            Guid.TryParse(x.Substring("vso:".Length), out result);
            return result;
          })).Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>();
        sessionTokenList.Add(new SessionToken()
        {
          AccessId = accessTokenKey1.AccessId,
          AuthorizationId = accessTokenKey1.AuthorizationId,
          UserId = accessTokenKey1.UserId,
          DisplayName = accessTokenKey1.DisplayName,
          Scope = accessTokenKey1.Scope,
          ValidFrom = accessTokenKey1.ValidFrom,
          ValidTo = accessTokenKey1.ValidTo,
          TargetAccounts = (IList<Guid>) guidList,
          IsValid = accessTokenKey1.IsValid,
          IsPublic = accessTokenKey1.IsPublic,
          PublicData = accessTokenKey1.PublicData,
          Source = accessTokenKey1.Source
        });
      }
      return (IList<SessionToken>) sessionTokenList;
    }

    public IList<SessionToken> ListSessionTokens(
      IVssRequestContext requestContext,
      bool isPublic = false,
      bool includePublicData = false)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokens));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return this.ExecuteListServiceMethods<SessionToken>(requestContext, (Func<IVssRequestContext, IEnumerable<SessionToken>>) (context => (IEnumerable<SessionToken>) this.ListSessionTokensInternal(context, isPublic, includePublicData)), (Func<IVssRequestContext, IEnumerable<SessionToken>>) (context => (IEnumerable<SessionToken>) this.ListSessionTokensRemote(context, isPublic, includePublicData)), "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokens));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokens));
      }
    }

    private string GetDictionaryKey(SessionToken sessionToken) => sessionToken.AccessId.ToString();

    private IDictionary<string, SessionToken> ToDictionary(IEnumerable<SessionToken> list) => (IDictionary<string, SessionToken>) list.ToDictionary<SessionToken, string, SessionToken>((Func<SessionToken, string>) (s => this.GetDictionaryKey(s)), (Func<SessionToken, SessionToken>) (s => s));

    private IList<SessionToken> ListSessionTokensRemote(
      IVssRequestContext requestContext,
      bool isPublic = false,
      bool includePublicData = false)
    {
      return requestContext.GetService<ITokenService>().ListSessionTokens(requestContext, isPublic, includePublicData);
    }

    private IList<SessionToken> ListSessionTokensInternal(
      IVssRequestContext requestContext,
      bool isPublic = false,
      bool includePublicData = false)
    {
      requestContext.TraceEnter(1048020, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensInternal));
      try
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        Guid userId = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
        List<AccessTokenKey> accessTokenKey;
        using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
          accessTokenKey = component.ListAccessTokenKeys(userId, isPublic, includePublicData);
        return this.AccessTokenKeyListToSessionTokenList((IList<AccessTokenKey>) accessTokenKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1048021, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1048022, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensInternal));
      }
    }

    public PagedSessionTokens ListSessionTokensByPage(
      IVssRequestContext requestContext,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      requestContext.TraceEnter(1048031, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensByPage));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return this.ExecuteServiceMethods<PagedSessionTokens>(requestContext, (Func<IVssRequestContext, PagedSessionTokens>) (context => this.ListSessionTokensByPageInternal(context, tokenPageRequest, isPublic, includePublicData)), (Func<IVssRequestContext, PagedSessionTokens>) (context => this.ListSessionTokensByPageRemote(context, tokenPageRequest, isPublic, includePublicData)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensByPage));
      }
      finally
      {
        requestContext.TraceLeave(1048027, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensByPage));
      }
    }

    private PagedSessionTokens ListSessionTokensByPageRemote(
      IVssRequestContext requestContext,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      return requestContext.GetService<ITokenService>().ListSessionTokensByPage(requestContext, tokenPageRequest, isPublic, includePublicData);
    }

    private PagedSessionTokens ListSessionTokensByPageInternal(
      IVssRequestContext requestContext,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      requestContext.TraceEnter(1048031, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensByPageInternal));
      try
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        Guid masterId = requestContext.GetUserIdentity().MasterId;
        PagedSessionTokens pagedSessionTokens = new PagedSessionTokens();
        using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
        {
          AccessTokenKeyPage accessTokenKeyPage = component.ListAccessTokensByPage(masterId, tokenPageRequest, isPublic, includePublicData);
          pagedSessionTokens.SessionTokens = this.AccessTokenKeyListToSessionTokenList(accessTokenKeyPage.AccessTokenKeyList);
          pagedSessionTokens.NextRowNumber = accessTokenKeyPage.NextRowNumber;
          return pagedSessionTokens;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1048026, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1048027, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ListSessionTokensByPageInternal));
      }
    }

    public SessionToken UpdateSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateSessionToken));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        return this.ExecuteServiceMethods<SessionToken>(requestContext, (Func<IVssRequestContext, SessionToken>) (context => this.UpdateSessionTokenInternal(context, authorizationId, displayName, scope, validTo, targetAccounts, isPublic)), (Func<IVssRequestContext, SessionToken>) (context => this.UpdateSessionTokenRemote(context, authorizationId, displayName, scope, validTo, targetAccounts, isPublic)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateSessionToken));
      }
      finally
      {
        requestContext.TraceLeave(1048009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateSessionToken));
      }
    }

    private SessionToken UpdateSessionTokenRemote(
      IVssRequestContext requestContext,
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      bool isPublic = false)
    {
      return requestContext.GetService<ITokenService>().UpdateSessionToken(requestContext, authorizationId, displayName, scope, validTo, targetAccounts, isPublic);
    }

    private SessionToken UpdateSessionTokenInternal(
      IVssRequestContext requestContext,
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      bool isPublic = false)
    {
      try
      {
        requestContext.TraceEnter(1049000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateSessionTokenInternal));
        ArgumentUtility.CheckForEmptyGuid(authorizationId, nameof (authorizationId));
        AccessTokenKey accessTokenKey1 = (AccessTokenKey) null;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          accessTokenKey1 = component.GetAccessTokenKey(authorizationId, isPublic);
        if (accessTokenKey1 == null || !accessTokenKey1.IsValid)
        {
          string message = DelegatedAuthorizationResources.NoValidSessionTokenFound((object) authorizationId);
          requestContext.Trace(1049001, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), message);
          throw new SessionTokenNotFoundException(message);
        }
        if (accessTokenKey1.UserId != requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment))
        {
          requestContext.Trace(1049002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity is not same as target identity for update session token.");
          if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
          {
            requestContext.Trace(1049002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity does not have framework impersonate permissions.");
            throw new AccessCheckException("Request identity does not have framework impersonate permissions.");
          }
        }
        IDelegatedAuthorizationConfigurationService service1 = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
        DelegatedAuthorizationSettings settings = service1.GetSettings(vssRequestContext);
        AuthorizationScopeConfiguration configuration = service1.GetConfiguration(vssRequestContext);
        validTo = SessionTokenLifetimePolicyFactory.Instance.Create(requestContext, settings, accessTokenKey1.UserId, validTo).ApplyForUpdate();
        if (!string.IsNullOrWhiteSpace(scope))
        {
          try
          {
            if (!string.Equals("app_token", scope, StringComparison.OrdinalIgnoreCase))
              scope = configuration.NormalizeScopes(scope, out bool _);
          }
          catch
          {
            throw new ArgumentException("Given scope is not valid.");
          }
        }
        if (!string.IsNullOrWhiteSpace(displayName))
          displayName = !ArgumentUtility.IsInvalidString(displayName, false, true) && displayName.Length <= 256 ? AntiXssEncoder.HtmlEncode(displayName, false) : throw new ArgumentException("DisplayName is not valid.");
        string audience = (string) null;
        if (targetAccounts != null)
          audience = targetAccounts.Count == 0 ? string.Empty : string.Join("|", targetAccounts.Select<Guid, string>((Func<Guid, string>) (x => "vso:" + x.ToString())));
        AccessTokenKey accessTokenKey2 = (AccessTokenKey) null;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          accessTokenKey2 = component.UpdateAccessToken(authorizationId, displayName, scope, validTo, audience, isPublic);
        if (accessTokenKey2 == null)
        {
          requestContext.Trace(1049004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Update session token meta data failed.");
          return (SessionToken) null;
        }
        string accessHash = accessTokenKey2.AccessHash;
        if (!string.IsNullOrWhiteSpace(accessHash))
        {
          requestContext.Trace(1049005, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Sending service bus notification to invalid session token cache for {0}", (object) accessHash);
          TeamFoundationTaskService service2 = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
          TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishServiceBusNotification), (object) new List<string>()
          {
            accessHash
          }, 0);
          Guid instanceId = requestContext.ServiceHost.InstanceId;
          TeamFoundationTask task = teamFoundationTask;
          service2.AddTask(instanceId, task);
        }
        return new SessionToken()
        {
          AccessId = accessTokenKey2.AccessId,
          AuthorizationId = accessTokenKey2.AuthorizationId,
          UserId = accessTokenKey2.UserId,
          ValidFrom = accessTokenKey2.ValidFrom,
          ValidTo = accessTokenKey2.ValidTo,
          DisplayName = accessTokenKey2.DisplayName,
          Scope = accessTokenKey2.Scope,
          TargetAccounts = this.GetTargetAccountsList(accessTokenKey2.Audience),
          IsValid = accessTokenKey2.IsValid,
          IsPublic = accessTokenKey2.IsPublic,
          PublicData = accessTokenKey2.PublicData,
          Source = accessTokenKey2.Source
        };
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1049008, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1049009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateSessionTokenInternal));
      }
    }

    public SessionTokenResult IssueSessionToken(
      IVssRequestContext requestContext,
      Guid? clientId = null,
      Guid? userId = null,
      string name = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      bool isPublic = false,
      string publicData = null,
      string source = null,
      bool isRequestedByTfsPatWebUI = false,
      Guid? authorizationId = null,
      Guid? accessId = null,
      IDictionary<string, string> customClaims = null)
    {
      requestContext.TraceEnter(1048000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueSessionToken));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 2);
        if (!authorizationId.HasValue || authorizationId.Value == Guid.Empty)
        {
          authorizationId = new Guid?(Guid.NewGuid());
          accessId = new Guid?(Guid.NewGuid());
        }
        if (string.IsNullOrEmpty(name))
          name = Guid.NewGuid().ToString();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (isPublic && vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          Policy<bool> policy = requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext, "Policy.DisallowSecureShell", false);
          if ((policy != null ? (policy.EffectiveValue ? 1 : 0) : 0) != 0)
          {
            requestContext.Trace(1048063, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Organization policy has SSH keys disabled.");
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.SSHPolicyDisabled
            };
          }
        }
        Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        try
        {
          this.CheckIdentityIssueTokenIsImpersonating(requestContext, out targetIdentity, userId);
          this.CheckRequestIdentityCanGenerateToken(requestContext, tokenType);
          this.TryAuthenticateTheCredentialsBeforeIssuingToken(requestContext);
        }
        catch (PlatformDelegatedAuthorizationServiceException ex)
        {
          return ex.ToSessionTokenResult();
        }
        SessionTokenResult sessionTokenResult = this.ExecuteServiceMethods<SessionTokenResult>(requestContext, (Func<IVssRequestContext, SessionTokenResult>) (context => this.IssueSessionTokenInternal(context, clientId, userId, name, validTo, scope, targetAccounts, tokenType, isPublic, publicData, source, isRequestedByTfsPatWebUI, authorizationId, accessId, customClaims)), (Func<IVssRequestContext, SessionTokenResult>) (context => this.IssueSessionTokenRemote(context, clientId, userId, name, validTo, scope, targetAccounts, tokenType, isPublic, publicData, source, isRequestedByTfsPatWebUI, authorizationId, accessId, customClaims, targetIdentity)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueSessionToken));
        if (!sessionTokenResult.HasError && tokenType == SessionTokenType.Compact && !requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          IDelegatedAuthorizationNotificationService service = requestContext.GetService<IDelegatedAuthorizationNotificationService>();
          if (isPublic)
            service.SendSSHKeyAddedNotification(requestContext, sessionTokenResult.SessionToken.UserId, sessionTokenResult.SessionToken.DisplayName, publicData, sessionTokenResult.SessionToken.TargetAccounts);
          else
            service.SendPATAddedNotification(requestContext, sessionTokenResult.SessionToken.UserId, clientId, sessionTokenResult.SessionToken.DisplayName, sessionTokenResult.SessionToken.ValidTo, sessionTokenResult.SessionToken.Scope, sessionTokenResult.SessionToken.TargetAccounts, sessionTokenResult.SessionToken.AuthorizationId);
        }
        return sessionTokenResult;
      }
      finally
      {
        requestContext.TraceLeave(1048009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueSessionToken));
      }
    }

    private void CheckRequestIdentityCanGenerateToken(
      IVssRequestContext requestContext,
      SessionTokenType tokenType)
    {
      if (tokenType != SessionTokenType.OpenIdConnect)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      if (!ServicePrincipals.IsServicePrincipal(requestContext, authenticatedIdentity.Descriptor))
      {
        requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Cannot issue IdToken for user identity - {0}.", (object) authenticatedIdentity.Id);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.AccessDenied);
      }
    }

    private SessionTokenResult IssueSessionTokenRemote(
      IVssRequestContext requestContext,
      Guid? clientId = null,
      Guid? userId = null,
      string name = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      bool isPublic = false,
      string publicData = null,
      string source = null,
      bool isRequestedByTfsPatWebUI = false,
      Guid? authorizationId = null,
      Guid? accessId = null,
      IDictionary<string, string> customClaims = null,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = null)
    {
      requestContext.Trace(2048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Entering CreateSessionTokenHttp");
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      Guid guid1 = instanceId;
      Guid guid2 = guid1;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        instanceId = requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId;
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          guid2 = requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
      }
      try
      {
        bool flag = false;
        Guid? nullable1 = new Guid?();
        Guid? nullable2;
        if (userId.HasValue)
        {
          nullable2 = userId;
          Guid empty = Guid.Empty;
          if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          {
            Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
            Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
            if (userIdentity == authenticatedIdentity && ServicePrincipals.IsServicePrincipal(requestContext, userIdentity.Descriptor))
            {
              flag = true;
              nullable1 = new Guid?(authenticatedIdentity.Id);
            }
            else if (targetIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment) == userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment))
              userId = new Guid?();
          }
        }
        IVssRequestContext context = flag ? requestContext.Elevate() : requestContext;
        ITokenService service = context.GetService<ITokenService>();
        IVssRequestContext requestContext1 = context;
        Guid hostId = guid1;
        Guid orgHostId = guid2;
        Guid deploymentHostId = instanceId;
        nullable2 = userId;
        Guid? clientId1 = clientId;
        Guid? userId1 = nullable2;
        string name1 = name;
        DateTime? validTo1 = validTo;
        string scope1 = scope;
        IList<Guid> targetAccounts1 = targetAccounts;
        int tokenType1 = (int) tokenType;
        int num1 = isPublic ? 1 : 0;
        string publicData1 = publicData;
        string source1 = source;
        int num2 = isRequestedByTfsPatWebUI ? 1 : 0;
        Guid? nullable3 = authorizationId;
        Guid empty1 = Guid.Empty;
        Guid? authorizationId1;
        if ((nullable3.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() != empty1 ? 1 : 0) : 0) : 1) == 0)
        {
          nullable3 = new Guid?();
          authorizationId1 = nullable3;
        }
        else
          authorizationId1 = authorizationId;
        nullable3 = accessId;
        Guid empty2 = Guid.Empty;
        Guid? accessId1;
        if ((nullable3.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() != empty2 ? 1 : 0) : 0) : 1) == 0)
        {
          nullable3 = new Guid?();
          accessId1 = nullable3;
        }
        else
          accessId1 = accessId;
        IDictionary<string, string> customClaims1 = customClaims;
        Guid? requestedById = nullable1;
        SessionTokenResult sessionTokenResult = service.IssueSessionToken(requestContext1, hostId, orgHostId, deploymentHostId, clientId1, userId1, name1, validTo1, scope1, targetAccounts1, (SessionTokenType) tokenType1, num1 != 0, publicData1, source1, num2 != 0, authorizationId1, accessId1, customClaims: customClaims1, requestedById: requestedById);
        SessionTokenTracing.TraceTokenIssuance(tokenType, sessionTokenResult.SessionTokenError, sessionTokenResult.SessionToken);
        return sessionTokenResult;
      }
      catch (Exception ex)
      {
        SessionTokenError result;
        if (System.Enum.TryParse<SessionTokenError>(ex.Message, out result))
          return new SessionTokenResult()
          {
            SessionTokenError = result
          };
        throw;
      }
    }

    private SessionTokenResult IssueSessionTokenInternal(
      IVssRequestContext requestContext,
      Guid? clientId = null,
      Guid? userId = null,
      string name = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      bool isPublic = false,
      string publicData = null,
      string source = null,
      bool isRequestedByTfsPatWebUI = false,
      Guid? authorizationId = null,
      Guid? accessId = null,
      IDictionary<string, string> customClaims = null)
    {
      requestContext.TraceEnter(1048000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueSessionTokenInternal));
      if (tokenType == SessionTokenType.OpenIdConnect)
        return new SessionTokenResult()
        {
          SessionTokenError = SessionTokenError.InvalidTokenType
        };
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IdentityService service1 = vssRequestContext.GetService<IdentityService>();
        IVssRequestContext requestContext1 = requestContext;
        Guid? nullable1 = userId;
        Guid? userId1 = new Guid?(nullable1 ?? Guid.Empty);
        Microsoft.VisualStudio.Services.Identity.Identity identity1;
        ref Microsoft.VisualStudio.Services.Identity.Identity local1 = ref identity1;
        Microsoft.VisualStudio.Services.Identity.Identity identity2;
        ref Microsoft.VisualStudio.Services.Identity.Identity local2 = ref identity2;
        userId = new Guid?(this.GetUserIdCheckForEmpty(requestContext1, userId1, out local1, out local2, true));
        if (!userId.HasValue || userId.Value == Guid.Empty)
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.UserIdRequired
          };
        if (identity2 == null)
        {
          if (userId.HasValue)
            requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Target identity {0} is not available in the system.", (object) userId.Value);
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.InvalidUserId
          };
        }
        if (string.IsNullOrEmpty(name))
          name = Guid.NewGuid().ToString();
        if (this.IsCurrentRequestAuthenticatedUsingAlternateCredentials(vssRequestContext) && !vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        {
          requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Alternate credentials cannot be used to request session token.");
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.AccessDenied
          };
        }
        name = AntiXssEncoder.HtmlEncode(name, false);
        if (string.IsNullOrWhiteSpace(name))
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.DisplayNameRequired
          };
        if (ArgumentUtility.IsInvalidString(name, false, true) || name.Length > 256)
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.InvalidDisplayName
          };
        if (!string.IsNullOrWhiteSpace(source))
        {
          source = source.ToLower().Replace("ipa:0.0.0.0", "ipa:" + requestContext.RemoteIPAddress());
          SessionTokenError sessionTokenError = this.ValidateSource(requestContext, source);
          if (sessionTokenError != SessionTokenError.None)
          {
            requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Source value {0} is not in supported format.", (object) source);
            return new SessionTokenResult()
            {
              SessionTokenError = sessionTokenError
            };
          }
        }
        IDelegatedAuthorizationConfigurationService service2 = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
        DelegatedAuthorizationSettings settings = service2.GetSettings(vssRequestContext);
        AuthorizationScopeConfiguration configuration = service2.GetConfiguration(vssRequestContext);
        if (!string.IsNullOrWhiteSpace(scope))
        {
          try
          {
            if (!string.Equals("app_token", scope, StringComparison.OrdinalIgnoreCase))
              scope = configuration.NormalizeScopes(scope, out bool _);
          }
          catch
          {
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.InvalidScope
            };
          }
        }
        else
        {
          if (clientId.HasValue)
          {
            nullable1 = clientId;
            Guid empty = Guid.Empty;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
              goto label_26;
          }
          scope = "app_token";
        }
label_26:
        if (!IdentityHelper.IsUserIdentity(vssRequestContext, (IReadOnlyVssIdentity) identity2) && identity2.Descriptor.IdentityType != "Microsoft.TeamFoundation.ServiceIdentity")
        {
          requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Cannot issue token for service identity - {0}.", (object) identity2.Id);
          return new SessionTokenResult()
          {
            SessionTokenError = SessionTokenError.InvalidUserType
          };
        }
        if (!IdentityHelper.IsShardedFrameworkIdentity(requestContext, identity2.Descriptor))
        {
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service1.ReadIdentities(vssRequestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (service1.IsMember(vssRequestContext, readIdentity.Descriptor, identity2.Descriptor) && requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Cannot issue a token for deployment administrator - {0}.", (object) identity2.Id);
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.InvalidUserType
            };
          }
        }
        Guid guid1 = identity1.StorageKey(requestContext, TeamFoundationHostType.Deployment);
        Guid guid2 = identity2.StorageKey(requestContext, TeamFoundationHostType.Deployment);
        if (guid1 != guid2)
        {
          requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity is not same as target identity for issue token.");
          if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
          {
            requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity {0} does not have framework impersonate permissions, target identity {1}.", (object) guid1, (object) guid2);
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.AccessDenied
            };
          }
        }
        DateTime? nullable2 = validTo;
        DateTime dateTime = new DateTime();
        DateTime? nullable3;
        if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == dateTime ? 1 : 0) : 1) : 0) == 0)
        {
          nullable3 = validTo;
        }
        else
        {
          nullable2 = new DateTime?();
          nullable3 = nullable2;
        }
        DateTime? validTo1 = nullable3;
        if (validTo1.HasValue && validTo1.Value.Kind == DateTimeKind.Unspecified)
          validTo1 = new DateTime?(DateTime.SpecifyKind(validTo1.Value, DateTimeKind.Utc));
        validTo = new DateTime?(SessionTokenLifetimePolicyFactory.Instance.Create(requestContext, settings, guid2, validTo1).ApplyForCreate());
        if (tokenType == SessionTokenType.Compact)
        {
          try
          {
            this.GetFrameworkAccessTokenSecret(vssRequestContext);
          }
          catch (Exception ex)
          {
            switch (ex)
            {
              case StrongBoxDrawerNotFoundException _:
              case StrongBoxItemNotFoundException _:
                return new SessionTokenResult()
                {
                  SessionTokenError = SessionTokenError.FailedToIssueAccessToken
                };
              default:
                throw;
            }
          }
        }
        Guid? nullable4 = new Guid?();
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          Guid instanceId1 = requestContext.ServiceHost.InstanceId;
          Guid instanceId2 = requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
          if (targetAccounts == null)
            targetAccounts = (IList<Guid>) new List<Guid>();
          if (targetAccounts.Count == 0)
          {
            targetAccounts.Add(instanceId1);
          }
          else
          {
            if (targetAccounts.Count > 1)
            {
              requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Target accounts counts is not expected.");
              return new SessionTokenResult()
              {
                SessionTokenError = SessionTokenError.InvalidTargetAccounts
              };
            }
            if (instanceId1 != targetAccounts[0] && instanceId2 != targetAccounts[0])
            {
              requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Target accounts does not match with current host.");
              return new SessionTokenResult()
              {
                SessionTokenError = SessionTokenError.InvalidTargetAccounts
              };
            }
          }
          if (clientId.HasValue)
          {
            nullable1 = clientId;
            Guid empty = Guid.Empty;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
            {
              HostAuthorization hostAuthorization = (HostAuthorization) null;
              using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
              {
                hostAuthorization = component.GetHostAuthorization(clientId.Value, instanceId1);
                if (hostAuthorization == null && instanceId1 != instanceId2)
                  hostAuthorization = component.GetHostAuthorization(clientId.Value, instanceId2);
                if (hostAuthorization == null)
                {
                  Guid instanceId3 = requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId;
                  hostAuthorization = component.GetHostAuthorization(clientId.Value, instanceId3);
                }
              }
              if (hostAuthorization == null)
              {
                requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Cannot issue SessionToken for input clientId and target account. Host authorization entry is missing.");
                return new SessionTokenResult()
                {
                  SessionTokenError = SessionTokenError.HostAuthorizationNotFound
                };
              }
              nullable4 = new Guid?(hostAuthorization.Id);
            }
          }
        }
        string str1 = (string) null;
        if (targetAccounts != null && targetAccounts.Count > 0)
          str1 = string.Join("|", targetAccounts.Select<Guid, string>((Func<Guid, string>) (x => "vso:" + x.ToString())));
        AccessTokenData accessTokenData1 = (AccessTokenData) null;
        if (tokenType == SessionTokenType.SelfDescribing)
        {
          requestContext.Trace(1048003, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Skipping DB writes for self-describing session token.");
          if (clientId.HasValue)
          {
            nullable1 = clientId;
            Guid empty = Guid.Empty;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
            {
              Registration registration;
              using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
              {
                registration = component.GetRegistration(Guid.Empty, clientId.Value);
                if (registration == null)
                {
                  string message = DelegatedAuthorizationResources.RegistrationNotFound((object) clientId.Value);
                  requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), message);
                  return new SessionTokenResult()
                  {
                    SessionTokenError = SessionTokenError.InvalidClientId
                  };
                }
              }
              if (registration.ClientType != ClientType.Public)
              {
                requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Invalid client type");
                return new SessionTokenResult()
                {
                  SessionTokenError = SessionTokenError.InvalidClientType
                };
              }
              if (string.IsNullOrWhiteSpace(scope))
                scope = registration.Scopes;
              else if (!string.Equals(scope, registration.Scopes, StringComparison.OrdinalIgnoreCase))
              {
                requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client scope does not match.");
                return new SessionTokenResult()
                {
                  SessionTokenError = SessionTokenError.InvalidScope
                };
              }
            }
          }
          DateTime tokenFromValidDate = this.GetTokenFromValidDate();
          AccessTokenData accessTokenData2 = new AccessTokenData();
          accessTokenData2.AccessToken = new AccessToken()
          {
            AccessId = !accessId.HasValue ? Guid.NewGuid() : accessId.Value,
            AuthorizationId = !authorizationId.HasValue ? Guid.NewGuid() : authorizationId.Value,
            ValidFrom = (DateTimeOffset) tokenFromValidDate,
            ValidTo = (DateTimeOffset) validTo.Value.ToUniversalTime(),
            Refreshed = DateTimeOffset.MinValue,
            IsRefresh = false,
            IsValid = true
          };
          Authorization authorization = new Authorization();
          authorization.AuthorizationId = !authorizationId.HasValue ? Guid.NewGuid() : authorizationId.Value;
          authorization.Scopes = scope;
          authorization.ValidFrom = (DateTimeOffset) tokenFromValidDate;
          authorization.ValidTo = (DateTimeOffset) validTo.Value;
          nullable1 = clientId;
          authorization.RegistrationId = nullable1 ?? this.WellKnownRegistrationIDForAppToken;
          authorization.Audience = str1;
          authorization.Source = source;
          authorization.IdentityId = guid2;
          accessTokenData2.Authorization = authorization;
          accessTokenData1 = accessTokenData2;
        }
        else
        {
          try
          {
            using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
            {
              DelegatedAuthorizationComponent authorizationComponent = component;
              Guid userId2 = guid2;
              Guid clientId1;
              if (!isRequestedByTfsPatWebUI)
              {
                nullable1 = clientId;
                clientId1 = nullable1 ?? this.WellKnownRegistrationIDForAppToken;
              }
              else
                clientId1 = this.TfsPatWebUiClientId;
              string scope1 = scope;
              DateTime validTo2 = validTo.Value;
              string audience = str1;
              string source1 = source;
              int num = isRequestedByTfsPatWebUI ? 1 : 0;
              Guid? authorizationId1 = authorizationId;
              Guid? accessId1 = accessId;
              accessTokenData1 = authorizationComponent.IssueAccessToken(userId2, clientId1, -1, scope1, validTo2, audience, source1, num != 0, authorizationId1, accessId1);
            }
          }
          catch (RegistrationNotFoundException ex)
          {
            requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client registration not found.");
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.InvalidClientId
            };
          }
          catch (InvalidAuthorizationScopeException ex)
          {
            requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client scope does not match.");
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.InvalidScope
            };
          }
          catch (InvalidClientTypeException ex)
          {
            requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Invalid client type");
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.InvalidClientType
            };
          }
          if (accessTokenData1 == null)
          {
            requestContext.Trace(1048004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Access token creation failed.");
            return new SessionTokenResult()
            {
              SessionTokenError = SessionTokenError.FailedToIssueAccessToken
            };
          }
        }
        string str2 = (string) null;
        string str3 = (string) null;
        if (tokenType == SessionTokenType.Compact)
        {
          (string, string) cryptoString;
          if (isPublic)
          {
            Guid hostId = Guid.Empty;
            if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ExecutionEnvironment.IsHostedDeployment)
              hostId = targetAccounts.Count != 1 ? requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId : targetAccounts.Single<Guid>();
            cryptoString = CryptoStringSecretGeneratorHelper.GenerateCryptoString(vssRequestContext, hostId, vssRequestContext.ServiceHost.InstanceId, publicData, "Framework drawer does not exist");
          }
          else
            cryptoString = CryptoStringSecretGeneratorHelper.GenerateCryptoString(vssRequestContext, CryptoStringType.PAT, "Framework drawer does not exist");
          str2 = cryptoString.Item1;
          string str4 = cryptoString.Item2;
          if (isPublic)
          {
            using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
            {
              try
              {
                if (component.GetAccessTokenByKey(str4, isPublic) != null)
                {
                  requestContext.Trace(1048008, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Generated token hash: " + str4 + " resulted in a duplicate, duplicate token " + str3);
                  return new SessionTokenResult()
                  {
                    SessionTokenError = SessionTokenError.DuplicateHash
                  };
                }
              }
              catch (InvalidAccessTokenKeyException ex)
              {
                requestContext.TraceException(1048008, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), (Exception) ex);
              }
            }
          }
          using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
            component.CreateAccessTokenKey(accessTokenData1.AccessToken.AccessId, str4, name, guid2, isPublic, publicData);
          requestContext.Trace(1048005, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Generated compact session token.");
        }
        VssSigningCredentials signingCredentials1 = service2.GetSigningCredentials(vssRequestContext, true);
        IVssRequestContext requestContext2 = vssRequestContext;
        Guid resourceOwnerIdentity = guid2;
        Guid authorizationId2 = accessTokenData1.AccessToken.AuthorizationId;
        string scopes = accessTokenData1.Authorization.Scopes;
        DateTime utcDateTime1 = accessTokenData1.AccessToken.ValidFrom.UtcDateTime;
        DateTime utcDateTime2 = accessTokenData1.AccessToken.ValidTo.UtcDateTime;
        VssSigningCredentials signingCredentials2 = signingCredentials1;
        string audience1 = accessTokenData1.Authorization.Audience;
        Guid? accessId2 = new Guid?(accessTokenData1.AccessToken.AccessId);
        string source2 = accessTokenData1.Authorization.Source;
        Guid? hostAuthorizationId = nullable4;
        IDictionary<string, string> dictionary = customClaims;
        nullable1 = new Guid?();
        Guid? appId = nullable1;
        IDictionary<string, string> customClaims1 = dictionary;
        string encodedToken = this.GenerateAccessToken(requestContext2, resourceOwnerIdentity, authorizationId2, scopes, utcDateTime1, utcDateTime2, signingCredentials2, audience1, accessId: accessId2, source: source2, hostAuthorizationId: hostAuthorizationId, appId: appId, customClaims: customClaims1).EncodedToken;
        requestContext.Trace(1048006, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Using access token as is for session token.");
        SessionToken sessionToken = new SessionToken()
        {
          AccessId = accessTokenData1.AccessToken.AccessId,
          AuthorizationId = accessTokenData1.Authorization.AuthorizationId,
          ClientId = accessTokenData1.Authorization.RegistrationId,
          UserId = accessTokenData1.Authorization.IdentityId,
          ValidFrom = accessTokenData1.AccessToken.ValidFrom.UtcDateTime,
          ValidTo = accessTokenData1.AccessToken.ValidTo.UtcDateTime,
          TargetAccounts = this.GetTargetAccountsList(accessTokenData1.Authorization.Audience),
          Source = accessTokenData1.Authorization.Source,
          DisplayName = name,
          Scope = accessTokenData1.Authorization.Scopes,
          IsPublic = isPublic,
          PublicData = publicData,
          IsValid = accessTokenData1.AccessToken.IsValid
        };
        if (nullable4.HasValue)
          sessionToken.HostAuthorizationId = nullable4.Value;
        if (tokenType == SessionTokenType.Compact)
        {
          sessionToken.Token = str2;
          sessionToken.AlternateToken = encodedToken;
        }
        else
          sessionToken.Token = encodedToken;
        return new SessionTokenResult()
        {
          SessionToken = sessionToken
        };
      }
      finally
      {
        requestContext.TraceLeave(1048009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueSessionTokenInternal));
      }
    }

    public AppSessionTokenResult IssueAppSessionToken(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? userId,
      Guid? authorizationId = null)
    {
      requestContext.TraceEnter(1048023, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueAppSessionToken));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 2);
        if (!authorizationId.HasValue)
          authorizationId = new Guid?(Guid.NewGuid());
        return this.ExecuteServiceMethods<AppSessionTokenResult>(requestContext, (Func<IVssRequestContext, AppSessionTokenResult>) (context => this.IssueAppSessionTokenInternal(context, clientId, userId, authorizationId)), (Func<IVssRequestContext, AppSessionTokenResult>) (context => this.IssueAppSessionTokenRemote(context, clientId, userId, authorizationId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueAppSessionToken));
      }
      finally
      {
        requestContext.TraceLeave(1048029, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueAppSessionToken));
      }
    }

    private AppSessionTokenResult IssueAppSessionTokenRemote(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? userId,
      Guid? authorizationId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = new Guid?(this.GetUserIdCheckForEmpty(requestContext, userId, out Microsoft.VisualStudio.Services.Identity.Identity _, out targetIdentity));
      try
      {
        this.TryValidateIdentityBeforeIssuingToken(requestContext, userId);
      }
      catch (PlatformDelegatedAuthorizationServiceException ex)
      {
        return ex.ToAppSessionTokenResult();
      }
      return TokenServiceBase.ExecuteTokenServiceResultRequest<AppSessionTokenResult>(requestContext, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), (Func<IVssRequestContext, bool, AppSessionTokenResult>) ((context, isImpersonating) =>
      {
        try
        {
          return context.GetService<ITokenService>().IssueAppSessionToken(context, targetIdentity.SubjectDescriptor, clientId, authorizationId);
        }
        catch (Exception ex)
        {
          AppSessionTokenError result;
          if (System.Enum.TryParse<AppSessionTokenError>(ex.Message, out result))
            return new AppSessionTokenResult()
            {
              AppSessionTokenError = result
            };
          throw;
        }
      }), userId);
    }

    private AppSessionTokenResult IssueAppSessionTokenInternal(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? userId,
      Guid? authorizationId = null)
    {
      requestContext.TraceEnter(1048023, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueAppSessionTokenInternal));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (clientId == Guid.Empty)
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.ClientIdRequired
          };
        if (!userId.HasValue || userId.Value == Guid.Empty)
          userId = new Guid?(requestContext.GetUserId());
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<Guid>) new Guid[1]
        {
          userId.Value
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
          identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext.Elevate(), (IList<Guid>) new Guid[1]
          {
            userId.Value
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.InvalidUserId
          };
        if (!IdentityHelper.IsUserIdentity(vssRequestContext, (IReadOnlyVssIdentity) identity))
        {
          requestContext.Trace(1048025, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "IssueAccessTokenKey is requested for non user identity.");
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.InvalidUserType
          };
        }
        if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        {
          requestContext.Trace(1048026, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "User does not have permission to issue app token.");
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.AccessDenied
          };
        }
        Registration registration;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.GetRegistration(Guid.Empty, clientId);
        if (registration == null)
        {
          requestContext.Trace(1048027, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client registration not found, make sure the client id is correct.");
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.InvalidClientId
          };
        }
        JsonWebToken secretInternal = vssRequestContext.GetService<PlatformDelegatedAuthorizationRegistrationService>().GetSecretInternal(vssRequestContext, registration);
        if (secretInternal == null)
        {
          requestContext.Trace(1048028, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "GetSecret resulted in null client secret.");
          return new AppSessionTokenResult()
          {
            AppSessionTokenError = AppSessionTokenError.FailedToIssueAppSessionToken
          };
        }
        List<Claim> additionalClaims = new List<Claim>()
        {
          new Claim("nameid", identity.Id.ToString())
        };
        object obj;
        if (identity.TryGetProperty("Domain", out obj))
          additionalClaims.Add(new Claim("tid", obj.ToString()));
        DelegatedAuthorizationSettings settings = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(vssRequestContext);
        if (registration.ClientType == ClientType.HighTrust)
        {
          if (identity.TryGetProperty("PUID", out obj))
            additionalClaims.Add(new Claim(DelegatedAuthorizationTokenClaims.Puid, obj.ToString()));
          if (identity.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out obj))
            additionalClaims.Add(new Claim(DelegatedAuthorizationTokenClaims.Oid, obj.ToString()));
          Authorization authorization = (Authorization) null;
          try
          {
            using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
              authorization = component.Authorize(identity.StorageKey(requestContext, TeamFoundationHostType.Deployment), (ResponseType) System.Enum.Parse(typeof (ResponseType), registration.ResponseTypes, true), clientId, registration.RedirectUris.FirstOrDefault<Uri>(), registration.Scopes, DateTime.UtcNow, DateTime.UtcNow.Add(settings.AuthorizationGrantLifetime), authorizationId: authorizationId);
          }
          catch (Exception ex)
          {
            requestContext.Trace(1048030, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Authorization is not successful. Exception is thrown: {0}", (object) ex.ToString());
            return new AppSessionTokenResult()
            {
              AppSessionTokenError = AppSessionTokenError.AuthorizationIsNotSuccessfull
            };
          }
          additionalClaims.Add(new Claim(DelegatedAuthorizationTokenClaims.AuthorizationId, authorization.AuthorizationId.ToString()));
        }
        VssSigningCredentials credentials = VssSigningCredentials.Create(Encoding.UTF8.GetBytes(secretInternal.EncodedToken));
        DateTime validTo = DateTime.UtcNow.Add(settings.AppSessionTokenLifetime);
        JsonWebToken jsonWebToken = JsonWebToken.Create(this.issuer, clientId.ToString(), this.GetTokenFromValidDate(), validTo, (IEnumerable<Claim>) additionalClaims, credentials);
        return new AppSessionTokenResult()
        {
          AppSessionToken = jsonWebToken.EncodedToken,
          ExpirationDate = validTo
        };
      }
      finally
      {
        requestContext.TraceLeave(1048029, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (IssueAppSessionTokenInternal));
      }
    }

    public HostAuthorizationDecision AuthorizeHost(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? newId = null)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (AuthorizeHost));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 2);
        if (!newId.HasValue)
          newId = new Guid?(Guid.NewGuid());
        return this.ExecuteServiceMethods<HostAuthorizationDecision>(requestContext, (Func<IVssRequestContext, HostAuthorizationDecision>) (context => this.AuthorizeHostInternal(context, clientId, newId)), (Func<IVssRequestContext, HostAuthorizationDecision>) (context => this.AuthorizeHostRemote(context, clientId, newId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (AuthorizeHost));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (AuthorizeHost));
      }
    }

    private HostAuthorizationDecision AuthorizeHostRemote(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? newId = null)
    {
      try
      {
        this.HasImpersonatePermission(requestContext, 1048083, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "User does not have permission to issue AuthorizeHost.");
      }
      catch (PlatformDelegatedAuthorizationServiceException ex)
      {
        return ex.ToHostAuthorizationDecision();
      }
      try
      {
        HostAuthorizationDecision hostAuthrorizationDecision = requestContext.GetService<ITokenService>().AuthorizeHost(requestContext, clientId, newId);
        if (!hostAuthrorizationDecision.HasError && !this.CreateApplicationPrincipalIdentity(requestContext, hostAuthrorizationDecision))
          hostAuthrorizationDecision.HostAuthorizationError = HostAuthorizationError.FailedToAuthorizeHost;
        return hostAuthrorizationDecision;
      }
      catch (Exception ex)
      {
        HostAuthorizationError result;
        if (System.Enum.TryParse<HostAuthorizationError>(ex.Message, out result))
          return new HostAuthorizationDecision()
          {
            HostAuthorizationError = result
          };
        throw;
      }
    }

    private bool CreateApplicationPrincipalIdentity(
      IVssRequestContext requestContext,
      HostAuthorizationDecision hostAuthrorizationDecision)
    {
      if (hostAuthrorizationDecision.ClientType == ClientType.Application)
      {
        if (hostAuthrorizationDecision != null)
        {
          Guid registrationId = hostAuthrorizationDecision.RegistrationId;
          IPlatformIdentityServiceInternal service = requestContext.GetService<IPlatformIdentityServiceInternal>();
          if (service == null)
          {
            requestContext.Trace(1048040, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Unable to retrieve the implementation of IPlatformIdentityServiceInternal on SPS side. This should never be the case.");
            return false;
          }
          if (!(service is IdentityService dentityService))
          {
            requestContext.Trace(1048040, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Unable to retrieve the implementation of IdentityService on SPS side. This should never be the case.");
            return false;
          }
          requestContext.TraceAlways(1048086, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Creating Application Service Principal for {0}", (object) hostAuthrorizationDecision.RegistrationId));
          Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = service.CreateFrameworkIdentity(requestContext, FrameworkIdentityType.ServiceIdentity, "Application", hostAuthrorizationDecision.RegistrationId.ToString(), hostAuthrorizationDecision.RegistrationName, hostAuthrorizationDecision.RegistrationId);
          Microsoft.VisualStudio.Services.Identity.Identity identity = dentityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            GroupWellKnownIdentityDescriptors.ApplicationPrincipalsGroup
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity != null)
          {
            requestContext.TraceAlways(1048086, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Adding Application Service Principal {0} to Application Principal Group", (object) hostAuthrorizationDecision.RegistrationId));
            if (!dentityService.AddMemberToGroup(requestContext, identity.Descriptor, frameworkIdentity))
            {
              requestContext.Trace(1048037, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Unable to add the Application principal : {0} to the Application Principal Group: {1}.", (object) frameworkIdentity.Descriptor, (object) identity.Descriptor));
              return false;
            }
          }
          else
          {
            requestContext.Trace(1048038, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Unable to read the Application Principal Group : {0}.", (object) GroupWellKnownIdentityDescriptors.ApplicationPrincipalsGroup));
            return false;
          }
        }
        else
        {
          requestContext.Trace(1048039, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Registration Id is null, this likely means that, AuthorizeHostInternal method was called.");
          return false;
        }
      }
      return true;
    }

    private HostAuthorizationDecision AuthorizeHostInternal(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? newId = null)
    {
      requestContext.TraceEnter(1048080, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (AuthorizeHostInternal));
      try
      {
        if (clientId == Guid.Empty)
          return new HostAuthorizationDecision()
          {
            HostAuthorizationError = HostAuthorizationError.ClientIdRequired
          };
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        try
        {
          this.HasImpersonatePermission(requestContext, 1048083, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "User does not have permission to issue AuthorizeHost.");
        }
        catch (PlatformDelegatedAuthorizationServiceException ex)
        {
          return ex.ToHostAuthorizationDecision();
        }
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        HostAuthorization hostAuthorization = (HostAuthorization) null;
        try
        {
          using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
            hostAuthorization = component.UpdateHostAuthorization(clientId, instanceId, newId);
        }
        catch (RegistrationNotFoundException ex)
        {
          requestContext.Trace(1048084, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client registration not found.");
          return new HostAuthorizationDecision()
          {
            HostAuthorizationError = HostAuthorizationError.ClientIdNotFound
          };
        }
        catch (InvalidRegistrationException ex)
        {
          requestContext.Trace(1048084, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client registration is invalid.");
          return new HostAuthorizationDecision()
          {
            HostAuthorizationError = HostAuthorizationError.InvalidClientId
          };
        }
        if (hostAuthorization == null)
        {
          requestContext.Trace(1048084, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "AuthorizationHost resulted in null.");
          return new HostAuthorizationDecision()
          {
            HostAuthorizationError = HostAuthorizationError.FailedToAuthorizeHost
          };
        }
        return new HostAuthorizationDecision()
        {
          HostAuthorizationId = hostAuthorization.Id
        };
      }
      finally
      {
        requestContext.TraceLeave(1048085, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (AuthorizeHostInternal));
      }
    }

    public void RevokeHostAuthorization(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? hostId = null)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeHostAuthorization));
      if (!hostId.HasValue || hostId.Value == Guid.Empty)
        hostId = new Guid?(requestContext.ServiceHost.InstanceId);
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        this.ExecuteServiceMethods<bool>(requestContext, (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeHostAuthorizationInternal(context, clientId, hostId);
          return true;
        }), (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeHostAuthorizationRemote(context, clientId, hostId);
          return true;
        }), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeHostAuthorization));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeHostAuthorization));
      }
    }

    private void RevokeHostAuthorizationRemote(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? hostId = null)
    {
      string revokeHost = DelegatedAuthorizationResources.NoPermissionToRevokeHost();
      try
      {
        this.HasImpersonatePermission(requestContext, 1048091, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), revokeHost);
      }
      catch (PlatformDelegatedAuthorizationServiceException ex)
      {
        string message = revokeHost;
        ex.ToAccessCheckException(message);
      }
      requestContext.GetService<ITokenService>().RevokeHostAuthorization(requestContext, clientId, hostId);
      this.RemoveApplicationPrincipalIdentity(requestContext, clientId, hostId ?? requestContext.ServiceHost.InstanceId);
    }

    private void RemoveApplicationPrincipalIdentity(
      IVssRequestContext requestContext,
      Guid registrationId,
      Guid hostId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IdentityDescriptor identityDescriptor = IdentityHelper.CreateApplicationPrincipalIdentityDescriptor(requestContext, registrationId);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.ApplicationPrincipalsGroup
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity1 != null && identity2 != null)
      {
        if (service.RemoveMemberFromGroup(requestContext, identity2.Descriptor, identity1.Descriptor))
          return;
        requestContext.Trace(1048037, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Removing the application principal Identity : {0} from the Application Principal Group: {1} failed.", (object) identity1.Descriptor, (object) identity2.Descriptor));
      }
      else
        requestContext.Trace(1048038, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), string.Format("Unable to read the Application Principal Group : {0} or Application Principal : {1}.", (object) GroupWellKnownIdentityDescriptors.ApplicationPrincipalsGroup, (object) identityDescriptor));
    }

    private void RevokeHostAuthorizationInternal(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid? hostId = null)
    {
      requestContext.TraceEnter(1048090, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeHostAuthorizationInternal));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(clientId, nameof (clientId));
        if (!hostId.HasValue || hostId.Value == Guid.Empty)
          hostId = new Guid?(requestContext.ServiceHost.InstanceId);
        string revokeHost = DelegatedAuthorizationResources.NoPermissionToRevokeHost();
        try
        {
          this.HasImpersonatePermission(requestContext, 1048091, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), revokeHost);
        }
        catch (PlatformDelegatedAuthorizationServiceException ex)
        {
          string message = revokeHost;
          ex.ToAccessCheckException(message);
        }
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        IList<string> taskArgs;
        try
        {
          using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
            taskArgs = component.RevokeHostAuthorization(clientId, hostId.Value);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1048092, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
          throw;
        }
        if (taskArgs == null)
          return;
        requestContext.Trace(1048094, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Sending service bus notification to invalidate cached compact session tokens.");
        TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishServiceBusNotification), (object) taskArgs, 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      finally
      {
        requestContext.TraceLeave(1048095, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeHostAuthorizationInternal));
      }
    }

    public void RemovePublicKey(IVssRequestContext requestContext, string publicKey)
    {
      requestContext.TraceEnter(1048220, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RemovePublicKey));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        this.ExecuteServiceMethods<bool>(requestContext, (Func<IVssRequestContext, bool>) (context =>
        {
          this.RemovePublicKeyInternal(context, publicKey);
          return true;
        }), (Func<IVssRequestContext, bool>) (context =>
        {
          this.RemovePublicKeyRemote(context, publicKey);
          return true;
        }), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RemovePublicKey));
      }
      finally
      {
        requestContext.TraceLeave(1048229, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RemovePublicKey));
      }
    }

    private void RemovePublicKeyRemote(IVssRequestContext requestContext, string publicKey)
    {
      string removeSshKey = DelegatedAuthorizationResources.NoPermissionToRemoveSshKey();
      try
      {
        this.HasImpersonatePermission(requestContext, 1048104, removeSshKey);
      }
      catch (PlatformDelegatedAuthorizationServiceException ex)
      {
        string message = removeSshKey;
        ex.ToAccessCheckException(message);
      }
      requestContext.GetService<ITokenService>().RemovePublicKey(requestContext, publicKey);
    }

    private void RemovePublicKeyInternal(IVssRequestContext requestContext, string publicKey)
    {
      requestContext.TraceEnter(1048220, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RemovePublicKeyInternal));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(publicKey, nameof (publicKey));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        {
          string removeSshKey = DelegatedAuthorizationResources.NoPermissionToRemoveSshKey();
          requestContext.Trace(1048104, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), removeSshKey);
          throw new AccessCheckException(removeSshKey);
        }
        using (DelegatedAuthorizationComponent component = requestContext.CreateComponent<DelegatedAuthorizationComponent>())
        {
          requestContext.Trace(1048101, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Getting authorizationIds for public data: {0}", (object) publicKey);
          IList<Guid> authorizationIdsByPublicData = component.GetAuthorizationIdsByPublicData(publicKey);
          if (authorizationIdsByPublicData == null || authorizationIdsByPublicData.Count == 0)
            throw new AuthorizationIdNotFoundException("Authorization Id not found for the provided public key data");
          foreach (Guid authorizationId in (IEnumerable<Guid>) authorizationIdsByPublicData)
            component.RemoveAccessTokenKey(authorizationId);
        }
      }
      finally
      {
        requestContext.TraceLeave(1048229, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RemovePublicKeyInternal));
      }
    }

    protected internal void UpdateAudienceForSSHKey(
      IVssRequestContext requestContext,
      Guid authorizationId,
      string audience,
      string accessHash)
    {
      requestContext.TraceEnter(1048230, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateAudienceForSSHKey));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(audience, nameof (audience));
        ArgumentUtility.CheckStringForNullOrEmpty(accessHash, nameof (accessHash));
        using (DelegatedAuthorizationComponent component = requestContext.CreateComponent<DelegatedAuthorizationComponent>())
        {
          requestContext.Trace(1048231, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Updating audience for key with authorizationId: {0}", (object) authorizationId);
          component.UpdateAudienceAndAccessHashForSSHKey(authorizationId, audience, accessHash);
        }
      }
      finally
      {
        requestContext.TraceLeave(1048232, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (UpdateAudienceForSSHKey));
      }
    }

    public void RevokeSessionToken(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048100, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeSessionToken));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        this.ExecuteServiceMethods<bool>(requestContext, (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeSessionTokenInternal(context, authorizationId, isPublic);
          return true;
        }), (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeSessionTokenRemote(context, authorizationId, isPublic);
          return true;
        }), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeSessionToken));
      }
      finally
      {
        requestContext.TraceLeave(1048109, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeSessionToken));
      }
    }

    private void RevokeSessionTokenRemote(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      requestContext.GetService<ITokenService>().RevokeSessionToken(requestContext, authorizationId, isPublic);
    }

    private void RevokeSessionTokenInternal(
      IVssRequestContext requestContext,
      Guid authorizationId,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048100, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeSessionTokenInternal));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(authorizationId, nameof (authorizationId));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        AccessTokenKey accessTokenKey;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
        {
          requestContext.Trace(1048101, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Checking to see if a session token exists with given authorization id: {0}", (object) authorizationId);
          accessTokenKey = component.GetAccessTokenKey(authorizationId, isPublic);
        }
        if (accessTokenKey == null || !accessTokenKey.IsValid)
        {
          string message = DelegatedAuthorizationResources.NoValidSessionTokenFound((object) authorizationId);
          requestContext.Trace(1048102, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), message);
          throw new SessionTokenNotFoundException(message);
        }
        Guid guid = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
        if (accessTokenKey.UserId != guid)
        {
          requestContext.Trace(1048103, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Session token associated with authorization id {0} is not owned by the current user {1}.", (object) authorizationId, (object) requestContext.GetUserId());
          if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
          {
            string revokeSessionToken = DelegatedAuthorizationResources.NoPermissionToRevokeSessionToken((object) guid, (object) authorizationId);
            requestContext.Trace(1048104, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), revokeSessionToken);
            throw new AccessCheckException(revokeSessionToken);
          }
        }
        string str;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          str = isPublic ? component.RemoveAccessTokenKey(authorizationId) : component.RevokeAccessTokenKey(authorizationId);
        if (string.IsNullOrWhiteSpace(str))
          return;
        requestContext.Trace(1048105, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Sending service bus notification to invalid session token cache for {0}", (object) str);
        TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishServiceBusNotification), (object) new List<string>()
        {
          str
        }, 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      finally
      {
        requestContext.TraceLeave(1048109, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeSessionTokenInternal));
      }
    }

    public void RevokeAllSessionTokensOfUser(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1048100, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUser));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        this.ExecuteServiceMethods<bool>(requestContext, (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeAllSessionTokensOfUserInternal(context);
          return true;
        }), (Func<IVssRequestContext, bool>) (context =>
        {
          this.RevokeAllSessionTokensOfUserRemote(context);
          return true;
        }), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUser));
      }
      finally
      {
        requestContext.TraceLeave(1048109, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUser));
      }
    }

    private void RevokeAllSessionTokensOfUserRemote(IVssRequestContext requestContext) => requestContext.GetService<ITokenService>().RevokeAllSessionTokensOfUser(requestContext);

    private void RevokeAllSessionTokensOfUserInternal(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1048100, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUserInternal));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        List<string> stringList;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          stringList = component.RevokeAllAccessTokenKeysForUser(userIdentity.StorageKey(vssRequestContext));
        if (stringList == null || !stringList.Any<string>((Func<string, bool>) (sessionTokenHash => !string.IsNullOrWhiteSpace(sessionTokenHash))))
          return;
        requestContext.Trace(1048105, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Sending service bus notification to invalid session token cache for {0}", (object) stringList);
        TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishServiceBusNotification), (object) stringList, 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      finally
      {
        requestContext.TraceLeave(1048109, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (RevokeAllSessionTokensOfUserInternal));
      }
    }

    public AccessTokenResult ExchangeAppToken(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      JsonWebToken clientSecret,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeAppToken));
      ArgumentUtility.CheckForNull<JsonWebToken>(appToken, nameof (appToken));
      ArgumentUtility.CheckForNull<JsonWebToken>(clientSecret, nameof (clientSecret));
      ClaimsPrincipal appTokenClaims = this.ValidateToken(appToken, clientSecret.EncodedToken);
      Guid appTokenClientId = Guid.Empty;
      int num = (int) ExchangeAppTokenValidator.ValidateAppTokenClaims(requestContext.To(TeamFoundationHostType.Deployment), appToken, appTokenClaims, out appTokenClientId, out Guid _, out Guid _);
      try
      {
        if (!accessId.HasValue)
          accessId = new Guid?(Guid.NewGuid());
        return this.ExecuteServiceMethods<AccessTokenResult>(requestContext, (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeAppTokenInternal(context, appToken, clientSecret, accessId)), (Func<IVssRequestContext, AccessTokenResult>) (context => this.ExchangeAppTokenRemote(context, appToken, clientSecret, accessId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeAppToken));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeAppToken));
      }
    }

    private AccessTokenResult ExchangeAppTokenRemote(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      JsonWebToken clientSecret,
      Guid? accessId = null)
    {
      try
      {
        return requestContext.GetService<ITokenService>().ExchangeAppToken(requestContext, appToken, clientSecret, accessId);
      }
      catch (Exception ex)
      {
        TokenError result;
        if (System.Enum.TryParse<TokenError>(ex.Message, out result))
          return new AccessTokenResult()
          {
            AccessTokenError = result
          };
        throw;
      }
    }

    private AccessTokenResult ExchangeAppTokenInternal(
      IVssRequestContext requestContext,
      JsonWebToken appToken,
      JsonWebToken clientSecret,
      Guid? accessId = null)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeAppTokenInternal));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<JsonWebToken>(appToken, nameof (appToken));
        ArgumentUtility.CheckForNull<JsonWebToken>(clientSecret, nameof (clientSecret));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Guid appTokenClientId = Guid.Empty;
        Guid appTokenAuthorizationId = Guid.Empty;
        Guid appTokenIdentityId = Guid.Empty;
        Registration registration = (Registration) null;
        Authorization authorization = (Authorization) null;
        Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        DelegatedAuthorizationCertificateManager certificateManager = new DelegatedAuthorizationCertificateManager();
        DelegatedAuthorizationSettings settings1 = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(vssRequestContext);
        IVssRequestContext requestContext1 = vssRequestContext;
        DelegatedAuthorizationSettings settings2 = settings1;
        X509Certificate2 certificate = certificateManager.GetCertificate(requestContext1, settings2);
        ClaimsPrincipal clientSecretClaims = this.ValidateToken(clientSecret, certificate);
        ClaimsPrincipal appTokenClaims = this.ValidateToken(appToken, clientSecret.EncodedToken);
        TokenError tokenError1 = ExchangeAppTokenValidator.ValidateAppTokenClaims(vssRequestContext, appToken, appTokenClaims, out appTokenClientId, out appTokenAuthorizationId, out appTokenIdentityId);
        if (tokenError1 != TokenError.None)
        {
          requestContext.Trace(1050111, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "App token validation fail.");
          return new AccessTokenResult()
          {
            AccessTokenError = tokenError1
          };
        }
        TokenError tokenError2 = ExchangeAppTokenValidator.ValidateClientSecretClaims(vssRequestContext, clientSecret, clientSecretClaims, appTokenClientId, out registration);
        if (tokenError2 != TokenError.None)
        {
          requestContext.Trace(1050112, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "client secret validation fail.");
          return new AccessTokenResult()
          {
            AccessTokenError = tokenError2
          };
        }
        TokenError tokenError3 = ExchangeAppTokenValidator.ValidateAppTokenAuthorization(vssRequestContext, appTokenClientId, appTokenAuthorizationId, appTokenIdentityId, out authorization);
        if (tokenError3 != TokenError.None)
        {
          requestContext.Trace(1050113, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "App token Authorization validation fail.");
          return new AccessTokenResult()
          {
            AccessTokenError = tokenError3
          };
        }
        TokenError tokenError4 = ExchangeAppTokenValidator.ValidateAppTokenIdentity(vssRequestContext, appTokenIdentityId, out targetIdentity);
        if (tokenError4 != TokenError.None)
        {
          requestContext.Trace(1050114, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "App token identity validation fail.");
          return new AccessTokenResult()
          {
            AccessTokenError = tokenError4
          };
        }
        AccessTokenResult accessTokenResult = new AccessTokenResult();
        try
        {
          AccessTokenData accessTokenRawData = (AccessTokenData) null;
          using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
            accessTokenRawData = component.CreateAccessToken(authorization.AuthorizationId, registration.SecretVersionId, registration.RedirectUris.FirstOrDefault<Uri>(), DateTime.UtcNow, DateTime.UtcNow.Add(settings1.AccessTokenLifetime), accessId);
          this.GenerateTokens(vssRequestContext, accessTokenResult, accessTokenRawData, certificate, targetIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment), authorization.Scopes);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1050118, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
          throw;
        }
        return accessTokenResult;
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (ExchangeAppTokenInternal));
      }
    }

    public IList<HostAuthorization> GetHostAuthorizations(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetHostAuthorizations));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return this.ExecuteListServiceMethods<HostAuthorization>(requestContext, (Func<IVssRequestContext, IEnumerable<HostAuthorization>>) (context => (IEnumerable<HostAuthorization>) this.GetHostAuthorizationsInternal(context, hostId)), (Func<IVssRequestContext, IEnumerable<HostAuthorization>>) (context => (IEnumerable<HostAuthorization>) this.GetHostAuthorizationsRemote(context, hostId)), "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetHostAuthorizations));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetHostAuthorizations));
      }
    }

    private string GetDictionaryKey(HostAuthorization hostAuthorization) => hostAuthorization.Id.ToString();

    private IDictionary<string, HostAuthorization> ToDictionary(IEnumerable<HostAuthorization> list) => (IDictionary<string, HostAuthorization>) list.ToDictionary<HostAuthorization, string, HostAuthorization>((Func<HostAuthorization, string>) (a => this.GetDictionaryKey(a)), (Func<HostAuthorization, HostAuthorization>) (a => a));

    private IList<HostAuthorization> GetHostAuthorizationsRemote(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      Guid requestIdentityStorageKey = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
      if (!this.IsUserAuthorized(requestContext, requestIdentityStorageKey, Guid.Empty))
      {
        requestContext.Trace(1048710, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity {0} is not authorized to GetHostAuthorizations.", (object) requestIdentityStorageKey);
        throw new PlatformDelegatedAuthorizationException("AccessDenied");
      }
      return requestContext.GetService<ITokenService>().GetHostAuthorizations(requestContext, hostId);
    }

    private IList<HostAuthorization> GetHostAuthorizationsInternal(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      requestContext.TraceEnter(1048702, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetHostAuthorizationsInternal));
      try
      {
        Guid requestIdentityStorageKey = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
        if (!this.IsUserAuthorized(requestContext, requestIdentityStorageKey, Guid.Empty))
        {
          requestContext.Trace(1048710, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Request identity {0} is not authorized to GetHostAuthorizations.", (object) requestIdentityStorageKey);
          throw new PlatformDelegatedAuthorizationException("AccessDenied");
        }
        using (DelegatedAuthorizationComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<DelegatedAuthorizationComponent>())
          return component.GetHostAuthorizations(hostId);
      }
      finally
      {
        requestContext.TraceLeave(1048703, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (GetHostAuthorizationsInternal));
      }
    }

    private void PublishServiceBusNotification(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(1048096, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (PublishServiceBusNotification));
      if (taskArgs is IList<string> stringList)
      {
        if (stringList.Any<string>())
        {
          try
          {
            requestContext.Trace(1048097, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Invalid compact session token count {0}", (object) stringList.Count);
            IMessageBusPublisherService service = requestContext.GetService<IMessageBusPublisherService>();
            foreach (DelegatedAuthorizationMessage changeMessage in this.GetChangeMessages(requestContext, stringList))
              service.Publish(requestContext, "Microsoft.VisualStudio.Services.DelegatedAuthorization", (object[]) new DelegatedAuthorizationMessage[1]
              {
                changeMessage
              }, false);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1048098, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), ex);
          }
        }
      }
      requestContext.TraceLeave(1048099, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), nameof (PublishServiceBusNotification));
    }

    private byte[] GetFrameworkAccessTokenSecret(IVssRequestContext deploymentRequestContext)
    {
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId;
      try
      {
        drawerId = service.UnlockDrawer(deploymentRequestContext.Elevate(), "DelegatedAuthorizationSecrets", false);
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        deploymentRequestContext.Trace(1048112, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Strongbox drawer not found.");
        throw;
      }
      if (drawerId == Guid.Empty)
      {
        deploymentRequestContext.Trace(1048113, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), DelegatedAuthorizationResources.DrawerEmptyGuid());
        throw new StrongBoxDrawerNotFoundException(DelegatedAuthorizationResources.DrawerEmptyGuid());
      }
      try
      {
        string s = service.GetString(deploymentRequestContext.Elevate(), drawerId, "FrameworkAccessTokenKeySecret");
        return !string.IsNullOrEmpty(s) ? Convert.FromBase64String(s) : throw new StrongBoxItemNotFoundException("FrameworkAccessTokenKeySecret");
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        deploymentRequestContext.Trace(1048114, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Access token key secrets are not found in strong box.");
        throw;
      }
    }

    private AccessTokenResult GenerateClientCredentialsAccessToken(
      IVssRequestContext requestContext,
      JsonWebToken clientAssertion,
      Guid accessId)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(clientAssertion, nameof (clientAssertion));
      Guid result = Guid.Empty;
      string subject = clientAssertion.Subject;
      if (string.IsNullOrEmpty(subject) || !Guid.TryParse(subject, out result))
      {
        requestContext.Trace(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Subject claim is not present or not a valid client identifier: {0}", (object) subject);
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidClientId
        };
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Registration registration;
      using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
        registration = component.GetRegistration(Guid.Empty, result);
      if (registration == null || registration.ClientType != ClientType.MediumTrust)
      {
        requestContext.Trace(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Registration was not found or is not medium trust. ClientType: {0}", (object) registration?.ClientType);
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidClientId
        };
      }
      HostAuthorization hostAuthorization;
      using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
      {
        hostAuthorization = component.GetHostAuthorization(registration.RegistrationId, requestContext.ServiceHost.InstanceId);
        if (hostAuthorization == null)
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
            hostAuthorization = component.GetHostAuthorization(registration.RegistrationId, requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId);
        }
      }
      if (hostAuthorization == null || !hostAuthorization.IsValid)
      {
        requestContext.Trace(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Registration {0} is not authorized for host {1}", (object) registration.RegistrationId, (object) requestContext.ServiceHost.InstanceId);
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidClientId
        };
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<Guid>) new Guid[1]
      {
        registration.IdentityId
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext.Elevate(), (IList<Guid>) new Guid[1]
        {
          registration.IdentityId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null || !string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.ServiceIdentity"))
      {
        requestContext.Trace(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Registration identity {0} ({1}) was not found or is not a framework service identity", (object) registration.IdentityId, (object) identity?.Descriptor);
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidClientId
        };
      }
      IDelegatedAuthorizationConfigurationService service = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
      DelegatedAuthorizationSettings settings = service.GetSettings(vssRequestContext);
      TokenValidationResult validationResult = this.IsValidClientCredentialsToken(requestContext, registration, settings, clientAssertion);
      if (validationResult.HasError)
      {
        requestContext.Trace(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Client credentials assertion for registration {0} was not valid", (object) registration.RegistrationId);
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidClientId,
          ErrorDescription = validationResult.ErrorDescription
        };
      }
      DateTime tokenFromValidDate = this.GetTokenFromValidDate();
      DateTime validTo = tokenFromValidDate.Add(settings.AccessTokenMaxLifetimeForMediumTrustClients);
      VssSigningCredentials signingCredentials = service.GetSigningCredentials(vssRequestContext, true);
      JsonWebToken accessToken = this.GenerateAccessToken(requestContext, identity.StorageKey(requestContext, TeamFoundationHostType.Deployment), Guid.NewGuid(), registration.Scopes, tokenFromValidDate, validTo, signingCredentials, string.Format("{0}{1}", (object) "vso:", (object) hostAuthorization.HostId), accessId: new Guid?(accessId));
      return new AccessTokenResult()
      {
        AccessToken = accessToken,
        ValidTo = validTo
      };
    }

    private AccessTokenResult GenerateImplicitAccessToken(
      IVssRequestContext requestContext,
      JsonWebToken grant)
    {
      DelegatedAuthorizationCertificateManager certificateManager = new DelegatedAuthorizationCertificateManager();
      DelegatedAuthorizationSettings settings1 = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(requestContext);
      IVssRequestContext requestContext1 = requestContext;
      DelegatedAuthorizationSettings settings2 = settings1;
      VssSigningCredentials credentials = VssSigningCredentials.Create(certificateManager.GetCertificate(requestContext1, settings2));
      IVssRequestContext context = requestContext.Elevate();
      IdentityService service = context.GetService<IdentityService>();
      Guid guid = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext2 = context;
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(requestContext2, (IList<Guid>) new List<Guid>()
      {
        guid
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidAuthorizationGrant
        };
      if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity))
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidAuthorizationGrant
        };
      List<Claim> additionalClaims = new List<Claim>()
      {
        new Claim("nameid", guid.ToString())
      };
      if (grant.NameIdentifier == "IdToken")
        additionalClaims.Add(new Claim("scp", "user_impersonation"));
      else if (grant.NameIdentifier == "SignoutToken")
        additionalClaims.Add(new Claim("scp", "signout"));
      else
        return new AccessTokenResult()
        {
          AccessTokenError = TokenError.InvalidAuthorizationGrant
        };
      DateTime tokenFromValidDate = this.GetTokenFromValidDate();
      DateTime validTo = tokenFromValidDate.Add(settings1.AuthorizationGrantLifetime);
      return new AccessTokenResult()
      {
        AccessToken = JsonWebToken.Create(this.issuer, this.issuer, tokenFromValidDate, validTo, (IEnumerable<Claim>) additionalClaims, credentials, true),
        TokenType = "jwt-bearer",
        ValidTo = validTo
      };
    }

    internal void GenerateTokens(
      IVssRequestContext requestContext,
      AccessTokenResult accessTokenResult,
      AccessTokenData accessTokenRawData,
      X509Certificate2 certificate,
      Guid resourceOwnerIdentityStorageKey,
      string scopes)
    {
      VssSigningCredentials signingCredentials1 = VssSigningCredentials.Create(certificate);
      Guid? nullable1 = requestContext.IsFeatureEnabled("VisualStudio.Services.OAuthWhitelist.AllowOAuthWhitelist") ? new Guid?(accessTokenRawData.Authorization.RegistrationId) : new Guid?();
      AccessTokenResult accessTokenResult1 = accessTokenResult;
      IVssRequestContext requestContext1 = requestContext;
      Guid resourceOwnerIdentity = resourceOwnerIdentityStorageKey;
      Guid authorizationId = accessTokenRawData.AccessToken.AuthorizationId;
      string scopes1 = scopes;
      DateTime utcDateTime1 = accessTokenRawData.AccessToken.ValidFrom.UtcDateTime;
      DateTimeOffset validTo = accessTokenRawData.AccessToken.ValidTo;
      DateTime utcDateTime2 = validTo.UtcDateTime;
      VssSigningCredentials signingCredentials2 = signingCredentials1;
      string audience = accessTokenRawData.Authorization.Audience;
      Guid? nullable2 = nullable1;
      Guid? accessId = new Guid?();
      Guid? hostAuthorizationId = new Guid?();
      Guid? appId = nullable2;
      JsonWebToken accessToken = this.GenerateAccessToken(requestContext1, resourceOwnerIdentity, authorizationId, scopes1, utcDateTime1, utcDateTime2, signingCredentials2, audience, accessId: accessId, hostAuthorizationId: hostAuthorizationId, appId: appId);
      accessTokenResult1.AccessToken = accessToken;
      accessTokenResult.TokenType = "jwt-bearer";
      AccessTokenResult accessTokenResult2 = accessTokenResult;
      validTo = accessTokenRawData.AccessToken.ValidTo;
      DateTime utcDateTime3 = validTo.UtcDateTime;
      accessTokenResult2.ValidTo = utcDateTime3;
      accessTokenResult.AuthorizationId = accessTokenRawData.Authorization.AuthorizationId;
      JsonWebToken refreshToken = this.GenerateRefreshToken(requestContext, accessTokenRawData, resourceOwnerIdentityStorageKey, scopes, signingCredentials1);
      accessTokenResult.RefreshToken = new RefreshTokenGrant(refreshToken);
    }

    private JsonWebToken GenerateAccessToken(
      IVssRequestContext requestContext,
      Guid resourceOwnerIdentity,
      Guid authorizationId,
      string scopes,
      DateTime validFrom,
      DateTime validTo,
      VssSigningCredentials signingCredentials,
      string audience = null,
      bool includeAuthorizationId = true,
      Guid? accessId = null,
      string source = null,
      Guid? hostAuthorizationId = null,
      Guid? appId = null,
      IDictionary<string, string> customClaims = null)
    {
      List<Claim> claimList = new List<Claim>()
      {
        new Claim("nameid", resourceOwnerIdentity.ToString()),
        new Claim("scp", scopes)
      };
      if (includeAuthorizationId)
        claimList.Add(new Claim(DelegatedAuthorizationTokenClaims.AuthorizationId, authorizationId.ToString()));
      Guid? nullable;
      if (hostAuthorizationId.HasValue)
      {
        nullable = hostAuthorizationId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          claimList.Add(new Claim(DelegatedAuthorizationTokenClaims.HostAuthorizationId, hostAuthorizationId.ToString()));
      }
      if (accessId.HasValue)
      {
        nullable = accessId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          claimList.Add(new Claim(DelegatedAuthorizationTokenClaims.AccessId, accessId.ToString()));
      }
      if (appId.HasValue)
      {
        nullable = appId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
        {
          claimList.Add(new Claim("appid", appId.ToString()));
          requestContext.TraceAlways(1050830, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Generate access token: NameId={0},AppId={1}", (object) resourceOwnerIdentity.ToString(), (object) appId.ToString());
        }
      }
      if (!string.IsNullOrEmpty(source))
        claimList.Add(new Claim("src", source));
      if (customClaims != null)
      {
        foreach (KeyValuePair<string, string> customClaim in (IEnumerable<KeyValuePair<string, string>>) customClaims)
        {
          KeyValuePair<string, string> cclaim = customClaim;
          if (!claimList.Any<Claim>((Func<Claim, bool>) (claim => string.Equals(claim.Type, cclaim.Key, StringComparison.OrdinalIgnoreCase))))
            claimList.Add(new Claim(cclaim.Key, cclaim.Value));
        }
      }
      audience = string.IsNullOrEmpty(audience) ? this.issuer : this.issuer + "|" + audience;
      return JsonWebToken.Create(this.issuer, audience, validFrom, validTo, (IEnumerable<Claim>) claimList, signingCredentials, true);
    }

    private JsonWebToken GenerateRefreshToken(
      IVssRequestContext requestContext,
      AccessTokenData accessTokenRawData,
      Guid resourceOwnerIdentityStorageKey,
      string scopes,
      VssSigningCredentials signingCredentials,
      DateTime? refreshValidTo = null)
    {
      Claim[] additionalClaims = new Claim[3]
      {
        new Claim("nameid", resourceOwnerIdentityStorageKey.ToString()),
        new Claim(DelegatedAuthorizationTokenClaims.AccessTokenId, accessTokenRawData.AccessToken.AccessId.ToString()),
        new Claim("scp", scopes)
      };
      DelegatedAuthorizationSettings settings = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(requestContext);
      if (!refreshValidTo.HasValue)
        refreshValidTo = new DateTime?(accessTokenRawData.AccessToken.ValidFrom.UtcDateTime.Add(settings.RefreshTokenLifetime));
      return JsonWebToken.Create(this.issuer, this.issuer, accessTokenRawData.AccessToken.ValidFrom.UtcDateTime, refreshValidTo.Value, (IEnumerable<Claim>) additionalClaims, signingCredentials, true);
    }

    internal JsonWebToken GenerateAuthorizationGrant(
      IVssRequestContext requestContext,
      Authorization authorization,
      DelegatedAuthorizationSettings settings)
    {
      VssSigningCredentials credentials = VssSigningCredentials.Create(new DelegatedAuthorizationCertificateManager().GetCertificate(requestContext, settings));
      authorization.Scopes = string.IsNullOrWhiteSpace(authorization.Scopes) ? "vso.authorization_grant" : authorization.Scopes + " vso.authorization_grant";
      Claim[] additionalClaims = new Claim[3]
      {
        new Claim(DelegatedAuthorizationTokenClaims.AuthorizationId, authorization.AuthorizationId.ToString()),
        new Claim("nameid", requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment).ToString()),
        new Claim("scp", authorization.Scopes)
      };
      return JsonWebToken.Create(this.issuer, this.issuer, authorization.ValidFrom.UtcDateTime, authorization.ValidTo.UtcDateTime, (IEnumerable<Claim>) additionalClaims, credentials, true);
    }

    private ClaimsPrincipal ValidateToken(JsonWebToken jwt, X509Certificate2 certificate)
    {
      try
      {
        JsonWebTokenValidationParameters parameters = new JsonWebTokenValidationParameters()
        {
          SigningCredentials = VssSigningCredentials.Create(certificate),
          ValidateActor = false,
          ValidateAudience = false,
          ValidateIssuer = false,
          ValidateExpiration = false,
          ValidateNotBefore = false,
          ValidateSignature = true
        };
        return jwt.ValidateToken(parameters);
      }
      catch (SignatureValidationException ex)
      {
        return (ClaimsPrincipal) null;
      }
    }

    private ClaimsPrincipal ValidateToken(JsonWebToken jwt, string signingKey)
    {
      try
      {
        JsonWebTokenValidationParameters parameters = new JsonWebTokenValidationParameters()
        {
          SigningCredentials = VssSigningCredentials.Create(Encoding.UTF8.GetBytes(signingKey)),
          ValidateActor = false,
          ValidateAudience = false,
          ValidateIssuer = false,
          ValidateExpiration = false,
          ValidateNotBefore = false,
          ValidateSignature = true
        };
        return jwt.ValidateToken(parameters);
      }
      catch (SignatureValidationException ex)
      {
        return (ClaimsPrincipal) null;
      }
    }

    private TokenValidationResult IsValidClientCredentialsToken(
      IVssRequestContext requestContext,
      Registration registration,
      DelegatedAuthorizationSettings settings,
      JsonWebToken jwt)
    {
      TimeSpan timeSpan = jwt.ValidTo - jwt.ValidFrom;
      if (timeSpan > settings.BearerTokenMaxLifetimeForMediumTrustClients)
      {
        requestContext.Trace(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Bearer token lifetime {0} is greater than the allowed maximum lifetime {1}", (object) timeSpan, (object) settings.BearerTokenMaxLifetimeForMediumTrustClients);
        return new TokenValidationResult()
        {
          HasError = true,
          ErrorDescription = string.Format("Bearer token lifetime {0} is greater than the allowed maximum lifetime {1}", (object) timeSpan, (object) settings.BearerTokenMaxLifetimeForMediumTrustClients)
        };
      }
      IVssRequestContext rootContext = requestContext.RootContext;
      Uri resourceUri = rootContext.GetService<ILocationService>().GetResourceUri(rootContext, "oauth2", OAuth2ResourceIds.Token, (object) null);
      JsonWebTokenValidationParameters parameters = new JsonWebTokenValidationParameters()
      {
        AllowedAudiences = (IEnumerable<string>) new string[1]
        {
          resourceUri.AbsoluteUri
        },
        SigningCredentials = VssSigningCredentials.Create((Func<RSACryptoServiceProvider>) (() => registration.GetPublicKey())),
        ValidateAudience = true,
        ValidateExpiration = true,
        ValidateIssuer = true,
        ValidateNotBefore = true,
        ValidateSignature = true,
        ValidIssuers = (IEnumerable<string>) new string[1]
        {
          registration.RegistrationId.ToString("D")
        }
      };
      TokenValidationResult validationResult = new TokenValidationResult();
      try
      {
        jwt.ValidateToken(parameters);
      }
      catch (TokenExpiredException ex)
      {
        validationResult.HasError = true;
        validationResult.ErrorDescription = string.Format("The bearer token expired on {0}. Current server time is {1}.", (object) jwt.ValidTo, (object) DateTime.UtcNow);
        requestContext.TraceException(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), (Exception) ex);
      }
      catch (TokenNotYetValidException ex)
      {
        validationResult.HasError = true;
        validationResult.ErrorDescription = string.Format("The bearer token is not valid until {0}. Current server time is {1}.", (object) jwt.ValidFrom, (object) DateTime.UtcNow);
        requestContext.TraceException(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), (Exception) ex);
      }
      catch (JsonWebTokenValidationException ex)
      {
        validationResult.HasError = true;
        validationResult.ErrorDescription = ex.Message;
        requestContext.TraceException(1050816, TraceLevel.Error, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), (Exception) ex);
      }
      return validationResult;
    }

    public JsonWebToken GenerateImplicitGrantTokenForCurrentUser(
      IVssRequestContext requestContext,
      ResponseType responseType)
    {
      string accessPoint = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext).AccessPoint;
      DelegatedAuthorizationCertificateManager certificateManager = new DelegatedAuthorizationCertificateManager();
      DelegatedAuthorizationSettings settings1 = requestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSettings(requestContext);
      IVssRequestContext requestContext1 = requestContext;
      DelegatedAuthorizationSettings settings2 = settings1;
      VssSigningCredentials credentials = VssSigningCredentials.Create(certificateManager.GetCertificate(requestContext1, settings2));
      Claim[] additionalClaims = new Claim[1]
      {
        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", responseType.ToString())
      };
      return JsonWebToken.Create(accessPoint, accessPoint, DateTime.UtcNow, DateTime.UtcNow, (IEnumerable<Claim>) additionalClaims, credentials, true);
    }

    private IList<Guid> GetTargetAccountsList(string audience)
    {
      IList<Guid> targetAccountsList = (IList<Guid>) null;
      if (!string.IsNullOrEmpty(audience))
      {
        string[] strArray = audience.Split('|');
        targetAccountsList = (IList<Guid>) new List<Guid>();
        for (int index = 0; index < strArray.Length; ++index)
        {
          string str = strArray[index];
          if (str.Length > "vso:".Length)
          {
            Guid result;
            Guid.TryParse(str.Remove(0, "vso:".Length), out result);
            if (result != Guid.Empty)
              targetAccountsList.Add(result);
          }
        }
      }
      return targetAccountsList;
    }

    private IEnumerable<DelegatedAuthorizationMessage> GetChangeMessages(
      IVssRequestContext requestContext,
      IList<string> invalidatedCompactSessionTokens)
    {
      int num = 1000;
      int batchSize = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/ChangePublisherMessageBufferSize", 3000);
      invalidatedCompactSessionTokens = (IList<string>) invalidatedCompactSessionTokens.Where<string>((Func<string, bool>) (t => !string.IsNullOrWhiteSpace(t))).ToList<string>();
      int count = invalidatedCompactSessionTokens.Count;
      if (batchSize < num || count <= batchSize)
      {
        yield return new DelegatedAuthorizationMessage()
        {
          CompactSessionTokenChanges = invalidatedCompactSessionTokens.Select<string, CompactSessionTokenChange>((Func<string, CompactSessionTokenChange>) (t => new CompactSessionTokenChange()
          {
            TokenKey = t
          })).ToList<CompactSessionTokenChange>()
        };
      }
      else
      {
        foreach (IList<string> source in invalidatedCompactSessionTokens.Batch<string>(batchSize))
          yield return new DelegatedAuthorizationMessage()
          {
            CompactSessionTokenChanges = source.Select<string, CompactSessionTokenChange>((Func<string, CompactSessionTokenChange>) (token => new CompactSessionTokenChange()
            {
              TokenKey = token
            })).ToList<CompactSessionTokenChange>()
          };
      }
    }

    internal SessionTokenError ValidateSource(IVssRequestContext requestContext, string source)
    {
      SessionTokenError sessionTokenError = SessionTokenError.None;
      if (!string.IsNullOrEmpty(source))
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.AllowSource"))
        {
          requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Source value is not supported.");
          return SessionTokenError.SourceNotSupported;
        }
        if (source.Length > 500)
        {
          requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Source value is greater than expected - {0}.", (object) source);
          return SessionTokenError.InvalidSource;
        }
        string str1 = source;
        string[] separator = new string[1]{ "|" };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          if (!str2.StartsWith("ipa:", StringComparison.OrdinalIgnoreCase) && !str2.StartsWith("ipr:", StringComparison.OrdinalIgnoreCase))
          {
            requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Source value {0} is not in supported format.", (object) str2);
            return SessionTokenError.InvalidSource;
          }
          bool flag = false;
          switch (str2.Substring(0, str2.IndexOf(":") + 1))
          {
            case "ipa:":
              flag = this.IsValidIpAddress(str2.Substring("ipa:".Length));
              break;
            case "ipr:":
              flag = IPAddressRange.IsValidRange(str2.Substring("ipr:".Length));
              break;
          }
          if (!flag)
          {
            requestContext.Trace(1048002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService), "Source {0}, item value {1} is not in supported format.", (object) source, (object) str2);
            return SessionTokenError.InvalidSourceIP;
          }
        }
      }
      return sessionTokenError;
    }

    private bool IsValidIpAddress(string ipString) => IPAddress.TryParse(ipString, out IPAddress _);

    internal bool IsUserAuthorized(
      IVssRequestContext requestContext,
      Guid requestIdentityStorageKey,
      Guid userId)
    {
      return this.IsUserAuthorized(requestContext, requestIdentityStorageKey, userId, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService));
    }

    private bool IsServicePrincipal(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity targetIdentity) => this.IsServicePrincipal(requestContext, targetIdentity, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationService));

    private DateTime GetTokenFromValidDate() => DateTime.UtcNow.AddMinutes(-10.0);
  }
}
