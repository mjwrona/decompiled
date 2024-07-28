// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CommonUtility
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class CommonUtility
  {
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

    internal static IEnumerable<T> LazyEnumerable<T>(
      Func<TableContinuationToken, ResultSegment<T>> segmentGenerator,
      long maxResults)
    {
      ResultSegment<T> currentSeg = segmentGenerator((TableContinuationToken) null);
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

    internal static void ArgumentOutOfRange(string paramName, object value) => throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument is out of range. Value passed: {0}", value));

    internal static void AssertInBounds<T>(string paramName, T val, T min, T max) where T : IComparable
    {
      if (val.CompareTo((object) min) < 0)
        throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is smaller than minimum of '{1}'", (object) paramName, (object) min));
      if (val.CompareTo((object) max) > 0)
        throw new ArgumentOutOfRangeException(paramName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is larger than maximum of '{1}'", (object) paramName, (object) max));
    }

    public static TimeSpan MaxTimeSpan(TimeSpan val1, TimeSpan val2) => !(val1 > val2) ? val2 : val1;

    internal static bool UsePathStyleAddressing(Uri uri)
    {
      CommonUtility.AssertNotNull(nameof (uri), (object) uri);
      return uri.HostNameType != UriHostNameType.Dns || ((IEnumerable<int>) CommonUtility.PathStylePorts).Contains<int>(uri.Port);
    }
  }
}
