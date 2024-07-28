// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationComponent8
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationComponent8 : CheckConfigurationComponent7
  {
    private static readonly SqlMetaData[] PolicyAssignmentGetParametersTableType = new SqlMetaData[2]
    {
      new SqlMetaData("AssignmentId", SqlDbType.Int),
      new SqlMetaData("AssignmentVersion", SqlDbType.Int)
    };

    public override CheckConfiguration AddNewCheckConfigurationVersion(
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      ArgumentUtility.CheckForEmptyGuid(checkConfiguration.Type.Id, "checkTypeId", "Pipeline.Checks");
      ArgumentUtility.CheckForNonPositiveInt(checkConfiguration.Version, "Version");
      object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
      string parameterValue = configurationSettings != null ? JsonUtility.ToString(configurationSettings) : string.Empty;
      string executionOptions = CheckExecutionOptions.GetSerializedExecutionOptions(checkConfiguration);
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddNewCheckConfigurationVersion)))
      {
        this.PrepareForAuditingAction("CheckConfiguration.Created", this.GetAuditLogData(checkConfiguration), projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@config"
        });
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddPolicyAssignmentVersion");
        this.BindInt("@assignmentId", checkConfiguration.Id);
        this.BindGuid("@typeId", checkConfiguration.Type.Id);
        this.BindString("@config", parameterValue, parameterValue.Length, false, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(checkConfiguration.ModifiedBy.Id));
        this.BindString("@executionOptions", executionOptions, -1, true, SqlDbType.NVarChar);
        this.BindBoolean("@isDisabled", checkConfiguration.IsDisabled);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().FirstOrDefault<CheckConfiguration>();
        }
      }
    }

    public override List<CheckConfiguration> GetCheckConfigurationsByIdVersion(
      IEnumerable<CheckConfigurationRef> checkParams,
      bool includeSettings = false,
      bool includeDeletedChecks = false)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckConfigurationsByIdVersion)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyAssignmentByIdVersion");
        this.BindBoolean("@includeSettings", includeSettings);
        this.BindBoolean("@includeDeletedChecks", includeDeletedChecks);
        this.BindTable("@assignmentParams", "PipelinePolicy.typ_PolicyAssignmentGetTable", checkParams.Select<CheckConfigurationRef, SqlDataRecord>(new System.Func<CheckConfigurationRef, SqlDataRecord>(this.ConvertToPolicyAssignmentGetTableSqlDataRecord)));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().Items;
        }
      }
    }

    public override int DeleteUnusedSoftDeletedCheckConfigurations(
      int batchSize,
      int deleteOffsetInHours)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (DeleteUnusedSoftDeletedCheckConfigurations)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_DeletePolicyAssignments", 3600);
        this.BindInt("@batchSize", batchSize);
        this.BindInt("@deleteOffsetInHours", deleteOffsetInHours);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder("DeletedCount"));
          return resultCollection.GetCurrent<int>().Items[0];
        }
      }
    }

    public override int DeleteHistoricalCheckConfigurationVersions(
      int batchSize,
      int deleteOffsetInHours)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (DeleteHistoricalCheckConfigurationVersions)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_DeletePolicyAssignmentsHistory", 3600);
        this.BindInt("@batchSize", batchSize);
        this.BindInt("@deleteOffsetInHours", deleteOffsetInHours);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder("DeletedCount"));
          return resultCollection.GetCurrent<int>().Items[0];
        }
      }
    }

    protected virtual SqlDataRecord ConvertToPolicyAssignmentGetTableSqlDataRecord(
      CheckConfigurationRef row)
    {
      SqlDataRecord tableSqlDataRecord = new SqlDataRecord(CheckConfigurationComponent8.PolicyAssignmentGetParametersTableType);
      tableSqlDataRecord.SetInt32(0, row.Id);
      tableSqlDataRecord.SetInt32(1, row.Version);
      return tableSqlDataRecord;
    }
  }
}
