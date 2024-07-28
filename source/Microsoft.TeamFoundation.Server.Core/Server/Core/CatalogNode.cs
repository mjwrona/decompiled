// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogNode
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class CatalogNode
  {
    private List<CatalogNodeDependency> m_nodeDependencies = new List<CatalogNodeDependency>();
    private CatalogDependencyGroup m_dependencies = new CatalogDependencyGroup();

    public CatalogNode()
    {
    }

    public CatalogNode(
      ITeamFoundationCatalogService catalogService,
      string parentPath,
      string childItem)
      : this(catalogService, parentPath, childItem, (CatalogResource) null)
    {
    }

    public CatalogNode(
      ITeamFoundationCatalogService catalogService,
      string parentPath,
      string childItem,
      CatalogResource catalogResource)
    {
      this.ParentPath = parentPath;
      this.ChildItem = childItem;
      this.Resource = catalogResource;
      this.CatalogService = catalogService;
      this.NodeDependenciesIncluded = true;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string FullPath { get; set; }

    [XmlAttribute("default")]
    [ClientProperty(ClientVisibility.Private)]
    public bool IsDefault { get; set; }

    [XmlIgnore]
    public CatalogResource Resource { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid ResourceIdentifier { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ParentPath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ChildItem { get; set; }

    [XmlIgnore]
    public CatalogDependencyGroup Dependencies => this.m_dependencies;

    [ClientProperty(ClientVisibility.Private)]
    public List<CatalogNodeDependency> NodeDependencies => this.m_nodeDependencies;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public bool NodeDependenciesIncluded { get; set; }

    [XmlIgnore]
    public CatalogNode ParentNode { get; internal set; }

    [XmlIgnore]
    public CatalogChangeType ChangeType
    {
      get => (CatalogChangeType) this.ChangeTypeValue;
      set => this.ChangeTypeValue = (int) value;
    }

    [XmlAttribute("ctype")]
    [ClientProperty(ClientVisibility.Private)]
    public int ChangeTypeValue { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public bool MatchedQuery { get; set; }

    public CatalogNode CreateChild(
      IVssRequestContext requestContext,
      CatalogResource existingResource)
    {
      return new CatalogNode(this.CatalogService, this.ParentPath + this.ChildItem, Convert.ToBase64String(Guid.NewGuid().ToByteArray()), existingResource);
    }

    public CatalogNode CreateChild(
      IVssRequestContext requestContext,
      Guid resourceTypeIdentifier,
      string displayName)
    {
      CatalogResource existingResource = new CatalogResource(this.CatalogService.QueryResourceType(requestContext, resourceTypeIdentifier), displayName);
      return this.CreateChild(requestContext, existingResource);
    }

    public void ExpandDependencies(IVssRequestContext requestContext)
    {
      if (string.IsNullOrEmpty(this.FullPath))
        return;
      CatalogNode catalogNode = this.CatalogService.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        this.FullPath
      }, (IEnumerable<Guid>) null, CatalogQueryOptions.ExpandDependencies).FirstOrDefault<CatalogNode>();
      this.NodeDependenciesIncluded = true;
      if (catalogNode != null)
        this.m_dependencies = catalogNode.Dependencies;
      else
        this.m_dependencies = new CatalogDependencyGroup();
    }

    public List<CatalogNode> QueryChildren(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurse,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryChildren(requestContext, resourceTypeFilters, (IEnumerable<KeyValuePair<string, string>>) null, recurse, queryOptions);
    }

    public List<CatalogNode> QueryChildren(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      bool recurse,
      CatalogQueryOptions queryOptions)
    {
      if (string.IsNullOrEmpty(this.FullPath))
        throw new CatalogNodeDoesNotExistException();
      string str = recurse ? CatalogConstants.FullRecurseStars : CatalogConstants.SingleRecurseStar;
      return this.CatalogService.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        this.FullPath + str
      }, resourceTypeFilters, propertyFilters, queryOptions);
    }

    public List<CatalogNode> QueryParents(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions)
    {
      List<string> pathSpecs = new List<string>();
      if (recurseToRoot)
      {
        for (int length = this.ParentPath.Length; length > 0; length -= CatalogConstants.MandatoryNodePathLength)
          pathSpecs.Add(this.ParentPath.Substring(0, length));
      }
      else
        pathSpecs.Add(this.ParentPath);
      return this.CatalogService.QueryNodes(requestContext, (IEnumerable<string>) pathSpecs, resourceTypeFilters, queryOptions);
    }

    public List<CatalogNode> QueryDependents(
      IVssRequestContext requestContext,
      CatalogQueryOptions queryOptions)
    {
      if (string.IsNullOrEmpty(this.FullPath))
        throw new CatalogNodeDoesNotExistException();
      return this.CatalogService.QueryCatalogDependents(requestContext, this.FullPath, queryOptions);
    }

    internal ITeamFoundationCatalogService CatalogService { get; set; }

    internal void UpdateSelf(CatalogNode updatedNode)
    {
      this.FullPath = updatedNode.FullPath;
      this.IsDefault = updatedNode.IsDefault;
      this.ParentPath = updatedNode.ParentPath;
      this.Resource = updatedNode.Resource;
      this.ResourceIdentifier = updatedNode.ResourceIdentifier;
      this.m_dependencies = updatedNode.Dependencies;
      this.NodeDependenciesIncluded = updatedNode.NodeDependenciesIncluded;
      this.ParentNode = updatedNode.ParentNode;
    }

    internal void PrepareForWebServiceSerialization(bool matchedQuery)
    {
      this.MatchedQuery = matchedQuery;
      if (!this.NodeDependenciesIncluded)
        return;
      this.m_nodeDependencies = new List<CatalogNodeDependency>();
      if (this.Dependencies.Singletons != null)
      {
        foreach (KeyValuePair<string, CatalogNode> singleton in (IEnumerable<KeyValuePair<string, CatalogNode>>) this.Dependencies.Singletons)
          this.NodeDependencies.Add(new CatalogNodeDependency()
          {
            FullPath = this.FullPath,
            AssociationKey = singleton.Key,
            RequiredNodeFullPath = singleton.Value != null ? singleton.Value.FullPath : "",
            IsSingleton = true
          });
      }
      if (this.Dependencies.Sets == null)
        return;
      foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) this.Dependencies.Sets)
      {
        if (set.Value != null)
        {
          foreach (CatalogNode catalogNode in (IEnumerable<CatalogNode>) set.Value)
            this.NodeDependencies.Add(new CatalogNodeDependency()
            {
              FullPath = this.FullPath,
              AssociationKey = set.Key,
              RequiredNodeFullPath = catalogNode.FullPath,
              IsSingleton = false
            });
        }
      }
    }

    internal void InitializeFromWebService(
      ITeamFoundationCatalogService catalogService,
      Dictionary<Guid, CatalogResource> resources)
    {
      this.CatalogService = catalogService;
      CatalogResource catalogResource;
      if (!resources.TryGetValue(this.ResourceIdentifier, out catalogResource))
        throw new CatalogResourceDoesNotExistException(FrameworkResources.CatalogResourceMustBePassedWithNode());
      this.Resource = catalogResource;
      catalogResource.NodeReferences.Add(this);
    }

    internal static void Validate(Dictionary<string, object> newDefaults, CatalogNode node)
    {
      CatalogPathSpec.ValidatePathSpec(node.ParentPath + node.ChildItem, false, true);
      ArgumentUtility.CheckForNull<CatalogResource>(node.Resource, "CatalogNode.Resource");
      if (node.IsDefault)
      {
        string key = node.Resource.ResourceType.Identifier.ToString() + node.ParentPath;
        if (newDefaults.ContainsKey(key))
          throw new InvalidCatalogSaveNodeException(FrameworkResources.CannotSetTwoDefaultsMessage());
        newDefaults[key] = (object) null;
      }
      if (node.Dependencies == null)
        return;
      foreach (KeyValuePair<string, CatalogNode> singleton in (IEnumerable<KeyValuePair<string, CatalogNode>>) node.Dependencies.Singletons)
      {
        PropertyValidation.CheckPropertyLength(singleton.Key, false, 0, 256, "NodeDependency.Key", typeof (CatalogNode), "NodeDependency.Key");
        ArgumentUtility.CheckForNull<CatalogNode>(singleton.Value, "CatalogNode.Dependencies.Singletons.Value");
        if (VssStringComparer.CatalogNodePath.Equals(node.ChildItem, singleton.Value.ChildItem))
          throw new InvalidCatalogSaveNodeException(FrameworkResources.CatalogInvalidSelfDependency());
      }
      foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) node.Dependencies.Sets)
      {
        PropertyValidation.CheckPropertyLength(set.Key, false, 0, 256, "NodeDependency.Key", typeof (CatalogNode), "NodeDependency.Key");
        if (set.Value == null || set.Value.Count == 0)
          throw new ArgumentException("CatalogNode.DependencySets.Values");
        foreach (CatalogNode catalogNode in (IEnumerable<CatalogNode>) set.Value)
        {
          if (VssStringComparer.CatalogNodePath.Equals(node.ChildItem, catalogNode.ChildItem))
            throw new InvalidCatalogSaveNodeException(FrameworkResources.CatalogInvalidSelfDependency());
        }
      }
    }
  }
}
