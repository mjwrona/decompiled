// Decompiled with JetBrains decompiler
// Type: Nest.IScrollAllRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface IScrollAllRequest
  {
    ProducerConsumerBackPressure BackPressure { get; set; }

    int? MaxDegreeOfParallelism { get; set; }

    Field RoutingField { get; set; }

    Time ScrollTime { get; set; }

    ISearchRequest Search { get; set; }

    int Slices { get; set; }
  }
}
