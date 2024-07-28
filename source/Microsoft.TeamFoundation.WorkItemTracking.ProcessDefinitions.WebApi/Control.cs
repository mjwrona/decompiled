// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models
{
  [DataContract]
  public class Control
  {
    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(Name = "inherited", EmitDefaultValue = false)]
    public bool FromInheritedLayout { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Overridden { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Label { get; set; }

    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ControlType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Order { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Height { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? ReadOnly { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Watermark { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Metadata { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Visible { get; set; }

    [DataMember(Name = "contribution", EmitDefaultValue = false)]
    public WitContribution Contribution { get; set; }

    [DataMember(Name = "isContribution")]
    public bool IsContribution => this.Contribution != null;
  }
}
