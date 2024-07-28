// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [ClientIncludeModel]
  [DataContract]
  public class BacklogLevelConfiguration
  {
    public BacklogLevelConfiguration()
    {
      this.AddPanelFields = Array.Empty<string>();
      this.ColumnFields = Array.Empty<BacklogColumn>();
      this.WorkItemTypes = (IReadOnlyCollection<string>) new List<string>();
    }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Rank { get; set; }

    [DataMember]
    public int WorkItemCountLimit { get; set; }

    [DataMember]
    public string[] AddPanelFields { get; set; }

    [DataMember]
    public BacklogColumn[] ColumnFields { get; set; }

    [DataMember]
    public IReadOnlyCollection<string> WorkItemTypes { get; set; }

    [DataMember]
    public string DefaultWorkItemType { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public bool Custom { get; set; }

    [DataMember]
    public bool IsRequirementsBacklog { get; set; }

    [DataMember]
    public bool IsTaskBacklog { get; set; }
  }
}
