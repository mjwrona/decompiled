// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TreeNode
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class TreeNode : BaseSecuredObjectModel
  {
    private List<TreeNode> m_children;

    public TreeNode(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    public int ParentId { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "guid")]
    public Guid Guid { get; set; }

    [DataMember(Name = "type")]
    public int Type { get; set; }

    [DataMember(Name = "structure")]
    public TreeStructureType StructureType { get; set; }

    [DataMember(Name = "children")]
    public IEnumerable<TreeNode> Children
    {
      get
      {
        if (this.m_children == null)
          return Enumerable.Empty<TreeNode>();
        this.m_children.Sort(TreeNode.\u003C\u003EO.\u003C0\u003E__NodeComparison ?? (TreeNode.\u003C\u003EO.\u003C0\u003E__NodeComparison = new Comparison<TreeNode>(CssUtils.NodeComparison)));
        return (IEnumerable<TreeNode>) this.m_children;
      }
    }

    [DataMember(Name = "startDate")]
    public DateTime? StartDate { get; set; }

    [DataMember(Name = "finishDate")]
    public DateTime? FinishDate { get; set; }

    public bool HasChildren => this.m_children != null && this.m_children.Count > 0;

    public TreeNode GetChild(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this.m_children == null)
        throw new IndexOutOfRangeException();
      return this.m_children.FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (n => StringComparer.OrdinalIgnoreCase.Equals(n.Name, name))) ?? throw new IndexOutOfRangeException();
    }

    internal void AddChild(TreeNode node)
    {
      if (this.m_children == null)
        this.m_children = new List<TreeNode>();
      this.m_children.Add(node);
    }

    internal static TreeNode CreateFrom(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode,
      ISecuredObject securedObject)
    {
      return new TreeNode(securedObject)
      {
        Id = treeNode.Id,
        ParentId = treeNode.ParentId,
        Name = treeNode.GetName(requestContext),
        Guid = treeNode.CssNodeId,
        Type = treeNode.TypeId,
        StructureType = treeNode.Type,
        StartDate = treeNode.StartDate,
        FinishDate = treeNode.FinishDate,
        m_children = treeNode.Children.Values.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, TreeNode>) (childNode => TreeNode.CreateFrom(requestContext, childNode, securedObject))).ToList<TreeNode>()
      };
    }
  }
}
