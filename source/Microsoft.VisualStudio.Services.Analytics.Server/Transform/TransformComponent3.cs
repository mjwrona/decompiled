// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent3
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent3 : TransformComponent2
  {
    public override void InstallTransformDefinitions(IEnumerable<TransformDefinition> definitions = null)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InstallTransformDefinitions", false);
      int order = 0;
      SqlMetaData[] schema = new SqlMetaData[8]
      {
        new SqlMetaData("TriggerTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TriggerOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("TargetTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TargetOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("SProcName", SqlDbType.VarChar, 256L),
        new SqlMetaData("OperationScope", SqlDbType.VarChar, 10L),
        new SqlMetaData("TransformOrder", SqlDbType.Int),
        new SqlMetaData("SProcVersion", SqlDbType.Int)
      };
      this.BindTable("@definitions", "AnalyticsInternal.typ_TransformDefinition1", (IEnumerable<SqlDataRecord>) ((IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All)).Where<TransformDefinition>((System.Func<TransformDefinition, bool>) (def => def.MinServiceVersion <= this.Version && (def.MaxServiceVersion ?? int.MaxValue) >= this.Version)).Select<TransformDefinition, SqlDataRecord>((System.Func<TransformDefinition, SqlDataRecord>) (def =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(schema);
        sqlDataRecord.SetString(0, def.TriggerTable);
        sqlDataRecord.SetString(1, def.TriggerOperation);
        sqlDataRecord.SetString(2, def.TargetTable);
        sqlDataRecord.SetString(3, def.Operation);
        sqlDataRecord.SetString(4, def.SprocName);
        sqlDataRecord.SetString(5, def.OperationScope);
        sqlDataRecord.SetInt32(6, order++);
        sqlDataRecord.SetInt32(7, def.SprocVersion);
        return sqlDataRecord;
      })).ToList<SqlDataRecord>());
      this.ExecuteNonQuery();
    }
  }
}
