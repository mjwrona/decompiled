// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WorkItemData
// Assembly: Microsoft.TeamFoundation.CodeSense.Shared.Hosted, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2039D619-61FF-4054-B164-BD20C0E404E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Shared.Hosted.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class WorkItemData
  {
    public WorkItemData(
      int id,
      string title,
      string type,
      string category,
      string state,
      UserData assignedTo,
      UserData createdBy,
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

    public WorkItemData(int id)
      : this(id, string.Empty, string.Empty, string.Empty, string.Empty, (UserData) null, (UserData) null, new DateTime?())
    {
    }

    [JsonConstructor]
    private WorkItemData()
      : this(0)
    {
    }

    [JsonProperty]
    public int Id { get; private set; }

    [JsonProperty]
    public string Title { get; private set; }

    [JsonProperty]
    public string Type { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Category { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public UserData AssignedTo { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public UserData CreatedBy { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string State { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastModified { get; private set; }
  }
}
