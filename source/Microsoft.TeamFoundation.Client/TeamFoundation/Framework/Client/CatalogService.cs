// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class CatalogService : ICatalogService
  {
    private bool m_resourceTypesLoaded;
    private ReaderWriterLock m_accessLock = new ReaderWriterLock();
    private Dictionary<Guid, CatalogResourceType> m_resourceTypes = new Dictionary<Guid, CatalogResourceType>();
    private TfsConfigurationServer m_server;
    private ILocationService m_locationService;
    private CatalogWebService m_catalogProxy;

    public CatalogService(TfsConfigurationServer server)
    {
      this.m_server = server;
      this.m_locationService = this.m_server.GetService<ILocationService>();
      this.m_catalogProxy = new CatalogWebService(server);
    }

    public ReadOnlyCollection<CatalogResourceType> QueryResourceTypes(
      IEnumerable<Guid> resourceTypeIdentifiers)
    {
      this.EnsureResourceTypesLoaded();
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        if (resourceTypeIdentifiers == null || resourceTypeIdentifiers.FirstOrDefault<Guid>() == Guid.Empty)
          return new List<CatalogResourceType>((IEnumerable<CatalogResourceType>) this.m_resourceTypes.Values).AsReadOnly();
        List<CatalogResourceType> catalogResourceTypeList = new List<CatalogResourceType>();
        foreach (Guid resourceTypeIdentifier in resourceTypeIdentifiers)
        {
          CatalogResourceType catalogResourceType;
          if (!this.m_resourceTypes.TryGetValue(resourceTypeIdentifier, out catalogResourceType))
            throw new CatalogResourceTypeDoesNotExistException(TFCommonResources.CatalogResourceTypeDoesNotExist((object) resourceTypeIdentifier.ToString()));
          catalogResourceTypeList.Add(catalogResourceType);
        }
        return catalogResourceTypeList.AsReadOnly();
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public ReadOnlyCollection<CatalogResource> QueryResources(
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resourceIdentifiers, nameof (resourceIdentifiers));
      CatalogData catalogData = this.m_catalogProxy.QueryResources(resourceIdentifiers.ToArray<Guid>(), (int) queryOptions);
      List<CatalogResource> matchingResources;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, queryOptions, out matchingResources, out List<CatalogNode> _);
      return matchingResources.AsReadOnly();
    }

    public ReadOnlyCollection<CatalogResource> QueryResourcesByType(
      IEnumerable<Guid> resourceTypeIdentifiers,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryResources(resourceTypeIdentifiers, (IEnumerable<KeyValuePair<string, string>>) null, queryOptions);
    }

    public ReadOnlyCollection<CatalogResource> QueryResources(
      IEnumerable<Guid> resourceTypeIdentifiers,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      CatalogData catalogData = this.m_catalogProxy.QueryResourcesByType(resourceTypeIdentifiers == null ? (Guid[]) null : resourceTypeIdentifiers.ToArray<Guid>(), KeyValueOfStringString.Convert(propertyFilters), (int) queryOptions);
      List<CatalogResource> matchingResources;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, queryOptions, out matchingResources, out List<CatalogNode> _);
      return matchingResources.AsReadOnly();
    }

    public ReadOnlyCollection<CatalogNode> QueryUpTree(
      string path,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      if (path.IndexOfAny(CatalogConstants.PatternMatchingCharacters) != -1)
        throw new InvalidCatalogNodePathException(TFCommonResources.InvalidCatalogNodePathNoWildcard((object) path));
      Guid[] array = resourceTypeFilters == null ? (Guid[]) null : resourceTypeFilters.ToArray<Guid>();
      List<string> stringList = new List<string>();
      for (int mandatoryNodePathLength = CatalogConstants.MandatoryNodePathLength; mandatoryNodePathLength < path.Length; mandatoryNodePathLength += CatalogConstants.MandatoryNodePathLength)
        stringList.Add(path.Substring(0, mandatoryNodePathLength) + CatalogConstants.SingleRecurseStar);
      CatalogData catalogData = this.m_catalogProxy.QueryNodes(stringList.ToArray(), array, (KeyValueOfStringString[]) null, (int) queryOptions);
      List<CatalogNode> matchingNodes;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, queryOptions, out List<CatalogResource> _, out matchingNodes);
      return matchingNodes.AsReadOnly();
    }

    public ReadOnlyCollection<CatalogNode> QueryParents(
      Guid resourceIdentifier,
      IEnumerable<string> pathFilters,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions)
    {
      ArgumentUtility.CheckForEmptyGuid(resourceIdentifier, nameof (resourceIdentifier));
      string[] array1 = pathFilters == null ? (string[]) null : pathFilters.ToArray<string>();
      Guid[] array2 = resourceTypeFilters == null ? (Guid[]) null : resourceTypeFilters.ToArray<Guid>();
      CatalogData catalogData = this.m_catalogProxy.QueryParents(resourceIdentifier, array1, array2, recurseToRoot, (int) queryOptions);
      List<CatalogNode> matchingNodes;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, queryOptions, out List<CatalogResource> _, out matchingNodes);
      return matchingNodes.AsReadOnly();
    }

    public ReadOnlyCollection<CatalogNode> QueryNodes(
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) pathSpecs, nameof (pathSpecs));
      return this.QueryNodes(pathSpecs, resourceTypeFilters, (IEnumerable<KeyValuePair<string, string>>) null, queryOptions);
    }

    public ReadOnlyCollection<CatalogNode> QueryNodes(
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      CatalogData catalogData = this.m_catalogProxy.QueryNodes(pathSpecs == null ? (string[]) null : pathSpecs.ToArray<string>(), resourceTypeFilters == null ? (Guid[]) null : resourceTypeFilters.ToArray<Guid>(), KeyValueOfStringString.Convert(propertyFilters), (int) queryOptions);
      List<CatalogNode> matchingNodes;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, queryOptions, out List<CatalogResource> _, out matchingNodes);
      return matchingNodes.AsReadOnly();
    }

    public ReadOnlyCollection<CatalogNode> QueryDependents(
      string path,
      CatalogQueryOptions queryOptions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      CatalogData catalogData = this.m_catalogProxy.QueryDependents(path, (int) queryOptions);
      List<CatalogNode> matchingNodes;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, queryOptions, out List<CatalogResource> _, out matchingNodes);
      return matchingNodes.AsReadOnly();
    }

    public ReadOnlyCollection<CatalogNode> RootNodes
    {
      get
      {
        CatalogData catalogData = this.m_catalogProxy.QueryNodes(new string[1]
        {
          CatalogConstants.SingleRecurseStar
        }, (Guid[]) null, (KeyValueOfStringString[]) null, 0);
        List<CatalogNode> matchingNodes;
        this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, CatalogQueryOptions.None, out List<CatalogResource> _, out matchingNodes);
        return matchingNodes.AsReadOnly();
      }
    }

    public CatalogNode QueryRootNode(CatalogTree tree)
    {
      CatalogData catalogData = this.m_catalogProxy.QueryNodes(new string[1]
      {
        CatalogRoots.DeterminePath(tree)
      }, (Guid[]) null, (KeyValueOfStringString[]) null, 0);
      List<CatalogNode> matchingNodes;
      this.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogData.CatalogResourceTypes, CatalogQueryOptions.None, out List<CatalogResource> _, out matchingNodes);
      return matchingNodes.Count == 1 ? matchingNodes[0] : throw new CatalogNodeDoesNotExistException(TFCommonResources.CatalogNodeDoesNotExist());
    }

    public void SaveResource(CatalogResource resource)
    {
      CatalogChangeContext catalogChangeContext = new CatalogChangeContext(this, this.m_catalogProxy);
      catalogChangeContext.AttachResource(resource);
      catalogChangeContext.Save();
    }

    public void SaveNode(CatalogNode node)
    {
      CatalogChangeContext catalogChangeContext = new CatalogChangeContext(this, this.m_catalogProxy);
      catalogChangeContext.AttachNode(node);
      catalogChangeContext.Save();
    }

    public void SaveDelete(CatalogNode node, bool recurse)
    {
      CatalogChangeContext catalogChangeContext = new CatalogChangeContext(this, this.m_catalogProxy);
      catalogChangeContext.AttachDelete(node, recurse);
      catalogChangeContext.Save();
    }

    public void SaveMove(CatalogNode nodeToMove, CatalogNode newParent)
    {
      CatalogChangeContext catalogChangeContext = new CatalogChangeContext(this, this.m_catalogProxy);
      catalogChangeContext.AttachMove(nodeToMove, newParent);
      catalogChangeContext.Save();
    }

    public CatalogChangeContext CreateChangeContext() => new CatalogChangeContext(this, this.m_catalogProxy);

    public ILocationService LocationService => this.m_server.GetService<ILocationService>();

    internal void BuildCatalogObjects(
      int locationServiceLastChangeId,
      CatalogResource[] resources,
      CatalogNode[] nodes,
      CatalogResourceType[] types,
      CatalogQueryOptions queryOptions,
      out List<CatalogResource> matchingResources,
      out List<CatalogNode> matchingNodes)
    {
      this.m_server.ReactToPossibleServerUpdate(locationServiceLastChangeId);
      matchingNodes = new List<CatalogNode>();
      matchingResources = new List<CatalogResource>();
      Dictionary<Guid, CatalogResourceType> resourceTypes = new Dictionary<Guid, CatalogResourceType>();
      foreach (CatalogResourceType type in types)
        resourceTypes[type.Identifier] = type;
      Dictionary<string, CatalogNode> nodes1 = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      foreach (CatalogNode node in nodes)
      {
        node.InitializeFromWebService(this);
        nodes1[node.FullPath] = node;
        if (node.MatchedQuery)
          matchingNodes.Add(node);
      }
      if (queryOptions != CatalogQueryOptions.None)
      {
        foreach (CatalogNode node in nodes)
        {
          if ((queryOptions & CatalogQueryOptions.ExpandDependencies) == CatalogQueryOptions.ExpandDependencies)
          {
            foreach (CatalogNodeDependency nodeDependency in node.NodeDependencies)
            {
              if (nodeDependency.IsSingleton)
                node.Dependencies.SetSingletonDependency(nodeDependency.AssociationKey, nodes1[nodeDependency.RequiredNodeFullPath]);
              else
                node.Dependencies.AddSetDependency(nodeDependency.AssociationKey, nodes1[nodeDependency.RequiredNodeFullPath]);
            }
          }
          if ((queryOptions & CatalogQueryOptions.IncludeParents) == CatalogQueryOptions.IncludeParents && !string.IsNullOrEmpty(node.ParentPath))
            node.ParentNode = nodes1[node.ParentPath];
        }
      }
      List<CatalogResource> catalogResourceList = new List<CatalogResource>(resources.Length);
      foreach (CatalogResource resource in resources)
      {
        resource.InitializeFromWebService(resourceTypes, nodes1, this.m_locationService);
        if (resource.MatchedQuery)
          matchingResources.Add(resource);
      }
    }

    private void EnsureResourceTypesLoaded()
    {
      if (this.m_resourceTypesLoaded)
        return;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_resourceTypesLoaded)
          return;
        this.m_resourceTypes.Clear();
        foreach (CatalogResourceType queryResourceType in this.m_catalogProxy.QueryResourceTypes((Guid[]) null))
          this.m_resourceTypes[queryResourceType.Identifier] = queryResourceType;
        this.m_resourceTypesLoaded = true;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }
  }
}
