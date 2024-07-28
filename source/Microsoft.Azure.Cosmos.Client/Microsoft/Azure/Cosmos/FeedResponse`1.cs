// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public abstract class FeedResponse<T> : Response<IEnumerable<T>>, IEnumerable<T>, IEnumerable
  {
    public override double RequestCharge
    {
      get
      {
        Headers headers = this.Headers;
        return headers == null ? 0.0 : headers.RequestCharge;
      }
    }

    public override string ActivityId => this.Headers?.ActivityId;

    public override string ETag => this.Headers?.ETag;

    public abstract string ContinuationToken { get; }

    public abstract int Count { get; }

    public abstract string IndexMetrics { get; }

    internal override RequestMessage RequestMessage { get; }

    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
