// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsTeamProjectCollection
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Proxy;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;

namespace Microsoft.TeamFoundation.Client
{
  public class TfsTeamProjectCollection : TfsConnection
  {
    private RegistrationService m_registrationProxy;
    private TfsConfigurationServer m_configurationServer;
    private object m_configurationServerLockObject = new object();
    private string m_name;
    private string m_displayName;
    private CatalogNode m_catalogNode;
    private object m_entityLockObject = new object();

    public TfsTeamProjectCollection(Uri uri)
      : this(uri, false)
    {
    }

    public TfsTeamProjectCollection(RegisteredProjectCollection projectCollection)
      : this(projectCollection, (IdentityDescriptor) null)
    {
    }

    public TfsTeamProjectCollection(Uri uri, VssCredentials credentials)
      : base(uri, credentials, (IdentityDescriptor) null, LocationServiceConstants.CollectionLocationServiceRelativePath, (ITfsRequestChannelFactory) null)
    {
    }

    public TfsTeamProjectCollection(Uri uri, IdentityDescriptor identityToImpersonate)
      : base(uri, LocationServiceConstants.CollectionLocationServiceRelativePath, identityToImpersonate, (ITfsRequestChannelFactory) null)
    {
    }

    public TfsTeamProjectCollection(
      RegisteredProjectCollection projectCollection,
      IdentityDescriptor identityToImpersonate)
      : this(TfsTeamProjectCollection.GetFullyQualifiedUriForName(projectCollection.Name), identityToImpersonate)
    {
    }

