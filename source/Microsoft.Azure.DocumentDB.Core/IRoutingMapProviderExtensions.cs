// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IRoutingMapProviderExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class IRoutingMapProviderExtensions
  {
    private static string Max(string left, string right) => StringComparer.Ordinal.Compare(left, right) >= 0 ? left : right;

    public static async Task<PartitionKeyRange> TryGetRangeByEffectivePartitionKey(
      this IRoutingMapProvider routingMapProvider,
      string collectionResourceId,
      string effectivePartitionKey)
    {
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await routingMapProvider.TryGetOverlappingRangesAsync(collectionResourceId, Range<string>.GetPointRange(effectivePartitionKey));
      return overlappingRangesAsync != null ? overlappingRangesAsync.Single<PartitionKeyRange>() : (PartitionKeyRange) null;
    }

    private static bool IsNonOverlapping<T>(SortedSet<Range<T>> sortedSet) where T : IComparable<T>
    {
      IComparer<T> comparer = (object) typeof (T) == (object) typeof (string) ? (IComparer<T>) StringComparer.Ordinal : (IComparer<T>) Comparer<T>.Default;
      foreach (Tuple<Range<T>, Range<T>> tuple in sortedSet.Take<Range<T>>(sortedSet.Count - 1).Zip<Range<T>, Range<T>, Tuple<Range<T>, Range<T>>>(sortedSet.Skip<Range<T>>(1), (Func<Range<T>, Range<T>, Tuple<Range<T>, Range<T>>>) ((current, next) => new Tuple<Range<T>, Range<T>>(current, next))))
      {
        Range<T> range1 = tuple.Item1;
        Range<T> range2 = tuple.Item2;
        int num = comparer.Compare(range1.Max, range2.Min);
        if (num > 0 || num == 0 && range1.IsMaxInclusive && range2.IsMinInclusive)
          return false;
      }
      return true;
    }

    public static async Task<List<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      this IRoutingMapProvider routingMapProvider,
      string collectionResourceId,
      IEnumerable<Range<string>> sortedRanges,
      bool forceRefresh = false)
    {
      if (sortedRanges == null)
        throw new ArgumentNullException(nameof (sortedRanges));
      if (!IRoutingMapProviderExtensions.IsNonOverlapping<string>(new SortedSet<Range<string>>(sortedRanges, (IComparer<Range<string>>) Range<string>.MinComparer.Instance)))
        throw new ArgumentException("sortedRanges had overlaps.");
      List<PartitionKeyRange> targetRanges = new List<PartitionKeyRange>();
      foreach (Range<string> sortedRange in sortedRanges)
      {
        if (!sortedRange.IsEmpty && (targetRanges.Count == 0 || Range<string>.MaxComparer.Instance.Compare(sortedRange, targetRanges.Last<PartitionKeyRange>().ToRange()) > 0))
        {
          Range<string> range;
          if (targetRanges.Count == 0)
          {
            range = sortedRange;
          }
          else
          {
            string str = IRoutingMapProviderExtensions.Max(targetRanges.Last<PartitionKeyRange>().MaxExclusive, sortedRange.Min);
            bool isMinInclusive = string.CompareOrdinal(str, sortedRange.Min) == 0 && sortedRange.IsMinInclusive;
            range = new Range<string>(str, sortedRange.Max, isMinInclusive, sortedRange.IsMaxInclusive);
          }
          IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await routingMapProvider.TryGetOverlappingRangesAsync(collectionResourceId, range, forceRefresh);
          if (overlappingRangesAsync == null)
            return (List<PartitionKeyRange>) null;
          targetRanges.AddRange((IEnumerable<PartitionKeyRange>) overlappingRangesAsync);
        }
      }
      return targetRanges;
    }
  }
}
