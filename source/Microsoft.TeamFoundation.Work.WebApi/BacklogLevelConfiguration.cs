// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class BacklogLevelConfiguration
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Rank { get; set; }

    [DataMember]
    public int WorkItemCountLimit { get; set; }

    [DataMember]
    public IReadOnlyCollection<WorkItemFieldReference> AddPanelFields { get; set; }

    [DataMember]
    public BacklogColumn[] ColumnFields { get; set; }

    [DataMember]
    public IReadOnlyCollection<WorkItemTypeReference> WorkItemTypes { get; set; }

    [DataMember]
    public WorkItemTypeReference DefaultWorkItemType { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public bool IsHidden { get; set; }

    [DataMember]
    public BacklogType Type { get; set; }
  }
}
