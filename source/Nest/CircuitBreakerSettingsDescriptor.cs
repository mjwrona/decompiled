// Decompiled with JetBrains decompiler
// Type: Nest.CircuitBreakerSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class CircuitBreakerSettingsDescriptor : 
    DescriptorBase<CircuitBreakerSettingsDescriptor, ICircuitBreakerSettings>,
    ICircuitBreakerSettings
  {
    string ICircuitBreakerSettings.FielddataLimit { get; set; }

    float? ICircuitBreakerSettings.FielddataOverhead { get; set; }

    string ICircuitBreakerSettings.RequestLimit { get; set; }

    float? ICircuitBreakerSettings.RequestOverhead { get; set; }

    string ICircuitBreakerSettings.TotalLimit { get; set; }

    public CircuitBreakerSettingsDescriptor TotalLimit(string limit) => this.Assign<string>(limit, (Action<ICircuitBreakerSettings, string>) ((a, v) => a.TotalLimit = v));

    public CircuitBreakerSettingsDescriptor FielddataLimit(string limit) => this.Assign<string>(limit, (Action<ICircuitBreakerSettings, string>) ((a, v) => a.FielddataLimit = v));

    public CircuitBreakerSettingsDescriptor RequestLimit(string limit) => this.Assign<string>(limit, (Action<ICircuitBreakerSettings, string>) ((a, v) => a.RequestLimit = v));

    public CircuitBreakerSettingsDescriptor FielddataOverhead(float? overhead) => this.Assign<float?>(overhead, (Action<ICircuitBreakerSettings, float?>) ((a, v) => a.FielddataOverhead = v));

    public CircuitBreakerSettingsDescriptor RequestOverhead(float? overhead) => this.Assign<float?>(overhead, (Action<ICircuitBreakerSettings, float?>) ((a, v) => a.RequestOverhead = v));
  }
}
