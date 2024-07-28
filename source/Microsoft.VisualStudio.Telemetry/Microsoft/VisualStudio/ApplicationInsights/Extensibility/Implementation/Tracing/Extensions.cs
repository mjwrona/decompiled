// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing.Extensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing
{
  public static class Extensions
  {
    public static string ToInvariantString(this Exception exception)
    {
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      try
      {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        return exception.ToString();
      }
      finally
      {
        Thread.CurrentThread.CurrentUICulture = currentUiCulture;
      }
    }
  }
}
