// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemClassificationNodeFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemClassificationNodeFactory
  {
    public static WorkItemClassificationNode Create(
      IVssRequestContext requestContext,
      TreeNode treeNode,
      int depth,
      bool includeLinks,
      bool getAllChildNodes = false,
      bool includeUrl = true)
    {
      if (treeNode.IsProject)
        throw new NotSupportedException();
      string classificationNodeUrl = includeUrl ? WitUrlHelper.GetClassificationNodeUrl(requestContext, treeNode) : (string) null;
      ReferenceLinks referenceLinks = (ReferenceLinks) null;
      if (includeLinks)
      {
        referenceLinks = new ReferenceLinks();
        referenceLinks.AddLink("self", classificationNodeUrl, (ISecuredObject) treeNode);
      }
      if (includeLinks && !treeNode.IsStructureSpecifier)
        referenceLinks.AddLink("parent", WitUrlHelper.GetClassificationNodeUrl(requestContext, treeNode.Parent), (ISecuredObject) treeNode);
      string sanitizedName = treeNode.GetSanitizedName(requestContext);
      WorkItemClassificationNode classificationNode1 = new WorkItemClassificationNode((ISecuredObject) treeNode);
      classificationNode1.Name = sanitizedName;
      classificationNode1.Id = treeNode.Id;
      classificationNode1.Identifier = treeNode.CssNodeId;
      classificationNode1.StructureType = treeNode.Type == TreeStructureType.Area ? TreeNodeStructureType.Area : TreeNodeStructureType.Iteration;
      classificationNode1.Url = classificationNodeUrl;
      classificationNode1.Links = referenceLinks;
      classificationNode1.HasChildren = new bool?(treeNode.HasChildren);
      classificationNode1.Path = treeNode.GetCssPath(requestContext);
      WorkItemClassificationNode classificationNode2 = classificationNode1;
      if (depth > 0 | getAllChildNodes)
      {
        List<WorkItemClassificationNode> list = treeNode.Children.Values.Select<TreeNode, WorkItemClassificationNode>((Func<TreeNode, WorkItemClassificationNode>) (tnc => WorkItemClassificationNodeFactory.Create(requestContext, tnc, depth - 1, false, getAllChildNodes, includeUrl))).ToList<WorkItemClassificationNode>();
        if (list.Any<WorkItemClassificationNode>())
          classificationNode2.Children = (IEnumerable<WorkItemClassificationNode>) list;
      }
      if (treeNode.StartDate.HasValue || treeNode.FinishDate.HasValue)
      {
        classificationNode2.Attributes = (IDictionary<string, object>) new Dictionary<string, object>();
        if (treeNode.StartDate.HasValue)
          classificationNode2.Attributes["startDate"] = (object) treeNode.StartDate.Value;
        if (treeNode.FinishDate.HasValue)
          classificationNode2.Attributes["finishDate"] = (object) treeNode.FinishDate.Value;
      }
      return classificationNode2;
    }
  }
}
