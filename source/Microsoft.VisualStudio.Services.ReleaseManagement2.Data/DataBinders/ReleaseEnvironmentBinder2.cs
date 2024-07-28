// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseEnvironmentBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseEnvironmentBinder2 : ReleaseEnvironmentBinder
  {
    private SqlColumnBinder runoptions = new SqlColumnBinder("RunOptions");
    private SqlColumnBinder approvalOptions = new SqlColumnBinder("ApprovalOptions");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder variables = new SqlColumnBinder("Variables");
    private SqlColumnBinder variableGroups = new SqlColumnBinder("VariableGroups");
    private SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder scheduledDeploymentTime = new SqlColumnBinder("ScheduledDeploymentTime");
    private SqlColumnBinder scheduledOperationId = new SqlColumnBinder("ScheduledOperationId");
    private SqlColumnBinder conditions = new SqlColumnBinder("Conditions");
    private SqlColumnBinder schedules = new SqlColumnBinder("Schedules");
    private SqlColumnBinder deployPhaseSnapshots = new SqlColumnBinder("DeployPhaseSnapshots");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");

    public ReleaseEnvironmentBinder2(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseEnvironment Bind()
    {
      DateTime dateTime = this.scheduledDeploymentTime.GetDateTime((IDataReader) this.Reader, SqlDateTime.MinValue.Value);
      string str1 = this.approvalOptions.GetString((IDataReader) this.Reader, (string) null);
      string json1 = this.conditions.GetString((IDataReader) this.Reader, (string) null);
      string json2 = this.schedules.GetString((IDataReader) this.Reader, (string) null);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      ReleaseEnvironment releaseEnvironment = new ReleaseEnvironment()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        Name = this.name.GetString((IDataReader) this.Reader, false),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        OwnerId = this.ownerId.GetGuid((IDataReader) this.Reader, false, Guid.Empty),
        ScheduledOperationId = this.scheduledOperationId.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        Status = (ReleaseEnvironmentStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0),
        EnvironmentOptions = ServerModelUtility.GetServerEnvironmentOptions(this.runoptions.GetString((IDataReader) this.Reader, (string) null))
      };
      releaseEnvironment.ScheduledDeploymentTime = dateTime == DateTime.MinValue ? new DateTime?() : new DateTime?(dateTime);
      VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(this.variables.GetString((IDataReader) this.Reader, false)), releaseEnvironment.Variables);
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> values = ServerModelUtility.FromString<IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>>(this.variableGroups.GetString((IDataReader) this.Reader, (string) null));
      if (values != null)
        releaseEnvironment.VariableGroups.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) values);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions> approvalOptions = ServerModelUtility.FromString<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions>>(str1);
      releaseEnvironment.PopulateReleaseEnvironmentPreAndPostApprovalOptions(approvalOptions);
      releaseEnvironment.Conditions = ReleaseEnvironmentBinder2.ParseJsonSafely<IList<ReleaseCondition>>(json1);
      if (!string.IsNullOrWhiteSpace(json2))
        releaseEnvironment.Schedules = ReleaseEnvironmentBinder2.ParseJsonSafely<IList<ReleaseSchedule>>(json2);
      string json3 = this.deployPhaseSnapshots.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(json3))
        releaseEnvironment.DeploymentSnapshot = ReleaseEnvironmentBinder2.ParseJsonSafely<IDeploymentSnapshot>(json3);
      ReleaseEnvironmentBinder2.NormalizeDeployPhaseSnapshots(releaseEnvironment.GetDesignerDeployPhaseSnapshots());
      string str2 = this.processParameters.GetString((IDataReader) this.Reader, (string) null);
      if (str2 != null)
        releaseEnvironment.ProcessParameters = ServerModelUtility.FromString<ProcessParameters>(str2);
      return releaseEnvironment;
    }

    private static void NormalizeDeployPhaseSnapshots(IList<DeployPhaseSnapshot> snapshots)
    {
      foreach (DeployPhaseSnapshot snapshot in (IEnumerable<DeployPhaseSnapshot>) snapshots)
      {
        if (snapshot.PhaseType == DeployPhaseTypes.Undefined)
        {
          snapshot.PhaseType = DeployPhaseTypes.AgentBasedDeployment;
          snapshot.Name = snapshot.Name ?? Resources.AgentBasedDeploymentDefaultName;
          snapshot.Rank = snapshot.Rank == 0 ? 1 : snapshot.Rank;
        }
        if (snapshot.PhaseType == DeployPhaseTypes.RunOnServer && snapshot.DeploymentInput == null)
          snapshot.DeploymentInput = JObject.FromObject((object) new ServerDeploymentInput()
          {
            ParallelExecution = (ExecutionInput) new NoneExecutionInput()
          });
        if (string.IsNullOrWhiteSpace(snapshot.Name))
          snapshot.Name = ServerModelUtility.GetDefaultPhaseName(snapshot.PhaseType);
      }
    }

    private static T ParseJsonSafely<T>(string json) where T : class
    {
      if (string.IsNullOrWhiteSpace(json))
        return default (T);
      try
      {
        return JsonConvert.DeserializeObject<T>(json);
      }
      catch (JsonReaderException ex) when (ex.Message.Contains("Bad JSON escape sequence"))
      {
        json = json.Replace("\\", "\\\\");
        return JsonConvert.DeserializeObject<T>(json);
      }
      catch (JsonSerializationException ex)
      {
        JToken jtoken = JsonUtilities.DeserializeTruncatedJson(json);
        return jtoken != null ? jtoken.ToObject<T>() : default (T);
      }
    }
  }
}
