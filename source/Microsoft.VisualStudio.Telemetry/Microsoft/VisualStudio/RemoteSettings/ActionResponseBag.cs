// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ActionResponseBag
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class ActionResponseBag
  {
    [JsonProperty(Required = Required.AllowNull)]
    public string ProductName { get; set; }

    [JsonProperty(Required = Required.Always)]
    public IEnumerable<ActionResponse> Actions { get; set; } = (IEnumerable<ActionResponse>) new List<ActionResponse>();

    [JsonProperty(Required = Required.Always)]
    public IEnumerable<ActionCategory> Categories { get; set; } = (IEnumerable<ActionCategory>) new List<ActionCategory>();
  }
}
