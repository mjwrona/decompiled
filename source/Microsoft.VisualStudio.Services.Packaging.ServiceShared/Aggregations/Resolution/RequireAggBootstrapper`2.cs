// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.RequireAggBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public abstract class RequireAggBootstrapper<TBootstrapped, TAgg1> : 
    IRequireAggBootstrapper<TBootstrapped>
    where TBootstrapped : class
    where TAgg1 : class
  {
    public TBootstrapped? Bootstrap(
      IReadOnlyCollection<IAggregationAccessor> aggTypeProvider)
    {
      TAgg1 aggregation;
      return !aggTypeProvider.TryGetAggregationAccessorOfType<TAgg1>(out aggregation) ? default (TBootstrapped) : this.Bootstrap(aggregation);
    }

    public IEnumerable<string> GetAggregationNamesForDiagnostics()
    {
      yield return typeof (TAgg1).GetPrettyName();
    }

    protected abstract TBootstrapped Bootstrap(TAgg1 agg1);
  }
}
