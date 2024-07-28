// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WorkItemTypeModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models
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
