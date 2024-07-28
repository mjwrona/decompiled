// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Utils
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class Utils
  {
    private const char TokenSeparator = ';';
    private const string ContinuationTokenHeaderName = "x-ms-continuationtoken";

    public static void SetParametersForPointsPaging(
      int skip,
      int top,
      string continuationToken,
      out int skipRows,
      out int topToFetch,
      out int watermark,
      out int topRemaining)
    {
      skipRows = skip;
      watermark = int.MinValue;
      topRemaining = -1;
      if (!string.IsNullOrEmpty(continuationToken))
      {
        WatermarkTopPair continuationToken1 = Utils.ParseTestPointWatermarkTopPairFromContinuationToken(continuationToken);
        watermark = continuationToken1.Watermark;
        top = continuationToken1.Top;
        skipRows = 0;
      }
      topToFetch = 200;
      if (top > 5000)
        throw new ArgumentOutOfRangeException("maxPageSize", (object) 5000, "continuationToken is out of supported range");
      if (top > 0)
      {
        topToFetch = top;
        topRemaining = -1;
      }
      ++topToFetch;
    }

    public static void SetParametersForPaging(
      int skip,
      int top,
      string continuationToken,
      out int skipRows,
      out int topToFetch,
      out int watermark,
      out int topRemaining)
    {
      skipRows = skip;
      watermark = 0;
      topRemaining = -1;
      if (!string.IsNullOrEmpty(continuationToken))
      {
        WatermarkTopPair continuationToken1 = Utils.ParseWatermarkTopPairFromContinuationToken(continuationToken);
        watermark = continuationToken1.Watermark;
        top = continuationToken1.Top;
        skipRows = 0;
      }
      topToFetch = 200;
      if (top > 5000)
        throw new ArgumentOutOfRangeException("maxPageSize", (object) 5000, "continuationToken is out of supported range");
      if (top > 0)
      {
        topToFetch = top;
        topRemaining = -1;
      }
      ++topToFetch;
    }

    public static string GenerateContinuationToken(int watermark, int topRemaining)
    {
      string continuationToken = (string) null;
      WatermarkTopPair watermarkTopPair = new WatermarkTopPair();
      watermarkTopPair.Watermark = watermark;
      watermarkTopPair.Top = -1;
      if (topRemaining > 0)
        watermarkTopPair.Top = topRemaining;
      else if (topRemaining == -1)
        watermarkTopPair.Top = 0;
      if (watermarkTopPair.Top != -1)
        continuationToken = Utils.GetContinuationTokenFromWatermarkTopPair(watermarkTopPair);
      return continuationToken;
    }

    public static WatermarkTopPair ParseTestPointWatermarkTopPairFromContinuationToken(
      string continuationToken)
    {
      WatermarkTopPair continuationToken1 = new WatermarkTopPair();
      if (!string.IsNullOrEmpty(continuationToken))
      {
        string[] strArray = continuationToken.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1 || strArray.Length == 2)
        {
          int result;
          if (!int.TryParse(strArray[0], out result))
            throw new ArgumentOutOfRangeException(nameof (continuationToken));
          continuationToken1.Watermark = result;
          if (strArray.Length == 2)
          {
            if (!int.TryParse(strArray[1], out result))
              throw new ArgumentOutOfRangeException(nameof (continuationToken));
            continuationToken1.Top = result;
          }
        }
      }
      return continuationToken1;
    }

    public static WatermarkTopPair ParseWatermarkTopPairFromContinuationToken(
      string continuationToken)
    {
      WatermarkTopPair continuationToken1 = new WatermarkTopPair();
      if (!string.IsNullOrEmpty(continuationToken))
      {
        string[] strArray = continuationToken.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1 || strArray.Length == 2)
        {
          int result;
          if (!int.TryParse(strArray[0], out result))
            throw new ArgumentOutOfRangeException(nameof (continuationToken));
          continuationToken1.Watermark = result;
          if (strArray.Length == 2)
          {
            if (!int.TryParse(strArray[1], out result))
              throw new ArgumentOutOfRangeException(nameof (continuationToken));
            continuationToken1.Top = result;
          }
        }
      }
      if (continuationToken1.Watermark < 0 || continuationToken1.Top < 0)
        throw new ArgumentOutOfRangeException(nameof (continuationToken));
      return continuationToken1;
    }

    public static string GenerateTestResultsContinuationToken(int testRunId, int testResultId) => string.Format("{0}_{1}", (object) testRunId, (object) testResultId);

    public static void SetContinuationToken(HttpResponseMessage responseMessage, string tokenValue)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (string.IsNullOrWhiteSpace(tokenValue))
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", tokenValue);
    }

    public static string GetContinuationTokenFromWatermarkTopPair(WatermarkTopPair watermarkTopPair) => string.Format("{0};{1}", (object) watermarkTopPair.Watermark, (object) watermarkTopPair.Top);

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
      T[] source1 = (T[]) null;
      int count = 0;
      foreach (T obj in source)
      {
        if (source1 == null)
          source1 = new T[batchSize];
        source1[count++] = obj;
        if (count == batchSize)
        {
          yield return (IEnumerable<T>) source1;
          source1 = (T[]) null;
          count = 0;
        }
      }
      if (source1 != null && count > 0)
        yield return ((IEnumerable<T>) source1).Take<T>(count);
    }

    public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IComparer<TKey> comparer = null)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (keySelector == null)
        throw new ArgumentNullException(nameof (keySelector));
      if (elementSelector == null)
        throw new ArgumentNullException(nameof (elementSelector));
      SortedDictionary<TKey, TElement> sortedDictionary = new SortedDictionary<TKey, TElement>(comparer);
      foreach (TSource source1 in source)
        sortedDictionary[keySelector(source1)] = elementSelector(source1);
      return sortedDictionary;
    }

    public static List<T> NullableUnion<T>(this List<T> s1, List<T> s2)
    {
      if (s1 == null)
        return s2;
      return s2 == null ? s1 : s1.Union<T>((IEnumerable<T>) s2).ToList<T>();
    }

    public static List<T> NullableExcept<T>(this List<T> s1, List<T> s2)
    {
      if (s1 == null)
        return (List<T>) null;
      return s2 == null ? s1 : s1.Except<T>((IEnumerable<T>) s2).ToList<T>();
    }

    public static List<TestOutcome> GetListOfOutcome(string outcomes)
    {
      if (string.IsNullOrWhiteSpace(outcomes))
        return (List<TestOutcome>) null;
      string[] strArray = outcomes.Split(new string[1]
      {
        ","
      }, StringSplitOptions.RemoveEmptyEntries);
      List<TestOutcome> listOfOutcome = new List<TestOutcome>();
      foreach (string str in (IEnumerable<string>) strArray ?? Enumerable.Empty<string>())
      {
        TestOutcome result;
        if (!Enum.TryParse<TestOutcome>(str, true, out result))
          throw new InvalidPropertyException(string.Format(ServerResources.InvalidArgument, (object) "outcome", (object) str));
        listOfOutcome.Add(result);
      }
      return listOfOutcome;
    }
  }
}
