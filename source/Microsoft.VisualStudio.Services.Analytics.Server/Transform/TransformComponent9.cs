// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent9
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent9 : TransformComponent7
  {
    public override void CreateTransformRework(
      long batchId,
      string triggerTableName,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory,
      bool ignoreWhenConsecutiveSprocFailures)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CreateProcessingRework");
      this.BindLong("@sourcebatchId", batchId);
      this.BindString("@triggerTableName", triggerTableName, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("@fromExistingState", fromExistingState);
      this.BindBoolean("@createDependentWork", createDependentWork);
      this.BindBoolean("@delayReworkPerAttemptHistory", delayReworkPerAttemptHistory);
      this.BindBoolean("@ignoreWhenConsecutiveSprocFailures", ignoreWhenConsecutiveSprocFailures);
      this.ExecuteNonQuery();
    }

    public override Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch GetNextTransformBatch(
      string tableName,
      int selectionOffset,
      IDictionary<string, string> transformSettings)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetNextTransformBatch");
      this.BindString("@tableName", tableName, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch>((ObjectBinder<Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch>) new TransformBatchColumns());
        return resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch>().Items.SingleOrDefault<Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch>();
      }
    }

    public override TransformResult TransformBatch(
      long batchId,
      IDictionary<string, string> transformSettings,
      int transformTimeoutSeconds)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_TransformBatch", transformTimeoutSeconds);
      this.BindLong("@batchId", batchId);
      this.BindKeyValuePairStringTable("@settings", (IEnumerable<KeyValuePair<string, string>>) transformSettings);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TransformResult>((ObjectBinder<TransformResult>) new TransformNextColumns4());
        List<TransformResult> items;
        try
        {
          items = resultCollection.GetCurrent<TransformResult>().Items;
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException(AnalyticsResources.UNABLE_TO_READ_TRANSFORM_RESULT(), ex);
        }
        if (resultCollection.TryNextResult())
          throw new InvalidOperationException(AnalyticsResources.UNEXPECTED_RESULT_SET());
        return items.SingleOrDefault<TransformResult>();
      }
    }

    public override TransformResult FailTransformBatch(
      long batchId,
      IDictionary<string, string> transformSettings,
      string failedMessage,
      int durationMS)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_FailTransformBatch");
      this.BindLong("@batchId", batchId);
      this.BindKeyValuePairStringTable("@settings", (IEnumerable<KeyValuePair<string, string>>) transformSettings);
      this.BindString("@failedMessage", failedMessage, 1000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@incDurationMS", durationMS);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TransformResult>((ObjectBinder<TransformResult>) new TransformNextColumns4());
        return resultCollection.GetCurrent<TransformResult>().Items.SingleOrDefault<TransformResult>();
      }
    }

    public override TransformResult TransformNext(
      string table,
      IDictionary<string, string> transformSettings)
    {
      throw new NotImplementedException();
    }

    public override void InstallTransformDefinitions(IEnumerable<TransformDefinition> definitions = null)
    {
      definitions = ((IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All)).Where<TransformDefinition>((System.Func<TransformDefinition, bool>) (def => def.MinServiceVersion <= this.Version && (def.MaxServiceVersion ?? int.MaxValue) >= this.Version));
      TransformDefinitions.Validate(definitions);
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InstallTransformDefinitions", false);
      SqlMetaData[] schema = new SqlMetaData[9]
      {
        new SqlMetaData("TriggerTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TriggerOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("TargetTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TargetOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("SProcName", SqlDbType.VarChar, 256L),
        new SqlMetaData("OperationScope", SqlDbType.VarChar, 10L),
        new SqlMetaData("TransformOrder", SqlDbType.Int),
        new SqlMetaData("SProcVersion", SqlDbType.Int),
        new SqlMetaData("TransformPriority", SqlDbType.Int)
      };
      this.BindTable("@definitions", "AnalyticsInternal.typ_TransformDefinition2", (IEnumerable<SqlDataRecord>) definitions.Select<TransformDefinition, SqlDataRecord>((Func<TransformDefinition, int, SqlDataRecord>) ((def, index) =>
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
        return sqlDataRecord;
      })).ToList<SqlDataRecord>());
      this.ExecuteNonQuery();
    }
  }
}
