// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionRoutingHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal static class PartitionRoutingHelper
  {
    public static IReadOnlyList<Range<string>> GetProvidedPartitionKeyRanges(
      SqlQuerySpec querySpec,
      bool enableCrossPartitionQuery,
      bool parallelizeCrossPartitionQuery,
      bool isContinuationExpected,
      bool hasLogicalPartitionKey,
      PartitionKeyDefinition partitionKeyDefinition,
      QueryPartitionProvider queryPartitionProvider,
      string clientApiVersion,
      out QueryInfo queryInfo)
    {
      if (querySpec == null)
        throw new ArgumentNullException(nameof (querySpec));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      if (queryPartitionProvider == null)
        throw new ArgumentNullException(nameof (queryPartitionProvider));
      Microsoft.Azure.Documents.Query.PartitionedQueryExecutionInfo queryExecutionInfo = queryPartitionProvider.GetPartitionedQueryExecutionInfo(querySpec, partitionKeyDefinition, VersionUtility.IsLaterThan(clientApiVersion, HttpConstants.Versions.v2016_11_14), isContinuationExpected, false, hasLogicalPartitionKey);
      if (queryExecutionInfo == null || queryExecutionInfo.QueryRanges == null || queryExecutionInfo.QueryInfo == null || queryExecutionInfo.QueryRanges.Any<Range<string>>((Func<Range<string>, bool>) (range => range.Min == null || range.Max == null)))
        DefaultTrace.TraceInformation("QueryPartitionProvider returned bad query info");
      bool flag = queryExecutionInfo.QueryRanges.Count == 1 && queryExecutionInfo.QueryRanges[0].IsSingleValue;
      if ((partitionKeyDefinition.Paths.Count <= 0 ? 0 : (!flag ? 1 : 0)) != 0)
      {
        if (!enableCrossPartitionQuery)
          throw new BadRequestException(RMResources.CrossPartitionQueryDisabled);
        if ((parallelizeCrossPartitionQuery || queryExecutionInfo.QueryInfo.HasTop || queryExecutionInfo.QueryInfo.HasOrderBy || queryExecutionInfo.QueryInfo.HasAggregates || queryExecutionInfo.QueryInfo.HasDistinct || queryExecutionInfo.QueryInfo.HasOffset || queryExecutionInfo.QueryInfo.HasLimit ? 1 : (queryExecutionInfo.QueryInfo.HasGroupBy ? 1 : 0)) != 0)
        {
          if (!PartitionRoutingHelper.IsSupportedPartitionedQueryExecutionInfo(queryExecutionInfo, clientApiVersion))
            throw new BadRequestException(RMResources.UnsupportedCrossPartitionQuery);
          if (queryExecutionInfo.QueryInfo.HasAggregates && !PartitionRoutingHelper.IsAggregateSupportedApiVersion(clientApiVersion))
            throw new BadRequestException(RMResources.UnsupportedCrossPartitionQueryWithAggregate);
          throw new DocumentClientException(RMResources.UnsupportedCrossPartitionQuery, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
          {
            Error = {
              AdditionalErrorInfo = JsonConvert.SerializeObject((object) queryExecutionInfo)
            }
          };
        }
      }
      else
      {
        if (queryExecutionInfo.QueryInfo.HasAggregates && !isContinuationExpected)
        {
          if (PartitionRoutingHelper.IsAggregateSupportedApiVersion(clientApiVersion))
            throw new DocumentClientException(RMResources.UnsupportedQueryWithFullResultAggregate, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
            {
              Error = {
                AdditionalErrorInfo = JsonConvert.SerializeObject((object) queryExecutionInfo)
              }
            };
          throw new BadRequestException(RMResources.UnsupportedQueryWithFullResultAggregate);
        }
        if (queryExecutionInfo.QueryInfo.HasDistinct)
          throw new DocumentClientException(RMResources.UnsupportedCrossPartitionQuery, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
          {
            Error = {
              AdditionalErrorInfo = JsonConvert.SerializeObject((object) queryExecutionInfo)
            }
          };
        if (queryExecutionInfo.QueryInfo.HasGroupBy)
          throw new DocumentClientException(RMResources.UnsupportedCrossPartitionQuery, HttpStatusCode.BadRequest, SubStatusCodes.Unknown);
      }
      queryInfo = queryExecutionInfo.QueryInfo;
      return (IReadOnlyList<Range<string>>) queryExecutionInfo.QueryRanges;
    }

    public static async Task<PartitionRoutingHelper.ResolvedRangeInfo> TryGetTargetRangeFromContinuationTokenRange(
      IReadOnlyList<Range<string>> providedPartitionKeyRanges,
      IRoutingMapProvider routingMapProvider,
      string collectionRid,
      Range<string> rangeFromContinuationToken,
      List<CompositeContinuationToken> suppliedTokens,
      RntbdConstants.RntdbEnumerationDirection direction = RntbdConstants.RntdbEnumerationDirection.Forward)
    {
      if (providedPartitionKeyRanges.Count == 0)
        return new PartitionRoutingHelper.ResolvedRangeInfo(await routingMapProvider.TryGetRangeByEffectivePartitionKey(collectionRid, PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey), suppliedTokens);
      if (rangeFromContinuationToken.IsEmpty)
      {
        if (direction != RntbdConstants.RntdbEnumerationDirection.Reverse)
          return new PartitionRoutingHelper.ResolvedRangeInfo(await routingMapProvider.TryGetRangeByEffectivePartitionKey(collectionRid, PartitionRoutingHelper.Min<Range<string>>(providedPartitionKeyRanges, (IComparer<Range<string>>) Range<string>.MinComparer.Instance).Min), suppliedTokens);
        IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, providedPartitionKeyRanges.Single<Range<string>>());
        return new PartitionRoutingHelper.ResolvedRangeInfo(overlappingRangesAsync[overlappingRangesAsync.Count - 1], suppliedTokens);
      }
      PartitionKeyRange effectivePartitionKey = await routingMapProvider.TryGetRangeByEffectivePartitionKey(collectionRid, rangeFromContinuationToken.Min);
      if (effectivePartitionKey == null)
        return new PartitionRoutingHelper.ResolvedRangeInfo((PartitionKeyRange) null, suppliedTokens);
      if (rangeFromContinuationToken.Equals(effectivePartitionKey.ToRange()))
        return new PartitionRoutingHelper.ResolvedRangeInfo(effectivePartitionKey, suppliedTokens);
      List<PartitionKeyRange> list = (await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, rangeFromContinuationToken, true)).ToList<PartitionKeyRange>();
      if (list == null || list.Count < 1)
        return new PartitionRoutingHelper.ResolvedRangeInfo((PartitionKeyRange) null, (List<CompositeContinuationToken>) null);
      if (!list[0].MinInclusive.Equals(rangeFromContinuationToken.Min) || !list[list.Count - 1].MaxExclusive.Equals(rangeFromContinuationToken.Max))
        return new PartitionRoutingHelper.ResolvedRangeInfo((PartitionKeyRange) null, (List<CompositeContinuationToken>) null);
      if (direction == RntbdConstants.RntdbEnumerationDirection.Reverse)
        list.Reverse();
      List<CompositeContinuationToken> continuationTokens = (List<CompositeContinuationToken>) null;
      if (suppliedTokens != null && suppliedTokens.Count > 0)
      {
        continuationTokens = new List<CompositeContinuationToken>(list.Count + suppliedTokens.Count - 1);
        foreach (PartitionKeyRange partitionKeyRange in list)
        {
          CompositeContinuationToken continuationToken = (CompositeContinuationToken) suppliedTokens[0].ShallowCopy();
          continuationToken.Range = partitionKeyRange.ToRange();
          continuationTokens.Add(continuationToken);
        }
        continuationTokens.AddRange(suppliedTokens.Skip<CompositeContinuationToken>(1));
      }
      return new PartitionRoutingHelper.ResolvedRangeInfo(list[0], continuationTokens);
    }

    public static async Task<List<PartitionKeyRange>> GetReplacementRanges(
      PartitionKeyRange targetRange,
      IRoutingMapProvider routingMapProvider,
      string collectionRid)
    {
      return (await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, targetRange.ToRange(), true)).ToList<PartitionKeyRange>();
    }

    public static async Task<bool> TryAddPartitionKeyRangeToContinuationTokenAsync(
      INameValueCollection backendResponseHeaders,
      IReadOnlyList<Range<string>> providedPartitionKeyRanges,
      IRoutingMapProvider routingMapProvider,
      string collectionRid,
      PartitionRoutingHelper.ResolvedRangeInfo resolvedRangeInfo,
      RntbdConstants.RntdbEnumerationDirection direction = RntbdConstants.RntdbEnumerationDirection.Forward)
    {
      PartitionKeyRange currentRange = resolvedRangeInfo.ResolvedRange;
      if (resolvedRangeInfo.ContinuationTokens != null && resolvedRangeInfo.ContinuationTokens.Count > 1)
      {
        if (!string.IsNullOrEmpty(backendResponseHeaders["x-ms-continuation"]))
          resolvedRangeInfo.ContinuationTokens[0].Token = backendResponseHeaders["x-ms-continuation"];
        else
          resolvedRangeInfo.ContinuationTokens.RemoveAt(0);
        backendResponseHeaders["x-ms-continuation"] = JsonConvert.SerializeObject((object) resolvedRangeInfo.ContinuationTokens);
      }
      else
      {
        PartitionKeyRange partitionKeyRange = currentRange;
        if (string.IsNullOrEmpty(backendResponseHeaders["x-ms-continuation"]))
        {
          if (direction == RntbdConstants.RntdbEnumerationDirection.Reverse)
          {
            partitionKeyRange = PartitionRoutingHelper.MinBefore((IReadOnlyList<PartitionKeyRange>) (await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, providedPartitionKeyRanges.Single<Range<string>>())).ToList<PartitionKeyRange>(), currentRange);
          }
          else
          {
            Range<string> range = PartitionRoutingHelper.MinAfter<Range<string>>(providedPartitionKeyRanges, currentRange.ToRange(), (IComparer<Range<string>>) Range<string>.MaxComparer.Instance);
            if (range == null)
              return true;
            string str = string.CompareOrdinal(range.Min, currentRange.MaxExclusive) > 0 ? range.Min : currentRange.MaxExclusive;
            if (string.CompareOrdinal(str, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey) == 0)
              return true;
            PartitionKeyRange effectivePartitionKey = await routingMapProvider.TryGetRangeByEffectivePartitionKey(collectionRid, str);
            if (effectivePartitionKey == null)
              return false;
            partitionKeyRange = effectivePartitionKey;
          }
        }
        if (partitionKeyRange != null)
          backendResponseHeaders["x-ms-continuation"] = PartitionRoutingHelper.AddPartitionKeyRangeToContinuationToken(backendResponseHeaders["x-ms-continuation"], partitionKeyRange);
      }
      return true;
    }

    public static Range<string> ExtractPartitionKeyRangeFromContinuationToken(
      INameValueCollection headers,
      out List<CompositeContinuationToken> compositeContinuationTokens)
    {
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      compositeContinuationTokens = (List<CompositeContinuationToken>) null;
      Range<string> continuationToken1 = Range<string>.GetEmptyRange(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey);
      if (string.IsNullOrEmpty(headers["x-ms-continuation"]))
        return continuationToken1;
      string header = headers["x-ms-continuation"];
      CompositeContinuationToken continuationToken2 = (CompositeContinuationToken) null;
      if (!string.IsNullOrEmpty(header))
      {
        try
        {
          if (header.Trim().StartsWith("[", StringComparison.Ordinal))
          {
            compositeContinuationTokens = JsonConvert.DeserializeObject<List<CompositeContinuationToken>>(header);
            if (compositeContinuationTokens != null && compositeContinuationTokens.Count > 0)
            {
              headers["x-ms-continuation"] = compositeContinuationTokens[0].Token;
              continuationToken2 = compositeContinuationTokens[0];
            }
            else
              headers.Remove("x-ms-continuation");
          }
          else
          {
            continuationToken2 = JsonConvert.DeserializeObject<CompositeContinuationToken>(header);
            compositeContinuationTokens = continuationToken2 != null ? new List<CompositeContinuationToken>()
            {
              continuationToken2
            } : throw new BadRequestException(RMResources.InvalidContinuationToken);
          }
          if (continuationToken2 != null && continuationToken2.Range != null)
            continuationToken1 = continuationToken2.Range;
          if (continuationToken2 != null && !string.IsNullOrEmpty(continuationToken2.Token))
            headers["x-ms-continuation"] = continuationToken2.Token;
          else
            headers.Remove("x-ms-continuation");
        }
        catch (JsonException ex)
        {
          DefaultTrace.TraceWarning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} Invalid JSON in the continuation token {1}", (object) DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) header));
          throw new BadRequestException(RMResources.InvalidContinuationToken, (Exception) ex);
        }
      }
      else
        headers.Remove("x-ms-continuation");
      return continuationToken1;
    }

    private static string AddPartitionKeyRangeToContinuationToken(
      string continuationToken,
      PartitionKeyRange partitionKeyRange)
    {
      return JsonConvert.SerializeObject((object) new CompositeContinuationToken()
      {
        Token = continuationToken,
        Range = partitionKeyRange.ToRange()
      });
    }

    private static bool IsSupportedPartitionedQueryExecutionInfo(
      Microsoft.Azure.Documents.Query.PartitionedQueryExecutionInfo partitionedQueryExecutionInfoueryInfo,
      string clientApiVersion)
    {
      return VersionUtility.IsLaterThan(clientApiVersion, HttpConstants.Versions.v2016_07_11) && partitionedQueryExecutionInfoueryInfo.Version <= 2;
    }

    private static bool IsAggregateSupportedApiVersion(string clientApiVersion) => VersionUtility.IsLaterThan(clientApiVersion, HttpConstants.Versions.v2016_11_14);

    private static T Min<T>(IReadOnlyList<T> values, IComparer<T> comparer)
    {
      T y = values.Count != 0 ? values[0] : throw new ArgumentException(nameof (values));
      for (int index = 1; index < values.Count; ++index)
      {
        if (comparer.Compare(values[index], y) < 0)
          y = values[index];
      }
      return y;
    }

    private static T MinAfter<T>(IReadOnlyList<T> values, T minValue, IComparer<T> comparer) where T : class
    {
      if (values.Count == 0)
        throw new ArgumentException(nameof (values));
      T y = default (T);
      foreach (T x in (IEnumerable<T>) values)
      {
        if (comparer.Compare(x, minValue) > 0 && ((object) y == null || comparer.Compare(x, y) < 0))
          y = x;
      }
      return y;
    }

    private static PartitionKeyRange MinBefore(
      IReadOnlyList<PartitionKeyRange> values,
      PartitionKeyRange minValue)
    {
      if (values.Count == 0)
        throw new ArgumentException(nameof (values));
      IComparer<Range<string>> instance = (IComparer<Range<string>>) Range<string>.MinComparer.Instance;
      PartitionKeyRange partitionKeyRange1 = (PartitionKeyRange) null;
      foreach (PartitionKeyRange partitionKeyRange2 in (IEnumerable<PartitionKeyRange>) values)
      {
        if (instance.Compare(partitionKeyRange2.ToRange(), minValue.ToRange()) < 0 && (partitionKeyRange1 == null || instance.Compare(partitionKeyRange2.ToRange(), partitionKeyRange1.ToRange()) > 0))
          partitionKeyRange1 = partitionKeyRange2;
      }
      return partitionKeyRange1;
    }

    public struct ResolvedRangeInfo
    {
      public readonly PartitionKeyRange ResolvedRange;
      public readonly List<CompositeContinuationToken> ContinuationTokens;

      public ResolvedRangeInfo(
        PartitionKeyRange range,
        List<CompositeContinuationToken> continuationTokens)
      {
        this.ResolvedRange = range;
        this.ContinuationTokens = continuationTokens;
      }
    }
  }
}
