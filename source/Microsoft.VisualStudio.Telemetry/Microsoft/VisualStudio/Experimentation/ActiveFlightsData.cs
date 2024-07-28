// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.ActiveFlightsData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class ActiveFlightsData : IFlightsData
  {
    [JsonProperty(PropertyName = "Features", Required = Required.Always)]
    public IList<string> Flights { get; set; }

    [JsonProperty(PropertyName = "Configs", Required = Required.Default)]
    public IList<ConfigData> Configs { get; set; }

    [JsonProperty(PropertyName = "AssignmentContext", Required = Required.Default)]
    public string AssignmentContext { get; set; }
  }
}
