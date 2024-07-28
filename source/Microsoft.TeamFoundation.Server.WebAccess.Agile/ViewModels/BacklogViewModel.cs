// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.BacklogViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class BacklogViewModel : CoreBacklogViewModel
  {
    [DataMember(Name = "queryResults", EmitDefaultValue = true)]
    public ProductBacklogQueryResults QueryResults { get; set; }

    [DataMember(Name = "backlogContextWorkItemTypeNames", EmitDefaultValue = true)]
    public string[] BacklogContextWorkItemTypeNames { get; set; }

    [DataMember(Name = "addPanelSettings", EmitDefaultValue = true)]
    public AddPanelViewModel AddPanelSettings { get; set; }

    [DataMember(Name = "pluralName", EmitDefaultValue = true)]
    public string PluralName { get; set; }

    [DataMember(Name = "sprintView", EmitDefaultValue = true)]
    public SprintViewViewModel SprintView { get; set; }

    [DataMember(Name = "forecastSettings", EmitDefaultValue = true)]
    public ForecastLinesViewModel ForecastSettings { get; set; }

    [DataMember(Name = "mappingPanelFilterState", EmitDefaultValue = true)]
    public string MappingPanelFilterState { get; set; }

    [DataMember(Name = "mappingPanel", EmitDefaultValue = true)]
    public MappingPanelViewModel MappingPanel { get; set; }

    [DataMember(Name = "isRootBacklog", EmitDefaultValue = true)]
    public bool IsRootBacklog { get; set; }

    [DataMember(Name = "isRequirementBacklog", EmitDefaultValue = true)]
    public bool IsRequirementBacklog { get; set; }

    [DataMember(Name = "pageTitle", EmitDefaultValue = true)]
    public string PageTitle { get; set; }

    [DataMember(Name = "pageTooltip", EmitDefaultValue = true)]
    public string PageTooltip { get; set; }

    [DataMember(Name = "velocityChartSettings", EmitDefaultValue = true)]
    public VelocityChartViewModel VelocityChartSettings { get; set; }

    [DataMember(Name = "cumulativeFlowDiagramSettings", EmitDefaultValue = true)]
    public CumulativeFlowDiagramViewModel CumulativeFlowDiagramSettings { get; set; }

    [DataMember(Name = "agilePortfolioManagementNotificationSettings", EmitDefaultValue = true)]
    public AgilePortfolioManagementNotificationViewModel AgilePortfolioManagementNotificationSettings { get; set; }

    [DataMember(Name = "inProgressStates", EmitDefaultValue = true)]
    public string InProgressStates { get; set; }

    [DataMember(Name = "columnPreferences", EmitDefaultValue = true)]
    public ColumnPreferences ColumnPreferences { get; set; }

    [DataMember(Name = "automationRulesStates")]
    public IDictionary<string, bool> AutomationRulesStates { get; set; }
  }
}
