// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WorkItemTypeModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class WorkItemTypeModel
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public string Inherits { get; set; }

    [DataMember]
    public WorkItemTypeClass Class { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public string Icon { get; set; }

    [DataMember]
    public bool? IsDisabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<WorkItemStateResultModel> States { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IReadOnlyCollection<WorkItemTypeBehavior> Behaviors { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public FormLayout Layout { get; set; }
  }
}
