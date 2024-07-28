// Decompiled with JetBrains decompiler
// Type: Nest.CircuitBreakerSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class CircuitBreakerSettings : ICircuitBreakerSettings
  {
    public string FielddataLimit { get; set; }

    public float? FielddataOverhead { get; set; }

    public string RequestLimit { get; set; }

    public float? RequestOverhead { get; set; }

    public string TotalLimit { get; set; }
  }
}
