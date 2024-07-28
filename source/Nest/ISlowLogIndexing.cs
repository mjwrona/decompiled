// Decompiled with JetBrains decompiler
// Type: Nest.ISlowLogIndexing
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface ISlowLogIndexing
  {
    Nest.LogLevel? LogLevel { get; set; }

    int? Source { get; set; }

    Time ThresholdDebug { get; set; }

    Time ThresholdInfo { get; set; }

    Time ThresholdTrace { get; set; }

    Time ThresholdWarn { get; set; }
  }
}
