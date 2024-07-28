// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableResultSegment
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class TableResultSegment : IEnumerable<CloudTable>, IEnumerable
  {
    private TableContinuationToken continuationToken;

    internal TableResultSegment(List<CloudTable> result) => this.Results = (IList<CloudTable>) result;

    public IList<CloudTable> Results { get; internal set; }

    public TableContinuationToken ContinuationToken
    {
      get => this.continuationToken != null ? this.continuationToken : (TableContinuationToken) null;
      internal set => this.continuationToken = value;
    }

    public IEnumerator<CloudTable> GetEnumerator() => this.Results.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Results.GetEnumerator();
  }
}
