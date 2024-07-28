// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ActionResponse
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class ActionResponse
  {
    [JsonProperty(Required = Required.Always)]
    public string ActionPath { get; set; }

    [JsonProperty(Required = Required.Always)]
    public virtual int Precedence { get; set; }

    [JsonProperty(Required = Required.Always)]
    public virtual string RuleId { get; set; }

    [JsonProperty(Required = Required.Always)]
    public virtual string ActionType { get; set; }

    [JsonProperty(Required = Required.AllowNull)]
    public virtual string FlightName { get; set; }

    [JsonProperty(Required = Required.Always)]
    public virtual string ActionJson { get; set; }

    [JsonProperty(Required = Required.AllowNull)]
    public virtual string TriggerJson { get; set; }

    [JsonProperty(Required = Required.AllowNull)]
    public virtual string MaxWaitTimeSpan { get; set; }

    [JsonProperty(Required = Required.Default)]
    public virtual bool SendAlways { get; set; }

    [JsonProperty(Required = Required.AllowNull)]
    public virtual IList<string> Categories { get; set; }

    [JsonProperty(Required = Required.Default)]
    public string Origin { get; set; }

    public override string ToString() => string.IsNullOrEmpty(this.Origin) ? this.RuleId : this.Origin + "-" + this.RuleId;
  }
}
