// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationComponent7
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationComponent7 : CheckConfigurationComponent6
  {
    public const string m_deletedCountColumn = "DeletedCount";

    public override List<CheckConfiguration> GetCheckConfigurationsOnResources(
      List<Resource> resources,
      bool includeDisabledChecks,
      bool includeSettings = false,
      bool includeDeletedChecks = false)
    {
      IEnumerable<string> rows = resources.Select<Resource, string>((System.Func<Resource, string>) (resource => resource.GetScopeString()));
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckConfigurationsOnResources)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyAssignmentsByScopes");
        this.BindStringTable("@scopesTable", rows, true, 256);
        this.BindBoolean("@includeSettings", includeSettings);
        this.BindBoolean("@includeDeletedChecks", includeDeletedChecks);
        this.BindBoolean("@includeDisabledChecks", includeDisabledChecks);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().Items;
        }
      }
    }

    public override CheckConfiguration UpdateCheckConfiguration(
      int assignmentId,
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
      string parameterValue = configurationSettings != null ? JsonUtility.ToString(configurationSettings) : string.Empty;
      string executionOptions = CheckExecutionOptions.GetSerializedExecutionOptions(checkConfiguration);
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (UpdateCheckConfiguration)))
      {
        this.PrepareForAuditingAction("CheckConfiguration.Updated", this.GetAuditLogData(checkConfiguration), projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@config"
        });
        this.PrepareStoredProcedure("PipelinePolicy.prc_UpdatePolicyAssignment");
        this.BindInt("@assignmentId", assignmentId);
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

    protected override CheckConfigurationBinder GetCheckConfigurationBinder() => (CheckConfigurationBinder) new CheckConfigurationBinder3();

    public override int DeleteUnusedSoftDeletedCheckConfigurations(
      int batchSize,
      int deleteOffsetInHours)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (DeleteUnusedSoftDeletedCheckConfigurations)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_DeletePolicyAssignments", 3600);
        this.BindInt("@batchSize", batchSize);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder("DeletedCount"));
          return resultCollection.GetCurrent<int>().Items[0];
        }
      }
    }
  }
}
