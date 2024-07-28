// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.CollectionQueryResult`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class CollectionQueryResult<T> : IEnumerable<T>, IEnumerable where T : EntityDescription
  {
    private IEnumerable<T> results;

    internal CollectionQueryResult(IEnumerable<T> results, string continuationToken)
    {
      this.results = results;
      if (this.results == null)
        this.results = (IEnumerable<T>) new T[0];
      this.ContinuationToken = continuationToken;
    }

    public string ContinuationToken { get; private set; }

    public IEnumerator<T> GetEnumerator() => this.results.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.results.GetEnumerator();
  }
}
