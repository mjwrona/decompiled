// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  [ClientIncludeModel]
  public class BoardSettings
  {
    private IEnumerable<BoardRow> _rows;
    private static readonly IEnumerable<BoardRow> _defaultRows = (IEnumerable<BoardRow>) new BoardRow[1]
    {
      new BoardRow()
      {
        Id = new Guid?(Guid.Empty),
        Name = (string) null,
        IsDeleted = false,
        Order = 0
      }
    };
    private string m_validationError;

    [DataMember]
    public Guid? Id { get; set; }

    [DataMember]
    public Guid TeamId { get; set; }

    [DataMember]
    public Guid? ExtensionId { get; set; }

    [DataMember]
    public DateTime ExtensionLastChangeDate { get; set; }

    [DataMember]
    public IEnumerable<BoardColumn> Columns { get; set; }

    [DataMember]
    public IEnumerable<BoardRow> Rows
    {
      get => this._rows == null || !this._rows.Any<BoardRow>() ? BoardSettings._defaultRows : this._rows;
      set => this._rows = value;
    }

    [DataMember]
    public IDictionary<string, IDictionary<string, string[]>> AllowedMappings { get; set; }

    [DataMember]
    public bool CanEdit { get; set; }

    [DataMember]
    public string BacklogLevelId { get; set; }

    [DataMember]
    public bool PreserveBacklogOrder { get; set; }

    [DataMember]
    public bool AutoRefreshState { get; set; }

    [DataMember]
    public IDictionary<string, string> BoardFields { get; set; }

    [DataMember]
    public IDictionary<string, string> SortableFieldsByColumnType { get; set; }

    [DataMember]
    public bool IsBoardValid { get; set; }

    [DataMember]
    public bool StatusBadgeIsPublic { get; set; }

    public string GetValidationError() => this.m_validationError;

    public virtual BoardCardSettings BoardCardSettings { get; set; }

    public virtual BoardFilterSettings BoardFilterSettings { get; set; }

    public bool IsValid(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration processSettings,
      string projectName,
      WebApiTeam team,
      ITeamSettings teamSettings)
    {
      using (requestContext.TraceBlock(290070, 290071, "Agile", TfsTraceLayers.BusinessLogic, "BoardSettings.IsValid"))
      {
        Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, projectId);
        BoardValidator boardValidator = this.GetBoardValidator(requestContext, backlogConfiguration);
        int num1 = boardValidator.Validate(false) ? 1 : 0;
        this.m_validationError = boardValidator.GetUserFriendlyErrorMessage();
        if (num1 == 0)
          return false;
        BoardColumnsValidator columnsValidator = this.GetBoardColumnsValidator(requestContext, processSettings, backlogConfiguration, projectName, teamSettings);
        int num2 = columnsValidator.Validate(false) ? 1 : 0;
        this.m_validationError = columnsValidator.GetUserFriendlyErrorMessage();
        return num2 != 0;
      }
    }

    internal void PopulateStateMappings(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      WorkItemTypeExtension extension)
    {
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, "Agile", nameof (PopulateStateMappings));
      using (requestContext.TraceBlock(290072, 290073, "Agile", TfsTraceLayers.BusinessLogic, "BoardSettings.PopulateStateMappings"))
      {
        string traceDetails = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExtensionId: {0}. ProjectName: {1}. Team: Name- {2}, Identity- {3}. BacklogLevelId: {4}.", (object) this.ExtensionId.Value, (object) project.Name, (object) team.Name, (object) team.Id.ToString(), (object) backlogLevel.Name);
        using (performanceScenarioHelper.Measure("GetExtension"))
        {
          if (!(this.ExtensionLastChangeDate == DateTime.MinValue))
          {
            if (!(this.ExtensionLastChangeDate > extension.LastChangedDate))
              goto label_8;
          }
          extension = requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, this.ExtensionId.Value, true);
          this.TraceIfNull(requestContext, (object) extension, 240316, "Could not retrieve the extension. " + traceDetails);
        }
