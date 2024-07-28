// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryEventCorrelation
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  public struct TelemetryEventCorrelation
  {
    public static readonly TelemetryEventCorrelation Empty = new TelemetryEventCorrelation(Guid.Empty, DataModelEventType.Trace);

    [JsonProperty(PropertyName = "id", Required = Required.Always)]
    internal Guid Id { get; private set; }

    internal bool IsEmpty => this.Id == Guid.Empty;

    [JsonProperty(PropertyName = "eventType", Required = Required.Always)]
    [JsonConverter(typeof (StringEnumConverter))]
    internal DataModelEventType EventType { get; private set; }

    internal TelemetryEventCorrelation(Guid id, DataModelEventType eventType)
    {
      this.Id = id;
      this.EventType = eventType;
    }

    public string Serialize() => JsonConvert.SerializeObject((object) this);

    public static TelemetryEventCorrelation Deserialize(string jsonString)
    {
      jsonString.RequiresArgumentNotNullAndNotWhiteSpace(nameof (jsonString));
      return JsonConvert.DeserializeObject<TelemetryEventCorrelation>(jsonString);
    }
  }
}
