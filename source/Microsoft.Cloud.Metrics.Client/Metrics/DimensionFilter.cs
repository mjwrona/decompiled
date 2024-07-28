// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.DimensionFilter
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public sealed class DimensionFilter
  {
    private const string StartsWithFilterMarker = "λαstartswith";
    private readonly string dimensionName;
    private readonly string[] dimensionValues;
    private readonly bool isExcludeFilter;

    [JsonConstructor]
    public DimensionFilter(
      string dimensionName,
      IEnumerable<string> dimensionValues,
      bool isExcludeFilter)
    {
      this.dimensionName = !string.IsNullOrWhiteSpace(dimensionName) ? dimensionName : throw new ArgumentException("dimensionName is null or empty");
      this.dimensionValues = dimensionValues != null ? dimensionValues.ToArray<string>() : (string[]) null;
      this.isExcludeFilter = isExcludeFilter;
    }

    public string DimensionName => this.dimensionName;

    public IReadOnlyList<string> DimensionValues => (IReadOnlyList<string>) this.dimensionValues;

    public bool IsExcludeFilter => this.isExcludeFilter;

    internal bool IsStartsWithFilter => this.DimensionValues != null && this.DimensionValues.Count == 1 && this.DimensionValues[0].StartsWith("λαstartswith");

    public static implicit operator DimensionFilter(string dimensionName) => DimensionFilter.CreateIncludeFilter(dimensionName);

    public static DimensionFilter CreateIncludeFilter(
      string dimensionName,
      params string[] dimensionValues)
    {
      return DimensionFilter.CreateIncludeFilter(dimensionName, ((IEnumerable<string>) dimensionValues).AsEnumerable<string>());
    }

    public static DimensionFilter CreateIncludeFilter(
      string dimensionName,
      IEnumerable<string> dimensionValues)
    {
      return new DimensionFilter(dimensionName, dimensionValues, false);
    }

    public static DimensionFilter CreateExcludeFilter(
      string dimensionName,
      params string[] dimensionValues)
    {
      return DimensionFilter.CreateExcludeFilter(dimensionName, ((IEnumerable<string>) dimensionValues).AsEnumerable<string>());
    }

    public static DimensionFilter CreateExcludeFilter(
      string dimensionName,
      IEnumerable<string> dimensionValues)
    {
      return new DimensionFilter(dimensionName, dimensionValues, true);
    }

    public static DimensionFilter CreateStartsWithIncludeFilter(
      string dimensionName,
      string startsWithCondition)
    {
      startsWithCondition = "λαstartswith" + startsWithCondition;
      return DimensionFilter.CreateIncludeFilter(dimensionName, startsWithCondition);
    }

    public static DimensionFilter CreateStartsWithExcludeFilter(
      string dimensionName,
      string startsWithCondition)
    {
      startsWithCondition = "λαstartswith" + startsWithCondition;
      return DimensionFilter.CreateExcludeFilter(dimensionName, startsWithCondition);
    }
  }
}
