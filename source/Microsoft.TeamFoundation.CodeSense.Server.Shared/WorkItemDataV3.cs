// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WorkItemDataV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class WorkItemDataV3
  {
    public WorkItemDataV3()
    {
    }

    public WorkItemDataV3(
      int id,
      string title,
      string type,
      string category,
      string state,
      string assignedTo,
      string createdBy,
      DateTime? lastModified)
    {
      this.Id = id;
      this.Title = title;
      this.Type = type;
      this.Category = category;
      this.State = state;
      this.AssignedTo = assignedTo;
      this.CreatedBy = createdBy;
      this.LastModified = lastModified;
    }

    public WorkItemDataV3(WorkItemData workItem)
    {
      this.Id = workItem.Id;
      this.Title = workItem.Title;
      this.Type = workItem.Type;
      this.Category = workItem.Category;
      this.State = workItem.State;
      this.AssignedTo = workItem.AssignedTo != null ? workItem.AssignedTo.UniqueName : (string) null;
      this.CreatedBy = workItem.CreatedBy != null ? workItem.CreatedBy.UniqueName : (string) null;
      this.LastModified = workItem.LastModified;
    }

    public void RemoveDetails()
    {
      this.Title = (string) null;
      this.CreatedBy = (string) null;
      this.AssignedTo = (string) null;
      this.State = (string) null;
      this.LastModified = new DateTime?();
    }

    [JsonProperty]
    public int Id { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Title { get; private set; }

    [JsonProperty]
    public string Type { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Category { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string AssignedTo { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string CreatedBy { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string State { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastModified { get; private set; }
  }
}
