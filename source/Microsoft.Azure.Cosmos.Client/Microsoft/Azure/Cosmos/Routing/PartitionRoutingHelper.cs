// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionRoutingHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal class PartitionRoutingHelper
  {
    public static IReadOnlyList<Range<string>> GetProvidedPartitionKeyRanges(
      string querySpecJsonString,
      bool enableCrossPartitionQuery,
      bool parallelizeCrossPartitionQuery,
      bool isContinuationExpected,
      bool hasLogicalPartitionKey,
      bool allowDCount,
      bool allowNonValueAggregates,
      bool useSystemPrefix,
      PartitionKeyDefinition partitionKeyDefinition,
      QueryPartitionProvider queryPartitionProvider,
      string clientApiVersion,
      out QueryInfo queryInfo)
    {
      if (querySpecJsonString == null)
        throw new ArgumentNullException(nameof (querySpecJsonString));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      if (queryPartitionProvider == null)
        throw new ArgumentNullException(nameof (queryPartitionProvider));
      TryCatch<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> queryExecutionInfo = queryPartitionProvider.TryGetPartitionedQueryExecutionInfo(querySpecJsonString, partitionKeyDefinition, VersionUtility.IsLaterThan(clientApiVersion, HttpConstants.VersionDates.v2016_11_14), isContinuationExpected, allowNonValueAggregates, hasLogicalPartitionKey, allowDCount, useSystemPrefix);
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfoueryInfo = queryExecutionInfo.Succeeded ? queryExecutionInfo.Result : throw new BadRequestException(queryExecutionInfo.Exception);
      if (partitionedQueryExecutionInfoueryInfo?.QueryRanges == null || partitionedQueryExecutionInfoueryInfo.QueryInfo == null || partitionedQueryExecutionInfoueryInfo.QueryRanges.Any<Range<string>>((Func<Range<string>, bool>) (range => range.Min == null || range.Max == null)))
        DefaultTrace.TraceInformation("QueryPartitionProvider returned bad query info");
      bool flag = partitionedQueryExecutionInfoueryInfo.QueryRanges.Count == 1 && partitionedQueryExecutionInfoueryInfo.QueryRanges[0].IsSingleValue;
      if ((partitionKeyDefinition.Paths.Count <= 0 ? 0 : (!flag ? 1 : 0)) != 0)
      {
        if (!enableCrossPartitionQuery)
        {
          BadRequestException requestException = new BadRequestException(RMResources.CrossPartitionQueryDisabled);
          requestException.Error.AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo);
          throw requestException;
        }
        if ((parallelizeCrossPartitionQuery || partitionedQueryExecutionInfoueryInfo.QueryInfo.HasTop || partitionedQueryExecutionInfoueryInfo.QueryInfo.HasOrderBy || partitionedQueryExecutionInfoueryInfo.QueryInfo.HasAggregates || partitionedQueryExecutionInfoueryInfo.QueryInfo.HasDistinct || partitionedQueryExecutionInfoueryInfo.QueryInfo.HasOffset || partitionedQueryExecutionInfoueryInfo.QueryInfo.HasLimit ? 1 : (partitionedQueryExecutionInfoueryInfo.QueryInfo.HasGroupBy ? 1 : 0)) != 0)
        {
          if (!PartitionRoutingHelper.IsSupportedPartitionedQueryExecutionInfo(partitionedQueryExecutionInfoueryInfo, clientApiVersion))
          {
            BadRequestException requestException = new BadRequestException(RMResources.UnsupportedCrossPartitionQuery);
            requestException.Error.AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo);
            throw requestException;
          }
          if (partitionedQueryExecutionInfoueryInfo.QueryInfo.HasAggregates && !PartitionRoutingHelper.IsAggregateSupportedApiVersion(clientApiVersion))
          {
            BadRequestException requestException = new BadRequestException(RMResources.UnsupportedCrossPartitionQueryWithAggregate);
            requestException.Error.AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo);
            throw requestException;
          }
          throw new DocumentClientException(RMResources.UnsupportedCrossPartitionQuery, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
          {
            Error = {
              AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo)
            }
          };
        }
      }
      else
      {
        if (partitionedQueryExecutionInfoueryInfo.QueryInfo.HasAggregates && !isContinuationExpected)
        {
          if (PartitionRoutingHelper.IsAggregateSupportedApiVersion(clientApiVersion))
            throw new DocumentClientException(RMResources.UnsupportedQueryWithFullResultAggregate, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
            {
              Error = {
                AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo)
              }
            };
          throw new BadRequestException(RMResources.UnsupportedQueryWithFullResultAggregate);
        }
        if (partitionedQueryExecutionInfoueryInfo.QueryInfo.HasDistinct)
          throw new DocumentClientException(RMResources.UnsupportedCrossPartitionQuery, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
          {
            Error = {
              AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo)
            }
          };
        if (partitionedQueryExecutionInfoueryInfo.QueryInfo.HasGroupBy)
          throw new DocumentClientException(RMResources.UnsupportedCrossPartitionQuery, HttpStatusCode.BadRequest, SubStatusCodes.CrossPartitionQueryNotServable)
          {
            Error = {
              AdditionalErrorInfo = JsonConvert.SerializeObject((object) partitionedQueryExecutionInfoueryInfo)
            }
          };
      }
      queryInfo = partitionedQueryExecutionInfoueryInfo.QueryInfo;
      return (IReadOnlyList<Range<string>>) partitionedQueryExecutionInfoueryInfo.QueryRanges;
    }

    public virtual async Task<PartitionRoutingHelper.ResolvedRangeInfo> TryGetTargetRangeFromContinuationTokenRangeAsync(
      IReadOnlyList<Range<string>> providedPartitionKeyRanges,
      IRoutingMapProvider routingMapProvider,
      string collectionRid,
      Range<string> rangeFromContinuationToken,
      List<CompositeContinuationToken> suppliedTokens,
      ITrace trace,
      RntbdConstants.RntdbEnumerationDirection direction = RntbdConstants.RntdbEnumerationDirection.Forward)
    {
      if (providedPartitionKeyRanges.Count == 0)
        return new PartitionRoutingHelper.ResolvedRangeInfo(await routingMapProvider.TryGetRangeByEffectivePartitionKeyAsync(collectionRid, PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, trace), suppliedTokens);
      if (rangeFromContinuationToken.IsEmpty)
      {
        if (direction != RntbdConstants.RntdbEnumerationDirection.Reverse)
          return new PartitionRoutingHelper.ResolvedRangeInfo(await routingMapProvider.TryGetRangeByEffectivePartitionKeyAsync(collectionRid, PartitionRoutingHelper.Min<Range<string>>(providedPartitionKeyRanges, (IComparer<Range<string>>) Range<string>.MinComparer.Instance).Min, trace), suppliedTokens);
        IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, providedPartitionKeyRanges.Single<Range<string>>(), trace);
        return new PartitionRoutingHelper.ResolvedRangeInfo(overlappingRangesAsync[overlappingRangesAsync.Count - 1], suppliedTokens);
      }
      PartitionKeyRange partitionKeyAsync = await routingMapProvider.TryGetRangeByEffectivePartitionKeyAsync(collectionRid, rangeFromContinuationToken.Min, trace);
      if (partitionKeyAsync == null)
        return new PartitionRoutingHelper.ResolvedRangeInfo((PartitionKeyRange) null, suppliedTokens);
      if (rangeFromContinuationToken.Equals(partitionKeyAsync.ToRange()))
        return new PartitionRoutingHelper.ResolvedRangeInfo(partitionKeyAsync, suppliedTokens);
      IReadOnlyList<PartitionKeyRange> source = await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, rangeFromContinuationToken, trace, true);
      if (source == null || source.Count < 1)
        return new PartitionRoutingHelper.ResolvedRangeInfo((PartitionKeyRange) null, (List<CompositeContinuationToken>) null);
      if (!source[0].MinInclusive.Equals(rangeFromContinuationToken.Min) || !source[source.Count - 1].MaxExclusive.Equals(rangeFromContinuationToken.Max))
        return new PartitionRoutingHelper.ResolvedRangeInfo((PartitionKeyRange) null, (List<CompositeContinuationToken>) null);
      if (direction == RntbdConstants.RntdbEnumerationDirection.Reverse)
        source = (IReadOnlyList<PartitionKeyRange>) new ReadOnlyCollection<PartitionKeyRange>((IList<PartitionKeyRange>) source.Reverse<PartitionKeyRange>().ToList<PartitionKeyRange>());
      List<CompositeContinuationToken> continuationTokens = (List<CompositeContinuationToken>) null;
      if (suppliedTokens != null && suppliedTokens.Count > 0)
      {
        continuationTokens = new List<CompositeContinuationToken>(source.Count + suppliedTokens.Count - 1);
        foreach (PartitionKeyRange partitionKeyRange in (IEnumerable<PartitionKeyRange>) source)
        {
          CompositeContinuationToken continuationToken = (CompositeContinuationToken) suppliedTokens[0].ShallowCopy();
          continuationToken.Range = partitionKeyRange.ToRange();
          continuationTokens.Add(continuationToken);
        }
        continuationTokens.AddRange(suppliedTokens.Skip<CompositeContinuationToken>(1));
      }
      return new PartitionRoutingHelper.ResolvedRangeInfo(source[0], continuationTokens);
    }

    public static async Task<List<PartitionKeyRange>> GetReplacementRangesAsync(
      PartitionKeyRange targetRange,
      IRoutingMapProvider routingMapProvider,
      string collectionRid,
      ITrace trace)
    {
      return (await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, targetRange.ToRange(), trace, true)).ToList<PartitionKeyRange>();
    }

    public virtual async Task<bool> TryAddPartitionKeyRangeToContinuationTokenAsync(
      INameValueCollection backendResponseHeaders,
      IReadOnlyList<Range<string>> providedPartitionKeyRanges,
      IRoutingMapProvider routingMapProvider,
      string collectionRid,
      PartitionRoutingHelper.ResolvedRangeInfo resolvedRangeInfo,
      ITrace trace,
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
            partitionKeyRange = PartitionRoutingHelper.MinBefore((IReadOnlyList<PartitionKeyRange>) (await routingMapProvider.TryGetOverlappingRangesAsync(collectionRid, providedPartitionKeyRanges.Single<Range<string>>(), trace)).ToList<PartitionKeyRange>(), currentRange);
          }
          else
          {
            Range<string> range = PartitionRoutingHelper.MinAfter<Range<string>>(providedPartitionKeyRanges, currentRange.ToRange(), (IComparer<Range<string>>) Range<string>.MaxComparer.Instance);
            if (range == null)
              return true;
            string str = string.CompareOrdinal(range.Min, currentRange.MaxExclusive) > 0 ? range.Min : currentRange.MaxExclusive;
            if (string.CompareOrdinal(str, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey) == 0)
              return true;
            PartitionKeyRange partitionKeyAsync = await routingMapProvider.TryGetRangeByEffectivePartitionKeyAsync(collectionRid, str, trace);
            if (partitionKeyAsync == null)
              return false;
            partitionKeyRange = partitionKeyAsync;
          }
        }
        if (partitionKeyRange != null)
          backendResponseHeaders["x-ms-continuation"] = PartitionRoutingHelper.AddPartitionKeyRangeToContinuationToken(backendResponseHeaders["x-ms-continuation"], partitionKeyRange);
      }
      return true;
    }

    public virtual Range<string> ExtractPartitionKeyRangeFromContinuationToken(
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
          if (continuationToken2?.Range != null)
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
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfoueryInfo,
      string clientApiVersion)
    {
      return VersionUtility.IsLaterThan(clientApiVersion, HttpConstants.VersionDates.v2016_07_11) && partitionedQueryExecutionInfoueryInfo.Version <= 2;
    }

    private static bool IsAggregateSupportedApiVersion(string clientApiVersion) => VersionUtility.IsLaterThan(clientApiVersion, HttpConstants.VersionDates.v2016_11_14);

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

    public readonly struct ResolvedRangeInfo
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
