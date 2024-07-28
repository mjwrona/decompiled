// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.TruncatedCollection`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class TruncatedCollection<T> : 
    List<T>,
    ITruncatedCollection,
    IEnumerable,
    IEnumerable<T>,
    ICountOptionCollection
  {
    private const int MinPageSize = 1;
    private bool _isTruncated;
    private int _pageSize;
    private long? _totalCount;

    public TruncatedCollection(IEnumerable<T> source, int pageSize)
      : base(source.Take<T>(checked (pageSize + 1)))
    {
      this.Initialize(pageSize);
    }

    public TruncatedCollection(IQueryable<T> source, int pageSize)
      : this(source, pageSize, false)
    {
    }

    public TruncatedCollection(IQueryable<T> source, int pageSize, bool parameterize)
      : base((IEnumerable<T>) TruncatedCollection<T>.Take(source, pageSize, parameterize))
    {
      this.Initialize(pageSize);
    }

    public TruncatedCollection(IEnumerable<T> source, int pageSize, long? totalCount)
      : base(pageSize > 0 ? source.Take<T>(checked (pageSize + 1)) : source)
    {
      if (pageSize > 0)
        this.Initialize(pageSize);
      this._totalCount = totalCount;
    }

    public TruncatedCollection(IQueryable<T> source, int pageSize, long? totalCount)
      : this(source, pageSize, totalCount, false)
    {
    }

    public TruncatedCollection(
      IQueryable<T> source,
      int pageSize,
      long? totalCount,
      bool parameterize)
      : base(pageSize > 0 ? (IEnumerable<T>) TruncatedCollection<T>.Take(source, pageSize, parameterize) : (IEnumerable<T>) source)
    {
      if (pageSize > 0)
        this.Initialize(pageSize);
      this._totalCount = totalCount;
    }

    private void Initialize(int pageSize)
    {
      this._pageSize = pageSize >= 1 ? pageSize : throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (pageSize), (object) pageSize, (object) 1);
      if (this.Count <= pageSize)
        return;
      this._isTruncated = true;
      this.RemoveAt(this.Count - 1);
    }

    private static IQueryable<T> Take(IQueryable<T> source, int pageSize, bool parameterize)
    {
      if (source == null)
        throw Error.ArgumentNull(nameof (source));
      return ExpressionHelpers.Take((IQueryable) source, checked (pageSize + 1), typeof (T), parameterize) as IQueryable<T>;
    }

    public int PageSize => this._pageSize;

    public bool IsTruncated => this._isTruncated;

    public long? TotalCount => this._totalCount;
  }
}
