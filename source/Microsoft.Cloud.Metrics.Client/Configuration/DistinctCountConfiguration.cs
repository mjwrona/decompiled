// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class DistinctCountConfiguration : IDistinctCountConfiguration
  {
    private readonly List<string> dimensions;

    public DistinctCountConfiguration() => this.dimensions = new List<string>();

    [JsonConstructor]
    internal DistinctCountConfiguration(IEnumerable<string> dimensions) => this.dimensions = (dimensions != null ? dimensions.ToList<string>() : (List<string>) null) ?? new List<string>();

    public IEnumerable<string> Dimensions => (IEnumerable<string>) this.dimensions;

    public void AddDimension(string dimensionToAdd)
    {
      if (string.IsNullOrWhiteSpace(dimensionToAdd))
        throw new ArgumentNullException(nameof (dimensionToAdd));
      if (this.dimensions.Count == 0)
      {
        this.dimensions.Add(dimensionToAdd);
      }
      else
      {
        for (int index = 0; index < this.dimensions.Count; ++index)
        {
          int num = string.Compare(this.dimensions[index], dimensionToAdd, StringComparison.OrdinalIgnoreCase);
          if (num == 0)
            throw new ConfigurationValidationException("Cannot add duplicate dimensions.", ValidationType.DuplicateDimension);
          if (num > 0)
          {
            this.dimensions.Insert(index, dimensionToAdd);
            break;
          }
          if (index + 1 == this.dimensions.Count)
          {
            this.dimensions.Add(dimensionToAdd);
            break;
          }
        }
      }
    }

    public void RemoveDimension(string dimension) => this.dimensions.RemoveAll((Predicate<string>) (x => string.Equals(x, dimension, StringComparison.OrdinalIgnoreCase)));
  }
}
