// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationRegistrationService
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Tokens;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class PlatformDelegatedAuthorizationRegistrationService : 
    PlatformDelegatedAuthorizationBase,
    IDelegatedAuthorizationRegistrationService,
    IVssFrameworkService
  {
    private const string Area = "DelegatedAuthorizationService";
    private const string Layer = "PlatformDelegatedAuthorizationRegistrationService";
    private static readonly string FullTrustClientResponseType = "Assertion " + ResponseType.AppToken.ToString();
    private string issuerUri;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      ILocationService service = vssRequestContext.GetService<ILocationService>();
      Guid serviceAreaIdentifier = vssRequestContext.ExecutionEnvironment.IsHostedDeployment ? ServiceInstanceTypes.SPS : Guid.Empty;
      this.LoadExtensions(systemRequestContext);
      this.issuerUri = new Uri(service.GetLocationServiceUrl(vssRequestContext, serviceAreaIdentifier, AccessMappingConstants.PublicAccessMappingMoniker)).Host;
    }

    public string Issuer
    {
      get => this.issuerUri;
      private set => throw new NotImplementedException();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.DisposeExtensions();

    internal Registration PrepareRegistrationForDualWrites(
      IVssRequestContext requestContext,
      Registration registration)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IDelegatedAuthorizationConfigurationService service = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
      service.GetConfiguration(vssRequestContext);
      DelegatedAuthorizationSettings settings = service.GetSettings(vssRequestContext);
      if (registration.Issuer == null)
      {
        Guid secretVersionId = registration.SecretVersionId;
        DateTimeOffset? nullable1 = registration.ValidFrom;
        if (nullable1.HasValue)
        {
          nullable1 = registration.SecretValidTo;
          if (nullable1.HasValue)
            goto label_4;
        }
        registration.SecretVersionId = Guid.NewGuid();
        registration.ValidFrom = new DateTimeOffset?((DateTimeOffset) DateTime.UtcNow);
        Registration registration1 = registration;
        nullable1 = registration.ValidFrom;
        DateTimeOffset? nullable2 = new DateTimeOffset?(nullable1.Value.Add(settings.ClientSecretLifetime));
        registration1.SecretValidTo = nullable2;
      }
label_4:
      if (registration.RegistrationId == Guid.Empty)
        registration.RegistrationId = Guid.NewGuid();
      return registration;
    }

    public Registration Get(IVssRequestContext requestContext, Guid registrationId) => this.Get(requestContext, registrationId, false);

    public Registration Get(
      IVssRequestContext requestContext,
      Guid registrationId,
      bool includeSecret)
    {
      requestContext.TraceEnter(1057000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Get));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return this.ExecuteServiceMethods<Registration>(requestContext, (Func<IVssRequestContext, Registration>) (context => this.GetInternal(context, registrationId, includeSecret)), (Func<IVssRequestContext, Registration>) (context => this.GetRemote(context, registrationId, includeSecret)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Get));
      }
      finally
      {
        requestContext.TraceLeave(1057009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Get));
      }
    }

    private Registration GetRemote(
      IVssRequestContext requestContext,
      Guid registrationId,
      bool includeSecret)
    {
      return requestContext.GetService<ITokenRegistrationService>().Get(requestContext, registrationId, includeSecret);
    }

    private Registration GetInternal(
      IVssRequestContext requestContext,
      Guid registrationId,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      try
      {
        requestContext.TraceEnter(1057000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (GetInternal));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Registration registration;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.GetRegistration(Guid.Empty, registrationId);
        if (registration == null)
        {
          string message = DelegatedAuthorizationResources.RegistrationNotFound((object) registrationId);
          requestContext.Trace(1057001, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), message);
          throw new RegistrationNotFoundException(message);
        }
        if (!requestContext.IsSystemContext)
        {
          Guid guid = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
          if (registration.IdentityId != guid)
          {
            requestContext.Trace(1057002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), "Request identity is not same as target identity for get registration.");
            if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
            {
              string impersonate = DelegatedAuthorizationResources.NoPermissionToImpersonate((object) guid);
              requestContext.Trace(1057003, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), impersonate);
              throw new AccessCheckException(impersonate);
            }
          }
        }
        registration.Issuer = this.Issuer;
        if (includeSecret)
          registration.Secret = this.GetSecretInternal(requestContext, registration).EncodedToken;
        return this.DecodeHtmlEncodedStringsInAppRegistration(registration);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1057008, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1057009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (GetInternal));
      }
    }

    public Registration Create(IVssRequestContext requestContext, Registration registration) => this.Create(requestContext, registration, false);

    public Registration Create(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      requestContext.TraceEnter(1059000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Create));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 2);
        registration = this.PrepareRegistrationForDualWrites(requestContext, registration);
        return this.ExecuteServiceMethods<Registration>(requestContext, (Func<IVssRequestContext, Registration>) (context => this.CreateInternal(context, registration, includeSecret)), (Func<IVssRequestContext, Registration>) (context => this.CreateRemote(context, registration, includeSecret)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Create));
      }
      finally
      {
        requestContext.TraceLeave(1059009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Create));
      }
    }

    private void ValidateApplicationClientType(
      IVssRequestContext requestContext,
      Registration registration)
    {
      if (registration.IsWellKnown)
      {
        if (!this.IsApplicationRegistrationAllowed(requestContext))
        {
          string createRegistration = DelegatedAuthorizationResources.NoPermissionToCreateRegistration((object) requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment));
          requestContext.Trace(1059002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), createRegistration);
          throw new AccessCheckException(createRegistration);
        }
      }
      else if (this.IsCurrentRequestAuthenticatedUsingAlternateCredentials(requestContext))
      {
        string createRegistration = DelegatedAuthorizationResources.NoPermissionToCreateRegistration((object) requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment));
        requestContext.Trace(1059002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), createRegistration);
        throw new AccessCheckException(createRegistration);
      }
      if (registration.TenantIds == null || registration.TenantIds.Count <= 0)
        return;
      this.AreTenantIdsValid(requestContext, (IList<Guid>) registration.TenantIds, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService));
    }

    private Registration CreateRemote(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      if (registration.ClientType == ClientType.Application && !requestContext.IsSystemContext)
        this.ValidateApplicationClientType(requestContext, registration);
      return requestContext.GetService<ITokenRegistrationService>().Create(requestContext, registration, includeSecret);
    }

    private Registration CreateInternal(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      ArgumentUtility.CheckForDefinedEnum<ClientType>(registration.ClientType, "ClientType");
      try
      {
        requestContext.TraceEnter(1059000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (CreateInternal));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if ((registration.ClientType == ClientType.FullTrust || registration.ClientType == ClientType.HighTrust) && !requestContext.IsSystemContext && userIdentity.Id != InstanceManagementHelper.ServicePrincipalFromServiceInstance(ServiceInstanceTypes.SPS))
        {
          string createRegistration = DelegatedAuthorizationResources.NoPermissionToCreateRegistration((object) userIdentity.Id);
          requestContext.Trace(1059002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), createRegistration);
          throw new AccessCheckException(createRegistration);
        }
        IDelegatedAuthorizationConfigurationService service = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
        AuthorizationScopeConfiguration configuration = service.GetConfiguration(vssRequestContext);
        DelegatedAuthorizationSettings settings = service.GetSettings(vssRequestContext);
        registration.ResponseTypes = registration.ClientType == ClientType.FullTrust ? PlatformDelegatedAuthorizationRegistrationService.FullTrustClientResponseType : "Assertion";
        if (registration.ClientType != ClientType.MediumTrust)
          registration.IdentityId = userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
        if (registration.RegistrationId == Guid.Empty)
          registration.RegistrationId = Guid.NewGuid();
        if (registration.SecretVersionId == Guid.Empty)
          registration.SecretVersionId = Guid.NewGuid();
        if (!registration.ValidFrom.HasValue)
          registration.ValidFrom = new DateTimeOffset?((DateTimeOffset) DateTime.UtcNow);
        if (!registration.SecretValidTo.HasValue)
          registration.SecretValidTo = new DateTimeOffset?(registration.ValidFrom.Value.Add(settings.ClientSecretLifetime));
        RegistrationValidator.Validate(registration, configuration, settings);
        if (registration.ClientType == ClientType.MediumTrust)
        {
          requestContext.Trace(1059001, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), "Verifying framework impersonate permissions for request identity");
          if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
          {
            string createRegistration = DelegatedAuthorizationResources.NoPermissionToCreateRegistration((object) userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment));
            requestContext.Trace(1059002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), createRegistration);
            throw new AccessCheckException(createRegistration);
          }
          Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<Guid>) new Guid[1]
          {
            registration.IdentityId
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity == null)
          {
            identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext.Elevate(), (IList<Guid>) new Guid[1]
            {
              registration.IdentityId
            }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identity == null)
              throw new RegistrationCreateException(DelegatedAuthorizationResources.MediumTrustRequiresServiceIdentity());
          }
          if (!string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.ServiceIdentity"))
          {
            requestContext.Trace(1048111, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), "User {0} is not framework service identity type.", (object) identity.Id);
            throw new RegistrationCreateException(DelegatedAuthorizationResources.MediumTrustRequiresServiceIdentityWithDescriptor((object) identity.Descriptor));
          }
          registration.IdentityId = identity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
        }
        registration.RegistrationName = AntiXssEncoder.HtmlEncode(registration.RegistrationName, false);
        if (!string.IsNullOrWhiteSpace(registration.OrganizationName))
          registration.OrganizationName = AntiXssEncoder.HtmlEncode(registration.OrganizationName, false);
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.CreateRegistration(registration);
        if (registration == null)
        {
          requestContext.Trace(1059007, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), DelegatedAuthorizationResources.RegistrationCreationFailure());
          throw new RegistrationCreateException(DelegatedAuthorizationResources.RegistrationCreationFailure());
        }
        if (includeSecret)
          registration.Secret = this.GetSecretInternal(requestContext, registration).EncodedToken;
        return registration;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1059008, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1059009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (CreateInternal));
      }
    }

    public Registration Update(IVssRequestContext requestContext, Registration registration) => this.Update(requestContext, registration, false);

    public Registration Update(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      ArgumentUtility.CheckForEmptyGuid(registration.RegistrationId, "RegistrationId");
      requestContext.TraceEnter(1058000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Update));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        return this.ExecuteServiceMethods<Registration>(requestContext, (Func<IVssRequestContext, Registration>) (context => this.UpdateInternal(context, registration, includeSecret)), (Func<IVssRequestContext, Registration>) (context => this.UpdateRemote(context, registration, includeSecret)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Update));
      }
      finally
      {
        requestContext.TraceLeave(1058009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Update));
      }
    }

    private Registration UpdateRemote(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      if (registration.ClientType == ClientType.Application && !requestContext.IsSystemContext)
        this.ValidateApplicationClientType(requestContext, registration);
      return requestContext.GetService<ITokenRegistrationService>().Update(requestContext, registration, includeSecret);
    }

    private Registration UpdateInternal(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      ArgumentUtility.CheckForEmptyGuid(registration.RegistrationId, "RegistrationId");
      ArgumentUtility.CheckForDefinedEnum<ClientType>(registration.ClientType, "ClientType");
      try
      {
        requestContext.TraceEnter(1058000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (UpdateInternal));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IDelegatedAuthorizationConfigurationService service = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
        AuthorizationScopeConfiguration configuration = service.GetConfiguration(vssRequestContext);
        DelegatedAuthorizationSettings settings = service.GetSettings(vssRequestContext);
        RegistrationValidator.Validate(registration, configuration, settings, true);
        if (!string.IsNullOrWhiteSpace(registration.RegistrationName))
          registration.RegistrationName = AntiXssEncoder.HtmlEncode(registration.RegistrationName, false);
        if (!string.IsNullOrWhiteSpace(registration.OrganizationName))
          registration.OrganizationName = AntiXssEncoder.HtmlEncode(registration.OrganizationName, false);
        Registration registration1 = (Registration) null;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration1 = component.GetRegistration(Guid.Empty, registration.RegistrationId);
        if (registration1 == null)
        {
          string message = DelegatedAuthorizationResources.RegistrationNotFound((object) registration.RegistrationId);
          requestContext.Trace(1058001, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), message);
          throw new RegistrationNotFoundException(message);
        }
        if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.DisableUserToModifyAnyAppRegistrationFields"))
        {
          registration.Issuer = registration1.Issuer;
          registration.Scopes = registration1.Scopes;
          registration.TenantIds = registration1.TenantIds;
          registration.ValidFrom = registration1.ValidFrom;
          registration.IsWellKnown = registration1.IsWellKnown;
        }
        Guid guid = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
        if (registration1.IsRestricted() || registration1.IdentityId != guid)
        {
          requestContext.Trace(1058002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), "Request identity is not same as target identity for update registration.");
          if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
          {
            string impersonate = DelegatedAuthorizationResources.NoPermissionToImpersonate((object) guid);
            requestContext.Trace(1058002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), impersonate);
            throw new AccessCheckException(impersonate);
          }
        }
        if (registration1.ClientType != registration.ClientType)
        {
          requestContext.Trace(1058002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), DelegatedAuthorizationResources.NoPermissionToUpdateClientType());
          throw new ArgumentException(DelegatedAuthorizationResources.NoPermissionToUpdateClientType());
        }
        if (registration.RedirectUris.Count == registration1.RedirectUris.Count && registration.RedirectUris.Count != 0)
        {
          bool flag = true;
          IList<Uri> list1 = (IList<Uri>) registration.RedirectUris.OrderBy<Uri, string>((Func<Uri, string>) (o => o.ToString())).ToList<Uri>();
          IList<Uri> list2 = (IList<Uri>) registration1.RedirectUris.OrderBy<Uri, string>((Func<Uri, string>) (o => o.ToString())).ToList<Uri>();
          for (int index = 0; index < list1.Count; ++index)
          {
            if (Uri.Compare(list1[index], list2[index], UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.Ordinal) != 0)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            registration.RedirectUris.Clear();
        }
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.UpdateRegistration(registration);
        if (registration == null)
        {
          requestContext.Trace(1058007, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), DelegatedAuthorizationResources.RegistrationUpdateFailure());
          throw new RegistrationUpdateException(DelegatedAuthorizationResources.RegistrationUpdateFailure());
        }
        if (includeSecret)
          registration.Secret = this.GetSecretInternal(requestContext, registration).EncodedToken;
        return registration;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1058008, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1058009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (UpdateInternal));
      }
    }

    public Registration RotateSecret(IVssRequestContext requestContext, Guid registrationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      requestContext.TraceEnter(1058010, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (RotateSecret));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 8);
        return this.ExecuteServiceMethods<Registration>(requestContext, (Func<IVssRequestContext, Registration>) (context => this.RotateSecretInternal(context, registrationId)), (Func<IVssRequestContext, Registration>) (context => this.RotateSecretRemote(context, registrationId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (RotateSecret));
      }
      finally
      {
        requestContext.TraceLeave(1058011, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (RotateSecret));
      }
    }

    private Registration RotateSecretRemote(IVssRequestContext requestContext, Guid registrationId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      ITokenRegistrationService service = requestContext.GetService<ITokenRegistrationService>();
      if (userIdentity != authenticatedIdentity || !ServicePrincipals.IsServicePrincipal(requestContext, userIdentity.Descriptor))
      {
        try
        {
          DelegatedAuthorizationSecurity.CheckForAdministrativePermission(requestContext);
        }
        catch
        {
          requestContext.Trace(1058012, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), string.Format("Administrative permission failed, checking user: {0}", (object) userIdentity.Id));
          if (service.Get(requestContext, registrationId).IdentityId != userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment))
          {
            requestContext.Trace(1058012, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), string.Format("User permission failed, registration {0} user: {1}", (object) registrationId, (object) userIdentity.Id));
            throw new AccessCheckException("User does not have access to this registration");
          }
        }
      }
      return service.RotateSecret(requestContext, registrationId);
    }

    private Registration RotateSecretInternal(
      IVssRequestContext requestContext,
      Guid registrationId)
    {
      throw new NotImplementedException("Secret rotation has not been implemented.");
    }

    internal virtual JsonWebToken GetSecretInternal(
      IVssRequestContext requestContext,
      Registration registration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IDelegatedAuthorizationConfigurationService service = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
      DelegatedAuthorizationSettings settings = service.GetSettings(vssRequestContext);
      VssSigningCredentials signingCredentials = service.GetSigningCredentials(vssRequestContext, true);
      System.Collections.Generic.List<Claim> additionalClaims = new System.Collections.Generic.List<Claim>(3)
      {
        new Claim(DelegatedAuthorizationTokenClaims.ClientId, registration.RegistrationId.ToString()),
        new Claim(DelegatedAuthorizationTokenClaims.ClientSecretVersionId, registration.SecretVersionId.ToString()),
        new Claim("nameid", registration.IdentityId.ToString())
      };
      DateTimeOffset? nullable = registration.SecretValidTo;
      ref DateTimeOffset? local = ref nullable;
      DateTimeOffset valueOrDefault;
      DateTime dateTime;
      if (!local.HasValue)
      {
        dateTime = DateTime.UtcNow.Add(settings.ClientSecretLifetime);
      }
      else
      {
        valueOrDefault = local.GetValueOrDefault();
        dateTime = valueOrDefault.DateTime;
      }
      DateTime validTo = dateTime;
      nullable = registration.ValidFrom;
      DateTime validFrom;
      if (nullable.HasValue)
      {
        nullable = registration.ValidFrom;
        valueOrDefault = nullable.Value;
        validFrom = valueOrDefault.DateTime;
      }
      else
      {
        validFrom = validTo.Subtract(settings.ClientSecretLifetime);
        string message = string.Format("GetSecret validFrom : {0} validTo {1}.", (object) validFrom, (object) validTo);
        requestContext.Trace(1048600, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), message);
      }
      return JsonWebToken.Create(this.issuerUri, this.issuerUri, validFrom, validTo, (IEnumerable<Claim>) additionalClaims, signingCredentials, true);
    }

    public JsonWebToken GetSecret(IVssRequestContext requestContext, Registration registration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      ArgumentUtility.CheckForEmptyGuid(registration.RegistrationId, "RegistrationId");
      return this.GetSecret(requestContext, registration.RegistrationId);
    }

    public JsonWebToken GetSecret(IVssRequestContext requestContext, Guid registrationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      requestContext.TraceEnter(1059000, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (GetSecret));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        return this.ExecuteServiceMethods<JsonWebToken>(requestContext, (Func<IVssRequestContext, JsonWebToken>) (context => this.GetSecretInternal(context, registrationId)), (Func<IVssRequestContext, JsonWebToken>) (context => this.GetSecretRemote(context, registrationId)), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (GetSecret));
      }
      finally
      {
        requestContext.TraceLeave(1059009, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (GetSecret));
      }
    }

    private JsonWebToken GetSecretRemote(IVssRequestContext requestContext, Guid registrationId) => requestContext.GetService<ITokenRegistrationService>().GetSecret(requestContext, registrationId);

    private JsonWebToken GetSecretInternal(IVssRequestContext requestContext, Guid registrationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Registration registration;
      using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
        registration = component.GetRegistration(Guid.Empty, registrationId);
      if (registration == null)
      {
        string message = DelegatedAuthorizationResources.RegistrationNotFound((object) registrationId);
        requestContext.Trace(1058005, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), message);
        throw new RegistrationNotFoundException(message);
      }
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      Guid? nullable1 = userIdentity != null ? new Guid?(userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment)) : new Guid?();
      if (nullable1.HasValue)
      {
        Guid? nullable2 = nullable1;
        Guid identityId = registration.IdentityId;
        if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != identityId ? 1 : 0) : 0) : 1) == 0)
          goto label_11;
      }
      requestContext.Trace(1058006, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), "Request identity is not same as target identity for getting secret.");
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        string impersonate = DelegatedAuthorizationResources.NoPermissionToImpersonate((object) (nullable1?.ToString() ?? "null user"));
        requestContext.Trace(1058002, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), impersonate);
        throw new AccessCheckException(impersonate);
      }
