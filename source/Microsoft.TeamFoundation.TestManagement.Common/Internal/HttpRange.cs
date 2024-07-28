// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.Internal.HttpRange
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class HttpRange
  {
    private const string c_prefix = "bytes=";

    public HttpRange(long start, long end, long total)
    {
      if (start < 0L)
        throw new ArgumentOutOfRangeException(nameof (start));
      if (end < start)
        throw new ArgumentOutOfRangeException(nameof (end));
      if (total < 0L)
        throw new ArgumentOutOfRangeException(nameof (total));
      this.Start = start;
      this.End = end;
      this.Total = total;
    }

    public static bool TryParse(string range, out HttpRange obj)
    {
      obj = new HttpRange(0L, 0L, 0L);
      bool flag = false;
      int num1 = range.IndexOf('-');
      int num2 = range.IndexOf('/');
      long result1;
      long result2;
      long result3;
      if (range.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase) && num1 > -1 && num2 > num1 && long.TryParse(range.Substring("bytes=".Length, num1 - "bytes=".Length), out result1) && long.TryParse(range.Substring(num1 + 1, num2 - num1 - 1), out result2) && long.TryParse(range.Substring(num2 + 1), out result3))
      {
        flag = true;
        obj.Start = result1;
        obj.End = result2;
        obj.Total = result3;
      }
      return flag;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}-{2}/{3}", (object) "bytes=", (object) this.Start, (object) this.End, (object) this.Total);

    public long Start { get; private set; }

    public long End { get; private set; }

    public long Total { get; private set; }
  }
}
