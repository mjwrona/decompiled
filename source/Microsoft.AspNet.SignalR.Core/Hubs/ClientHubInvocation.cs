// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.ClientHubInvocation
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class ClientHubInvocation
  {
    [JsonProperty("H")]
    public string Hub { get; set; }

    [JsonProperty("M")]
    public string Method { get; set; }

    [JsonProperty("A")]
    public object[] Args { get; set; }

    [JsonProperty("S", NullValueHandling = NullValueHandling.Ignore)]
    public IDictionary<string, object> State { get; set; }
  }
}
