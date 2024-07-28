// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.CollectionRoutingMap
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class CollectionRoutingMap
  {
    private readonly Dictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>> rangeById;
    private readonly List<PartitionKeyRange> orderedPartitionKeyRanges;
    private readonly List<Range<string>> orderedRanges;
    private readonly HashSet<string> goneRanges;
    private static readonly int InvalidPkRangeId = -1;

    internal int HighestNonOfflinePkRangeId { get; private set; }

    public CollectionRoutingMap(
      CollectionRoutingMap collectionRoutingMap,
      string changeFeedNextIfNoneMatch)
    {
      this.rangeById = new Dictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>>((IDictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>>) collectionRoutingMap.rangeById);
      this.orderedPartitionKeyRanges = new List<PartitionKeyRange>((IEnumerable<PartitionKeyRange>) collectionRoutingMap.orderedPartitionKeyRanges);
      this.orderedRanges = new List<Range<string>>((IEnumerable<Range<string>>) collectionRoutingMap.orderedRanges);
      this.goneRanges = new HashSet<string>((IEnumerable<string>) collectionRoutingMap.goneRanges);
      this.HighestNonOfflinePkRangeId = collectionRoutingMap.HighestNonOfflinePkRangeId;
      this.CollectionUniqueId = collectionRoutingMap.CollectionUniqueId;
      this.ChangeFeedNextIfNoneMatch = changeFeedNextIfNoneMatch;
    }

    private CollectionRoutingMap(
      Dictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>> rangeById,
      List<PartitionKeyRange> orderedPartitionKeyRanges,
      string collectionUniqueId,
      string changeFeedNextIfNoneMatch)
    {
      this.rangeById = rangeById;
      this.orderedPartitionKeyRanges = orderedPartitionKeyRanges;
      this.orderedRanges = orderedPartitionKeyRanges.Select<PartitionKeyRange, Range<string>>((Func<PartitionKeyRange, Range<string>>) (range => new Range<string>(range.MinInclusive, range.MaxExclusive, true, false))).ToList<Range<string>>();
      this.CollectionUniqueId = collectionUniqueId;
      this.ChangeFeedNextIfNoneMatch = changeFeedNextIfNoneMatch;
      this.goneRanges = new HashSet<string>(orderedPartitionKeyRanges.SelectMany<PartitionKeyRange, string>((Func<PartitionKeyRange, IEnumerable<string>>) (r => (IEnumerable<string>) r.Parents ?? Enumerable.Empty<string>())));
      this.HighestNonOfflinePkRangeId = orderedPartitionKeyRanges.Max<PartitionKeyRange>((Func<PartitionKeyRange, int>) (range =>
      {
        int result = CollectionRoutingMap.InvalidPkRangeId;
        if (!int.TryParse(range.Id, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        {
          DefaultTrace.TraceCritical("Could not parse partition key range Id as int {0} for collectionRid {1}", (object) range.Id, (object) this.CollectionUniqueId);
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not parse partition key range Id as int {0} for collectionRid {1}", (object) range.Id, (object) this.CollectionUniqueId));
        }
        return range.Status != PartitionKeyRangeStatus.Offline ? result : CollectionRoutingMap.InvalidPkRangeId;
      }));
    }

    public static CollectionRoutingMap TryCreateCompleteRoutingMap(
      IEnumerable<Tuple<PartitionKeyRange, ServiceIdentity>> ranges,
      string collectionUniqueId,
      string changeFeedNextIfNoneMatch = null)
    {
      Dictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>> rangeById = new Dictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (Tuple<PartitionKeyRange, ServiceIdentity> range in ranges)
        rangeById[range.Item1.Id] = range;
      List<Tuple<PartitionKeyRange, ServiceIdentity>> list1 = rangeById.Values.ToList<Tuple<PartitionKeyRange, ServiceIdentity>>();
      list1.Sort((IComparer<Tuple<PartitionKeyRange, ServiceIdentity>>) new CollectionRoutingMap.MinPartitionKeyTupleComparer());
      List<PartitionKeyRange> list2 = list1.Select<Tuple<PartitionKeyRange, ServiceIdentity>, PartitionKeyRange>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, PartitionKeyRange>) (range => range.Item1)).ToList<PartitionKeyRange>();
      return !CollectionRoutingMap.IsCompleteSetOfRanges((IList<PartitionKeyRange>) list2) ? (CollectionRoutingMap) null : new CollectionRoutingMap(rangeById, list2, collectionUniqueId, changeFeedNextIfNoneMatch);
    }

    public string CollectionUniqueId { get; private set; }

    public string ChangeFeedNextIfNoneMatch { get; private set; }

    public IReadOnlyList<PartitionKeyRange> OrderedPartitionKeyRanges => (IReadOnlyList<PartitionKeyRange>) this.orderedPartitionKeyRanges;

    public IReadOnlyList<PartitionKeyRange> GetOverlappingRanges(Range<string> range) => this.GetOverlappingRanges((IReadOnlyList<Range<string>>) new Range<string>[1]
    {
      range
    });

    public IReadOnlyList<PartitionKeyRange> GetOverlappingRanges(
      IReadOnlyList<Range<string>> providedPartitionKeyRanges)
    {
      if (providedPartitionKeyRanges == null)
        throw new ArgumentNullException(nameof (providedPartitionKeyRanges));
      SortedList<string, PartitionKeyRange> sortedList = new SortedList<string, PartitionKeyRange>();
      foreach (Range<string> partitionKeyRange in (IEnumerable<Range<string>>) providedPartitionKeyRanges)
      {
        int num1 = this.orderedRanges.BinarySearch(partitionKeyRange, (IComparer<Range<string>>) Range<string>.MinComparer.Instance);
        if (num1 < 0)
          num1 = Math.Max(0, ~num1 - 1);
        int num2 = this.orderedRanges.BinarySearch(partitionKeyRange, (IComparer<Range<string>>) Range<string>.MaxComparer.Instance);
        if (num2 < 0)
          num2 = Math.Min(this.OrderedPartitionKeyRanges.Count - 1, ~num2);
        for (int index = num1; index <= num2; ++index)
        {
          if (Range<string>.CheckOverlapping(this.orderedRanges[index], partitionKeyRange))
            sortedList[this.OrderedPartitionKeyRanges[index].MinInclusive] = this.OrderedPartitionKeyRanges[index];
        }
      }
      return (IReadOnlyList<PartitionKeyRange>) new ReadOnlyCollection<PartitionKeyRange>(sortedList.Values);
    }

    public PartitionKeyRange GetRangeByEffectivePartitionKey(string effectivePartitionKeyValue)
    {
      if (string.CompareOrdinal(effectivePartitionKeyValue, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey) >= 0)
        throw new ArgumentException(nameof (effectivePartitionKeyValue));
      if (string.CompareOrdinal(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, effectivePartitionKeyValue) == 0)
        return this.orderedPartitionKeyRanges[0];
      int index = this.orderedRanges.BinarySearch(new Range<string>(effectivePartitionKeyValue, effectivePartitionKeyValue, true, true), (IComparer<Range<string>>) Range<string>.MinComparer.Instance);
      if (index < 0)
        index = ~index - 1;
      return this.orderedPartitionKeyRanges[index];
    }

    public PartitionKeyRange TryGetRangeByPartitionKeyRangeId(string partitionKeyRangeId)
    {
      Tuple<PartitionKeyRange, ServiceIdentity> tuple;
      return this.rangeById.TryGetValue(partitionKeyRangeId, out tuple) ? tuple.Item1 : (PartitionKeyRange) null;
    }

    public ServiceIdentity TryGetInfoByPartitionKeyRangeId(string partitionKeyRangeId)
    {
      Tuple<PartitionKeyRange, ServiceIdentity> tuple;
      return this.rangeById.TryGetValue(partitionKeyRangeId, out tuple) ? tuple.Item2 : (ServiceIdentity) null;
    }

    public CollectionRoutingMap TryCombine(
      IEnumerable<Tuple<PartitionKeyRange, ServiceIdentity>> ranges,
      string changeFeedNextIfNoneMatch)
    {
      HashSet<string> newGoneRanges = new HashSet<string>(ranges.SelectMany<Tuple<PartitionKeyRange, ServiceIdentity>, string>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, IEnumerable<string>>) (tuple => (IEnumerable<string>) tuple.Item1.Parents ?? Enumerable.Empty<string>())));
      newGoneRanges.UnionWith((IEnumerable<string>) this.goneRanges);
      Dictionary<string, Tuple<PartitionKeyRange, ServiceIdentity>> dictionary = this.rangeById.Values.Where<Tuple<PartitionKeyRange, ServiceIdentity>>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, bool>) (tuple => !newGoneRanges.Contains(tuple.Item1.Id))).ToDictionary<Tuple<PartitionKeyRange, ServiceIdentity>, string>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, string>) (tuple => tuple.Item1.Id), (IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (Tuple<PartitionKeyRange, ServiceIdentity> tuple in ranges.Where<Tuple<PartitionKeyRange, ServiceIdentity>>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, bool>) (tuple => !newGoneRanges.Contains(tuple.Item1.Id))))
      {
        dictionary[tuple.Item1.Id] = tuple;
        DefaultTrace.TraceInformation("CollectionRoutingMap.TryCombine newRangeById[{0}] = {1}", (object) tuple.Item1.Id, (object) tuple);
      }
      List<Tuple<PartitionKeyRange, ServiceIdentity>> list1 = dictionary.Values.ToList<Tuple<PartitionKeyRange, ServiceIdentity>>();
      list1.Sort((IComparer<Tuple<PartitionKeyRange, ServiceIdentity>>) new CollectionRoutingMap.MinPartitionKeyTupleComparer());
      List<PartitionKeyRange> list2 = list1.Select<Tuple<PartitionKeyRange, ServiceIdentity>, PartitionKeyRange>((Func<Tuple<PartitionKeyRange, ServiceIdentity>, PartitionKeyRange>) (range => range.Item1)).ToList<PartitionKeyRange>();
      return !CollectionRoutingMap.IsCompleteSetOfRanges((IList<PartitionKeyRange>) list2) ? (CollectionRoutingMap) null : new CollectionRoutingMap(dictionary, list2, this.CollectionUniqueId, changeFeedNextIfNoneMatch);
    }

    private static bool IsCompleteSetOfRanges(IList<PartitionKeyRange> orderedRanges)
    {
      bool flag = false;
      if (orderedRanges.Count > 0)
      {
        PartitionKeyRange orderedRange1 = orderedRanges[0];
        PartitionKeyRange orderedRange2 = orderedRanges[orderedRanges.Count - 1];
        flag = string.CompareOrdinal(orderedRange1.MinInclusive, PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey) == 0 & string.CompareOrdinal(orderedRange2.MaxExclusive, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey) == 0;
        for (int index = 1; index < orderedRanges.Count; ++index)
        {
          PartitionKeyRange orderedRange3 = orderedRanges[index - 1];
          PartitionKeyRange orderedRange4 = orderedRanges[index];
          flag &= orderedRange3.MaxExclusive.Equals(orderedRange4.MinInclusive);
          if (!flag)
          {
            if (string.CompareOrdinal(orderedRange3.MaxExclusive, orderedRange4.MinInclusive) > 0)
              throw new InvalidOperationException("Ranges overlap");
            break;
          }
        }
      }
      return flag;
    }

    public bool IsGone(string partitionKeyRangeId) => this.goneRanges.Contains(partitionKeyRangeId);

    private class MinPartitionKeyTupleComparer : IComparer<Tuple<PartitionKeyRange, ServiceIdentity>>
    {
      public int Compare(
        Tuple<PartitionKeyRange, ServiceIdentity> left,
        Tuple<PartitionKeyRange, ServiceIdentity> right)
      {
        return string.CompareOrdinal(left.Item1.MinInclusive, right.Item1.MinInclusive);
      }
    }
  }
}
