// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogNode
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class CatalogNode
  {
    private CatalogResource m_resource;
    private int m_changeTypeValue;
    private string m_childItem;
    private string m_fullPath;
    private bool m_isDefault;
    private bool m_matchedQuery;
    internal CatalogNodeDependency[] m_nodeDependencies = Helper.ZeroLengthArrayOfCatalogNodeDependency;
    private bool m_nodeDependenciesIncluded;
    private string m_parentPath;
    private Guid m_resourceIdentifier = Guid.Empty;

    private CatalogNode(
      CatalogService catalogService,
      string parentPath,
      string childItem,
      CatalogDependencyGroup dependencies,
      CatalogResource resource)
    {
      this.CatalogService = catalogService;
      this.m_parentPath = parentPath;
      this.m_childItem = childItem;
      this.Dependencies = dependencies;
      this.m_resource = resource;
    }

    public CatalogNode ParentNode { get; internal set; }

    public string ParentPath => this.m_parentPath;

    public string FullPath => this.m_fullPath;

    public CatalogResource Resource
    {
      get => this.m_resource;
      internal set => this.m_resource = value;
    }

    public bool IsDefault
    {
      get => this.m_isDefault;
      set => this.m_isDefault = value;
    }

    public CatalogNode CreateChild(Guid resourceTypeIdentifier, string resourceDisplayName) => this.CreateChild(new CatalogResource(resourceDisplayName, this.CatalogService.QueryResourceTypes((IEnumerable<Guid>) new Guid[1]
    {
      resourceTypeIdentifier
    }).FirstOrDefault<CatalogResourceType>() ?? throw new CatalogResourceTypeDoesNotExistException(TFCommonResources.CatalogResourceTypeDoesNotExist((object) resourceTypeIdentifier))));

    public CatalogNode CreateChild(CatalogResource existingResource) => new CatalogNode(this.CatalogService, this.ParentPath + this.ChildItem, Convert.ToBase64String(Guid.NewGuid().ToByteArray()), new CatalogDependencyGroup(), existingResource);

    public ReadOnlyCollection<CatalogNode> QueryChildren(
      IEnumerable<Guid> resourceTypeFilters,
      bool recurse,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryChildren(resourceTypeFilters, (IEnumerable<KeyValuePair<string, string>>) null, recurse, queryOptions);
    }

    public ReadOnlyCollection<CatalogNode> QueryChildren(
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      bool recurse,
      CatalogQueryOptions queryOptions)
    {
      if (string.IsNullOrEmpty(this.FullPath))
        throw new CatalogNodeDoesNotExistException(TFCommonResources.CatalogNodeDoesNotExist());
      return this.CatalogService.QueryNodes((IEnumerable<string>) new string[1]
      {
        this.FullPath + (recurse ? CatalogConstants.FullRecurseStars : CatalogConstants.SingleRecurseStar)
      }, resourceTypeFilters, propertyFilters, queryOptions);
    }

    public ReadOnlyCollection<CatalogNode> QueryParents(
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
      return this.CatalogService.QueryNodes((IEnumerable<string>) pathSpecs, resourceTypeFilters, queryOptions);
    }

    public ReadOnlyCollection<CatalogNode> QueryDependents(CatalogQueryOptions queryOptions)
    {
      if (string.IsNullOrEmpty(this.FullPath))
        throw new CatalogNodeDoesNotExistException(TFCommonResources.CatalogNodeDoesNotExist());
      return this.CatalogService.QueryDependents(this.FullPath, queryOptions);
    }

    public CatalogDependencyGroup Dependencies { get; internal set; }

    public CatalogTree CatalogTree => CatalogRoots.DetermineTree(this.ParentPath + this.ChildItem);

    public void ExpandDependencies()
    {
      if (string.IsNullOrEmpty(this.FullPath))
        return;
      CatalogNode catalogNode = this.CatalogService.QueryNodes((IEnumerable<string>) new string[1]
      {
        this.FullPath
      }, (IEnumerable<Guid>) null, CatalogQueryOptions.ExpandDependencies).FirstOrDefault<CatalogNode>();
      if (catalogNode != null)
        this.Dependencies = catalogNode.Dependencies;
      else
        this.Dependencies = new CatalogDependencyGroup();
    }

    internal string ChildItem
    {
      get => this.m_childItem;
      private set => this.m_childItem = value;
    }

    internal bool MatchedQuery => this.m_matchedQuery;

    internal CatalogNodeDependency[] NodeDependencies => this.m_nodeDependencies;

    internal bool NodeDependenciesIncluded => this.m_nodeDependenciesIncluded;

    internal CatalogService CatalogService { get; set; }

    internal CatalogChangeType ChangeType
    {
      get => (CatalogChangeType) this.m_changeTypeValue;
      set => this.m_changeTypeValue = (int) value;
    }

    internal void UpdateSelf(CatalogNode updatedNode)
    {
      this.m_fullPath = updatedNode.FullPath;
      this.m_parentPath = updatedNode.ParentPath;
      this.IsDefault = updatedNode.IsDefault;
      this.m_resource = updatedNode.Resource;
      this.m_resourceIdentifier = updatedNode.m_resourceIdentifier;
      this.Dependencies = updatedNode.Dependencies;
      this.ParentNode = updatedNode.ParentNode;
    }

    internal void InitializeFromWebService(CatalogService catalogService)
    {
      this.CatalogService = catalogService;
      this.m_parentPath = this.FullPath.Substring(0, this.FullPath.Length - CatalogConstants.MandatoryNodePathLength);
      this.ChildItem = this.FullPath.Substring(this.FullPath.Length - CatalogConstants.MandatoryNodePathLength);
      if (!this.m_nodeDependenciesIncluded)
        return;
      this.Dependencies = new CatalogDependencyGroup();
    }

    internal static CatalogNode PrepareForWebServiceSerialization(CatalogNode node)
    {
      node.m_resourceIdentifier = node.Resource.TempCorrelationId;
      List<CatalogNodeDependency> catalogNodeDependencyList = new List<CatalogNodeDependency>();
      node.m_nodeDependenciesIncluded = false;
      if (node.Dependencies != null)
      {
        foreach (KeyValuePair<string, CatalogNode> singleton in node.Dependencies.Singletons)
          catalogNodeDependencyList.Add(new CatalogNodeDependency()
          {
            FullPath = node.ParentPath + node.ChildItem,
            AssociationKey = singleton.Key,
            RequiredNodeFullPath = singleton.Value.ParentPath + singleton.Value.ChildItem,
            IsSingleton = true
          });
        foreach (KeyValuePair<string, IList<CatalogNode>> set in node.Dependencies.Sets)
        {
          foreach (CatalogNode catalogNode in (IEnumerable<CatalogNode>) set.Value)
            catalogNodeDependencyList.Add(new CatalogNodeDependency()
            {
              FullPath = node.ParentPath + node.ChildItem,
              AssociationKey = set.Key,
              RequiredNodeFullPath = catalogNode.ParentPath + catalogNode.ChildItem,
              IsSingleton = false
            });
        }
        node.m_nodeDependencies = catalogNodeDependencyList.ToArray();
        node.m_nodeDependenciesIncluded = true;
      }
      return node;
    }

    internal CatalogNode()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static CatalogNode FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      CatalogNode catalogNode = new CatalogNode();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 5:
                if (name == "ctype")
                {
                  catalogNode.m_changeTypeValue = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 7:
                if (name == "default")
                {
                  catalogNode.m_isDefault = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 8:
                if (name == "FullPath")
                {
                  catalogNode.m_fullPath = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 9:
                if (name == "ChildItem")
                {
                  catalogNode.m_childItem = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 10:
                if (name == "ParentPath")
                {
                  catalogNode.m_parentPath = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 12:
                if (name == "MatchedQuery")
                {
                  catalogNode.m_matchedQuery = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 18:
                if (name == "ResourceIdentifier")
                {
                  catalogNode.m_resourceIdentifier = XmlUtility.GuidFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 24:
                if (name == "NodeDependenciesIncluded")
                {
                  catalogNode.m_nodeDependenciesIncluded = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "NodeDependencies")
            catalogNode.m_nodeDependencies = Helper.ArrayOfCatalogNodeDependencyFromXml(serviceProvider, reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return catalogNode;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CatalogNode instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ChangeTypeValue: " + this.m_changeTypeValue.ToString());
      stringBuilder.AppendLine("  ChildItem: " + this.m_childItem);
      stringBuilder.AppendLine("  FullPath: " + this.m_fullPath);
      stringBuilder.AppendLine("  IsDefault: " + this.m_isDefault.ToString());
      stringBuilder.AppendLine("  MatchedQuery: " + this.m_matchedQuery.ToString());
      stringBuilder.AppendLine("  NodeDependencies: " + Helper.ArrayToString<CatalogNodeDependency>(this.m_nodeDependencies));
      stringBuilder.AppendLine("  NodeDependenciesIncluded: " + this.m_nodeDependenciesIncluded.ToString());
      stringBuilder.AppendLine("  ParentPath: " + this.m_parentPath);
      stringBuilder.AppendLine("  ResourceIdentifier: " + this.m_resourceIdentifier.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_changeTypeValue != 0)
        XmlUtility.ToXmlAttribute(writer, "ctype", this.m_changeTypeValue);
      if (this.m_childItem != null)
        XmlUtility.ToXmlAttribute(writer, "ChildItem", this.m_childItem);
      if (this.m_fullPath != null)
        XmlUtility.ToXmlAttribute(writer, "FullPath", this.m_fullPath);
      if (this.m_isDefault)
        XmlUtility.ToXmlAttribute(writer, "default", this.m_isDefault);
      if (this.m_matchedQuery)
        XmlUtility.ToXmlAttribute(writer, "MatchedQuery", this.m_matchedQuery);
      if (this.m_nodeDependenciesIncluded)
        XmlUtility.ToXmlAttribute(writer, "NodeDependenciesIncluded", this.m_nodeDependenciesIncluded);
      if (this.m_parentPath != null)
        XmlUtility.ToXmlAttribute(writer, "ParentPath", this.m_parentPath);
      if (this.m_resourceIdentifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "ResourceIdentifier", this.m_resourceIdentifier);
      Helper.ToXml(writer, "NodeDependencies", this.m_nodeDependencies, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, CatalogNode obj) => obj.ToXml(writer, element);
  }
}
