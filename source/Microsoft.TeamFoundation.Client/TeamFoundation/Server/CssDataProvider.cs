// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CssDataProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.ComponentModel;
using System.Resources;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CssDataProvider : ICssDataProvider
  {
    private CssNodeCollection _nodeCollection;
    private NodeInfo _cssRootNode;
    private string _initialPath;
    private static readonly string PATHSEPARATOR = "\\";
    private ResourceManager _resources;

    public CssDataProvider(
      TfsTeamProjectCollection tfs,
      string projectUri,
      string structureType,
      string initialPath,
      string[] nodeUrisToBeSkipped)
    {
      this._resources = new ResourceManager(this.GetType().Name, this.GetType().Assembly);
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(tfs, nameof (tfs));
      ICommonStructureService service = (ICommonStructureService) tfs.GetService(typeof (ICommonStructureService));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      ArgumentUtility.CheckStringForNullOrEmpty(structureType, nameof (structureType));
      if (!TFStringComparer.CssStructureType.Equals(structureType, "ProjectModelHierarchy") && !TFStringComparer.CssStructureType.Equals(structureType, "ProjectLifecycle"))
        throw new ArgumentException(this._resources.GetString("UnsupportedType"), nameof (structureType));
      foreach (NodeInfo listStructure in service.ListStructures(projectUri))
      {
        if (listStructure.StructureType == structureType)
        {
          this._cssRootNode = listStructure;
          break;
        }
      }
      if (this._cssRootNode == null)
        throw new InvalidOperationException(this._resources.GetString("CorruptedData"));
      if (nodeUrisToBeSkipped == null)
        nodeUrisToBeSkipped = Array.Empty<string>();
      string projectName = this._cssRootNode.Path.Substring(1, this._cssRootNode.Path.IndexOf(CssDataProvider.PATHSEPARATOR, 1, StringComparison.Ordinal) - 1);
      this.SetInitialPath(service, projectName, initialPath);
      this.LoadTree(service, projectName, projectUri, nodeUrisToBeSkipped);
    }

    public CssDataProvider(TfsTeamProjectCollection tfs, string projectUri, string structureType)
      : this(tfs, projectUri, structureType, (string) null, (string[]) null)
    {
    }

    public CssDataProvider(
      TfsTeamProjectCollection tfs,
      string projectUri,
      string structureType,
      string initialPath)
      : this(tfs, projectUri, structureType, initialPath, (string[]) null)
    {
    }

    private void LoadTree(
      ICommonStructureService proxy,
      string projectName,
      string projectUri,
      string[] nodeUrisToBeSkipped)
    {
      CssDataProviderNode dataProviderNode = new CssDataProviderNode(new NodeInfo(projectUri, projectName, string.Empty, Array.Empty<Property>(), string.Empty, projectUri, string.Empty));
      this._nodeCollection = new CssNodeCollection();
      XmlNode nodesXml = (XmlNode) proxy.GetNodesXml(new string[1]
      {
        this._cssRootNode.Uri
      }, true);
      dataProviderNode.Children = this.GetChildren(nodesXml.ChildNodes[0], nodeUrisToBeSkipped);
      this._nodeCollection.Add((CssNode) dataProviderNode);
    }

    private CssNodeCollection GetChildren(XmlNode cssTree, string[] nodeUrisToBeSkipped)
    {
      CssNodeCollection children = new CssNodeCollection();
      foreach (XmlNode xmlNode in cssTree)
      {
        if (TFStringComparer.CssXmlNodeName.Equals(xmlNode.Name, "Children"))
        {
          IEnumerator enumerator = xmlNode.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              XmlNode current = (XmlNode) enumerator.Current;
              NodeInfo ni = this.GetNodeInfo(current);
              Predicate<string> match = (Predicate<string>) (target => TFStringComparer.CssXmlNodeInfoUri.Equals(target, ni.Uri));
              if (Array.FindIndex<string>(nodeUrisToBeSkipped, 0, nodeUrisToBeSkipped.Length, match) == -1)
              {
                CssDataProviderNode dataProviderNode = new CssDataProviderNode(ni);
                children.Add((CssNode) dataProviderNode);
                dataProviderNode.Children = this.GetChildren(current, nodeUrisToBeSkipped);
              }
            }
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
        }
      }
      return children;
    }

    private NodeInfo GetNodeInfo(XmlNode xn) => new NodeInfo(xn.Attributes["NodeID"].Value, xn.Attributes["Name"].Value, xn.Attributes["StructureType"].Value, Array.Empty<Property>(), xn.Attributes["ParentID"].Value, xn.Attributes["ProjectID"].Value, xn.Attributes["Path"].Value);

    private void SetInitialPath(
      ICommonStructureService proxy,
      string projectName,
      string initialPath)
    {
      this._initialPath = string.Empty;
      if (string.IsNullOrEmpty(initialPath) || proxy.GetNodeFromPath(initialPath) == null)
        return;
      initialPath = initialPath.Remove(0, 1);
      int startIndex = initialPath.IndexOf(CssDataProvider.PATHSEPARATOR, StringComparison.Ordinal);
      int num = initialPath.IndexOf(CssDataProvider.PATHSEPARATOR, startIndex + 1, StringComparison.Ordinal);
      initialPath = num == -1 ? projectName : initialPath.Remove(startIndex, num - startIndex);
      this._initialPath = initialPath;
    }

    public NodeInfo TryGetCssNodeInfo(string path)
    {
      string name = this._nodeCollection[0].Name;
      if (string.IsNullOrEmpty(path) || TFStringComparer.CssTreePathName.Equals(path, name))
        return this._cssRootNode;
      string[] nodeNames = path.Split(CssDataProvider.PATHSEPARATOR.ToCharArray(), path.Length);
      CssNodeCollection children = (this._nodeCollection[0] as CssDataProviderNode).Children;
      dataProviderNode = (CssDataProviderNode) null;
      for (int i = 1; i < nodeNames.Length; i++)
      {
        Predicate<CssNode> match = (Predicate<CssNode>) (target => TFStringComparer.CssTreeNodeName.Equals(target.Name, nodeNames[i]));
        if (children.Find(match) is CssDataProviderNode dataProviderNode)
          children = dataProviderNode.Children;
        else
          break;
      }
      return dataProviderNode?.NodeInfo;
    }

    CssNodeCollection ICssDataProvider.GetChildNodes(CssNode parent)
    {
      CssNodeCollection cssNodeCollection = new CssNodeCollection();
      if (parent == null)
        return this._nodeCollection;
      return parent is CssDataProviderNode dataProviderNode ? dataProviderNode.Children : throw new ArgumentException(this._resources.GetString("ParentNotCssDataProviderNode"));
    }

    string ICssDataProvider.GetInitialPath() => this._initialPath;

    string ICssDataProvider.GetDefaultRootNodeName() => string.Empty;
  }
}
