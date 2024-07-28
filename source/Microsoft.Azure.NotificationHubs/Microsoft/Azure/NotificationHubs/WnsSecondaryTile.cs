// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.WnsSecondaryTile
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs
{
  [JsonObject(MemberSerialization = MemberSerialization.OptOut, ItemRequired = Required.Default)]
  public class WnsSecondaryTile
  {
    [JsonProperty(Required = Required.Always, PropertyName = "pushChannel")]
    public string PushChannel { get; set; }

    [JsonProperty(PropertyName = "pushChannelExpired")]
    public bool? PushChannelExpired { get; set; }

    [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> Tags { get; set; }

    [JsonProperty(PropertyName = "templates", NullValueHandling = NullValueHandling.Ignore)]
    public IDictionary<string, InstallationTemplate> Templates { get; set; }

    internal string ToJson() => JsonConvert.SerializeObject((object) this);
  }
}
