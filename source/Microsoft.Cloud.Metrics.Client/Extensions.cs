// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Extensions
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Online.Metrics.Serialization.Configuration;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client
{
  public static class Extensions
  {
    public static int GetIndexInDimensionCombination(
      this IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>> definitions,
      string dimensionName)
    {
      if (definitions == null || definitions.Count == 0)
        throw new ArgumentException("definitions is null or empty.");
      if (string.IsNullOrWhiteSpace(dimensionName))
        throw new ArgumentException("dimensionName is null or empty.");
      TimeSeriesDefinition<MetricIdentifier> definition = definitions[0];
      if (definition.DimensionCombination == null)
        return -1;
      for (int index = 0; index < definition.DimensionCombination.Count; ++index)
      {
        if (dimensionName.Equals(definition.DimensionCombination[index].Key, StringComparison.OrdinalIgnoreCase))
          return index;
      }
      return -1;
    }
  }
}
