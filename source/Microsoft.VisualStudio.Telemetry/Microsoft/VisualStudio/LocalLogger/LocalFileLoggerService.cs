// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.LocalLogger.LocalFileLoggerService
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.LocalLogger
{
  public static class LocalFileLoggerService
  {
    private static readonly object lockDefaultLoggerObject = new object();

    private static ILocalFileLogger DefaultLogger { get; set; }

    public static ILocalFileLogger Default
    {
      get
      {
        if (LocalFileLoggerService.DefaultLogger == null)
        {
          lock (LocalFileLoggerService.lockDefaultLoggerObject)
          {
            if (LocalFileLoggerService.DefaultLogger == null)
              LocalFileLoggerService.DefaultLogger = !Platform.IsWindows ? (ILocalFileLogger) new NullLocalFileLogger() : (ILocalFileLogger) new LocalFileLogger();
          }
        }
        return LocalFileLoggerService.DefaultLogger;
      }
    }
  }
}
