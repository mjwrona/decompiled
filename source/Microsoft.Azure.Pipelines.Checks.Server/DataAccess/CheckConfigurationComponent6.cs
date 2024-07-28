// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationComponent6
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationComponent6 : CheckConfigurationComponent5
  {
    private static readonly SqlMetaData[] CheckConfigurationCreateParametersTableType = new SqlMetaData[7]
    {
      new SqlMetaData("TypeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Scope", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Config", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ExecuteOptions", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AssignmentId", SqlDbType.Int),
      new SqlMetaData("ConfigurationId", SqlDbType.Int)
    };

    public override IList<CheckConfiguration> AddCheckConfigurations(
      IList<CheckConfiguration> checkConfigurations)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckConfigurations)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddPolicyBatchAssignment");
        this.BindTable("@createParameters", "PipelinePolicy.typ_PolicyAssignmentCreateParametersTable", checkConfigurations.Select<CheckConfiguration, SqlDataRecord>(new System.Func<CheckConfiguration, SqlDataRecord>(this.ConvertToCheckConfigurationCreateTableSqlDataRecord)));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return (IList<CheckConfiguration>) resultCollection.GetCurrent<CheckConfiguration>().Items;
        }
      }
    }

    protected virtual SqlDataRecord ConvertToCheckConfigurationCreateTableSqlDataRecord(
      CheckConfiguration checkConfiguration)
    {
      object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
      string str = configurationSettings != null ? JsonUtility.ToString(configurationSettings) : string.Empty;
      string scopeString = checkConfiguration.Resource.GetScopeString();
      int num1 = 0;
      SqlDataRecord record = new SqlDataRecord(CheckConfigurationComponent6.CheckConfigurationCreateParametersTableType);
      int ordinal1 = num1;
      int num2 = ordinal1 + 1;
      record.SetGuid(ordinal1, checkConfiguration.Type.Id);
      int ordinal2 = num2;
      int num3 = ordinal2 + 1;
      record.SetString(ordinal2, scopeString);
      int ordinal3 = num3;
      int num4 = ordinal3 + 1;
      record.SetString(ordinal3, str);
      int ordinal4 = num4;
      int num5 = ordinal4 + 1;
      record.SetGuid(ordinal4, new Guid(checkConfiguration.CreatedBy.Id));
      int ordinal5 = num5;
      int num6 = ordinal5 + 1;
      record.SetNullableString(ordinal5, (string) null);
      int ordinal6 = num6;
      int num7 = ordinal6 + 1;
      record.SetNullableInt32(ordinal6, new int?());
      int ordinal7 = num7;
      int num8 = ordinal7 + 1;
      record.SetNullableInt32(ordinal7, new int?());
      return record;
    }
  }
}
