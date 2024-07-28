// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ClassificationNodeComponentV3
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ClassificationNodeComponentV3 : ClassificationNodeComponentV2
  {
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
      new SqlMetaData("SecurityHashcode", SqlDbType.Binary, 32L)
    };

    public ClassificationNodeComponentV3()
    {
    }

    internal ClassificationNodeComponentV3(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    protected SqlParameter BindClassificationNodeLookupTableV3(
      string parameterName,
      IEnumerable<ClassificationNode> rows)
    {
      rows = rows ?? Enumerable.Empty<ClassificationNode>();
      System.Func<ClassificationNode, SqlDataRecord> selector = (System.Func<ClassificationNode, SqlDataRecord>) (node =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ClassificationNodeComponentV3.s_classificationNodeLookupTable);
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
          sqlDataRecord.SetBytes(8, 0L, node.SecurityHashcode, 0, node.SecurityHashcode.Length);
        else
          sqlDataRecord.SetDBNull(8);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ClassificationNodeDescriptorV2", rows.Select<ClassificationNode, SqlDataRecord>(selector));
    }

    public override List<ClassificationNode> AddTableEntityBatch(
      List<ClassificationNode> nodeList,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<ClassificationNode>(nameof (nodeList), (IList<ClassificationNode>) nodeList);
      this.PrepareStoredProcedure("Search.prc_AddOrUpdateClassificationNodes");
      this.BindClassificationNodeLookupTableV3("@itemList", (IEnumerable<ClassificationNode>) nodeList);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNode>((ObjectBinder<ClassificationNode>) new ClassificationNodeComponent.ClassificationNodeColumns());
        return resultCollection.GetCurrent<ClassificationNode>().Items;
      }
    }
  }
}
