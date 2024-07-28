// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Logging.ConsoleLogEngine
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Globalization;

namespace Microsoft.Cloud.Metrics.Client.Logging
{
  internal sealed class ConsoleLogEngine : ILogEngine, IDisposable
  {
    public void Log(
      LoggerLevel level,
      object logId,
      string tag,
      string format,
      params object[] objectParams)
    {
      if (level == LoggerLevel.CustomerFacingInfo)
      {
        string format1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] {1}", new object[2]
        {
          (object) DateTime.UtcNow.ToString("hh:mm:ss"),
          (object) format
        });
        Console.BackgroundColor = ConsoleColor.Black;
        if (objectParams == null || objectParams.Length == 0)
          Console.WriteLine(format1);
        else
          Console.WriteLine(format1, objectParams);
      }
      else
      {
        string format2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UTC=[{0}] Level=[{1}] LogId=[{2}] Tag=[{3}] {4}", (object) DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"), (object) level, logId, (object) tag, (object) format);
        Console.BackgroundColor = level > LoggerLevel.Warning ? ConsoleColor.Black : ConsoleColor.Blue;
        if (objectParams == null || objectParams.Length == 0)
          Console.WriteLine(format2);
        else
          Console.WriteLine(format2, objectParams);
        Console.BackgroundColor = ConsoleColor.Black;
      }
    }

    public bool IsLogged(LoggerLevel level, object logId, string tag) => true;

    public void Dispose()
    {
    }
  }
}
