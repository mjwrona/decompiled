// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent49
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
  internal class TransformComponent49 : TransformComponent47
  {
    public override void InstallTransformDefinitions(IEnumerable<TransformDefinition> definitions = null)
    {
      definitions = ((IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All)).Where<TransformDefinition>((System.Func<TransformDefinition, bool>) (def => def.MinServiceVersion <= this.Version && (def.MaxServiceVersion ?? int.MaxValue) >= this.Version));
      TransformDefinitions.Validate(definitions);
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InstallTransformDefinitions", false);
      SqlMetaData[] schema = new SqlMetaData[12]
      {
        new SqlMetaData("TriggerTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TriggerOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("TriggerGroup", SqlDbType.UniqueIdentifier),
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
      this.BindTable("@definitions", "AnalyticsInternal.typ_TransformDefinition5", (IEnumerable<SqlDataRecord>) definitions.Select<TransformDefinition, SqlDataRecord>((Func<TransformDefinition, int, SqlDataRecord>) ((def, index) =>
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(schema);
        sqlDataRecord1.SetString(0, def.TriggerTable);
        sqlDataRecord1.SetString(1, def.TriggerOperation);
        if (def.TriggerGroup.HasValue)
          sqlDataRecord1.SetGuid(2, def.TriggerGroup.Value);
        sqlDataRecord1.SetString(3, def.TargetTable);
        sqlDataRecord1.SetString(4, def.Operation);
        sqlDataRecord1.SetString(5, def.SprocName);
        sqlDataRecord1.SetString(6, def.OperationScope);
        sqlDataRecord1.SetInt32(7, index);
        sqlDataRecord1.SetInt32(8, def.SprocVersion);
        sqlDataRecord1.SetInt32(9, def.TransformPriority);
        int? intervalMinutes = def.IntervalMinutes;
        if (intervalMinutes.HasValue)
        {
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          intervalMinutes = def.IntervalMinutes;
          int num = intervalMinutes.Value;
          sqlDataRecord2.SetInt32(10, num);
        }
        sqlDataRecord1.SetBoolean(11, def.EnableWithFeatureNames != null || def.DisableWithFeatureNames != null);
        return sqlDataRecord1;
      })).ToList<SqlDataRecord>());
      this.ExecuteNonQuery();
    }
  }
}
