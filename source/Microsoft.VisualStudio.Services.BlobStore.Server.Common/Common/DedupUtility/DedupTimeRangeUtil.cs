// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.DedupTimeRangeUtil
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class DedupTimeRangeUtil
  {
    private const string format = "yyyy-MM-dd\\THH:mm:ss\\Z";

    public static DateTimeOffset? FromISO8601UTC(string raw)
    {
      if (string.IsNullOrEmpty(raw))
        return new DateTimeOffset?();
      DateTimeOffset result;
      if (DateTimeOffset.TryParseExact(raw, "yyyy-MM-dd\\THH:mm:ss\\Z", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
        return new DateTimeOffset?(result);
      throw new ArgumentException("Invalid date time: " + raw + ". It must be a UTC time in ISO 8601 format with separators, and have second-level accuracy. Example: 2018-06-26T12:40:05Z");
    }

    public static string ToISO8601UTC(DateTime dt) => dt.Kind == DateTimeKind.Utc ? dt.ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss\\Z") : throw new ArgumentException(string.Format("Cannot convert {0} to ISO-8601 UTC format. The given datetime is not in UTC. ", (object) dt) + "It's intentional to disable processing with any local time.");
  }
}
