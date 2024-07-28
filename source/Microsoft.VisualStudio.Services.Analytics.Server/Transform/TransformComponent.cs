// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent
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
  public class TransformComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[58]
    {
      (IComponentCreator) new ComponentCreator<TransformComponent>(1, true),
      (IComponentCreator) new ComponentCreator<TransformComponent2>(2),
      (IComponentCreator) new ComponentCreator<TransformComponent3>(3),
      (IComponentCreator) new ComponentCreator<TransformComponent4>(4),
      (IComponentCreator) new ComponentCreator<TransformComponent5>(5),
      (IComponentCreator) new ComponentCreator<TransformComponent6>(6),
      (IComponentCreator) new ComponentCreator<TransformComponent7>(7),
      (IComponentCreator) new ComponentCreator<TransformComponent7>(8),
      (IComponentCreator) new ComponentCreator<TransformComponent9>(9),
      (IComponentCreator) new ComponentCreator<TransformComponent10>(10),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(11),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(12),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(13),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(14),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(15),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(16),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(17),
      (IComponentCreator) new ComponentCreator<TransformComponent11>(18),
      (IComponentCreator) new ComponentCreator<TransformComponent19>(19),
      (IComponentCreator) new ComponentCreator<TransformComponent19>(20),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(21),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(22),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(23),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(24),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(25),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(26),
      (IComponentCreator) new ComponentCreator<TransformComponent21>(27),
      (IComponentCreator) new ComponentCreator<TransformComponent28>(28),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(29),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(30),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(31),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(32),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(33),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(34),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(35),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(36),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(37),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(38),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(39),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(40),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(41),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(42),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(43),
      (IComponentCreator) new ComponentCreator<TransformComponent29>(44),
      (IComponentCreator) new ComponentCreator<TransformComponent30>(45),
      (IComponentCreator) new ComponentCreator<TransformComponent30>(46),
      (IComponentCreator) new ComponentCreator<TransformComponent47>(47),
      (IComponentCreator) new ComponentCreator<TransformComponent47>(48),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(49),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(50),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(51),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(52),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(53),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(54),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(55),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(56),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(57),
      (IComponentCreator) new ComponentCreator<TransformComponent49>(58)
    }, "TransformService");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public TransformComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TransformComponent.s_sqlExceptionFactories;

    public virtual void InstallTransformDefinitions(IEnumerable<TransformDefinition> definitions = null)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InstallTransformDefinitions", false);
      int order = 0;
      SqlMetaData[] schema = new SqlMetaData[7]
      {
        new SqlMetaData("TriggerTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TriggerOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("TargetTableName", SqlDbType.VarChar, 64L),
        new SqlMetaData("TargetOperation", SqlDbType.VarChar, 10L),
        new SqlMetaData("SProcName", SqlDbType.VarChar, 256L),
        new SqlMetaData("OperationScope", SqlDbType.VarChar, 10L),
        new SqlMetaData("TransformOrder", SqlDbType.Int)
      };
      this.BindTable("@definitions", "AnalyticsInternal.typ_TransformDefinition", (IEnumerable<SqlDataRecord>) ((IEnumerable<TransformDefinition>) ((object) definitions ?? (object) TransformDefinitions.All)).Where<TransformDefinition>((System.Func<TransformDefinition, bool>) (def => def.MinServiceVersion <= this.Version && (def.MaxServiceVersion ?? int.MaxValue) >= this.Version)).Select<TransformDefinition, SqlDataRecord>((System.Func<TransformDefinition, SqlDataRecord>) (def =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(schema);
        sqlDataRecord.SetString(0, def.TriggerTable);
        sqlDataRecord.SetString(1, def.TriggerOperation);
        sqlDataRecord.SetString(2, def.TargetTable);
        sqlDataRecord.SetString(3, def.Operation);
        sqlDataRecord.SetString(4, def.SprocName);
        sqlDataRecord.SetString(5, def.OperationScope);
        sqlDataRecord.SetInt32(6, order++);
        return sqlDataRecord;
      })).ToList<SqlDataRecord>());
      this.ExecuteNonQuery();
    }

    public virtual TransformResult TransformNext(
      string table,
      IDictionary<string, string> transformSettings)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_TransformNextBatch");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<bool>((ObjectBinder<bool>) new TransformNextColumns());
        return resultCollection.GetCurrent<bool>().Items.Select<bool, TransformResult>((System.Func<bool, TransformResult>) (r => new TransformResult()
        {
          AllProcessingComplete = r
        })).SingleOrDefault<TransformResult>();
      }
    }

    public virtual void GenerateCalendar(string cultureName, int lcid) => throw new NotImplementedException();

    public virtual void CreateTransformRework(
      long sourceBatchId,
      string triggerTableName,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory,
      bool ignoreWhenConsecutiveSprocFailures)
    {
      throw new NotImplementedException();
    }

    public virtual void CreateTransformReworkFromUncorrectedBatches(
      string operationSproc,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory)
    {
      throw new NotImplementedException();
    }

    public virtual Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatch GetNextTransformBatch(
      string tableName,
      int selectionOffset,
      IDictionary<string, string> transformSettings)
    {
      throw new NotImplementedException();
    }

    public virtual TransformResult TransformBatch(
      long batchId,
      IDictionary<string, string> transformSettings,
      int transformTimeoutSeconds)
    {
      throw new NotImplementedException();
    }

    public virtual TransformResult FailTransformBatch(
      long batchId,
      IDictionary<string, string> transformSettings,
      string failedMessage,
      int durationMS)
    {
      throw new NotImplementedException();
    }

    public virtual CleanupDeletedTableResult CleanupDeletedTable(
      string tableName,
      bool continueToNextTable,
      int retainHistoryDays)
    {
      return new CleanupDeletedTableResult()
      {
        Complete = true,
        TableName = (string) null,
        DeletedRows = 0
      };
    }
  }
}
