// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.Jobs.WatermarkUtil
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins.Jobs
{
  public static class WatermarkUtil
  {
    public static long ToInt64(string watermark)
    {
      long result = 0;
      if (!string.IsNullOrEmpty(watermark) && !long.TryParse(watermark, out result))
        throw new FormatException("Watermark '" + watermark + "' is not in a valid format.");
      return result;
    }

    public static int ToInt32(string watermark)
    {
      int result = 0;
      if (!string.IsNullOrEmpty(watermark) && !int.TryParse(watermark, out result))
        throw new FormatException("Watermark '" + watermark + "' is not in a valid format.");
      return result;
    }

    public static bool IsDateTime(string watermark) => !string.IsNullOrEmpty(watermark) && DateTime.TryParse(watermark, out DateTime _);

    public static int ToInt32(string watermark, int defaultValue)
    {
      int result;
      if (!int.TryParse(watermark, out result))
        result = defaultValue;
      return result;
    }

    public static string GetTimestampWatermark() => DateTime.UtcNow.ToString("O");
  }
}
