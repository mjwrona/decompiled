// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Partitioning.RangePartitionResolver`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents.Partitioning
{
  [Obsolete("Support for IPartitionResolver based classes is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput.")]
  public class RangePartitionResolver<T> : IPartitionResolver where T : IComparable<T>, IEquatable<T>
  {
    public string PartitionKeyPropertyName { get; private set; }

    public IDictionary<Range<T>, string> PartitionMap { get; private set; }

    public Func<object, object> PartitionKeyExtractor { get; private set; }

    public RangePartitionResolver(
      string partitionKeyPropertyName,
      IDictionary<Range<T>, string> partitionMap)
    {
      if (string.IsNullOrEmpty(partitionKeyPropertyName))
        throw new ArgumentNullException(nameof (partitionKeyPropertyName));
      if (partitionMap == null)
        throw new ArgumentNullException(nameof (partitionMap));
      this.PartitionKeyPropertyName = partitionKeyPropertyName;
      this.PartitionMap = partitionMap;
    }

    public RangePartitionResolver(
      Func<object, object> partitionKeyExtractor,
      IDictionary<Range<T>, string> partitionMap)
    {
      if (partitionKeyExtractor == null)
        throw new ArgumentNullException(nameof (partitionKeyExtractor));
      if (partitionMap == null)
        throw new ArgumentNullException(nameof (partitionMap));
      this.PartitionKeyExtractor = partitionKeyExtractor;
      this.PartitionMap = partitionMap;
    }

    public virtual object GetPartitionKey(object document)
    {
      if (this.PartitionKeyPropertyName != null)
        return PartitionResolverUtils.ExtractPartitionKeyFromDocument(document, this.PartitionKeyPropertyName);
      if (this.PartitionKeyExtractor != null)
        return this.PartitionKeyExtractor(document);
      throw new InvalidOperationException(ClientResources.PartitionKeyExtractError);
    }

    public virtual string ResolveForCreate(object partitionKey)
    {
      Range<T> inRange = new Range<T>((T) Convert.ChangeType(partitionKey, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture));
      Range<T> containingRange = (Range<T>) null;
      if (this.TryGetContainingRange(inRange, out containingRange))
        return this.PartitionMap[containingRange];
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ClientResources.RangeNotFoundError, (object) inRange.ToString()));
    }

    public virtual IEnumerable<string> ResolveForRead(object partitionKey)
    {
      ICollection<Range<T>> ranges = partitionKey != null ? (ICollection<Range<T>>) this.GetIntersectingRanges(this.ProcessPartitionKey(partitionKey)) : this.PartitionMap.Keys;
      List<string> stringList = new List<string>();
      foreach (Range<T> key in (IEnumerable<Range<T>>) ranges)
        stringList.Add(this.PartitionMap[key]);
      return (IEnumerable<string>) stringList;
    }

    private IEnumerable<Range<T>> ProcessPartitionKey(object partitionKey)
    {
      List<Range<T>> rangeList = new List<Range<T>>();
      switch (partitionKey)
      {
        case null:
          return (IEnumerable<Range<T>>) null;
        case Range<T> _:
          rangeList.Add((Range<T>) partitionKey);
          break;
        case IEnumerable<T> _:
          using (IEnumerator<T> enumerator = ((IEnumerable<T>) partitionKey).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              T current = enumerator.Current;
              rangeList.Add(new Range<T>(current));
            }
            break;
          }
        case IEnumerable<Range<T>> _:
          rangeList.AddRange((IEnumerable<Range<T>>) partitionKey);
          break;
        default:
          try
          {
            T point = (T) Convert.ChangeType(partitionKey, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
            rangeList.Add(new Range<T>(point));
            break;
          }
          catch (Exception ex)
          {
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ClientResources.UnsupportedPartitionKey, (object) partitionKey.GetType()), ex);
          }
      }
      return (IEnumerable<Range<T>>) rangeList;
    }

    private List<Range<T>> GetIntersectingRanges(IEnumerable<Range<T>> inRanges)
    {
      List<Range<T>> intersectingRanges = new List<Range<T>>();
      ICollection<Range<T>> keys = this.PartitionMap.Keys;
      foreach (Range<T> inRange in inRanges)
      {
        foreach (Range<T> range in (IEnumerable<Range<T>>) keys)
        {
          if (range.Intersect(inRange))
            intersectingRanges.Add(range);
        }
      }
      return intersectingRanges;
    }

    private bool TryGetContainingRange(Range<T> inRange, out Range<T> containingRange)
    {
      foreach (Range<T> key in (IEnumerable<Range<T>>) this.PartitionMap.Keys)
      {
        if (key.Contains(inRange))
        {
          containingRange = key;
          return true;
        }
      }
      containingRange = (Range<T>) null;
      return false;
    }
  }
}
