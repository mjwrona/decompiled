// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CreateProcessWorkItemTypeRequest
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class CreateProcessWorkItemTypeRequest
  {
    [DataMember(IsRequired = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Description { get; set; }

    [DataMember(IsRequired = false)]
    public string Color { get; set; }

    [DataMember(IsRequired = false)]
    public string Icon { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsDisabled { get; set; }

    [DataMember(IsRequired = false)]
    public string InheritsFrom { get; set; }
  }
}
