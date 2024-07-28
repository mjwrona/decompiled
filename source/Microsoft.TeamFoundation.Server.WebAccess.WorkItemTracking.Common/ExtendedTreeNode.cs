// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ExtendedTreeNode
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class ExtendedTreeNode : TreeNode
  {
    public ExtendedTreeNode(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(Name = "path")]
    public string Path { get; set; }

    public static ExtendedTreeNode Create(
      IVssRequestContext requestContext,
      string path,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode,
      ISecuredObject securedObject)
    {
      ExtendedTreeNode extendedTreeNode = new ExtendedTreeNode(securedObject);
      extendedTreeNode.Id = treeNode.Id;
      extendedTreeNode.ParentId = treeNode.ParentId;
      extendedTreeNode.Name = treeNode.GetName(requestContext);
      extendedTreeNode.Guid = treeNode.CssNodeId;
      extendedTreeNode.Type = treeNode.TypeId;
      extendedTreeNode.StructureType = treeNode.Type;
      extendedTreeNode.StartDate = treeNode.StartDate;
      extendedTreeNode.FinishDate = treeNode.FinishDate;
      extendedTreeNode.Path = path;
      return extendedTreeNode;
    }
  }
}
