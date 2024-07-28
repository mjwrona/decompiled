// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubResponse
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class HubResponse
  {
    [JsonProperty("S", NullValueHandling = NullValueHandling.Ignore)]
    public IDictionary<string, object> State { get; set; }

    [JsonProperty("R", NullValueHandling = NullValueHandling.Ignore)]
    public object Result { get; set; }

    [JsonProperty("I")]
    public string Id { get; set; }

    [JsonProperty("P", NullValueHandling = NullValueHandling.Ignore)]
    public object Progress { get; set; }

    [JsonProperty("H", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsHubException { get; set; }

    [JsonProperty("E", NullValueHandling = NullValueHandling.Ignore)]
    public string Error { get; set; }

    [JsonProperty("T", NullValueHandling = NullValueHandling.Ignore)]
    public string StackTrace { get; set; }

    [JsonProperty("D", NullValueHandling = NullValueHandling.Ignore)]
    public object ErrorData { get; set; }
  }
}
