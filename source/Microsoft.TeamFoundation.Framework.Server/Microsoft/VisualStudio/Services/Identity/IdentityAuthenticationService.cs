// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityAuthenticationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityAuthenticationService : IIdentityAuthenticationService, IVssFrameworkService
  {
    private const string Area = "IdentityAuthenticationService";
    private const string Layer = "BusinessLogic";
    private static readonly string[] IdentityAttributes = new string[6]
    {
      "Description",
      "Domain",
      "Account",
      "DN",
      "Mail",
      "PUID"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity ResolveIdentity(
      IVssRequestContext requestContext,
      IIdentity bclIdentity)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (ResolveIdentity));
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.LegacyResolveIdentity(requestContext, bclIdentity);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      string identityType = IdentityHelper.GetIdentityType(bclIdentity);
      IIdentityProvider provider;
      if (service.SyncAgents == null || !service.SyncAgents.TryGetValue(identityType, out provider))
        throw new NotSupportedException(FrameworkResources.IdentityProviderNotFoundMessage((object) identityType));
      IdentityDescriptor descriptor = IdentityAuthenticationService.CreateDescriptor(requestContext, bclIdentity, vssRequestContext, provider);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (bclIdentity is WindowsIdentity)
        requestContext.TraceAlways(1012992, TraceLevel.Error, nameof (IdentityAuthenticationService), "BusinessLogic", "Found Windows Identity in hosted environement");
      else
        identity1 = IdentityAuthenticationService.GetIdentityFromProvider(requestContext, bclIdentity, vssRequestContext, provider, descriptor);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (identity1 == null || identity1.Id == Guid.Empty)
      {
        SubjectDescriptor subjectDescriptor = identity1 != null ? identity1.SubjectDescriptor : new SubjectDescriptor();
        bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSubjectDescriptorForDIMS");
        if (flag && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && subjectDescriptor != new SubjectDescriptor())
          identity2 = service.ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
          {
            subjectDescriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (identity2 == null)
        {
          identity2 = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            descriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (flag && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && identity2 != null)
            requestContext.TraceAlways(1012989, TraceLevel.Error, nameof (IdentityAuthenticationService), "BusinessLogic", "Cannot find identity using SubjectDescriptor {0}, falling back to read IdentityDescriptor {1}. Found deployment storage key {2}.", (object) subjectDescriptor, (object) descriptor, (object) identity2.Id);
        }
      }
      else
      {
        if (identity1.IsClaims)
          requestContext.TraceAlways(1012989, TraceLevel.Error, nameof (IdentityAuthenticationService), "BusinessLogic", "GetIdentityFromProvider created full claims identity in hosted environment (with storagekey set)");
        identity2 = identity1;
      }
      if (identity2 != null && IdentityDescriptorComparer.Instance.Equals(descriptor, identity2.Descriptor))
        identity2.Descriptor.Data = descriptor.Data;
      if (identity2 == null)
        identity1 = IdentityAuthenticationService.GetIdentityFromProvider(requestContext, bclIdentity, vssRequestContext, provider, descriptor);
      if (identity1 != null)
        identity2 = IdentityAuthenticationService.EnsureRolesIfServiceIdentity(requestContext, vssRequestContext, service, identity2, identity1);
      Microsoft.VisualStudio.Services.Identity.Identity identity3 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (identity1 != null)
        identity3 = requestContext.GetService<IClaimsUpdateService>().UpdateClaims(requestContext, identity1, bclIdentity);
      if (identity3 != null)
        identity2 = identity3;
      if (identity1 != null && identity2 != null)
        this.ProcessAadTokens(requestContext, vssRequestContext, descriptor, identity2);
      if (identity2 == null)
        identity2 = identity1;
      if (identity2 == null)
        throw new IdentityNotFoundException(FrameworkResources.IdentityNotFoundSimpleMessage());
      requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (ResolveIdentity));
      return identity2;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity LegacyResolveIdentity(
      IVssRequestContext requestContext,
      IIdentity bclIdentity)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (LegacyResolveIdentity));
      bool isAlternateLoginIdentity = requestContext.To(TeamFoundationHostType.Deployment).Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.AlternateAuthCredentialsContextKey);
      bool castedValueOrDefault1 = requestContext.To(TeamFoundationHostType.Deployment).Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.AlternateAuthCredentialsIdentityCreatorContextKey);
      bool castedValueOrDefault2 = requestContext.To(TeamFoundationHostType.Deployment).Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.AlternateAuthCredentialsIdentityCreatorContextKey);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IIdentityProvider provider;
      if (service.SyncAgents == null || !service.SyncAgents.TryGetValue(IdentityHelper.GetIdentityType(bclIdentity), out provider))
        throw new NotSupportedException(FrameworkResources.IdentityProviderNotFoundMessage((object) bclIdentity.GetType().FullName));
      IdentityDescriptor descriptor = IdentityAuthenticationService.CreateDescriptor(requestContext, bclIdentity, vssRequestContext, provider);
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (identity != null && IdentityDescriptorComparer.Instance.Equals(descriptor, identity.Descriptor))
        identity.Descriptor.Data = descriptor.Data;
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!(bclIdentity is WindowsIdentity) || identity == null)
        identityFromProvider = IdentityAuthenticationService.GetIdentityFromProvider(requestContext, bclIdentity, vssRequestContext, provider, descriptor);
      if (bclIdentity != null && !isAlternateLoginIdentity)
        isAlternateLoginIdentity = IdentityAuthenticationService.CheckForAlternateLoginIdentity(requestContext, bclIdentity, vssRequestContext);
      if (identityFromProvider != null)
        identity = IdentityAuthenticationService.EnsureRolesIfServiceIdentity(requestContext, vssRequestContext, service, identity, identityFromProvider);
      bool createdIdentity = false;
      bool bindPendingIdentity = false;
      if (!isAlternateLoginIdentity | castedValueOrDefault1 && identity == null && identityFromProvider != null && vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        identity = IdentityAuthenticationService.LegacyFindBindPendingIdentity(requestContext, vssRequestContext, service, descriptor, identityFromProvider);
        bindPendingIdentity = identity != null;
        if (!bindPendingIdentity)
          identity = IdentityAuthenticationService.LegacyCreateIdentityWithClaims(requestContext, vssRequestContext, service, descriptor, identityFromProvider, ref createdIdentity);
      }
      if (identityFromProvider != null && identity != null)
        this.ProcessAadTokens(requestContext, vssRequestContext, descriptor, identity);
      bool isServiceIdentity = false;
      if (identity != null && !createdIdentity)
        isServiceIdentity = IdentityHelper.IsServiceIdentity(requestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) identity);
      if (identity == null)
        identity = identityFromProvider;
      else if (!createdIdentity)
        identity = IdentityAuthenticationService.UpdateIdentityWithClaims(requestContext, isAlternateLoginIdentity, castedValueOrDefault2, vssRequestContext, service, provider, identity, identityFromProvider, bindPendingIdentity, isServiceIdentity);
      if (identity == null)
        throw new IdentityNotFoundException(FrameworkResources.IdentityNotFoundSimpleMessage());
      requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (LegacyResolveIdentity));
      return identity;
    }

    private static IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity bclIdentity,
      IVssRequestContext elevatedRequestContext,
      IIdentityProvider provider)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (CreateDescriptor));
      try
      {
        IdentityDescriptor descriptor = provider.CreateDescriptor(elevatedRequestContext, bclIdentity);
        return !(descriptor == (IdentityDescriptor) null) ? descriptor : throw new IdentityNotFoundException(FrameworkResources.IdentityNotFoundSimpleMessage());
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (CreateDescriptor));
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity LegacyFindBindPendingIdentity(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedRequestContext,
      IdentityService identityService,
      IdentityDescriptor identityDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (LegacyFindBindPendingIdentity));
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity bindPendingIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        string property1 = identityFromProvider.GetProperty<string>("Domain", (string) null);
        string property2 = identityFromProvider.GetProperty<string>("Account", (string) null);
        string property3 = identityFromProvider.GetProperty<string>("Mail", (string) null);
        if (IdentityHelper.IsValidUserDomain(property1) && !string.IsNullOrWhiteSpace(property2))
        {
          IdentityDescriptor descriptorFromAccountName = IdentityHelper.CreateDescriptorFromAccountName(property1, property2, true);
          bindPendingIdentity = identityService.ReadIdentities(elevatedRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            descriptorFromAccountName
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        if (bindPendingIdentity == null && !string.IsNullOrWhiteSpace(property3))
        {
          IdentityDescriptor identityDescriptor1 = IdentityAuthenticationHelper.BuildTemporaryDescriptorFromEmailAddress(property3);
          bindPendingIdentity = identityService.ReadIdentities(elevatedRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identityDescriptor1
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        if (bindPendingIdentity != null)
        {
          bindPendingIdentity.Descriptor = identityDescriptor;
          identityService.UpdateIdentities(elevatedRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            bindPendingIdentity
          }, true);
        }
        return bindPendingIdentity;
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (LegacyFindBindPendingIdentity));
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity UpdateIdentityWithClaims(
      IVssRequestContext requestContext,
      bool isAlternateLoginIdentity,
      bool isAlternateLoginIdentityUpdater,
      IVssRequestContext elevatedRequestContext,
      IdentityService identityService,
      IIdentityProvider provider,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      bool bindPendingIdentity,
      bool isServiceIdentity)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (UpdateIdentityWithClaims));
      try
      {
        if (!isAlternateLoginIdentity | bindPendingIdentity | isAlternateLoginIdentityUpdater)
        {
          if (identityFromProvider != null)
          {
            if (!identityFromProvider.IsContainer)
            {
              if (!isServiceIdentity)
              {
                if (IdentityAuthenticationService.IdentityFromProviderHasPropertiesToRefresh(elevatedRequestContext, identity, identityFromProvider))
                {
                  IdentityAuthenticationService.MergeIdentityPropertiesForRefresh(elevatedRequestContext, identity, identityFromProvider);
                  identityService.UpdateIdentities(elevatedRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
                  {
                    identityFromProvider
                  }, true);
                  identity = identityFromProvider;
                }
                else
                  IdentityAuthenticationService.CopyAvailableAttributes(identity, identityFromProvider, provider);
              }
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (UpdateIdentityWithClaims));
      }
      return identity;
    }

    private static bool CheckForAlternateLoginIdentity(
      IVssRequestContext requestContext,
      IIdentity bclIdentity,
      IVssRequestContext elevatedRequestContext)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (CheckForAlternateLoginIdentity));
      bool flag = false;
      try
      {
        string fullName = bclIdentity.GetType().FullName;
        elevatedRequestContext.RootContext.WebRequestContextInternal().SetAuthenticationType(bclIdentity.AuthenticationType);
        if (string.Equals(fullName, "Microsoft.VisualStudio.Services.Cloud.AlternateLoginIdentity", StringComparison.OrdinalIgnoreCase))
          flag = true;
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (CheckForAlternateLoginIdentity));
      }
      return flag;
    }

    private void ProcessAadTokens(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedRequestContext,
      IdentityDescriptor identityDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (ProcessAadTokens));
      try
      {
        object aadIdentityToken;
        requestContext.RootContext.Items.TryGetValue(RequestContextItemsKeys.AadAuthorizationToken, out aadIdentityToken);
        object authorizationCode;
        requestContext.RootContext.Items.TryGetValue(RequestContextItemsKeys.AadAuthorizationCode, out authorizationCode);
        if (aadIdentityToken == null)
          return;
        if (authorizationCode != null)
        {
          requestContext.Trace(1012996, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Auth code is received for Identity {0} from AAD, processing it", (object) identityDescriptor);
          IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          JwtSecurityToken jwtSecurityToken = requestContext1.GetExtensions<IAadAuthCodeHandler>(ExtensionLifetime.Service).SingleOrDefault<IAadAuthCodeHandler>()?.ProcessAadAuthCode(requestContext1, (string) aadIdentityToken, (string) authorizationCode, identity);
          if (requestContext.Items.Keys.Contains<string>("PreAuthSilentAadProfileCreationRequested", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !requestContext.IsFeatureEnabled("VisualStudio.Services.ProfileService.DisableSilentProfileFromGallery"))
          {
            requestContext.Trace(1012998, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "requestContext contains key {0} requesting silent profile. Adding key {1} to create silent profile this request", (object) "PreAuthSilentAadProfileCreationRequested", (object) "PostAuthCreateSilentAadProfile");
            requestContext.Items.Add("PostAuthCreateSilentAadProfile", (object) bool.TrueString);
          }
          if (jwtSecurityToken != null)
            requestContext.Items["AadCookieTokenValidator_ValidatedToken"] = (object) jwtSecurityToken;
        }
        else
        {
          requestContext.Trace(1012997, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Identity {0} is authenticated with an AAD access token, trying to refresh its refresh token if it is expired", (object) identityDescriptor);
          IVssRequestContext requestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
          requestContext2.GetExtensions<IAadOnBehalfOfHandler>(ExtensionLifetime.Service).SingleOrDefault<IAadOnBehalfOfHandler>()?.TryUpdateRefreshTokenOnBehalfOfUser(requestContext2, (string) aadIdentityToken, identity);
        }
        requestContext.GetExtensions<IAadIdTokenHandler>(ExtensionLifetime.Service).ForEach<IAadIdTokenHandler>((Action<IAadIdTokenHandler>) (idTokenHandler => idTokenHandler?.ProcessAadIdToken(requestContext, new JwtSecurityToken((string) aadIdentityToken), identity)));
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (ProcessAadTokens));
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity LegacyCreateIdentityWithClaims(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedRequestContext,
      IdentityService identityService,
      IdentityDescriptor identityDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      ref bool createdIdentity)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (LegacyCreateIdentityWithClaims));
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identityWithClaims;
        if (elevatedRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          identityWithClaims = identityFromProvider;
          identityService.UpdateIdentities(elevatedRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identityWithClaims
          }, true);
          createdIdentity = true;
        }
        else
        {
          requestContext.Trace(1012998, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Identity with descriptor '{0}' is not found at account conext, reading at the deployment context.", (object) identityDescriptor);
          IVssRequestContext vssRequestContext = elevatedRequestContext.To(TeamFoundationHostType.Deployment);
          IdentityService service = vssRequestContext.GetService<IdentityService>();
          identityWithClaims = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identityDescriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (identityWithClaims == null)
          {
            requestContext.Trace(1012999, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Identity with descriptor '{0}' is not found at deployment context, introducing the new user.", (object) identityDescriptor);
            identityWithClaims = identityFromProvider;
            service.UpdateIdentities(vssRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              identityWithClaims
            }, true);
            createdIdentity = true;
          }
        }
        return identityWithClaims;
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (LegacyCreateIdentityWithClaims));
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity EnsureRolesIfServiceIdentity(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedRequestContext,
      IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider)
    {
      requestContext.TraceEnter(1012990, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (EnsureRolesIfServiceIdentity));
      try
      {
        string property = identityFromProvider.GetProperty<string>("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", (string) null);
        if (!string.IsNullOrEmpty(property))
        {
          if (property == "Service")
          {
            requestContext.Trace(1012994, TraceLevel.Verbose, nameof (IdentityAuthenticationService), "BusinessLogic", "Found a ServiceRoleClaim on identity: {0}", (object) identityFromProvider.DisplayName);
            if ((!identityFromProvider.Descriptor.IsClaimsIdentityType() || !ServicePrincipals.IsServicePrincipal(requestContext, identityFromProvider.Descriptor)) && (!requestContext.ExecutionEnvironment.IsHostedDeployment || !identityFromProvider.Descriptor.IsSystemServicePrincipalType()))
            {
              requestContext.Trace(1012993, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Entering Fault-in logic for Service identity: {0}; NOT ServicePrincipal.", (object) identityFromProvider.DisplayName);
              if (!identityService.IsMember(elevatedRequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, identityFromProvider.Descriptor))
              {
                requestContext.Trace(1012993, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Faulting-in ServiceIdentity: {0}", (object) identityFromProvider.DisplayName);
                identityService.AddMemberToGroup(elevatedRequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, identityFromProvider);
              }
              else
                requestContext.Trace(1012993, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Skipping fault-in for ServiceIdentity: {0}, already present in ServiceUsersGroup", (object) identityFromProvider.DisplayName);
            }
            if (identity == null)
              identity = identityFromProvider;
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(1012991, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (EnsureRolesIfServiceIdentity));
      }
      return identity;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetIdentityFromProvider(
      IVssRequestContext requestContext,
      IIdentity bclIdentity,
      IVssRequestContext elevatedRequestContext,
      IIdentityProvider provider,
      IdentityDescriptor identityDescriptor)
    {
      requestContext.TraceEnter(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (GetIdentityFromProvider));
      try
      {
        return provider.GetIdentity(elevatedRequestContext, bclIdentity);
      }
      catch (Exception ex)
      {
        throw new IdentitySyncException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1} - {2}", (object) identityDescriptor.IdentityType, (object) identityDescriptor.Identifier, (object) ex.Message), ex);
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (IdentityAuthenticationService), "BusinessLogic", nameof (GetIdentityFromProvider));
      }
    }

    private static bool IdentityFromProviderHasPropertiesToRefresh(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity databaseIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider)
    {
      if (!StringComparer.OrdinalIgnoreCase.Equals(identityFromProvider.ProviderDisplayName, databaseIdentity.ProviderDisplayName) && !string.IsNullOrEmpty(identityFromProvider.ProviderDisplayName))
      {
        requestContext.Trace(999999, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "ProviderDisplayName is different Source:{0} Db:{1}", (object) identityFromProvider.ProviderDisplayName, (object) databaseIdentity.ProviderDisplayName);
        return true;
      }
      string property1 = identityFromProvider.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", (string) null);
      string property2 = databaseIdentity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", (string) null);
      if (!string.IsNullOrEmpty(property1))
      {
        if (string.IsNullOrEmpty(property2))
        {
          requestContext.Trace(999999, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Identity object id found in source claims but does not exist in database. Refreshing database claims.");
          return true;
        }
        if (!string.Equals(property1, property2, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.Trace(999999, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "Identity object id found in source claims does not match object id in database. Refreshing database claims.");
          return true;
        }
      }
      foreach (string identityAttribute in IdentityAuthenticationService.IdentityAttributes)
      {
        string property3 = databaseIdentity.GetProperty<string>(identityAttribute, string.Empty);
        string property4 = identityFromProvider.GetProperty<string>(identityAttribute, string.Empty);
        if (!StringComparer.OrdinalIgnoreCase.Equals(property4, property3) && !string.IsNullOrEmpty(property4))
        {
          requestContext.Trace(999999, TraceLevel.Info, nameof (IdentityAuthenticationService), "BusinessLogic", "{0} is different Source:{1} Db:{2}", (object) identityAttribute, (object) property4, (object) property3);
          return true;
        }
      }
      return false;
    }

    private static void MergeIdentityPropertiesForRefresh(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity databaseIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider)
    {
      identityFromProvider.Id = databaseIdentity.Id;
      DateTime defaultValue = new DateTime(0L, DateTimeKind.Utc);
      DateTime property = databaseIdentity.GetProperty<DateTime>("ComplianceValidated", defaultValue);
      if (!(property != defaultValue))
        return;
      identityFromProvider.SetProperty("ComplianceValidated", (object) property);
    }

    private static void CopyAvailableAttributes(
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      IIdentityProvider provider)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(targetIdentity, nameof (targetIdentity));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identityFromProvider, nameof (identityFromProvider));
      ArgumentUtility.CheckForNull<IIdentityProvider>(provider, nameof (provider));
      if (identityFromProvider.Descriptor.Data != null && targetIdentity.Descriptor.Data == null)
        targetIdentity.Descriptor.Data = identityFromProvider.Descriptor.Data;
      if (provider.AvailableIdentityAttributes == null)
        return;
      foreach (string identityAttribute in provider.AvailableIdentityAttributes)
      {
        object obj;
        if (identityFromProvider.TryGetProperty(identityAttribute, out obj))
          targetIdentity.SetProperty(identityAttribute, obj);
      }
    }
  }
}
