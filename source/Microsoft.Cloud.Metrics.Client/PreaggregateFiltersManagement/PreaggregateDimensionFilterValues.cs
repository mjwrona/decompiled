// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement.PreaggregateDimensionFilterValues
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement
{
  [JsonObject]
  internal sealed class PreaggregateDimensionFilterValues
  {
    [JsonConstructor]
    public PreaggregateDimensionFilterValues(
      string filterDimensionName,
      IReadOnlyList<string> filterValues)
    {
      if (string.IsNullOrEmpty(filterDimensionName))
        throw new ArgumentNullException(nameof (filterDimensionName));
      if (filterValues == null)
        throw new ArgumentNullException(nameof (filterValues));
      foreach (string filterValue in (IEnumerable<string>) filterValues)
      {
        if (string.IsNullOrEmpty(filterValue))
          throw new ArgumentException("filterValues cannot have empty of null values");
      }
      this.FilterDimensionName = filterDimensionName;
      this.FilterValues = (ISet<string>) new HashSet<string>((IEnumerable<string>) filterValues, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public string FilterDimensionName { get; }

    public ISet<string> FilterValues { get; }
  }
}
