// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.CatalogObjectContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Common.Catalog.Objects;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CatalogObjectContext
  {
    private ICatalogService m_catalogService;
    private CatalogChangeContext m_transactionContext;
    private OrganizationalRoot m_OrganizationalRoot;
    private InfrastructureRoot m_InfrastructureRoot;
    private List<CatalogObject> m_objectsAffectedByTransactionContext = new List<CatalogObject>();

    public CatalogObjectContext(ICatalogService catalogService)
    {
      ArgumentUtility.CheckForNull<ICatalogService>(catalogService, nameof (catalogService));
      this.m_catalogService = catalogService;
      this.QueryOptions = CatalogQueryOptions.ExpandDependencies;
    }

    public CatalogQueryOptions QueryOptions { get; set; }

    public ICatalogService CatalogService => this.m_catalogService;

    public ILocationService LocationService => this.CatalogService.LocationService;

    public CatalogChangeContext TransactionContext
    {
      get
      {
        if (this.m_transactionContext == null)
          this.m_transactionContext = this.CatalogService.CreateChangeContext();
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

    public void SaveChanges()
    {
      if (this.m_transactionContext == null)
        return;
      try
      {
        this.TransactionContext.Save();
      }
      catch (Exception ex)
      {
        this.RefreshAffectedObjects();
        throw;
      }
      finally
      {
        this.m_transactionContext = (CatalogChangeContext) null;
        this.m_objectsAffectedByTransactionContext.Clear();
      }
    }

    public void ResetTransactionContext()
    {
      this.m_transactionContext = (CatalogChangeContext) null;
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
        result = this.CreateCatalogObject<T>(this.CatalogService.QueryRootNode(tree));
      return result;
    }

    private bool TryGetCatalogResourceType(
      Type catalogObjectType,
      out CatalogResourceType catalogResourceType)
    {
      ReadOnlyCollection<CatalogResourceType> source = this.CatalogService.QueryResourceTypes((IEnumerable<Guid>) null);
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

    public T CreateCatalogObject<T>(CatalogBulkData bulkData, string nodePath) where T : CatalogObject
    {
      T catalogObject = default (T);
      Guid resourceTypeId = CatalogObjectUtilities.GetResourceTypeIdentifier<T>();
      if (bulkData.QueriedForResourceType(resourceTypeId))
        catalogObject = this.CreateCatalogObject<T>(bulkData.Nodes.FirstOrDefault<CatalogNode>((Func<CatalogNode, bool>) (node => VssStringComparer.Guid.Equals((object) node.Resource.ResourceType.Identifier, (object) resourceTypeId) && VssStringComparer.CatalogNodePath.Equals(node.FullPath, nodePath))));
      if ((object) catalogObject != null)
        catalogObject.Preload(bulkData);
      return catalogObject;
    }

    private void RefreshAffectedObjects()
    {
      foreach (CatalogObject catalogObject in this.m_objectsAffectedByTransactionContext)
      {
        if (!string.IsNullOrEmpty(catalogObject.CatalogNodeFullPath))
          catalogObject.Refresh();
      }
    }

    public static T CreateCatalogObject<T>(
      TfsConfigurationServer tfsServer,
      CatalogNode catalogNode)
      where T : CatalogObject
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(tfsServer, nameof (tfsServer));
      return new CatalogObjectContext(tfsServer.GetService<ICatalogService>()).CreateCatalogObject<T>(catalogNode);
    }
  }
}
