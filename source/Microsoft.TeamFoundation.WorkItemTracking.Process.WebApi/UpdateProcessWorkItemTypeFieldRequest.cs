// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.UpdateProcessWorkItemTypeFieldRequest
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class UpdateProcessWorkItemTypeFieldRequest : IFieldRuleProperties
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool ReadOnly { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool Required { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    [JsonConverter(typeof (IdentityRefOrJValueConverter))]
    public object DefaultValue { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool? AllowGroups { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string[] AllowedValues { get; set; }
  }
}
