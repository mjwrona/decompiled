// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Process.FieldUsage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Process
{
  [DataContract]
  public class FieldUsage
  {
    [DataMember(Name = "WorkItemTypeId")]
    public string WorkItemTypeId { get; set; }

    [DataMember(Name = "IsInherited")]
    public bool IsInherited { get; set; }

    [DataMember(Name = "IsSystem")]
    public bool IsSystem { get; set; }

    [DataMember(Name = "IsBehaviorField")]
    public bool IsBehaviorField { get; set; }

    [DataMember(Name = "CanEditFieldProperties")]
    public bool CanEditFieldProperties { get; set; }

    [DataMember(Name = "Properties")]
    public UsageProperties Properties { get; set; }
  }
}
