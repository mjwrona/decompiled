// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ReadOnlyEnumerableForUriParser`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class ReadOnlyEnumerableForUriParser<T> : IEnumerable<T>, IEnumerable
  {
    private IEnumerable<T> sourceEnumerable;

    internal ReadOnlyEnumerableForUriParser(IEnumerable<T> sourceEnumerable) => this.sourceEnumerable = sourceEnumerable;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.sourceEnumerable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.sourceEnumerable.GetEnumerator();
  }
}
