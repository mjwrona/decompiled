// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.PartitionMapper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition
{
  internal static class PartitionMapper
  {
    public static TryCatch<PartitionMapper.PartitionMapping<PartitionedToken>> MonadicGetPartitionMapping<PartitionedToken>(
      IReadOnlyList<FeedRangeEpk> feedRanges,
      IReadOnlyList<PartitionedToken> tokens)
      where PartitionedToken : IPartitionedToken
    {
      if (feedRanges == null)
        throw new ArgumentNullException(nameof (feedRanges));
      if (tokens == null)
        throw new ArgumentNullException(nameof (tokens));
      if (feedRanges.Count < 1)
        throw new ArgumentException(nameof (feedRanges));
      List<(FeedRangeEpk, PartitionedToken)> splitRangesAndTokens = tokens.Count >= 1 ? PartitionMapper.SplitRangesBasedOffContinuationToken<PartitionedToken>((IReadOnlyList<FeedRangeEpk>) PartitionMapper.MergeRangesWherePossible(feedRanges), tokens) : throw new ArgumentException(nameof (feedRanges));
      FeedRangeEpk targetFeedRange = PartitionMapper.GetTargetFeedRange<PartitionedToken>(tokens);
      IReadOnlyList<PartitionedToken> tokens1 = tokens;
      FeedRangeEpk targetRange = targetFeedRange;
      return PartitionMapper.MonadicConstructPartitionMapping<PartitionedToken>((IReadOnlyList<(FeedRangeEpk, PartitionedToken)>) splitRangesAndTokens, tokens1, targetRange);
    }

    public static TryCatch<PartitionMapper.PartitionMapping<PartitionedToken>> MonadicGetPartitionMapping<PartitionedToken>(
      FeedRangeEpk feedRange,
      PartitionedToken token)
      where PartitionedToken : IPartitionedToken
    {
      if (feedRange == null)
        throw new ArgumentNullException(nameof (feedRange));
      return (object) token != null ? PartitionMapper.MonadicGetPartitionMapping<PartitionedToken>((IReadOnlyList<FeedRangeEpk>) new List<FeedRangeEpk>()
      {
        feedRange
      }, (IReadOnlyList<PartitionedToken>) new List<PartitionedToken>()
      {
        token
      }) : throw new ArgumentNullException(nameof (token));
    }

    private static List<FeedRangeEpk> MergeRangesWherePossible(
      IReadOnlyList<FeedRangeEpk> feedRanges)
    {
      Stack<(string, string)> source = new Stack<(string, string)>(feedRanges.Count);
      foreach (FeedRangeEpk feedRangeEpk in (IEnumerable<FeedRangeEpk>) feedRanges.OrderBy<FeedRangeEpk, string>((Func<FeedRangeEpk, string>) (feedRange => feedRange.Range.Min)))
      {
        if (source.Count == 0)
        {
          source.Push((feedRangeEpk.Range.Min, feedRangeEpk.Range.Max));
        }
        else
        {
          (string str1, string str2) = source.Pop();
          if (str2 == feedRangeEpk.Range.Min)
          {
            source.Push((str1, feedRangeEpk.Range.Max));
          }
          else
          {
            source.Push((str1, str2));
            source.Push((feedRangeEpk.Range.Min, feedRangeEpk.Range.Max));
          }
        }
      }
      return source.Select<(string, string), FeedRangeEpk>((Func<(string, string), FeedRangeEpk>) (range => new FeedRangeEpk(new Range<string>(range.min, range.max, true, false)))).ToList<FeedRangeEpk>();
    }

    private static List<(FeedRangeEpk, PartitionedToken)> SplitRangesBasedOffContinuationToken<PartitionedToken>(
      IReadOnlyList<FeedRangeEpk> feedRanges,
      IReadOnlyList<PartitionedToken> tokens)
      where PartitionedToken : IPartitionedToken
    {
      HashSet<FeedRangeEpk> source = new HashSet<FeedRangeEpk>((IEnumerable<FeedRangeEpk>) feedRanges);
      List<(FeedRangeEpk, PartitionedToken)> valueTupleList = new List<(FeedRangeEpk, PartitionedToken)>();
      foreach (PartitionedToken token in (IEnumerable<PartitionedToken>) tokens)
      {
        PartitionedToken partitionedToken = token;
        List<FeedRangeEpk> list = source.Where<FeedRangeEpk>((Func<FeedRangeEpk, bool>) (feedRange => ((feedRange.Range.Min == string.Empty ? 1 : (!(partitionedToken.Range.Min != string.Empty) ? 0 : (partitionedToken.Range.Min.CompareTo(feedRange.Range.Min) >= 0 ? 1 : 0))) & (feedRange.Range.Max == string.Empty ? (true ? 1 : 0) : (!(partitionedToken.Range.Max != string.Empty) ? (false ? 1 : 0) : (partitionedToken.Range.Max.CompareTo(feedRange.Range.Max) <= 0 ? 1 : 0)))) != 0)).ToList<FeedRangeEpk>();
        if (list.Count != 0)
        {
          FeedRangeEpk feedRangeEpk1 = list.Count == 1 ? list.First<FeedRangeEpk>() : throw new InvalidOperationException("Token was overlapped by multiple ranges.");
          source.Remove(feedRangeEpk1);
          if (feedRangeEpk1.Range.Min != partitionedToken.Range.Min)
          {
            FeedRangeEpk feedRangeEpk2 = new FeedRangeEpk(new Range<string>(feedRangeEpk1.Range.Min, partitionedToken.Range.Min, true, false));
            source.Add(feedRangeEpk2);
          }
          FeedRangeEpk feedRangeEpk3 = new FeedRangeEpk(new Range<string>(partitionedToken.Range.Min, partitionedToken.Range.Max, true, false));
          valueTupleList.Add((feedRangeEpk3, partitionedToken));
          if (partitionedToken.Range.Max != feedRangeEpk1.Range.Max)
          {
            FeedRangeEpk feedRangeEpk4 = new FeedRangeEpk(new Range<string>(partitionedToken.Range.Max, feedRangeEpk1.Range.Max, true, false));
            source.Add(feedRangeEpk4);
          }
        }
      }
      foreach (FeedRangeEpk feedRangeEpk in source)
        valueTupleList.Add((feedRangeEpk, default (PartitionedToken)));
      return valueTupleList;
    }

    private static FeedRangeEpk GetTargetFeedRange<PartitionedToken>(
      IReadOnlyList<PartitionedToken> tokens)
      where PartitionedToken : IPartitionedToken
    {
      PartitionedToken partitionedToken1 = tokens.OrderBy<PartitionedToken, string>((Func<PartitionedToken, string>) (partitionedToken => partitionedToken.Range.Min)).First<PartitionedToken>();
      return new FeedRangeEpk(new Range<string>(partitionedToken1.Range.Min, partitionedToken1.Range.Max, true, false));
    }

    private static TryCatch<PartitionMapper.PartitionMapping<PartitionedToken>> MonadicConstructPartitionMapping<PartitionedToken>(
      IReadOnlyList<(FeedRangeEpk, PartitionedToken)> splitRangesAndTokens,
      IReadOnlyList<PartitionedToken> tokens,
      FeedRangeEpk targetRange)
      where PartitionedToken : IPartitionedToken
    {
      ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)> readOnlyMemory1 = (ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)>) splitRangesAndTokens.OrderBy<(FeedRangeEpk, PartitionedToken), string>((Func<(FeedRangeEpk, PartitionedToken), string>) (rangeAndToken => rangeAndToken.Item1.Range.Min)).ToArray<(FeedRangeEpk, PartitionedToken)>();
      int? nullable = new int?();
      for (int index = 0; index < readOnlyMemory1.Length && !nullable.HasValue; ++index)
      {
        if (readOnlyMemory1.Span[index].Item1.Equals(targetRange))
          nullable = new int?(index);
      }
      if (!nullable.HasValue)
      {
        if (splitRangesAndTokens.Count != 1)
          return TryCatch<PartitionMapper.PartitionMapping<PartitionedToken>>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} - Could not find continuation token for range: '{1}'", (object) RMResources.InvalidContinuationToken, (object) targetRange)));
        readOnlyMemory1 = (ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)>) new (FeedRangeEpk, PartitionedToken)[1]
        {
          (readOnlyMemory1.Span[0].Item1, tokens[0])
        };
        nullable = new int?(0);
      }
      ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)> readOnlyMemory2 = nullable.Value != 0 ? readOnlyMemory1.Slice(0, nullable.Value) : ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)>.Empty;
      ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)> readOnlyMemory3 = readOnlyMemory1.Slice(nullable.Value, 1);
      ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)> readOnlyMemory4 = nullable.Value != readOnlyMemory1.Length - 1 ? readOnlyMemory1.Slice(nullable.Value + 1) : ReadOnlyMemory<(FeedRangeEpk, PartitionedToken)>.Empty;
      Dictionary<FeedRangeEpk, PartitionedToken> mappingFromTuples1 = CreateMappingFromTuples(readOnlyMemory2.Span);
      IReadOnlyDictionary<FeedRangeEpk, PartitionedToken> mappingFromTuples2 = (IReadOnlyDictionary<FeedRangeEpk, PartitionedToken>) CreateMappingFromTuples(readOnlyMemory3.Span);
      IReadOnlyDictionary<FeedRangeEpk, PartitionedToken> mappingFromTuples3 = (IReadOnlyDictionary<FeedRangeEpk, PartitionedToken>) CreateMappingFromTuples(readOnlyMemory4.Span);
      IReadOnlyDictionary<FeedRangeEpk, PartitionedToken> targetMapping = mappingFromTuples2;
      IReadOnlyDictionary<FeedRangeEpk, PartitionedToken> mappingRightOfTarget = mappingFromTuples3;
      return TryCatch<PartitionMapper.PartitionMapping<PartitionedToken>>.FromResult(new PartitionMapper.PartitionMapping<PartitionedToken>((IReadOnlyDictionary<FeedRangeEpk, PartitionedToken>) mappingFromTuples1, targetMapping, mappingRightOfTarget));

      static Dictionary<FeedRangeEpk, PartitionedToken> CreateMappingFromTuples(
        ReadOnlySpan<(FeedRangeEpk, PartitionedToken)> rangeAndTokens)
      {
        Dictionary<FeedRangeEpk, PartitionedToken> mappingFromTuples = new Dictionary<FeedRangeEpk, PartitionedToken>();
        ReadOnlySpan<(FeedRangeEpk, PartitionedToken)> readOnlySpan = rangeAndTokens;
        for (int index = 0; index < readOnlySpan.Length; ++index)
        {
          (FeedRangeEpk key, PartitionedToken partitionedToken) = readOnlySpan[index];
          mappingFromTuples[key] = partitionedToken;
        }
        return mappingFromTuples;
      }
    }

    public readonly struct PartitionMapping<T>
    {
      public PartitionMapping(
        IReadOnlyDictionary<FeedRangeEpk, T> mappingLeftOfTarget,
        IReadOnlyDictionary<FeedRangeEpk, T> targetMapping,
        IReadOnlyDictionary<FeedRangeEpk, T> mappingRightOfTarget)
      {
        this.MappingLeftOfTarget = mappingLeftOfTarget ?? throw new ArgumentNullException(nameof (mappingLeftOfTarget));
        this.TargetMapping = targetMapping ?? throw new ArgumentNullException(nameof (targetMapping));
        this.MappingRightOfTarget = mappingRightOfTarget ?? throw new ArgumentNullException(nameof (mappingRightOfTarget));
      }

      public IReadOnlyDictionary<FeedRangeEpk, T> MappingLeftOfTarget { get; }

      public IReadOnlyDictionary<FeedRangeEpk, T> TargetMapping { get; }

      public IReadOnlyDictionary<FeedRangeEpk, T> MappingRightOfTarget { get; }
    }
  }
}
