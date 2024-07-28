// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseEnvironmentBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseEnvironmentBinder : ReleaseManagementObjectBinderBase<ReleaseEnvironment>
  {
    private SqlColumnBinder queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder runoptions = new SqlColumnBinder("RunOptions");
    private SqlColumnBinder approvalOptions = new SqlColumnBinder("ApprovalOptions");
    private SqlColumnBinder definitionDemands = new SqlColumnBinder("DefinitionDemands");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder workflow = new SqlColumnBinder("Workflow");
    private SqlColumnBinder variables = new SqlColumnBinder("Variables");
    private SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder scheduledDeploymentTime = new SqlColumnBinder("ScheduledDeploymentTime");
    private SqlColumnBinder scheduledOperationId = new SqlColumnBinder("ScheduledOperationId");
    private SqlColumnBinder conditions = new SqlColumnBinder("Conditions");
    private SqlColumnBinder schedules = new SqlColumnBinder("Schedules");

    public ReleaseEnvironmentBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseEnvironment Bind()
    {
      DateTime dateTime = this.scheduledDeploymentTime.GetDateTime((IDataReader) this.Reader, SqlDateTime.MinValue.Value);
      ReleaseEnvironment releaseEnvironment = new ReleaseEnvironment()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        Name = this.name.GetString((IDataReader) this.Reader, false),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        OwnerId = this.ownerId.GetGuid((IDataReader) this.Reader, false, Guid.Empty),
        ScheduledOperationId = this.scheduledOperationId.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        Status = (ReleaseEnvironmentStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0)
      };
      releaseEnvironment.ScheduledDeploymentTime = dateTime == DateTime.MinValue ? new DateTime?() : new DateTime?(dateTime);
      VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(this.variables.GetString((IDataReader) this.Reader, false)), releaseEnvironment.Variables);
      string runOptions = this.runoptions.GetString((IDataReader) this.Reader, (string) null);
      releaseEnvironment.EnvironmentOptions = ServerModelUtility.GetServerEnvironmentOptions(runOptions);
      IDictionary<string, ApprovalOptions> approvalOptions = ServerModelUtility.FromString<IDictionary<string, ApprovalOptions>>(this.approvalOptions.GetString((IDataReader) this.Reader, (string) null));
      releaseEnvironment.PopulateReleaseEnvironmentPreAndPostApprovalOptions(approvalOptions);
      string str1 = this.conditions.GetString((IDataReader) this.Reader, (string) null);
      releaseEnvironment.Conditions = str1 == null ? (IList<ReleaseCondition>) null : JsonConvert.DeserializeObject<IList<ReleaseCondition>>(str1);
      string str2 = this.schedules.GetString((IDataReader) this.Reader, (string) null);
      if (!string.IsNullOrWhiteSpace(str2))
        releaseEnvironment.Schedules = JsonConvert.DeserializeObject<IList<ReleaseSchedule>>(str2);
      string workflow = this.workflow.GetString((IDataReader) this.Reader, false);
      int int32 = this.queueId.GetInt32((IDataReader) this.Reader, 0, 0);
      string demands = this.definitionDemands.GetString((IDataReader) this.Reader, true);
      DeployPhaseSnapshot releaseDeployPhase = EnvironmentCompatExtensions.GetCompatReleaseDeployPhase(ServerModelUtility.GetWebApiEnvironmentOptions(runOptions), workflow, demands, int32);
      releaseEnvironment.AddDesignerDeployPhaseSnapshot(releaseDeployPhase);
      return releaseEnvironment;
    }
  }
}