    public TfsTeamProjectCollection(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate)
      : this(uri, credentials, identityToImpersonate, (ITfsRequestChannelFactory) null)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsTeamProjectCollection(Uri uri, bool fromFactory)
      : base(uri, LocationServiceConstants.CollectionLocationServiceRelativePath)
    {
      this.UseFactory = fromFactory;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsTeamProjectCollection(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      bool fromFactory)
      : this(uri, credentials, identityToImpersonate, (ITfsRequestChannelFactory) null, fromFactory)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsTeamProjectCollection(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      ITfsRequestChannelFactory channelFactory)
      : this(uri, credentials, identityToImpersonate, channelFactory, false)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsTeamProjectCollection(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      ITfsRequestChannelFactory channelFactory,
      bool fromFactory)
      : base(uri, credentials, identityToImpersonate, LocationServiceConstants.CollectionLocationServiceRelativePath, channelFactory)
    {
      this.UseFactory = fromFactory;
    }

    [Obsolete("This constructor is obsolete and will be removed in a future version. See TfsTeamProjectCollection.TfsTeamProjectCollection(Uri, VssCredentials) instead", false)]
    public TfsTeamProjectCollection(Uri uri, ICredentials credentials)
      : base(uri, credentials, LocationServiceConstants.CollectionLocationServiceRelativePath)
    {
    }

    public override string Name
    {
      get
      {
        if (this.m_name == null)
        {
          try
          {
            RegisteredProjectCollection projectCollection = RegisteredTfsConnections.GetProjectCollection(this.Uri);
            if (projectCollection != null)
              this.m_name = projectCollection.Name;
          }
          catch (Exception ex)
          {
          }
          if (this.m_name == null)
            return base.Name;
        }
        return this.m_name;
      }
    }

    public string DisplayName
    {
      get
      {
        if (this.m_displayName == null)
        {
          try
          {
            RegisteredProjectCollection projectCollection = RegisteredTfsConnections.GetProjectCollection(this.Uri);
            if (projectCollection != null)
              this.m_displayName = projectCollection.DisplayName;
          }
          catch (Exception ex)
          {
          }
          if (this.m_displayName == null)
            return base.Name;
        }
        return this.m_displayName;
      }
    }

    public override CatalogNode CatalogNode
    {
      get
      {
        if (this.m_catalogNode == null && this.CatalogResourceId != Guid.Empty)
        {
          ReadOnlyCollection<CatalogResource> readOnlyCollection = this.ConfigurationServer.GetService<ICatalogService>().QueryResources((IEnumerable<Guid>) new Guid[1]
          {
            this.CatalogResourceId
          }, CatalogQueryOptions.None);
          if (readOnlyCollection.Count == 1)
          {
            foreach (CatalogNode nodeReference in readOnlyCollection[0].NodeReferences)
            {
              if (VssStringComparer.CatalogNodePath.StartsWith(nodeReference.FullPath, CatalogRoots.OrganizationalPath))
                this.m_catalogNode = nodeReference;
            }
          }
        }
        return this.m_catalogNode;
      }
    }

    public TfsConfigurationServer ConfigurationServer
    {
      get
      {
        if (this.m_configurationServer == null)
        {
          lock (this.m_configurationServerLockObject)
          {
            if (this.m_configurationServer != null)
              return this.m_configurationServer;
            string uriString = this.ServerDataProvider.LocationForCurrentConnection("LocationService", LocationServiceConstants.ApplicationLocationServiceIdentifier);
            if (!string.IsNullOrEmpty(uriString))
            {
              this.m_configurationServer = !this.UseFactory ? new TfsConfigurationServer(new Uri(uriString), this.ClientCredentials) : TfsConfigurationServerFactory.GetConfigurationServer(new Uri(uriString), this.HasAuthenticated ? this.ClientCredentials : (VssCredentials) null);
              this.m_configurationServer.EnsureAuthenticated();
            }
          }
        }
        return this.m_configurationServer;
      }
      internal set
      {
        if (this.m_configurationServer != null)
          return;
        lock (this.m_configurationServerLockObject)
        {
          if (this.m_configurationServer != null)
            return;
          this.m_configurationServer = value;
        }
      }
    }

    internal bool UseFactory { get; set; }

    public static Uri GetFullyQualifiedUriForName(string name) => TfsConnection.GetFullyQualifiedUriForName(name, LocationServiceConstants.CollectionLocationServiceRelativePath, (Func<string, Uri>) (friendlyName => RegisteredTfsConnections.GetProjectCollection(friendlyName)?.Uri));

    protected override object GetServiceInstance(Type serviceType, object serviceInstance)
    {
      if (serviceType == typeof (ILinking) || serviceType == typeof (IRegistration) || serviceType == typeof (IFrameworkRegistration) || serviceType == typeof (ICommonStructureService) || serviceType == typeof (ICommonStructureService3) || serviceType == typeof (ICommonStructureService4) || serviceType == typeof (IGroupSecurityService) || serviceType == typeof (IGroupSecurityService2) || serviceType == typeof (IAuthorizationService) || serviceType == typeof (IServerStatusService) || serviceType == typeof (IProcessTemplates) || serviceType == typeof (ISyncService))
        return this.CreateServiceProxy(serviceType);
      if (serviceType.FullName == "Microsoft.TeamFoundation.Build.Client.IBuildServer")
        return this.CreateServiceInstance(serviceType.Assembly, "Microsoft.TeamFoundation.Build.Client.BuildServer");
      if (serviceType.FullName == "Microsoft.TeamFoundation.TestManagement.Client.ITestManagementService")
        return this.CreateServiceInstance(serviceType.Assembly, "Microsoft.TeamFoundation.TestManagement.Client.TestManagementService");
      if (serviceType.FullName == "Microsoft.TeamFoundation.TestManagement.Client.ITestManagementService2")
        return this.CreateServiceInstance(serviceType.Assembly, "Microsoft.TeamFoundation.TestManagement.Client.TestManagementService");
      if (serviceType.FullName == "Microsoft.TeamFoundation.CodeSense.Client.ICodeSenseService")
      {
        try
        {
          return this.CreateServiceInstance(Assembly.Load(CodeSenseVersionInfo.CoreAssemblyFullname), CodeSenseVersionInfo.ServiceObjectName);
        }
        catch (FileNotFoundException ex)
        {
          return (object) null;
        }
      }
      else
      {
        if (!(serviceType.FullName == "Microsoft.TeamFoundation.CodeSense.Client.ICachedCodeSenseServiceFactory"))
          return base.GetServiceInstance(serviceType, serviceInstance);
        try
        {
          return this.CreateServiceInstance(Assembly.Load(CodeSenseVersionInfo.CoreAssemblyFullname), CodeSenseVersionInfo.CachedServiceFactoryObjectName);
        }
        catch (FileNotFoundException ex)
        {
          return (object) null;
        }
      }
    }

    private object CreateServiceProxy(Type serviceType)
    {
      if (serviceType == typeof (IRegistration) || serviceType == typeof (IFrameworkRegistration))
        return (object) this.RegistrationProxy;
      if (serviceType == typeof (ILinking))
        return (object) new LinkingService(this);
      if (serviceType == typeof (IProcessTemplates))
        return (object) new TeamFoundationProcessTemplateService(this);
      object serviceProxy = (object) null;
      if (serviceType == typeof (ICommonStructureService))
      {
        string url = this.ServerDataProvider.LocationForCurrentConnection("CommonStructure4", IntegrationServiceIdentifiers.CommonStructure4);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("CommonStructure3", IntegrationServiceIdentifiers.CommonStructure3);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("CommonStructure", IntegrationServiceIdentifiers.CommonStructure);
        serviceProxy = (object) new CommonStructureService(this, url);
      }
      else if (serviceType == typeof (ICommonStructureService3))
        serviceProxy = (object) new CommonStructureService3(this, this.ServerDataProvider.LocationForCurrentConnection("CommonStructure3", IntegrationServiceIdentifiers.CommonStructure3));
      else if (serviceType == typeof (ICommonStructureService4))
        serviceProxy = (object) new CommonStructureService4(this, this.ServerDataProvider.LocationForCurrentConnection("CommonStructure4", IntegrationServiceIdentifiers.CommonStructure4));
      else if (serviceType == typeof (IGroupSecurityService))
        serviceProxy = (object) new Microsoft.TeamFoundation.Proxy.GroupSecurityService(this, this.ServerDataProvider.LocationForCurrentConnection("GroupSecurity", IntegrationServiceIdentifiers.GroupSecurity));
      else if (serviceType == typeof (IGroupSecurityService2))
        serviceProxy = (object) new GroupSecurityService2(this, this.ServerDataProvider.LocationForCurrentConnection("GroupSecurity2", IntegrationServiceIdentifiers.GroupSecurity2));
      else if (serviceType == typeof (IAuthorizationService))
      {
        string url = this.ServerDataProvider.LocationForCurrentConnection("Authorization7", IntegrationServiceIdentifiers.Authorization7);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("Authorization6", IntegrationServiceIdentifiers.Authorization6);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("Authorization5", IntegrationServiceIdentifiers.Authorization5);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("Authorization4", IntegrationServiceIdentifiers.Authorization4);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("Authorization3", IntegrationServiceIdentifiers.Authorization3);
        if (string.IsNullOrEmpty(url))
          url = this.ServerDataProvider.LocationForCurrentConnection("Authorization", IntegrationServiceIdentifiers.Authorization);
        serviceProxy = (object) new Microsoft.TeamFoundation.Proxy.AuthorizationService(this, url);
      }
      else if (serviceType == typeof (IServerStatusService))
        serviceProxy = (object) new ServerStatusService(this, this.ServerDataProvider.LocationForCurrentConnection("ServerStatus", IntegrationServiceIdentifiers.ServerStatus));
      else if (serviceType == typeof (ISyncService))
      {
        if (!string.IsNullOrEmpty(this.ServerDataProvider.LocationForCurrentConnection("SyncService4", IntegrationServiceIdentifiers.SyncService4)))
        {
          serviceProxy = (object) new SyncService4(this);
        }
        else
        {
          this.ServerDataProvider.LocationForCurrentConnection("SyncService", IntegrationServiceIdentifiers.SyncService);
          serviceProxy = (object) new SyncService(this);
        }
      }
      return serviceProxy;
    }

    protected override object InitializeTeamFoundationObject(string fullName, object instance)
    {
      if (instance is ITfsTeamProjectCollectionObject collectionObject)
      {
        collectionObject.Initialize(this);
        return (object) collectionObject;
      }
      instance = base.InitializeTeamFoundationObject(fullName, instance);
      if (instance == null && instance is IDisposable disposable)
        disposable.Dispose();
      return instance;
    }

    private RegistrationService RegistrationProxy
    {
      get
      {
        if (this.m_registrationProxy == null)
          this.m_registrationProxy = new RegistrationService(this);
        return this.m_registrationProxy;
      }
    }
  }
}