label_8:
        WorkItemFieldRule kanbanColumnFieldRule = extension.FieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r =>
        {
          this.TraceIfNull(requestContext, (object) r, 240317, "Could not retrieve the extension FieldRule. " + traceDetails);
          this.TraceIfNull(requestContext, (object) r.Field, 240318, "Could not retrieve the Field. " + traceDetails);
          return TFStringComparer.WorkItemFieldReferenceName.Equals(r.Field, "[Kanban.Column]");
        })).FirstOrDefault<WorkItemFieldRule>();
        this.TraceIfNull(requestContext, (object) kanbanColumnFieldRule, 240319, "Could not retrieve the fieldRule. " + traceDetails);
        List<WhenRule> list = kanbanColumnFieldRule.SelectRules<WhenRule>().Where<WhenRule>((Func<WhenRule, bool>) (r =>
        {
          this.TraceIfNull(requestContext, (object) r, 240320, "Could not retrieve the extension FieldRule. " + traceDetails);
          this.TraceIfNull(requestContext, (object) r.Field, 240321, "Could not retrieve the Field. " + traceDetails);
          return TFStringComparer.WorkItemFieldReferenceName.Equals(r.Field, CoreFieldReferenceNames.WorkItemType);
        })).ToList<WhenRule>();
        ITeamSettings teamSettings = (ITeamSettings) null;
        using (performanceScenarioHelper.Measure("GetTeamSettings"))
        {
          teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, false);
          this.TraceIfNull(requestContext, (object) teamSettings, 240323, "Could not retrieve the teamSettings. " + traceDetails);
        }
        int bugsBehavior = (int) teamSettings.BugsBehavior;
        this.TraceIfNull(requestContext, (object) backlogLevel, 240324, "Could not retrieve the backlogLevel. " + traceDetails);
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        {
          using (performanceScenarioHelper.Measure("SyncWhenRulesForCustomWorkItemTypes"))
            this.SyncWhenRulesForCustomWorkItemTypes(requestContext, project, team, teamSettings, backlogLevel, list, kanbanColumnFieldRule, extension);
        }
        if (teamSettings.BugsBehavior != BugsBehavior.AsRequirements && backlogLevel.IsRequirementsBacklog)
        {
          IEnumerable<string> workItemTypeNames = (IEnumerable<string>) null;
          using (performanceScenarioHelper.Measure("GetEffectiveBacklogWorkItemTypes"))
          {
            workItemTypeNames = (IEnumerable<string>) backlogLevel.WorkItemTypes;
            if (workItemTypeNames.IsNullOrEmpty<string>())
              requestContext.Trace(240325, TraceLevel.Warning, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "categoryWorkItemTypeNames is empty or null. ExtensionId: {0}. ProjectName: {1}. Team: Name- {2}, Identity- {3}. BacklogLevelConfiguration-  {4}.", (object) this.ExtensionId.Value, (object) project.Name, (object) team.Name, (object) team.Id.ToString(), (object) backlogLevel.Name);
          }
          IEnumerable<WhenRule> second = kanbanColumnFieldRule.SelectRules<WhenRule>().Where<WhenRule>((Func<WhenRule, bool>) (r => !workItemTypeNames.Contains<string>(r.Value, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
          list = list.Except<WhenRule>(second).ToList<WhenRule>();
        }
        Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
        foreach (WhenRule whenRule in list)
        {
          this.TraceIfNull(requestContext, (object) whenRule, 240331, "Could not retrieve the when rule. " + traceDetails);
          string key1 = whenRule.Value;
          MapRule mapRule = whenRule.SelectRules<MapRule>().Where<MapRule>((Func<MapRule, bool>) (r =>
          {
            this.TraceIfNull(requestContext, (object) r, 240326, "Could not retrieve the extension whenRule. " + traceDetails);
            this.TraceIfNull(requestContext, (object) r.Field, 240327, "Could not retrieve the whenRuleField. " + traceDetails);
            return TFStringComparer.WorkItemFieldReferenceName.Equals(r.Field, CoreFieldReferenceNames.State);
          })).FirstOrDefault<MapRule>();
          this.TraceIfNull(requestContext, (object) mapRule, 240328, "Could not retrieve the mapRule. " + traceDetails);
          foreach (MapCase mapCase in mapRule.Cases)
          {
            this.TraceIfNull(requestContext, (object) mapCase, 240329, "Could not retrieve the mapCase. " + traceDetails);
            foreach (string key2 in mapCase.Values)
            {
              if (!dictionary.TryGetValue(key2, out Dictionary<string, string> _))
                dictionary[key2] = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
              dictionary[key2][key1] = mapCase.Value;
            }
          }
        }
        foreach (BoardColumn column in this.Columns)
        {
          this.TraceIfNull(requestContext, (object) column, 240330, "Could not retrieve the column. " + traceDetails);
          if (dictionary.ContainsKey(column.Name))
          {
            column.StateMappings = dictionary[column.Name];
          }
          else
          {
            requestContext.Trace(240302, TraceLevel.Info, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Could not find work item type in columnWitStateMappings");
            column.StateMappings = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
            foreach (WhenRule whenRule in list)
              column.StateMappings[whenRule.Value] = string.Empty;
          }
        }
      }
      performanceScenarioHelper.EndScenario();
    }

    private void TraceIfNull(
      IVssRequestContext requestContext,
      object value,
      int tracepoint,
      string message,
      params object[] args)
    {
      if (value != null)
        return;
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Warning, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, message, args);
    }

    public virtual BoardValidator GetBoardValidator(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration)
    {
      return new BoardValidator(requestContext, this, backlogConfiguration);
    }

    public virtual BoardColumnsValidator GetBoardColumnsValidator(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration processSettings,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      string projectName,
      ITeamSettings teamSettings,
      BoardSettings existingBoardSettings = null)
    {
      return new BoardColumnsValidator(requestContext, this, processSettings, backlogConfiguration, projectName, teamSettings, existingBoardSettings);
    }

    public virtual BoardRowsValidator GetBoardRowsValidator(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration processSettings)
    {
      return new BoardRowsValidator(requestContext, this, processSettings);
    }

    internal void SyncWhenRulesForCustomWorkItemTypes(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      ITeamSettings teamSettings,
      BacklogLevelConfiguration backlogLevel,
      List<WhenRule> whenRules,
      WorkItemFieldRule kanbanColumnFieldRule,
      WorkItemTypeExtension extension)
    {
      requestContext.GetService<ITeamFoundationProcessService>();
      IWorkItemTypeService service1 = requestContext.GetService<IWorkItemTypeService>();
      Guid id = project.GetId();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = id;
      IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = service1.GetWorkItemTypes(requestContext1, projectId);
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> list = workItemTypes.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, bool>) (t => t.IsCustomType)).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>();
      List<WhenRule> whenRuleList1 = new List<WhenRule>();
      List<WhenRule> rulesToRemove = new List<WhenRule>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType in list)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType customWit = workItemType;
        List<WhenRule> customWitWhenRules = whenRules.Where<WhenRule>((Func<WhenRule, bool>) (r => TFStringComparer.WorkItemTypeName.Equals(r.Value, customWit.Name))).ToList<WhenRule>();
        IReadOnlyCollection<WorkItemStateDefinition> states = customWit.Source.GetStates(requestContext);
        if (customWitWhenRules.Any<WhenRule>())
        {
          List<WhenRule> whenRuleList2 = new List<WhenRule>();
          foreach (WhenRule whenRule1 in customWitWhenRules)
          {
            WhenRule whenRule = whenRule1;
            if (!backlogLevel.WorkItemTypes.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeName.Equals(n, whenRule.Value))))
              whenRuleList2.Add(whenRule);
          }
          whenRuleList2.ForEach((Action<WhenRule>) (r =>
          {
            customWitWhenRules.Remove(r);
            rulesToRemove.Add(r);
          }));
        }
        if (!customWitWhenRules.Any<WhenRule>() && backlogLevel.WorkItemTypes.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeName.Equals(n, customWit.Name))))
        {
          WhenRule customWorkItemType = KanbanUtils.Instance.CreateDefaultWhenRuleForCustomWorkItemType(requestContext, project, this.Columns, states, customWit.Name);
          whenRuleList1.Add(customWorkItemType);
        }
      }
      foreach (WhenRule whenRule2 in whenRules)
      {
        WhenRule whenRule = whenRule2;
        if (!workItemTypes.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, bool>) (r => TFStringComparer.WorkItemTypeName.Equals(r.Name, whenRule.Value))))
          rulesToRemove.Add(whenRule);
      }
      if (!whenRuleList1.Any<WhenRule>() && !rulesToRemove.Any<WhenRule>())
        return;
      whenRules.AddRange((IEnumerable<WhenRule>) whenRuleList1);
      rulesToRemove.ForEach((Action<WhenRule>) (r => whenRules.Remove(r)));
      IEnumerable<WorkItemRule> first = ((IEnumerable<WorkItemRule>) kanbanColumnFieldRule.SubRules).Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !(r is WhenRule)));
      kanbanColumnFieldRule.SubRules = first.Union<WorkItemRule>((IEnumerable<WorkItemRule>) whenRules).ToArray<WorkItemRule>();
      WorkItemExtensionPredicate extensionPredicate = KanbanUtils.Instance.CreateExtensionPredicate(requestContext, project, team, backlogLevel, teamSettings);
      IWorkItemTypeExtensionService service2 = requestContext.GetService<IWorkItemTypeExtensionService>();
      WorkItemTypeExtension extension1 = service2.UpdateExtension(requestContext, extension.Id, extension.ProjectId, extension.OwnerId, extension.Name, extension.Description, extension.Fields.Select<WorkItemTypeExtensionFieldEntry, WorkItemTypeExtensionFieldDeclaration>((Func<WorkItemTypeExtensionFieldEntry, WorkItemTypeExtensionFieldDeclaration>) (f => new WorkItemTypeExtensionFieldDeclaration(f))), extensionPredicate, extension.FieldRules);
      ReconcileRequestResult reconcileRequestResult = service2.ReconcileExtension(requestContext, extension1, 1000, false);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ExtensionName", extension.Name);
      properties.Add("ProjectName", project.Name);
      properties.Add("ProjectId", (object) id);
      properties.Add("AddedRuleCount", (double) whenRuleList1.Count);
      properties.Add("RemovedRuleCount", (double) rulesToRemove.Count);
      properties.Add("ReconcileResult", Enum.GetName(typeof (ReconcileRequestResult), (object) reconcileRequestResult));
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "PopulateStateMappings", "SyncWhenRulesForCustomWorkItemTypes.UpdateExtension", properties);
    }
  }
}
