// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ClassificationNode
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ClassificationNode
  {
    public const int RootNodeParentId = -1;
    private const string ClassificationNodeSecurityTokenFormat = "vstfs:///Classification/Node/{0}";

    public ClassificationNode()
    {
    }

    public ClassificationNode(
      WorkItemClassificationNode sourceNode,
      ClassificationNode parent,
      Guid projectId)
    {
      this.Id = sourceNode != null ? sourceNode.Id : throw new ArgumentNullException(nameof (sourceNode));
      this.Identifier = sourceNode.Identifier;
      this.Name = sourceNode.Name;
      this.NodeType = ClassificationNode.GetNodeType(sourceNode.StructureType);
      this.ParentId = parent != null ? parent.Id : -1;
      this.ProjectId = projectId;
      this.Path = this.ComputePath(parent);
      if (sourceNode.StructureType != TreeNodeStructureType.Area)
        return;
      this.Token = this.ComputeSecurityToken(parent);
    }

    public List<ClassificationNode> Children { get; private set; }

    public int Id { get; set; }

    public Guid Identifier { get; set; }

    public string Name { get; set; }

    public ClassificationNodeType NodeType { get; set; }

    public int ParentId { get; set; }

    public Guid ProjectId { get; set; }

    public string Path { get; set; }

    public string Token { get; set; }

    public byte[] SecurityHashcode { get; set; }

    public static List<ClassificationNode> Expand(
      Guid projectId,
      IEnumerable<WorkItemClassificationNode> workItemCssNodes,
      ClassificationNode searchParentCssNode = null)
    {
      List<ClassificationNode> classificationNodeList = new List<ClassificationNode>();
      if (searchParentCssNode != null)
        searchParentCssNode.Children = new List<ClassificationNode>();
      if (workItemCssNodes != null)
      {
        foreach (WorkItemClassificationNode workItemCssNode in workItemCssNodes)
        {
          ClassificationNode searchParentCssNode1 = new ClassificationNode(workItemCssNode, searchParentCssNode, projectId);
          searchParentCssNode?.Children?.Add(searchParentCssNode1);
          classificationNodeList.Add(searchParentCssNode1);
          classificationNodeList.AddRange((IEnumerable<ClassificationNode>) ClassificationNode.Expand(projectId, workItemCssNode.Children, searchParentCssNode1));
        }
      }
      return classificationNodeList;
    }

    public void RelateToItsDescendants(WorkItemClassificationNode tfsCssNode)
    {
      if (tfsCssNode == null)
        throw new ArgumentNullException(nameof (tfsCssNode));
      if (tfsCssNode.Id != this.Id)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Id of {0} does not match this node's Id.", (object) nameof (tfsCssNode))));
      this.Children = new List<ClassificationNode>();
      if (tfsCssNode.Children == null)
        return;
      foreach (WorkItemClassificationNode child in tfsCssNode.Children)
      {
        ClassificationNode classificationNode = new ClassificationNode(child, this, this.ProjectId);
        this.Children.Add(classificationNode);
        classificationNode.RelateToItsDescendants(child);
      }
    }

    public List<ClassificationNode> GetAllDescendantNodes()
    {
      if (this.Children == null)
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Value of {0} is null. Consider running {1} method before this to set the property correctly.", (object) "Children", (object) "RelateToItsDescendants")));
      List<ClassificationNode> allDescendantNodes = new List<ClassificationNode>()
      {
        this
      };
      foreach (ClassificationNode child in this.Children)
        allDescendantNodes.AddRange((IEnumerable<ClassificationNode>) child.GetAllDescendantNodes());
      return allDescendantNodes;
    }

    public override int GetHashCode() => this.Id;

    private static ClassificationNodeType GetNodeType(TreeNodeStructureType sourceNodeType)
    {
      if (sourceNodeType == TreeNodeStructureType.Area)
        return ClassificationNodeType.Area;
      return sourceNodeType == TreeNodeStructureType.Iteration ? ClassificationNodeType.Iteration : ClassificationNodeType.Unsupported;
    }

    private string ComputeSecurityToken(ClassificationNode parent)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstfs:///Classification/Node/{0}", (object) this.Identifier);
      return parent != null ? parent.Token + ":" + str : str;
    }

    private string ComputePath(ClassificationNode parent) => parent != null ? parent.Path + "\\" + this.Name : this.Name;
  }
}
