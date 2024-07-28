// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public abstract class TreeNode : ISecuredObject
  {
    private const string c_pathSeparator = "\\";
    private Lazy<Dictionary<string, TreeNode>> m_lazyChildren;
    private Lazy<string> m_lazyPath;
    private Lazy<string> m_lazyCssPath;
    protected IEqualityComparer<string> m_comparer;

    public TreeNode(IEqualityComparer<string> comparer = null)
    {
      TreeNode treeNode = this;
      this.m_comparer = comparer;
      this.m_lazyChildren = new Lazy<Dictionary<string, TreeNode>>((Func<Dictionary<string, TreeNode>>) (() => new Dictionary<string, TreeNode>(comparer ?? (IEqualityComparer<string>) TFStringComparer.CssNodeName)));
      this.m_lazyPath = new Lazy<string>((Func<string>) (() => treeNode.IsProject || treeNode.IsStructureSpecifier ? string.Empty : treeNode.Parent?.RelativePath + "\\" + treeNode.Name));
      this.m_lazyCssPath = new Lazy<string>((Func<string>) (() => treeNode.IsProject ? string.Empty : treeNode.Parent?.RelativeCssPath + "\\" + treeNode.Name));
    }

    public virtual Uri Uri => new Uri(LinkingUtilities.EncodeUri(new ArtifactId()
    {
      Tool = "Classification",
      ArtifactType = this.IsProject ? "TeamProject" : "Node",
      ToolSpecificId = this.CssNodeId.ToString("D")
    }));

    public int Id { get; protected set; }

    public int ParentId { get; protected set; }

    public TreeNode Parent { get; protected set; }

    public Guid ProjectId { get; protected set; }

    public TreeNode Project { get; protected set; }

    public virtual TreeStructureType Type { get; protected set; }

    protected string Name { get; set; }

    public int TypeId { get; protected set; }

    public Guid CssNodeId { get; protected set; }

    public DateTime? FinishDate { get; protected set; }

    public DateTime? StartDate { get; protected set; }

    public bool HasChildren => this.m_lazyChildren.IsValueCreated && this.m_lazyChildren.Value.Count > 0;

    public virtual IDictionary<string, TreeNode> Children => (IDictionary<string, TreeNode>) this.m_lazyChildren.Value;

    public virtual string RelativePath => this.m_lazyPath.Value;

    public string RelativeCssPath => this.m_lazyCssPath.Value;

    public bool IsProject => this.TypeId == -42;

    public bool IsStructureSpecifier => this.TypeId == -43;

    public virtual string GetName(IVssRequestContext requestContext)
    {
      if (!this.IsProject)
        return this.Name;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.WitContext().GetProjectName(this.CssNodeId);
    }

    public virtual string GetSanitizedName(IVssRequestContext requestContext) => this.IsStructureSpecifier ? this.Parent.GetName(requestContext) : this.GetName(requestContext);

    public virtual string GetPath(IVssRequestContext requestContext) => this.IsProject ? this.GetName(requestContext) : this.Project.GetName(requestContext) + this.RelativePath;

    public virtual string GetCssPath(IVssRequestContext requestContext) => this.IsProject ? this.GetName(requestContext) : "\\" + this.Project.GetName(requestContext) + this.RelativeCssPath;

    public TreeNode FindChildNodeByPath(string relativePath, TreeStructureType type)
    {
      ArgumentUtility.CheckForNull<string>(relativePath, nameof (relativePath));
      string[] strArray = relativePath.Split(new string[1]
      {
        "\\"
      }, StringSplitOptions.RemoveEmptyEntries);
      TreeNode childNode = this;
      if (childNode.IsProject && type != TreeStructureType.None)
        childNode = childNode.Children.Values.FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (x => x.Type == type));
      foreach (string name in strArray)
      {
        if (childNode == null || !childNode.TryGetChildNodeByName(name, out childNode))
          return (TreeNode) null;
      }
      return childNode;
    }

    public TreeNode FindChildNodeByCssPath(string relativePath)
    {
      ArgumentUtility.CheckForNull<string>(relativePath, nameof (relativePath));
      string[] strArray = relativePath.Split(new string[1]
      {
        "\\"
      }, StringSplitOptions.RemoveEmptyEntries);
      TreeNode childNode = this;
      foreach (string name in strArray)
      {
        if (!childNode.TryGetChildNodeByName(name, out childNode))
          return (TreeNode) null;
      }
      return childNode;
    }

    private bool TryGetChildNodeByName(string name, out TreeNode childNode)
    {
      if (this.m_lazyChildren.IsValueCreated)
        return this.m_lazyChildren.Value.TryGetValue(name, out childNode);
      childNode = (TreeNode) null;
      return false;
    }

    int ISecuredObject.RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    Guid ISecuredObject.NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    public string GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.ProjectId));

    public virtual TreeNode Clone() => throw new NotImplementedException();
  }
}
