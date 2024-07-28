// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataEnumerable`1
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataEnumerable<T> : IEnumerable<T>, IEnumerable, IODataEnumerable
  {
    private ODataEnumerator<T> enumerator;
    private IEnumerable<T> enumerable;
    private int recordCount;
    public int? pageSize;
    private bool nextPagePresent;
    private Uri skipUri;
    private Uri skipTokenUri;

    public ODataEnumerable(
      IEnumerable<T> enumerable,
      int? pageSize,
      Uri skipUri,
      Uri skipTokenUri)
    {
      this.enumerable = enumerable;
      this.pageSize = pageSize;
      this.skipUri = skipUri;
      this.skipTokenUri = skipTokenUri;
    }

    public IEnumerator<T> GetEnumerator()
    {
      if (this.enumerator != null)
        throw new InvalidOperationException(AnalyticsResources.DO_NOT_ENUMERATE_ODATA_ENUMERABLE_MORE_THAN_ONCE());
      this.recordCount = 0;
      this.enumerator = new ODataEnumerator<T>(this.enumerable.GetEnumerator(), this);
      return (IEnumerator<T>) this.enumerator;
    }

    public int GetCurrentCount() => this.recordCount;

    public void IncrementCurrentCount() => ++this.recordCount;

    public void SetNextPagePresent() => this.nextPagePresent = true;

    public void Reset()
    {
      this.recordCount = 0;
      this.nextPagePresent = false;
    }

    public bool ReachedPageSize()
    {
      if (!this.pageSize.HasValue)
        return false;
      int recordCount = this.recordCount;
      int? pageSize = this.pageSize;
      int valueOrDefault = pageSize.GetValueOrDefault();
      return recordCount >= valueOrDefault & pageSize.HasValue;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public Uri NextPageLink()
    {
      if (!this.pageSize.HasValue && this.skipTokenUri != (Uri) null)
        return this.skipTokenUri;
      return this.pageSize.HasValue && this.nextPagePresent && this.skipUri != (Uri) null ? this.skipUri : (Uri) null;
    }
  }
}
