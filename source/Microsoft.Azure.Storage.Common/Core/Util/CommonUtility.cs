// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.CommonUtility
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class CommonUtility
  {
    private static int seed = Environment.TickCount;
    private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>((Func<Random>) (() => new Random(Interlocked.Increment(ref CommonUtility.seed))));
    private static readonly int[] PathStylePorts = new int[20]
    {
      10000,
      10001,
      10002,
      10003,
      10004,
      10100,
      10101,
      10102,
      10103,
      10104,
      11000,
      11001,
      11002,
      11003,
      11004,
      11100,
      11101,
      11102,
      11103,
      11104
    };

    internal static Random Random => CommonUtility.random.Value;

    internal static CommandLocationMode GetListingLocationMode(IContinuationToken token)
    {
      if (token != null && token.TargetLocation.HasValue)
      {
        switch (token.TargetLocation.Value)
        {
          case StorageLocation.Primary:
            return CommandLocationMode.PrimaryOnly;
          case StorageLocation.Secondary:
            return CommandLocationMode.SecondaryOnly;
          default:
            CommonUtility.ArgumentOutOfRange("TargetLocation", (object) token.TargetLocation.Value);
            break;
        }
      }
      return CommandLocationMode.PrimaryOrSecondary;
    }

    public static TimeSpan MaxTimeSpan(TimeSpan val1, TimeSpan val2) => !(val1 > val2) ? val2 : val1;

    public static string GetFirstHeaderValue<T>(IEnumerable<T> headerValues) where T : class
    {
      if (headerValues != null)
      {
        T obj = headerValues.FirstOrDefault<T>();
        if ((object) obj != null)
          return obj.ToString().TrimStart();
      }
      return (string) null;
    }

    internal static void AssertNotNullOrEmpty(string paramName, string value)
    {
      CommonUtility.AssertNotNull(paramName, (object) value);
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException("The argument must not be empty string.", paramName);
    }

    internal static void AssertNotNull(string paramName, object value)
    {
      if (value == null)
        throw new ArgumentNullException(paramName);
    }

    internal static void ArgumentOutOfRange(string paramName, object value) => throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument is out of range. Value passed: {0}", value));

    internal static void AssertInBounds<T>(string paramName, T val, T min, T max) where T : IComparable
    {
      if (val.CompareTo((object) min) < 0)
        throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is smaller than minimum of '{1}'", (object) paramName, (object) min));
      if (val.CompareTo((object) max) > 0)
        throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is larger than maximum of '{1}'", (object) paramName, (object) max));
    }

    internal static void AssertInBounds<T>(string paramName, T val, T min) where T : IComparable
    {
      if (val.CompareTo((object) min) < 0)
        throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is smaller than minimum of '{1}'", (object) paramName, (object) min));
    }

    internal static void CheckStringParameter(
      string paramName,
      bool canBeNullOrEmpty,
      string value,
      int maxSize)
    {
      if (!canBeNullOrEmpty)
        CommonUtility.AssertNotNullOrEmpty(value, paramName);
      CommonUtility.AssertInBounds<int>(value, paramName.Length, 0, maxSize);
    }

    internal static int RoundUpToSeconds(this TimeSpan timeSpan) => (int) Math.Ceiling(timeSpan.TotalSeconds);

    internal static byte[] BinaryAppend(byte[] arr1, byte[] arr2)
    {
      byte[] destinationArray = new byte[arr1.Length + arr2.Length];
      Array.Copy((Array) arr1, (Array) destinationArray, arr1.Length);
      Array.Copy((Array) arr2, 0, (Array) destinationArray, arr1.Length, arr2.Length);
      return destinationArray;
    }

    internal static bool UsePathStyleAddressing(Uri uri)
    {
      CommonUtility.AssertNotNull(nameof (uri), (object) uri);
      return uri.HostNameType != UriHostNameType.Dns || ((IEnumerable<int>) CommonUtility.PathStylePorts).Contains<int>(uri.Port);
    }

    internal static string ReadElementAsString(string elementName, XmlReader reader)
    {
      string str = (string) null;
      if (!reader.IsStartElement(elementName))
        throw new XmlException(elementName);
      if (reader.IsEmptyElement)
        reader.Skip();
      else
        str = reader.ReadElementContentAsString();
      int content = (int) reader.MoveToContent();
      return str;
    }

    internal static async Task<string> ReadElementAsStringAsync(
      string elementName,
      XmlReader reader)
    {
      string res = (string) null;
      if (!await reader.IsStartElementAsync(elementName).ConfigureAwait(false))
        throw new XmlException(elementName);
      if (reader.IsEmptyElement)
        await reader.SkipAsync().ConfigureAwait(false);
      else
        res = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
      int num = (int) await reader.MoveToContentAsync().ConfigureAwait(false);
      string str = res;
      res = (string) null;
      return str;
    }

    internal static IEnumerable<T> LazyEnumerable<T>(
      Func<IContinuationToken, ResultSegment<T>> segmentGenerator,
      long maxResults)
    {
      ResultSegment<T> currentSeg = segmentGenerator((IContinuationToken) null);
      long count = 0;
      while (true)
      {
        foreach (T result in currentSeg.Results)
        {
          yield return result;
          ++count;
          if (count >= maxResults)
            break;
        }
        if (count < maxResults && currentSeg.ContinuationToken != null)
          currentSeg = segmentGenerator(currentSeg.ContinuationToken);
        else
          break;
      }
    }

    internal static void RunWithoutSynchronizationContext(Action actionToRun)
    {
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) null);
        actionToRun();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }

    internal static T RunWithoutSynchronizationContext<T>(Func<T> actionToRun)
    {
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) null);
        return actionToRun();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }
  }
}
