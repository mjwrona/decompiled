// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ClassificationNodeComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ClassificationNodeComponent : SQLTable<ClassificationNode>
  {
    private const string ServiceName = "Search_ClassificationNode";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponentV2>(2),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponentV3>(3),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponentV4>(4)
    }, "Search_ClassificationNode");
    private static readonly SqlMetaData[] s_classificationNodeLookupTable = new SqlMetaData[9]
    {
      new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Path", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("NodeType", SqlDbType.Int),
      new SqlMetaData("ParentId", SqlDbType.Int),
      new SqlMetaData("Token", SqlDbType.VarChar, 512L),
      new SqlMetaData("SecurityHashcode", SqlDbType.Binary, 20L)
    };
    private static readonly SqlMetaData[] s_int32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.Int)
    };

    public ClassificationNodeComponent()
      : base()
    {
    }

    internal ClassificationNodeComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override List<ClassificationNode> AddTableEntityBatch(
      List<ClassificationNode> nodeList,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<ClassificationNode>(nameof (nodeList), (IList<ClassificationNode>) nodeList);
      this.PrepareStoredProcedure("Search.prc_AddOrUpdateClassificationNodes");
      this.BindClassificationNodeLookupTable("@itemList", (IEnumerable<ClassificationNode>) nodeList);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNode>((ObjectBinder<ClassificationNode>) new ClassificationNodeComponent.ClassificationNodeColumns());
        return resultCollection.GetCurrent<ClassificationNode>().Items;
      }
    }

    public override List<ClassificationNode> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      Stopwatch stopwatch = new Stopwatch();
      this.PrepareStoredProcedure("Search.prc_QueryClassificationNodes");
      this.BindInt("@count", count);
      TableEntityFilter propertyFilter;
      if (filterList.TryRetrieveFilter("NodeType", out propertyFilter))
        this.BindInt("@nodeType", Convert.ToInt32(propertyFilter.Value));
      if (filterList.TryRetrieveFilter("ParentId", out propertyFilter))
        this.BindInt("@parentId", Convert.ToInt32(propertyFilter.Value));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNode>((ObjectBinder<ClassificationNode>) new ClassificationNodeComponent.ClassificationNodeColumns());
        ObjectBinder<ClassificationNode> current = resultCollection.GetCurrent<ClassificationNode>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<ClassificationNode>();
      }
    }

    public virtual List<ClassificationNode> GetClassificationNodes(List<int> nodeIds)
    {
      this.ValidateNotNullOrEmptyList<int>(nameof (nodeIds), (IList<int>) nodeIds);
      this.PrepareStoredProcedure("Search.prc_QueryClassificationNodesById");
      this.BindClassificationNodeIdTable("@nodeIdList", (IEnumerable<int>) nodeIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNode>((ObjectBinder<ClassificationNode>) new ClassificationNodeComponent.ClassificationNodeColumns());
        ObjectBinder<ClassificationNode> current = resultCollection.GetCurrent<ClassificationNode>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<ClassificationNode>();
      }
    }

    public virtual ClassificationNode GetClassificationNode(int nodeId)
    {
      List<ClassificationNode> classificationNodes = this.GetClassificationNodes(new List<int>()
      {
        nodeId
      });
      return classificationNodes != null && classificationNodes.Count > 0 ? classificationNodes[0] : (ClassificationNode) null;
    }

    public virtual int DeleteClassificationNodes(List<int> classificationNodeIds) => 0;

    protected SqlParameter BindClassificationNodeLookupTable(
      string parameterName,
      IEnumerable<ClassificationNode> rows)
    {
      rows = rows ?? Enumerable.Empty<ClassificationNode>();
      System.Func<ClassificationNode, SqlDataRecord> selector = (System.Func<ClassificationNode, SqlDataRecord>) (node =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ClassificationNodeComponent.s_classificationNodeLookupTable);
        sqlDataRecord.SetGuid(0, node.ProjectId);
        sqlDataRecord.SetInt32(1, node.Id);
        sqlDataRecord.SetGuid(2, node.Identifier);
        sqlDataRecord.SetString(3, node.Name);
        sqlDataRecord.SetString(4, node.Path);
        sqlDataRecord.SetInt32(5, (int) node.NodeType);
        sqlDataRecord.SetInt32(6, node.ParentId);
        if (!string.IsNullOrEmpty(node.Token))
          sqlDataRecord.SetString(7, node.Token);
        else
          sqlDataRecord.SetDBNull(7);
        if (node.SecurityHashcode != null)
          sqlDataRecord.SetBytes(8, 0L, node.SecurityHashcode, 0, 20);
        else
          sqlDataRecord.SetDBNull(8);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ClassificationNodeDescriptor", rows.Select<ClassificationNode, SqlDataRecord>(selector));
    }

    protected SqlParameter BindClassificationNodeIdTable(
      string parameterName,
      IEnumerable<int> nodeIds)
    {
      nodeIds = nodeIds ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (nodeId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ClassificationNodeComponent.s_int32Table);
        sqlDataRecord.SetInt32(0, nodeId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Int32Table", nodeIds.Select<int, SqlDataRecord>(selector));
    }

    protected class ClassificationNodeColumns : ObjectBinder<ClassificationNode>
    {
      private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_identifier = new SqlColumnBinder("Identifier");
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_path = new SqlColumnBinder("Path");
      private SqlColumnBinder m_nodeType = new SqlColumnBinder("NodeType");
      private SqlColumnBinder m_parentId = new SqlColumnBinder("ParentId");
      private SqlColumnBinder m_token = new SqlColumnBinder("Token");
      private SqlColumnBinder m_securityHashcode = new SqlColumnBinder("SecurityHashcode");

      protected override ClassificationNode Bind()
      {
        if (this.m_id.IsNull((IDataReader) this.Reader))
          return (ClassificationNode) null;
        return new ClassificationNode()
        {
          ProjectId = this.m_projectId.GetGuid((IDataReader) this.Reader),
          Id = this.m_id.GetInt32((IDataReader) this.Reader),
          Identifier = this.m_identifier.GetGuid((IDataReader) this.Reader),
          Name = this.m_name.GetString((IDataReader) this.Reader, false),
          Path = this.m_path.GetString((IDataReader) this.Reader, false),
          NodeType = (ClassificationNodeType) this.m_nodeType.GetInt32((IDataReader) this.Reader),
          ParentId = this.m_parentId.GetInt32((IDataReader) this.Reader),
          Token = this.m_token.GetString((IDataReader) this.Reader, true),
          SecurityHashcode = this.m_securityHashcode.GetBytes((IDataReader) this.Reader, true)
        };
      }
    }
  }
}
