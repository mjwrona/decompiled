// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent29
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent29 : TransformComponent28
  {
    public override void InstallTransformDefinitions(IEnumerable<TransformDefinition> definitions = null)
    {
      definitions = ((IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All)).Where<TransformDefinition>((System.Func<TransformDefinition, bool>) (def => def.MinServiceVersion <= this.Version && (def.MaxServiceVersion ?? int.MaxValue) >= this.Version));
      TransformDefinitions.Validate(definitions);
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InstallTransformDefinitions", false);
      SqlMetaData[] schema = new SqlMetaData[11]
      {
        new SqlMetaData("TriggerTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TriggerOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("TargetTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TargetOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("SProcName", SqlDbType.VarChar, 256L),
        new SqlMetaData("OperationScope", SqlDbType.VarChar, 10L),
        new SqlMetaData("TransformOrder", SqlDbType.Int),
        new SqlMetaData("SProcVersion", SqlDbType.Int),
        new SqlMetaData("TransformPriority", SqlDbType.Int),
        new SqlMetaData("IntervalMinutes", SqlDbType.Int),
        new SqlMetaData("FeatureFlagged", SqlDbType.Bit)
      };
      this.BindTable("@definitions", "AnalyticsInternal.typ_TransformDefinition4", (IEnumerable<SqlDataRecord>) definitions.Select<TransformDefinition, SqlDataRecord>((Func<TransformDefinition, int, SqlDataRecord>) ((def, index) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(schema);
        sqlDataRecord.SetString(0, def.TriggerTable);
        sqlDataRecord.SetString(1, def.TriggerOperation);
        sqlDataRecord.SetString(2, def.TargetTable);
        sqlDataRecord.SetString(3, def.Operation);
        sqlDataRecord.SetString(4, def.SprocName);
        sqlDataRecord.SetString(5, def.OperationScope);
        sqlDataRecord.SetInt32(6, index);
        sqlDataRecord.SetInt32(7, def.SprocVersion);
        sqlDataRecord.SetInt32(8, def.TransformPriority);
        if (def.IntervalMinutes.HasValue)
          sqlDataRecord.SetInt32(9, def.IntervalMinutes.Value);
        sqlDataRecord.SetBoolean(10, def.EnableWithFeatureNames != null || def.DisableWithFeatureNames != null);
        return sqlDataRecord;
      })).ToList<SqlDataRecord>());
      this.ExecuteNonQuery();
    }
  }
}
