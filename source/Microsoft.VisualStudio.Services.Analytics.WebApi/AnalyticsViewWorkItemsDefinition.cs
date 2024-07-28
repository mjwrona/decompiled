// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsViewWorkItemsDefinition
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  [ClientIncludeModel]
  public class AnalyticsViewWorkItemsDefinition
  {
    [DataMember]
    public FieldSet FieldSet { get; set; }

    [DataMember]
    public HistoryConfiguration HistoryConfiguration { get; set; }

    [DataMember]
    public bool IsTeamFilterBySelectionMode { get; set; }

    [DataMember]
    public IList<ProjectTeamFilter> ProjectTeamFilters { get; set; }

    [DataMember]
    public IList<AreaPathFilter> AreaPathFilters { get; set; }

    [DataMember]
    public IList<string> Backlogs { get; set; }

    [DataMember]
    public FilterValues<string> WorkItemTypes { get; set; }

    [DataMember]
    public IList<FieldFilter> FieldFilters { get; set; }
  }
}
