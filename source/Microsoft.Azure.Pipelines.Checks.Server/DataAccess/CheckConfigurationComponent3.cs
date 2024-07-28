// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationComponent3
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationComponent3 : CheckConfigurationComponent2
  {
    public override CheckConfiguration AddCheckConfiguration(
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      ArgumentUtility.CheckForEmptyGuid(checkConfiguration.Type.Id, "checkTypeId", "Pipeline.Checks");
      object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
      string parameterValue = configurationSettings != null ? JsonUtility.ToString(configurationSettings) : string.Empty;
      string scopeString = checkConfiguration.Resource.GetScopeString();
      string executionOptions = CheckExecutionOptions.GetSerializedExecutionOptions(checkConfiguration);
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckConfiguration)))
      {
        this.PrepareForAuditingAction("CheckConfiguration.Created", this.GetAuditLogData(checkConfiguration), projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@config"
        });
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddPolicyAssignment");
        this.BindGuid("@typeId", checkConfiguration.Type.Id);
        this.BindString("@scope", scopeString, 256, false, SqlDbType.NVarChar);
        this.BindString("@config", parameterValue, parameterValue.Length, false, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(checkConfiguration.CreatedBy.Id));
        this.BindString("@executionOptions", executionOptions, -1, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().FirstOrDefault<CheckConfiguration>();
        }
      }
    }

    protected override CheckConfigurationBinder GetCheckConfigurationBinder() => (CheckConfigurationBinder) new CheckConfigurationBinder2();
  }
}
