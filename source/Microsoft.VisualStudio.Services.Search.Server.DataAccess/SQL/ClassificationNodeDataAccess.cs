// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.ClassificationNodeDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class ClassificationNodeDataAccess : SqlAzureDataAccess, IClassificationNodeDataAccess
  {
    public ClassificationNodeDataAccess()
    {
    }

    protected ClassificationNodeDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    public List<ClassificationNode> AddOrUpdateClassificationNodes(
      IVssRequestContext requestContext,
      List<ClassificationNode> classificationNodes,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<ClassificationNode>("ClassificationNode", (IList<ClassificationNode>) classificationNodes);
      using (ITable<ClassificationNode> table = this.m_tableAccessPlatform.GetTable<ClassificationNode>(requestContext))
        return this.InvokeTableOperation<List<ClassificationNode>>((Func<List<ClassificationNode>>) (() => table.AddTableEntityBatch(classificationNodes, merge)));
    }

    public List<ClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      int topCount)
    {
      TableEntityFilterList tableFilters = new TableEntityFilterList();
      using (ITable<ClassificationNode> table = this.m_tableAccessPlatform.GetTable<ClassificationNode>(requestContext))
        return this.InvokeTableOperation<List<ClassificationNode>>((Func<List<ClassificationNode>>) (() => table.RetriveTableEntityList(topCount, tableFilters)));
    }

    public ClassificationNode GetClassificationNode(IVssRequestContext requestContext, int nodeId)
    {
      using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
        return this.InvokeTableOperation<ClassificationNode>((Func<ClassificationNode>) (() => component.GetClassificationNode(nodeId)));
    }

    public List<ClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      ClassificationNodeType nodeType,
      int topCount)
    {
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("NodeType", "eq", ((int) nodeType).ToString()));
      TableEntityFilterList tableFilters = entityFilterList;
      using (ITable<ClassificationNode> table = this.m_tableAccessPlatform.GetTable<ClassificationNode>(requestContext))
        return this.InvokeTableOperation<List<ClassificationNode>>((Func<List<ClassificationNode>>) (() => table.RetriveTableEntityList(topCount, tableFilters)));
    }

    public int DeleteClassificationNodes(
      IVssRequestContext requestContext,
      List<int> classificationNodeIds)
    {
      this.ValidateNotNullOrEmptyList<int>(nameof (classificationNodeIds), (IList<int>) classificationNodeIds);
      int nodesDeleted = 0;
      using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
        this.InvokeTableOperation<List<ClassificationNode>>((Func<List<ClassificationNode>>) (() =>
        {
          nodesDeleted = component.DeleteClassificationNodes(classificationNodeIds);
          return (List<ClassificationNode>) null;
        }));
      return nodesDeleted;
    }
  }
}
