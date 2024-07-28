// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.CatalogObjectContext
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Common.Catalog.Objects;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CatalogObjectContext
  {
    private IVssDeploymentServiceHost m_deploymentHost;
    private Guid m_instanceId;
    private CatalogTransactionContext m_transactionContext;
    private AccessMapping m_defaultAccessMapping;
    private AccessMapping m_publicAccessMapping;
    private bool m_isServicing;
    private Dictionary<string, object> m_requestContextItems;
    private OrganizationalRoot m_OrganizationalRoot;
    private InfrastructureRoot m_InfrastructureRoot;
    private IVssRequestContext m_requestContext;
    private List<CatalogObject> m_objectsAffectedByTransactionContext = new List<CatalogObject>();

    public CatalogObjectContext(IVssRequestContext requestContext)
      : this(requestContext, false)
    {
    }

    internal CatalogObjectContext(IVssRequestContext requestContext, bool saveRequestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInOnPremTfsOnly());
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.QueryOptions = CatalogQueryOptions.ExpandDependencies;
      if (saveRequestContext)
      {
        this.m_requestContext = requestContext;
      }
      else
      {
        this.m_deploymentHost = requestContext.ServiceHost.DeploymentServiceHost;
        this.m_instanceId = requestContext.ServiceHost.InstanceId;
        this.m_isServicing = requestContext.IsServicingContext;
        this.m_requestContextItems = new Dictionary<string, object>(requestContext.Items);
      }
    }

    public CatalogQueryOptions QueryOptions { get; set; }

    public AccessMapping DefaultAccessMapping
    {
      get
      {
        if (this.m_defaultAccessMapping == null)
        {
          IVssRequestContext requestContext;
          using (this.GetRequestContext(out requestContext))
            this.m_defaultAccessMapping = this.GetLocationService(requestContext).GetServerAccessMapping(requestContext);
        }
        return this.m_defaultAccessMapping;
      }
      set => this.m_defaultAccessMapping = value;
    }

    public AccessMapping PublicAccessMapping
    {
      get
      {
        if (this.m_publicAccessMapping == null)
        {
          IVssRequestContext requestContext;
          using (this.GetRequestContext(out requestContext))
            this.m_publicAccessMapping = this.GetLocationService(requestContext).GetPublicAccessMapping(requestContext);
        }
        return this.m_publicAccessMapping;
      }
    }

    public ITeamFoundationCatalogService GetCatalogService(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationCatalogService>();

    public ILocationService GetLocationService(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>();

    public TeamFoundationIdentityService GetIdentityService(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationIdentityService>();

    public CatalogTransactionContext TransactionContext
    {
      get
      {
        if (this.m_transactionContext == null)
        {
          IVssRequestContext requestContext;
          using (this.GetRequestContext(out requestContext))
            this.m_transactionContext = this.GetCatalogService(requestContext).CreateTransactionContext();
        }
        return this.m_transactionContext;
      }
    }

    public OrganizationalRoot OrganizationalRoot => this.GetRootNode<OrganizationalRoot>(CatalogTree.Organizational, ref this.m_OrganizationalRoot);

    public InfrastructureRoot InfrastructureRoot => this.GetRootNode<InfrastructureRoot>(CatalogTree.Infrastructure, ref this.m_InfrastructureRoot);

    public string GetDisplayName(Type catalogObjectType)
    {
      CatalogResourceType catalogResourceType;
      return this.TryGetCatalogResourceType(catalogObjectType, out catalogResourceType) ? catalogResourceType.DisplayName : catalogObjectType.Name;
    }

    public string GetDescription(Type catalogObjectType)
    {
      CatalogResourceType catalogResourceType;
      return this.TryGetCatalogResourceType(catalogObjectType, out catalogResourceType) ? catalogResourceType.Description : string.Empty;
    }

    public void AddObject(CatalogObject entity)
    {
      this.m_objectsAffectedByTransactionContext.Add(entity);
      this.TransactionContext.AttachNode(entity.CatalogNode);
    }

    public void ModifyObject(CatalogObject entity)
    {
      this.m_objectsAffectedByTransactionContext.Add(entity);
      this.TransactionContext.AttachNode(entity.CatalogNode);
    }

    public void DeleteObject(CatalogObject entity)
    {
      this.m_objectsAffectedByTransactionContext.Add(entity);
      this.TransactionContext.AttachDelete(entity.CatalogNode, true);
    }

    public void RefreshOnTransactionContextFailureOrReset(CatalogObject entity) => this.m_objectsAffectedByTransactionContext.Add(entity);

    public IDisposable GetRequestContext(out IVssRequestContext requestContext)
    {
      if (this.m_requestContext != null)
      {
        requestContext = this.m_requestContext;
        return (IDisposable) null;
      }
      if (!this.m_isServicing)
      {
        using (IVssRequestContext systemContext = this.m_deploymentHost.CreateSystemContext(false))
        {
          ITeamFoundationHostManagementService service = systemContext.GetService<ITeamFoundationHostManagementService>();
          requestContext = service.BeginRequest(systemContext, this.m_instanceId, RequestContextType.SystemContext, throwIfShutdown: false);
          foreach (KeyValuePair<string, object> requestContextItem in this.m_requestContextItems)
            requestContext.Items.Add(requestContextItem.Key, requestContextItem.Value);
          return (IDisposable) requestContext;
        }
      }
      else
      {
        using (IVssRequestContext servicingContext = this.m_deploymentHost.CreateServicingContext())
        {
          ITeamFoundationHostManagementService service = servicingContext.GetService<ITeamFoundationHostManagementService>();
          requestContext = service.BeginRequest(servicingContext, this.m_instanceId, RequestContextType.ServicingContext, throwIfShutdown: false);
          foreach (KeyValuePair<string, object> requestContextItem in this.m_requestContextItems)
            requestContext.Items.Add(requestContextItem.Key, requestContextItem.Value);
          return (IDisposable) requestContext;
        }
      }
    }

    public void SaveChanges()
    {
      if (this.m_transactionContext == null)
        return;
      try
      {
        IVssRequestContext requestContext;
        using (this.GetRequestContext(out requestContext))
          this.TransactionContext.Save(requestContext, false);
      }
      catch (Exception ex)
      {
        this.RefreshAffectedObjects();
        throw;
      }
      finally
      {
        this.m_transactionContext = (CatalogTransactionContext) null;
        this.m_objectsAffectedByTransactionContext.Clear();
      }
    }

    public void ResetTransactionContext()
    {
      this.m_transactionContext = (CatalogTransactionContext) null;
      this.RefreshAffectedObjects();
      this.m_objectsAffectedByTransactionContext.Clear();
    }

    public T CreateCatalogObject<T>(CatalogNode catalogNode) where T : CatalogObject
    {
      if (catalogNode == null)
        return default (T);
      if (typeof (T) != typeof (CatalogObject) && catalogNode.FullPath != null)
        CatalogObjectUtilities.GetResourceTypeIdentifier<T>();
      T instance = (T) Activator.CreateInstance(typeof (T));
      instance.Context = this;
      instance.CatalogNode = catalogNode;
      instance.Initialize();
      return instance;
    }

    private T GetRootNode<T>(CatalogTree tree, ref T result) where T : CatalogObject
    {
      if ((object) result == null)
      {
        IVssRequestContext requestContext;
        using (this.GetRequestContext(out requestContext))
          result = this.CreateCatalogObject<T>(this.GetCatalogService(requestContext).QueryRootNode(requestContext, tree));
      }
      return result;
    }

    private T GetService<T>(IVssRequestContext requestContext, ref T service) where T : class, IVssFrameworkService
    {
      if ((object) service == null)
        service = requestContext.GetService<T>();
      return service;
    }

    private bool TryGetCatalogResourceType(
      Type catalogObjectType,
      out CatalogResourceType catalogResourceType)
    {
      IVssRequestContext requestContext;
      using (this.GetRequestContext(out requestContext))
      {
        List<CatalogResourceType> source = this.GetCatalogService(requestContext).QueryResourceTypes(requestContext, (IEnumerable<Guid>) null);
        Guid resourseTypeIdentifier = CatalogObjectUtilities.GetResourceTypeIdentifier(catalogObjectType);
        Func<CatalogResourceType, bool> predicate = (Func<CatalogResourceType, bool>) (c => VssStringComparer.Guid.Equals((object) c.Identifier, (object) resourseTypeIdentifier));
        CatalogResourceType catalogResourceType1 = source.FirstOrDefault<CatalogResourceType>(predicate);
        if (catalogResourceType1 != null)
        {
          catalogResourceType = catalogResourceType1;
          return true;
        }
        catalogResourceType = (CatalogResourceType) null;
        return false;
      }
    }

    private void RefreshAffectedObjects()
    {
      using (this.GetRequestContext(out IVssRequestContext _))
      {
        foreach (CatalogObject catalogObject in this.m_objectsAffectedByTransactionContext)
        {
          if (!string.IsNullOrEmpty(catalogObject.CatalogNodeFullPath))
            catalogObject.Refresh();
        }
      }
    }
  }
}
