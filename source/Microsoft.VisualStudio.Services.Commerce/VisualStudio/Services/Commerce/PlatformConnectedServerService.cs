// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformConnectedServerService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformConnectedServerService : IConnectedServerService, IVssFrameworkService
  {
    private const string HiddenCollectionRegionPath = "/Service/Commerce/Commerce/HiddenCollectionRegion";
    private const string HiddenCollectionRegionDefault = "SCUS";
    private const string Area = "Commerce";
    private const string Layer = "PlatformConnectedServerService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public ConnectedServer Create(IVssRequestContext requestContext, ConnectedServer server)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ConnectedServer>(server, nameof (server));
      ArgumentUtility.CheckForEmptyGuid(server.SubscriptionId, "SubscriptionId");
      ArgumentUtility.CheckForEmptyGuid(server.TargetId, "TargetId");
      ArgumentUtility.CheckForEmptyGuid(server.ServerId, "ServerId");
      ArgumentUtility.CheckForNull<ConnectedServerAuthorization>(server.Authorization, "Authorization");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(server.Authorization.PublicKey, "PublicKey");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(server.ServerName, "ServerName");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(server.TargetName, "TargetName");
      ConnectedServerPublicKey publicKey = this.DecodePublicKey(server);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      try
      {
        requestContext.TraceEnter(5104500, "Commerce", nameof (PlatformConnectedServerService), new object[1]
        {
          (object) server
        }, nameof (Create));
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.BypassConnectedServerSubscriptionCheck"))
        {
          IAzureResourceHelper service = requestContext.GetService<IAzureResourceHelper>();
          if (!service.IsValidAadUser(requestContext))
          {
            requestContext.Trace(5104518, TraceLevel.Error, "Commerce", nameof (PlatformConnectedServerService), string.Format("User with CUID {0} is not a valid AAD user.", (object) requestContext.GetUserCuid()));
            throw new AadAuthorizationException();
          }
          if (service.GetSubscriptionAuthorization(requestContext, server.SubscriptionId) == SubscriptionAuthorizationSource.Unauthorized)
          {
            requestContext.Trace(5104519, TraceLevel.Error, "Commerce", nameof (PlatformConnectedServerService), string.Format("User with CUID {0} is not authorized to make purchases for subscription {1}.", (object) requestContext.GetUserCuid(), (object) server.SubscriptionId));
            throw new UserIsNotSubscriptionAdminException();
          }
        }
        Guid infrastructureCollection = this.GetOrCreateInfrastructureCollection(requestContext, server);
        try
        {
          ISubscriptionService service = requestContext.GetService<ISubscriptionService>();
          requestContext.Trace(5104503, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Linking collection: {0} to Subscription {1}", (object) infrastructureCollection, (object) server.SubscriptionId));
          requestContext.GetService<IArmAdapterService>().RegisterSubscriptionAgainstResourceProvider(requestContext, server.SubscriptionId);
          IVssRequestContext requestContext1 = vssRequestContext;
          Guid subscriptionId = server.SubscriptionId;
          Guid collectionId = infrastructureCollection;
          Guid? ownerId = new Guid?(CommerceConstants.SpsMasterId);
          service.LinkCollection(requestContext1, subscriptionId, AccountProviderNamespace.OnPremise, collectionId, ownerId);
          requestContext.Trace(5104514, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Linking collection completed: {0} to Subscription {1}", (object) infrastructureCollection, (object) server.SubscriptionId));
        }
        catch (AccountAlreadyLinkedException ex)
        {
          requestContext.Trace(5104522, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("The collection {0} is already linked to subscription {1}", (object) infrastructureCollection, (object) server.SubscriptionId));
        }
        return this.Connect(requestContext, server, infrastructureCollection, publicKey);
      }
      finally
      {
        requestContext.TraceLeave(5104501, "Commerce", nameof (PlatformConnectedServerService), nameof (Create));
      }
    }

    internal virtual ConnectedServer Connect(
      IVssRequestContext requestContext,
      ConnectedServer server,
      Guid organizationId,
      ConnectedServerPublicKey publicKey)
    {
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        requestContext.GetService<CommerceHostManagementService>().EnsureHostUpdated(requestContext, organizationId);
        using (IVssRequestContext requestContext1 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, organizationId, RequestContextType.SystemContext))
        {
          requestContext1.TraceEnter(5104505, "Commerce", nameof (PlatformConnectedServerService), new object[3]
          {
            (object) server,
            (object) organizationId,
            (object) publicKey
          }, nameof (Connect));
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ProvisionServiceIdentity(requestContext1, server);
          Registration registration1 = new Registration()
          {
            ClientType = ClientType.MediumTrust,
            IdentityId = identity.Id,
            IsValid = true,
            PublicKey = publicKey.ToXmlString(),
            RegistrationId = server.Authorization.ClientId,
            RegistrationName = string.Format("Connected Server {0} - Host {1}", (object) server.ServerId, (object) server.TargetId),
            Scopes = "vso.connected_server"
          };
          IVssRequestContext vssRequestContext1 = requestContext1.Elevate();
          requestContext.Trace(5104515, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), "Creating registration");
          IDelegatedAuthorizationRegistrationService service = vssRequestContext1.GetService<IDelegatedAuthorizationRegistrationService>();
          try
          {
            Registration registration2 = service.Create(vssRequestContext1, registration1);
            requestContext.Trace(5104516, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Authorizing host for registration id: {0}", (object) registration2.RegistrationId));
          }
          catch (RegistrationAlreadyExistsException ex)
          {
            requestContext.Trace(5104520, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Registration with id {0} already created", (object) registration1.RegistrationId));
          }
          IVssRequestContext vssRequestContext2 = requestContext1.Elevate();
          vssRequestContext2.GetService<IDelegatedAuthorizationService>().AuthorizeHost(vssRequestContext2, registration1.RegistrationId);
          requestContext.Trace(5104517, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Authorizing host for registration id succeeded: {0}", (object) registration1.RegistrationId));
          server.Authorization.AuthorizationUrl = this.GetAuthorizationUrl(requestContext1);
          return server;
        }
      }
      finally
      {
        requestContext.TraceLeave(5104506, "Commerce", nameof (PlatformConnectedServerService), nameof (Connect));
      }
    }

    internal virtual Guid GetOrCreateInfrastructureCollection(
      IVssRequestContext requestContext,
      ConnectedServer server)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.Elevate().GetService<ISubscriptionService>();
      Guid guid = server.TargetId;
      string upper = guid.ToString("N").ToUpper();
      guid = server.SubscriptionId;
      string str = guid.GetHashCode().ToString("G");
      string hostName = "OPS-" + upper + "-" + str;
      Guid? nullable = InfrastructureHostHelper.ResolveCollectionHostName(vssRequestContext, hostName);
      if (!nullable.HasValue)
      {
        nullable = new Guid?(PlatformConnectedServerService.CreateInfrastructureCollection(server, vssRequestContext, hostName));
        requestContext.Trace(5104524, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Created infrastructure collection {0} (collectionId: {1}).", (object) hostName, (object) nullable.Value));
      }
      else
        requestContext.Trace(5104523, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Name resolution entries for {0} (collectionId: {1}) already existed.", (object) hostName, (object) nullable.Value));
      server.AccountName = hostName;
      server.AccountId = nullable.Value;
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, nullable.Value);
      server.SpsUrl = hostUri.ToString();
      return nullable.Value;
    }

    private static Guid CreateInfrastructureCollection(
      ConnectedServer server,
      IVssRequestContext deploymentContext,
      string hostName)
    {
      string hostRegion = deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) "/Service/Commerce/Commerce/HiddenCollectionRegion", "SCUS");
      ServiceHostTags serviceHostTags = new ServiceHostTags();
      serviceHostTags.AddTag(WellKnownServiceHostTags.IsInfrastructureHost);
      serviceHostTags.AddTag(WellKnownServiceHostTags.NoOrgMetadata);
      serviceHostTags.AddTag(CommerceWellKnownServiceHostTags.AssociatedWithOnPremisesCollection);
      Guid guid = Guid.Empty;
      try
      {
        guid = InfrastructureHostHelper.CreateInfrastructureOrganization(deploymentContext, hostName, hostName, hostRegion, serviceHostTags);
        using (CommerceVssRequestContextExtensions.VssRequestContextHolder organization = deploymentContext.ToOrganization(guid))
        {
          IVssRequestContext requestContext = organization.RequestContext;
          CommercePropertyStore commercePropertyStore = new CommercePropertyStore();
          PropertiesCollection properties = new PropertiesCollection()
          {
            [ConnectedServerConstants.ConnectedServerNameProperty] = (object) server.ServerName,
            [ConnectedServerConstants.ConnectedServerHostNameProperty] = (object) server.TargetName
          };
          commercePropertyStore.CreatePropertyKind(requestContext, CommerceConstants.ConnectedServerPropertyKind, string.Format("ConnectedServer.{0}", (object) guid), internalKind: true);
          commercePropertyStore.UpdateProperties(requestContext, CommerceConstants.ConnectedServerPropertyKind, properties);
        }
        Guid collectionId;
        if (!HostNameResolver.TryGetCollectionServiceHostId(deploymentContext, hostName, out collectionId))
          throw new InvalidOperationException(string.Format("Unable to find collection host for newly created infrastructure host {0} on subscription {1}.", (object) guid, (object) server.SubscriptionId));
        return collectionId;
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5108926, "Commerce", nameof (PlatformConnectedServerService), ex);
        if (guid != Guid.Empty)
          deploymentContext.GetService<IHostDeletionService>().DeleteHost(deploymentContext, guid, DeleteHostResourceOptions.MarkForDeletion, HostDeletionReason.HostDeleted);
        throw;
      }
    }

    internal virtual ConnectedServerPublicKey DecodePublicKey(ConnectedServer server)
    {
      Dictionary<string, string> dictionary = CloudConnectedUtilities.DecodeToken(server.Authorization.PublicKey);
      string str1;
      if (!dictionary.TryGetValue(CloudConnectedServerShortNameConstants.Exponent, out str1))
        dictionary.TryGetValue(CloudConnectedServerConstants.Exponent, out str1);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(str1, "exponent");
      string str2;
      if (!dictionary.TryGetValue(CloudConnectedServerShortNameConstants.Modulus, out str2))
        dictionary.TryGetValue(CloudConnectedServerConstants.Modulus, out str2);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(str2, "modulus");
      return new ConnectedServerPublicKey(Convert.FromBase64String(str1), Convert.FromBase64String(str2));
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ProvisionServiceIdentity(
      IVssRequestContext requestContext,
      ConnectedServer server)
    {
      string identifier = server.TargetId.ToString("D");
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(vssRequestContext, FrameworkIdentityType.ServiceIdentity, "ConnectedServer", identifier, true);
      if (frameworkIdentity != null)
      {
        vssRequestContext.Trace(5104507, TraceLevel.Verbose, "Commerce", nameof (PlatformConnectedServerService), string.Format("Found service identity with ID {0}", (object) frameworkIdentity.Id));
        if (!frameworkIdentity.IsActive)
        {
          vssRequestContext.Trace(5104508, TraceLevel.Error, "Commerce", nameof (PlatformConnectedServerService), string.Format("Service identity with ID {0} is not active in scope {1}", (object) frameworkIdentity.Id, (object) vssRequestContext.ServiceHost.InstanceId));
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.GetIdentity(vssRequestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup);
          if (identity != null)
          {
            if (service.AddMemberToGroup(vssRequestContext, identity.Descriptor, frameworkIdentity))
              vssRequestContext.Trace(5104509, TraceLevel.Error, "Commerce", nameof (PlatformConnectedServerService), string.Format("Service identity with ID {0} has been successfully added to security service group {1}", (object) frameworkIdentity.Id, (object) identity.Id));
            else
              vssRequestContext.Trace(5104510, TraceLevel.Error, "Commerce", nameof (PlatformConnectedServerService), string.Format("Service identity with ID {0} is marked inactive but was not added to security service group {1}", (object) frameworkIdentity.Id, (object) identity.Id));
          }
        }
      }
      else
      {
        vssRequestContext.Trace(5104511, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), "Service identity with ID " + identifier + " was not found. Provisioning a new service identity.");
        string displayName = string.Format("Connected Server Service ({0})", (object) server.TargetId);
        frameworkIdentity = service.CreateFrameworkIdentity(vssRequestContext.Elevate(), FrameworkIdentityType.ServiceIdentity, "ConnectedServer", identifier, displayName);
        if (frameworkIdentity != null)
          vssRequestContext.Trace(5104512, TraceLevel.Info, "Commerce", nameof (PlatformConnectedServerService), string.Format("Successfully provisioned service identity {0} with VSID {1}", (object) frameworkIdentity.DisplayName, (object) frameworkIdentity.Id));
      }
      IVssRequestContext rootContext = requestContext.RootContext;
      rootContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(rootContext, FrameworkSecurity.FrameworkNamespaceId).SetPermissions(rootContext, FrameworkSecurity.FrameworkNamespaceToken, frameworkIdentity.Descriptor, 1, 0, false);
      return frameworkIdentity;
    }

    internal virtual Uri GetAuthorizationUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationData(requestContext, new Guid("585028FE-17D8-49E2-9A1B-EFB4D8502156")).GetResourceUri(requestContext, "oauth2", OAuth2ResourceIds.Token, (object) null, false);

    internal virtual Guid GetUserId(IVssRequestContext requestContext) => requestContext.GetUserId();

    private Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor identifier,
      QueryMembership queryMembership = QueryMembership.None)
    {
      return this.GetIdentity(requestContext, requestContext.ServiceHost.InstanceId, identifier, queryMembership);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      Guid scopeId,
      IdentityDescriptor identifier,
      QueryMembership queryMembership = QueryMembership.None)
    {
      return this.GetIdentities(requestContext, scopeId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identifier
      }, queryMembership).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext requestContext,
      Guid scopeId,
      IEnumerable<IdentityDescriptor> identifiers,
      QueryMembership queryMembership = QueryMembership.None)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<IdentityDescriptor> descriptors = (IList<IdentityDescriptor>) new List<IdentityDescriptor>();
      foreach (IdentityDescriptor identifier in identifiers)
        descriptors.Add(IdentityDomain.MapFromWellKnownIdentifier(scopeId, identifier));
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, descriptors, queryMembership, (IEnumerable<string>) null);
    }
  }
}
