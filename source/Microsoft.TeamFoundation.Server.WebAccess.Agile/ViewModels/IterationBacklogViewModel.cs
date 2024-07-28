// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.IterationBacklogViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class IterationBacklogViewModel : CoreBacklogViewModel
  {
    [DataMember(Name = "agileContext", IsRequired = true)]
    public SprintInformation SprintInformation { get; set; }

    [DataMember(Name = "sprintDatesOptions")]
    public SprintDatesOptionsViewModel SprintDatesOptions { get; set; }

    [DataMember(Name = "burndownChartOptions")]
    public BurndownChartOptionsViewModel BurndownChartOptions { get; set; }

    [DataMember(Name = "iterationBacklogOptions")]
    public IterationBacklogOptionsViewModel IterationBacklogOptions { get; set; }

    [DataMember(Name = "aggregatedCapacityData")]
    public AggregatedCapacityDataViewModel AggregatedCapacityData { get; set; }

    [DataMember(Name = "capacityOptions")]
    public CapacityOptionsViewModel CapacityOptions { get; set; }

    [DataMember(Name = "teamSettingsModel")]
    public TeamWITSettingsModel TeamSettingsModel { get; set; }

    [DataMember(Name = "addPanelViewModel")]
    public AddPanelViewModel AddPanelViewModel { get; set; }

    [DataMember(Name = "queryResults")]
    public ProductBacklogQueryResults QueryResults { get; set; }
  }
}
