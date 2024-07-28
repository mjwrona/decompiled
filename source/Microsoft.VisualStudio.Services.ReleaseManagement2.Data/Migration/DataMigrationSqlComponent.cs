// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration.DataMigrationSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration
{
  public class DataMigrationSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<DataMigrationSqlComponent>(0, true)
    }, "ReleaseManagementDataMigration", "ReleaseManagement");
    private static readonly SqlMetaData[] DeployPhaseSnapshotSqlMetadata = new SqlMetaData[4]
    {
      new SqlMetaData("ReleaseId", SqlDbType.Int),
      new SqlMetaData("ReleaseEnvironmentId", SqlDbType.Int),
      new SqlMetaData("DeployPhaseSnapshot", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SnapshotRevision", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] ReleaseTriggerSqlMetadata = new SqlMetaData[7]
    {
      new SqlMetaData("ReleaseDefinitionId", SqlDbType.Int),
      new SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new SqlMetaData("TriggerEntityId", SqlDbType.Int),
      new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TargetEnvironmentId", SqlDbType.Int),
      new SqlMetaData("TriggerContent", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ReleaseDefinitionRevision", SqlDbType.Int)
    };

    public IEnumerable<ReleaseEnvironmentDeployPhasesSnapshotData> GetReleaseEnvironmentDeployPhaseSnapshots(
      Guid projectId,
      int releaseEnvironmentContinuationToken,
      int maxReleaseEnvironments,
      bool isMigrated,
      Guid taskId)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseEnvironmentDeployPhaseSnapshots", projectId);
      this.BindInt(nameof (releaseEnvironmentContinuationToken), releaseEnvironmentContinuationToken);
      this.BindInt(nameof (maxReleaseEnvironments), maxReleaseEnvironments);
      this.BindBoolean(nameof (isMigrated), isMigrated);
      this.BindNullableGuid(nameof (taskId), taskId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentDeployPhasesSnapshotData>((ObjectBinder<ReleaseEnvironmentDeployPhasesSnapshotData>) new ReleaseEnvironmentDeployPhaseSnapshotRowBinder());
        return (IEnumerable<ReleaseEnvironmentDeployPhasesSnapshotData>) resultCollection.GetCurrent<ReleaseEnvironmentDeployPhasesSnapshotData>().Items;
      }
    }

    public void BulkUpdateReleaseEnvironmentDeployPhaseSnapshots(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentDeployPhasesSnapshotData> updateData)
    {
      this.PrepareStoredProcedure("Release.prc_BulkUpdateReleaseEnvironmentDeployPhaseSnapshots", projectId);
      this.BindTable("environmentSnapshotData", "Release.typ_ReleaseEnvironmentDeployPhaseSnapshotTable", DataMigrationSqlComponent.GetSqlDataRecords(updateData));
      this.ExecuteScalar();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Need to be a method")]
    public IEnumerable<int> GetReleaseTriggerDataspaceIds()
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseTriggerDataspaceIds");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ReleaseTriggerDataspaceIdDataBinder());
        return (IEnumerable<int>) resultCollection.GetCurrent<int>().Items;
      }
    }

    public IEnumerable<ReleaseTriggerData> GetReleaseTriggers(int dataspaceId)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseTriggers");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null))
      {
        resultCollection.AddBinder<ReleaseTriggerData>((ObjectBinder<ReleaseTriggerData>) new ReleaseTriggerDataBinder());
        return (IEnumerable<ReleaseTriggerData>) resultCollection.GetCurrent<ReleaseTriggerData>().Items;
      }
    }

    public void BulkUpdateReleaseTriggers(
      int dataspaceId,
      IEnumerable<ReleaseTriggerData> updateData)
    {
      this.PrepareStoredProcedure("Release.prc_BulkUpdateReleaseTriggers");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindTable("triggers", "Release.typ_ReleaseTriggerTable", DataMigrationSqlComponent.GetSqlDataRecords(updateData));
      this.ExecuteNonQuery();
    }

    public IList<ReleaseSchedule> GetReleaseDefinitionSchedules(int dataspaceId)
    {
      this.PrepareStoredProcedure("Release.prc_GetScheduledReleaseTriggers");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null))
      {
        SqlColumnBinder triggerContentBinder = new SqlColumnBinder("TriggerContent");
        resultCollection.AddBinder<ReleaseSchedule>((ObjectBinder<ReleaseSchedule>) new SimpleObjectBinder<ReleaseSchedule>((System.Func<IDataReader, ReleaseSchedule>) (reader => JsonConvert.DeserializeObject<ReleaseSchedule>(triggerContentBinder.GetString(reader, false)))));
        return (IList<ReleaseSchedule>) resultCollection.GetCurrent<ReleaseSchedule>().Items;
      }
    }

    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<ReleaseEnvironmentDeployPhasesSnapshotData> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseEnvironmentDeployPhasesSnapshotData>();
      foreach (ReleaseEnvironmentDeployPhasesSnapshotData phasesSnapshotData in rows.Where<ReleaseEnvironmentDeployPhasesSnapshotData>((System.Func<ReleaseEnvironmentDeployPhasesSnapshotData, bool>) (r => r != null)))
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(DataMigrationSqlComponent.DeployPhaseSnapshotSqlMetadata);
        sqlDataRecord.SetInt32(0, phasesSnapshotData.ReleaseId);
        sqlDataRecord.SetInt32(1, phasesSnapshotData.ReleaseEnvironmentId);
        sqlDataRecord.SetString(2, JsonConvert.SerializeObject((object) phasesSnapshotData.DeployPhaseSnapshots));
        sqlDataRecord.SetInt32(3, phasesSnapshotData.DeployPhaseSnapshotRevision);
        yield return sqlDataRecord;
      }
    }

    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<ReleaseTriggerData> rows)
    {
      List<SqlDataRecord> sqlDataRecords = new List<SqlDataRecord>();
      if (rows == null)
        return (IEnumerable<SqlDataRecord>) sqlDataRecords;
      foreach (ReleaseTriggerData releaseTriggerData in rows.Where<ReleaseTriggerData>((System.Func<ReleaseTriggerData, bool>) (r => r != null)))
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(DataMigrationSqlComponent.ReleaseTriggerSqlMetadata);
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        int releaseDefinitionId = releaseTriggerData.ReleaseDefinitionId;
        sqlDataRecord2.SetInt32(ordinal1, releaseDefinitionId);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int triggerType = (int) (byte) releaseTriggerData.TriggerType;
        sqlDataRecord3.SetByte(ordinal2, (byte) triggerType);
        int? triggerEntityId = releaseTriggerData.TriggerEntityId;
        int num4;
        if (triggerEntityId.HasValue)
        {
          SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
          int ordinal3 = num3;
          num4 = ordinal3 + 1;
          triggerEntityId = releaseTriggerData.TriggerEntityId;
          int num5 = triggerEntityId.Value;
          sqlDataRecord4.SetInt32(ordinal3, num5);
        }
        else
        {
          SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
          int ordinal4 = num3;
          num4 = ordinal4 + 1;
          sqlDataRecord5.SetDBNull(ordinal4);
        }
        SqlDataRecord sqlDataRecord6 = sqlDataRecord1;
        int ordinal5 = num4;
        int num6 = ordinal5 + 1;
        string alias = releaseTriggerData.Alias;
        sqlDataRecord6.SetString(ordinal5, alias);
        SqlDataRecord sqlDataRecord7 = sqlDataRecord1;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        int targetEnvironmentId = releaseTriggerData.TargetEnvironmentId;
        sqlDataRecord7.SetInt32(ordinal6, targetEnvironmentId);
        int num8;
        if (releaseTriggerData.TriggerContent != null)
        {
          SqlDataRecord sqlDataRecord8 = sqlDataRecord1;
          int ordinal7 = num7;
          num8 = ordinal7 + 1;
          string triggerContent = releaseTriggerData.TriggerContent;
          sqlDataRecord8.SetString(ordinal7, triggerContent);
        }
        else
        {
          SqlDataRecord sqlDataRecord9 = sqlDataRecord1;
          int ordinal8 = num7;
          num8 = ordinal8 + 1;
          sqlDataRecord9.SetDBNull(ordinal8);
        }
        SqlDataRecord sqlDataRecord10 = sqlDataRecord1;
        int ordinal9 = num8;
        int num9 = ordinal9 + 1;
        int definitionRevision = releaseTriggerData.ReleaseDefinitionRevision;
        sqlDataRecord10.SetInt32(ordinal9, definitionRevision);
        sqlDataRecords.Add(sqlDataRecord1);
      }
      return (IEnumerable<SqlDataRecord>) sqlDataRecords;
    }
  }
}
