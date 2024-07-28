// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BoardFilterSettingsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class BoardFilterSettingsModel
  {
    public BoardFilterSettingsModel() => this.ParentWorkItemIds = (IEnumerable<int>) new List<int>();

    [DataMember(Order = 1, Name = "queryText", IsRequired = false, EmitDefaultValue = false)]
    public string QueryText { get; set; }

    [DataMember(Order = 2, Name = "criteria", IsRequired = false, EmitDefaultValue = false)]
    public FilterModel QueryExpression { get; set; }

    [DataMember(Order = 3, Name = "parentWorkItemIds", IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<int> ParentWorkItemIds { get; set; }
  }
}
