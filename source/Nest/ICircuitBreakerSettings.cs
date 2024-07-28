// Decompiled with JetBrains decompiler
// Type: Nest.ICircuitBreakerSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface ICircuitBreakerSettings
  {
    string FielddataLimit { get; set; }

    float? FielddataOverhead { get; set; }

    string RequestLimit { get; set; }

    float? RequestOverhead { get; set; }

    string TotalLimit { get; set; }
  }
}
