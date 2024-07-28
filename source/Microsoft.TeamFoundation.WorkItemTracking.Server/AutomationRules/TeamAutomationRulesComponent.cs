// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules.TeamAutomationRulesComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules
{
  internal class TeamAutomationRulesComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<TeamAutomationRulesComponent>(0)
    }, "TeamAutomationRules", "WorkItem");

    public virtual IEnumerable<TeamAutomationRulesSettings> GetTeamAutomationRulesSettings(
      Guid projectId,
      Guid? teamId = null,
      int? areaId = null)
    {
      this.PrepareStoredProcedure("prc_GetTeamAutomationRulesSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindNullableGuid("@teamId", teamId);
      this.BindNullableInt("@areaId", areaId);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow>((ObjectBinder<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow>) new TeamAutomationRulesComponent.TeamAutomationRulesSettingsRowBinder());
      List<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow> items = resultCollection.GetCurrent<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow>().Items;
      List<TeamAutomationRulesSettings> workItemAutomationRules = new List<TeamAutomationRulesSettings>();
      Action<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow> action = (Action<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow>) (item =>
      {
        IDictionary<string, bool> rulesStates = TeamAutomationRulesComponent.DeserializeRulesStatesProperty<bool>(item);
        workItemAutomationRules.Add(new TeamAutomationRulesSettings(item.TeamId, item.BacklogLevelId, rulesStates));
      });
      items.ForEach(action);
      return (IEnumerable<TeamAutomationRulesSettings>) workItemAutomationRules;
    }

    public virtual void UpdateWorkItemAutomationRulesSettings(
      Guid projectId,
      TeamAutomationRulesSettings teamAutomationRuleSettings)
    {
      this.PrepareStoredProcedure("prc_SaveTeamAutomationRulesSettings");
      string settingsAsString = TeamAutomationRulesComponent.GetRulesSettingsAsString(teamAutomationRuleSettings.RulesStates);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindGuid("@teamId", teamAutomationRuleSettings.TeamId);
      this.BindString("@backlogLevelId", teamAutomationRuleSettings.BacklogLevelId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@rulesStates", settingsAsString, settingsAsString.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQueryEx();
    }

    protected static IDictionary<string, T> DeserializeRulesStatesProperty<T>(
      TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow row)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(row.RulesStates ?? "")))
        return (IDictionary<string, T>) new DataContractSerializer(typeof (Dictionary<string, T>)).ReadObject((Stream) memoryStream);
    }

    protected static string GetRulesSettingsAsString(IDictionary<string, bool> rulesSettings)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractSerializer(typeof (Dictionary<string, bool>)).WriteObject((Stream) memoryStream, (object) rulesSettings);
        memoryStream.Close();
        return Encoding.UTF8.GetString(memoryStream.ToArray());
      }
    }

    internal class TeamAutomationRulesSettingsRow
    {
      internal Guid TeamId { get; set; }

      internal string BacklogLevelId { get; set; }

      internal string RulesStates { get; set; }
    }

    internal class TeamAutomationRulesSettingsRowBinder : 
      ObjectBinder<TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow>
    {
      private SqlColumnBinder teamId = new SqlColumnBinder("TeamId");
      private SqlColumnBinder backlogLevelId = new SqlColumnBinder("BacklogLevelId");
      private SqlColumnBinder rulesState = new SqlColumnBinder("RulesStates");

      protected override TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow Bind() => new TeamAutomationRulesComponent.TeamAutomationRulesSettingsRow()
      {
        TeamId = this.teamId.GetGuid((IDataReader) this.Reader, false),
        BacklogLevelId = this.backlogLevelId.GetString((IDataReader) this.Reader, false),
        RulesStates = this.rulesState.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
