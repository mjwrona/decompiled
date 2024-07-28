// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DefinitionEnvironmentBinder : ReleaseManagementObjectBinderBase<DefinitionEnvironment>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder approvalOptions = new SqlColumnBinder("ApprovalOptions");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder variables = new SqlColumnBinder("Variables");
    private SqlColumnBinder conditions = new SqlColumnBinder("Conditions");
    private SqlColumnBinder executionPolicies = new SqlColumnBinder("ExecutionPolicies");
    private SqlColumnBinder schedules = new SqlColumnBinder("Schedules");
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder currentReleaseId = new SqlColumnBinder(nameof (currentReleaseId));
    private SqlColumnBinder retentionPolicy = new SqlColumnBinder("RetentionPolicy");
    private SqlColumnBinder retainBuild = new SqlColumnBinder("RetainBuild");

    public DefinitionEnvironmentBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DefinitionEnvironment Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      DefinitionEnvironment definitionEnvironment = new DefinitionEnvironment()
      {
        ProjectId = guid,
        Id = this.id.GetInt32((IDataReader) this.Reader),
        Name = this.name.GetString((IDataReader) this.Reader, false),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        OwnerId = this.ownerId.GetGuid((IDataReader) this.Reader),
        ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
        CurrentReleaseId = this.currentReleaseId.GetInt32((IDataReader) this.Reader, 0, 0)
      };
      VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(this.variables.GetString((IDataReader) this.Reader, false)), definitionEnvironment.Variables);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> approvalOptions = ServerModelUtility.FromString<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>>(this.approvalOptions.GetString((IDataReader) this.Reader, (string) null));
      definitionEnvironment.PopulateDefinitionPreAndPostApprovalOptions(approvalOptions);
      string str1 = this.conditions.GetString((IDataReader) this.Reader, (string) null);
      definitionEnvironment.Conditions = str1 == null ? (IList<Condition>) null : JsonConvert.DeserializeObject<IList<Condition>>(str1);
      string str2 = this.executionPolicies.GetString((IDataReader) this.Reader, (string) null);
      if (!string.IsNullOrWhiteSpace(str2))
        definitionEnvironment.ExecutionPolicy = JsonConvert.DeserializeObject<EnvironmentExecutionPolicy>(str2);
      string str3 = this.schedules.GetString((IDataReader) this.Reader, (string) null);
      if (!string.IsNullOrWhiteSpace(str3))
        definitionEnvironment.Schedules = JsonConvert.DeserializeObject<IList<ReleaseSchedule>>(str3);
      string str4 = this.retentionPolicy.GetString((IDataReader) this.Reader, (string) null);
      definitionEnvironment.RetentionPolicy = string.IsNullOrWhiteSpace(str4) ? (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy) null : JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentRetentionPolicy>(str4);
      if (definitionEnvironment.RetentionPolicy != null)
        definitionEnvironment.RetentionPolicy.RetainBuild = this.retainBuild.ColumnExists((IDataReader) this.Reader) && this.retainBuild.GetBoolean((IDataReader) this.Reader, true);
      return definitionEnvironment;
    }
  }
}
