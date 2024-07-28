// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryMetricsUtils
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class QueryMetricsUtils
  {
    public static Dictionary<string, double> ParseDelimitedString(string delimitedString)
    {
      if (delimitedString == null)
        throw new ArgumentNullException(nameof (delimitedString));
      Dictionary<string, double> delimitedString1 = new Dictionary<string, double>();
      string str1 = delimitedString;
      char[] chArray1 = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '=' };
        string[] strArray = str2.Split(chArray2);
        string key = strArray.Length == 2 ? strArray[0] : throw new ArgumentException("recieved a malformed delimited string");
        double num = double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
        delimitedString1[key] = num;
      }
      return delimitedString1;
    }

    public static TimeSpan DoubleMillisecondsToTimeSpan(double milliseconds) => TimeSpan.FromTicks((long) (10000.0 * milliseconds));

    public static TimeSpan TimeSpanFromMetrics(Dictionary<string, double> metrics, string key)
    {
      double milliseconds;
      return !metrics.TryGetValue(key, out milliseconds) ? new TimeSpan() : QueryMetricsUtils.DoubleMillisecondsToTimeSpan(milliseconds);
    }
  }
}
