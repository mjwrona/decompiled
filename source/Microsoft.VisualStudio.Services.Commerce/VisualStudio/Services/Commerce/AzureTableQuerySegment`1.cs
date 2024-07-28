// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureTableQuerySegment`1
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureTableQuerySegment<TElement> : IEnumerable<TElement>, IEnumerable
  {
    public AzureTableQuerySegment(List<TElement> results, TableContinuationToken continuationToken)
    {
      this.Results = results;
      this.ContinuationToken = continuationToken;
    }

    public List<TElement> Results { get; private set; }

    public TableContinuationToken ContinuationToken { get; private set; }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Results.GetEnumerator();

    public IEnumerator<TElement> GetEnumerator() => (IEnumerator<TElement>) this.Results.GetEnumerator();
  }
}
