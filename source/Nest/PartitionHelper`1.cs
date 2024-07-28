// Decompiled with JetBrains decompiler
// Type: Nest.PartitionHelper`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class PartitionHelper<TDocument> : IEnumerable<IList<TDocument>>, IEnumerable
  {
    private readonly IEnumerable<TDocument> _items;
    private readonly int _partitionSize;
    private bool _hasMoreItems;

    internal PartitionHelper(IEnumerable<TDocument> i, int ps)
    {
      this._items = i;
      this._partitionSize = ps;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<IList<TDocument>> GetEnumerator()
    {
      using (IEnumerator<TDocument> enumerator = this._items.GetEnumerator())
      {
        this._hasMoreItems = enumerator.MoveNext();
        while (this._hasMoreItems)
          yield return (IList<TDocument>) this.GetNextBatch(enumerator).ToList<TDocument>();
      }
    }

    private IEnumerable<TDocument> GetNextBatch(IEnumerator<TDocument> enumerator)
    {
      for (int i = 0; i < this._partitionSize; ++i)
      {
        yield return enumerator.Current;
        this._hasMoreItems = enumerator.MoveNext();
        if (!this._hasMoreItems)
          break;
      }
    }
  }
}