label_11:
      return this.GetSecretInternal(requestContext, registration);
    }

    public IList<Registration> List(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(requestContext.GetUserId(), "userId");
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (List));
      int num = requestContext.GetUserId().GetHashCode() % 100;
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 1);
        return this.ExecuteListServiceMethods<Registration>(requestContext, (Func<IVssRequestContext, IEnumerable<Registration>>) (context => (IEnumerable<Registration>) this.ListInternal(context)), (Func<IVssRequestContext, IEnumerable<Registration>>) (context => (IEnumerable<Registration>) this.ListRemote(context)), "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (List));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (List));
      }
    }

    private IList<Registration> ListRemote(IVssRequestContext requestContext) => requestContext.GetService<ITokenRegistrationService>().List(requestContext);

    private IList<Registration> ListInternal(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(requestContext.GetUserId(), "userId");
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IList<Registration> registrations = (IList<Registration>) null;
      Guid identityId = requestContext.GetUserIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
      using (DelegatedAuthorizationComponent component = context.CreateComponent<DelegatedAuthorizationComponent>())
        registrations = component.ListRegistrations(identityId);
      return this.DecodeHtmlEncodedStringsInAppRegistration(registrations);
    }

    public void Delete(IVssRequestContext requestContext, Guid registrationId)
    {
      requestContext.TraceEnter(1050110, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Delete));
      try
      {
        DelegatedAuthorizationSecurity.CheckPermission(requestContext, 3);
        this.ExecuteServiceMethods<bool>(requestContext, (Func<IVssRequestContext, bool>) (context =>
        {
          this.DeleteInternal(context, registrationId);
          return true;
        }), (Func<IVssRequestContext, bool>) (context =>
        {
          this.DeleteRemote(context, registrationId);
          return true;
        }), 2600001, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Delete));
      }
      finally
      {
        requestContext.TraceLeave(1050119, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (Delete));
      }
    }

    private void DeleteRemote(IVssRequestContext requestContext, Guid registrationId) => requestContext.GetService<ITokenRegistrationService>().Delete(requestContext, registrationId);

    private void DeleteInternal(IVssRequestContext requestContext, Guid registrationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        vssRequestContext.TraceEnter(1048525, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (DeleteInternal));
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        Registration registration;
        using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
          registration = component.GetRegistration(Guid.Empty, registrationId);
        if (registration == null)
        {
          string message = DelegatedAuthorizationResources.RegistrationNotFound((object) registrationId);
          requestContext.Trace(1056001, TraceLevel.Info, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), message);
        }
        else
        {
          Guid guid = userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
          if (registration.IsRestricted() || registration.IdentityId != guid)
          {
            requestContext.Trace(1058004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), "Request identity is not same as target identity for delete registration.");
            if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
            {
              string impersonate = DelegatedAuthorizationResources.NoPermissionToImpersonate((object) guid);
              requestContext.Trace(1058004, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), impersonate);
              throw new AccessCheckException(impersonate);
            }
          }
          using (DelegatedAuthorizationComponent component = vssRequestContext.CreateComponent<DelegatedAuthorizationComponent>())
            component.DeleteRegistration(registrationId);
        }
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(1048526, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), ex);
        throw;
      }
      finally
      {
        vssRequestContext.TraceLeave(1048527, "DelegatedAuthorizationService", nameof (PlatformDelegatedAuthorizationRegistrationService), nameof (DeleteInternal));
      }
    }

    private IList<Registration> DecodeHtmlEncodedStringsInAppRegistration(
      IList<Registration> registrations)
    {
      if (registrations == null || registrations.Count == 0)
        return registrations;
      System.Collections.Generic.List<Registration> registrationList = new System.Collections.Generic.List<Registration>();
      foreach (Registration registration in (IEnumerable<Registration>) registrations)
        registrationList.Add(this.DecodeHtmlEncodedStringsInAppRegistration(registration));
      return (IList<Registration>) registrationList;
    }

    private Registration DecodeHtmlEncodedStringsInAppRegistration(Registration registration)
    {
      if (registration != null)
      {
        registration.OrganizationName = HttpUtility.HtmlDecode(registration.OrganizationName);
        registration.RegistrationName = HttpUtility.HtmlDecode(registration.RegistrationName);
      }
      return registration;
    }
  }
}
