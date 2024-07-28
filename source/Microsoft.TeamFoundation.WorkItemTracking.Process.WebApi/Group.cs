// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class Group
  {
    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(Name = "inherited", EmitDefaultValue = false)]
    public bool FromInheritedLayout { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Overridden { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Label { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Order { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WitContribution Contribution { get; set; }

    [DataMember(Name = "isContribution")]
    public bool IsContribution => this.Contribution != null;

    [DataMember(EmitDefaultValue = false)]
    public bool? Visible { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Control> Controls { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Height { get; set; }
  }
}
