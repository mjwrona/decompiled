// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemTypeField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class ProcessWorkItemTypeField : 
    IBasicFieldProperties,
    IFieldRuleProperties,
    IFieldLockingProperties
  {
    [DataMember(IsRequired = false)]
    public string ReferenceName { get; set; }

    [DataMember(IsRequired = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false)]
    public FieldType Type { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool ReadOnly { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool Required { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    [JsonConverter(typeof (IdentityRefOrJValueConverter))]
    public object DefaultValue { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool? AllowGroups { get; set; }

    [DataMember(IsRequired = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false)]
    public CustomizationType Customization { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ValueArrayConverter))]
    public object[] AllowedValues { get; set; }

    [DataMember]
    public bool IsLocked { get; set; }
  }
}
