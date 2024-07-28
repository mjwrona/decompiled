// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[16]
    {
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent>(1),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent2>(2),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent2>(3),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent4>(4),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent5>(5),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent6>(6),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent7>(7),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent8>(8),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent9>(9),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent10>(10),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent11>(11),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent12>(12),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent13>(13),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent14>(14),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent15>(15),
      (IComponentCreator) new ComponentCreator<TeamConfigurationComponent16>(16)
    }, "TeamConfiguration", "WorkItem");
    protected static readonly Guid WholeTeamId = Guid.Empty;
    private static SqlMetaData[] typ_TeamConfigurationTeamFieldTable = new SqlMetaData[3]
    {
      new SqlMetaData("TeamFieldValue", SqlDbType.NVarChar, 256L),
      new SqlMetaData("IncludeChildren", SqlDbType.Bit),
      new SqlMetaData("Order", SqlDbType.Int)
    };
    private static SqlMetaData[] typ_TeamConfigurationCapacityTable = new SqlMetaData[3]
    {
      new SqlMetaData("TeamMemberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Capacity", SqlDbType.Real),
      new SqlMetaData("Activity", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static SqlMetaData[] typ_TeamConfigurationCapacityDaysOffRangeTable = new SqlMetaData[3]
    {
      new SqlMetaData("TeamMemberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StartTime", SqlDbType.DateTime),
      new SqlMetaData("EndTime", SqlDbType.DateTime)
    };

    public static TeamConfigurationComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<TeamConfigurationComponent>();

    internal virtual bool IsMultipleActivityPerMemberSupported() => false;

    internal virtual TeamConfiguration GetTeamConfiguration(Guid projectId, Guid teamId)
    {
      this.PrepareStoredProcedure("prc_GetTeamConfiguration");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      TeamConfiguration settings = new TeamConfiguration();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFieldRow>((ObjectBinder<TeamFieldRow>) new TeamFieldRowBinder());
        resultCollection.AddBinder<TeamIterationRow>((ObjectBinder<TeamIterationRow>) new IterationRowBinder());
        resultCollection.AddBinder<TeamConfigurationPropertyRow>((ObjectBinder<TeamConfigurationPropertyRow>) new TeamConfigurationPropertyRowBinder());
        TeamConfigurationComponent.FillTeamConfigurationTeamFields(settings, (IEnumerable<TeamFieldRow>) resultCollection.GetCurrent<TeamFieldRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent.FillTeamConfigurationIterations(settings, (IEnumerable<TeamIterationRow>) resultCollection.GetCurrent<TeamIterationRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent.FillTeamConfigurationProperties(settings, (IEnumerable<TeamConfigurationPropertyRow>) resultCollection.GetCurrent<TeamConfigurationPropertyRow>().Items);
      }
      return settings;
    }

    internal virtual IList<ITeamSettings> GetTeamConfigurations(IList<Tuple<Guid, Guid>> teams) => throw new NotSupportedException();

    internal TeamCapacity GetTeamIterationCapacity(Guid projectId, Guid teamId, Guid iterationId)
    {
      this.PrepareStoredProcedure("prc_GetTeamConfigurationIterationCapacity");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuid("@iterationId", iterationId);
      TeamCapacity capacity = new TeamCapacity();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamCapacityRow>((ObjectBinder<TeamCapacityRow>) new TeamCapacityRowBinder());
        resultCollection.AddBinder<TeamCapacityDaysOffRangeRow>((ObjectBinder<TeamCapacityDaysOffRangeRow>) new TeamCapacityDaysOffRangeRowBinder());
        TeamConfigurationComponent.FillTeamCapacity(capacity, (IEnumerable<TeamCapacityRow>) resultCollection.GetCurrent<TeamCapacityRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent.FillTeamCapacityDaysOff(capacity, (IEnumerable<TeamCapacityDaysOffRangeRow>) resultCollection.GetCurrent<TeamCapacityDaysOffRangeRow>().Items);
      }
      return capacity;
    }

    internal void SaveTeamFields(
      Guid projectId,
      Guid teamId,
      ITeamFieldValue[] fieldValues,
      int defaultValueIndex)
    {
      this.PrepareStoredProcedure("prc_SetTeamConfigurationTeamFields");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindTeamConfigurationTeamFieldTable("@teamFieldTable", TeamConfigurationComponent.GetTeamFieldRows(fieldValues));
      this.BindKeyValuePairStringTableNullable("@propertyTable", TeamConfigurationComponent.GetTeamFieldProperties(defaultValueIndex));
      this.ExecuteNonQuery();
    }

    internal virtual void SaveBacklogIterations(
      Guid projectId,
      Guid teamId,
      IEnumerable<Guid> iterationIds,
      Guid rootIterationId)
    {
      this.PrepareStoredProcedure("prc_SetTeamConfigurationBacklogIterations");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuidTable("@iterationIdTable", iterationIds);
      this.BindKeyValuePairStringTableNullable("@propertyTable", TeamConfigurationComponent.GetIterationProperties(rootIterationId));
      this.ExecuteNonQuery();
    }

    internal virtual void DeleteTeamIterations(
      Guid projectId,
      Guid teamId,
      IEnumerable<Guid> iterationIds)
    {
      string sqlStatement = "SET NOCOUNT ON\r\nSET XACT_ABORT ON\r\n\r\nDECLARE @status INT; -- error reporting\r\nDECLARE @errorMessage NVARCHAR(2048); -- error reporting\r\nDECLARE @tfError NVARCHAR(255); -- used by macro RAISETFSERROR\r\n\r\nBEGIN TRAN\r\n\r\nBEGIN TRY;\r\n    -- Delete iterations for the team\r\n    DELETE FROM tbl_TeamConfigurationIterations \r\n           WHERE PartitionId = @partitionId AND DataspaceID = @dataspaceId\r\n                 AND TeamId = @teamId \r\n                 AND IterationId IN (SELECT Id FROM @iterationIdTable)\r\n    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN));\r\n    EXEC dbo.prc_iiSendNotification @partitionId, '5BC78524-EA1A-411B-A30F-DD77B3C44A04', @teamId\r\n\r\nEND TRY\r\nBEGIN CATCH\r\n     SET @status = ERROR_NUMBER();\r\n\r\n    ROLLBACK TRAN\r\n\r\n    SELECT @errorMessage = dbo.func_FormatErrorMessage(@status, ERROR_MESSAGE(), ERROR_LINE())\r\n    SET @tfError = dbo.func_GetMessage(1300001); RAISERROR(@tfError, 16, -1, @errorMessage)\r\n    RETURN\r\nEND CATCH\r\n\r\nCOMMIT TRAN";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuidTable("@iterationIdTable", iterationIds);
      this.ExecuteNonQuery();
    }

    internal void SaveIterationCapacity(
      Guid projectId,
      Guid teamId,
      Guid iterationId,
      TeamCapacity capacity)
    {
      this.PrepareStoredProcedure("prc_SetTeamConfigurationIterationCapacity");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuid("@iterationId", iterationId);
      this.BindTeamConfigurationCapacityTable("@capacityTable", this.ConvertToTeamCapacityRows(capacity));
      this.BindTeamConfigurationCapacityDaysOffRangeTable("@dateRangeTable", TeamConfigurationComponent.GetTeamCapacityDaysOffRangeRows(capacity));
      this.ExecuteNonQuery();
    }

    internal virtual void SaveIterationTeamDaysOff(
      Guid projectId,
      Guid teamId,
      Guid iterationId,
      IEnumerable<DateRange> teamDaysOff)
    {
      foreach (DateRange dateRange in teamDaysOff)
        dateRange.ValidateForSql();
      string sqlStatement = "SET NOCOUNT ON\r\nSET XACT_ABORT ON\r\n\r\nDECLARE @status INT; -- error reporting\r\nDECLARE @errorMessage NVARCHAR(2048); -- error reporting\r\nDECLARE @tfError NVARCHAR(255); -- used by macro RAISETFSERROR\r\n\r\nBEGIN TRAN\r\n\r\nBEGIN TRY;\r\n    -- Sync capacity with @dateRangeTable. Delete non-matching date range rows for same team/iteration\r\n    WITH Partitioned_TeamConfigurationCapacityDaysOffRange AS (SELECT * FROM tbl_TeamConfigurationCapacityDaysOffRange WHERE PartitionId = @partitionId AND TeamMemberId='00000000-0000-0000-0000-000000000000')\r\n    MERGE Partitioned_TeamConfigurationCapacityDaysOffRange AS T\r\n    USING @dateRangeTable AS S ON (T.DataspaceId = @dataspaceId AND T.TeamId = @teamId AND T.IterationId = @iterationId AND T.TeamMemberId = S.TeamMemberId AND T.StartTime = S.StartTime) \r\n    WHEN MATCHED AND T.EndTime <> S.EndTime THEN \r\n        UPDATE SET T.EndTime = S.EndTime\r\n    WHEN NOT MATCHED BY TARGET THEN \r\n        INSERT (PartitionId, DataspaceId, TeamId, IterationId, TeamMemberId, StartTime, EndTime) \r\n        VALUES (@partitionId, @dataspaceId, @teamId, @iterationId, S.TeamMemberId, S.StartTime, S.EndTime)\r\n    WHEN NOT MATCHED BY SOURCE AND (T.DataspaceId = @dataspaceId AND T.TeamId = @teamId AND T.IterationId = @iterationId) THEN\r\n        DELETE\r\n    OPTION (OPTIMIZE FOR (@partitionId UNKNOWN));\r\nEND TRY\r\nBEGIN CATCH\r\n    SET @status = ERROR_NUMBER();\r\n    ROLLBACK TRAN   \r\n    RETURN \r\nEND CATCH\r\n\r\nCOMMIT TRAN";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuid("@iterationId", iterationId);
      this.BindTeamConfigurationCapacityDaysOffRangeTable("@dateRangeTable", (IEnumerable<TeamCapacityDaysOffRangeRow>) TeamConfigurationComponent.GetDateRangeRows(TeamConfigurationComponent.WholeTeamId, teamDaysOff));
      this.ExecuteNonQuery();
    }

    internal void DeleteTeamConfiguration(IEnumerable<Guid> teamIds)
    {
      this.PrepareStoredProcedure("prc_DeleteTeamConfiguration");
      this.BindGuidTable("@teamIdsTable", teamIds);
      this.ExecuteNonQuery();
    }

    internal virtual void SaveTeamWeekends(Guid projectId, Guid teamId, ITeamWeekends weekends)
    {
    }

    internal virtual void SetTeamConfigurationProperties(
      Guid projectId,
      Guid teamId,
      IEnumerable<KeyValuePair<string, string>> kvps)
    {
      throw new NotSupportedException();
    }

    protected virtual SqlParameter BindTeamConfigurationTeamFieldTable(
      string parameterName,
      IEnumerable<TeamFieldRow> rows)
    {
      return this.BindTable(parameterName, "typ_TeamConfigurationTeamFieldTable", rows.Select<TeamFieldRow, SqlDataRecord>((System.Func<TeamFieldRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamConfigurationComponent.typ_TeamConfigurationTeamFieldTable);
        sqlDataRecord.SetString(0, row.TeamFieldValue);
        sqlDataRecord.SetBoolean(1, row.IncludeChildren);
        sqlDataRecord.SetInt32(2, row.Order);
        return sqlDataRecord;
      })));
    }

    protected virtual SqlParameter BindTeamConfigurationCapacityTable(
      string parameterName,
      IEnumerable<TeamCapacityRow> rows)
    {
      return this.BindTable(parameterName, "typ_TeamConfigurationCapacityTable", rows.Select<TeamCapacityRow, SqlDataRecord>((System.Func<TeamCapacityRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(TeamConfigurationComponent.typ_TeamConfigurationCapacityTable);
        record.SetGuid(0, row.TeamMemberId);
        record.SetFloat(1, row.Capacity);
        record.SetNullableString(2, row.Activity);
        return record;
      })));
    }

    protected virtual SqlParameter BindTeamConfigurationCapacityDaysOffRangeTable(
      string parameterName,
      IEnumerable<TeamCapacityDaysOffRangeRow> rows)
    {
      return this.BindTable(parameterName, "typ_TeamConfigurationCapacityDaysOffRangeTable", (IEnumerable<SqlDataRecord>) rows.Select<TeamCapacityDaysOffRangeRow, SqlDataRecord>((System.Func<TeamCapacityDaysOffRangeRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TeamConfigurationComponent.typ_TeamConfigurationCapacityDaysOffRangeTable);
        sqlDataRecord.SetGuid(0, row.TeamMemberId);
        sqlDataRecord.SetDateTime(1, row.StartTime);
        sqlDataRecord.SetDateTime(2, row.EndTime);
        return sqlDataRecord;
      })).ToList<SqlDataRecord>());
    }

    protected virtual void BindDataspaceId(Guid dataspaceIdentifier)
    {
    }

    internal virtual void SetBacklogVisibilitiesProperty(
      Guid projectId,
      Guid teamId,
      IDictionary<string, bool> visibilities)
    {
    }

    internal virtual void SetCFDProperties(
      Guid projectId,
      Guid teamId,
      IDictionary<string, CumulativeFlowDiagramSettings> cfdSettings)
    {
    }

    internal virtual IEnumerable<Tuple<Guid, Guid>> GetTeamIterationsWithCapacitiesForCollection() => Enumerable.Empty<Tuple<Guid, Guid>>();

    internal virtual IEnumerable<TeamCapacity> GetBulkCapacityData(
      IEnumerable<Tuple<Guid, Guid>> teamIdAndIterationIdPairs)
    {
      return Enumerable.Empty<TeamCapacity>();
    }

    internal virtual IEnumerable<Tuple<Guid, Guid, int>> GetChangedTeamConfigurationCapacity(
      int watermark)
    {
      throw new NotImplementedException();
    }

    internal virtual IEnumerable<Tuple<Guid, int>> GetChangedTeamSettings(int watermark) => throw new NotImplementedException();

    internal virtual IDictionary<Guid, IDictionary<string, bool>> GetTeamBacklogVisibilitiesForProject(
      Guid projectId)
    {
      throw new NotImplementedException();
    }

    internal virtual IEnumerable<Guid> GetTeamsWithSubscribedIterations(Guid projectId) => throw new NotImplementedException();

    internal virtual IDictionary<Guid, IEnumerable<Guid>> GetIterationSubscriptionsForTeams(
      Guid projectId,
      IEnumerable<Guid> teamIds)
    {
      throw new NotImplementedException();
    }

    internal virtual void CleanupDeletedTeamConfiguration()
    {
    }

    internal virtual IDictionary<string, IDictionary<Guid, bool>> GetAllTeamFieldsForProject(
      Guid projectId)
    {
      this.PrepareDynamicProcedure("dynprc_getAllTeamFieldsForProject", "\r\nSELECT TeamId, TeamFieldValue, IncludeChildren\r\nFROM tbl_TeamConfigurationTeamFields\r\nWHERE DataspaceId = @dataspaceId\r\nAND PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN));");
      this.BindDataspaceId(projectId);
      Dictionary<string, IDictionary<Guid, bool>> result = new Dictionary<string, IDictionary<Guid, bool>>();
      TeamConfigurationComponent.FillTeamConfigurationTeamFields((IDictionary<string, IDictionary<Guid, bool>>) result, this.ExecuteUnknown<IList<TeamFieldRow>>((System.Func<IDataReader, IList<TeamFieldRow>>) (reader => (IList<TeamFieldRow>) new TeamFieldConfigurationRowBinder().BindAll(reader).ToList<TeamFieldRow>())));
      return (IDictionary<string, IDictionary<Guid, bool>>) result;
    }

    protected virtual SqlCommand PrepareDynamicProcedure(string procedureName, string sqlStatement)
    {
      SqlCommand sqlCommand = this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      return sqlCommand;
    }

    protected override object ExecuteUnknown(SqlDataReader reader, object parameter) => parameter is System.Func<IDataReader, object> func ? func((IDataReader) reader) : (object) null;

    protected virtual TResult ExecuteUnknown<TResult>(System.Func<IDataReader, TResult> binder) => (TResult) this.ExecuteUnknown((object) (System.Func<IDataReader, object>) (reader => (object) binder(reader)));

    protected static void FillTeamConfigurationTeamFields(
      IDictionary<string, IDictionary<Guid, bool>> result,
      IList<TeamFieldRow> rows)
    {
      foreach (TeamFieldRow row in (IEnumerable<TeamFieldRow>) rows)
      {
        IDictionary<Guid, bool> dictionary;
        if (!result.TryGetValue(row.TeamFieldValue, out dictionary))
        {
          dictionary = (IDictionary<Guid, bool>) new Dictionary<Guid, bool>();
          result.Add(row.TeamFieldValue, dictionary);
        }
        dictionary.Add(row.TeamId, row.IncludeChildren);
      }
    }

    protected static void FillTeamConfigurationTeamFields(
      TeamConfiguration settings,
      IEnumerable<TeamFieldRow> rows)
    {
      settings.TeamFieldConfig.TeamFieldValues = (ITeamFieldValue[]) rows.OrderBy<TeamFieldRow, int>((System.Func<TeamFieldRow, int>) (row => row.Order)).Select<TeamFieldRow, TeamFieldValue>((System.Func<TeamFieldRow, TeamFieldValue>) (row => new TeamFieldValue()
      {
        Value = row.TeamFieldValue,
        IncludeChildren = row.IncludeChildren
      })).ToArray<TeamFieldValue>();
    }

    protected static void FillTeamConfigurationIterations(
      TeamConfiguration settings,
      IEnumerable<TeamIterationRow> rows)
    {
      TeamIterationsCollection iterations = (TeamIterationsCollection) settings.Iterations;
      foreach (TeamIterationRow row in rows)
        iterations.AddIteration((ITeamIteration) new TeamIteration(row.IterationId));
    }

    protected static void FillTeamConfigurationProperties(
      TeamConfiguration settings,
      IEnumerable<TeamConfigurationPropertyRow> rows)
    {
      bool flag = false;
      foreach (TeamConfigurationPropertyRow row in rows)
      {
        TeamPropertiesEnum result1;
        if (Enum.TryParse<TeamPropertiesEnum>(row.Key, out result1))
        {
          switch (result1)
          {
            case TeamPropertiesEnum.TeamFieldValueDefaultIndex:
              settings.TeamFieldConfig.DefaultValueIndex = int.Parse(row.Value);
              continue;
            case TeamPropertiesEnum.BacklogIterationId:
              settings.BacklogIterationId = new Guid(row.Value);
              continue;
            case TeamPropertiesEnum.BugCategoryVisibility:
              if (!flag)
              {
                settings.BugsBehavior = bool.Parse(row.Value) ? BugsBehavior.AsRequirements : BugsBehavior.Off;
                continue;
              }
              continue;
            case TeamPropertiesEnum.BugsBehavior:
              BugsBehavior result2;
              if (!Enum.TryParse<BugsBehavior>(row.Value, out result2))
                result2 = BugsBehavior.Off;
              settings.BugsBehavior = result2;
              flag = true;
              continue;
            case TeamPropertiesEnum.BacklogVisibilities:
              settings.BacklogVisibilities = (IDictionary<string, bool>) new Dictionary<string, bool>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
              TeamConfigurationComponent.DeserializeAndFillTeamSettingsProperty<bool>(settings.BacklogVisibilities, row);
              continue;
            case TeamPropertiesEnum.CumulativeFlowDiagramSettings:
              settings.CumulativeFlowDiagramSettings = (IDictionary<string, CumulativeFlowDiagramSettings>) new Dictionary<string, CumulativeFlowDiagramSettings>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
              TeamConfigurationComponent.DeserializeAndFillTeamSettingsProperty<CumulativeFlowDiagramSettings>(settings.CumulativeFlowDiagramSettings, row);
              continue;
            case TeamPropertiesEnum.DefaultIteration:
              Guid result3 = Guid.Empty;
              if (Guid.TryParse(row.Value, out result3))
              {
                settings.DefaultIterationId = result3;
                continue;
              }
              settings.DefaultIterationMacro = row.Value;
              continue;
            default:
              continue;
          }
        }
      }
    }

    protected static void FillTeamIdIterationIdAndWatermarkPair(
      IEnumerable<TeamIdIterationIdWatermarkPairTypeRow> rows,
      out IEnumerable<Tuple<Guid, Guid, int>> changedCapacityTeamIdIterationIdWithWatermark)
    {
      changedCapacityTeamIdIterationIdWithWatermark = rows.Select<TeamIdIterationIdWatermarkPairTypeRow, Tuple<Guid, Guid, int>>((System.Func<TeamIdIterationIdWatermarkPairTypeRow, Tuple<Guid, Guid, int>>) (row => new Tuple<Guid, Guid, int>(row.TeamId, row.IterationId, row.Watermark)));
    }

    protected static void FillTeamIdWatermarkPair(
      IEnumerable<TeamIdWatermarkPairTypeRow> rows,
      out IEnumerable<Tuple<Guid, int>> changedTeamIdWithWatermark)
    {
      changedTeamIdWithWatermark = rows.Select<TeamIdWatermarkPairTypeRow, Tuple<Guid, int>>((System.Func<TeamIdWatermarkPairTypeRow, Tuple<Guid, int>>) (row => new Tuple<Guid, int>(row.TeamId, row.Watermark)));
    }

    protected static void DeserializeAndFillTeamSettingsProperty<T>(
      IDictionary<string, T> settingsProperty,
      TeamConfigurationPropertyRow row)
    {
      try
      {
        foreach (KeyValuePair<string, T> keyValuePair in (IEnumerable<KeyValuePair<string, T>>) new DataContractSerializer(typeof (Dictionary<string, T>)).ReadObject((Stream) new MemoryStream(Encoding.UTF8.GetBytes(row.Value ?? ""))))
          settingsProperty.Add(keyValuePair);
      }
      catch (Exception ex)
      {
      }
    }

    internal static void FillTeamCapacity(TeamCapacity capacity, IEnumerable<TeamCapacityRow> rows)
    {
      List<TeamMemberCapacity> teamMemberCapacityList = new List<TeamMemberCapacity>();
      foreach (IGrouping<Guid, TeamCapacityRow> source in rows.GroupBy<TeamCapacityRow, Guid>((System.Func<TeamCapacityRow, Guid>) (k => k.TeamMemberId)))
        teamMemberCapacityList.Add(new TeamMemberCapacity()
        {
          TeamMemberId = source.Key,
          Activities = (IList<Activity>) source.Select<TeamCapacityRow, Activity>((System.Func<TeamCapacityRow, Activity>) (a => new Activity()
          {
            CapacityPerDay = a.Capacity,
            Name = a.Activity == null ? string.Empty : a.Activity
          })).ToList<Activity>()
        });
      capacity.TeamMemberCapacityCollection = (IReadOnlyCollection<TeamMemberCapacity>) teamMemberCapacityList;
    }

    internal static void FillTeamCapacityDaysOff(
      TeamCapacity capacity,
      IEnumerable<TeamCapacityDaysOffRangeRow> rows)
    {
      Dictionary<Guid, TeamMemberCapacity> dictionary = capacity.TeamMemberCapacityCollection.ToDictionary<TeamMemberCapacity, Guid>((System.Func<TeamMemberCapacity, Guid>) (teamMember => teamMember.TeamMemberId));
      foreach (IGrouping<Guid, TeamCapacityDaysOffRangeRow> source in rows.GroupBy<TeamCapacityDaysOffRangeRow, Guid>((System.Func<TeamCapacityDaysOffRangeRow, Guid>) (row => row.TeamMemberId)))
      {
        if (source.Key == TeamConfigurationComponent.WholeTeamId)
        {
          capacity.TeamDaysOffDates = source.Select<TeamCapacityDaysOffRangeRow, DateRange>((System.Func<TeamCapacityDaysOffRangeRow, DateRange>) (item => new DateRange()
          {
            Start = item.StartTime,
            End = item.EndTime
          })).ToList<DateRange>();
        }
        else
        {
          TeamMemberCapacity teamMemberCapacity;
          if (!dictionary.TryGetValue(source.Key, out teamMemberCapacity))
          {
            teamMemberCapacity = new TeamMemberCapacity();
            teamMemberCapacity.TeamMemberId = source.Key;
            capacity.TeamMemberCapacityCollection = (IReadOnlyCollection<TeamMemberCapacity>) new List<TeamMemberCapacity>((IEnumerable<TeamMemberCapacity>) capacity.TeamMemberCapacityCollection)
            {
              teamMemberCapacity
            };
          }
          teamMemberCapacity.DaysOffDates = (IList<DateRange>) source.Select<TeamCapacityDaysOffRangeRow, DateRange>((System.Func<TeamCapacityDaysOffRangeRow, DateRange>) (item => new DateRange()
          {
            Start = item.StartTime,
            End = item.EndTime
          })).ToList<DateRange>();
        }
      }
    }

    private static IEnumerable<TeamFieldRow> GetTeamFieldRows(ITeamFieldValue[] fieldValues) => fieldValues == null || fieldValues.Length == 0 ? Enumerable.Empty<TeamFieldRow>() : ((IEnumerable<ITeamFieldValue>) fieldValues).Select<ITeamFieldValue, TeamFieldRow>((Func<ITeamFieldValue, int, TeamFieldRow>) ((item, index) => new TeamFieldRow()
    {
      TeamFieldValue = item.Value,
      IncludeChildren = item.IncludeChildren,
      Order = index
    }));

    private static IEnumerable<KeyValuePair<string, string>> GetTeamFieldProperties(
      int defaultValueIndex)
    {
      yield return new KeyValuePair<string, string>("TeamFieldValueDefaultIndex", defaultValueIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static IEnumerable<KeyValuePair<string, string>> GetIterationProperties(
      Guid rootIterationId)
    {
      yield return new KeyValuePair<string, string>("BacklogIterationId", rootIterationId.ToString());
    }

    private IEnumerable<TeamCapacityRow> ConvertToTeamCapacityRows(TeamCapacity capacity)
    {
      if (capacity == null || capacity.TeamMemberCapacityCollection == null || capacity.TeamMemberCapacityCollection.Count == 0)
        return Enumerable.Empty<TeamCapacityRow>();
      List<TeamCapacityRow> teamCapacityRows = new List<TeamCapacityRow>();
      foreach (TeamMemberCapacity teamMemberCapacity in (IEnumerable<TeamMemberCapacity>) capacity.TeamMemberCapacityCollection)
      {
        TeamMemberCapacity memberCapacity = teamMemberCapacity;
        if (memberCapacity.Activities == null)
          memberCapacity.Activities = (IList<Activity>) new List<Activity>();
        if (memberCapacity.Activities.Count == 0 && !memberCapacity.IsEmpty)
          memberCapacity.Activities.Add(new Activity()
          {
            Name = string.Empty,
            CapacityPerDay = 0.0f
          });
        teamCapacityRows.AddRange(memberCapacity.Activities.Select<Activity, TeamCapacityRow>((System.Func<Activity, TeamCapacityRow>) (a => new TeamCapacityRow()
        {
          TeamMemberId = memberCapacity.TeamMemberId,
          Activity = string.IsNullOrEmpty(a.Name) ? (string) null : a.Name,
          Capacity = a.CapacityPerDay
        })));
      }
      return (IEnumerable<TeamCapacityRow>) teamCapacityRows;
    }

    private static IEnumerable<TeamCapacityDaysOffRangeRow> GetTeamCapacityDaysOffRangeRows(
      TeamCapacity capacity)
    {
      if (capacity == null)
        return Enumerable.Empty<TeamCapacityDaysOffRangeRow>();
      IEnumerable<TeamCapacityDaysOffRangeRow> first = (IEnumerable<TeamCapacityDaysOffRangeRow>) TeamConfigurationComponent.GetDateRangeRows(TeamConfigurationComponent.WholeTeamId, (IEnumerable<DateRange>) capacity.TeamDaysOffDates);
      foreach (TeamMemberCapacity teamMemberCapacity in (IEnumerable<TeamMemberCapacity>) capacity.TeamMemberCapacityCollection)
        first = first.Union<TeamCapacityDaysOffRangeRow>((IEnumerable<TeamCapacityDaysOffRangeRow>) TeamConfigurationComponent.GetDateRangeRows(teamMemberCapacity.TeamMemberId, (IEnumerable<DateRange>) teamMemberCapacity.DaysOffDates));
      return first;
    }

    protected static IReadOnlyCollection<TeamCapacityDaysOffRangeRow> GetDateRangeRows(
      Guid teamMemberId,
      IEnumerable<DateRange> ranges)
    {
      return ranges == null || ranges.Count<DateRange>() == 0 ? (IReadOnlyCollection<TeamCapacityDaysOffRangeRow>) new List<TeamCapacityDaysOffRangeRow>() : (IReadOnlyCollection<TeamCapacityDaysOffRangeRow>) ranges.Select<DateRange, TeamCapacityDaysOffRangeRow>((System.Func<DateRange, TeamCapacityDaysOffRangeRow>) (item => new TeamCapacityDaysOffRangeRow()
      {
        TeamMemberId = teamMemberId,
        StartTime = item.Start,
        EndTime = item.End
      })).ToList<TeamCapacityDaysOffRangeRow>();
    }
  }
}
