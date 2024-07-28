// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.Models.AdminBehaviorField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43776F51-3CE9-4177-A1CB-61A3432CC4EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessTemplate.WebApi.Models
{
  [DataContract]
  public class AdminBehaviorField
  {
    [DataMember]
    public string BehaviorFieldId { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }
  }
}
