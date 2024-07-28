// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.FileActionResponse
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class FileActionResponse : ActionResponse
  {
    [JsonProperty(Required = Required.Default)]
    public override int Precedence { get; set; }

    [JsonProperty(Required = Required.Default)]
    public override string FlightName { get; set; }

    [JsonProperty(Required = Required.Default)]
    public override string RuleId { get; set; }

    [JsonProperty(Required = Required.Default)]
    public override string ActionType { get; set; }

    [JsonProperty(Required = Required.Always)]
    [JsonConverter(typeof (TestActionJsonConverter))]
    public override string ActionJson { get; set; }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
      this.RuleId = this.RuleId ?? Guid.NewGuid().ToString();
      this.ActionType = this.ActionType ?? string.Empty;
      this.FlightName = this.FlightName ?? string.Empty;
    }
  }
}
