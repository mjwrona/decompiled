// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformClaimsUpdateService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity.Events;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class PlatformClaimsUpdateService : IClaimsUpdateService, IVssFrameworkService
  {
    private static readonly string[] IdentityAttributes = new string[6]
    {
      "Description",
      "Domain",
      "Account",
      "DN",
      "Mail",
      "PUID"
    };
    private const string Area = "Identity";
    private const string Layer = "PlatformClaimsUpdateService";
    private const int TraceOidChanges = 7689098;
    private readonly IConfigQueryable<bool> mungeSourceIdentitiesConfig;
    private readonly IIdentityProvider identityProvider;
    private readonly PlatformClaimsUpdateService.IIdentityClaimsComparer claimsComparer;
    private static readonly IConfigPrototype<bool> mungeSourceIdentitiesPrototype = ConfigPrototype.Create<bool>("AzureDevOps.Identity.MungeSourceIdentities.M226", false);
    private static readonly VssPerformanceCounter s_GitHubBindPendingUpgradeCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.GitHubIdentities.BindPendingUpgrade");

    public PlatformClaimsUpdateService()
      : this((IIdentityProvider) new ClaimsProvider(), (PlatformClaimsUpdateService.IIdentityClaimsComparer) PlatformClaimsUpdateService.IdentityClaimsComparer.Instance, ConfigProxy.Create<bool>(PlatformClaimsUpdateService.mungeSourceIdentitiesPrototype))
    {
    }

    public PlatformClaimsUpdateService(
      IIdentityProvider provider,
      PlatformClaimsUpdateService.IIdentityClaimsComparer claimsComparer,
      IConfigQueryable<bool> mungeSourceIdentitiesConfig)
    {
      this.identityProvider = provider;
      this.claimsComparer = claimsComparer;
      this.mungeSourceIdentitiesConfig = mungeSourceIdentitiesConfig;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateOrBindWithClaims(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity)
    {
      requestContext.TraceEnter(15109030, "Identity", nameof (PlatformClaimsUpdateService), nameof (CreateOrBindWithClaims));
      try
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(sourceIdentity, nameof (sourceIdentity));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(sourceIdentity.Descriptor, "Descriptor");
        this.CheckPermissions(requestContext);
        Microsoft.VisualStudio.Services.Identity.Identity identityFromDatabase = PlatformClaimsUpdateService.ReadIdentityWithDeploymentLevelFallback(requestContext, sourceIdentity.Descriptor);
        if (identityFromDatabase == null)
        {
          requestContext.Trace(15109031, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "IdentityFromDatabase is null, checking to find BindPending identity");
          PlatformClaimsUpdateService.LocalAndMasterIdentityId identityIds = PlatformClaimsUpdateService.FindBindPendingId(requestContext, sourceIdentity);
          if (identityIds != null)
          {
            requestContext.TraceConditionally(15109031, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), (Func<string>) (() => string.Format("Found bind pending identity with id {0} and masterId {1}, binding it", (object) identityIds.LocalId, (object) identityIds.MasterId)));
            sourceIdentity.Id = identityIds.LocalId;
            sourceIdentity.MasterId = identityIds.MasterId;
          }
          requestContext.GetService<IdentityService>().UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            sourceIdentity
          }, true);
          return PlatformClaimsUpdateService.ReadIdentityWithDeploymentLevelFallback(requestContext, sourceIdentity.Descriptor);
        }
        requestContext.TraceConditionally(15109031, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), (Func<string>) (() => string.Format("Found existing identity with descriptor {0}, id {1} and masterId {2}. Skipping CreateOrBind and returning existing identity", (object) identityFromDatabase.Descriptor, (object) identityFromDatabase.Id, (object) identityFromDatabase.MasterId)));
        return identityFromDatabase;
      }
      finally
      {
        requestContext.TraceLeave(15109039, "Identity", nameof (PlatformClaimsUpdateService), nameof (CreateOrBindWithClaims));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity UpdateClaims(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal)
    {
      requestContext.TraceEnter(15109000, "Identity", nameof (PlatformClaimsUpdateService), nameof (UpdateClaims));
      try
      {
        IdentityDescriptor descriptor = this.identityProvider.CreateDescriptor(requestContext, claimsPrincipal?.Identity);
        if (descriptor != (IdentityDescriptor) null)
        {
          IdentityService service = requestContext.GetService<IdentityService>();
          Microsoft.VisualStudio.Services.Identity.Identity databaseIdentity = PlatformClaimsUpdateService.ReadIdentityWithDeploymentLevelFallback(requestContext, descriptor);
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.identityProvider.GetIdentity(requestContext, claimsPrincipal.Identity);
          if (databaseIdentity != null && identity != null)
          {
            identity.Id = databaseIdentity.Id;
            identity.MasterId = databaseIdentity.MasterId;
            if (!this.claimsComparer.SourceClaimsRequireRefresh(requestContext, databaseIdentity, identity))
              return databaseIdentity;
          }
          if (identity != null)
          {
            service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              identity
            }, true);
            return databaseIdentity != null ? identity : PlatformClaimsUpdateService.ReadIdentityAtDeploymentLevel(requestContext, descriptor);
          }
        }
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(15109009, "Identity", nameof (PlatformClaimsUpdateService), nameof (UpdateClaims));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity UpdateClaims(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      IIdentity claimsIdentityFromToken)
    {
      requestContext.TraceEnter(15109010, "Identity", nameof (PlatformClaimsUpdateService), nameof (UpdateClaims));
      try
      {
        requestContext.TraceSerializedConditionally(15109013, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Identity from provider: {0} | bclIdentity:{1}", (object) identityFromProvider, (object) claimsIdentityFromToken);
        bool flag1 = requestContext.To(TeamFoundationHostType.Deployment).Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.AlternateAuthCredentialsContextKey) || this.CheckForAlternateLoginIdentity(requestContext, claimsIdentityFromToken);
        bool castedValueOrDefault1 = requestContext.To(TeamFoundationHostType.Deployment).Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.AlternateAuthCredentialsIdentityCreatorContextKey);
        bool castedValueOrDefault2 = requestContext.To(TeamFoundationHostType.Deployment).Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.AlternateAuthCredentialsIdentityUpdaterContextKey);
        requestContext.Trace(15109012, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "isAlternateLoginIdentity: {0}, isAlternateLoginIdentityCreator: {1}, isAlternateLoginIdentityUpdater: {2}", (object) flag1, (object) castedValueOrDefault1, (object) castedValueOrDefault2);
        requestContext = requestContext.Elevate();
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identityInDatabase = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSubjectDescriptorForDIMS"))
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && identityFromProvider.SubjectDescriptor != new SubjectDescriptor())
          {
            identityInDatabase = service.ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
            {
              identityFromProvider.SubjectDescriptor
            }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identityInDatabase == null)
            {
              identityInDatabase = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
              {
                identityFromProvider.Descriptor
              }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
              if (identityInDatabase != null)
                requestContext.TraceAlways(15109007, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Cannot find identity using SubjectDescriptor {0}, falling back to read IdentityDescriptor {1}. Found deployment storage key {2}.", (object) identityFromProvider.SubjectDescriptor, (object) identityFromProvider.Descriptor, (object) identityInDatabase.Id);
            }
          }
          else
            identityInDatabase = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              identityFromProvider.Descriptor
            }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        else
          identityInDatabase = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identityFromProvider.Descriptor
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        bool flag2 = false;
        if (identityInDatabase == null || !identityInDatabase.IsActive)
        {
          requestContext.TraceSerializedConditionally(15109049, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Identity not found or not active for : {0}, Identity: {1}", (object) identityFromProvider.Descriptor, (object) identityInDatabase);
          flag2 = true;
          if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && identityInDatabase == null)
          {
            identityInDatabase = requestContext.GetService<PlatformIdentityService>().ReadIdentitiesFromStore(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              identityFromProvider.Descriptor
            }, QueryMembership.None, (IEnumerable<string>) null, filterResults: false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            requestContext.TraceSerializedConditionally(253716, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Read Identity response without filtering out based on membership for : {0}, Identity: {1}", (object) identityFromProvider.Descriptor, (object) identityInDatabase);
          }
        }
        else
          requestContext.TraceSerializedConditionally(15109011, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Identity read at current host level: Descriptor:{0}, Identity: {1}", (object) identityFromProvider.Descriptor, (object) identityInDatabase);
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && identityInDatabase != null && identityInDatabase.Descriptor != (IdentityDescriptor) null && !string.Equals(identityFromProvider.Descriptor.Identifier, identityInDatabase.Descriptor.Identifier, StringComparison.Ordinal))
        {
          identityInDatabase = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          flag2 = true;
        }
        identityInDatabase = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseNewHandleOidChangeFlow") ? PlatformClaimsUpdateService.HandleOidChangesForConflictingUpn2(requestContext, identityFromProvider, identityInDatabase) : PlatformClaimsUpdateService.HandleOidChangesForConflictingUpn(requestContext, identityFromProvider, identityInDatabase);
        bool flag3 = requestContext.IsFeatureEnabled(PlatformIdentityStore.ResolveByOid);
        bool flag4 = false;
        if (identityInDatabase == null & flag3)
        {
          identityInDatabase = PlatformClaimsUpdateService.ReadIdentityByDomainAndOid(requestContext, identityFromProvider);
          if (identityInDatabase != null)
          {
            flag4 = true;
            if (IdentityDescriptorComparer.Instance.Equals(identityFromProvider.Descriptor, identityInDatabase.Descriptor))
            {
              requestContext.TraceSerializedConditionally(15109028, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Identity from provider descriptor: {0} and databaseidentity descriptor: {1} are the same in UPN rename flow", (object) identityFromProvider.Descriptor, (object) identityInDatabase.Descriptor);
            }
            else
            {
              flag2 = false;
              requestContext.TraceSerializedConditionally(15109029, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Updating UPN of identity: {0} that was found in the database (read using domain and oid combination) with the Identity from provider: {1}", (object) identityInDatabase.Descriptor, (object) identityFromProvider.Descriptor);
            }
          }
        }
        if (identityInDatabase != null)
        {
          identityFromProvider.Id = identityInDatabase.Id;
          identityFromProvider.MasterId = identityInDatabase.MasterId;
        }
        bool bindPendingIdentity = false;
        if (!flag1 | castedValueOrDefault1 && identityFromProvider != null && !identityFromProvider.IsCspPartnerUser && requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (identityInDatabase == null)
          {
            bindPendingIdentity = PlatformClaimsUpdateService.CheckForBindPendingReclamationForIdentity(requestContext, identityFromProvider) || PlatformClaimsUpdateService.CheckForBindPendingReclamationForIdentityWithEmaillessUpn(requestContext, identityFromProvider, false);
            requestContext.TraceDataConditionally(15109116, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Attempted to reclaim invitation for incoming identity", (Func<object>) (() => (object) new
            {
              identityFromProvider = identityFromProvider,
              bindPendingIdentity = bindPendingIdentity
            }), nameof (UpdateClaims));
          }
          else
          {
            bindPendingIdentity = PlatformClaimsUpdateService.CheckForBindPendingReclamationForIdentityWithEmaillessUpn(requestContext, identityInDatabase, true);
            requestContext.TraceDataConditionally(15109118, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Attempted to reclaim invitation for persisted identity", (Func<object>) (() => (object) new
            {
              identityInDatabase = identityInDatabase,
              bindPendingIdentity = bindPendingIdentity
            }), nameof (UpdateClaims));
            if (flag2)
              PlatformClaimsUpdateService.FindAndTransferRightsFromBindPendingMSAIfApplicable(requestContext, identityInDatabase);
          }
        }
        requestContext.Trace(15109015, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "AuthenticationByIdentityProvider: {0}", (object) requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>("AuthenticationByIdentityProvider"));
        bool flag5 = !requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>("AuthenticationByIdentityProvider") && requestContext.ExecutionEnvironment.IsHostedDeployment;
        if (flag5)
        {
          HttpContextBase current = HttpContextFactory.Current;
          if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && AuthenticationHelpers.IsSignedInRequest(current.Request))
          {
            flag5 = false;
            requestContext.RootContext.Items["CredentialValidFrom"] = (object) DateTime.UtcNow;
          }
        }
        if (flag5)
        {
          requestContext.Trace(15109016, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Returning without updating identity - not authoritative claims.");
          return identityInDatabase ?? identityFromProvider;
        }
        bool flag6 = this.claimsComparer.SourceClaimsRequireRefresh(requestContext, identityInDatabase, identityFromProvider);
        Microsoft.VisualStudio.Services.Identity.Identity masterIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (!flag3 && identityInDatabase == null && !bindPendingIdentity)
        {
          masterIdentity = PlatformClaimsUpdateService.ReadIdentityByDomainAndOid(requestContext, identityFromProvider);
          if (masterIdentity != null)
          {
            flag4 = true;
            if (!IdentityDescriptorComparer.Instance.Equals(identityFromProvider.Descriptor, masterIdentity.Descriptor))
            {
              identityFromProvider.Id = masterIdentity.Id;
              identityFromProvider.MasterId = masterIdentity.MasterId;
              requestContext.TraceSerializedConditionally(15109029, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Updating UPN of identity: {0} that was found in the database (read using domain and oid combination) with the source identity: {1}", (object) masterIdentity.Descriptor, (object) identityFromProvider.Descriptor);
              if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && this.mungeSourceIdentitiesConfig.QueryByCtx<bool>(requestContext))
              {
                Microsoft.VisualStudio.Services.Identity.Identity localIdentity = PlatformClaimsUpdateService.GetLocalIdentity(requestContext, masterIdentity);
                if (localIdentity != null && localIdentity.GetProperty<string>("Account", string.Empty).Equals(identityFromProvider.GetProperty<string>("Account", string.Empty), StringComparison.OrdinalIgnoreCase))
                {
                  requestContext.Trace(15109034, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Munging local identity {0} as it matches the new upn of the master identity", (object) localIdentity.Id);
                  PlatformClaimsUpdateService.MungeIdentities(requestContext, new Microsoft.VisualStudio.Services.Identity.Identity[1]
                  {
                    localIdentity
                  }, "MAPPED");
                }
              }
            }
            else
              requestContext.TraceSerializedConditionally(15109028, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Identity from provider descriptor: {0} and databaseidentity descriptor: {1} are not the same in UPN rename flow", (object) identityFromProvider.Descriptor, (object) masterIdentity.Descriptor);
          }
        }
        DateTime property = identityFromProvider.GetProperty<DateTime>("MetadataUpdateDate", DateTime.MinValue);
        DateTime dateTime;
        requestContext.RootContext.TryGetItem<DateTime>("CredentialValidFrom", out dateTime);
        requestContext.Trace(15109015, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "claimsChanged: {0}; metadataUpdateDate: {1}; credentialValidFrom: {2}", (object) flag6, (object) property, (object) dateTime);
        bool flag7 = ((identityInDatabase == null ? 1 : (!flag6 ? 0 : (property < dateTime ? 1 : 0))) | (bindPendingIdentity ? 1 : 0)) != 0;
        if (identityInDatabase == null && masterIdentity == null && !bindPendingIdentity && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          flag7 = false;
        if (!flag7)
        {
          if (identityInDatabase == null && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          {
            if (identityFromProvider.IsMsaIdentity())
            {
              try
              {
                requestContext.Trace(15109032, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Trying to repair account name collision");
                ((IPlatformIdentityServiceInternal) requestContext.GetService<PlatformIdentityService>()).RepairAccountNameCollision(requestContext, identityFromProvider, "PuidChange");
                requestContext.Trace(15109033, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Tried to repair account name collision");
              }
              catch (IdentityAccountNameCollisionRepairUnsafeException ex)
              {
                requestContext.TraceException(325305, "Identity", nameof (PlatformClaimsUpdateService), (Exception) ex);
              }
              catch (IdentityAccountNameCollisionRepairFailedException ex)
              {
                requestContext.TraceException(879992, "Identity", nameof (PlatformClaimsUpdateService), (Exception) ex);
              }
            }
          }
          requestContext.Trace(15109016, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Returning without updating identity - identity did not need to be updated according to cache.");
          PlatformClaimsUpdateService.RaiseClaimsUpdateEventOnEnterprise(requestContext, identityInDatabase);
          return identityInDatabase ?? identityFromProvider;
        }
        if (property < dateTime)
          identityFromProvider.SetProperty("MetadataUpdateDate", (object) dateTime);
        requestContext.TraceSerializedConditionally(15109017, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Updating identity - {0}", (object) identityFromProvider);
        Microsoft.VisualStudio.Services.Identity.Identity identityInDatabase1 = PlatformClaimsUpdateService.CreateOrUpdateIdentityInDatabase(requestContext, identityFromProvider, ref identityInDatabase, bindPendingIdentity);
        PlatformClaimsUpdateService.RemoveBindPendingIfItExists(requestContext, identityFromProvider);
        if (flag4)
          PlatformClaimsUpdateService.InvalidateMembershipCacheForNewIdentity(requestContext, identityInDatabase1);
        PlatformClaimsUpdateService.RaiseClaimsUpdateEventOnEnterprise(requestContext, identityInDatabase1);
        return identityInDatabase1;
      }
      catch (IdentityAccountNameAlreadyInUseException ex)
      {
        requestContext.Trace(15109018, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Account Name Collision while updating claims for Identity [{0}] with ID: [{1}] and OID: [{2}]", (object) identityFromProvider.Descriptor, (object) identityFromProvider.Id, (object) identityFromProvider.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty)));
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15109019, "Identity", nameof (PlatformClaimsUpdateService), nameof (UpdateClaims));
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity HandleOidChangesForConflictingUpn(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      Microsoft.VisualStudio.Services.Identity.Identity identityInDatabase)
    {
      if (identityFromProvider != null && identityInDatabase != null && !identityFromProvider.IsMsaIdentity() && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        object a1 = (object) null;
        object b1 = (object) null;
        identityFromProvider.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out a1);
        identityInDatabase.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out b1);
        object a2;
        identityFromProvider.TryGetProperty("Domain", out a2);
        object b2;
        identityInDatabase.TryGetProperty("Domain", out b2);
        if (a2 != null && b2 != null && !string.Equals((string) a2, (string) b2, StringComparison.OrdinalIgnoreCase) || a1 == null || b1 == null || string.Equals((string) a1, (string) b1, StringComparison.OrdinalIgnoreCase))
          return identityInDatabase;
        requestContext.Trace(7689098, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Oid claims updated for UPN: {0}, Munging identity: {1}", (object) identityInDatabase.GetProperty<string>("Account", string.Empty), (object) identityInDatabase.Id));
        PlatformClaimsUpdateService.RemoveLicenses(requestContext, identityInDatabase);
        PlatformClaimsUpdateService.MungeIdentities(requestContext, new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          identityInDatabase
        }, "UpnReuse");
        identityInDatabase = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        identityInDatabase = PlatformClaimsUpdateService.CreateOrUpdateIdentityInDatabase(requestContext, identityFromProvider, ref identityInDatabase, false);
      }
      return identityInDatabase;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity HandleOidChangesForConflictingUpn2(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      Microsoft.VisualStudio.Services.Identity.Identity identityInDatabase)
    {
      if (identityFromProvider != null && identityInDatabase != null && !identityFromProvider.IsMsaIdentity())
      {
        object a1 = (object) null;
        object b1 = (object) null;
        identityFromProvider.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out a1);
        identityInDatabase.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out b1);
        object a2;
        identityFromProvider.TryGetProperty("Domain", out a2);
        object b2;
        identityInDatabase.TryGetProperty("Domain", out b2);
        if (a2 != null && b2 != null && !string.Equals((string) a2, (string) b2, StringComparison.OrdinalIgnoreCase) || a1 == null || b1 == null || string.Equals((string) a1, (string) b1, StringComparison.OrdinalIgnoreCase))
          return identityInDatabase;
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          requestContext.Trace(7689098, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Oid claims updated for UPN: {0}, Munging identity: {1}", (object) identityInDatabase.GetProperty<string>("Account", string.Empty), (object) identityInDatabase.Id));
          PlatformClaimsUpdateService.RemoveLicenses(requestContext, identityInDatabase);
          PlatformClaimsUpdateService.MungeIdentities(requestContext, new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identityInDatabase
          }, "UpnReuse");
          identityInDatabase = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          identityInDatabase = PlatformClaimsUpdateService.CreateOrUpdateIdentityInDatabase(requestContext, identityFromProvider, ref identityInDatabase, false);
        }
        else
          identityInDatabase = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      return identityInDatabase;
    }

    private static void RemoveLicenses(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identityInDatabase)
    {
      requestContext.GetService<ILicensingEntitlementService>().DeleteAccountEntitlement(requestContext, identityInDatabase.MasterId);
      requestContext.Trace(7689098, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Deleted Licenses for MasterId: {0}.", (object) identityInDatabase.MasterId));
    }

    private static void RemoveBindPendingIfItExists(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(15109051, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Request Context is at deployment. So return from this flow.");
      }
      else
      {
        string domain = identityFromProvider.GetProperty<string>("Domain", (string) null);
        string property = identityFromProvider.GetProperty<string>("Account", (string) null);
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        string resolvedWithMethod = (string) null;
        TeamFoundationHostType resolvedAtHostType = TeamFoundationHostType.Unknown;
        if (PlatformClaimsUpdateService.IsAadIdentityInMsa(requestContext, identityFromProvider))
          domain = "Windows Live ID";
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = PlatformClaimsUpdateService.GetBindPendingIdentity(requestContext, service, identity1, domain, property, ref resolvedWithMethod, ref resolvedAtHostType) ?? PlatformClaimsUpdateService.GetLegacyBindPendingIdentity(requestContext, service, identity1, property, ref resolvedWithMethod, ref resolvedAtHostType);
        if (identity2 == null)
          return;
        requestContext.GetService<ILicensingEntitlementService>().DeleteAccountEntitlement(requestContext, identity2.MasterId);
        requestContext.Trace(15109051, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Deleted Licenses for MasterId: {0}.", (object) identity2.MasterId));
        service.RemoveMemberFromGroup(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identity2.Descriptor);
        Microsoft.VisualStudio.Services.Identity.Identity identity3 = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          identity2.Descriptor
        }, QueryMembership.Direct, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity3 != null && identity3.MemberOf.Count > 0)
        {
          foreach (IdentityDescriptor groupDescriptor in (IEnumerable<IdentityDescriptor>) identity3.MemberOf)
            service.RemoveMemberFromGroup(requestContext, groupDescriptor, identity2.Descriptor);
          requestContext.Trace(15109051, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Deleted group memberships for the bind Pending Identity: {0}. Group Descriptors : {1}", (object) identity2.MasterId, (object) string.Join(", ", (IEnumerable<string>) identity3.MemberOf.Select<IdentityDescriptor, string>((Func<IdentityDescriptor, string>) (x => x.Identifier)).ToList<string>())));
        }
        PlatformClaimsUpdateService.MungeIdentities(requestContext, new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          identity2
        }, "BindPending");
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetLocalIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity masterIdentity)
    {
      Guid guid = requestContext.GetService<IIdentityIdTranslationService>().TranslateFromMasterId(requestContext, masterIdentity.MasterId);
      PlatformIdentityService service = requestContext.GetService<PlatformIdentityService>();
      if (guid == masterIdentity.MasterId)
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return service.ReadIdentitiesFromStore(requestContext, (IList<Guid>) new List<Guid>()
      {
        guid
      }, QueryMembership.None, (IEnumerable<string>) null, filterResults: false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private static bool MungeIdentities(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] identitiesToMunge,
      string reason)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identitiesToMunge).Select<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity =>
      {
        string property = identity.GetProperty<string>("Account", string.Empty);
        Guid id = identity.Id;
        requestContext.TraceAlways(15109051, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Before: Munging the bind pending identity with ID : [{0}].", (object) identity.Id));
        if (identity.IsMsaIdentity())
        {
          string[] strArray = property.Split('@');
          string str = string.Format("{0}.NOCONFLICT.{1}.{2}@{3}", (object) strArray[0], (object) reason, (object) id, (object) strArray?[1]);
          identity.SetProperty("Account", (object) str);
          string identifier = "upn:" + "Windows Live ID" + (object) '\\' + str;
          identity.Descriptor = new IdentityDescriptor("Microsoft.TeamFoundation.BindPendingIdentity", identifier);
        }
        else
        {
          identity.SetProperty("Account", (object) string.Format("OIDCONFLICT_{0}_{1}_{2}", (object) reason, (object) id, (object) property));
          string[] strArray = identity.Descriptor.Identifier.Split('\\');
          if (strArray.Length > 1)
          {
            string str1 = strArray[0];
            string str2 = strArray[1];
            identity.Descriptor.Identifier = string.Format("{0}\\OIDCONFLICT_{1}.{2}_{3}", (object) str1, (object) reason, (object) identity.Id, (object) str2);
          }
          else
            identity.Descriptor.Identifier = "OIDCONFLICT_" + reason + "." + identity.Descriptor.Identifier;
        }
        return identity;
      })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      return requestContext.GetService<IdentityService>().UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list, true);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity CreateOrUpdateIdentityInDatabase(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identityFromProvider,
      ref Microsoft.VisualStudio.Services.Identity.Identity identityInDatabase,
      bool bindPendingIdentity)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      if ((identityFromProvider.Descriptor.IsClaimsIdentityType() || identityFromProvider.Descriptor.IsCspPartnerIdentityType()) && identityFromProvider.GetProperty<string>("Account", string.Empty).Contains("\\"))
      {
        requestContext.Trace(15109018, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Identity update blocked - ClaimsIdentity AccountName contains slash.");
        requestContext.TraceSerializedConditionally(15109017, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Updating identity - {0} | RootContext Items: {2} | identityInDatabase: {3}", (object) identityFromProvider, (object) requestContext.RootContext.Items, (object) identityInDatabase);
        return identityInDatabase ?? identityFromProvider;
      }
      service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identityFromProvider
      }, true);
      if (identityFromProvider.IsCspPartnerUser)
        requestContext.TraceAlways(1012995, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), string.Format("Updated claims for CSP identity: {0}.", (object) identityFromProvider));
      if (identityInDatabase == null && !bindPendingIdentity)
      {
        identityInDatabase = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityFromProvider.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() ?? PlatformClaimsUpdateService.ReadIdentityAtDeploymentLevel(requestContext, identityFromProvider.Descriptor);
        if (identityInDatabase != null)
        {
          identityFromProvider.Id = identityInDatabase.Id;
          identityFromProvider.MasterId = identityInDatabase.MasterId;
        }
      }
      return identityFromProvider;
    }

    private static void InvalidateMembershipCacheForNewIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity newIdentity)
    {
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) || !(newIdentity?.Descriptor != (IdentityDescriptor) null))
        return;
      List<MembershipChangeInfo> membershipChangeInfoList = new List<MembershipChangeInfo>()
      {
        new MembershipChangeInfo()
        {
          MemberId = newIdentity.Id,
          MemberDescriptor = newIdentity.Descriptor,
          InvalidateStrongly = true
        }
      };
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IPlatformIdentityServiceInternal>().InvalidateMembershipsCache(vssRequestContext, (ICollection<MembershipChangeInfo>) membershipChangeInfoList);
    }

    private static void RaiseClaimsUpdateEventOnEnterprise(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        return;
      if (identity?.Descriptor == (IdentityDescriptor) null)
        return;
      try
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Application);
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext1, (object) new IdentityClaimsUpdateEvent()
        {
          IdentityDescriptor = identity.Descriptor
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15109024, "Identity", nameof (PlatformClaimsUpdateService), ex);
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOid(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity != null && identity.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.Ordinal))
      {
        string property1 = identity.GetProperty<string>("Domain", string.Empty);
        Guid property2 = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
        requestContext.TraceSerializedConditionally(15109026, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "The domain and oid for the identity: {0} is {1} and {2}", (object) identity, (object) property1, (object) property2);
        if (!string.IsNullOrEmpty(property1) && !property1.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase) && property2 != Guid.Empty)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = requestContext.GetService<IPlatformIdentityServiceInternal>().ReadIdentityByDomainAndOid(requestContext, property1, property2);
          requestContext.TraceSerializedConditionally(15109027, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "ReadIdentityByDomainAndOid returned the identity: {0} for domain: {1} and oid: {2}", (object) identity1, (object) property1, (object) property2);
          return identity1;
        }
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private static bool CheckForBindPendingReclamationForIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
      {
        requestContext.TraceDataConditionally(15109101, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Unexpected no-op since target identity is null", methodName: nameof (CheckForBindPendingReclamationForIdentity));
        return false;
      }
      PlatformClaimsUpdateService.LocalAndMasterIdentityId bindPendingId = PlatformClaimsUpdateService.FindBindPendingId(requestContext, identity, true, true);
      bool flag = bindPendingId != null;
      requestContext.TraceSerializedConditionally(15109014, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Bind Pending identity IDs: {0}", (object) bindPendingId);
      if (flag)
      {
        identity.Id = bindPendingId.LocalId;
        identity.MasterId = bindPendingId.MasterId;
      }
      return flag;
    }

    private static bool CheckForBindPendingReclamationForIdentityWithEmaillessUpn(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      bool isTargetIdentityPersisted)
    {
      requestContext.TraceEnter(15109130, "Identity", nameof (PlatformClaimsUpdateService), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
      try
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableCheckForBindPendingReclamationForIdentityWithEmaillessUpn"))
        {
          requestContext.TraceDataConditionally(15109131, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since feature is disabled", (Func<object>) (() => (object) new
          {
            feature = "VisualStudio.Services.Identity.DisableCheckForBindPendingReclamationForIdentityWithEmaillessUpn",
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          requestContext.TraceDataConditionally(15109132, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since request context is not collection level", (Func<object>) (() => (object) new
          {
            hostType = requestContext.ServiceHost.HostType,
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (targetIdentity == null)
        {
          requestContext.TraceDataConditionally(15109133, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Unexpected no op since input identity is null", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (!targetIdentity.IsClaims)
        {
          requestContext.TraceDataConditionally(15109134, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since input identity is not a claims identity", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, targetIdentity.Descriptor))
        {
          requestContext.TraceDataConditionally(15109135, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since input identity is a service principal", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (isTargetIdentityPersisted && targetIdentity.MasterId == new Guid())
        {
          requestContext.TraceDataConditionally(15109136, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Unexpected no op since input identity has invalid master ID", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (!targetIdentity.HasEmaillessUpn())
        {
          requestContext.TraceDataConditionally(15109137, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since input identity does not have an emailless UPN", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        string mailAddress = targetIdentity.GetProperty<string>("Mail", string.Empty);
        if (string.IsNullOrEmpty(mailAddress))
        {
          requestContext.TraceDataConditionally(15109138, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since input identity does not have a mail address", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        string property = targetIdentity.GetProperty<string>("Domain", string.Empty);
        if (string.IsNullOrEmpty(property))
        {
          requestContext.TraceDataConditionally(15109139, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Unexpected no op since input identity does not have a domain", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        if (service.IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, targetIdentity.Descriptor))
        {
          requestContext.TraceDataConditionally(15109140, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since input identity is already a member of the target scope", (Func<object>) (() => (object) new
          {
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        PlatformClaimsUpdateService.LocalAndMasterIdentityId sourceIdentityIds = (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        requestContext.TraceDataConditionally(15109141, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Looking for IDs for claims identity which has not yet signed in to transfer to user with emailless UPN", (Func<object>) (() => (object) new
        {
          mailAddress = mailAddress,
          targetIdentity = targetIdentity,
          isTargetIdentityPersisted = isTargetIdentityPersisted
        }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
        sourceIdentityIds = PlatformClaimsUpdateService.FindIdForClaimsUserWhoHasNotYetSignedIn(requestContext, property, mailAddress, nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
        if (sourceIdentityIds == null)
        {
          requestContext.TraceDataConditionally(15109142, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Looking for bind-pending identity IDs to transfer to user with emailless UPN", (Func<object>) (() => (object) new
          {
            mailAddress = mailAddress,
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          sourceIdentityIds = PlatformClaimsUpdateService.FindBindPendingId(requestContext, property, mailAddress, mailAddress, true, callerName: nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
        }
        if (sourceIdentityIds == null)
        {
          requestContext.TraceDataConditionally(15109143, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Could not find any valid source identity IDs to transfer to user with emailless UPN", (Func<object>) (() => (object) new
          {
            mailAddress = mailAddress,
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        requestContext.TraceDataConditionally(15109144, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Found source identity IDs to transfer to user with emailless UPN", (Func<object>) (() => (object) new
        {
          sourceIdentityIds = sourceIdentityIds,
          mailAddress = mailAddress,
          targetIdentity = targetIdentity,
          isTargetIdentityPersisted = isTargetIdentityPersisted
        }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
        if (sourceIdentityIds.LocalId == new Guid() || sourceIdentityIds.MasterId == new Guid())
        {
          requestContext.TraceDataConditionally(15109145, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Source identity IDs have invalid data", (Func<object>) (() => (object) new
          {
            sourceIdentityIds = sourceIdentityIds,
            mailAddress = mailAddress,
            targetIdentity = targetIdentity,
            isTargetIdentityPersisted = isTargetIdentityPersisted
          }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
          return false;
        }
        if (!isTargetIdentityPersisted)
        {
          service.UpdateIdentities(vssRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            targetIdentity
          }, true);
          if (targetIdentity.MasterId == new Guid())
          {
            requestContext.TraceDataConditionally(15109146, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Unexpected no op since input identity has invalid master ID", (Func<object>) (() => (object) new
            {
              targetIdentity = targetIdentity,
              isTargetIdentityPersisted = isTargetIdentityPersisted
            }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
            return false;
          }
        }
        IdentityRightsTransfer rightsTransfer = new IdentityRightsTransfer()
        {
          SourceId = sourceIdentityIds.LocalId,
          SourceMasterId = sourceIdentityIds.MasterId,
          TargetMasterId = targetIdentity.MasterId
        };
        requestContext.TraceDataConditionally(15109147, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Created rights transfer between source identity and user with emailless UPN", (Func<object>) (() => (object) new
        {
          rightsTransfer = rightsTransfer,
          sourceIdentityIds = sourceIdentityIds,
          mailAddress = mailAddress,
          targetIdentity = targetIdentity,
          isTargetIdentityPersisted = isTargetIdentityPersisted
        }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
        vssRequestContext.GetService<ITransferIdentityRightsService>().TransferIdentityRights(vssRequestContext, (IList<IdentityRightsTransfer>) new IdentityRightsTransfer[1]
        {
          rightsTransfer
        });
        targetIdentity.Id = rightsTransfer.SourceId;
        requestContext.TraceDataConditionally(15109148, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Completed rights transfer between source identity and user with emailless UPN", (Func<object>) (() => (object) new
        {
          rightsTransfer = rightsTransfer,
          sourceIdentityIds = sourceIdentityIds,
          mailAddress = mailAddress,
          targetIdentity = targetIdentity,
          isTargetIdentityPersisted = isTargetIdentityPersisted
        }), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
        return true;
      }
      finally
      {
        requestContext.TraceLeave(15109130, "Identity", nameof (PlatformClaimsUpdateService), nameof (CheckForBindPendingReclamationForIdentityWithEmaillessUpn));
      }
    }

    private static void FindAndTransferRightsFromBindPendingMSAIfApplicable(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.TraceEnter(15109150, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
      try
      {
        if (!PlatformClaimsUpdateService.IsAadIdentityInMsa(requestContext, identity))
        {
          requestContext.TraceDataConditionally(15109154, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "No op since this is not an AAD identity in MSA account scenario", (Func<object>) (() => (object) new
          {
            identity = identity
          }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
        }
        else
        {
          string accountName = identity.GetProperty<string>("Account", string.Empty);
          if (string.IsNullOrEmpty(accountName))
          {
            requestContext.TraceDataConditionally(15109155, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "No op since input identity account name is empty", (Func<object>) (() => (object) new
            {
              identity = identity
            }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
          }
          else
          {
            requestContext.TraceDataConditionally(15109156, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Looking for bind-pending identity IDs for user with given account name", (Func<object>) (() => (object) new
            {
              accountName = accountName,
              identity = identity
            }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
            PlatformClaimsUpdateService.LocalAndMasterIdentityId bindPendingIdentityIds = PlatformClaimsUpdateService.FindBindPendingId(requestContext, "Windows Live ID", accountName, accountName, true, true, nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
            if (bindPendingIdentityIds == null)
            {
              requestContext.TraceDataConditionally(15109157, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Could not find any active bind-pending identity IDs for user with given account name", (Func<object>) (() => (object) new
              {
                accountName = accountName,
                identity = identity
              }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
            }
            else
            {
              requestContext.TraceDataConditionally(15109143, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Found bind-pending identity IDs for user with given account name", (Func<object>) (() => (object) new
              {
                bindPendingIdentityIds = bindPendingIdentityIds,
                accountName = accountName,
                identity = identity
              }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
              if (bindPendingIdentityIds.LocalId == new Guid() || bindPendingIdentityIds.MasterId == new Guid())
              {
                requestContext.TraceDataConditionally(15109158, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Bind-pending identity IDs have invalid data", (Func<object>) (() => (object) new
                {
                  bindPendingIdentityIds = bindPendingIdentityIds,
                  accountName = accountName,
                  identity = identity
                }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
              }
              else
              {
                IdentityRightsTransfer rightsTransfer = new IdentityRightsTransfer()
                {
                  SourceId = bindPendingIdentityIds.LocalId,
                  SourceMasterId = bindPendingIdentityIds.MasterId,
                  TargetMasterId = identity.MasterId
                };
                requestContext.TraceDataConditionally(15109159, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Created rights transfer between bind-pending identity and user with given account name", (Func<object>) (() => (object) new
                {
                  rightsTransfer = rightsTransfer,
                  bindPendingIdentityIds = bindPendingIdentityIds,
                  accountName = accountName,
                  identity = identity
                }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
                IVssRequestContext vssRequestContext = requestContext.Elevate();
                vssRequestContext.GetService<ITransferIdentityRightsService>().TransferIdentityRights(vssRequestContext, (IList<IdentityRightsTransfer>) new IdentityRightsTransfer[1]
                {
                  rightsTransfer
                });
                requestContext.TraceDataConditionally(15109160, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Completed rights transfer between bind-pending identity and user with given account name", (Func<object>) (() => (object) new
                {
                  rightsTransfer = rightsTransfer,
                  bindPendingIdentityIds = bindPendingIdentityIds,
                  accountName = accountName,
                  identity = identity
                }), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
              }
            }
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(15109130, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindAndTransferRightsFromBindPendingMSAIfApplicable));
      }
    }

    private static bool IsAadIdentityInMsa(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.TraceDataConditionally(15109161, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Feature not applicable for deployment level requests", (Func<object>) (() => (object) new
        {
          identity = identity
        }), nameof (IsAadIdentityInMsa));
        return false;
      }
      Guid accountTenantId = requestContext.To(TeamFoundationHostType.Application).GetOrganizationAadTenantId();
      if (!(accountTenantId != Guid.Empty))
        return identity.Descriptor.IsAadUserType();
      requestContext.TraceDataConditionally(15109162, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Not an msa account", (Func<object>) (() => (object) new
      {
        accountTenantId = accountTenantId,
        identity = identity
      }), nameof (IsAadIdentityInMsa));
      return false;
    }

    private static PlatformClaimsUpdateService.LocalAndMasterIdentityId FindBindPendingId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      bool requireUserToExistInCurrentContext = false,
      bool considerActiveIdentityOnly = false)
    {
      requestContext.TraceEnter(15109020, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindBindPendingId));
      try
      {
        string domain = sourceIdentity.GetProperty<string>("Domain", (string) null);
        string accountName = sourceIdentity.GetProperty<string>("Account", (string) null);
        string emailAddress = sourceIdentity.GetProperty<string>("Mail", (string) null);
        if (PlatformClaimsUpdateService.IsAadIdentityInMsa(requestContext, sourceIdentity))
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DoNotBindAadGuests") && PlatformClaimsUpdateService.ReadIdentityAtDeploymentLevel(requestContext, sourceIdentity.Descriptor).MetaType == IdentityMetaType.Guest)
          {
            requestContext.TraceAlways(15109008, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Did not bind aad guest identity in MSA org.");
            return (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
          }
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DoNotBindAadGuestsGraph") && string.Equals(requestContext.GetExtension<IAadUserTypeExtension>(ExtensionLifetime.Service)?.QueryUserType(requestContext, sourceIdentity), "guest", StringComparison.OrdinalIgnoreCase))
          {
            requestContext.TraceAlways(15109045, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Did not bind AAD guest identity in MSA org (via MS Graph).");
            return (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
          }
          domain = "Windows Live ID";
        }
        requestContext.TraceDataConditionally(15109021, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Looking for bind-pending identity IDs", (Func<object>) (() => (object) new
        {
          domain = domain,
          accountName = accountName,
          emailAddress = emailAddress,
          sourceIdentity = sourceIdentity
        }), nameof (FindBindPendingId));
        PlatformClaimsUpdateService.LocalAndMasterIdentityId identityIds;
        if (sourceIdentity.SocialDescriptor != new SocialDescriptor())
        {
          identityIds = PlatformClaimsUpdateService.FindBindPendingGitHubId(requestContext, sourceIdentity);
          if (identityIds != null)
            PlatformClaimsUpdateService.s_GitHubBindPendingUpgradeCounter.Increment();
        }
        else
          identityIds = PlatformClaimsUpdateService.FindBindPendingId(requestContext, domain, accountName, emailAddress, requireUserToExistInCurrentContext, considerActiveIdentityOnly, nameof (FindBindPendingId));
        requestContext.TraceDataConditionally(15109029, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Returning bind-pending identity IDs", (Func<object>) (() => (object) new
        {
          identityIds = identityIds,
          domain = domain,
          accountName = accountName,
          emailAddress = emailAddress,
          sourceIdentity = sourceIdentity
        }), nameof (FindBindPendingId));
        return identityIds;
      }
      finally
      {
        requestContext.TraceLeave(15109020, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindBindPendingId));
      }
    }

    private static PlatformClaimsUpdateService.LocalAndMasterIdentityId FindBindPendingId(
      IVssRequestContext requestContext,
      string domain,
      string primaryBindPendingKey,
      string legacyBindPendingKey,
      bool requireUserToExistInCurrentContext = false,
      bool considerActiveIdentityOnly = false,
      [CallerMemberName] string callerName = null)
    {
      requestContext.TraceEnter(15109120, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindBindPendingId));
      try
      {
        requestContext.TraceDataConditionally(15109121, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Looking for bind-pending identity IDs", (Func<object>) (() => (object) new
        {
          domain = domain,
          primaryBindPendingKey = primaryBindPendingKey,
          legacyBindPendingKey = legacyBindPendingKey,
          requireUserToExistInCurrentContext = requireUserToExistInCurrentContext,
          callerName = callerName
        }), nameof (FindBindPendingId));
        requestContext = requestContext.Elevate();
        IdentityService service1 = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        PlatformClaimsUpdateService.LocalAndMasterIdentityId identityIds = (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        string resolvedWithMethod = (string) null;
        TeamFoundationHostType resolvedAtHostType = TeamFoundationHostType.Unknown;
        if (requireUserToExistInCurrentContext)
        {
          identity = PlatformClaimsUpdateService.GetBindPendingIdentity(requestContext, service1, identity, domain, primaryBindPendingKey, ref resolvedWithMethod, ref resolvedAtHostType) ?? PlatformClaimsUpdateService.GetLegacyBindPendingIdentity(requestContext, service1, identity, legacyBindPendingKey, ref resolvedWithMethod, ref resolvedAtHostType);
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
          IdentityService service2 = vssRequestContext.GetService<IdentityService>();
          identity = PlatformClaimsUpdateService.GetBindPendingIdentity(requestContext, service1, identity, domain, primaryBindPendingKey, ref resolvedWithMethod, ref resolvedAtHostType) ?? PlatformClaimsUpdateService.GetBindPendingIdentity(vssRequestContext, service2, identity, domain, primaryBindPendingKey, ref resolvedWithMethod, ref resolvedAtHostType) ?? PlatformClaimsUpdateService.GetLegacyBindPendingIdentity(requestContext, service1, identity, legacyBindPendingKey, ref resolvedWithMethod, ref resolvedAtHostType) ?? PlatformClaimsUpdateService.GetLegacyBindPendingIdentity(vssRequestContext, service2, identity, legacyBindPendingKey, ref resolvedWithMethod, ref resolvedAtHostType);
        }
        if (identity != null)
        {
          if (considerActiveIdentityOnly && !identity.IsActive)
            requestContext.TraceDataConditionally(619422, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Found inactive identity. Skipping it", (Func<object>) (() => (object) new
            {
              Id = identity.Id,
              resolvedWithMethod = resolvedWithMethod,
              resolvedAtHostType = resolvedAtHostType,
              domain = domain,
              primaryBindPendingKey = primaryBindPendingKey,
              legacyBindPendingKey = legacyBindPendingKey,
              requireUserToExistInCurrentContext = requireUserToExistInCurrentContext,
              callerName = callerName
            }), nameof (FindBindPendingId));
          else
            identityIds = new PlatformClaimsUpdateService.LocalAndMasterIdentityId()
            {
              LocalId = identity.Id,
              MasterId = identity.MasterId
            };
        }
        requestContext.TraceDataConditionally(15109129, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Returning bind-pending identity IDs", (Func<object>) (() => (object) new
        {
          identityIds = identityIds,
          resolvedWithMethod = resolvedWithMethod,
          resolvedAtHostType = resolvedAtHostType,
          domain = domain,
          primaryBindPendingKey = primaryBindPendingKey,
          legacyBindPendingKey = legacyBindPendingKey,
          requireUserToExistInCurrentContext = requireUserToExistInCurrentContext,
          callerName = callerName
        }), nameof (FindBindPendingId));
        return identityIds;
      }
      finally
      {
        requestContext.TraceLeave(15109120, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindBindPendingId));
      }
    }

    private static PlatformClaimsUpdateService.LocalAndMasterIdentityId FindIdForClaimsUserWhoHasNotYetSignedIn(
      IVssRequestContext requestContext,
      string domain,
      string principalName,
      [CallerMemberName] string callerName = null)
    {
      requestContext.TraceEnter(15109150, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
      try
      {
        requestContext.TraceDataConditionally(15109151, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Looking for IDs for claims identities who have not yet signed in", (Func<object>) (() => (object) new
        {
          domain = domain,
          principalName = principalName,
          callerName = callerName
        }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        string domainQualifiedUpn = string.Format("{0}\\{1}", (object) domain, (object) principalName);
        requestContext.TraceDataConditionally(15109152, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Searching for claims identity by domain-qualified UPN", (Func<object>) (() => (object) new
        {
          domainQualifiedUpn = domainQualifiedUpn,
          callerName = callerName
        }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> claimsIdentities = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, IdentitySearchFilter.AccountName, domainQualifiedUpn, QueryMembership.None, (IEnumerable<string>) null);
        int num = claimsIdentities.Count<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (num == 0)
        {
          requestContext.TraceDataConditionally(15109153, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Could not find claims identity by domain-qualified UPN; no result", (Func<object>) (() => (object) new
          {
            domainQualifiedUpn = domainQualifiedUpn,
            callerName = callerName
          }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
          return (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        }
        if (num > 1)
        {
          requestContext.TraceDataConditionally(15109153, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Found too many claims identities by domain-qualified UPN; no result", (Func<object>) (() => (object) new
          {
            claimsIdentities = claimsIdentities,
            domainQualifiedUpn = domainQualifiedUpn,
            callerName = callerName
          }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
          return (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        }
        Microsoft.VisualStudio.Services.Identity.Identity claimsIdentity = claimsIdentities.Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        requestContext.TraceDataConditionally(15109154, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Found claims identity by descriptor", (Func<object>) (() => (object) new
        {
          domainQualifiedUpn = domainQualifiedUpn,
          claimsIdentity = claimsIdentity,
          callerName = callerName
        }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
        IAuthenticationStateChecker extension = vssRequestContext.GetExtension<IAuthenticationStateChecker>();
        if (extension == null)
        {
          requestContext.TraceDataConditionally(15109155, TraceLevel.Error, "Identity", nameof (PlatformClaimsUpdateService), "Cannot check authentication state; excluding result", (Func<object>) (() => (object) new
          {
            domainQualifiedUpn = domainQualifiedUpn,
            claimsIdentity = claimsIdentity,
            callerName = callerName
          }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
          return (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        }
        bool hasCredentials = extension.HasAuthenticationCredentials(vssRequestContext, claimsIdentity);
        if (hasCredentials)
        {
          requestContext.TraceDataConditionally(15109156, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Claims identity has already signed in; excluding result", (Func<object>) (() => (object) new
          {
            claimsIdentity = claimsIdentity,
            hasCredentials = hasCredentials,
            callerName = callerName
          }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
          return (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        }
        requestContext.TraceDataConditionally(15109157, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Claims identity has not yet signed in; including result", (Func<object>) (() => (object) new
        {
          claimsIdentity = claimsIdentity,
          hasCredentials = hasCredentials,
          callerName = callerName
        }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
        PlatformClaimsUpdateService.LocalAndMasterIdentityId identityIds = new PlatformClaimsUpdateService.LocalAndMasterIdentityId()
        {
          LocalId = claimsIdentity.Id,
          MasterId = claimsIdentity.MasterId
        };
        requestContext.TraceDataConditionally(15109159, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Returning IDs for claims identity which has not signed in", (Func<object>) (() => (object) new
        {
          identityIds = identityIds,
          domain = domain,
          principalName = principalName,
          callerName = callerName
        }), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
        return identityIds;
      }
      finally
      {
        requestContext.TraceLeave(15109150, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindIdForClaimsUserWhoHasNotYetSignedIn));
      }
    }

    private static PlatformClaimsUpdateService.LocalAndMasterIdentityId FindBindPendingGitHubId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity)
    {
      requestContext.TraceEnter(15110150, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindBindPendingGitHubId));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        PlatformClaimsUpdateService.LocalAndMasterIdentityId bindPendingGitHubId = (PlatformClaimsUpdateService.LocalAndMasterIdentityId) null;
        SocialDescriptor socialDescriptor = sourceIdentity.SocialDescriptor;
        IdentityDescriptor pendingDescriptor = IdentityHelper.GetGitHubBindPendingDescriptor(sourceIdentity);
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          pendingDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          bindPendingGitHubId = new PlatformClaimsUpdateService.LocalAndMasterIdentityId()
          {
            LocalId = identity.Id,
            MasterId = identity.MasterId
          };
        return bindPendingGitHubId;
      }
      finally
      {
        requestContext.TraceLeave(15110150, "Identity", nameof (PlatformClaimsUpdateService), nameof (FindBindPendingGitHubId));
      }
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetBindPendingIdentity(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string domain,
      string accountName,
      ref string resolvedWithMethod,
      ref TeamFoundationHostType resolvedAtHostType)
    {
      if (identity == null && IdentityHelper.IsValidUserDomain(domain) && !string.IsNullOrWhiteSpace(accountName))
      {
        IdentityDescriptor descriptorFromAccountName = IdentityHelper.CreateDescriptorFromAccountName(domain, accountName, true);
        requestContext.TraceSerializedConditionally(15109022, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Searching for descriptor {0}", (object) descriptorFromAccountName);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptorFromAccountName
        }, QueryMembership.None, (IEnumerable<string>) null);
        requestContext.TraceSerializedConditionally(15109022, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Identities found by accountName: {0}", (object) source);
        identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
        {
          resolvedWithMethod = nameof (GetBindPendingIdentity);
          resolvedAtHostType = requestContext.ServiceHost.HostType;
        }
      }
      return identity;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetLegacyBindPendingIdentity(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string emailAddress,
      ref string resolvedWithMethod,
      ref TeamFoundationHostType resolvedAtHostType)
    {
      if (identity == null && !string.IsNullOrWhiteSpace(emailAddress))
      {
        IdentityDescriptor identityDescriptor = IdentityAuthenticationHelper.BuildTemporaryDescriptorFromEmailAddress(emailAddress);
        requestContext.TraceSerializedConditionally(15109023, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Searching for descriptor: {0}", (object) identityDescriptor);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null);
        requestContext.TraceSerializedConditionally(15109023, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "Identities found by email: {0}", (object) source);
        identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
        {
          resolvedWithMethod = nameof (GetLegacyBindPendingIdentity);
          resolvedAtHostType = requestContext.ServiceHost.HostType;
        }
      }
      return identity;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityWithDeploymentLevelFallback(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() ?? PlatformClaimsUpdateService.ReadIdentityAtDeploymentLevel(requestContext, descriptor);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityAtDeploymentLevel(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private void CheckPermissions(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<PlatformIdentityService>().CheckPermission(vssRequestContext, (string) null, 2, false);
    }

    internal virtual bool CheckForAlternateLoginIdentity(
      IVssRequestContext requestContext,
      IIdentity bclIdentity)
    {
      requestContext.TraceEnter(0, "Identity", nameof (PlatformClaimsUpdateService), nameof (CheckForAlternateLoginIdentity));
      if (bclIdentity == null)
        return false;
      bool flag = false;
      try
      {
        string fullName = bclIdentity.GetType().FullName;
        requestContext.RootContext.WebRequestContextInternal().SetAuthenticationType(bclIdentity.AuthenticationType);
        requestContext.Trace(15109025, TraceLevel.Verbose, "Identity", nameof (PlatformClaimsUpdateService), "AuthenticationType: {0}, identityType: {1}", (object) bclIdentity.AuthenticationType, (object) fullName);
        if (string.Equals(fullName, "Microsoft.VisualStudio.Services.Cloud.AlternateLoginIdentity", StringComparison.OrdinalIgnoreCase))
          flag = true;
      }
      finally
      {
        requestContext.TraceLeave(0, "Identity", nameof (PlatformClaimsUpdateService), nameof (CheckForAlternateLoginIdentity));
      }
      return flag;
    }

    private static bool IsCspPartnerUserMetaTypeChanged(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Microsoft.VisualStudio.Services.Identity.Identity databaseIdentity)
    {
      return (identity.IsCspPartnerUser || databaseIdentity.IsCspPartnerUser) && identity.MetaTypeId != databaseIdentity.MetaTypeId;
    }

    internal interface IIdentityClaimsComparer
    {
      bool SourceClaimsRequireRefresh(
        IVssRequestContext requestContext,
        Microsoft.VisualStudio.Services.Identity.Identity databaseIdentity,
        Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity);
    }

    internal class IdentityClaimsComparer : PlatformClaimsUpdateService.IIdentityClaimsComparer
    {
      public static PlatformClaimsUpdateService.IdentityClaimsComparer Instance { get; } = new PlatformClaimsUpdateService.IdentityClaimsComparer();

      public bool SourceClaimsRequireRefresh(
        IVssRequestContext requestContext,
        Microsoft.VisualStudio.Services.Identity.Identity databaseIdentity,
        Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity)
      {
        if (databaseIdentity == null)
        {
          requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "There is no identity in the database matching {0}", (object) sourceIdentity.Descriptor);
          return true;
        }
        if (!StringComparer.OrdinalIgnoreCase.Equals(sourceIdentity.ProviderDisplayName, databaseIdentity.ProviderDisplayName) && !string.IsNullOrEmpty(sourceIdentity.ProviderDisplayName))
        {
          requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "ProviderDisplayName is different Source:{0} Db:{1}", (object) sourceIdentity.ProviderDisplayName, (object) databaseIdentity.ProviderDisplayName);
          return true;
        }
        object a = (object) null;
        object b = (object) null;
        sourceIdentity.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out a);
        databaseIdentity.TryGetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", out b);
        if (!string.IsNullOrEmpty(a as string))
        {
          if (string.IsNullOrEmpty(b as string))
          {
            requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Identity object id found in source claims but does not exist in database. Refreshing database claims.");
            return true;
          }
          if (!string.Equals((string) a, (string) b, StringComparison.OrdinalIgnoreCase))
          {
            requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Identity object id found in source claims does not match object id in database. Refreshing database claims.");
            return true;
          }
        }
        if (sourceIdentity.SubjectDescriptor == new SubjectDescriptor() && sourceIdentity.Descriptor.IsCuidBased() && sourceIdentity.Cuid() != Guid.Empty)
          sourceIdentity.SubjectDescriptor = new SubjectDescriptor(sourceIdentity.GetSubjectTypeForCuidBasedIdentity(requestContext), sourceIdentity.Cuid().ToString("D"));
        SubjectDescriptor subjectDescriptor1 = sourceIdentity.SubjectDescriptor;
        SubjectDescriptor subjectDescriptor2 = databaseIdentity.SubjectDescriptor;
        if (subjectDescriptor1 != new SubjectDescriptor() && subjectDescriptor2 != new SubjectDescriptor() && subjectDescriptor1 != subjectDescriptor2)
        {
          requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Identity Subject Descriptor found in source claims does not match Subject Descriptor in database. Refreshing database claims.");
          return true;
        }
        if (PlatformClaimsUpdateService.IsCspPartnerUserMetaTypeChanged(sourceIdentity, databaseIdentity))
        {
          requestContext.Trace(15109040, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), string.Format("CSP partner user: {0}`s MetaType has changed from {1} to {2}, refreshing database claims.", (object) databaseIdentity, (object) databaseIdentity.MetaType, (object) sourceIdentity.MetaType));
          return true;
        }
        if (!object.Equals((object) sourceIdentity.SocialDescriptor, (object) databaseIdentity.SocialDescriptor))
        {
          requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "Identity social descriptor found in source claims does not match social descriptor in database. Refreshing database claims.");
          return true;
        }
        string property1 = databaseIdentity.GetProperty<string>("DirectoryAlias", string.Empty);
        string property2 = sourceIdentity.GetProperty<string>("DirectoryAlias", string.Empty);
        if (!string.IsNullOrEmpty(sourceIdentity.SocialDescriptor.Identifier) && !StringComparer.OrdinalIgnoreCase.Equals(property2, property1))
        {
          requestContext.Trace(999998, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "{0} is different Source:{1} Db:{2}", (object) "DirectoryAlias", (object) property2, (object) property1);
          return true;
        }
        foreach (string identityAttribute in PlatformClaimsUpdateService.IdentityAttributes)
        {
          string property3 = databaseIdentity.GetProperty<string>(identityAttribute, string.Empty);
          string property4 = sourceIdentity.GetProperty<string>(identityAttribute, string.Empty);
          if (!StringComparer.OrdinalIgnoreCase.Equals(property4, property3) && !string.IsNullOrEmpty(property4) && (identityAttribute != "PUID" || !sourceIdentity.IsExternalUser))
          {
            requestContext.Trace(999999, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "{0} is different Source:{1} Db:{2}", (object) identityAttribute, (object) property4, (object) property3);
            return true;
          }
        }
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.FillInMissingCuidOnSignIn"))
        {
          Guid cuid = databaseIdentity.GetProperty<Guid>("CUID", Guid.Empty);
          CuidState cuidState = databaseIdentity.GetProperty<CuidState>("CUIDState", CuidState.Unknown);
          if (cuidState != CuidState.RequiresPersist)
          {
            requestContext.TraceDataConditionally(15109043, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "CUID not marked for persistence (either already persisted or state unknown)", (Func<object>) (() => (object) new
            {
              id = databaseIdentity.Id,
              descriptor = databaseIdentity.Descriptor,
              puid = databaseIdentity.GetProperty<string>("PUID", string.Empty),
              oid = databaseIdentity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty),
              cuid = cuid,
              cuidState = cuidState
            }), nameof (SourceClaimsRequireRefresh));
          }
          else
          {
            requestContext.TraceDataConditionally(15109044, TraceLevel.Info, "Identity", nameof (PlatformClaimsUpdateService), "CUID marked for persistence", (Func<object>) (() => (object) new
            {
              id = databaseIdentity.Id,
              descriptor = databaseIdentity.Descriptor,
              puid = databaseIdentity.GetProperty<string>("PUID", string.Empty),
              oid = databaseIdentity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty),
              cuid = cuid,
              cuidState = cuidState
            }), nameof (SourceClaimsRequireRefresh));
            return true;
          }
        }
        return false;
      }
    }

    protected class LocalAndMasterIdentityId
    {
      public Guid LocalId;
      public Guid MasterId;
    }

    private static class Features
    {
      public const string FillInMissingCuidOnSignIn = "VisualStudio.Services.Identity.FillInMissingCuidOnSignIn";
      public const string DisableCheckForBindPendingReclamationForIdentityWithEmaillessUpn = "VisualStudio.Services.Identity.DisableCheckForBindPendingReclamationForIdentityWithEmaillessUpn";
      public const string DoNotBindAadGuests = "VisualStudio.Services.Identity.DoNotBindAadGuests";
      public const string DoNotBindAadGuestsGraph = "VisualStudio.Services.Identity.DoNotBindAadGuestsGraph";
    }
  }
}
