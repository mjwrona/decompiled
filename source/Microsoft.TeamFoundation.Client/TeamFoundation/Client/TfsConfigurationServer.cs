// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConfigurationServer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  public class TfsConfigurationServer : TfsConnection
  {
    private Dictionary<string, TfsTeamProjectCollection> m_collections = new Dictionary<string, TfsTeamProjectCollection>((IEqualityComparer<string>) VssStringComparer.Url);
    private CatalogNode m_catalogNode;
    private string m_displayName;
    private int? m_instanceIdHashcode;

    public TfsConfigurationServer(Uri uri)
      : this(uri, false)
    {
    }

    public TfsConfigurationServer(Uri uri, VssCredentials credentials)
      : base(uri, credentials, (IdentityDescriptor) null, LocationServiceConstants.ApplicationLocationServiceRelativePath, (ITfsRequestChannelFactory) null)
    {
    }

    public TfsConfigurationServer(Uri uri, IdentityDescriptor identityToImpersonate)
      : base(uri, LocationServiceConstants.ApplicationLocationServiceRelativePath, identityToImpersonate, (ITfsRequestChannelFactory) null)
    {
    }

    [Obsolete("This constructor is obsolete and will be removed in a future version. See TfsConfigurationServer.TfsConfigurationServer(Uri, VssCredentials) instead", false)]
    public TfsConfigurationServer(Uri uri, ICredentials credentials)
      : base(uri, credentials, LocationServiceConstants.ApplicationLocationServiceRelativePath)
    {
    }

    public TfsConfigurationServer(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate)
      : this(uri, credentials, identityToImpersonate, false)
    {
    }

    public TfsConfigurationServer(RegisteredConfigurationServer application)
      : this(application, (IdentityDescriptor) null)
    {
    }

    public TfsConfigurationServer(
      RegisteredConfigurationServer application,
      IdentityDescriptor identityToImpersonate)
      : base(application.Uri, LocationServiceConstants.ApplicationLocationServiceRelativePath, identityToImpersonate, (ITfsRequestChannelFactory) null)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsConfigurationServer(Uri uri, bool fromFactory)
      : base(uri, LocationServiceConstants.ApplicationLocationServiceRelativePath)
    {
      this.UseFactory = fromFactory;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsConfigurationServer(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      bool fromFactory)
      : this(uri, credentials, identityToImpersonate, (ITfsRequestChannelFactory) null, fromFactory)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsConfigurationServer(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      ITfsRequestChannelFactory channelFactory)
      : this(uri, credentials, identityToImpersonate, channelFactory, false)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsConfigurationServer(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      ITfsRequestChannelFactory channelFactory,
      bool fromFactory)
      : base(uri, credentials, identityToImpersonate, LocationServiceConstants.ApplicationLocationServiceRelativePath, channelFactory)
    {
      this.UseFactory = fromFactory;
    }

    public override string Name
    {
      get
      {
        if (this.m_displayName == null)
        {
          try
          {
            RegisteredConfigurationServer configurationServer = RegisteredTfsConnections.GetConfigurationServer(this.Uri);
            if (configurationServer != null)
              this.m_displayName = configurationServer.Name;
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
          ReadOnlyCollection<CatalogResource> readOnlyCollection = this.GetService<ICatalogService>().QueryResources((IEnumerable<Guid>) new Guid[1]
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

    internal bool UseFactory { get; set; }

    internal int? InstanceIdHashCode
    {
      get
      {
        if (!this.m_instanceIdHashcode.HasValue && this.HasAuthenticated)
          this.m_instanceIdHashcode = new int?(ClrHashUtil.GetStringHashOrcas64(this.InstanceId.ToString("D").ToUpperInvariant()));
        return this.m_instanceIdHashcode;
      }
    }

    public TfsTeamProjectCollection GetTeamProjectCollection(Guid collectionId)
    {
      string serverLocation = this.ServerDataProvider.FindServerLocation(collectionId);
      TfsTeamProjectCollection projectCollection = (TfsTeamProjectCollection) null;
      lock (this.m_collections)
      {
        if (serverLocation != null)
        {
          if (!this.m_collections.TryGetValue(serverLocation, out projectCollection))
          {
            projectCollection = !this.UseFactory ? new TfsTeamProjectCollection(new Uri(serverLocation), this.ClientCredentials) : TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(serverLocation));
            this.m_collections[serverLocation] = projectCollection;
          }
        }
      }
      return projectCollection;
    }

    internal IEnumerable<TfsTeamProjectCollection> GetTeamProjectCollections()
    {
      lock (this.m_collections)
        return (IEnumerable<TfsTeamProjectCollection>) this.m_collections.Values.ToArray<TfsTeamProjectCollection>();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Uri GetFullyQualifiedUriForName(string name) => TfsConnection.GetFullyQualifiedUriForName(name, LocationServiceConstants.ApplicationLocationServiceRelativePath, (Func<string, Uri>) (friendlyName => RegisteredTfsConnections.GetConfigurationServer(friendlyName)?.Uri));

    protected override object GetServiceInstance(Type serviceType, object serviceInstance) => serviceType == typeof (ITeamProjectCollectionService) || serviceType == typeof (IAdministrationService) || serviceType == typeof (ICatalogService) ? this.CreateServiceProxy(serviceType) : base.GetServiceInstance(serviceType, serviceInstance);

    private object CreateServiceProxy(Type serviceType)
    {
      if (serviceType == typeof (ITeamProjectCollectionService))
        return (object) new TeamProjectCollectionService(this);
      if (serviceType == typeof (IAdministrationService))
        return (object) new TeamFoundationAdministrationService(this);
      return serviceType == typeof (ICatalogService) ? (object) new CatalogService(this) : (object) null;
    }

    protected override object InitializeTeamFoundationObject(string fullName, object instance)
    {
      if (instance is ITfsConfigurationServerObject configurationServerObject)
      {
        configurationServerObject.Initialize(this);
        return (object) configurationServerObject;
      }
      instance = base.InitializeTeamFoundationObject(fullName, instance);
      if (instance == null && instance is IDisposable disposable)
        disposable.Dispose();
      return instance;
    }
  }
}
