// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ValidationItem
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [KnownType(typeof (ExpressionValidationItem))]
  [KnownType(typeof (InputValidationItem))]
  [JsonConverter(typeof (ValidationItemJsonConverter))]
  public class ValidationItem
  {
    protected ValidationItem(string type) => this.Type = type;

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public string Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsValid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Reason { get; set; }
  }
}
