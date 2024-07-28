// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.QueryOptions`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class QueryOptions<TEntry>
  {
    private readonly List<string> projectionProperties;
    private readonly List<Func<TEntry, bool>> filters;

    public QueryOptions()
    {
    }

    private QueryOptions(string propertyName, QueryOptions<TEntry> queryOptions)
    {
      this.projectionProperties = new List<string>((IEnumerable<string>) (queryOptions.projectionProperties ?? new List<string>()));
      this.projectionProperties.Add(propertyName);
      this.VersionLower = queryOptions.VersionLower;
      this.VersionUpper = queryOptions.VersionUpper;
      if (queryOptions.Filters == null)
        return;
      this.filters = new List<Func<TEntry, bool>>(queryOptions.Filters);
    }

    private QueryOptions(Func<TEntry, bool> filter, QueryOptions<TEntry> queryOptions)
    {
      if (queryOptions.projectionProperties != null)
        this.projectionProperties = new List<string>((IEnumerable<string>) queryOptions.projectionProperties);
      this.filters = new List<Func<TEntry, bool>>(queryOptions.Filters ?? (IEnumerable<Func<TEntry, bool>>) new List<Func<TEntry, bool>>());
      this.filters.Add(filter);
      this.VersionLower = queryOptions.VersionLower;
      this.VersionUpper = queryOptions.VersionUpper;
    }

    public IPackageVersion VersionLower { get; set; }

    public IPackageVersion VersionUpper { get; set; }

    public IEnumerable<Func<TEntry, bool>> Filters => (IEnumerable<Func<TEntry, bool>>) this.filters;

    public QueryOptions<TEntry> OnlyProjecting(Expression<Func<TEntry, object>> input) => new QueryOptions<TEntry>(ExpressionUtils.NameOf<TEntry>(input), this);

    public QueryOptions<TEntry> WithFilter(Func<TEntry, bool> filter) => new QueryOptions<TEntry>(filter, this);

    public List<string> GetProjections() => this.projectionProperties;
  }
}
