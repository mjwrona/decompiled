// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ConnectionDataController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Location", ResourceName = "ConnectionData")]
  public class ConnectionDataController : TfsApiController
  {
    [HttpGet]
    [PublicCollectionRequestRestrictions(false, true, null)]
    [ClientResponseType(typeof (ConnectionData), null, null)]
    public HttpResponseMessage GetConnectionData(
      ConnectOptions connectOptions = ConnectOptions.None,
      int lastChangeId = 0,
      long lastChangeId64 = 0)
    {
      this.ValidateOptions(connectOptions);
      SecuredLocationManager service1 = this.TfsRequestContext.GetService<SecuredLocationManager>();
      ILocationService service2 = this.TfsRequestContext.GetService<ILocationService>();
      service1.Connect(this.TfsRequestContext);
      IdentityDescriptor userContext = this.TfsRequestContext.UserContext;
      IdentityDescriptor authenticatedDescriptor = this.TfsRequestContext.GetAuthenticatedDescriptor();
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this.TfsRequestContext.GetAuthenticatedIdentity();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      if (authenticatedIdentity == null)
        throw new IdentityNotFoundException(authenticatedDescriptor);
      if (userIdentity == null)
        throw new IdentityNotFoundException(userContext);
      LocationServiceData locationServiceData;
      if (connectOptions.HasFlag((Enum) ConnectOptions.IncludeServices))
      {
        locationServiceData = this.GetLocationServiceData(this.TfsRequestContext, lastChangeId64 != 0L ? lastChangeId64 : (long) lastChangeId, connectOptions);
      }
      else
      {
        long lastChangeId1 = service1.GetLastChangeId(this.TfsRequestContext);
        AccessMapping defaultAccessMapping = service2.GetDefaultAccessMapping(this.TfsRequestContext);
        locationServiceData = new LocationServiceData()
        {
          ServiceOwner = service2.InstanceType,
          LastChangeId = (int) lastChangeId1,
          LastChangeId64 = lastChangeId1,
          ServiceDefinitions = (ICollection<ServiceDefinition>) null,
          AccessMappings = (ICollection<AccessMapping>) null,
          DefaultAccessMappingMoniker = defaultAccessMapping?.Moniker,
          ClientCacheFresh = (long) lastChangeId == lastChangeId1,
          ClientCacheTimeToLive = LocationServiceHelper.GetClientCacheTimeToLive(this.TfsRequestContext)
        };
      }
      IdentityHelper.ScrubMasterId(authenticatedIdentity);
      IdentityHelper.ScrubMasterId(userIdentity);
      ConnectionData connectionData;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.ConnectionData.UseLegacyIdentity"))
        connectionData = new ConnectionData()
        {
          AuthenticatedUser = authenticatedIdentity,
          AuthorizedUser = userIdentity
        };
      else
        connectionData = (ConnectionData) new ConnectionDataController.PublicConnectionData()
        {
          AuthenticatedUser = new ConnectionDataController.ConnectionDataIdentity(authenticatedIdentity),
          AuthorizedUser = new ConnectionDataController.ConnectionDataIdentity(userIdentity)
        };
      connectionData.LocationServiceData = locationServiceData;
      connectionData.InstanceId = this.TfsRequestContext.ServiceHost.InstanceId;
      connectionData.DeploymentId = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId;
      connectionData.LastUserAccess = connectOptions.HasFlag((Enum) ConnectOptions.IncludeLastUserAccess) ? this.GetLastUserAccess() : new DateTime?();
      connectionData.WebApplicationRelativeDirectory = LocationServiceHelper.GetWebApplicationRelativeDirectory(this.TfsRequestContext);
      connectionData.DeploymentType = this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? DeploymentFlags.Hosted : DeploymentFlags.OnPremises;
      return this.Request.CreateResponse<ConnectionData>(connectionData);
    }

    private void ValidateOptions(ConnectOptions options)
    {
      if (options.HasFlag((Enum) ConnectOptions.IncludeInheritedDefinitionsOnly) && options.HasFlag((Enum) ConnectOptions.IncludeNonInheritedDefinitionsOnly))
        throw new ArgumentException("IncludeInheritedDefinitionsOnly and IncludeNonInheritedDefinitionsOnly are mutually exclusive", nameof (options));
      if (options.HasFlag((Enum) ConnectOptions.IncludeInheritedDefinitionsOnly))
      {
        this.TfsRequestContext.CheckDeploymentRequestContext();
        if (!options.HasFlag((Enum) ConnectOptions.IncludeServices))
          throw new ArgumentException("You must provide IncludeServices when specifying IncludeInheritedDefinitionsOnly.", nameof (options));
      }
      if (!options.HasFlag((Enum) ConnectOptions.IncludeNonInheritedDefinitionsOnly))
        return;
      if (this.TfsRequestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(this.TfsRequestContext.ServiceHost.HostType);
      if (!options.HasFlag((Enum) ConnectOptions.IncludeServices))
        throw new ArgumentException("You must provide IncludeServices when specifying IncludeNonInheritedDefinitionsOnly.", nameof (options));
    }

    internal DateTime? GetLastUserAccess()
    {
      this.TfsRequestContext.RootContext.Items[RequestContextItemsKeys.SkipHostLastUserAccessUpdate] = (object) true;
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      HostProperties hostProperties = service.QueryServiceHostPropertiesCached(vssRequestContext, this.TfsRequestContext.ServiceHost.InstanceId);
      if (hostProperties == null)
        return new DateTime?();
      if (!this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && hostProperties.LastUserAccess < DateTime.UtcNow.AddHours(-12.0))
        hostProperties = (HostProperties) service.QueryServiceHostProperties(vssRequestContext, this.TfsRequestContext.ServiceHost.InstanceId);
      return hostProperties?.LastUserAccess;
    }

    private LocationServiceData GetLocationServiceData(
      IVssRequestContext requestContext,
      long lastChangeId,
      ConnectOptions options)
    {
      SecuredLocationManager service1 = requestContext.GetService<SecuredLocationManager>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      ICollection<AccessMapping> accessMappings = (ICollection<AccessMapping>) new List<AccessMapping>(service1.GetAccessMappings(requestContext));
      long lastChangeId1 = service1.GetLastChangeId(requestContext);
      AccessMapping defaultAccessMapping = service2.GetDefaultAccessMapping(this.TfsRequestContext);
      if (lastChangeId == lastChangeId1)
        return new LocationServiceData()
        {
          ServiceOwner = service2.InstanceType,
          LastChangeId = (int) lastChangeId1,
          LastChangeId64 = lastChangeId1,
          ServiceDefinitions = (ICollection<ServiceDefinition>) null,
          AccessMappings = accessMappings,
          DefaultAccessMappingMoniker = defaultAccessMapping?.Moniker,
          ClientCacheFresh = true,
          ClientCacheTimeToLive = LocationServiceHelper.GetClientCacheTimeToLive(this.TfsRequestContext)
        };
      IEnumerable<ServiceDefinition> serviceDefinitions;
      if (options.HasFlag((Enum) ConnectOptions.IncludeInheritedDefinitionsOnly))
        serviceDefinitions = requestContext.GetService<InheritedLocationDataService>().GetData(requestContext, Guid.Empty, TeamFoundationHostType.All).GetAllServiceDefinitions();
      else if (options.HasFlag((Enum) ConnectOptions.IncludeNonInheritedDefinitionsOnly))
      {
        serviceDefinitions = service1.QueryNonInheritedServiceDefinitions(requestContext);
      }
      else
      {
        serviceDefinitions = service1.QueryServices(requestContext);
        if (LocationServiceHelper.ShouldRemoveApplicationDefinitionForDev12(requestContext))
          serviceDefinitions = serviceDefinitions.Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (def => def.Identifier != LocationServiceConstants.ApplicationIdentifier));
      }
      return new LocationServiceData()
      {
        ServiceOwner = service2.InstanceType,
        LastChangeId = (int) lastChangeId1,
        LastChangeId64 = lastChangeId1,
        ServiceDefinitions = (ICollection<ServiceDefinition>) new List<ServiceDefinition>(serviceDefinitions),
        AccessMappings = accessMappings,
        DefaultAccessMappingMoniker = defaultAccessMapping?.Moniker,
        ClientCacheFresh = false,
        ClientCacheTimeToLive = LocationServiceHelper.GetClientCacheTimeToLive(this.TfsRequestContext)
      };
    }

    public override string TraceArea => "LocationService";

    public override string ActivityLogArea => "Framework";

    [DataContract]
    private class PublicConnectionData : ConnectionData
    {
      [DataMember(IsRequired = false, EmitDefaultValue = false)]
      public ConnectionDataController.ConnectionDataIdentity AuthenticatedUser { get; set; }

      [DataMember(IsRequired = false, EmitDefaultValue = false)]
      public ConnectionDataController.ConnectionDataIdentity AuthorizedUser { get; set; }
    }

    [DataContract]
    private class ConnectionDataIdentity : IdentityBase, ISecuredObject
    {
      private static readonly string[] s_preservedProperties = new string[1]
      {
        "Account"
      };

      public ConnectionDataIdentity()
        : base((PropertiesCollection) null)
      {
      }

      public ConnectionDataIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
        : base((PropertiesCollection) null)
      {
        this.Id = identity.Id;
        this.Descriptor = new IdentityDescriptor(identity.Descriptor);
        this.SubjectDescriptor = identity.SubjectDescriptor;
        this.SocialDescriptor = identity.SocialDescriptor;
        this.ProviderDisplayName = identity.ProviderDisplayName;
        this.CustomDisplayName = identity.CustomDisplayName;
        this.IsActive = identity.IsActive;
        this.UniqueUserId = identity.UniqueUserId;
        this.IsContainer = identity.IsContainer;
        this.ResourceVersion = identity.ResourceVersion;
        this.MetaTypeId = identity.MetaTypeId;
        identity.ValidateProperties = false;
        foreach (string preservedProperty in ConnectionDataController.ConnectionDataIdentity.s_preservedProperties)
        {
          object obj;
          if (identity.Properties.TryGetValue(preservedProperty, out obj))
            this.Properties.Add(preservedProperty, obj);
        }
      }

      Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

      int ISecuredObject.RequiredPermissions => 1;

      string ISecuredObject.GetToken() => LocationSecurityConstants.NamespaceRootToken;
    }
  }
}
