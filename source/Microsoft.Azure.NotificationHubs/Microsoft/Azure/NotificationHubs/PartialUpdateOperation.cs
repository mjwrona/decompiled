// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PartialUpdateOperation
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs
{
  [JsonObject(MemberSerialization = MemberSerialization.OptOut, ItemRequired = Required.Default)]
  public class PartialUpdateOperation
  {
    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "op")]
    public UpdateOperationType Operation { get; set; }

    [JsonProperty(PropertyName = "path")]
    public string Path { get; set; }

    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    internal static IList<PartialUpdateOperation> ListFromJson(string json) => (IList<PartialUpdateOperation>) JsonConvert.DeserializeObject<PartialUpdateOperation[]>(json);
  }
}
